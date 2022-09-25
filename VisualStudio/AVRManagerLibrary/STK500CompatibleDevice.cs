using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVRManagerLibrary
{
    public class STK500CompatibleDevice
    {
        private SerialPort _serial;
        private int _inBootloaderMode = 0;

        // Events exposed
        public event EventHandler SerialOpened;
        public event EventHandler SerialClosed;
        public event EventHandler HasGotDeviceInfo;
        public event EventHandler HasReadWholeFlash;
        public event EventHandler<UInt16> ProgressUpdate; // No need to create a whole new inherited class for Args, just pass UInt16
        
        protected virtual void OnSerialOpened()
        {
            SerialOpened?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSerialClosed()
        {
            SerialClosed?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnHasGotDeviceInfo()
        {
            HasGotDeviceInfo?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnHasReadWholeFlash()
        {
            HasReadWholeFlash?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnProgressUpdate(UInt16 progress)
        {
            ProgressUpdate?.Invoke(this, progress);
        }

        public STK500CompatibleDevice()
        {
            _serial = new SerialPort();
            _serial.BaudRate = 115200;
            _serial.Parity = Parity.None;
            _serial.DataBits = 8;
            _serial.StopBits = StopBits.One;
            _serial.Handshake = Handshake.None;
            _serial.DataReceived += this.SerialPortDataReceived;
        }

        public STK500CompatibleDevice(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake)
        {
            _serial = new SerialPort();
            _serial.PortName = portName;
            _serial.BaudRate = baudRate;
            _serial.Parity = parity;
            _serial.DataBits = dataBits;
            _serial.StopBits = stopBits;
            _serial.Handshake = handshake;
            _serial.DataReceived += this.SerialPortDataReceived;
        }

        public string PortName
        {
            set
            {
                _serial.PortName = value;
            }
            get
            {
                return _serial.PortName;
            }
        }

        public int BaudRate
        {
            set
            {
                _serial.BaudRate = value;
            }
            get
            {
                return _serial.BaudRate;
            }
        }

        public bool SerialIsOpen
        {
            get
            {
                return _serial.IsOpen;
            }
        }

        public void SerialOpen()
        {
            try
            {
                _serial.Open();
                OnSerialOpened();
            }
            catch
            {
                throw new Exception("Cannot open serial port");
            }
        }

        public void SerialClose()
        {
            try
            {
                _serial.Close();
                OnSerialClosed();
            }
            catch
            {
                throw new Exception("Cannot close serial port");
            }
        }

        public string[] GetPortNames()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }

        public void SerialPortDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            var serialPort = (System.IO.Ports.SerialPort)sender;
            var bytes = new int[serialPort.BytesToRead];
            if (_inBootloaderMode > 0)
            {
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = serialPort.ReadByte();//ReadChar();
                    STK500_ProcessStates(bytes[i]);
                }
            }
            else
            {
                Debug.WriteLine("{0}", serialPort.ReadLine());
            }
        }

        public void ResetDevice()
        {
            ResetPulse(100);
        }

        public STK500CompatibleDevice GetDeviceInfo(AVRDevice device)
        {
            ResetDevice();
            _serial.DiscardInBuffer();
            GetSyncCOMMAND(GetSyncCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16) (1 * 100 / 6) })
                .ReadSignatureCOMMAND(ReadSignatureCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(2 * 100 / 6) })
                .SendReadFuseExtCOMMAND(ReadFuseExtCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(3 * 100 / 6) })
                .GetParameterCOMMAND(STK500_States.Parm_STK_HW_VER, GetHardwareVersionCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(4 * 100 / 6) })
                .GetParameterCOMMAND(STK500_States.Parm_STK_SW_MAJOR, GetSoftwareMajorCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(5 * 100 / 6) })
                .GetParameterCOMMAND(STK500_States.Parm_STK_SW_MINOR, GetSoftwareMinorCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)100 })
                .GetSyncCOMMAND(GetDeviceInfoCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = 0 })
                ;
            return this;
        }

        public STK500CompatibleDevice ReadWholeFlash(AVRDevice device)
        {
            var startAddressInWords = device.FlashWordStartAddress;
            var length = device.FlashSizeInBytes;

            ResetDevice();

            _serial.DiscardInBuffer();

            if (length <= 256)
            {
                GetSyncCOMMAND(GetSyncCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(1 * 100 / 3) })
                    .LoadAddressCOMMAND(startAddressInWords, LoadedAddressCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(2 * 100 / 3) })
                    .ReadPageCOMMAND(length, FinishedReadingWholeFlashMemoryCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = 100 });
            }
            else
            {
                var iterations = (UInt16)((length - 1) / 256);
                GetSyncCOMMAND(GetSyncCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(1 * 100 / (iterations + 1)) });
                for (var i = 0; i <= iterations; i++)
                {
                    UInt16 tmp_address = (UInt16)(i * 256 / 2 + startAddressInWords); // Divided by 2 because address is in 16bit words
                    if (i == iterations)
                    {
                        UInt16 tmp_length = (UInt16)(length - i * 256);
                        LoadAddressCOMMAND((UInt16)tmp_address, LoadedAddressCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(i * 100 / (iterations + 1)) })
                            .ReadPageCOMMAND((UInt16)tmp_length, FinishedReadingWholeFlashMemoryCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(i * 100 / (iterations + 1)) });
                    }
                    else
                    {
                        UInt16 tmp_length = 256;
                        LoadAddressCOMMAND((UInt16)tmp_address, LoadedAddressCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(i * 100 / (iterations + 1)) })
                            .ReadPageCOMMAND((UInt16)tmp_length, InReadingWholeFlashMemoryCALLBACK, new BatchCommandParameter { ParameterObject = device, Progress = (UInt16)(i * 100 / (iterations + 1)) });
                    }
                }
            }
            return this;
        }

        public STK500CompatibleDevice ProgApplicationFlash(AVRDevice device)
        {
            var startAddressInWords = device.ApplicationWordStartAddress;
            var length = device.UserApplicationBinImageWriteToDeviceFileBytes.Length;
            var data = device.UserApplicationBinImageWriteToDeviceFileBytes;

            if (length == 0) throw new Exception("Length cannot be zero");
            if (length > 128) throw new Exception("Length cannot be more than 128 bytes"); // Flash page size is 64 words so 128 bytes, cannot write more than that per iteration

            if (length <= 128)
            {
                LoadAddressCOMMAND(startAddressInWords, LoadedAddressCALLBACK, (object)device)
                    .ProgPageCOMMAND(data, FinishedProgrammingApplicationFlashMemoryCALLBACK, (object)device);
            }
            else
            {
                var iterations = (UInt16)((length - 1) / 128);
                for (var i = 0; i <= iterations; i++)
                {
                    byte[] dataBlock;
                    if (i == iterations)
                    {
                        var remaining = data.Length - iterations * 128;
                        dataBlock = new byte[remaining];
                        Array.Copy(data, i * 128, dataBlock, 0, remaining);
                    }
                    else
                    {
                        dataBlock = new byte[128];
                        Array.Copy(data, i * 128, dataBlock, 0, 128);
                    }

                    UInt16 tmp_address = (UInt16)(i * 128 / 2 + startAddressInWords); // Divided by 2 because address is in 16bit words
                    LoadAddressCOMMAND((UInt16)tmp_address, LoadedAddressCALLBACK, (object)device).ProgPageCOMMAND(dataBlock, FinishedProgrammingApplicationFlashMemoryCALLBACK, (object)device);
                }
            }


            return this;
        }

        private void FinishedProgrammingApplicationFlashMemoryCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(command.Parameters);
            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");

            Debug.WriteLine("Finished programming");
        }

        private void GetDeviceInfoCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            var batchCommandParameter = (BatchCommandParameter)(command.Parameters);
            AVRDevice device = (AVRDevice)(batchCommandParameter.ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);
            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");

            OnHasGotDeviceInfo();
        }

        private void ResetPulse(int duration)
        {
            if (!_serial.IsOpen) throw new Exception("Serial port not open");

            _serial.DtrEnable = true;
            Thread.Sleep(duration);
            _serial.DtrEnable = false;
            Thread.Sleep(500);
        }

        private void ReadSignatureCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(((BatchCommandParameter)(command.Parameters)).ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);
            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");
            device.DeviceSignature(command.TempReceivedBytes[0], command.TempReceivedBytes[1], command.TempReceivedBytes[2]);
            OnProgressUpdate(progress);
        }

        private void GetSyncCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(((BatchCommandParameter)(command.Parameters)).ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);
            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");
            //device.IsValid = true;
            //DebugPrint($"SYNC: \t{device.Synced}");
            OnProgressUpdate(progress);
        }

        private void GetHardwareVersionCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(((BatchCommandParameter)(command.Parameters)).ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);
            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");
            device.HardwareVersion = command.TempReceivedBytes[0];
            //Debug.WriteLine($"HW_VERSION: \t{device.HardwareVersion}");
            OnProgressUpdate(progress);
        }

        private void GetSoftwareMajorCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(((BatchCommandParameter)(command.Parameters)).ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);
            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");
            device.SoftwareMajor = command.TempReceivedBytes[0];
            //Debug.WriteLine($"SW_MAJOR: \t{device.SoftwareMajor}");
            OnProgressUpdate(progress);
        }

        private void GetSoftwareMinorCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(((BatchCommandParameter)(command.Parameters)).ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);
            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");
            device.SoftwareMinor = command.TempReceivedBytes[0];
            //Debug.WriteLine($"SW_MINOR: \t{device.SoftwareMinor}");
            OnProgressUpdate(progress);
        }

        private void ReadFuseCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(((BatchCommandParameter)(command.Parameters)).ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);
            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");
            device.FuseLow = command.TempReceivedBytes[0];
            device.FuseHigh = command.TempReceivedBytes[1];
            //Debug.WriteLine($"FUSE LOW: \t{device.FuseLow.ToString("X2")}");
            //Debug.WriteLine($"FUSE HIGH: \t{device.FuseHigh.ToString("X2")}");
            OnProgressUpdate(progress);
        }

        private void ReadFuseExtCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(((BatchCommandParameter)(command.Parameters)).ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);
            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");
            device.FuseLow = command.TempReceivedBytes[0];
            device.FuseHigh = command.TempReceivedBytes[1];
            device.FuseExt = command.TempReceivedBytes[2];
            device.DeviceFuses(device.FuseLow, device.FuseHigh, device.FuseExt);
            //Debug.WriteLine($"FUSE LOW: \t{device.FuseLow.ToString("X2")}");
            //Debug.WriteLine($"FUSE HIGH: \t{device.FuseHigh.ToString("X2")}");
            //Debug.WriteLine($"FUSE EXT: \t{device.FuseExt.ToString("X2")}");
            OnProgressUpdate(progress);
        }

        private void LoadedAddressCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(((BatchCommandParameter)(command.Parameters)).ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);
            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");
            device.LastLoadedAddress = command.Address;
            OnProgressUpdate(progress);
        }

        private void InReadingWholeFlashMemoryCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(((BatchCommandParameter)(command.Parameters)).ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);

            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");

            for (var i = 0; i < command.ByteCount; i++)
            {
                device.WholeFlashBinImageReadFromDeviceFileBytes[device.LastLoadedAddress * 2 + i] = command.TempReceivedBytes[i];
            }
            OnProgressUpdate(progress);
        }

        private void FinishedReadingWholeFlashMemoryCALLBACK(object currentCommand)
        {
            STK500_Command command = (STK500_Command)currentCommand;
            AVRDevice device = (AVRDevice)(((BatchCommandParameter)(command.Parameters)).ParameterObject);
            var progress = (UInt16)(((BatchCommandParameter)(command.Parameters)).Progress);

            if (device == null)
                throw new Exception("Object parameter passed to callback is null. It shouldnt be! Check if a new flashImage was enqueued together with the command");

            for (var i = 0; i < command.ByteCount; i++)
            {
                device.WholeFlashBinImageReadFromDeviceFileBytes[device.LastLoadedAddress * 2 + i] = command.TempReceivedBytes[i];
            }

            Debug.WriteLine("Finished reading whole Flash.");

            //SaveFlashImageBytesToFile(device);

            OnProgressUpdate(progress);

            OnHasReadWholeFlash();

        }

        public void SaveWholeFlashImageBytesToFile(AVRDevice device)
        {
            Debug.WriteLine("Saving flash image to file");

            using (var stream = File.Open(device.WholeFlashBinImageReadFromDeviceFilePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, System.Text.Encoding.Default, false))
                {
                    for (var i = 0; i < device.WholeFlashBinImageReadFromDeviceFileBytes.Length; i++)
                        writer.Write(device.WholeFlashBinImageReadFromDeviceFileBytes[i]);
                }
            }
        }

        public void SaveApplicationImageBytesToFile(AVRDevice device)
        {
            Debug.WriteLine("Saving application image to file");

            using (var stream = File.Open(device.ApplicationBinImageReadFromDeviceFilePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, System.Text.Encoding.Default, false))
                {
                    for (var i = 0; i < device.ApplicationSizeInBytes; i++)
                        writer.Write(device.WholeFlashBinImageReadFromDeviceFileBytes[device.ApplicationByteStartAddress + i]);
                }
            }
        }

        public void SaveBootloaderImageBytesToFile(AVRDevice device)
        {
            Debug.WriteLine("Saving bootloader image to file");

            using (var stream = File.Open(device.BootloaderBinImageReadFromDeviceFilePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, System.Text.Encoding.Default, false))
                {
                    for (var i = 0; i < device.BootloaderSizeInBytes; i++)
                        writer.Write(device.WholeFlashBinImageReadFromDeviceFileBytes[device.BootloaderByteStartAddress + i]);
                }
            }
        }

        public void ReadFlashImageBytesFromFile(AVRDevice device, string filePath)
        {
            Debug.WriteLine($"Opening file {filePath} and reading binary contents.");

            if (!File.Exists(filePath))
                throw new Exception($"File {filePath} does not exist!.");

            FileInfo fi = new FileInfo(filePath);
            Debug.WriteLine($"File {filePath} has length {fi.Length} bytes.");

            if (fi.Length > device.ApplicationSizeInBytes)
                throw new Exception("File size greater than device application size");

            device.UserApplicationBinImageWriteToDeviceFileBytes = new byte[fi.Length];

            using (var stream = File.Open(filePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, System.Text.Encoding.Default, false))
                {
                    for (var i = 0; i < fi.Length; i++)
                        device.UserApplicationBinImageWriteToDeviceFileBytes[i] = reader.ReadByte();
                }
            }
        }

        private STK500_Command? _STK500_RunningCommand;
        private Queue<STK500_Command> _STK500_CommandQueue = new Queue<STK500_Command>(); // Create struct that includes STK500_States and PARAMETER NAME
        private Queue<STK500_SerialCommand> _STK500_SerialCommandBytesQueue = new Queue<STK500_SerialCommand>();

        private STK500CompatibleDevice GetSyncCOMMAND(Action<object>? callbackOnEnd = null, object? parameters = null)
        {
            if (!_serial.IsOpen) throw new Exception("Serial port not open");

            IncrementBootloaderMode();

            var cmd = new byte[2];
            // GET SYNC
            cmd[0] = (byte)STK500_States.Cmnd_STK_GET_SYNC;
            cmd[1] = (byte)STK500_States.Sync_CRC_EOP;
            //Thread.Sleep(500);
            if (_STK500_CommandQueue.Count == 0)
                _serial.Write(cmd, 0, cmd.Length);
            else
                _STK500_SerialCommandBytesQueue.Enqueue(new STK500_SerialCommand { cmd = cmd, length = cmd.Length }); ;
            _STK500_CommandQueue.Enqueue(new STK500_Command
            {
                Command = STK500_States.Cmnd_STK_GET_SYNC,
                CallbackOnEnd = callbackOnEnd,
                Parameters = parameters
            });

            return this;
        }

        private STK500CompatibleDevice ReadSignatureCOMMAND(Action<object>? callbackOnEnd = null, object? parameters = null)
        {
            if (!_serial.IsOpen) throw new Exception("Serial port not open");

            IncrementBootloaderMode();

            var cmd = new byte[2];
            cmd[0] = (byte)STK500_States.Cmnd_STK_READ_SIGN;
            cmd[1] = (byte)STK500_States.Sync_CRC_EOP;
            if (_STK500_CommandQueue.Count == 0)
                _serial.Write(cmd, 0, cmd.Length);
            else
                _STK500_SerialCommandBytesQueue.Enqueue(new STK500_SerialCommand { cmd = cmd, length = cmd.Length }); ;
            _STK500_CommandQueue.Enqueue(new STK500_Command
            {
                Command = STK500_States.Cmnd_STK_READ_SIGN,
                ByteCount = 3,
                CallbackOnEnd = callbackOnEnd,
                Parameters = parameters
            });
            //Thread.Sleep(500);
            return this;
        }

        private STK500CompatibleDevice GetParameterCOMMAND(STK500_States getparameter, Action<object>? callbackOnEnd = null, object? parameters = null)
        {
            if (!_serial.IsOpen) throw new Exception("Serial port not open");

            IncrementBootloaderMode();

            var cmd = new byte[3];
            cmd[0] = (byte)STK500_States.Cmnd_STK_GET_PARAMETER;
            cmd[1] = (byte)getparameter;
            cmd[2] = (byte)STK500_States.Sync_CRC_EOP;
            if (_STK500_CommandQueue.Count == 0)
                _serial.Write(cmd, 0, cmd.Length);
            else
                _STK500_SerialCommandBytesQueue.Enqueue(new STK500_SerialCommand { cmd = cmd, length = cmd.Length }); ;
            _STK500_CommandQueue.Enqueue(new STK500_Command
            {
                Command = STK500_States.Cmnd_STK_GET_PARAMETER,
                ParameterAsked = getparameter,
                CallbackOnEnd = callbackOnEnd,
                Parameters = parameters
            });
            //Thread.Sleep(500);
            return this;
        }

        private STK500CompatibleDevice SendReadFuseCOMMAND(Action<object>? callbackOnEnd = null, object? parameters = null)
        {
            if (!_serial.IsOpen) throw new Exception("Serial port not open");

            IncrementBootloaderMode();

            var cmd = new byte[2];
            cmd[0] = (byte)STK500_States.Cmnd_STK_READ_FUSE;
            cmd[1] = (byte)STK500_States.Sync_CRC_EOP;
            if (_STK500_CommandQueue.Count == 0)
                _serial.Write(cmd, 0, cmd.Length);
            else
                _STK500_SerialCommandBytesQueue.Enqueue(new STK500_SerialCommand { cmd = cmd, length = cmd.Length }); ;
            _STK500_CommandQueue.Enqueue(new STK500_Command
            {
                Command = STK500_States.Cmnd_STK_READ_FUSE,
                ByteCount = 2,
                CallbackOnEnd = callbackOnEnd,
                Parameters = parameters
            });
            //Thread.Sleep(500);
            return this;
        }

        private STK500CompatibleDevice SendReadFuseExtCOMMAND(Action<object>? callbackOnEnd = null, object? parameters = null)
        {
            if (!_serial.IsOpen) throw new Exception("Serial port not open");

            IncrementBootloaderMode();

            var cmd = new byte[2];
            cmd[0] = (byte)STK500_States.Cmnd_STK_READ_FUSE_EXT;
            cmd[1] = (byte)STK500_States.Sync_CRC_EOP;
            if (_STK500_CommandQueue.Count == 0)
                _serial.Write(cmd, 0, cmd.Length);
            else
                _STK500_SerialCommandBytesQueue.Enqueue(new STK500_SerialCommand { cmd = cmd, length = cmd.Length }); ;
            _STK500_CommandQueue.Enqueue(new STK500_Command
            {
                Command = STK500_States.Cmnd_STK_READ_FUSE_EXT,
                ByteCount = 3,
                CallbackOnEnd = callbackOnEnd,
                Parameters = parameters
            });
            //Thread.Sleep(500);
            return this;
        }

        private STK500CompatibleDevice LoadAddressCOMMAND(UInt16 address, Action<object>? callbackOnEnd = null, object? parameters = null)
        {
            if (!_serial.IsOpen) throw new Exception("Serial port not open");

            IncrementBootloaderMode();

            var cmd = new byte[4];
            cmd[0] = (byte)STK500_States.Cmnd_STK_LOAD_ADDRESS;
            cmd[1] = (byte)(address & 0x00FF); // Address LO
            cmd[2] = (byte)(address >> 8); // Address HI
            cmd[3] = (byte)STK500_States.Sync_CRC_EOP;
            if (_STK500_CommandQueue.Count == 0)
                _serial.Write(cmd, 0, cmd.Length);
            else
                _STK500_SerialCommandBytesQueue.Enqueue(new STK500_SerialCommand { cmd = cmd, length = cmd.Length }); ;
            _STK500_CommandQueue.Enqueue(new STK500_Command
            {
                Command = STK500_States.Cmnd_STK_LOAD_ADDRESS,
                Address = address,
                CallbackOnEnd = callbackOnEnd,
                Parameters = parameters
            });
            //Thread.Sleep(500);
            return this;
        }

        private STK500CompatibleDevice ReadPageCOMMAND(UInt16 length, Action<object>? callbackOnEnd = null, object? parameters = null)
        {
            if (!_serial.IsOpen) throw new Exception("Serial port not open");

            if (length == 0) throw new Exception("Length cannot be zero");
            if (length > 256) throw new Exception("Length cannot be more than 256 bytes");

            IncrementBootloaderMode();

            var cmd = new byte[5];
            cmd[0] = (byte)STK500_States.Cmnd_STK_READ_PAGE;
            cmd[1] = 0x00;
            cmd[2] = (byte)(length & 0x00FF); // 8bit length
            cmd[3] = (byte)'F';
            cmd[4] = (byte)STK500_States.Sync_CRC_EOP;
            if (_STK500_CommandQueue.Count == 0)
                _serial.Write(cmd, 0, cmd.Length);
            else
                _STK500_SerialCommandBytesQueue.Enqueue(new STK500_SerialCommand { cmd = cmd, length = cmd.Length });

            _STK500_CommandQueue.Enqueue(new STK500_Command
            {
                Command = STK500_States.Cmnd_STK_READ_PAGE,
                ByteCount = length,
                CallbackOnEnd = callbackOnEnd,
                Parameters = parameters
            });
            //Thread.Sleep(500);
            return this;
        }

        private STK500CompatibleDevice ProgPageCOMMAND(byte[] data, Action<object>? callbackOnEnd = null, object? parameters = null)
        {
            if (!_serial.IsOpen) throw new Exception("Serial port not open");

            if (data == null) throw new Exception("Data is null");
            if (data.Length == 0) throw new Exception("Length cannot be zero");
            if (data.Length > 128) throw new Exception("Length cannot be more than 128 bytes");

            IncrementBootloaderMode();

            var cmd = new byte[data.Length + 5];
            cmd[0] = (byte)STK500_States.Cmnd_STK_PROG_PAGE;
            cmd[1] = 0x00;
            cmd[2] = (byte)(data.Length & 0x00FF); // 8bit length
            cmd[3] = (byte)'F';

            var i = 0;
            for (i = 0; i < data.Length; i++)
            {
                cmd[i + 4] = data[i];
            }

            cmd[i + 4] = (byte)STK500_States.Sync_CRC_EOP;

            if (_STK500_CommandQueue.Count == 0)
                _serial.Write(cmd, 0, cmd.Length);
            else
                _STK500_SerialCommandBytesQueue.Enqueue(new STK500_SerialCommand { cmd = cmd, length = cmd.Length });

            _STK500_CommandQueue.Enqueue(new STK500_Command
            {
                Command = STK500_States.Cmnd_STK_PROG_PAGE,
                CallbackOnEnd = callbackOnEnd,
                Parameters = parameters
            });
            //Thread.Sleep(500);
            return this;
        }

        private STK500CompatibleDevice SendUniversalCOMMAND(Action<object>? callbackOnEnd = null, object? parameters = null)
        {
            if (!_serial.IsOpen) throw new Exception("Serial port not open");

            IncrementBootloaderMode();

            var cmd = new byte[6];
            cmd[0] = (byte)STK500_States.Cmnd_STK_UNIVERSAL;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x00;
            cmd[4] = 0x00;
            cmd[5] = (byte)STK500_States.Sync_CRC_EOP;
            if (_STK500_CommandQueue.Count == 0)
                _serial.Write(cmd, 0, cmd.Length);
            else
                _STK500_SerialCommandBytesQueue.Enqueue(new STK500_SerialCommand { cmd = cmd, length = cmd.Length }); ;
            _STK500_CommandQueue.Enqueue(new STK500_Command
            {
                Command = STK500_States.Cmnd_STK_UNIVERSAL,
                CallbackOnEnd = callbackOnEnd,
                Parameters = parameters
            });
            //Thread.Sleep(500);
            return this;
        }

        private void STK500_ProcessStates(int b)
        {
            if (_inBootloaderMode == 0) return;

            if (_STK500_RunningCommand == null || _STK500_RunningCommand.Command == STK500_States.ZEROSTATE)
            {
                if (_STK500_CommandQueue.Count == 0) {
                    Debug.WriteLine("QUEUE EMPTY!!");
                    return;
                }  else
                    _STK500_RunningCommand = _STK500_CommandQueue.Dequeue();
            }

            switch (_STK500_RunningCommand.Command)
            {
                case STK500_States.ZEROSTATE:
                    throw new Exception("Reached Zero State. Not supposed to reach here");
                    break;
                case STK500_States.Cmnd_STK_GET_SYNC:
                    switch (_STK500_RunningCommand.InCmdState)
                    {
                        case STK500_States.ZEROSTATE:
                            if (b == (int)STK500_States.Resp_STK_INSYNC)
                            {
                                Debug.WriteLine("Received SYNC {}");
                                _STK500_RunningCommand.InCmdState = STK500_States.Resp_STK_INSYNC;
                            }
                            else
                            {
                                Debug.WriteLine("Did not receive SYNC");
                                STK500_ProcessStatesResetPrint($"NO SYNC RECEIVED {b}");
                            }
                            break;
                        case STK500_States.Resp_STK_INSYNC:
                            if (b == (int)STK500_States.Resp_STK_OK)
                            {
                                _STK500_RunningCommand.CallbackOnEnd?.Invoke(_STK500_RunningCommand);
                                STK500_ProcessStatesReset();
                            }
                            else STK500_ProcessStatesResetPrint("NO RESPONSE OK RECEIVED");
                            break;
                    }
                    break;
                case STK500_States.Cmnd_STK_GET_PARAMETER:
                    switch (_STK500_RunningCommand.InCmdState)
                    {
                        case STK500_States.ZEROSTATE:
                            if (b == (int)STK500_States.Resp_STK_INSYNC) _STK500_RunningCommand.InCmdState = STK500_States.Resp_STK_INSYNC;
                            else STK500_ProcessStatesResetPrint("Command Lost");
                            break;
                        case STK500_States.Resp_STK_INSYNC:
                            _STK500_RunningCommand.TempReceivedBytes[0] = (byte)b;
                            _STK500_RunningCommand.InCmdState = STK500_States.VALUE;
                            break;
                        case STK500_States.VALUE:
                            if (b == (int)STK500_States.Resp_STK_OK)
                            {
                                _STK500_RunningCommand.CallbackOnEnd?.Invoke(_STK500_RunningCommand);
                                STK500_ProcessStatesReset();
                            }
                            else STK500_ProcessStatesResetPrint("Cmnd_STK_GET_PARAM: NO RESPONSE OK RECEIVED");
                            break;
                    }
                    break;
                case STK500_States.Cmnd_STK_READ_FUSE:
                    switch (_STK500_RunningCommand.InCmdState)
                    {
                        case STK500_States.ZEROSTATE:
                            if (b == (int)STK500_States.Resp_STK_INSYNC) _STK500_RunningCommand.InCmdState = STK500_States.Resp_STK_INSYNC;
                            else STK500_ProcessStatesResetPrint("Command Lost");
                            break;
                        case STK500_States.Resp_STK_INSYNC:
                            _STK500_RunningCommand.TempReceivedBytes[_STK500_RunningCommand.ByteCounter] = (byte)b;
                            if (_STK500_RunningCommand.ByteCounter == _STK500_RunningCommand.ByteCount - 1)
                                _STK500_RunningCommand.InCmdState = STK500_States.VALUE;
                            else
                                _STK500_RunningCommand.ByteCounter++;
                            break;
                        case STK500_States.VALUE:
                            if (b == (int)STK500_States.Resp_STK_OK)
                            {
                                _STK500_RunningCommand.CallbackOnEnd?.Invoke(_STK500_RunningCommand);
                                STK500_ProcessStatesReset();
                            }
                            else STK500_ProcessStatesResetPrint("Cmnd_STK_READ_FUSE: NO RESPONSE OK RECEIVED");
                            break;
                    }
                    break;
                case STK500_States.Cmnd_STK_READ_FUSE_EXT:
                    switch (_STK500_RunningCommand.InCmdState)
                    {
                        case STK500_States.ZEROSTATE:
                            if (b == (int)STK500_States.Resp_STK_INSYNC) _STK500_RunningCommand.InCmdState = STK500_States.Resp_STK_INSYNC;
                            else STK500_ProcessStatesResetPrint("Command Lost");
                            break;
                        case STK500_States.Resp_STK_INSYNC:
                            _STK500_RunningCommand.TempReceivedBytes[_STK500_RunningCommand.ByteCounter] = (byte)b;
                            if (_STK500_RunningCommand.ByteCounter == _STK500_RunningCommand.ByteCount - 1)
                                _STK500_RunningCommand.InCmdState = STK500_States.VALUE;
                            else
                                _STK500_RunningCommand.ByteCounter++;
                            break;
                        case STK500_States.VALUE:
                            if (b == (int)STK500_States.Resp_STK_OK)
                            {
                                _STK500_RunningCommand.CallbackOnEnd?.Invoke(_STK500_RunningCommand);
                                STK500_ProcessStatesReset();
                            }
                            else STK500_ProcessStatesResetPrint("Cmnd_STK_READ_FUSE_EXT: NO RESPONSE OK RECEIVED");
                            break;
                    }
                    break;
                case STK500_States.Cmnd_STK_READ_SIGN:
                    switch (_STK500_RunningCommand.InCmdState)
                    {
                        case STK500_States.ZEROSTATE:
                            if (b == (int)STK500_States.Resp_STK_INSYNC) _STK500_RunningCommand.InCmdState = STK500_States.Resp_STK_INSYNC;
                            else STK500_ProcessStatesResetPrint("Command Lost");
                            break;
                        case STK500_States.Resp_STK_INSYNC:
                            _STK500_RunningCommand.TempReceivedBytes[_STK500_RunningCommand.ByteCounter] = (byte)b;
                            if (_STK500_RunningCommand.ByteCounter == _STK500_RunningCommand.ByteCount - 1)
                                _STK500_RunningCommand.InCmdState = STK500_States.VALUE;
                            else
                                _STK500_RunningCommand.ByteCounter++;
                            break;
                        case STK500_States.VALUE:
                            if (b == (int)STK500_States.Resp_STK_OK)
                            {
                                _STK500_RunningCommand.CallbackOnEnd?.Invoke(_STK500_RunningCommand);
                                STK500_ProcessStatesReset();
                            }
                            else STK500_ProcessStatesResetPrint("Cmnd_STK_READ_SIGN: NO RESPONSE OK RECEIVED");
                            break;
                    }
                    break;
                case STK500_States.Cmnd_STK_UNIVERSAL:
                    switch (_STK500_RunningCommand.InCmdState)
                    {
                        case STK500_States.ZEROSTATE:
                            Debug.WriteLine("Decoding UNIVERSAL command");
                            if (b == (int)STK500_States.Resp_STK_INSYNC) _STK500_RunningCommand.InCmdState = STK500_States.Resp_STK_INSYNC;
                            else STK500_ProcessStatesResetPrint("Command Lost");
                            break;
                        case STK500_States.Resp_STK_INSYNC:
                            _STK500_RunningCommand.TempReceivedBytes[0] = (byte)b;
                            _STK500_RunningCommand.InCmdState = STK500_States.VALUE;
                            break;
                        case STK500_States.VALUE:
                            if (b == (int)STK500_States.Resp_STK_OK)
                            {
                                _STK500_RunningCommand.CallbackOnEnd?.Invoke(_STK500_RunningCommand);
                                STK500_ProcessStatesResetPrint($"Got Universal: {_STK500_RunningCommand.TempReceivedBytes[0]}");
                            }
                            else STK500_ProcessStatesResetPrint("Cmnd_STK_UNIVERSAL: NO RESPONSE OK RECEIVED");
                            break;
                    }
                    break;
                case STK500_States.Cmnd_STK_LOAD_ADDRESS:
                    switch (_STK500_RunningCommand.InCmdState)
                    {
                        case STK500_States.ZEROSTATE:
                            if (b == (int)STK500_States.Resp_STK_INSYNC) _STK500_RunningCommand.InCmdState = STK500_States.Resp_STK_INSYNC;
                            else STK500_ProcessStatesResetPrint("Command Lost");
                            break;
                        case STK500_States.Resp_STK_INSYNC:
                            if (b == (int)STK500_States.Resp_STK_OK)
                            {
                                _STK500_RunningCommand.CallbackOnEnd?.Invoke(_STK500_RunningCommand);
                                STK500_ProcessStatesReset();
                            }
                            else STK500_ProcessStatesResetPrint("Cmnd_LOAD_ADDRESS: NO RESPONSE OK RECEIVED");
                            break;
                    }
                    break;
                case STK500_States.Cmnd_STK_READ_PAGE:
                    switch (_STK500_RunningCommand.InCmdState)
                    {
                        case STK500_States.ZEROSTATE:
                            if (b == (int)STK500_States.Resp_STK_INSYNC) _STK500_RunningCommand.InCmdState = STK500_States.Resp_STK_INSYNC;
                            else STK500_ProcessStatesResetPrint("Command Lost");
                            break;
                        case STK500_States.Resp_STK_INSYNC:
                            _STK500_RunningCommand.TempReceivedBytes[_STK500_RunningCommand.ByteCounter] = (byte)b;
                            if (_STK500_RunningCommand.ByteCounter == _STK500_RunningCommand.ByteCount - 1)
                                _STK500_RunningCommand.InCmdState = STK500_States.VALUE;
                            else
                                _STK500_RunningCommand.ByteCounter++;
                            break;
                        case STK500_States.VALUE:
                            if (b == (int)STK500_States.Resp_STK_OK)
                            {
                                _STK500_RunningCommand.CallbackOnEnd?.Invoke(_STK500_RunningCommand);
                                STK500_ProcessStatesReset();

                            }
                            else STK500_ProcessStatesResetPrint("Cmnd_READ PAGE: NO RESPONSE OK RECEIVED");
                            break;
                    }
                    break;
                case STK500_States.Cmnd_STK_PROG_PAGE:
                    switch (_STK500_RunningCommand.InCmdState)
                    {
                        case STK500_States.ZEROSTATE:
                            Debug.WriteLine("DECODING PROG PAGE");
                            if (b == (int)STK500_States.Resp_STK_INSYNC) _STK500_RunningCommand.InCmdState = STK500_States.Resp_STK_INSYNC;
                            else STK500_ProcessStatesResetPrint("Command Lost");
                            break;
                        case STK500_States.Resp_STK_INSYNC:
                            if (b == (int)STK500_States.Resp_STK_OK)
                            {
                                _STK500_RunningCommand.CallbackOnEnd?.Invoke(_STK500_RunningCommand);
                                STK500_ProcessStatesResetPrint($"PROG PAGE SUCCEEDED");
                            }
                            else STK500_ProcessStatesResetPrint("Cmnd_PROG_PAGE: NO RESPONSE OK RECEIVED");
                            break;
                    }
                    break;
                default:
                    _STK500_RunningCommand.Command = STK500_States.ZEROSTATE;
                    throw new Exception("Fell to default state");
                    break;
            }
        }

        private void STK500_ProcessStatesReset()
        {
            _STK500_RunningCommand.Command = STK500_States.ZEROSTATE;
            _STK500_RunningCommand.InCmdState = STK500_States.ZEROSTATE;
            _STK500_RunningCommand.ParameterAsked = STK500_States.ZEROSTATE;
            _STK500_RunningCommand.ByteCount = 0;
            _STK500_RunningCommand.ByteCounter = 0;
            _STK500_RunningCommand.Address = 0;
            Array.Clear(_STK500_RunningCommand.TempReceivedBytes, 0, _STK500_RunningCommand.TempReceivedBytes.Length);
            DecrementBootloaderMode();
            if (_STK500_CommandQueue.Count > 0)
            {
                // Previous command has finished through the state machine, so dequeue and fire up the next Serial command in the queue
                STK500_SerialCommand nextSerialCommand = _STK500_SerialCommandBytesQueue.Dequeue();
                _serial.Write(nextSerialCommand.cmd, 0, nextSerialCommand.length);
            }
        }

        private void STK500_ProcessStatesResetPrint(string message)
        {
            _STK500_RunningCommand.Command = STK500_States.ZEROSTATE;
            _STK500_RunningCommand.InCmdState = STK500_States.ZEROSTATE;
            _STK500_RunningCommand.ParameterAsked = STK500_States.ZEROSTATE;
            _STK500_RunningCommand.ByteCount = 0;
            _STK500_RunningCommand.ByteCounter = 0;
            _STK500_RunningCommand.Address = 0;
            Array.Clear(_STK500_RunningCommand.TempReceivedBytes, 0, _STK500_RunningCommand.TempReceivedBytes.Length);
            Debug.WriteLine(message);
            DecrementBootloaderMode();
            if (_STK500_CommandQueue.Count > 0)
            {
                // Previous command has finished through the state machine, so dequeue and fire up the next Serial command in the queue
                STK500_SerialCommand nextSerialCommand = _STK500_SerialCommandBytesQueue.Dequeue();
                _serial.Write(nextSerialCommand.cmd, 0, nextSerialCommand.length);
            }
        }

        private void IncrementBootloaderMode()
        {
            var tmp = _inBootloaderMode;
            _inBootloaderMode++;
            if (tmp == 0 && _inBootloaderMode == 1)
                Debug.WriteLine($"Entered bootloader mode {_inBootloaderMode}");
            else if (tmp == 1 && _inBootloaderMode == 0)
                Debug.WriteLine($"Exited bootloader mode {_inBootloaderMode}");
        }

        private void DecrementBootloaderMode()
        {
            var tmp = _inBootloaderMode;
            _inBootloaderMode--;
            if (tmp == 0 && _inBootloaderMode == 1)
                Debug.WriteLine($"Entered bootloader mode {_inBootloaderMode}");
            else if (tmp == 1 && _inBootloaderMode == 0)
                Debug.WriteLine($"Exited bootloader mode {_inBootloaderMode}");
        }
        internal enum STK500_States : int
        {
            ZEROSTATE = 0xFF, // Used only for state machine
            VALUE,

            Resp_STK_OK = 0x10,
            Resp_STK_FAILED = 0x11,
            Resp_STK_UNKNOWN = 0x12,
            Resp_STK_NODEVICE = 0x13,
            Resp_STK_INSYNC = 0x14,
            Resp_STK_NOSYNC = 0x15,
            Resp_ADC_CHANNEL_ERROR = 0x16,
            Resp_ADC_MEASURE_OK = 0x17,
            Resp_PWM_CHANNEL_ERROR = 0x18,
            Resp_PWM_ADJUST_OK = 0x19,

            Sync_CRC_EOP = 0x20,

            Cmnd_STK_GET_SYNC = 0x30,
            Cmnd_STK_GET_SIGN_ON = 0x31,
            Cmnd_STK_SET_PARAMETER = 0x40,
            Cmnd_STK_GET_PARAMETER = 0x41,
            Cmnd_STK_SET_DEVICE = 0x42,
            Cmnd_STK_SET_DEVICE_EXT = 0x45,
            Cmnd_STK_ENTER_PROGMODE = 0x50,
            Cmnd_STK_LEAVE_PROGMODE = 0x51,
            Cmnd_STK_CHIP_ERASE = 0x52,
            Cmnd_STK_CHECK_AUTOINC = 0x53,
            Cmnd_STK_LOAD_ADDRESS = 0x55,
            Cmnd_STK_UNIVERSAL = 0x56,
            Cmnd_STK_UNIVERSAL_MULTI = 0x57,
            Cmnd_STK_PROG_FLASH = 0x60,
            Cmnd_STK_PROG_DATA = 0x61,
            Cmnd_STK_PROG_FUSE = 0x62,
            Cmnd_STK_PROG_LOCK = 0x63,
            Cmnd_STK_PROG_PAGE = 0x64,
            Cmnd_STK_PROG_FUSE_EXT = 0x65,
            Cmnd_STK_READ_FLASH = 0x70,
            Cmnd_STK_READ_DATA = 0x71,
            Cmnd_STK_READ_FUSE = 0x72,
            Cmnd_STK_READ_LOCK = 0x73,
            Cmnd_STK_READ_PAGE = 0x74,
            Cmnd_STK_READ_SIGN = 0x75,
            Cmnd_STK_READ_OSCCAL = 0x76,
            Cmnd_STK_READ_FUSE_EXT = 0x77,
            Cmnd_STK_READ_OSCCAL_EXT = 0x78,

            Parm_STK_HW_VER = 0x80,
            Parm_STK_SW_MAJOR = 0x81,
            Parm_STK_SW_MINOR = 0x82,
            Parm_STK_LEDS = 0x83,
            Parm_STK_VTARGET = 0x84,
            Parm_STK_VADJUST = 0x85,
            Parm_STK_OSC_PSCALE = 0x86,
            Parm_STK_OSC_CMATCH = 0x87,
            Parm_STK_RESET_DURATION = 0x88,
            Parm_STK_SCK_DURATION = 0x89,
            Parm_STK_BUFSIZEL = 0x90,
            Parm_STK_BUFSIZEH = 0x91,
            Parm_STK_DEVICE = 0x92,
            Parm_STK_PROGMODE = 0x93,
            Parm_STK_PARAMODE = 0x94,
            Parm_STK_POLLING = 0x95,
            Parm_STK_SELFTIMED = 0x96,

            /*Stat_STK_INSYNC = 0x01,
            Stat_STK_PROGMODE = 0x02,
            Stat_STK_STANDALONE = 0x04,
            Stat_STK_RESET = 0x08,
            Stat_STK_PROGRAM = 0x10,
            Stat_STK_LEDG = 0x20,
            Stat_STK_LEDR = 0x40,
            Stat_STK_LEDBLINK = 0x80*/
        }


        internal class STK500_Command
        {
            public STK500_States Command;
            public STK500_States ParameterAsked;
            public UInt16 Address;
            public STK500_States InCmdState;
            public int ByteCount;
            public int ByteCounter;
            public byte[] TempReceivedBytes;
            public Action<object>? CallbackOnEnd;
            public object? Parameters;
            public STK500_Command()
            {
                Command = STK500_States.ZEROSTATE;
                ParameterAsked = STK500_States.ZEROSTATE;
                Address = 0;
                InCmdState = STK500_States.ZEROSTATE;
                ByteCount = 0;
                ByteCounter = 0;
                TempReceivedBytes = new byte[256];
                CallbackOnEnd = null;
                Parameters = null;
            }
        }

        internal class STK500_SerialCommand
        {
            public byte[] cmd;
            public int length;

            public STK500_SerialCommand()
            {
                cmd = new byte[7];
                length = 0;
            }
        }

        internal class BatchCommandParameter
        {
            public object? ParameterObject = null;
            public UInt16 Progress = 0;
        }
    }
}
