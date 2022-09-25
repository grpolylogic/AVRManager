using System.Diagnostics;
using System.Reflection;
using AVRManagerLibrary;

namespace AVRManagerWindows
{
    public partial class MainForm : Form
    {
        private STK500CompatibleDevice _deviceManager;
        private AVRDevice _device;
        private bool _serialCOMListNotEmpty = false;
        private bool _serialOpen = false;
        private bool _gotDeviceInfo = false;
        private bool _flashHasBeenRead = false;
        private ProgramStates _programState = ProgramStates.NoCOMPortFound;

        public MainForm()
        {
            InitializeComponent();

            _deviceManager = new STK500CompatibleDevice();
            _device = new AVRDevice();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = Text + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            _deviceManager.SerialOpened += HandleSerialOpened;
            _deviceManager.SerialClosed += HandleSerialClosed;
            _deviceManager.HasGotDeviceInfo += HandleGotDeviceInfo;
            _deviceManager.HasReadWholeFlash += HandleReadWholeFlash;
            _deviceManager.ProgressUpdate += HandleProgressUpdate;

            FlashReadFilePathLabel.Text = _device.WholeFlashBinImageReadFromDeviceFilePath;
            ApplicationReadFilePathLabel.Text = _device.ApplicationBinImageReadFromDeviceFilePath;
            BootloaderReadFilePathLabel.Text = _device.BootloaderBinImageReadFromDeviceFilePath;
            ReadFlashFilenameToolTip.SetToolTip(FlashReadFilePathLabel, _device.WholeFlashBinImageReadFromDeviceFilePath);

            serialPortTimer.Start();

            ProcessProgramState();
            //UpdateGUIForSerial();
        }

        private void PopulateCOMComboBox()
        {
            COMPortComboBox.Items.Clear();

            string[] entries = _deviceManager.GetPortNames();
            if (entries != null && entries.Length != 0) {
                foreach (var e in entries)
                {
                    COMPortComboBox.Items.Add(e);
                }

                COMPortComboBox.SelectedIndex = 0;
                _serialCOMListNotEmpty = true;
            } else
            {
                _serialCOMListNotEmpty = false;
            }
        }

        private void InvalidateButtons()
        {
            COMPortComboBox.Enabled = false;
            COMPortListRefreshButton.Enabled = false;
            COMPortConnectButton.Enabled = false;
            GetDeviceInfoButton.Enabled = false;
            FlashReadButton.Enabled = false;
            FlashReadFilePathButton.Enabled = false;
            ApplicationReadFilePathButton.Enabled = false;
            BootloaderReadFilePathButton.Enabled = false;
            FlashWriteToFileButton.Enabled = false;
            ApplicationWriteToFileButton.Enabled = false;
            BootloaderWriteToFileButton.Enabled = false;
        }

        private void InvalidateInfoLabels()
        {
            DeviceNameLabel.Text = "-";
            HardwareVersionLabel.Text = "-";
            SoftwareMajorLabel.Text = "-";
            SoftwareMinorLabel.Text = "-";
            FlashSizeBytesLabel.Text = "-";
            AppMemSizeLabel.Text = "-";
            BootMemSizeLabel.Text = "-";
        }

        private void refreshCOMPortListButton_Click(object sender, EventArgs e)
        {
            PopulateCOMComboBox();
            ProcessProgramState();
        }

        private void serialPortTimer_Tick(object sender, EventArgs e)
        {
            if (!_deviceManager.SerialIsOpen)
            {
                Debug.WriteLine("TIMER IN NOT CONNECTED");
                PopulateCOMComboBox();
                _serialOpen = false;
                ProcessProgramState();
            }
        }

        private void COMPortConnectButton_Click(object sender, EventArgs e)
        {
            InvalidateButtons();
            if (!_deviceManager.SerialIsOpen)
            {
                Debug.WriteLine("PRESSED CONNECT");
                _deviceManager.PortName = COMPortComboBox.Text;
                _deviceManager.SerialOpen();
            } else
            {
                Debug.WriteLine("PRESSED DISCONNECT");
                _deviceManager.SerialClose();
            }
        }

        private void HandleSerialOpened(object sender, EventArgs e)
        {
            Debug.WriteLine("SERIAL OPENED EVENT");
            this.BeginInvoke(() =>
            {
                _serialOpen = true;
                ProcessProgramState();
            });
        }

