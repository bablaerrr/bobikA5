using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Enums;
using Newtonsoft.Json.Linq;
using SpiderPRO.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using VMProtect;

namespace SpiderPRO;

public class Form1 : Form
{

    private Guna.UI2.WinForms.Guna2CheckBox server2CheckBox;

    private Dropshadow dropShadow;

	private ColorDialog colorDialog;

	public const int WM_NCLBUTTONDOWN = 161;

	public const int HT_CAPTION = 2;

	public static string ToolDir = Directory.GetCurrentDirectory();

	public static string Win64Path = Path.Combine(ToolDir, "win-x64");

	private readonly HttpClient _httpClient = new HttpClient
	{
		Timeout = TimeSpan.FromSeconds(30.0)
	};

	private ProcessMonitor _processMonitor;

	private bool isDeviceCurrentlyConnected;

	private DateTime? deviceDisconnectedAt;

	private System.Timers.Timer deviceCheckTimer;

	private string currentUdid;

	private string currentProductType = "";

	private string currentProductVersion = "";

	private string currentSerialNumber = "";

	private string currentActivationState = "";

	private string currentImei = "";

	private string currentEcid = "";

	private string lastConnectedUdid;

	private string lastDeviceModel = "";

	private string lastDeviceType = "";

	private string lastDeviceSN = "";

	private string lastDeviceVersion = "";

	private string lastDeviceActivation = "";

	private string lastDeviceECID = "";

	private string lastDeviceIMEI = "";

    private string lastDeviceRegionInfo = "";

    private string lastDeviceMEID = "";

    private string lastDeviceUDID = "";

    private string lastDevicebuildVersion = "";

	private static readonly string pythonTargetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SpiderPRO", "python");

	public DeviceData devicedata = new DeviceData();

	private bool isProcessRunning;

	private int totalProgress;

	private IContainer components;

	internal PictureBox pictureBoxModel;

	private Label labelType;

	private Label labelVersion;

	private Label labelSN;

	private Label ModeloffHello;

	private Label label15;

	private Label label16;

	private Label label20;

	private Label label23;

	internal PictureBox pictureBoxDC;

	private Guna2Elipse guna2Elipse1;

	internal Label labelInfoProgres;

	internal Guna2ProgressBar Guna2ProgressBar1;

	internal Guna2GradientButton ActivateButton;

	private Label Status;

	private Label labelActivaction;

	private Label label2;

	private Label labelIMEI;

	private Guna2TextBox LogsBox;
    private Guna2Panel guna2Panel1;
    private Guna2CircleButton guna2CircleButton5;
    private Label label8;
    private Guna2CircleButton guna2CircleButton3;
    private Label labeludidsn;
    private Label labelimeimeid;
    private Label labelptios;
    private Label labeluuid;
    internal Guna2GradientButton guna2GradientButton3;
    private Label labelECID;

	public static Form1 Instance { get; private set; }

	public string DeviceModel => ModeloffHello.Text;

	public string iOSVer => labelVersion.Text;

	public string IMEI => labelIMEI.Text;

	public DeviceData CurrentDeviceData { get; private set; }

	[DllImport("user32.dll")]
	public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

	[DllImport("user32.dll")]
	public static extern bool ReleaseCapture();

	public Form1()
	{

		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
		InitializeComponent();
        this.AutoSize = false;
        this.MaximumSize = Size.Empty;
        this.MinimumSize = Size.Empty;
        this.Size = new Size(714, 386); 
		APP_UUID();
        this.CheckVersion();
		ActivateButton.Hide();
        labelptios.Text = "There is not Device Connected";
        labelimeimeid.Text = "";
        labeludidsn.Text = "";
        SetupDragging();
        InitializeDeviceManagers();
		InitializeFormSettings();
		InitializeProcessMonitor();
		Instance = this;
		base.FormClosing += Form1_FormClosing;
		base.Shown += Form1_Shown;
	}

    private void APP_UUID()
    {
        Guid myuuid = Guid.NewGuid();
        string myuuidAsString = myuuid.ToString().ToUpper();
        labeluuid.Text = ("APP_UUID: " + myuuidAsString);
    }

    private void SetupDragging()
    {
        this.labeluuid.MouseDown += delegate (object s, MouseEventArgs e)
        {
            bool flag = e.Button == MouseButtons.Left;
            if (flag)
            {
                Form1.ReleaseCapture();
                Form1.SendMessage(base.Handle, 161, 2, 0);
            }
        };
        this.labelptios.MouseDown += delegate (object s, MouseEventArgs e)
        {
            bool flag = e.Button == MouseButtons.Left;
            if (flag)
            {
                Form1.ReleaseCapture();
                Form1.SendMessage(base.Handle, 161, 2, 0);
            }
        };
        this.labelimeimeid.MouseDown += delegate (object s, MouseEventArgs e)
        {
            bool flag = e.Button == MouseButtons.Left;
            if (flag)
            {
                Form1.ReleaseCapture();
                Form1.SendMessage(base.Handle, 161, 2, 0);
            }
        };
        this.labeludidsn.MouseDown += delegate (object s, MouseEventArgs e)
        {
            bool flag = e.Button == MouseButtons.Left;
            if (flag)
            {
                Form1.ReleaseCapture();
                Form1.SendMessage(base.Handle, 161, 2, 0);
            }
        };
        this.guna2Panel1.MouseDown += delegate (object s, MouseEventArgs e)
        {
            bool flag = e.Button == MouseButtons.Left;
            if (flag)
            {
                Form1.ReleaseCapture();
                Form1.SendMessage(base.Handle, 161, 2, 0);
            }
        };
        this.label8.MouseDown += delegate (object s, MouseEventArgs e)
        {
            bool flag = e.Button == MouseButtons.Left;
            if (flag)
            {
                Form1.ReleaseCapture();
                Form1.SendMessage(base.Handle, 161, 2, 0);
            }
        };
        this.pictureBoxModel.MouseDown += delegate (object s, MouseEventArgs e)
        {
            bool flag = e.Button == MouseButtons.Left;
            if (flag)
            {
                Form1.ReleaseCapture();
                Form1.SendMessage(base.Handle, 161, 2, 0);
            }
        };
        this.pictureBoxDC.MouseDown += delegate (object s, MouseEventArgs e)
        {
            bool flag = e.Button == MouseButtons.Left;
            if (flag)
            {
                Form1.ReleaseCapture();
                Form1.SendMessage(base.Handle, 161, 2, 0);
            }
        };
    }

