namespace AVRManagerWindows
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.COMPortComboBox = new System.Windows.Forms.ComboBox();
            this.COMPortListRefreshButton = new System.Windows.Forms.Button();
            this.COMPortConnectButton = new System.Windows.Forms.Button();
            this.serialPortTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.SerialConnectionStatusBarLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.commandProgressUpdateBar = new System.Windows.Forms.ToolStripProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.DeviceNameLabel = new System.Windows.Forms.Label();
            this.GetDeviceInfoButton = new System.Windows.Forms.Button();
            this.HardwareVersionLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SoftwareMinorLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SoftwareMajorLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.COMPortLabel = new System.Windows.Forms.Label();
            this.FlashSizeBytesLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AppMemSizeLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BootMemSizeLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FlashReadFilePathLabel = new System.Windows.Forms.Label();
            this.FlashReadFilePathButton = new System.Windows.Forms.Button();
            this.readFlashImageToFileSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.ReadFlashFilenameToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.FlashReadButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BootloaderWriteToFileButton = new System.Windows.Forms.Button();
            this.BootloaderReadFilePathLabel = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.BootloaderReadFilePathButton = new System.Windows.Forms.Button();
            this.ApplicationWriteToFileButton = new System.Windows.Forms.Button();
            this.ApplicationReadFilePathLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.ApplicationReadFilePathButton = new System.Windows.Forms.Button();
            this.FlashWriteToFileButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // COMPortComboBox
            // 
            this.COMPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMPortComboBox.Enabled = false;
            this.COMPortComboBox.FormattingEnabled = true;
            this.COMPortComboBox.Location = new System.Drawing.Point(117, 48);
            this.COMPortComboBox.Name = "COMPortComboBox";
            this.COMPortComboBox.Size = new System.Drawing.Size(182, 33);
            this.COMPortComboBox.TabIndex = 0;
            // 
            // COMPortListRefreshButton
            // 
            this.COMPortListRefreshButton.Enabled = false;
            this.COMPortListRefreshButton.Location = new System.Drawing.Point(305, 48);
            this.COMPortListRefreshButton.Name = "COMPortListRefreshButton";
            this.COMPortListRefreshButton.Size = new System.Drawing.Size(112, 34);
            this.COMPortListRefreshButton.TabIndex = 1;
            this.COMPortListRefreshButton.Text = "Refresh";
            this.COMPortListRefreshButton.UseVisualStyleBackColor = true;
            this.COMPortListRefreshButton.Click += new System.EventHandler(this.refreshCOMPortListButton_Click);
            // 
            // COMPortConnectButton
            // 
            this.COMPortConnectButton.Enabled = false;
            this.COMPortConnectButton.Location = new System.Drawing.Point(423, 48);
            this.COMPortConnectButton.Name = "COMPortConnectButton";
            this.COMPortConnectButton.Size = new System.Drawing.Size(112, 34);
            this.COMPortConnectButton.TabIndex = 2;
            this.COMPortConnectButton.Text = "Connect";
            this.COMPortConnectButton.UseVisualStyleBackColor = true;
            this.COMPortConnectButton.Click += new System.EventHandler(this.COMPortConnectButton_Click);
            // 
            // serialPortTimer
            // 
            this.serialPortTimer.Interval = 1000;
            this.serialPortTimer.Tick += new System.EventHandler(this.serialPortTimer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SerialConnectionStatusBarLabel,
            this.commandProgressUpdateBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 657);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(894, 32);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // SerialConnectionStatusBarLabel
            // 
            this.SerialConnectionStatusBarLabel.AutoSize = false;
            this.SerialConnectionStatusBarLabel.Name = "SerialConnectionStatusBarLabel";
            this.SerialConnectionStatusBarLabel.Size = new System.Drawing.Size(350, 25);
            this.SerialConnectionStatusBarLabel.Text = "COM Port Disconnected";
            this.SerialConnectionStatusBarLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // commandProgressUpdateBar
            // 
            this.commandProgressUpdateBar.Name = "commandProgressUpdateBar";
            this.commandProgressUpdateBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.commandProgressUpdateBar.Size = new System.Drawing.Size(500, 24);
            this.commandProgressUpdateBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 30);
            this.label1.TabIndex = 4;
            this.label1.Text = "Device";
            // 
            // DeviceNameLabel
            // 
            this.DeviceNameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DeviceNameLabel.Location = new System.Drawing.Point(176, 37);
            this.DeviceNameLabel.Name = "DeviceNameLabel";
            this.DeviceNameLabel.Size = new System.Drawing.Size(240, 30);
            this.DeviceNameLabel.TabIndex = 5;
            this.DeviceNameLabel.Text = "DeviceName";
            // 
            // GetDeviceInfoButton
            // 
            this.GetDeviceInfoButton.Location = new System.Drawing.Point(6, 197);
            this.GetDeviceInfoButton.Name = "GetDeviceInfoButton";
            this.GetDeviceInfoButton.Size = new System.Drawing.Size(156, 34);
            this.GetDeviceInfoButton.TabIndex = 6;
            this.GetDeviceInfoButton.Text = "Read";
            this.GetDeviceInfoButton.UseVisualStyleBackColor = true;
            this.GetDeviceInfoButton.Click += new System.EventHandler(this.GetDeviceInfoButton_Click);
            // 
            // HardwareVersionLabel
            // 
            this.HardwareVersionLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.HardwareVersionLabel.Location = new System.Drawing.Point(608, 37);
            this.HardwareVersionLabel.Name = "HardwareVersionLabel";
            this.HardwareVersionLabel.Size = new System.Drawing.Size(240, 30);
            this.HardwareVersionLabel.TabIndex = 8;
            this.HardwareVersionLabel.Text = "HWVersion";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(430, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(160, 25);
            this.label4.TabIndex = 7;
            this.label4.Text = "Hardware Version";
            // 
            // SoftwareMinorLabel
            // 
            this.SoftwareMinorLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SoftwareMinorLabel.Location = new System.Drawing.Point(608, 77);
            this.SoftwareMinorLabel.Name = "SoftwareMinorLabel";
            this.SoftwareMinorLabel.Size = new System.Drawing.Size(240, 30);
            this.SoftwareMinorLabel.TabIndex = 12;
            this.SoftwareMinorLabel.Text = "SWMinor";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(430, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(160, 25);
            this.label6.TabIndex = 11;
            this.label6.Text = "Software Minor";
            // 
            // SoftwareMajorLabel
            // 
            this.SoftwareMajorLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SoftwareMajorLabel.Location = new System.Drawing.Point(176, 77);
            this.SoftwareMajorLabel.Name = "SoftwareMajorLabel";
            this.SoftwareMajorLabel.Size = new System.Drawing.Size(240, 30);
            this.SoftwareMajorLabel.TabIndex = 10;
            this.SoftwareMajorLabel.Text = "SWMajor";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(160, 25);
            this.label8.TabIndex = 9;
            this.label8.Text = "Software Major";
            // 
            // COMPortLabel
            // 
            this.COMPortLabel.AutoSize = true;
            this.COMPortLabel.Location = new System.Drawing.Point(18, 51);
            this.COMPortLabel.Name = "COMPortLabel";
            this.COMPortLabel.Size = new System.Drawing.Size(90, 25);
            this.COMPortLabel.TabIndex = 13;
            this.COMPortLabel.Text = "COM Port";
            // 
            // FlashSizeBytesLabel
            // 
            this.FlashSizeBytesLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.FlashSizeBytesLabel.Location = new System.Drawing.Point(176, 117);
            this.FlashSizeBytesLabel.Name = "FlashSizeBytesLabel";
            this.FlashSizeBytesLabel.Size = new System.Drawing.Size(240, 30);
            this.FlashSizeBytesLabel.TabIndex = 15;
            this.FlashSizeBytesLabel.Text = "FlashSize";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 25);
            this.label3.TabIndex = 14;
            this.label3.Text = "Flash Size (B)";
            // 
            // AppMemSizeLabel
            // 
            this.AppMemSizeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AppMemSizeLabel.Location = new System.Drawing.Point(608, 117);
            this.AppMemSizeLabel.Name = "AppMemSizeLabel";
            this.AppMemSizeLabel.Size = new System.Drawing.Size(240, 30);
            this.AppMemSizeLabel.TabIndex = 17;
            this.AppMemSizeLabel.Text = "AppMemSize";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(430, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 25);
            this.label5.TabIndex = 16;
            this.label5.Text = "App Mem Size (B)";
            // 
            // BootMemSizeLabel
            // 
            this.BootMemSizeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BootMemSizeLabel.Location = new System.Drawing.Point(176, 157);
            this.BootMemSizeLabel.Name = "BootMemSizeLabel";
            this.BootMemSizeLabel.Size = new System.Drawing.Size(240, 30);
            this.BootMemSizeLabel.TabIndex = 19;
            this.BootMemSizeLabel.Text = "BootMemSize";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(6, 157);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(160, 25);
            this.label9.TabIndex = 18;
            this.label9.Text = "Boot Mem Size (B)";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(894, 33);
            this.menuStrip1.TabIndex = 20;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(55, 29);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // FlashReadFilePathLabel
            // 
            this.FlashReadFilePathLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.FlashReadFilePathLabel.Location = new System.Drawing.Point(172, 77);
            this.FlashReadFilePathLabel.Name = "FlashReadFilePathLabel";
            this.FlashReadFilePathLabel.Size = new System.Drawing.Size(549, 30);
            this.FlashReadFilePathLabel.TabIndex = 22;
            this.FlashReadFilePathLabel.TextChanged += new System.EventHandler(this.FlashReadFilePathLabel_TextChanged);
            // 
            // FlashReadFilePathButton
            // 
            this.FlashReadFilePathButton.Location = new System.Drawing.Point(727, 77);
            this.FlashReadFilePathButton.Name = "FlashReadFilePathButton";
            this.FlashReadFilePathButton.Size = new System.Drawing.Size(42, 34);
            this.FlashReadFilePathButton.TabIndex = 23;
            this.FlashReadFilePathButton.Text = "...";
            this.FlashReadFilePathButton.UseVisualStyleBackColor = true;
            this.FlashReadFilePathButton.Click += new System.EventHandler(this.FlashReadFilePathButton_Click);
            // 
            // readFlashImageToFileSaveDialog
            // 
            this.readFlashImageToFileSaveDialog.CreatePrompt = true;
            this.readFlashImageToFileSaveDialog.DefaultExt = "bin";
            this.readFlashImageToFileSaveDialog.FileName = "flash";
            this.readFlashImageToFileSaveDialog.Filter = "BIN files (*.bin)|*.bin";
            this.readFlashImageToFileSaveDialog.Title = "Select filename to write flash image";
            // 
            // FlashReadButton
            // 
            this.FlashReadButton.Enabled = false;
            this.FlashReadButton.Location = new System.Drawing.Point(6, 37);
            this.FlashReadButton.Name = "FlashReadButton";
            this.FlashReadButton.Size = new System.Drawing.Size(156, 34);
            this.FlashReadButton.TabIndex = 24;
            this.FlashReadButton.Text = "Read to buffer";
            this.FlashReadButton.UseVisualStyleBackColor = true;
            this.FlashReadButton.Click += new System.EventHandler(this.FlashReadButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.DeviceNameLabel);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.HardwareVersionLabel);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.BootMemSizeLabel);
            this.groupBox1.Controls.Add(this.GetDeviceInfoButton);
            this.groupBox1.Controls.Add(this.SoftwareMajorLabel);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.AppMemSizeLabel);
            this.groupBox1.Controls.Add(this.SoftwareMinorLabel);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.FlashSizeBytesLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 100);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(865, 246);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Info";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BootloaderWriteToFileButton);
            this.groupBox2.Controls.Add(this.BootloaderReadFilePathLabel);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.BootloaderReadFilePathButton);
            this.groupBox2.Controls.Add(this.ApplicationWriteToFileButton);
            this.groupBox2.Controls.Add(this.ApplicationReadFilePathLabel);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.ApplicationReadFilePathButton);
            this.groupBox2.Controls.Add(this.FlashWriteToFileButton);
            this.groupBox2.Controls.Add(this.FlashReadFilePathLabel);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.FlashReadButton);
            this.groupBox2.Controls.Add(this.FlashReadFilePathButton);
            this.groupBox2.Location = new System.Drawing.Point(12, 364);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(865, 215);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Write flash images to files";
            // 
            // BootloaderWriteToFileButton
            // 
            this.BootloaderWriteToFileButton.Enabled = false;
            this.BootloaderWriteToFileButton.Location = new System.Drawing.Point(773, 157);
            this.BootloaderWriteToFileButton.Name = "BootloaderWriteToFileButton";
            this.BootloaderWriteToFileButton.Size = new System.Drawing.Size(67, 34);
            this.BootloaderWriteToFileButton.TabIndex = 33;
            this.BootloaderWriteToFileButton.Text = "Write";
            this.BootloaderWriteToFileButton.UseVisualStyleBackColor = true;
            this.BootloaderWriteToFileButton.Click += new System.EventHandler(this.BootloaderWriteToFileButton_Click);
            // 
            // BootloaderReadFilePathLabel
            // 
            this.BootloaderReadFilePathLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BootloaderReadFilePathLabel.Location = new System.Drawing.Point(172, 157);
            this.BootloaderReadFilePathLabel.Name = "BootloaderReadFilePathLabel";
            this.BootloaderReadFilePathLabel.Size = new System.Drawing.Size(549, 30);
            this.BootloaderReadFilePathLabel.TabIndex = 31;
            this.BootloaderReadFilePathLabel.TextChanged += new System.EventHandler(this.BootloaderReadFilePathLabel_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 157);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(141, 25);
            this.label12.TabIndex = 30;
            this.label12.Text = "Bootloader path";
            // 
            // BootloaderReadFilePathButton
            // 
            this.BootloaderReadFilePathButton.Location = new System.Drawing.Point(727, 157);
            this.BootloaderReadFilePathButton.Name = "BootloaderReadFilePathButton";
            this.BootloaderReadFilePathButton.Size = new System.Drawing.Size(42, 34);
            this.BootloaderReadFilePathButton.TabIndex = 32;
            this.BootloaderReadFilePathButton.Text = "...";
            this.BootloaderReadFilePathButton.UseVisualStyleBackColor = true;
            this.BootloaderReadFilePathButton.Click += new System.EventHandler(this.BootloaderReadFilePathButton_Click);
            // 
            // ApplicationWriteToFileButton
            // 
            this.ApplicationWriteToFileButton.Enabled = false;
            this.ApplicationWriteToFileButton.Location = new System.Drawing.Point(773, 117);
            this.ApplicationWriteToFileButton.Name = "ApplicationWriteToFileButton";
            this.ApplicationWriteToFileButton.Size = new System.Drawing.Size(67, 34);
            this.ApplicationWriteToFileButton.TabIndex = 29;
            this.ApplicationWriteToFileButton.Text = "Write";
            this.ApplicationWriteToFileButton.UseVisualStyleBackColor = true;
            this.ApplicationWriteToFileButton.Click += new System.EventHandler(this.ApplicationWriteToFileButton_Click);
            // 
            // ApplicationReadFilePathLabel
            // 
            this.ApplicationReadFilePathLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ApplicationReadFilePathLabel.Location = new System.Drawing.Point(172, 117);
            this.ApplicationReadFilePathLabel.Name = "ApplicationReadFilePathLabel";
            this.ApplicationReadFilePathLabel.Size = new System.Drawing.Size(549, 30);
            this.ApplicationReadFilePathLabel.TabIndex = 27;
            this.ApplicationReadFilePathLabel.TextChanged += new System.EventHandler(this.ApplicationReadFilePathLabel_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 117);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(143, 25);
            this.label10.TabIndex = 26;
            this.label10.Text = "Application path";
            // 
            // ApplicationReadFilePathButton
            // 
            this.ApplicationReadFilePathButton.Location = new System.Drawing.Point(727, 117);
            this.ApplicationReadFilePathButton.Name = "ApplicationReadFilePathButton";
            this.ApplicationReadFilePathButton.Size = new System.Drawing.Size(42, 34);
            this.ApplicationReadFilePathButton.TabIndex = 28;
            this.ApplicationReadFilePathButton.Text = "...";
            this.ApplicationReadFilePathButton.UseVisualStyleBackColor = true;
            this.ApplicationReadFilePathButton.Click += new System.EventHandler(this.ApplicationReadFilePathButton_Click);
            // 
            // FlashWriteToFileButton
            // 
            this.FlashWriteToFileButton.Enabled = false;
            this.FlashWriteToFileButton.Location = new System.Drawing.Point(773, 77);
            this.FlashWriteToFileButton.Name = "FlashWriteToFileButton";
            this.FlashWriteToFileButton.Size = new System.Drawing.Size(67, 34);
            this.FlashWriteToFileButton.TabIndex = 25;
            this.FlashWriteToFileButton.Text = "Write";
            this.FlashWriteToFileButton.UseVisualStyleBackColor = true;
            this.FlashWriteToFileButton.Click += new System.EventHandler(this.FlashWriteToFileButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 25);
            this.label2.TabIndex = 21;
            this.label2.Text = "Flash path";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 689);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.COMPortLabel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.COMPortConnectButton);
            this.Controls.Add(this.COMPortListRefreshButton);
            this.Controls.Add(this.COMPortComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "AVRManager GUI";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComboBox COMPortComboBox;
        private Button COMPortListRefreshButton;
        private Button COMPortConnectButton;
        private System.Windows.Forms.Timer serialPortTimer;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel SerialConnectionStatusBarLabel;
        private Label label1;
        private Label DeviceNameLabel;
        private Button GetDeviceInfoButton;
        private Label HardwareVersionLabel;
        private Label label4;
        private Label SoftwareMinorLabel;
        private Label label6;
        private Label SoftwareMajorLabel;
        private Label label8;
        private Label COMPortLabel;
        private Label FlashSizeBytesLabel;
        private Label label3;
        private Label AppMemSizeLabel;
        private Label label5;
        private Label BootMemSizeLabel;
        private Label label9;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripProgressBar commandProgressUpdateBar;
        private Label FlashReadFilePathLabel;
        private Button FlashReadFilePathButton;
        private SaveFileDialog readFlashImageToFileSaveDialog;
        private ToolTip ReadFlashFilenameToolTip;
        private Button FlashReadButton;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label2;
        private Button BootloaderWriteToFileButton;
        private Label BootloaderReadFilePathLabel;
        private Label label12;
        private Button BootloaderReadFilePathButton;
        private Button ApplicationWriteToFileButton;
        private Label ApplicationReadFilePathLabel;
        private Label label10;
        private Button ApplicationReadFilePathButton;
        private Button FlashWriteToFileButton;
    }
}