        private void HandleSerialClosed(object sender, EventArgs e)
        {
            Debug.WriteLine("SERIAL CLOSED EVENT");
            _serialOpen = false;
            ProcessProgramState();
        }

        private void GetDeviceInfoButton_Click(object sender, EventArgs e)
        {
            InvalidateButtons();
            //GetDeviceInfoButton.Enabled = false;
            _deviceManager.GetDeviceInfo(_device);

        }

        private void HandleGotDeviceInfo(object sender, EventArgs e)
        {
            // Serial Port Object from _deviceManager spawns a new Thread to use for its DataReceived Event.
            // This is called by the _deviceManager object and from a different thread created by Serial Data Received handler
            // So BeginInvoke is needed to act on the forms members that got created by a different thread
            this.BeginInvoke(() =>
            {
                commandProgressUpdateBar.Value = 0;
                _gotDeviceInfo = true;
                ProcessProgramState();
            });
        }

        private void FlashReadFilePathButton_Click(object sender, EventArgs e)
        {
            readFlashImageToFileSaveDialog.InitialDirectory = Directory.GetCurrentDirectory();
            if (readFlashImageToFileSaveDialog.ShowDialog() == DialogResult.OK)
            {
                _device.WholeFlashBinImageReadFromDeviceFilePath = readFlashImageToFileSaveDialog.FileName;
                FlashReadFilePathLabel.Text = _device.WholeFlashBinImageReadFromDeviceFilePath;
            }
        }

        private void FlashReadFilePathLabel_TextChanged(object sender, EventArgs e)
        {
            ReadFlashFilenameToolTip.SetToolTip(FlashReadFilePathLabel, _device.WholeFlashBinImageReadFromDeviceFilePath);
        }

        private void ApplicationReadFilePathButton_Click(object sender, EventArgs e)
        {
            readFlashImageToFileSaveDialog.InitialDirectory = Directory.GetCurrentDirectory();
            if (readFlashImageToFileSaveDialog.ShowDialog() == DialogResult.OK)
            {
                _device.ApplicationBinImageReadFromDeviceFilePath = readFlashImageToFileSaveDialog.FileName;
                ApplicationReadFilePathLabel.Text = _device.ApplicationBinImageReadFromDeviceFilePath;
            }
        }

        private void ApplicationReadFilePathLabel_TextChanged(object sender, EventArgs e)
        {
            ReadFlashFilenameToolTip.SetToolTip(ApplicationReadFilePathLabel, _device.ApplicationBinImageReadFromDeviceFilePath);
        }

        private void BootloaderReadFilePathButton_Click(object sender, EventArgs e)
        {
            readFlashImageToFileSaveDialog.InitialDirectory = Directory.GetCurrentDirectory();
            if (readFlashImageToFileSaveDialog.ShowDialog() == DialogResult.OK)
            {
                _device.BootloaderBinImageReadFromDeviceFilePath = readFlashImageToFileSaveDialog.FileName;
                BootloaderReadFilePathLabel.Text = _device.BootloaderBinImageReadFromDeviceFilePath;
            }
        }

        private void BootloaderReadFilePathLabel_TextChanged(object sender, EventArgs e)
        {
            ReadFlashFilenameToolTip.SetToolTip(BootloaderReadFilePathLabel, _device.BootloaderBinImageReadFromDeviceFilePath);
        }

        private void FlashWriteToFileButton_Click(object sender, EventArgs e)
        {
            _deviceManager.SaveWholeFlashImageBytesToFile(_device);
        }

        private void ApplicationWriteToFileButton_Click(object sender, EventArgs e)
        {
            _deviceManager.SaveApplicationImageBytesToFile(_device);
        }

        private void BootloaderWriteToFileButton_Click(object sender, EventArgs e)
        {
            _deviceManager.SaveBootloaderImageBytesToFile(_device);
        }

        private void FlashReadButton_Click(object sender, EventArgs e)
        {
            InvalidateButtons();
            //FlashReadButton.Enabled = false;
            _deviceManager.ReadWholeFlash(_device);
        }