    private void CheckVersion()
    {
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string text = "1.0.5";
            string text2 = "http://bobik.atwebpages.com/version.php";
            using (WebClient webClient = new WebClient())
            {
                string text3 = webClient.DownloadString(text2).Trim();
                if (!string.Equals(text, text3, StringComparison.OrdinalIgnoreCase))
                {
                    Form2.Show("BobikA5", "You're using and outdated Version, get the Last Version of BobikA5 " + text3 + " . Please download and continue. Download link is same", MessageBoxIcon.Exclamation);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
        }
        catch (Exception)
        {
            Form2.Show("Bobik A5", "Error while checking Version of the Tools please check your Internet Connection and try again. Important our tools require good internet connection before start. ", MessageBoxIcon.Exclamation);
            Application.Exit();
            Environment.Exit(0);
        }
    }

    private void InitializeProcessMonitor()
	{
		_processMonitor = new ProcessMonitor();
		_processMonitor.ProcessKilled += OnProcessKilled;
		_processMonitor.StartMonitoring();
	}

	public void Form1_Shown(object sender, EventArgs e)
	{
	}

	private void OnProcessKilled(object sender, ProcessKilledEventArgs e)
	{
		AddLog("Security: Process " + e.ProcessName + " was terminated", Color.Orange);
	}

	private void Form1_Load(object sender, EventArgs e)
	{
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
        base.FormBorderStyle = FormBorderStyle.None;
		InitializeFormSettings();
		StartDeviceListener();
        this.pictureBoxModel.SendToBack();
        InitializeDeviceManagers();
		UpdateButtonStates(activateEnabled: true, otaBlockEnabled: false);
	}

	private void InitializeFormSettings()
	{
		base.MouseDown += Form1_MouseDown;
		DoubleBuffered = true;
	}

	private void AddLog(string message, Color? color = null)
	{
		if (LogsBox.InvokeRequired)
		{
			LogsBox.Invoke(new Action<string, Color?>(AddLog), message, color);
			return;
		}
		try
		{
			Color obj = color ?? Color.Black;
			string timestamp = DateTime.Now.ToString("HH:mm:ss");
			string logEntry = "[" + timestamp + "] " + message;
			int originalSelectionStart = LogsBox.SelectionStart;
			int originalSelectionLength = LogsBox.SelectionLength;
			if (!string.IsNullOrEmpty(LogsBox.Text))
			{
				LogsBox.AppendText(Environment.NewLine);
			}
			LogsBox.AppendText(logEntry);
			if (obj != Color.Black)
			{
				int startIndex = LogsBox.Text.Length - logEntry.Length;
				LogsBox.Select(startIndex, logEntry.Length);
				LogsBox.SelectionLength = 0;
			}
			LogsBox.SelectionStart = LogsBox.Text.Length;
			LogsBox.ScrollToCaret();
			if (originalSelectionLength > 0)
			{
				LogsBox.Select(originalSelectionStart, originalSelectionLength);
			}
		}
		catch (Exception)
		{
		}
	}

	public async Task<string> SkipSetup(string arguments)
	{
		string iOSPath = Path.Combine(Directory.GetCurrentDirectory(), "win-x64\\ios.exe");
		Process process = new Process
		{
			StartInfo = 
			{
				FileName = iOSPath,
				Arguments = arguments,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			}
		};
		Console.WriteLine("[DEBUG] Ejecutando ios.exe con argumentos: " + arguments);
		process.Start();
		string output = await process.StandardOutput.ReadToEndAsync();
		string errorOutput = await process.StandardError.ReadToEndAsync();
		process.WaitForExit();
		string obj = output + errorOutput;
		Console.WriteLine("ios.exe Output: " + output);
		Console.WriteLine("ios.exe Error Output: " + errorOutput);
		return obj.Trim();
	}

	private void ClearLogs()
	{
		if (LogsBox.InvokeRequired)
		{
			LogsBox.Invoke(new Action(ClearLogs));
			return;
		}
		LogsBox.Clear();
		AddLog("Logs cleared", Color.Gray);
	}

	private void StartDeviceListener()
	{
		deviceCheckTimer = new System.Timers.Timer(3000.0);
		deviceCheckTimer.Elapsed += async delegate
		{
			await CheckForDevices();
		};
		deviceCheckTimer.Start();
	}

	private async Task CheckForDevices()
	{
		try
		{
			string udid = await GetDeviceUdid();
			if (!string.IsNullOrEmpty(udid))
			{
				if (currentUdid != udid || !isDeviceCurrentlyConnected)
				{
					AddLog("Device connected: " + udid, Color.Green);
					await HandleDeviceConnected(udid);
					return;
				}
				await GetDeviceInfo(udid);
				Invoke((Action)delegate
				{
					UpdateDeviceUI();
				});
			}
			else if (isDeviceCurrentlyConnected)
			{
				AddLog("Device disconnected", Color.Red);
				HandleDeviceDisconnected();
			}
		}
		catch (Exception ex)
		{
			AddLog("Device check error: " + ex.Message, Color.Red);
		}
	}

	private async Task<string> GetDeviceUdid()
	{
		try
		{
			string ideviceIdPath = Path.Combine(Win64Path, "idevice_id.exe");
			if (!File.Exists(ideviceIdPath))
			{
				AddLog("idevice_id.exe not found", Color.Red);
				return null;
			}
			string output = await ExecuteProcessAsync(ideviceIdPath, "-l");
			if (!string.IsNullOrEmpty(output))
			{
				string[] udids = output.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				return (udids.Length != 0) ? udids[0] : null;
			}
		}
		catch (Exception ex)
		{
			AddLog("GetDeviceUdid error: " + ex.Message, Color.Red);
		}
		return null;
	}

	private async Task<bool> GetDeviceInfo(string udid)
	{
		try
		{
			string ideviceInfoPath = Path.Combine(Win64Path, "ideviceinfo.exe");
			if (!File.Exists(ideviceInfoPath))
			{
				AddLog("ideviceinfo.exe not found", Color.Red);
				return false;
			}
			string output = await ExecuteProcessAsync(ideviceInfoPath, "");
			if (output.Contains("invalid HostID") || output.Contains("Could not connect to lockdownd") || output.Contains("Lockdown error"))
			{
				AddLog("cleaning lockdown data...", Color.Orange);
				await ManualPairingCleanup();
				await Task.Delay(1000);
				output = await ExecuteProcessAsync(ideviceInfoPath, "-u " + udid);
			}
			if (string.IsNullOrWhiteSpace(output))
			{
				AddLog("No device info received", Color.Orange);
				return false;
			}
			string[] array = output.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			Dictionary<string, string> deviceInfo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			string[] array2 = array;
			foreach (string line in array2)
			{
				int idx = line.IndexOf(':');
				if (idx > 0 && idx < line.Length - 1)
				{
					string key = line.Substring(0, idx).Trim();
					string value = line.Substring(idx + 1).Trim();
					deviceInfo[key] = value;
				}
			}
			currentProductType = GetDictValue(deviceInfo, "ProductType", "(null)");
			currentProductVersion = GetDictValue(deviceInfo, "ProductVersion", "(null)");
			currentSerialNumber = GetDictValue(deviceInfo, "SerialNumber", "(null)");
			currentActivationState = GetDictValue(deviceInfo, "ActivationState", "(null)");
			currentImei = GetDictValue(deviceInfo, "InternationalMobileEquipmentIdentity", "(null)");
			currentEcid = GetDictValue(deviceInfo, "UniqueChipID", "(null)");
            lastDeviceRegionInfo = GetDictValue(deviceInfo, "RegionInfo", "(null)");
            lastDeviceMEID = GetDictValue(deviceInfo, "MobileEquipmentIdentifier", "(null)");
            lastDeviceUDID = GetDictValue(deviceInfo, "UniqueDeviceID", "(null)");
			lastDevicebuildVersion = GetDictValue(deviceInfo, "BuildVersion", "(null)");
            devicedata.SerialNumber = currentSerialNumber;
			devicedata.Model = currentProductType;
			devicedata.Udid = currentUdid;
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private async Task ManualPairingCleanup()
	{
		try
		{
			string lockdownFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Apple", "Lockdown");
			if (Directory.Exists(lockdownFolder))
			{
				Directory.Delete(lockdownFolder, recursive: true);
				await Task.Delay(500);
			}
		}
		catch (Exception)
		{
		}
	}

	private static string GetDictValue(Dictionary<string, string> dict, string key, string defaultValue)
	{
		if (!dict.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		return value;
	}

	private async Task<string> ExecuteProcessAsync(string fileName, string arguments)
	{
		try
		{
			using Process process = new Process();
			process.StartInfo.FileName = fileName;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			string output = await process.StandardOutput.ReadToEndAsync();
			string value = await process.StandardError.ReadToEndAsync();
			process.WaitForExit();
			string.IsNullOrEmpty(value);
			return output;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private async Task HandleDeviceConnected(string udid)
	{
		currentUdid = udid;
		isDeviceCurrentlyConnected = true;
		deviceDisconnectedAt = null;
		if (!(await GetDeviceInfo(udid)))
		{
			return;
		}
		Invoke((Action)async delegate
		{
			await UpdateDeviceUI();
			if (!isProcessRunning)
			{
				UpdateButtonStates(activateEnabled: true, otaBlockEnabled: false);
				if (labelActivaction.Text.ToLower() == "activated")
				{
					UpdateButtonStates(activateEnabled: false, otaBlockEnabled: true);
				}
			}
		});
	}

	private async Task UpdateDeviceUI()
	{
		try
		{
			pictureBoxDC.SendToBack();
			pictureBoxDC.Visible = false;
			if (currentUdid == lastConnectedUdid)
			{
				ModeloffHello.Text = lastDeviceModel;
				labelType.Text = lastDeviceType;
				labelSN.Text = lastDeviceSN;
				labelVersion.Text = lastDeviceVersion;
				labelActivaction.Text = lastDeviceActivation;
				labelIMEI.Text = lastDeviceIMEI;
				labelptios.Text = "Device: " + lastDeviceType + " iOS: " + lastDeviceVersion + " Build: " + lastDevicebuildVersion;
                labelimeimeid.Text = "IMEI: " + lastDeviceIMEI + " SN: " + lastDeviceSN;
                labeludidsn.Text = "UDID Number: " + lastConnectedUdid;
            }
            else
			{
				await LoadImageWithZoomAsync(2f);
				UpdateDeviceModel();
				UpdateDeviceInfo();
				if (!string.IsNullOrEmpty(currentUdid))
				{
					lastConnectedUdid = currentUdid;
				}
			}
			await ShowElementsAsync();
		}
		catch (Exception ex)
		{
			AddLog("Error updating UI: " + ex.Message, Color.Red);
		}
	}

    private void UpdateDeviceInfo()
    {
        try
        {
            labelVersion.Text = currentProductVersion ?? "(null)";
            labelSN.Text = currentSerialNumber ?? "(null)";
            labelType.Text = currentProductType ?? "(null)";
            labelActivaction.Text = currentActivationState ?? "(null)";
            labelIMEI.Text = currentImei ?? "(null)";
            labelECID.Text = currentEcid;
            lastDeviceVersion = labelVersion.Text;
            lastDeviceSN = labelSN.Text;
            lastDeviceType = labelType.Text;
            lastDeviceActivation = labelActivaction.Text;
            lastDeviceIMEI = labelIMEI.Text;
            lastDeviceECID = currentEcid ?? "";
            labelptios.Text = "Device: " + lastDeviceType + " iOS: " + lastDeviceVersion + " Build: " + lastDeviceRegionInfo;
            labelimeimeid.Text = "IMEI: " + lastDeviceIMEI + " SN: " + lastDeviceSN;
            labeludidsn.Text = "UDID Number: " + lastConnectedUdid;

            // ВКЛЮЧАЕМ КНОПКУ ТУТ:
            guna2GradientButton3.Enabled = true;

            Refresh();
            AddLog("Model: " + lastDeviceModel + ", iOS: " + lastDeviceVersion + ", Activation: " + lastDeviceActivation, Color.DarkGreen);
        }
        catch (Exception ex)
        {
            labelVersion.Text = "Error";
            labelSN.Text = "Error";
            labelType.Text = "Error";
            labelActivaction.Text = "Error";
            labelIMEI.Text = "Error";

            // На случай ошибки можно оставить выключенной или тоже включить
            // guna2GradientButton3.Enabled = false; 

            AddLog("Error updating device info: " + ex.Message, Color.Red);
        }
    }

    private void UpdateButtonStates(bool activateEnabled, bool otaBlockEnabled)
	{
		if (base.InvokeRequired)
		{
			Invoke(new Action<bool, bool>(UpdateButtonStates), activateEnabled, otaBlockEnabled);
		}
		else
		{
            guna2GradientButton3.Enabled = activateEnabled && isDeviceCurrentlyConnected;
            ActivateButton.Enabled = activateEnabled && isDeviceCurrentlyConnected;
			ActivateButton.ForeColor = (ActivateButton.Enabled ? Color.White : Color.White);
		}
	}

	private void HandleDeviceDisconnected()
	{
		if (!string.IsNullOrEmpty(currentUdid))
		{
			lastConnectedUdid = currentUdid;
			lastDeviceModel = ModeloffHello.Text;
			lastDeviceType = labelType.Text;
			lastDeviceSN = labelSN.Text;
			lastDeviceVersion = labelVersion.Text;
			lastDeviceActivation = labelActivaction.Text;
			lastDeviceECID = currentEcid ?? "";
			lastDeviceIMEI = labelIMEI.Text;
		}
		currentUdid = null;
		isDeviceCurrentlyConnected = false;
		deviceDisconnectedAt = DateTime.Now;
		Invoke((Action)delegate
		{
            labelptios.Text = "There is not Device Connected";
            labelimeimeid.Text = "";
            labeludidsn.Text = "";
			pictureBoxDC.BringToFront();
			pictureBoxDC.Visible = true;
			UpdateButtonStates(activateEnabled: false, otaBlockEnabled: false);
			ClearDeviceLabels();
		});
	}

	private void ClearDeviceLabels()
	{
	}

	private void UpdateDeviceModel()
	{
		if (string.IsNullOrEmpty(currentProductType))
		{
			ModeloffHello.Text = "Unknown Device";
			return;
		}
		if (new Dictionary<string, string>
		{
			{ "iPhone1,1", "iPhone 2G" },
			{ "iPhone1,2", "iPhone 3G" },
			{ "iPhone2,1", "iPhone 3GS" },
			{ "iPhone3,1", "iPhone 4 (GSM)" },
			{ "iPhone3,2", "iPhone 4 (GSM) R2 2012" },
			{ "iPhone3,3", "iPhone 4 (CDMA)" },
			{ "iPhone4,1", "iPhone 4s" },
			{ "iPhone5,1", "iPhone 5 (GSM)" },
			{ "iPhone5,2", "iPhone 5 (Global)" },
			{ "iPhone5,3", "iPhone 5c (GSM)" },
			{ "iPhone5,4", "iPhone 5c (Global)" },
			{ "iPhone6,1", "iPhone 5s (GSM)" },
			{ "iPhone6,2", "iPhone 5s (Global)" },
			{ "iPhone7,1", "iPhone 6 Plus" },
			{ "iPhone7,2", "iPhone 6" },
			{ "iPhone8,1", "iPhone 6s" },
			{ "iPhone8,2", "iPhone 6s Plus" },
			{ "iPhone8,4", "iPhone SE (1st gen)" },
			{ "iPhone9,1", "iPhone 7 (Global)" },
			{ "iPhone9,2", "iPhone 7 Plus (Global)" },
			{ "iPhone9,3", "iPhone 7 (GSM)" },
			{ "iPhone9,4", "iPhone 7 Plus (GSM)" },
			{ "iPhone10,1", "iPhone 8 (Global)" },
			{ "iPhone10,2", "iPhone 8 Plus (Global)" },
			{ "iPhone10,3", "iPhone X (Global)" },
			{ "iPhone10,4", "iPhone 8 (GSM)" },
			{ "iPhone10,5", "iPhone 8 Plus (GSM)" },
			{ "iPhone10,6", "iPhone X (GSM)" },
			{ "iPhone11,2", "iPhone XS" },
			{ "iPhone11,4", "iPhone XS Max (China)" },
			{ "iPhone11,6", "iPhone XS Max" },
			{ "iPhone11,8", "iPhone XR" },
			{ "iPhone12,1", "iPhone 11" },
			{ "iPhone12,3", "iPhone 11 Pro" },
			{ "iPhone12,5", "iPhone 11 Pro Max" },
			{ "iPhone12,8", "iPhone SE (2nd gen)" },
			{ "iPhone13,1", "iPhone 12 mini" },
			{ "iPhone13,2", "iPhone 12" },
			{ "iPhone13,3", "iPhone 12 Pro" },
			{ "iPhone13,4", "iPhone 12 Pro Max" },
			{ "iPhone14,2", "iPhone 13 Pro" },
			{ "iPhone14,3", "iPhone 13 Pro Max" },
			{ "iPhone14,4", "iPhone 13 mini" },
			{ "iPhone14,5", "iPhone 13" },
			{ "iPhone14,6", "iPhone SE (3rd gen)" },
			{ "iPhone14,7", "iPhone 14" },
			{ "iPhone14,8", "iPhone 14 Plus" },
			{ "iPhone15,2", "iPhone 14 Pro" },
			{ "iPhone15,3", "iPhone 14 Pro Max" },
			{ "iPhone15,4", "iPhone 15" },
			{ "iPhone15,5", "iPhone 15 Plus" },
			{ "iPhone16,1", "iPhone 15 Pro" },
			{ "iPhone16,2", "iPhone 15 Pro Max" },
			{ "iPhone17,1", "iPhone 16 Pro" },
			{ "iPhone17,2", "iPhone 16 Pro Max" },
			{ "iPhone17,3", "iPhone 16" },
			{ "iPhone17,4", "iPhone 16 Plus" },
			{ "iPhone17,5", "iPhone 16e" },
			{ "iPhone18,1", "iPhone 17 Pro" },
			{ "iPhone18,2", "iPhone 17 Pro Max" },
			{ "iPhone18,3", "iPhone 17" },
			{ "iPhone18,4", "iPhone Air" },
			{ "iPad1,1", "iPad" },
			{ "iPad2,1", "iPad 2 (WiFi)" },
			{ "iPad2,2", "iPad 2 (GSM)" },
			{ "iPad2,3", "iPad 2 (CDMA)" },
			{ "iPad2,4", "iPad 2 (WiFi) R2 2012" },
			{ "iPad2,5", "iPad mini (WiFi)" },
			{ "iPad2,6", "iPad mini (GSM)" },
			{ "iPad2,7", "iPad mini (Global)" },
			{ "iPad3,1", "iPad (3rd gen, WiFi)" },
			{ "iPad3,2", "iPad (3rd gen, CDMA)" },
			{ "iPad3,3", "iPad (3rd gen, GSM)" },
			{ "iPad3,4", "iPad (4th gen, WiFi)" },
			{ "iPad3,5", "iPad (4th gen, GSM)" },
			{ "iPad3,6", "iPad (4th gen, Global)" },
			{ "iPad4,1", "iPad Air (WiFi)" },
			{ "iPad4,2", "iPad Air (Cellular)" },
			{ "iPad4,3", "iPad Air (China)" },
			{ "iPad4,4", "iPad mini 2 (WiFi)" },
			{ "iPad4,5", "iPad mini 2 (Cellular)" },
			{ "iPad4,6", "iPad mini 2 (China)" },
			{ "iPad4,7", "iPad mini 3 (WiFi)" },
			{ "iPad4,8", "iPad mini 3 (Cellular)" },
			{ "iPad4,9", "iPad mini 3 (China)" },
			{ "iPad5,1", "iPad mini 4 (WiFi)" },
			{ "iPad5,2", "iPad mini 4 (Cellular)" },
			{ "iPad5,3", "iPad Air 2 (WiFi)" },
			{ "iPad5,4", "iPad Air 2 (Cellular)" },
			{ "iPad6,3", "iPad Pro 9.7-inch (WiFi)" },
			{ "iPad6,4", "iPad Pro 9.7-inch (Cellular)" },
			{ "iPad6,7", "iPad Pro 12.9-inch (1st gen, WiFi)" },
			{ "iPad6,8", "iPad Pro 12.9-inch (1st gen, Cellular)" },
			{ "iPad6,11", "iPad (5th gen, WiFi)" },
			{ "iPad6,12", "iPad (5th gen, Cellular)" },
			{ "iPad7,1", "iPad Pro 12.9-inch (2nd gen, WiFi)" },
			{ "iPad7,2", "iPad Pro 12.9-inch (2nd gen, Cellular)" },
			{ "iPad7,3", "iPad Pro 10.5-inch (WiFi)" },
			{ "iPad7,4", "iPad Pro 10.5-inch (Cellular)" },
			{ "iPad7,5", "iPad (6th gen, WiFi)" },
			{ "iPad7,6", "iPad (6th gen, Cellular)" },
			{ "iPad7,11", "iPad (7th gen, WiFi)" },
			{ "iPad7,12", "iPad (7th gen, Cellular)" },
			{ "iPad8,1", "iPad Pro 11-inch (1st gen, WiFi)" },
			{ "iPad8,2", "iPad Pro 11-inch (1st gen, WiFi, 1TB)" },
			{ "iPad8,3", "iPad Pro 11-inch (1st gen, Cellular)" },
			{ "iPad8,4", "iPad Pro 11-inch (1st gen, Cellular, 1TB)" },
			{ "iPad8,5", "iPad Pro 12.9-inch (3rd gen, WiFi)" },
			{ "iPad8,6", "iPad Pro 12.9-inch (3rd gen, WiFi, 1TB)" },
			{ "iPad8,7", "iPad Pro 12.9-inch (3rd gen, Cellular)" },
			{ "iPad8,8", "iPad Pro 12.9-inch (3rd gen, Cellular, 1TB)" },
			{ "iPad8,9", "iPad Pro 11-inch (2nd gen, WiFi)" },
			{ "iPad8,10", "iPad Pro 11-inch (2nd gen, Cellular)" },
			{ "iPad8,11", "iPad Pro 12.9-inch (4th gen, WiFi)" },
			{ "iPad8,12", "iPad Pro 12.9-inch (4th gen, Cellular)" },
			{ "iPad11,1", "iPad mini (5th gen, WiFi)" },
			{ "iPad11,2", "iPad mini (5th gen, Cellular)" },
			{ "iPad11,3", "iPad Air (3rd gen, WiFi)" },
			{ "iPad11,4", "iPad Air (3rd gen, Cellular)" },
			{ "iPad11,6", "iPad (8th gen, WiFi)" },
			{ "iPad11,7", "iPad (8th gen, Cellular)" },
			{ "iPad12,1", "iPad (9th gen, WiFi)" },
			{ "iPad12,2", "iPad (9th gen, Cellular)" },
			{ "iPad13,1", "iPad Air (4th gen, WiFi)" },
			{ "iPad13,2", "iPad Air (4th gen, Cellular)" },
			{ "iPad13,4", "iPad Pro 11-inch (3rd gen, WiFi)" },
			{ "iPad13,5", "iPad Pro 11-inch (3rd gen, WiFi, 2TB)" },
			{ "iPad13,6", "iPad Pro 11-inch (3rd gen, Cellular)" },
			{ "iPad13,7", "iPad Pro 11-inch (3rd gen, Cellular, 2TB)" },
			{ "iPad13,8", "iPad Pro 12.9-inch (5th gen, WiFi)" },
			{ "iPad13,9", "iPad Pro 12.9-inch (5th gen, WiFi, 2TB)" },
			{ "iPad13,10", "iPad Pro 12.9-inch (5th gen, Cellular)" },
			{ "iPad13,11", "iPad Pro 12.9-inch (5th gen, Cellular, 2TB)" },
			{ "iPad13,16", "iPad Air (5th gen, WiFi)" },
			{ "iPad13,17", "iPad Air (5th gen, Cellular)" },
			{ "iPad13,18", "iPad (10th gen, WiFi)" },
			{ "iPad13,19", "iPad (10th gen, Cellular)" },
			{ "iPad14,1", "iPad mini (6th gen, WiFi)" },
			{ "iPad14,2", "iPad mini (6th gen, Cellular)" },
			{ "iPad14,3", "iPad Pro 11-inch (4th gen, WiFi)" },
			{ "iPad14,4", "iPad Pro 11-inch (4th gen, Cellular)" },
			{ "iPad14,5", "iPad Pro 12.9-inch (6th gen, WiFi)" },
			{ "iPad14,6", "iPad Pro 12.9-inch (6th gen, Cellular)" },
			{ "iPad14,8", "iPad Air 11-inch (M2, WiFi)" },
			{ "iPad14,9", "iPad Air 11-inch (M2, Cellular)" },
			{ "iPad14,10", "iPad Air 13-inch (M2, WiFi)" },
			{ "iPad14,11", "iPad Air 13-inch (M2, Cellular)" },
			{ "iPad15,3", "iPad Air 11-inch (M3, WiFi)" },
			{ "iPad15,4", "iPad Air 11-inch (M3, Cellular)" },
			{ "iPad15,5", "iPad Air 13-inch (M3, WiFi)" },
			{ "iPad15,6", "iPad Air 13-inch (M3, Cellular)" },
			{ "iPad15,7", "iPad (A16, WiFi)" },
			{ "iPad15,8", "iPad (A16, Cellular)" },
			{ "iPad16,1", "iPad mini (A17 Pro, WiFi)" },
			{ "iPad16,2", "iPad mini (A17 Pro, Cellular)" },
			{ "iPad16,3", "iPad Pro 11-inch (M4, WiFi)" },
			{ "iPad16,4", "iPad Pro 11-inch (M4, Cellular)" },
			{ "iPad16,5", "iPad Pro 13-inch (M4, WiFi)" },
			{ "iPad16,6", "iPad Pro 13-inch (M4, Cellular)" },
			{ "iPad17,1", "iPad Pro 11-inch (M5, WiFi)" },
			{ "iPad17,2", "iPad Pro 11-inch (M5, Cellular)" },
			{ "iPad17,3", "iPad Pro 13-inch (M5, WiFi)" },
			{ "iPad17,4", "iPad Pro 13-inch (M5, Cellular)" },
            { "iPod5,1", "iPod Touch 5 Generation" }
        }.TryGetValue(currentProductType, out var modelName))
		{
			ModeloffHello.Text = modelName;
		}
		else
		{
			ModeloffHello.Text = "Unknown Model";
		}
		lastDeviceModel = ModeloffHello.Text;
	}

	private void Form1_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			ReleaseCapture();
			SendMessage(base.Handle, 161, 2, 0);
		}
	}

	private void Form1_FormClosing(object sender, FormClosingEventArgs e)
	{
		AddLog("Application closing...", Color.Gray);
		deviceCheckTimer?.Stop();
		deviceCheckTimer?.Dispose();
		CloseExitAPP("ideviceinfo");
		CloseExitAPP("idevice_id");
		CloseExitAPP("idevicebackup");
		CloseExitAPP("idevicebackup2");
		CloseExitAPP("python");
		CloseExitAPP("SEC");
		CloseExitAPP("pymobiledevice3");
		AddLog("Application closed", Color.Gray);
	}

	public static string SeparateNumber(int number)
	{
		return ((double)number / 10.0).ToString("0.0");
	}

	private void UpdatelabelInfoProgres(string text)
	{
		if (labelInfoProgres.InvokeRequired)
		{
			labelInfoProgres.Invoke(new Action<string>(UpdatelabelInfoProgres), text);
		}
		else
		{
			labelInfoProgres.Text = text;
		}
		AddLog(" " + text, Color.Blue);
	}

	private void UpdateProgressLabel(string message)
	{
		if (labelInfoProgres.InvokeRequired)
		{
			labelInfoProgres.Invoke(new Action<string>(UpdateProgressLabel), message);
		}
		else
		{
			labelInfoProgres.Text = message;
			AddLog(message ?? "", Color.Blue);
		}
	}

	private void UpdateGuna2ProgressBar1Control(int value)
	{
		if (Guna2ProgressBar1.InvokeRequired)
		{
			Guna2ProgressBar1.Invoke(new Action<int>(UpdateGuna2ProgressBar1Control), value);
		}
		else
		{
			Guna2ProgressBar1.Value = Math.Max(0, Math.Min(100, value));
		}
	}

	private void InsertLabelText(string text, Color color, string additionalText = "")
	{
		if (labelInfoProgres.InvokeRequired)
		{
			Invoke(new Action<string, Color, string>(InsertLabelText), text, color, additionalText);
		}
		else
		{
			labelInfoProgres.ForeColor = color;
			labelInfoProgres.Text = (string.IsNullOrEmpty(additionalText) ? text : (text + additionalText));
			AddLog("Status: " + text + " " + additionalText, color);
		}
	}

	private async Task ShowElementsAsync()
	{
		await Task.Run(delegate
		{
			if (ModeloffHello.InvokeRequired)
			{
				ModeloffHello.Invoke((MethodInvoker)delegate
				{
					ModeloffHello.Visible = true;
				});
			}
			else
			{
				ModeloffHello.Visible = true;
			}
		});
	}

    private async Task LoadImageWithZoomAsync(float zoomFactor)
    {
        if (string.IsNullOrEmpty(currentProductType)) return;

        // 1. Форматируем название: iPhone7,2 -> iPhone7.2
        string formattedProductType = currentProductType.Replace(",", ".");

        // 2. Логика папок (iPad, iPod, iPhone)
        string typeIMG = "iPhone";
        if (formattedProductType.Contains("iPad")) typeIMG = "iPad";
        else if (formattedProductType.Contains("iPod")) typeIMG = "iPod";

       
        string imageUrl = $"https://github.com/bablaerrr/iphoneimages/blob/main/{typeIMG}/{formattedProductType}/device.png?raw=true";

        try
        {
            AddLog($"Загрузка: {imageUrl}", Color.Blue);

            using (HttpClient httpClient = new HttpClient())
            {
                // Маскировка под браузер (обязательно для бесплатных хостингов)
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                // Получаем данные
                byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                using (MemoryStream stream = new MemoryStream(imageBytes))
                {
                    Image downloadedImage = Image.FromStream(stream);

                    Invoke((Action)delegate
                    {
                        pictureBoxModel.Image?.Dispose(); // Очистка памяти
                        pictureBoxModel.SizeMode = PictureBoxSizeMode.Zoom;
                        pictureBoxModel.Image = new Bitmap(downloadedImage);
                    });
                }
            }
            AddLog("Изображение загружено", Color.Green);
        }
        catch (Exception ex)
        {
            // Если будет ошибка 404, значит путь к файлу на сервере не совпадает
            AddLog($"Ошибка загрузки: {ex.Message}", Color.Orange);
        }
    }

    private static async Task<string> ShellCMDAsync(string command, string arguments)
	{
		using Process process = new Process();
		ProcessStartInfo processStartInfo = new ProcessStartInfo(command)
		{
			RedirectStandardOutput = true,
			Arguments = arguments,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};
		process.StartInfo = processStartInfo;
		process.Start();
		string output = await process.StandardOutput.ReadToEndAsync();
		string error = await process.StandardError.ReadToEndAsync();
		process.WaitForExit();
		return string.IsNullOrEmpty(error) ? output : error;
	}

	private void DisableButtons()
	{
		ActivateButton.Enabled = false;
	}

    public async Task<bool> IsCheckm8SupportedAsync()
    {
        // Берем данные из существующих переменных
        string model = lastDeviceType;
        string iosVersion = lastDeviceVersion; // Эта переменная содержит версию (например, 9.3.5)

        using (HttpClient client = new HttpClient())
        {
            // Добавляем параметр &ios= в строку запроса
            string url = $"http://bobik.atwebpages.com/A5.php?model={Uri.EscapeDataString(model)}&ios={Uri.EscapeDataString(iosVersion)}";

            try
            {
                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);

                // Теперь сервер вернет true только если и модель, и iOS подходят
                return json["supported"].Value<bool>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Checking Device: " + ex.Message);
                return false;
            }
        }
    }

    public async Task Checker()
    {
        UpdateUIProgress(30, "", "Checking compatibillity your device, please wait...");
        await Task.Delay(20000);
        UpdateUIProgress(60, "", "Contacting the server for compatibillity, please wait...");

        bool supported = await IsCheckm8SupportedAsync();
        UpdateUIProgress(80, "", "Checking compatibillity success, please wait for result...");

        if (!supported)
        {
            UpdateUIProgress(0, "", "Sorry your device is currently not supported!");
            Form2.Show("BobikA5", "Your device is currently not supported for A5 Activation! We will notify you via Telegram channel when your device is supported for A5 Activation.", MessageBoxIcon.Exclamation);
            return;
        }

        guna2GradientButton3.Hide();
        ActivateButton.Show();
        ActivateButton.Enabled = true;

        UpdateUIProgress(100, "", "Congratulations your device is supported!");
        Form2.Show("BobikA5", "Congratulations your device is supported for A5 Activation. MAKE SURE YOU CONECTED TO WIFI ON DEVICE. Click the button 'Activate Your Device' to activate your device");
    }

    private async void ActivateButton_Click(object sender, EventArgs e)
	{
		isProcessRunning = true;
		DisableButtons();
		ActivateButton.Text = "Processing...";
		try
		{
			if (!isDeviceCurrentlyConnected || string.IsNullOrEmpty(currentUdid))
			{
                labelInfoProgres.Text = "There is not Device Connected";
                Guna2ProgressBar1.Value = 0;
                Form2.Show("BobikA5", "There is not device connected, please connect your device using USB Cable. If problem still same ensure driver is fine and 3uTools and iTunes must installed.", MessageBoxIcon.Exclamation);
                return;
			}
            await AfcPutAsync(lastDeviceUDID);
		}
		catch (Exception ex)
		{
			AddLog("Activation error: " + ex.Message, Color.Red);
			CleanUP();
			ShowError("An error occurred: " + ex.Message, "Error");
		}
		finally
		{
            isProcessRunning = false;
            ActivateButton.Enabled = true;
            ActivateButton.Text = "Activate Your Device";
            CleanUP();
			await GetDeviceInfo(currentUdid);
		}
	}

    private async Task AfcPutAsync(string udid)
    {
        try
        {
            UpdateUIProgress(20, "", "Preparing your device info, please wait...");
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string toolPath = Path.Combine(baseDir, "win-x64", "afcclient.exe");
            string spl28Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "fuck");
            string otaPath = Path.Combine(baseDir, "win-x64", server2CheckBox.Checked ? "A52" : "A5");
            File.WriteAllBytes(spl28Path, File.ReadAllBytes(otaPath));
            string cmd = "\"" + toolPath + "\" put --udid " + udid + " \"" + spl28Path + "\" /Downloads/downloads.28.sqlitedb";
            string ouputAfc = await RunCommandAsyncReturn(cmd);
            if (File.Exists(spl28Path))
            {
                File.Delete(spl28Path);
            }
            if (ouputAfc.Contains("ERROR") || ouputAfc.Contains("error"))
            {
                UpdateUIProgress(0, "", "AFC File not found, please re-install the software!");
            }
            else
            {
                await IdeviceRestartTwiceAsync();
            }
        }
        catch (Exception ex)
        {
            Exception ex2 = ex;
            UpdateUIProgress(0, "", "[AFC]" + ex.Message);
            await SendReport("FAILED BYPASSED A5 ❌ " + ex.Message);
        }
    }


    private async Task IdeviceRestartTwiceAsync()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string toolPath = Path.Combine(baseDir, "win-x64", "idevicediagnostics.exe");
        string cmd = "\"" + toolPath + "\" restart";
        if ((await RunCommandAsyncReturn(cmd)).Contains("Restarting device"))
        {
            UpdateUIProgress(60, "", "[1] Restarting your device, please wait...");
            await CountdownAsync(90);
            if ((await RunCommandAsyncReturn(cmd)).Contains("Restarting device"))
            {
                UpdateUIProgress(60, "", "[2] Restarting your device, please wait...");
                await CountdownAsync(90);
                UpdateUIProgress(80, "", "Checking activation status, please wait...");
                await IdeviceMobileGestaltAsyncH("hactivation");
            }
            else
            {
                UpdateUIProgress(0, "", "Trust Device or check connection USB Cable!");
            }
        }
        else
        {
            UpdateUIProgress(0, "", "Trust Device or check connection USB Cable!");
        }
    }

    public async Task CountdownAsync(int seconds)
    {
        for (int i = seconds; i >= 0; i--)
        {
            string Text = $"Please wait for reboot after: {i} seconds";
            UpdateUIProgress(60, "", $"Please wait for reboot after: {i} seconds");
            await Task.Delay(1000);
        }
        UpdateUIProgress(70, "", "Restarting your device is Completed now...");
    }

    private async Task IdeviceMobileGestaltAsyncH(string key)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string toolPath = Path.Combine(baseDir, "win-x64", "idevicediagnostics.exe");
        string cmd = "\"" + toolPath + "\" mobilegestalt KEY \"" + key + "\"";
        string output = await RunCommandAsyncReturn(cmd);
        if (output.Contains("MobileGestalt") && output.Contains("Success"))
        {
            UpdateUIProgress(80, "", "Activating your device, please wait..");
            Thread.Sleep(5000);
            await IdeviceMobileGestaltAsync("ShouldHactivate");
            return;
        }
        UpdateUIProgress(80, "", "[M] Failed to activate your device, please retry the process..");
        await SendReport("[Mobilegestalt] FAILED BYPASSED A5 ❌");
    }

