using Microsoft.VisualBasic.Logging;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Printing;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.Design.Behavior;
using System.Xml;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Configuration;
using System.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
//using static Azure.Core.HttpHeader;
using System.Windows.Forms;
using System.Data;
using System.Security.Principal;
using System.Threading;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Reflection.Metadata;
using Microsoft.VisualBasic.Devices;
using RasheedTag;
using Terminal;
using System;




namespace VPrinterMonitor
{

    public partial class Terminal : Form
    {
        string FileToReadOrPrint = "";
        string FileToReadOrPrintXML = "";
        string outputFileJsonPath = "";
        string sPrinterName = "";
        static int CurrPageNum = 0;
        string XMLPrinterPath = "";
        string configFilePath = "";
        static string FullJsonStr = "";
        static  public int TerminalID = 0;

        PrintDocument PrintDoc = new PrintDocument();

        private ProgressForm progressForm;

        string messageToShow = "";
        string messageTitel = "";
        string messageType = "";


        private BackgroundWorker RasheedBGWorker;
        private readonly object _lock = new object();


        private static readonly object CreditLock = new object();
        Stopwatch stopwatch = new Stopwatch();

        string privateKey = "";

        private UseRasheedTag URasheedTag;
        private Tag TagObj;


        public Terminal()
        {
            InitializeComponent();


        }