        private void HandleReadWholeFlash(object sender, EventArgs e)
        {
            // Serial Port Object from _deviceManager spawns a new Thread to use for its DataReceived Event.
            // This is called by the _deviceManager object and from a different thread created by Serial Data Received handler
            // So BeginInvoke is needed to act on the forms members that got created by a different thread
            this.BeginInvoke(() =>
            {
                commandProgressUpdateBar.Value = 0;
                _flashHasBeenRead = true;
                ProcessProgramState();
            });
        }

        private void HandleProgressUpdate(object sender, UInt16 progress)
        {
            // Serial Port Object from _deviceManager spawns a new Thread to use for its DataReceived Event.
            // This is called by the _deviceManager object and from a different thread created by Serial Data Received handler
            // So BeginInvoke is needed to act on the forms members that got created by a different thread
            this.BeginInvoke(() =>
            {
                commandProgressUpdateBar.Value = progress;
            });
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ProcessProgramState()
        {
            // CAUTION: Recursion will happen when I place ProcessProgramState in a path that is outside of a "moving from one state to other" line

            switch(_programState)
            {
                case ProgramStates.NoCOMPortFound:
                    Debug.WriteLine("NoCOMPortFound");
                    COMPortComboBox.Enabled = true;
                    COMPortListRefreshButton.Enabled = true;
                    COMPortConnectButton.Text = "Connect";
                    COMPortConnectButton.Enabled = false;
                    GetDeviceInfoButton.Enabled = false;
                    FlashReadButton.Enabled = false;
                    FlashReadFilePathButton.Enabled = false;
                    ApplicationReadFilePathButton.Enabled = false;
                    BootloaderReadFilePathButton.Enabled = false;
                    FlashWriteToFileButton.Enabled = false;
                    ApplicationWriteToFileButton.Enabled = false;
                    BootloaderWriteToFileButton.Enabled = false;
                    InvalidateInfoLabels();
                    SerialConnectionStatusBarLabel.Text = "No COM ports available";
                    // Next state decision
                    if (_serialCOMListNotEmpty == true)
                    {
                        _programState = ProgramStates.SerialClosed;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    }
                    // !!!!! ProcessProgramState() <-- This will lock program into recursion because when in this state it will recursively run it!!!
                    break;
                case ProgramStates.SerialClosed:
                    Debug.WriteLine("SerialClosed");
                    COMPortComboBox.Enabled = true;
                    COMPortListRefreshButton.Enabled = true;
                    COMPortConnectButton.Text = "Connect";
                    COMPortConnectButton.Enabled = true;
                    GetDeviceInfoButton.Enabled = false;
                    FlashReadButton.Enabled = false;
                    FlashReadFilePathButton.Enabled = false;
                    ApplicationReadFilePathButton.Enabled = false;
                    BootloaderReadFilePathButton.Enabled = false;
                    FlashWriteToFileButton.Enabled = false;
                    ApplicationWriteToFileButton.Enabled = false;
                    BootloaderWriteToFileButton.Enabled = false;
                    InvalidateInfoLabels();
                    SerialConnectionStatusBarLabel.Text = "COM Port Closed";
                    // Next state decision
                    if (_serialCOMListNotEmpty == false)
                    {
                        _programState = ProgramStates.NoCOMPortFound;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    }
                    else if (_serialOpen == true)
                    {
                        _programState = ProgramStates.SerialOpened;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    }
                    break;
                case ProgramStates.SerialOpened:
                    Debug.WriteLine("SerialOpened");
                    COMPortComboBox.Enabled = false;
                    COMPortListRefreshButton.Enabled = false;
                    COMPortConnectButton.Text = "Disconnect";
                    COMPortConnectButton.Enabled = true;
                    GetDeviceInfoButton.Enabled = true;
                    FlashReadButton.Enabled = false;
                    FlashReadFilePathButton.Enabled = false;
                    ApplicationReadFilePathButton.Enabled = false;
                    BootloaderReadFilePathButton.Enabled = false;
                    FlashWriteToFileButton.Enabled = false;
                    ApplicationWriteToFileButton.Enabled = false;
                    BootloaderWriteToFileButton.Enabled = false;
                    SerialConnectionStatusBarLabel.Text = "COM Port Open";
                    // Next state decision
                    if (_serialCOMListNotEmpty == false)
                    {
                        Debug.WriteLine("SerialOpened -> NoCOMPortFound");
                        _programState = ProgramStates.NoCOMPortFound;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    }
                    else if (_serialOpen == false)
                    {
                        Debug.WriteLine("SerialOpened -> SerialClosed");
                        _gotDeviceInfo = false;
                        _flashHasBeenRead = false;
                        _programState = ProgramStates.SerialClosed;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    }
                    else if (_gotDeviceInfo == true)
                    {
                        _programState = ProgramStates.DeviceValid;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    }
                    break;
                case ProgramStates.DeviceValid:
                    Debug.WriteLine("DeviceValid");
                    COMPortComboBox.Enabled = false;
                    COMPortListRefreshButton.Enabled = false;
                    COMPortConnectButton.Text = "Disconnect";
                    COMPortConnectButton.Enabled = true;
                    GetDeviceInfoButton.Enabled = true;
                    FlashReadButton.Enabled = true;
                    FlashReadFilePathButton.Enabled = false;
                    ApplicationReadFilePathButton.Enabled = false;
                    BootloaderReadFilePathButton.Enabled = false;
                    FlashWriteToFileButton.Enabled = false;
                    ApplicationWriteToFileButton.Enabled = false;
                    BootloaderWriteToFileButton.Enabled = false;

                    DeviceNameLabel.Text = _device.DeviceName;
                    HardwareVersionLabel.Text = _device.HardwareVersion.ToString();
                    SoftwareMajorLabel.Text = _device.SoftwareMajor.ToString();
                    SoftwareMinorLabel.Text = _device.SoftwareMinor.ToString();
                    FlashSizeBytesLabel.Text = _device.FlashSizeInBytes.ToString();
                    AppMemSizeLabel.Text = _device.ApplicationSizeInBytes.ToString();
                    BootMemSizeLabel.Text = _device.BootloaderSizeInBytes.ToString();

                    SerialConnectionStatusBarLabel.Text = "COM Port Open - Got Device Info";

                    if (_serialCOMListNotEmpty == false)
                    {
                        _programState = ProgramStates.NoCOMPortFound;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    }
                    else if (_serialOpen == false)
                    {
                        _gotDeviceInfo = false;
                        _flashHasBeenRead = false;
                        _device = null; // Delete object and create a fresh one with default values
                        _device = new AVRDevice(); // Maybe move that invalidation to the AVRDevice class
                        _programState = ProgramStates.SerialClosed;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    } else if(_flashHasBeenRead == true)
                    {
                        _programState = ProgramStates.FlashHasBeenRead;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    }
                    break;
                case ProgramStates.FlashHasBeenRead:
                    Debug.WriteLine("FlashHasBeenRead");
                    COMPortComboBox.Enabled = false;
                    COMPortListRefreshButton.Enabled = false;
                    COMPortConnectButton.Text = "Disconnect";
                    COMPortConnectButton.Enabled = true;
                    GetDeviceInfoButton.Enabled = true;
                    FlashReadButton.Enabled = true;
                    FlashReadFilePathButton.Enabled = true;
                    ApplicationReadFilePathButton.Enabled = true;
                    BootloaderReadFilePathButton.Enabled = true;
                    FlashWriteToFileButton.Enabled = true;
                    ApplicationWriteToFileButton.Enabled = true;
                    BootloaderWriteToFileButton.Enabled = true;
                    SerialConnectionStatusBarLabel.Text = "COM Port Open - Got Flash Image";

                    if (_serialCOMListNotEmpty == false)
                    {
                        _programState = ProgramStates.NoCOMPortFound;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    }
                    else if (_serialOpen == false)
                    {
                        _gotDeviceInfo = false;
                        _flashHasBeenRead = false;
                        _device = null; // Delete object and create a fresh one with default values
                        _device = new AVRDevice(); // Maybe move that invalidation to the AVRDevice class
                        _programState = ProgramStates.SerialClosed;
                        // Need to rerun it so that it moves and does not delay "one cycle"
                        // There is no recursion because at the second execution of ProcessProgramState it will not path through here
                        ProcessProgramState();
                    }
                    break;
                default:
                    throw new Exception("Not supposed to reach here");
                    break;
            }
        }

        enum ProgramStates
        {
            NoCOMPortFound,
            SerialClosed,
            SerialOpened, // and Device Invalid
            DeviceValid,
            FlashHasBeenRead
        }
    }
}