    private async Task IdeviceMobileGestaltAsync(string key)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string toolPath = Path.Combine(baseDir, "win-x64", "idevicediagnostics.exe");
        string cmd = "\"" + toolPath + "\" mobilegestalt KEY \"" + key + "\"";
        if ((await RunCommandAsyncReturn(cmd)).Contains("true"))
        {
            Thread.Sleep(5000);
            UpdateUIProgress(100, "", "Rebooting your device, please wait...");
            await SendReport("SUCCESSFULLY BYPASSED A5 ✅");
            Form2.Show("BobikA5", "Your Device was Successfully Activated and it's rebooting now. Please complete activation as normal.");
            try
            {
                string baseDir2 = AppDomain.CurrentDomain.BaseDirectory;
                string toolPath2 = Path.Combine(baseDir2, "win-x64", "idevicediagnostics.exe");
                string cmd2 = "\"" + toolPath2 + "\" restart";
                await RunCommandAsyncReturn(cmd2);
                return;
            }
            catch
            {
                return;
            }
        }
        UpdateUIProgress(0, "", "[Gestalt] Failed activate, please connect to wifi on device and try again or use different ios version");
        await SendReport("[L] FAILED BYPASSED A5 ❌");
    }

    private async Task<string> RunCommandAsyncReturn(string command)
    {
        Console.WriteLine($"[START] {DateTime.Now:HH:mm:ss} → {command}");
        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c \"" + command + "\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        string output = "";
        string error = "";
        Process process = new Process();
        try
        {
            process.StartInfo = processInfo;
            process.Start();
            output = await process.StandardOutput.ReadToEndAsync();
            error = await process.StandardError.ReadToEndAsync();
            await Task.Run(delegate
            {
                process.WaitForExit();
            });
        }
        finally
        {
            if (process != null)
            {
                ((IDisposable)process).Dispose();
            }
        }
        Console.WriteLine("Output:\n" + output);
        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine("Error:\n" + error);
            output = output + "\nERROR:\n" + error;
        }
        Console.WriteLine($"[END]   {DateTime.Now:HH:mm:ss} → {command}");
        Console.WriteLine("=====================================");
        return output;
    }

    private void CleanUP()
	{
		try
		{
			if (Directory.Exists(pythonTargetPath))
			{
				Directory.Delete(pythonTargetPath, recursive: true);
			}
			CloseExitAPP("SEC");
			CloseExitAPP("pymobiledevice3");
		}
		catch
		{
		}
	}

    private async Task SendReport(string status)
    {
        using (HttpClient client = new HttpClient())
        {
            var data = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("status", status),
            new KeyValuePair<string, string>("model", lastDeviceModel),
            new KeyValuePair<string, string>("serial", lastDeviceSN),
            new KeyValuePair<string, string>("imei", lastDeviceIMEI),
            new KeyValuePair<string, string>("type", lastDeviceType),
            new KeyValuePair<string, string>("ios", lastDeviceVersion),
            new KeyValuePair<string, string>("region", lastDeviceRegionInfo),
            new KeyValuePair<string, string>("build", lastDevicebuildVersion)
        });

            await client.PostAsync("http://bobik.atwebpages.com/telegram_report.php", data);
        }
    }

	private async Task<bool> CheckActivationViaDeviceProperty(int maxRetries = 5, int delayMs = 6000)
	{
		AddLog($"Checking activation status (max {maxRetries} attempts)...", Color.Blue);
		for (int attempt = 1; attempt <= maxRetries; attempt++)
		{
			try
			{
				AddLog($"Activation check attempt {attempt}/{maxRetries}", Color.Gray);
				await GetDeviceInfo(await GetDeviceUdid());
				if (!string.IsNullOrEmpty(currentActivationState))
				{
					if (currentActivationState.Equals("Activated", StringComparison.OrdinalIgnoreCase))
					{
						AddLog("Device is activated!", Color.Green);
						return true;
					}
					if (currentActivationState.Equals("Unactivated", StringComparison.OrdinalIgnoreCase))
					{
						AddLog("Device is still unactivated", Color.Orange);
						return false;
					}
				}
				await Task.Delay(delayMs);
			}
			catch (Exception ex)
			{
				AddLog("Activation check error: " + ex.Message, Color.Orange);
				await Task.Delay(delayMs);
			}
		}
		AddLog("Activation check timeout reached", Color.Orange);
		return false;
	}

	private void ShowError(string message, string title)
	{
		AddLog("Error: " + title + " - " + message, Color.Red);
		labelInfoProgres.Text = "❌ " + title;
		Guna2ProgressBar1.Value = 0;
		MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
	}

	private void UpdateUIProgress(int progressValue, string progressText, string statusText)
	{
		Invoke((Action)delegate
		{
			Guna2ProgressBar1.Value = progressValue;
			if (progressText != null)
			{
				labelInfoProgres.Text = progressText;
			}
			if (statusText != null)
			{
				labelInfoProgres.Text = statusText;
			}
		});
		AddLog($"Progress: {progressValue}% - {statusText}", Color.Blue);
	}

	private async Task<bool> DownloadFileWithProgressAsync(string url, string localPath)
	{
		AddLog("Downloading Activation File...", Color.Blue);
		for (int retryCount = 0; retryCount < 3; retryCount++)
		{
			try
			{
				HttpResponseMessage response = await _httpClient.GetAsync(url, (HttpCompletionOption)1);
				try
				{
					response.EnsureSuccessStatusCode();
					using Stream stream = await response.Content.ReadAsStreamAsync();
					using FileStream fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write);
					await stream.CopyToAsync(fileStream);
				}
				finally
				{
					((IDisposable)response)?.Dispose();
				}
				AddLog("File downloaded successfully", Color.Green);
				return true;
			}
			catch (Exception ex)
			{
				AddLog($"Download attempt {retryCount + 1} failed: {ex.Message}", Color.Orange);
				try
				{
					if (File.Exists(localPath))
					{
						File.Delete(localPath);
					}
				}
				catch
				{
				}
				if (retryCount >= 2)
				{
					return false;
				}
				await Task.Delay(1000 * (retryCount + 1));
			}
		}
		AddLog("All download attempts failed", Color.Red);
		return false;
	}

	private void ClearTxtLog()
	{
		labelInfoProgres.Text = string.Empty;
		AddLog("Progress log cleared", Color.Gray);
	}

	private async Task ProgressTask(int targetValue)
	{
		AddLog($"Progress task started: {targetValue}%", Color.Gray);
		int finalTarget = Math.Min(targetValue, 100);
		if (totalProgress >= finalTarget)
		{
			return;
		}
		while (totalProgress < finalTarget)
		{
			totalProgress++;
			if (Guna2ProgressBar1.InvokeRequired)
			{
				Guna2ProgressBar1.Invoke((Action)delegate
				{
					UpdateProgressUI(totalProgress);
				});
			}
			else
			{
				UpdateProgressUI(totalProgress);
			}
			await Task.Delay(15);
		}
		AddLog($"Progress task completed: {targetValue}%", Color.Gray);
	}

	private void UpdateProgressUI(int value)
	{
		Guna2ProgressBar1.Value = value;
	}

	private void CloseExitAPP(string processName)
	{
		AddLog("Closing process: " + processName, Color.Gray);
		Process[] processesByName = Process.GetProcessesByName(processName);
		foreach (Process process in processesByName)
		{
			try
			{
				process.Kill();
			}
			catch (Exception)
			{
			}
		}
	}

	private void guna2GradientButton2_Click(object sender, EventArgs e)
	{
	}

	private void guna2GradientButton3_Click(object sender, EventArgs e)
	{
	}

	private void guna2CircleButton1_Click(object sender, EventArgs e)
	{
		CloseApplication();
	}

	private void guna2CircleButton2_Click(object sender, EventArgs e)
	{
		base.WindowState = FormWindowState.Minimized;
	}

	private void CloseApplication()
	{
		AddLog("Close application requested", Color.Blue);
		try
		{
			deviceCheckTimer?.Stop();
			deviceCheckTimer?.Dispose();
			CloseExitAPP("ideviceinfo");
			CloseExitAPP("idevice_id");
			CloseExitAPP("idevicebackup");
			CloseExitAPP("idevicebackup2");
			CloseExitAPP("pymobiledevice3");
			Application.Exit();
		}
		catch (Exception ex)
		{
			AddLog("Close application error: " + ex.Message, Color.Red);
			Environment.Exit(0);
		}
	}

	private void InitializeDeviceManagers()
	{
		CurrentDeviceData = new DeviceData();

	}

	private void PictureBox1_Click(object sender, EventArgs e)
	{
		Clipboard.SetText(labelSN.Text);
		AddLog("Serial number copied to clipboard: " + labelSN.Text, Color.Green);
		MessageBox.Show("Serial number '" + labelSN.Text + "' copied to clipboard.", "Serial Copied", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	}

	private void LogsBox_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right)
		{
			return;
		}
		ContextMenu contextMenu = new ContextMenu();
		MenuItem copyItem = new MenuItem("Copy");
		MenuItem clearItem = new MenuItem("Clear Logs");
		copyItem.Click += delegate
		{
			if (!string.IsNullOrEmpty(LogsBox.SelectedText))
			{
				Clipboard.SetText(LogsBox.SelectedText);
				AddLog("Selected text copied to clipboard", Color.Green);
			}
		};
		clearItem.Click += delegate
		{
			ClearLogs();
		};
		contextMenu.MenuItems.Add(copyItem);
		contextMenu.MenuItems.Add(clearItem);
		contextMenu.Show(LogsBox, new Point(e.X, e.Y));
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.labelType = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelSN = new System.Windows.Forms.Label();
            this.ModeloffHello = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.Status = new System.Windows.Forms.Label();
            this.labelActivaction = new System.Windows.Forms.Label();
            this.pictureBoxModel = new System.Windows.Forms.PictureBox();
            this.pictureBoxDC = new System.Windows.Forms.PictureBox();
            this.labelInfoProgres = new System.Windows.Forms.Label();
            this.Guna2ProgressBar1 = new Guna.UI2.WinForms.Guna2ProgressBar();
            this.ActivateButton = new Guna.UI2.WinForms.Guna2GradientButton();
            this.label2 = new System.Windows.Forms.Label();
            this.labelIMEI = new System.Windows.Forms.Label();
            this.LogsBox = new Guna.UI2.WinForms.Guna2TextBox();
            this.labelECID = new System.Windows.Forms.Label();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2CircleButton5 = new Guna.UI2.WinForms.Guna2CircleButton();
            this.label8 = new System.Windows.Forms.Label();
            this.guna2CircleButton3 = new Guna.UI2.WinForms.Guna2CircleButton();
            this.labeludidsn = new System.Windows.Forms.Label();
            this.labelimeimeid = new System.Windows.Forms.Label();
            this.labelptios = new System.Windows.Forms.Label();
            this.labeluuid = new System.Windows.Forms.Label();
            this.guna2GradientButton3 = new Guna.UI2.WinForms.Guna2GradientButton();
            this.server2CheckBox = new Guna.UI2.WinForms.Guna2CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxModel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDC)).BeginInit();
            this.guna2Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.BackColor = System.Drawing.Color.Transparent;
            this.labelType.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(147)))));
            this.labelType.Location = new System.Drawing.Point(1105, 76);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(29, 15);
            this.labelType.TabIndex = 754;
            this.labelType.Text = "N/A";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(147)))));
            this.labelVersion.Location = new System.Drawing.Point(1104, 138);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(29, 15);
            this.labelVersion.TabIndex = 753;
            this.labelVersion.Text = "N/A";
            // 
            // labelSN
            // 
            this.labelSN.AutoSize = true;
            this.labelSN.BackColor = System.Drawing.Color.Transparent;
            this.labelSN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelSN.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(159)))), ((int)(((byte)(10)))));
            this.labelSN.Location = new System.Drawing.Point(1104, 108);
            this.labelSN.Name = "labelSN";
            this.labelSN.Size = new System.Drawing.Size(29, 15);
            this.labelSN.TabIndex = 751;
            this.labelSN.Text = "N/A";
            // 
            // ModeloffHello
            // 
            this.ModeloffHello.AutoSize = true;
            this.ModeloffHello.BackColor = System.Drawing.Color.Transparent;
            this.ModeloffHello.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.ModeloffHello.ForeColor = System.Drawing.Color.White;
            this.ModeloffHello.Location = new System.Drawing.Point(1104, 45);
            this.ModeloffHello.Name = "ModeloffHello";
            this.ModeloffHello.Size = new System.Drawing.Size(29, 15);
            this.ModeloffHello.TabIndex = 749;
            this.ModeloffHello.Text = "N/A";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Transparent;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label15.ForeColor = System.Drawing.Color.White;
            this.label15.Location = new System.Drawing.Point(1030, 138);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(26, 15);
            this.label15.TabIndex = 748;
            this.label15.Text = "iOS";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.Transparent;
            this.label16.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label16.ForeColor = System.Drawing.Color.White;
            this.label16.Location = new System.Drawing.Point(1030, 76);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(80, 15);
            this.label16.TabIndex = 747;
            this.label16.Text = "ProductType ";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label20.ForeColor = System.Drawing.Color.White;
            this.label20.Location = new System.Drawing.Point(1030, 108);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(38, 15);
            this.label20.TabIndex = 744;
            this.label20.Text = "Serial";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.Color.Transparent;
            this.label23.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label23.ForeColor = System.Drawing.Color.White;
            this.label23.Location = new System.Drawing.Point(1030, 45);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(42, 15);
            this.label23.TabIndex = 743;
            this.label23.Text = "Model";
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 22;
            this.guna2Elipse1.TargetControl = this;
            // 
            // Status
            // 
            this.Status.AutoSize = true;
            this.Status.BackColor = System.Drawing.Color.Transparent;
            this.Status.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Status.ForeColor = System.Drawing.Color.White;
            this.Status.Location = new System.Drawing.Point(1030, 200);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(42, 15);
            this.Status.TabIndex = 817;
            this.Status.Text = "Status";
            // 
            // labelActivaction
            // 
            this.labelActivaction.AutoSize = true;
            this.labelActivaction.BackColor = System.Drawing.Color.Transparent;
            this.labelActivaction.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelActivaction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.labelActivaction.Location = new System.Drawing.Point(1104, 200);
            this.labelActivaction.Name = "labelActivaction";
            this.labelActivaction.Size = new System.Drawing.Size(29, 15);
            this.labelActivaction.TabIndex = 818;
            this.labelActivaction.Text = "N/A";
            // 
            // pictureBoxModel
            // 
            this.pictureBoxModel.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxModel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxModel.Location = new System.Drawing.Point(-42, 45);
            this.pictureBoxModel.Name = "pictureBoxModel";
            this.pictureBoxModel.Size = new System.Drawing.Size(456, 338);
            this.pictureBoxModel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxModel.TabIndex = 674;
            this.pictureBoxModel.TabStop = false;
            this.pictureBoxModel.Click += new System.EventHandler(this.pictureBoxModel_Click);
            // 
            // pictureBoxDC
            // 
            this.pictureBoxDC.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxDC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxDC.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxDC.Image")));
            this.pictureBoxDC.Location = new System.Drawing.Point(-5, 45);
            this.pictureBoxDC.Name = "pictureBoxDC";
            this.pictureBoxDC.Size = new System.Drawing.Size(284, 338);
            this.pictureBoxDC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxDC.TabIndex = 777;
            this.pictureBoxDC.TabStop = false;
            // 
            // labelInfoProgres
            // 
            this.labelInfoProgres.BackColor = System.Drawing.Color.Transparent;
            this.labelInfoProgres.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelInfoProgres.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(147)))));
            this.labelInfoProgres.Location = new System.Drawing.Point(302, 285);
            this.labelInfoProgres.Name = "labelInfoProgres";
            this.labelInfoProgres.Size = new System.Drawing.Size(355, 18);
            this.labelInfoProgres.TabIndex = 811;
            this.labelInfoProgres.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Guna2ProgressBar1
            // 
            this.Guna2ProgressBar1.BackColor = System.Drawing.Color.Transparent;
            this.Guna2ProgressBar1.BorderColor = System.Drawing.Color.Transparent;
            this.Guna2ProgressBar1.BorderRadius = 4;
            this.Guna2ProgressBar1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(42)))));
            this.Guna2ProgressBar1.ForeColor = System.Drawing.Color.Transparent;
            this.Guna2ProgressBar1.Location = new System.Drawing.Point(331, 331);
            this.Guna2ProgressBar1.Minimum = 10;
            this.Guna2ProgressBar1.Name = "Guna2ProgressBar1";
            this.Guna2ProgressBar1.ProgressBrushMode = Guna.UI2.WinForms.Enums.BrushMode.Solid;
            this.Guna2ProgressBar1.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.Guna2ProgressBar1.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.Guna2ProgressBar1.Size = new System.Drawing.Size(372, 8);
            this.Guna2ProgressBar1.TabIndex = 816;
            this.Guna2ProgressBar1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.Guna2ProgressBar1.Value = 100;
            // 
            // ActivateButton
            // 
            this.ActivateButton.Animated = true;
            this.ActivateButton.BackColor = System.Drawing.Color.Transparent;
            this.ActivateButton.BorderColor = System.Drawing.Color.Transparent;
            this.ActivateButton.BorderRadius = 10;
            this.ActivateButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ActivateButton.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.ActivateButton.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.ActivateButton.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.ActivateButton.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.ActivateButton.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.ActivateButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.ActivateButton.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(210)))));
            this.ActivateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.ActivateButton.ForeColor = System.Drawing.Color.White;
            this.ActivateButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            this.ActivateButton.IndicateFocus = true;
            this.ActivateButton.Location = new System.Drawing.Point(331, 347);
            this.ActivateButton.Name = "ActivateButton";
            this.ActivateButton.Size = new System.Drawing.Size(371, 32);
            this.ActivateButton.TabIndex = 817;
            this.ActivateButton.Text = "Activate Your Device";
            this.ActivateButton.UseTransparentBackground = true;
            this.ActivateButton.Click += new System.EventHandler(this.ActivateButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(1030, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 821;
            this.label2.Text = "IMEI";
            // 
            // labelIMEI
            // 
            this.labelIMEI.AutoSize = true;
            this.labelIMEI.BackColor = System.Drawing.Color.Transparent;
            this.labelIMEI.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelIMEI.ForeColor = System.Drawing.Color.White;
            this.labelIMEI.Location = new System.Drawing.Point(1105, 170);
            this.labelIMEI.Name = "labelIMEI";
            this.labelIMEI.Size = new System.Drawing.Size(29, 15);
            this.labelIMEI.TabIndex = 825;
            this.labelIMEI.Text = "N/A";
            // 
            // LogsBox
            // 
            this.LogsBox.BackColor = System.Drawing.Color.Transparent;
            this.LogsBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(52)))));
            this.LogsBox.BorderRadius = 12;
            this.LogsBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.LogsBox.DefaultText = "";
            this.LogsBox.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(20)))));
            this.LogsBox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.LogsBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.LogsBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(209)))), ((int)(((byte)(88)))));
            this.LogsBox.Location = new System.Drawing.Point(1183, 20);
            this.LogsBox.Multiline = true;
            this.LogsBox.Name = "LogsBox";
            this.LogsBox.PlaceholderText = "";
            this.LogsBox.ReadOnly = true;
            this.LogsBox.SelectedText = "";
            this.LogsBox.Size = new System.Drawing.Size(8, 8);
            this.LogsBox.TabIndex = 827;
            // 
            // labelECID
            // 
            this.labelECID.AutoSize = true;
            this.labelECID.BackColor = System.Drawing.Color.Transparent;
            this.labelECID.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelECID.ForeColor = System.Drawing.Color.White;
            this.labelECID.Location = new System.Drawing.Point(1030, 231);
            this.labelECID.Name = "labelECID";
            this.labelECID.Size = new System.Drawing.Size(29, 15);
            this.labelECID.TabIndex = 829;
            this.labelECID.Text = "N/A";
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(32)))));
            this.guna2Panel1.Controls.Add(this.guna2CircleButton5);
            this.guna2Panel1.Controls.Add(this.label8);
            this.guna2Panel1.Controls.Add(this.guna2CircleButton3);
            this.guna2Panel1.Location = new System.Drawing.Point(-5, 1);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(722, 38);
            this.guna2Panel1.TabIndex = 836;
            // 
            // guna2CircleButton5
            // 
            this.guna2CircleButton5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.guna2CircleButton5.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(194)))), ((int)(((byte)(11)))));
            this.guna2CircleButton5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2CircleButton5.ForeColor = System.Drawing.Color.White;
            this.guna2CircleButton5.Location = new System.Drawing.Point(671, 11);
            this.guna2CircleButton5.Name = "guna2CircleButton5";
            this.guna2CircleButton5.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CircleButton5.Size = new System.Drawing.Size(13, 13);
            this.guna2CircleButton5.TabIndex = 834;
            this.guna2CircleButton5.Click += new System.EventHandler(this.guna2CircleButton5_Click);
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(141, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(490, 19);
            this.label8.TabIndex = 721;
            this.label8.Text = "BobikA5 v1.0.5 beta, made with love by Pkkf5673 and other";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // guna2CircleButton3
            // 
            this.guna2CircleButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.guna2CircleButton3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(0)))), ((int)(((byte)(59)))));
            this.guna2CircleButton3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2CircleButton3.ForeColor = System.Drawing.Color.White;
            this.guna2CircleButton3.Location = new System.Drawing.Point(692, 11);
            this.guna2CircleButton3.Name = "guna2CircleButton3";
            this.guna2CircleButton3.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CircleButton3.Size = new System.Drawing.Size(13, 13);
            this.guna2CircleButton3.TabIndex = 6;
            this.guna2CircleButton3.Click += new System.EventHandler(this.guna2CircleButton3_Click);
            // 
            // labeludidsn
            // 
            this.labeludidsn.BackColor = System.Drawing.Color.Transparent;
            this.labeludidsn.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.labeludidsn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(147)))));
            this.labeludidsn.Location = new System.Drawing.Point(302, 241);
            this.labeludidsn.Name = "labeludidsn";
            this.labeludidsn.Size = new System.Drawing.Size(456, 19);
            this.labeludidsn.TabIndex = 854;
            this.labeludidsn.Text = "UDID - SN";
            this.labeludidsn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labeludidsn.Click += new System.EventHandler(this.labeludidsn_Click);
            // 
            // labelimeimeid
            // 
            this.labelimeimeid.BackColor = System.Drawing.Color.Transparent;
            this.labelimeimeid.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.labelimeimeid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(147)))));
            this.labelimeimeid.Location = new System.Drawing.Point(324, 210);
            this.labelimeimeid.Name = "labelimeimeid";
            this.labelimeimeid.Size = new System.Drawing.Size(452, 19);
            this.labelimeimeid.TabIndex = 853;
            this.labelimeimeid.Text = "IMEI - MEID";
            this.labelimeimeid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelptios
            // 
            this.labelptios.BackColor = System.Drawing.Color.Transparent;
            this.labelptios.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.labelptios.ForeColor = System.Drawing.Color.White;
            this.labelptios.Location = new System.Drawing.Point(328, 170);
            this.labelptios.Name = "labelptios";
            this.labelptios.Size = new System.Drawing.Size(448, 19);
            this.labelptios.TabIndex = 855;
            this.labelptios.Text = "There is not Device Connected";
            this.labelptios.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labeluuid
            // 
            this.labeluuid.BackColor = System.Drawing.Color.Transparent;
            this.labeluuid.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labeluuid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(74)))));
            this.labeluuid.Location = new System.Drawing.Point(301, 90);
            this.labeluuid.Name = "labeluuid";
            this.labeluuid.Size = new System.Drawing.Size(452, 19);
            this.labeluuid.TabIndex = 856;
            this.labeluuid.Text = "APP UUID: ";
            this.labeluuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // guna2GradientButton3
            // 
            this.guna2GradientButton3.Animated = true;
            this.guna2GradientButton3.BackColor = System.Drawing.Color.Transparent;
            this.guna2GradientButton3.BorderRadius = 10;
            this.guna2GradientButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.guna2GradientButton3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(46)))));
            this.guna2GradientButton3.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(30)))));
            this.guna2GradientButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.guna2GradientButton3.ForeColor = System.Drawing.Color.White;
            this.guna2GradientButton3.IndicateFocus = true;
            this.guna2GradientButton3.Location = new System.Drawing.Point(331, 347);
            this.guna2GradientButton3.Name = "guna2GradientButton3";
            this.guna2GradientButton3.Size = new System.Drawing.Size(371, 32);
            this.guna2GradientButton3.TabIndex = 858;
            this.guna2GradientButton3.Text = "Check Device Compatibility";
            this.guna2GradientButton3.UseTransparentBackground = true;
            this.guna2GradientButton3.Click += new System.EventHandler(this.guna2GradientButton3_Click_1);
            // 
            // server2CheckBox
            // 
            this.server2CheckBox.AutoSize = true;
            this.server2CheckBox.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.server2CheckBox.CheckedState.BorderRadius = 2;
            this.server2CheckBox.CheckedState.BorderThickness = 0;
            this.server2CheckBox.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.server2CheckBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.server2CheckBox.ForeColor = System.Drawing.Color.White;
            this.server2CheckBox.Location = new System.Drawing.Point(12, 360);
            this.server2CheckBox.Name = "server2CheckBox";
            this.server2CheckBox.Size = new System.Drawing.Size(74, 19);
            this.server2CheckBox.TabIndex = 859;
            this.server2CheckBox.Text = "Server 2";
            this.server2CheckBox.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.server2CheckBox.UncheckedState.BorderRadius = 2;
            this.server2CheckBox.UncheckedState.BorderThickness = 0;
            this.server2CheckBox.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(46)))));
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(717, 405);
            this.Controls.Add(this.server2CheckBox);
            this.Controls.Add(this.guna2GradientButton3);
            this.Controls.Add(this.ActivateButton);
            this.Controls.Add(this.Guna2ProgressBar1);
            this.Controls.Add(this.labeludidsn);
            this.Controls.Add(this.labelimeimeid);
            this.Controls.Add(this.labelptios);
            this.Controls.Add(this.labeluuid);
            this.Controls.Add(this.guna2Panel1);
            this.Controls.Add(this.pictureBoxModel);
            this.Controls.Add(this.labelECID);
            this.Controls.Add(this.LogsBox);
            this.Controls.Add(this.labelIMEI);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelActivaction);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.labelSN);
            this.Controls.Add(this.ModeloffHello);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.labelInfoProgres);
            this.Controls.Add(this.pictureBoxDC);
            this.Controls.Add(this.label23);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BobikA5";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxModel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDC)).EndInit();
            this.guna2Panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    private void guna2CircleButton3_Click(object sender, EventArgs e)
    {
		CloseApplication();
    }

    private void guna2CircleButton5_Click(object sender, EventArgs e)
    {
        base.WindowState = FormWindowState.Minimized;

    }

    private async void guna2GradientButton3_Click_1(object sender, EventArgs e)
    {
        guna2GradientButton3.Enabled = true;
        await Checker();
    }

    private void pictureBox2_Click(object sender, EventArgs e)
    {

    }

    private void pictureBox1_Click_1(object sender, EventArgs e)
    {

    }

    private void labeludidsn_Click(object sender, EventArgs e)
    {

    }

    private void pictureBoxModel_Click(object sender, EventArgs e)
    {

    }

    private void label8_Click(object sender, EventArgs e)
    {

    }
}
