using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.Design.Behavior;
using System.Xml.Linq;
using System.Xml;
using System.Net.NetworkInformation;

using Newtonsoft.Json;
using System.IO.Compression;
using RasheedTag;
using System.Globalization;
using Terminal;
static class XMLPrinter
{
    private static int stopLine = 3000000;
    public static byte[] ConvertXMLFromFolder()
    {
        //Configuration.LoadFromXml("E:\\Bills\\XMLInvoice\\config.xml");
        Invoice inv = ProcessPrintXMLInv(GetMaxNumberXmlFile(Configuration.XMLFolder));

        string json = JsonConvert.SerializeObject(inv, Newtonsoft.Json.Formatting.Indented);
        byte[] compressed = Compress(json);
        return compressed;
    }
    public static byte[] Compress(string text)
    {
        // Compress the JSON string using GZIP
        byte[] compressedData;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            using (StreamWriter writer = new StreamWriter(gzipStream))
            {
                writer.Write(text);
            }
            compressedData = memoryStream.ToArray();
        }
        return compressedData;
    }
    public static string Decompress(byte[] compressedData)
    {
        string decompressedJson = null;
        using (MemoryStream memoryStream = new MemoryStream(compressedData))
        using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        using (StreamReader reader = new StreamReader(gzipStream))
        {
            decompressedJson = reader.ReadToEnd();
            //Person receivedPerson = JsonConvert.DeserializeObject<Person>(decompressedJson);
            //Console.WriteLine($"Received Person: {receivedPerson.FirstName}, {receivedPerson.LastName}, Age: {receivedPerson.Age}");
        }
        return decompressedJson;
    }

    static string GetMaxNumberXmlFile(string folderPath)
    {
        Console.WriteLine("folderPath = {0}", folderPath);
        if (Directory.Exists(folderPath))
        {
            Console.WriteLine("Folder {0} exists", folderPath);
        }
        string[] xmlFiles = Directory.GetFiles(folderPath, "*.xml");

        Console.WriteLine(xmlFiles.Count());

        if (xmlFiles.Length == 0)
        {
            return null;
        }

        string maxNumberXmlFile = xmlFiles
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => int.TryParse(new string(name.Where(char.IsDigit).ToArray()), out _))
            .OrderByDescending(name => int.Parse(new string(name.Where(char.IsDigit).ToArray())))
            .FirstOrDefault();

        if (maxNumberXmlFile != null)
        {
            maxNumberXmlFile = Path.Combine(folderPath, maxNumberXmlFile + ".xml");
        }

        return maxNumberXmlFile;
    }
    public static string ConvertXML(string filePath)
    {

        //  MessageBox.Show(filePath);
        Invoice inv = ProcessPrintXMLInv(filePath);


        Console.WriteLine("input file {0} processed file {1}", filePath, GetMaxNumberXmlFile(Configuration.XMLFolder));

        string json = JsonConvert.SerializeObject(inv, Newtonsoft.Json.Formatting.Indented);
        //       byte[] compressed = Compress(json);
        return json;

    }

    //public static (string JSON, Invoice INV)? ConvertXML(string filePath)
    //{

    //    //  MessageBox.Show(filePath);
    //    Invoice inv = ProcessPrintXMLInv(filePath);


    //    Console.WriteLine("input file {0} processed file {1}", filePath, GetMaxNumberXmlFile(Configuration.XMLFolder));

    //    string json = JsonConvert.SerializeObject(inv, Newtonsoft.Json.Formatting.Indented);
    //    //       byte[] compressed = Compress(json);
    //    return (json, inv);

    //}


    private static Invoice ProcessPrintXMLInv(string filePath)
    {


        XmlReaderSettings settings = new XmlReaderSettings();
        settings.DtdProcessing = DtdProcessing.Parse;
        try
        {
            using (XmlReader reader = XmlReader.Create(filePath, settings))
            {
                // XmlReaderSettings settings = new XmlReaderSettings();
                // settings.DtdProcessing = true;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);

                /*var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Record, Line>();
                });
                var mapper = mapperConfig.CreateMapper();*/
                Invoice inv = new Invoice();
                //POS pos = new POS();

                int currentPageNumber = 1;
                HashSet<Record> lines = new HashSet<Record>();
                string arabicPattern = @"[\u0600-\u06FF]+";
                //string arabicPattern = @"a";
                string cleanNumbersPattern = @"[,\n]";
                string arabicNumeralPattern = @"[\u0660-\u0669]+";
                string newLinePattern = @"[\s]+";
                string pattern = @"<text left=""([\d]+)"" top=""([\d]+)"" width=""([\d]+)"" height=""([\d]+)""[\w|\W]*?>([a-zA-Z\s0-9 #.ا-ي,\/]*)";
                RegexOptions options = RegexOptions.Multiline;
                inv.storeName = Configuration.RetailerName;
                inv.categoryCode = Configuration.CategoryCode;
                inv.terminalCode = Configuration.Category;
                //   MessageBox.Show(inv.storeName);
                //inv.Logo = Configuration.Logo;
                //inv.Id = GetMacList().Count > 0 ? GetMacList()[0] : "";
                inv.storeCode = Configuration.StoreCode;
                inv.transactionDateTime = "";
                
                BuyerInfo buyerInfo = new BuyerInfo();
                XmlNodeList pages = xmlDoc.GetElementsByTagName("page");
                foreach (XmlNode page in pages)
                {
                    currentPageNumber = int.Parse(page.Attributes["number"].Value);
                    //  Console.WriteLine("page {0}", page.Attributes["number"].Value);
                    XmlNodeList elements = page.SelectNodes("text");
                    int elementnumcounter = 0;
                    foreach (XmlNode element in elements)
                    {
                        Int32 x = int.Parse(element.Attributes["left"].Value);
                        Int32 y = int.Parse(element.Attributes["top"].Value) / 10 * 10;
                        // Console.WriteLine("x {0},y {1}",x,y);
                        Record r = null;
                        string text = element.InnerText;

                        int xxxxxxxxxxxxxx;
                        if (text.Contains("201"))
                            xxxxxxxxxxxxxx = elementnumcounter;
                        elementnumcounter++;
                        if (currentPageNumber == 1)
                        {

                            if (y == Configuration.BuyerNameY && x >= Configuration.BuyerNameStartX && x <= Configuration.BuyerNameEndX)
                                buyerInfo.Name = text;

                            if (y == Configuration.BuyerAddressY && x >= Configuration.BuyerAddressStartX && x <= Configuration.BuyerAddressEndX)
                                buyerInfo.Address = text;
                            if ((y >= Configuration.InvoiceNumberStartY && y <= Configuration.InvoiceNumberEndY) && x >= Configuration.InvoiceStartX && x <= Configuration.InvoiceEndX)
                                inv.invoiceNo = text;
                            if ((y >= Configuration.DateTimeStartY && y <= Configuration.DateTimeStartY + 30) && x >= Configuration.TimeStartX && x <= Configuration.TimeEndX)
                            {

                                inv.transactionDateTime = inv.transactionDateTime + " " + text;
                                inv.transactionDateTime = inv.transactionDateTime.Trim();
                                // Console.WriteLine("date time = {0}",text);
                            }
                            if (y == Configuration.SalesY && x >= Configuration.SalesStartX)
                            {
                                // inv.pos.Sales=text;
                            }
                            if (y == Configuration.CashierY && x >= Configuration.CashierStartX)
                            {
                                 inv.cashierName = text;
                            }

                            if (y == Configuration.TaxNumberStartY && x >= Configuration.TaxNumberStartX)
                            {
                                inv.taxNumber = text;
                            }
                            if (y == Configuration.commercialRegisterNumY && x >= Configuration.commercialRegisterNumX)
                            {
                                inv.commercialRegisterNumber = text;
                            }
                            if (y == Configuration.discountAmountY && x >= Configuration.discountAmountX)
                            {
                                inv.discountAmount = decimal.Parse(text);
                            }
                            if (y == Configuration.totalQtyY && x >= Configuration.totalQtyX)
                            {
                                inv.totalQty = int.Parse(text);
                            }

                            if (y == Configuration.salesNameY && x >= Configuration.salesNameX)
                            {
                                inv.salesName = text;
                            }
                            if (y == Configuration.branchNameStartY && x >= Configuration.branchNameX)
                            {
                                inv.branchName = ReverseString(text);
                            }
                            if (y == Configuration.branchNameEndY && x >= Configuration.branchNameX)
                            {
                                try
                                {
                                    string[] stringbn = inv.branchName.Split(new char[] { '-' });

                                    inv.branchName = stringbn[0] + "-" + stringbn[1] + ReverseString(text) + "-" + stringbn[2];
                                }
                                catch (Exception ex) { }
                               // inv.branchName = ReverseString(inv.branchName);
                            }
                            //Console.WriteLine("inv.invoiceNo {0}",inv.invoiceNo);
                        }
                        decimal decimalValue;
                        bool v = false;
                        if (y == Configuration.ProductStartY || y > Configuration.ProductStartY || currentPageNumber > 1)
                        {
                            if (text.Equals(Configuration.StopLinePhrase))
                            {
                                // Console.WriteLine("stop line {0}", y);
                                stopLine = y;
                            }
                            if (y != stopLine && y < stopLine)
                            {
                                r = new Record(y, currentPageNumber);
                                if (lines.Contains(r))
                                {
                                    // Console.WriteLine("lines.Contains({1}):{0}", text, y);
                                    Record rr = null;
                                    bool f = lines.TryGetValue(r, out rr);

                                    if (x >= Configuration.ProductTotalPriceStartX && x <= Configuration.ProductUnitQuantEndX)
                                    {
                                        v = Decimal.TryParse(Regex.Replace(text, cleanNumbersPattern, ""), out decimalValue);
                                        //Console.WriteLine("{0} {1} {2} {3}",v,x,y,text);
                                    }
                                    if (v)
                                    {
                                     
                                        if (x >= Configuration.ProductUnitPriceStartX && x <= Configuration.ProductUnitPriceEndX)
                                            rr.UnitPrice = Decimal.Parse(Regex.Replace(text, cleanNumbersPattern, ""));
                                        if (x >= Configuration.ProductTotalPriceStartX && x <= Configuration.ProductTotalPriceEndX)
                                            rr.TotalPrice = Decimal.Parse(Regex.Replace(text, cleanNumbersPattern, ""));
                                        if (x >= Configuration.ProductUnitQuantStartX && x <= Configuration.ProductUnitQuantEndX)
                                            rr.Quantity = Decimal.Parse(Regex.Replace(text, cleanNumbersPattern, ""));

                                    }
                                    else if (x >= Configuration.ProductNameStartX && x <= Configuration.ProductNameEndX)
                                    {
                                        if (x >= Configuration.ProductUnitCodeStartX && x <= Configuration.ProductUnitCodeEndX)
                                            rr.Code = text;

                                        if (!Regex.IsMatch(text, arabicPattern) || Regex.IsMatch(text, arabicNumeralPattern))
                                        {
                                           

                                            rr.AddProductChunck(new XText { Text = Regex.Replace(text, newLinePattern, " "), x = x });
                                        }
                                        else
                                        {
                                            if (x >= Configuration.ProductItemUnitStartX)
                                                rr.Unit = text;

                                            rr.AddProductChunck(new XText { Text = Regex.Replace(ReverseString(text) + " ", newLinePattern, " "), x = x });
                                            rr.Reversed = true;
                                        }
                                    }
                                    if (x >= Configuration.ProductUnitCodeStartX && x <= Configuration.ProductUnitCodeEndX)
                                        rr.Code = text;
                                   
                                    if (x >= Configuration.ProductItemUnitStartX && x <= Configuration.ProductItemUnitEndX)
                                        rr.Unit = ReverseString(text);
                                }
                                else
                                {
                                    //Console.WriteLine("lines deos not Contain({1}):{0}", text, y);
                                    if (x >= Configuration.ProductTotalPriceStartX && x <= Configuration.ProductUnitQuantEndX)
                                    {
                                        v = Decimal.TryParse(Regex.Replace(text, cleanNumbersPattern, ""), out decimalValue);
                                    }
                                    if (v)
                                    {
                                        if (x >= Configuration.ProductUnitPriceStartX && x <= Configuration.ProductUnitPriceEndX)
                                            r.UnitPrice = Decimal.Parse(Regex.Replace(text, cleanNumbersPattern, ""));
                                        if (x >= Configuration.ProductTotalPriceStartX && x <= Configuration.ProductTotalPriceEndX)
                                            r.TotalPrice = Decimal.Parse(Regex.Replace(text, cleanNumbersPattern, ""));
                                        if (x >= Configuration.ProductUnitQuantStartX && x <= Configuration.ProductUnitQuantEndX)
                                            r.Quantity = Decimal.Parse(Regex.Replace(text, cleanNumbersPattern, ""));
                                    }
                                    else if (x >= Configuration.ProductNameStartX && x <= Configuration.ProductNameEndX)
                                    {

                                        if (x >= Configuration.ProductUnitCodeStartX && x <= Configuration.ProductUnitCodeEndX)
                                            r.Code = text;

                                        if (!Regex.IsMatch(text, arabicPattern) || Regex.IsMatch(text, arabicNumeralPattern))
                                        {
                                        
                                            r.AddProductChunck(new XText { Text = Regex.Replace(text, newLinePattern, " "), x = x });
                                        }
                                        else
                                        {
                                            
                                            r.AddProductChunck(new XText { Text = Regex.Replace(ReverseString(text) + " ", newLinePattern, " "), x = x });
                                            r.Reversed = true;
                                        }

                                   
                                    }
                                    if (x >= Configuration.ProductUnitCodeStartX && x <= Configuration.ProductUnitCodeEndX)
                                        r.Code = text;

                                    if (x >= Configuration.ProductItemUnitStartX && x <= Configuration.ProductItemUnitEndX)
                                        r.Unit = ReverseString(text);


                                    lines.Add(r);
                                }
                            }
                        }
                    }

                }
                // MessageBox.Show("test2");
                foreach (Record line in lines)
                {
                    //	Console.WriteLine("y={0}, page={1},Name:{2},price={3}, qty = {4}", line.Y, line.PageNumber, line.ProductName,line.UnitPrice,line.Quantity);
                }
                //,Unit="حبة"
                inv.transactionDetails = lines.Select(p => new Line { itemId = p.Code, quantity = p.Quantity, unitPrice = p.UnitPrice, totalPrice = p.TotalPrice, itemName = p.ProductName , itemUnit=p.Unit }).Where(line =>line.itemId !="" && line.quantity > 0 && line.unitPrice > 0 && line.totalPrice > 0).ToList();
                var itemsNames = lines.Select(p => new { Y = p.Y, PageNumber = p.PageNumber, Code = p.Code ,itemName = p.ProductName, unitPrice = p.UnitPrice, totalPrice = p.TotalPrice , itemUnit = p.Unit }).Where(line => line.Y > 0 && line.unitPrice == 0 && line.totalPrice == 0).OrderBy(p => p.PageNumber).ThenBy(p => p.Y).ToList();
                int ff = 0;

                foreach (var item in itemsNames)
                {
                    //Console.WriteLine("{2}:{0}:{1}",item.PageNumber,item.Y,item.Name);
                    if (ff < inv.transactionDetails.Count)
                    {
                        inv.transactionDetails[ff].itemName = item.itemName;
                        inv.transactionDetails[ff].itemUnit = item.itemUnit;
                        ff++;
                    }

                }
                decimal totalInvoiceAmount = lines
                .Select(p => new Line { quantity = p.Quantity, unitPrice = p.UnitPrice, totalPrice = p.TotalPrice, itemName = p.ProductName ,  itemUnit = p.Unit , itemId = p.Code})
                .Where(line => line.quantity > 0 && line.unitPrice > 0 && line.totalPrice > 0)
                .Sum(line => line.totalPrice);
                inv.totalAmount = totalInvoiceAmount;

                //Set barcode 
                inv.barCodes = new List<string>();
                inv.barCodes.Add(inv.invoiceNo);


                //Set QR Codes
                //Get QR Images Name 
                inv.qrCodes = new List<string>();
                int QRCodeFileNum = 0;
                for (int i = 0; i < Configuration.NumOfQRCode; i++)
                {
                    string QRFileName = filePath.Replace(".xml", "_0000" + i.ToString() + ".jpg");
                    if (File.Exists(QRFileName))
                    {
                        string QRRes = QRCodeReader.QRCodeReaderFromImage(QRFileName);
                        if (QRRes != "No QR code found")
                            inv.qrCodes.Add(QRRes);
                    }
                    else
                        break;
                }
                // convert datetime
                string inputFormat = "dd/MM/yyyy hh:mm:sstt"; // dd/MM/yyyy format with 12-hour clock and AM/PM


                //   MessageBox.Show("test3");

                // Parse the date string to DateTime object
                try
                {
                    DateTime dateTime = DateTime.ParseExact(inv.transactionDateTime, inputFormat, CultureInfo.InvariantCulture);
                    //   MessageBox.Show("test4");
                    // Convert to ISO 8601 format with 'Z' indicating UTC (Zulu time)
                    string outputFormat = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    //    MessageBox.Show("test5");
                    inv.transactionDateTime = outputFormat;
                    //   MessageBox.Show("outputFormat" + outputFormat);
                }
                catch (FormatException ex)
                {
                    // Handle the exception (e.g., log it, throw a custom exception, etc.)
                    //    MessageBox.Show(ex.Message);
                }

                return inv;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.StackTrace);
            return null;
        }
    }
    private static List<string> GetMacList()
    {
        List<string> macList = new List<string>();
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            // Check if the interface is operational and not a loopback interface
            if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback && networkInterface.GetPhysicalAddress().GetAddressBytes().Length > 0)
            {
                PhysicalAddress macAddress = networkInterface.GetPhysicalAddress();
                string macAddressString = BitConverter.ToString(macAddress.GetAddressBytes()).Replace("-", ":");
                macList.Add(macAddressString);
            }
        }
        return macList;
    }
    private static string ReverseString(string input)
    {
        StringBuilder sb = new StringBuilder();
        char[] array = input.ToCharArray();
        Array.Reverse(array);
        for (int i = 0; i < array.Length; i++)
        {
            sb.Append(array[i]);
        }

        return sb.ToString();
    }

}
