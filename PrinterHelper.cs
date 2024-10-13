using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;





namespace VPrinterMonitor
{
 
    public class PrinterHelper
    {

         [StructLayout(LayoutKind.Sequential)]
    public struct DOC_INFO_1
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDocName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pOutputFile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDataType;
    }

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool OpenPrinter(string src, out IntPtr hPrinter, IntPtr pd);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOC_INFO_1 di);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool WritePrinter(IntPtr hPrinter, IntPtr buffer, int buf, out int pcWritten);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int GetLastError();




        /*[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public  struct DOC_INFO_1
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDataType;
        }


        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern bool ClosePrinter(IntPtr hPrinter);


        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int Level, ref DOC_INFO_1 pDocInfo);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);



        */
        public static void AddPrintJob22(string printerName, string documentName, string xpsFilePath)
        {
            
        }


        public static void AddPrintJob(string printerName, string documentName, string xpsFilePath)
        {
            IntPtr hPrinter;
            if (OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
            {
                DOC_INFO_1 docInfo = new DOC_INFO_1
                {
                    pDocName = documentName,
                    pDataType = "RAW",
                    pOutputFile = null
                };

                if (StartDocPrinter(hPrinter, 1, ref docInfo))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        byte[] fileBytes = File.ReadAllBytes(xpsFilePath);
                        IntPtr unmanagedBytes = Marshal.AllocHGlobal(fileBytes.Length);
                        Marshal.Copy(fileBytes, 0, unmanagedBytes, fileBytes.Length);

                        int bytesWritten;
                        if (!WritePrinter(hPrinter, unmanagedBytes, fileBytes.Length, out bytesWritten))
                        {
                            int errorCode = Marshal.GetLastWin32Error();
                            Console.WriteLine($"Failed to open printer. Error code: {errorCode}");
                        }


                        EndPagePrinter(hPrinter);

                        Marshal.FreeHGlobal(unmanagedBytes);
                    }
                    EndDocPrinter(hPrinter);
                }
                else
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Failed to open printer. Error code: {errorCode}");
                }

                ClosePrinter(hPrinter);
            }
            else
            {
                int errorCode = Marshal.GetLastWin32Error();
                Console.WriteLine($"Failed to open printer. Error code: {errorCode}");
            }
        }









        public static void AddPrintJob2(string printerName, string documentName , string FilePath)
        {
            string error = "";
            try
            {
                IntPtr hPrinter;
                if (OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
                {
                    DOC_INFO_1 docInfo = new DOC_INFO_1
                    {
                        pDocName = documentName,
                        pDataType = "XPS_PASS", // Set the document type to XPS
                        pOutputFile = FilePath // Set the path to the XPS file
                    };
                    if ( StartDocPrinter(hPrinter, 1, ref docInfo))
                    {
                        // Add your printing logic here
                        EndDocPrinter(hPrinter);
                    }
                    else
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        Console.WriteLine($"Failed to start document print. Error code: {errorCode}");
                    }

                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }
    }
}

