using RasheedTag;
using static RasheedTag.ITagStatusUpdate;

namespace VPrinterMonitor
{
    public class UseRasheedTag : ITagStatusUpdate
    {
        static int TerminalID = 0;
        static Terminal TermDialog;
        public Tag CreateTag(byte[] JsonData, int terminalID, Terminal terminal)
        {
            var tag = new Tag(JsonData);
            tag.Subscribe(this);
            TerminalID = terminalID;
            TermDialog = terminal;

            LogInFile("New Tag created...");
            return tag;

        }
        public void ReleaseTag(Tag CurrTag)
        {
            LogInFile("Tag Released ...");
            CurrTag.Unsubscribe(this);
            CurrTag.Dispose();
        }



        public void OnTagStatusChanged(ITagStatusUpdate.TagStatus status)
        {
            switch (status)
            {
                case TagStatus.ScanDevice:
                    {
                        LogInFile("Scan rasheed device 🔎");
                        break;
                    }
                case TagStatus.DeviceNotFound:
                    {
                        MessageBox.Show("Rasheed device not found .", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogInFile("Rasheed device not found 🤷🏻");
                        if (TermDialog != null)
                            TermDialog.Close();
                        break;
                    }
                case TagStatus.DeviceFailedToConnect:
                    {
                        MessageBox.Show("Failed to connect to rasheed device.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogInFile("Failed to connect to rasheed device 😢");
                        if (TermDialog != null)
                            TermDialog.Close();
                        break;
                    }
                case TagStatus.DeviceConnected:
                    {
                        LogInFile("Rasheed connected 🤝");
                        break;
                    }
                case TagStatus.WaitingMobile:
                    {
                        LogInFile("I'm waiting a mobile");
                        break;
                    }
                case TagStatus.MobileConnected:
                    {
                        LogInFile("Mobile connected");
                        break;
                    }
                case TagStatus.TransmissionSuccess:
                    {
                        MessageBox.Show("Rasheed NFC has successfully completed sending Invoice data .", "Success Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogInFile("Rasheed NFC Success! 🎉");

                        if (TermDialog != null)
                            TermDialog.Close();
                        break;
                    }
                case TagStatus.TransmissionInProgress:
                    {
                        LogInFile("Rasheed NFC InProgress... 🕒️");
                        break;
                    }
                case TagStatus.TransmissionFailed:
                    {
                        MessageBox.Show("Rasheed NFC Failed to send Invoice data .", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogInFile("Rasheed NFC Failed! ❌");
                        if (TermDialog != null)
                            TermDialog.Close();
                        break;
                    }
            }
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
                // MessageBox.Show($"Logging failed: {ex.Message}");
                return;
            }
        }
    }
}
