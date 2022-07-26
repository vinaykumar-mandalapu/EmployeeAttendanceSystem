﻿using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Timer = System.Windows.Forms.Timer;
using EmployeeAttendanceSystem.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EAS_QRScanner
{
    public partial class EmployeeAttendance : Form
    {
		private QRDecoder QRCodeDecoder;
		private Bitmap QRCodeInputImage;
		private bool VideoCameraExists;
		private FrameSize FrameSize;
		private Camera VideoCamera;
		private IMoniker CameraMoniker;
		private Timer QRCodeTimer;
		private Panel CameraPanel;
		private static string employeeID;

		public EmployeeAttendance()
        {
            InitializeComponent();
			return;
        }
        private void OnLoad(object sender, EventArgs e)
		{
			Text = "Employee Attendance System";
			string CurDir = Environment.CurrentDirectory;
			int Index = CurDir.IndexOf("bin\\Debug");
			if (Index > 0)
			{
				string WorkDir = string.Concat(CurDir.AsSpan(0, Index), "Work");
				if (Directory.Exists(WorkDir)) Environment.CurrentDirectory = WorkDir;
			}
			QRCodeTrace.Open("QRCodeDecoderTrace.txt");
			QRCodeTrace.Write("QRCodeDecoder");
			QRCodeDecoder = new QRDecoder();
			VideoCameraExists = TestForVideoCamera();
			QRCodeTimer = new Timer
			{
				Interval = 200
			};
			QRCodeTimer.Tick += QRCodeTimer_Tick;
			scanCard();
			OnResize(sender, e);
			return;
		}
		private void OnLoadImage(object sender, EventArgs e)
		{
			if (!videoPanel.Visible)
			{
				DisableCameraMode();
				videoPanel.Invalidate();
			}
			OpenFileDialog Dialog = new()
			{
				Filter = "Image Files(*.png;*.jpg;*.gif;*.tif)|*.png;*.jpg;*.gif;*.tif;*.bmp)|All files (*.*)|*.*",
				Title = "Load QR Code Image",
				InitialDirectory = Directory.GetCurrentDirectory(),
				RestoreDirectory = true,
				FileName = string.Empty
			};

			if (Dialog.ShowDialog() != DialogResult.OK) return;
			if (QRCodeInputImage != null) QRCodeInputImage.Dispose();
			QRCodeInputImage = new(Dialog.FileName);
			QRCodeResult[] QRCodeResultArray = QRCodeDecoder.ImageDecoder(QRCodeInputImage);
			QRCodeTrace.Format("****");
			QRCodeTrace.Format("Decode image: {0} ", Dialog.FileName);
			QRCodeTrace.Format("Image width: {0}, Height: {1}", QRCodeInputImage.Width, QRCodeInputImage.Height);
			if (QRCodeResultArray != null)
			{
				DisplayResult(QRCodeResultArray);
			}
			videoPanel.Invalidate();
			return;
		}
		private void DisplayResult(QRCodeResult[] ResultArray)
		{
			employeeID = ConvertResultToDisplayString(ResultArray);
			return;
		}
		private void scanCard()
		{
			try
			{
				if (videoPanel.Visible)
				{
					if (QRCodeInputImage != null)
					{
						QRCodeInputImage.Dispose();
						QRCodeInputImage = null;
					}
					videoPanel.Visible = false;
					if (VideoCamera == null)
					{
						CameraPanel = new Panel();
						Controls.Add(CameraPanel);
						CameraPanel.Name = "CameraPanel";
						CameraPanel.TabIndex = 20;
						VideoCamera = new Camera(CameraPanel, CameraMoniker, FrameSize);
					}
					else
					{
						CameraPanel.Visible = true;
						VideoCamera.RunGraph();
					}
				}
				else
				{
					VideoCamera.RunGraph();
				}
				QRCodeTimer.Enabled = true;
			}
			catch (Exception Ex)
			{
				MessageBox.Show("Video camera problem\r\n" + Ex.Message);
				DisableCameraMode();
			}
			return;
		}
		private void QRCodeTimer_Tick(object sender, EventArgs e)
		{
			QRCodeTimer.Enabled = false;
			Bitmap QRCodeImage;
			try
			{
				QRCodeImage = VideoCamera.SnapshotSourceImage();
				QRCodeTrace.Format("Image width: {0}, Height: {1}", QRCodeImage.Width, QRCodeImage.Height);
			}
			catch (Exception Ex)
			{
				MessageBox.Show("Video camera problem\r\n" + Ex.Message);
				DisableCameraMode();
				return;
			}
			QRCodeResult[] DataByteArray = QRCodeDecoder.ImageDecoder(QRCodeImage);
			if (DataByteArray == null)
			{
				QRCodeTimer.Enabled = true;
				return;
			}
			DisplayResult(DataByteArray);
			VideoCamera.PauseGraph();
			AddAttendance();
			return;
		}

		private void DisableCameraMode()
		{
			if (QRCodeInputImage != null)
			{
				QRCodeInputImage.Dispose();
				QRCodeInputImage = null;
			}
			QRCodeTimer.Enabled = false;
			try
			{
				VideoCamera.PauseGraph();
			}
			catch
			{
				VideoCameraExists = false;
			}
			CameraPanel.Visible = false;
			videoPanel.Visible = true;
			return;
		}

		private bool TestForVideoCamera()
		{
			DsDevice[] CameraDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
			if (CameraDevices == null || CameraDevices.Length == 0) return false;
			DsDevice CameraDevice = CameraDevices[1];
			CameraMoniker = CameraDevice.Moniker;
			FrameSize[] FrameSizes = Camera.GetFrameSizeList(CameraMoniker);
			if (FrameSizes == null || FrameSizes.Length == 0)
			{
				CameraMoniker = null;
				return false;
			}
			int Index;
			for (Index = 0; Index < FrameSizes.Length &&
				(FrameSizes[Index].Width != 640 || FrameSizes[Index].Height != 480); Index++) ;
			if (Index < FrameSizes.Length)
			{
				FrameSize = new FrameSize(640, 480);
			}
			else
			{
				FrameSize = FrameSizes[0];
			}
			return true;
		}

		private static string ConvertResultToDisplayString(QRCodeResult[] DataByteArray)
		{
			if (DataByteArray == null) return string.Empty;
			if (DataByteArray.Length == 1) return SingleQRCodeResult(QRDecoder.ByteArrayToStr(DataByteArray[0].DataArray));
			StringBuilder Str = new();
			for (int Index = 0; Index < DataByteArray.Length; Index++)
			{
				if (Index != 0) Str.Append("\r\n");
				Str.AppendFormat("QR Code {0}\r\n", Index + 1);
				Str.Append(SingleQRCodeResult(QRDecoder.ByteArrayToStr(DataByteArray[Index].DataArray)));
			}
			return Str.ToString();
		}
		private static string SingleQRCodeResult(string Result)
		{
			int Index;
			for (Index = 0; Index < Result.Length && (Result[Index] >= ' ' && Result[Index] <= '~' || Result[Index] >= 160); Index++) ;
			if (Index == Result.Length) return Result;
			StringBuilder Display = new(Result[..Index]);
			for (; Index < Result.Length; Index++)
			{
				char OneChar = Result[Index];
				if (OneChar >= ' ' && OneChar <= '~' || OneChar >= 160)
				{
					Display.Append(OneChar);
					continue;
				}
				if (OneChar == '\r')
				{
					Display.Append("\r\n");
					if (Index + 1 < Result.Length && Result[Index + 1] == '\n') Index++;
					continue;
				}
				if (OneChar == '\n')
				{
					Display.Append("\r\n");
					continue;
				}
				Display.Append('¿');
			}
			return Display.ToString();
		}
		private void AddAttendance()
		{
            Attendance attendance = new()
            {
                EmployeeID = Convert.ToInt32(employeeID),
                Status = Status.P,
                Date = DateTime.Now
            };
			Employee employee = new();
			int RowsAffected = 0;
			bool attendanceExists = false;
			string addAttendanceSQL = "INSERT INTO dbo.Attendance VALUES(@EmployeeID, @Date, @Status)";
			using (SqlConnection cnn = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=termproject1group1;Trusted_Connection=True;MultipleActiveResultSets=true"))
			{
				cnn.Open();
				using (SqlCommand CommandObject = new SqlCommand("SELECT LastName, FirstName FROM dbo.Employee WHERE EmployeeID = " + employeeID, cnn))
				{
					using (SqlDataReader dr = CommandObject.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (dr.Read())
						{
							employee.FirstName = dr.GetString(dr.GetOrdinal("FirstName"));
							employee.LastName = dr.GetString(dr.GetOrdinal("LastName"));
						}
					}
				}
				cnn.Open();
				using (SqlCommand cmdObj = new SqlCommand("SELECT COUNT(*) AS CheckAttendance FROM Attendance WHERE EmployeeID = " + employeeID + " AND CAST(Date AS DATE) = CAST(GETDATE() AS DATE)", cnn))
				{
					using (SqlDataReader dr = cmdObj.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (dr.Read())
						{
							attendanceExists = dr.GetInt32(dr.GetOrdinal("CheckAttendance")) > 0 ? true : false;
						}
					}
				}
                if (!attendanceExists)
                {
					using (SqlCommand cmd = new SqlCommand(addAttendanceSQL, cnn))
					{
						cmd.Parameters.Add(new SqlParameter("@EmployeeID", Convert.ToInt32(employeeID)));
						cmd.Parameters.Add(new SqlParameter("@Date", DateTime.Now));
						cmd.Parameters.Add(new SqlParameter("@Status", Status.P));
						cmd.CommandType = CommandType.Text;
						cnn.Open();
						RowsAffected = cmd.ExecuteNonQuery();
						if (RowsAffected == 1)
						{
							MessageBox.Show("Attendance added for " + employee.FullName);
						}
					}
				}
                else
                {
					MessageBox.Show("Attendance already added for " + employee.FullName);
				}
			}
			DisableCameraMode();
			scanCard();
			return;
		}

		private void OnvideoPanelPaint(object sender, PaintEventArgs e)
		{
			if (QRCodeInputImage == null) return;

			int ImageHeight = (videoPanel.Width * QRCodeInputImage.Height) / QRCodeInputImage.Width;
			int ImageWidth;
			if (ImageHeight <= videoPanel.Height)
				ImageWidth = videoPanel.Width;
			else
			{
				ImageWidth = (videoPanel.Height * QRCodeInputImage.Width) / QRCodeInputImage.Height;
				ImageHeight = videoPanel.Height;
			}
			int ImageX = (videoPanel.Width - ImageWidth) / 2;
			int ImageY = (videoPanel.Height - ImageHeight) / 2;
			e.Graphics.DrawImage(QRCodeInputImage, new Rectangle(ImageX, ImageY, ImageWidth, ImageHeight));
			return;
		}
		private void OnResize(object sender, EventArgs e)
		{
			if (ClientSize.Width == 0) return;
			if (CameraPanel != null)
			{
				CameraPanel.Location = new Point(videoPanel.Left, videoPanel.Top);
				CameraPanel.Size = new Size(videoPanel.Width, videoPanel.Height);
			}
			if (QRCodeInputImage != null) videoPanel.Invalidate();
			return;
		}
		private void OnClosing(object sender, FormClosingEventArgs e)
		{
			if (QRCodeInputImage != null) QRCodeInputImage.Dispose();
			if (VideoCamera != null) VideoCamera.Dispose();
			return;
		}
	}
}