        private void PrintersCBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PrintersCBox.SelectedIndex != -1)
            {
                sPrinterName = PrintersCBox.Text;

            }
        }

        public void PrintXpsFile(string xpsFilePath, string printerName)
        {

            //PrinterHelper.AddPrintJob(printerName, " MyDoc", xpsFilePath);
            //string command = @"g:\Program Files\XML Printer\xmlprn.exe";

            // string arguments = @"/reprint  '" + xpsFilePath + "'  '" + printerName + "'";


            string command = XMLPrinterPath;
            string arguments = $"/reprint \"{xpsFilePath}\" \"{printerName}\"";



            RunCommandSilently(command, arguments);
        }
        private void SendBtn_Click(object sender, EventArgs e)
        {
            sPrinterName = PrintersCBox.Text;
            PhysicalPWithoutInterface();

            

        }

        static string ReadRegistryValue(string ConfigkeyPath, string valueName)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(ConfigkeyPath))
                {
                    if (key != null)
                    {

                        object o = key.GetValue(valueName);
                        if (o != null)
                        {
                            return o.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading registry: {ex.Message}");
            }

            return "";
        }

        private void RashidPWithoutInterface()
        {

            CheckBalanceAndCreateJson();

        }
        private void PhysicalPWithoutInterface()
        {
            try
            {

                if (FileToReadOrPrint == "" || !FileToReadOrPrint.EndsWith("xps", StringComparison.OrdinalIgnoreCase))
                {
                    //FileToReadOrPrint = "D:\\xmloutput\\0025.xps";
                    MessageBox.Show("There is no file to print", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //   MessageBox.Show(FileToReadOrPrint);
                LogInFile("Print xps  :: " + FileToReadOrPrint + " With :: " + PrintersCBox.Text);
                if (sPrinterName == "")
                {
                    if (Configuration.DefaultPhyPrinter == "")
                    {
                        MessageBox.Show("Please choose a physical printer first.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        sPrinterName = Configuration.DefaultPhyPrinter;
                    }
                }

                PrintXpsFile(FileToReadOrPrint, sPrinterName);


            }
            catch { }




        }
        private void BothPWithoutInterface()
        {
            PhysicalPWithoutInterface();
            CheckBalanceAndCreateJson();

        }
        private void BothPWithInterface()
        {

            ClientSize = new Size(573, 180);
            FillPrintersCombo();


        }
        public void CloseRunningInstance()
        {
            try
            {
                // Get the current process name
                string currentProcessName = Process.GetCurrentProcess().ProcessName;

                //  MessageBox.Show("currentProcessName" + currentProcessName + "with ID ::" + Process.GetCurrentProcess().Id);

                // Get all processes with the same name
                Process[] processes = Process.GetProcessesByName(currentProcessName);

                // Terminate all other instances of the application
                foreach (Process process in processes)
                {
                    //     MessageBox.Show(process.ProcessName + "with ID::: " + process.Id.ToString());
                    // Ensure not to terminate the current process
                    if (process.Id != Process.GetCurrentProcess().Id)
                    {
                        //MessageBox.Show(" will Kill this ID::: " + process.Id.ToString());
                        process.Kill(); // Terminate the process
                    }
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show(ex.Message.ToString());

            }

        }
        static bool IsRunningAsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void RunAsAdministrator(string arguments)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = Application.ExecutablePath, // Path to your executable
                Arguments = arguments,
                UseShellExecute = true,
                Verb = "runas" // Causes the process to run with elevated privileges
            };

            try
            {
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to restart the application with administrator privileges: {ex.Message}");
            }

            // Close the current instance of the application
            Application.Exit();
        }

        private void Terminal_Load(object sender, EventArgs e)
        {
            Random random = new Random();
            TerminalID = random.Next(0, 10);
            try
            {
                // MessageBox.Show("Test1 from terminal # " + x.ToString());

#if !DEBUG
                // This code will only run in Release mode
                if (!IsRunningAsAdministrator())
                {

                  //  MessageBox.Show("Test2");
                    string[] args2 = Environment.GetCommandLineArgs();
                    string arguments = string.Join(" ", args2.Skip(1).Select(arg => $"\"{arg}\""));


                  //  MessageBox.Show("arguments");

                    // Restart the application with elevated privileges
                    RunAsAdministrator(arguments);
                  //  MessageBox.Show("Test3");
                    this.Close();
                }

#endif
            }
            catch (Exception ex)
            {
                
              //  MessageBox.Show(ex.ToString() +" terminal # " + x.ToString());
            }



            // Close any running instance of the app
            //MessageBox.Show("Test4");
            CloseRunningInstance();

           // MessageBox.Show("Test2 from terminal # " + x.ToString());
            //////// BackgroundWorker to run Rasheed Tag
            //RasheedBGWorker = new BackgroundWorker();
            //RasheedBGWorker.WorkerReportsProgress = true;
            //RasheedBGWorker.WorkerSupportsCancellation = true;
            //RasheedBGWorker.DoWork += new DoWorkEventHandler(RasheedBGWorker_DoWork);
            //RasheedBGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RasheedBGWorker_RunWorkerCompleted);


            // MessageBox.Show("Test5");
            string registryAppKey = @"SOFTWARE\Rashid";
            string registryCPName = "ConfigPath";
            string registryPPValue = "PrinterPath";


            configFilePath = ReadRegistryValue(registryAppKey, registryCPName);
            XMLPrinterPath = ReadRegistryValue(registryAppKey, registryPPValue);

         //   MessageBox.Show("Test3 from terminal # " + x.ToString());
            if (string.IsNullOrEmpty(configFilePath))
            {
           //     MessageBox.Show("Configuration file path not found in the registry. from terminal # " + x.ToString());
                Console.WriteLine("Configuration file path not found in the registry.");
                return;
            }

            //MessageBox.Show("Test4 from terminal # " + x.ToString());
            //MessageBox.Show(configFilePath + "from terminal # " + x.ToString());
            Configuration.LoadFromXml(configFilePath);

            //MessageBox.Show("Test5 from terminal # " + x.ToString());


            LogInFile("Load Config file :: " + configFilePath);

            if (Configuration.XMLFolder == "")
            {
                MessageBox.Show("Please prepare the setting from PrinterSettingsTool First", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Close();

            }


            string[] args = Environment.GetCommandLineArgs();

            int index = args.Length;

            if (index > 1)
                if (args[1] != null)
                {
                    FileToReadOrPrint = args[1];
                    if (FileToReadOrPrint != "")
                    {
                        LogInFile("Start New Print with File :: " + FileToReadOrPrint);
              //          MessageBox.Show("Start New Print with File :: " + FileToReadOrPrint + " from terminal # " + x.ToString());
                        CheckXPSFileCreateXML(FileToReadOrPrint);

                        LogInFile("CreateXML File :: " + FileToReadOrPrintXML);

                //        MessageBox.Show("CreateXML File:: " + FileToReadOrPrintXML + " from terminal # " + x.ToString());

                    }

                }






            if (FileToReadOrPrint == "")
            {

#if !DEBUG
                //    MessageBox.Show("There is no file to print from terminal # " + x.ToString(), "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();

#endif

#if DEBUG
                FileToReadOrPrint = Configuration.XMLFolder + "\\tt0006.xps";
              //  MessageBox.Show(FileToReadOrPrint);
                CheckXPSFileCreateXML(FileToReadOrPrint);





#endif




            }



            string DbRet = BillsChainLite.CreateDatabase(configFilePath);
            LogInFile(DbRet);

            string selectedPrinter = Configuration.selectedPrinter;
            switch (selectedPrinter)
            {
                case "RashidPrinter":
                    {
                        LogInFile("Case RashidPrinter WithoutInterface ");
                        RashidPWithoutInterface();
                        this.Hide();
                        this.Visible = false;
                        break;
                    }
                case "PhysicalPrinter":
                    {
                        LogInFile("Case PhysicalPrinter WithoutInterface ");
                        PhysicalPWithoutInterface();
                        this.Close();
                        break;
                    }
                case "BothPrinter":
                    {
                        LogInFile("Case Both Printers WithoutInterface ");
                        BothPWithoutInterface();
                        this.Hide();
                        this.Visible = false;
                        break;
                        
                    }
                case "ChoosingBetweenPrinters":
                default:
                    {
                        LogInFile("Case Both Printers With Interface ");

                        BothPWithInterface();
                        break;
                    }
            }

        }
        private void CheckXPSFileCreateXML(string InputFileName)
        {

            if (InputFileName.EndsWith("xps", StringComparison.OrdinalIgnoreCase))
            {
                FileToReadOrPrintXML = InputFileName.Replace(".xps", ".xml");

                //  XMLPrinterPath =  XMLPrinterPath.TrimEnd();
                string command = XMLPrinterPath;
                //string arguments = @"/convert  " + InputFileName + " " + FileToReadOrPrintXML;
                string arguments = @"/convert " + " \"" + InputFileName + "\"  \"" + FileToReadOrPrintXML + "\" ";

                //  MessageBox.Show(arguments);


                RunCommandSilently(command, arguments);

                LogInFile("Convert xps to xml File :: " + FileToReadOrPrintXML);
            }
        }
        public static void RunCommandSilently(string command, string arguments)
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = true, // Set to true to use the OS shell
                    Verb = "runas", // This will prompt for elevation
                    RedirectStandardOutput = false, // No need to redirect output if using runas
                    RedirectStandardError = false
                };

                using (Process process = new Process())
                {
                    process.StartInfo = processInfo;
                    process.Start();

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void FillPrintersCombo()
        {
            // Add list of installed printers found to the combo box.
            // The pkInstalledPrinters string will be used to provide the display string.
            string pkInstalledPrinters;
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                pkInstalledPrinters = PrinterSettings.InstalledPrinters[i];
                PrintersCBox.Items.Add(pkInstalledPrinters);
            }
            PrintersCBox.SelectedItem = Configuration.DefaultPhyPrinter;
        }

        private void LogInFile(string LogStr)
        {
            if (string.IsNullOrWhiteSpace(LogStr))
            {
                return;
            }

            try
            {
                // Append the log entry to the file
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} _ TerminalID-{TerminalID.ToString()}:: {LogStr}{Environment.NewLine}";
                string logFilePath = Configuration.XMLFolder + "\\LogFile.txt";

                // Ensure the directory exists
                string directory = Path.GetDirectoryName(logFilePath);
                if (Directory.Exists(directory))
                {
                    // Open the file with FileStream and allow multiple processes to write simultaneously
                    using (FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(logEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Logging failed: {ex.Message}");
                return;
            }
        }


        private void DBalance_UpdateDB()
        {

            CreateEKeys();

            ////////////////////////
            ///

            EBalance();

            /////////////////////////////////////
            // Load the private key from file
            //string privateKey = File.ReadAllText("privateKey.xml");

            // Load the encrypted data from file
            byte[] encryptedData = File.ReadAllBytes("encryptedData.bin");

            byte[] decryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Load the private key
                if (privateKey == "")
                    return;
                rsa.FromXmlString(privateKey);

                // Decrypt the data
                decryptedData = rsa.Decrypt(encryptedData, false);
            }

            // Convert the decrypted data to a string
            string decryptedText = Encoding.UTF8.GetString(decryptedData);

            Console.WriteLine("Decrypted data: " + decryptedText);

            int NewBalance = int.Parse(decryptedText);
            BillsChainLite.UpdateNewBalance(NewBalance);

        }
        private void DBalance_UpdateDB_Click(object sender, EventArgs e)
        {
            DBalance_UpdateDB();

        }

        private void CreateEKeys()
        {
            // Generate a new RSA key pair
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                // Extract the public and private keys
                string publicKey = rsa.ToXmlString(false); // Public key
                privateKey = rsa.ToXmlString(true); // Private key

                // Save the keys to files
                File.WriteAllText("publicKey.xml", publicKey);
                File.WriteAllText("privateKey.xml", privateKey);

                Console.WriteLine("Keys generated and saved to files.");
            }

        }
        private void EBalance()
        {
            // Load the public key from file
            string publicKey = File.ReadAllText("publicKey.xml");

            // Data to be encrypted
            string data = "1000";
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(data);

            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Load the public key
                rsa.FromXmlString(publicKey);

                // Encrypt the data
                encryptedData = rsa.Encrypt(dataToEncrypt, false);
            }

            // Save the encrypted data to a file
            File.WriteAllBytes("encryptedData.bin", encryptedData);

            Console.WriteLine("Data encrypted and saved to file.");

        }

        private string XMLPrintReadBill()
        {

            //Configuration.LoadFromXml(configFilePath);


            if (FileToReadOrPrintXML != "")
            {
                string decompressed = XMLPrinter.ConvertXML(FileToReadOrPrintXML);

                outputFileJsonPath = FileToReadOrPrint + ".json";


                try
                {
                    // Serialize the string to JSON and write it to the file
                    File.WriteAllText(outputFileJsonPath, decompressed);
                    //RichTBox.Text = decompressed;
                    return decompressed;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return "";
                }
            }
            else
                return "";
        }
        // Function to run in a separate thread



        private void RasheedBGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            LogInFile("Start RasheedBGWorker Thread ");

            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;


                // Check if cancellation is requested
                if (worker.CancellationPending)
                {
                    e.Cancel = true; // Set Cancel to true to indicate the worker was cancelled
                    return; // Exit the work
                }

                //byte[] data = e.Argument as byte[];

                //LogInFile("Create New Rasheed Tag and Wait to mobile response ");
                //URasheedTag = new UseURasheedTag();
                //TagObj = URasheedTag.CreateTag(data);

            }
            catch (OperationCanceledException)
            {
                if (URasheedTag != null)
                    URasheedTag.ReleaseTag(TagObj);
                LogInFile("Rasheed NFC Failed to send Invoice data.");
                MessageBox.Show("Rasheed NFC Failed to send Invoice data.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RasheedBGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Operation was cancelled.");
                LogInFile("Rasheed NFC cancelled");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("An error occurred: " + e.Error.Message);
            }
            else
            {


                //if (URasheedTag != null)
                //  URasheedTag.ReleaseTag(TagObj);


                if (messageType == "Error")
                {
                    MessageBox.Show("Rasheed NFC Failed to send Invoice data.);", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
                else if (messageType == "Success")
                {
                    MessageBox.Show("Rasheed NFC has successfully completed sending Invoice data .", "Success Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }



        private void CheckBalanceAndCreateJson()
        {
            //  MessageBox.Show(Configuration.dbFilePath);
            var result = BillsChainLite.GetCurrCredit();
            if (result.HasValue)
            {
                int Credit = result.Value.CurrentBalance;
                string status = result.Value.CurrStatus;
                string PrevSignature = result.Value.Signature;

                //
                //MessageBox.Show(status);
                if (status == "Signature_NotCorrect")
                {
                    LogInFile("Print Error ::: Signature NotCorrect ");
                    MessageBox.Show("Data is corrupted. Please contact support for assistance.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (status == "Database_Error")
                {
                    LogInFile("Print Error ::: Database Error ");
                    MessageBox.Show("Unable to access the database. Please try again later or contact support for assistance.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }



                if (Credit == 0 && status == "Signature_Correct")
                {
                    LogInFile("Print Error ::: Balance is zero ");
                    MessageBox.Show("Balance is zero. Please recharge your balance to continue using our services.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                else if (Credit > 0 && status == "Signature_Correct")  //print the bill and update balance
                {

                    LogInFile("Start Print JSON File ");
                    //
                    //build json
                    string billstr = XMLPrintReadBill();


                    //    MessageBox.Show(billstr);


                    //MessageBox.Show(billstr);
                    bool already_Found = FindIfPrintedBefore(billstr);


                    //
                    bool retValue = false;
                    if (already_Found)
                    {
                        retValue = BillsChainLite.UpdateCredit(Credit, billstr, PrevSignature); //if its found before save the same balance
                        LogInFile("Add Database Record without update balance because File Printed Before ");
                    }
                    else
                    {
                        retValue = BillsChainLite.UpdateCredit(Credit - 1, billstr, PrevSignature); //if its new bill update balance
                        LogInFile("Add Database Record and  update balance ");
                    }


                    //
                    AddToDailyArchive(billstr, already_Found);
                    LogInFile("Add File To DailyArchive");





                    if (retValue)
                    {

                        MessageBox.Show("Invoice printing completed successfully and Rasheed NFC start sending data.", "Success Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LogInFile("Invoice printing completed successfully ");
                        LogInFile("Rasheed NFC start sending data.");



                        if (URasheedTag != null)
                            URasheedTag.ReleaseTag(TagObj);


                        byte[] compressed = XMLPrinter.Compress(billstr);
                        // File.WriteAllBytes("D:\\JsonBytes", compressed);

                        LogInFile("Create New Rasheed Tag and Wait to mobile response ");
                        URasheedTag = new UseRasheedTag();
                        TagObj = URasheedTag.CreateTag(compressed ,TerminalID , this);


                    }

                    else
                    {
                        MessageBox.Show("Invoice printing failed. You can try again later", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogInFile("Invoice printing failed");
                    }


                }
                else if (Credit == 0 && status == "No_Data") //check recharged balance then print the bill and add the first record 
                {
                    //  DBalance_UpdateDB();

                    ////////////////
                    LogInFile("First Time To Print Set Balance to 1000");
                    LogInFile("Start Print JSON File ");
                    int NewBalance = 1000;

                    string billstr = XMLPrintReadBill();
                    byte[] compressed = XMLPrinter.Compress(billstr);


                    bool retValue = BillsChainLite.UpdateCredit(NewBalance, billstr, "");

                    LogInFile("Add Database Record and  update balance ");



                    AddToDailyArchive(billstr, false);
                    LogInFile("Add File To DailyArchive");


                    if (retValue)
                    {
                        MessageBox.Show("Invoice printing completed successfully and Rasheed NFC start sending data.", "Success Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LogInFile("Invoice printing completed successfully ");
                        LogInFile("Rasheed NFC start sending data.");


                        //if (RasheedBGWorker.IsBusy)
                        //{
                        //    RasheedBGWorker.CancelAsync();
                        //    Thread.Sleep(100);

                        //    if (URasheedTag != null)
                        //        URasheedTag.ReleaseTag(TagObj);


                        //}

                        //RasheedBGWorker.RunWorkerAsync(compressed);


                        if (URasheedTag != null)
                            URasheedTag.ReleaseTag(TagObj);


                        LogInFile("Create New Rasheed Tag and Wait to mobile response ");
                        URasheedTag = new UseRasheedTag();

                        TagObj = URasheedTag.CreateTag(compressed ,TerminalID, this);



                    }
                    else
                    {
                        MessageBox.Show("Invoice printing failed. You can try again later", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogInFile("Invoice printing failed");
                    }

                }


            }


        }
        private bool FindIfPrintedBefore(string billstr)
        {
            try
            {
                JObject jsonObj = JObject.Parse(billstr);

                return BillsChainLite.CheckIfPrintedBefore(jsonObj["invoiceNo"].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }



        }
        private void AddToDailyArchive(string billstr, bool already_Found)
        {
            try
            {


                string DailyArchivePath = Configuration.XMLFolder + "\\DailyFilesArchive.xml";

                // Create XML file if it does not exist
                if (!File.Exists(DailyArchivePath))
                {
                    XmlDocument doc = new XmlDocument();
                    XmlNode declarationNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    doc.AppendChild(declarationNode);

                    XmlNode rootNode = doc.CreateElement("DailyFilesArchive");
                    doc.AppendChild(rootNode);

                    doc.Save(DailyArchivePath);
                }

                if (File.Exists(DailyArchivePath))
                {
                    //parse json
                    JObject jsonObj = JObject.Parse(billstr);

                    // Update the XML file with new field
                    XmlDocument doc = new XmlDocument();
                    doc.Load(DailyArchivePath);

                    XmlNode root = doc.DocumentElement;

                    XmlNode fieldNode = doc.CreateElement("Files");


                    XmlNode InvoiceNumNode = doc.CreateElement("InvoiceNum");
                    InvoiceNumNode.InnerText = jsonObj["invoiceNo"].ToString();
                    fieldNode.AppendChild(InvoiceNumNode);

                    XmlNode InvoiceDateNode = doc.CreateElement("InvoiceDate");
                    InvoiceDateNode.InnerText = jsonObj["transactionDateTime"].ToString();
                    fieldNode.AppendChild(InvoiceDateNode);

                    XmlNode InvoicePriceNode = doc.CreateElement("InvoicePrice");
                    InvoicePriceNode.InnerText = jsonObj["totalAmount"].ToString();
                    fieldNode.AppendChild(InvoicePriceNode);


                    XmlNode xmlFileNode = doc.CreateElement("XmlFile");
                    xmlFileNode.InnerText = FileToReadOrPrintXML;
                    fieldNode.AppendChild(xmlFileNode);

                    XmlNode xpsFileNode = doc.CreateElement("XpsFile");
                    xpsFileNode.InnerText = FileToReadOrPrint;
                    fieldNode.AppendChild(xpsFileNode);



                    XmlNode alreadyPrinted = doc.CreateElement("AlreadyPrinted");
                    alreadyPrinted.InnerText = already_Found.ToString();
                    fieldNode.AppendChild(alreadyPrinted);


                    root.AppendChild(fieldNode);
                    doc.Save(DailyArchivePath);
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }



        }
        private void XMLPrintBtn_Click(object sender, EventArgs e)
        {


            ClientSize = new Size(573, 459);

            LogInFile("Start Rasheed Printer with XML File ::" + FileToReadOrPrintXML);
            CheckBalanceAndCreateJson();

            RichTBox.Visible = true;
            RichTBox.Text = "";
            label2.Location = new Point(7, 300);


            if (outputFileJsonPath != "")
            {
                string JsonData = File.ReadAllText(outputFileJsonPath);
                RichTBox.Text = JsonData;
            }





        }


        private void Terminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            DispatchPrinter();
            LogInFile("Dispatch XML Printer");

            if (URasheedTag != null)
                URasheedTag.ReleaseTag(TagObj);


            //if (RasheedBGWorker.IsBusy)
            //{
            //    if (URasheedTag != null)
            //        URasheedTag.ReleaseTag(TagObj);

            //    RasheedBGWorker.CancelAsync();
            //}

        }
        private void DispatchPrinter()
        {
            string command = XMLPrinterPath;

            string arguments = $"/dispatch";
            RunCommandSilently(command, arguments);




        }

        private void Terminal_Shown(object sender, EventArgs e)
        {
            string selectedPrinter = Configuration.selectedPrinter;
            switch (selectedPrinter)
            {
                case "RashidPrinter":
                    {
                        this.Hide();
                        this.Visible = false;
                        break;
                    }
                case "PhysicalPrinter":
                    {
                        this.Hide();
                        this.Visible = false;
                        break;
                    }
                case "BothPrinter":
                    {
                        this.Hide();
                        this.Visible = false;
                        break;
                    }
                case "ChoosingBetweenPrinters":
                default:
                    {
                       break;
                    }
            }

        }
    }



}




