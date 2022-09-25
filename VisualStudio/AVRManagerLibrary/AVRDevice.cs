using System.Diagnostics;
using System.IO;

namespace AVRManagerLibrary
{
    public class AVRDevice
    {
        public bool isValid = false;
        public byte[] Signature = new byte[3];
        public string DeviceName = "";
        public UInt16 HardwareVersion;
        public UInt16 SoftwareMajor;
        public UInt16 SoftwareMinor;
        public byte FuseLow;
        public byte FuseHigh;
        public byte FuseExt;
        public UInt16 LastLoadedAddress;

        public UInt16 FlashSizeInPages;
        public UInt16 ApplicationSizeInPages;
        public UInt16 BootloaderSizeInPages;

        public UInt16 FlashSizeInWords;
        public UInt16 ApplicationSizeInWords;
        public UInt16 BootloaderSizeInWords;

        public UInt16 FlashSizeInBytes;
        public UInt16 ApplicationSizeInBytes;
        public UInt16 BootloaderSizeInBytes;

        public UInt16 FlashWordStartAddress;
        public UInt16 FlashWordEndAddress;
        public UInt16 ApplicationWordStartAddress;
        public UInt16 ApplicationWordEndAddress;
        public UInt16 BootloaderWordStartAddress;
        public UInt16 BootloaderWordEndAddresss;

        public UInt16 FlashByteStartAddress;
        public UInt16 FlashByteEndAddress;
        public UInt16 ApplicationByteStartAddress;
        public UInt16 ApplicationByteEndAddress;
        public UInt16 BootloaderByteStartAddress;
        public UInt16 BootloaderByteEndAddress;

        public byte[] WholeFlashBinImageReadFromDeviceFileBytes;
        public byte[] UserApplicationBinImageWriteToDeviceFileBytes;

        public string WholeFlashBinImageReadFromDeviceFilePath = Directory.GetCurrentDirectory() + @"\flash.bin";
        public string ApplicationBinImageReadFromDeviceFilePath = Directory.GetCurrentDirectory() + @"\app.bin";
        public string BootloaderBinImageReadFromDeviceFilePath = Directory.GetCurrentDirectory() + @"\boot.bin";
        public string UserApplicationBinImageWriteToDeviceFilePath = Directory.GetCurrentDirectory() + @"\program.bin";


        public UInt16 Progress = 0; // Helper var to denote progress of command, should be moved inside STK500

        public void DeviceSignature(byte sign0, byte sign1, byte sign2)
        {
            // 1E 95 0F
            if (!(sign0 == 0x1E && sign1 == 0x95 && sign2 == 0x0F))
                throw new Exception("Device not supported");
            Signature[0] = sign0;
            Signature[1] = sign1;
            Signature[2] = sign2;
            DeviceName = "ATMega328p";
            FlashSizeInPages = 256;
            FlashSizeInWords = (UInt16)(64 * FlashSizeInPages);
            FlashSizeInBytes = (UInt16)(FlashSizeInWords * 2);
            WholeFlashBinImageReadFromDeviceFileBytes = new byte[FlashSizeInBytes];
        }

        public void DeviceFuses(byte fuseLow, byte fuseHigh, byte fuseExt)
        {
            UInt16 bootsz = (UInt16)((fuseHigh & (byte)FuseHighBits.BOOTSZ) >> (int)FuseHighBits.BOOTSZ_POS);
            FuseLow = fuseLow;
            FuseHigh = fuseHigh;
            FuseExt = fuseExt;
            Debug.WriteLine($"fuseLow: {0}, fuseHigh: {0}, fuseExt: {0}", fuseLow.ToString("X2"), fuseHigh, fuseExt);
            switch (bootsz)
            {
                case 3:

                    BootloaderSizeInWords = 256;
                    BootloaderSizeInPages = 4;
                    BootloaderWordStartAddress = 0x3F00;
                    ApplicationWordEndAddress = 0x3EFF;
                    break;
                case 2:
                    BootloaderSizeInWords = 512;
                    BootloaderSizeInPages = 8;
                    BootloaderWordStartAddress = 0x3E00;
                    ApplicationWordEndAddress = 0x3DFF;
                    break;
                case 1:
                    BootloaderSizeInWords = 1024;
                    BootloaderSizeInPages = 16;
                    BootloaderWordStartAddress = 0x3C00;
                    ApplicationWordEndAddress = 0x3BFF;
                    break;
                case 0:
                    BootloaderSizeInWords = 2048;
                    BootloaderSizeInPages = 32;
                    BootloaderWordStartAddress = 0x3800;
                    ApplicationWordEndAddress = 0x37FF;
                    break;
                default:
                    throw new Exception("Not valid boot fuse");
                    break;
            }

            BootloaderWordEndAddresss = 0x3FFF;
            ApplicationWordStartAddress = 0x0000;
            BootloaderSizeInBytes = (UInt16)(BootloaderSizeInWords * 2);
            BootloaderByteStartAddress = (UInt16)(BootloaderWordStartAddress * 2);
            BootloaderByteEndAddress = (UInt16)(BootloaderWordEndAddresss * 2);
            ApplicationSizeInWords = (UInt16)(FlashSizeInWords - BootloaderSizeInWords);
            ApplicationSizeInPages = (UInt16)(FlashSizeInPages - BootloaderSizeInPages);
            ApplicationSizeInBytes = (UInt16)(FlashSizeInBytes - BootloaderSizeInBytes);
            ApplicationByteStartAddress = (UInt16)(ApplicationWordStartAddress * 2);
            ApplicationByteEndAddress = (UInt16)(ApplicationWordEndAddress * 2);
            UserApplicationBinImageWriteToDeviceFileBytes = new byte[ApplicationSizeInBytes];
        }

        private enum FuseLowBits
        {
            SUT_CKSEL = 0x3F,
            SUT_CKSEL_POS = 0,
            CKOUT = 0x40,
            CKOUT_POS = 6,
            CKDIV8 = 0x80,
            CKDIV8_POS = 7
        }

        private enum FuseHighBits
        {
            BOOTRST = 0x01,
            BOOTRST_POS = 0,
            BOOTSZ = 0x06,
            BOOTSZ_POS = 1,
            EESAVE = 0x40,
            EESAVE_POS = 3,
            WDTON = 0x10,
            WDTON_POS = 4,
            SPIEN = 0x20,
            SPIEN_POS = 5,
            DWEN = 0x40,
            DWEN_POS = 6,
            RSTDISBL = 0x80,
            RSTDISBL_POS = 7
        }

        private enum FuseExtBits
        {
            BODLEVEL = 0x07,
            BODLEVEL_POS = 0,
        }
    }
}