using System.Xml;
//using AutoMapper;
static class Configuration
{

    public static string? selectedPrinter { get; set; }

    public static string? DefaultPhyPrinter { get; set; }
    public static string? folderPath { get; set; }
    public static string? fileFilter { get; set; }

    public static int ProductCodeStartX { get; set; }
    public static int ProductCodeEndX { get; set; }
    public static int ProductUnitStartX { get; set; }
    public static int ProductUnitEndX { get; set; }

    public static int TaxNumberStartY { get; set; }
    public static int TaxNumberStartX { get; set; }

    public static int BuyerNameY { get; set; }

    public static int BuyerNameStartX { get; set; }


    public static int BuyerNameEndX { get; set; }

    public static int BuyerAddressY { get; set; }

    public static int BuyerAddressStartX { get; set; }

    public static int BuyerAddressEndX { get; set; }

    public static int InvoiceNumberStartY { get; set; }
    public static int InvoiceNumberEndY { get; set; }

    public static int InvoiceStartX { get; set; }

    public static int InvoiceEndX { get; set; }

    public static int ProductStartY { get; set; }

    public static string ProductStartPhrase { get; set; }
    public static string VAT { get; set; }
    public static string CR { get; set; }
    public static string XMLFolder { get; set; }
    public static string connectionString { get; set; }
    public static string dbFilePath { get; set; }

    public static string Store { get; set; }
    public static string ReturnPolicy { get; set; }

    public static string JsonFilePath { get; set; }

    public static int LineHeight { get; set; }

    public static int ProductNameStartX { get; set; }

    public static int ProductNameEndX { get; set; }
    public static int DateTimeStartY { get; private set; }
    public static int TimeStartX { get; private set; }
    public static int TimeEndX { get; private set; }

    public static int ProductUnitCodeStartX { get; set; }
    public static int ProductUnitCodeEndX { get; set; }
    public static int ProductUnitPriceStartX { get; set; }

    public static int ProductUnitPriceEndX { get; set; }

    public static int ProductUnitQuantStartX { get; set; }

    public static int ProductUnitQuantEndX { get; set; }

    public static int ProductTotalPriceStartX { get; set; }

    public static int ProductTotalPriceEndX { get; set; }

    public static int ProductItemUnitStartX    { get; set; }
    public static int ProductItemUnitEndX { get; set; }
    public static string StopLinePhrase { get; set; }
    public static string RetailerName { get; set; }
    public static string POS { get; set; }

    public static string Category { get; set; }

    public static int CategoryId { get; set; }
    public static int CategoryCode { get; set; }
    public static string? Logo { get; set; }
    public static int StoreId { get; set; }
    public static string StoreCode { get; set; }
    public static int SalesY { get; set; }


    public static int SalesStartX { get; set; }
    public static int CashierY { get; set; }
    public static int CashierStartX { get; set; }
    public static int NumOfQRCode { get; set; }

    public static int commercialRegisterNumY { get; set; }
    public static int commercialRegisterNumX { get; set; }
    public static string STAmount { get; set; }

    public static int  vatAmountY {  get; set; }
    public static int  vatAmountX { get; set; }

    public static int discountAmountY { get; set; }
    public static int discountAmountX { get; set; }

    public static int  salesNameY { get; set; }
    public static int  salesNameX { get; set; }

    public static int totalQtyY { get; set; }
    public static int totalQtyX { get; set; }

    public static int branchNameStartY { get; set; }
    public static int branchNameEndY { get; set; }
    public static int branchNameX { get; set; }


    public static void LoadFromXml(string filePath)
    {
        try
        {
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                
                XmlDocument xmlDoc1 = new XmlDocument();
                xmlDoc1.Load(reader); // Load the XML file
                XmlNode configNode = xmlDoc1.SelectSingleNode("Configuration");
                LineHeight = int.Parse(configNode.SelectSingleNode("LineHeight").InnerText);
                ProductStartY = int.Parse(configNode.SelectSingleNode("ProductStartY").InnerText);
                ProductNameStartX = int.Parse(configNode.SelectSingleNode("ProductNameStartX").InnerText);
                ProductNameEndX = int.Parse(configNode.SelectSingleNode("ProductNameEndX").InnerText);
                SalesY = int.Parse(configNode.SelectSingleNode("SalesY").InnerText);
                SalesStartX = int.Parse(configNode.SelectSingleNode("SalesStartX").InnerText);
                CashierY = int.Parse(configNode.SelectSingleNode("CashierY").InnerText);
                CashierStartX = int.Parse(configNode.SelectSingleNode("CashierStartX").InnerText);
                NumOfQRCode = int.Parse(configNode.SelectSingleNode("NumOfQRCode").InnerText);
                DateTimeStartY = int.Parse(configNode.SelectSingleNode("DateTimeStartY").InnerText);
                TimeStartX = int.Parse(configNode.SelectSingleNode("TimeStartX").InnerText);
                TimeEndX = int.Parse(configNode.SelectSingleNode("TimeEndX").InnerText);

                STAmount = configNode.SelectSingleNode("STAmount").InnerText;
                ProductUnitPriceStartX = int.Parse(configNode.SelectSingleNode("ProductUnitPriceStartX").InnerText);
                ProductUnitPriceEndX = int.Parse(configNode.SelectSingleNode("ProductUnitPriceEndX").InnerText);
                ProductCodeStartX = int.Parse(configNode.SelectSingleNode("ProductCodeStartX").InnerText);
                ProductCodeEndX = int.Parse(configNode.SelectSingleNode("ProductCodeEndX").InnerText);

                TaxNumberStartY = int.Parse(configNode.SelectSingleNode("TaxNumberStartY").InnerText);
                TaxNumberStartX = int.Parse(configNode.SelectSingleNode("TaxNumberStartX").InnerText);

                ProductUnitStartX = int.Parse(configNode.SelectSingleNode("ProductUnitStartX").InnerText);
                ProductUnitEndX = int.Parse(configNode.SelectSingleNode("ProductUnitEndX").InnerText);
                ProductUnitCodeStartX = int.Parse(configNode.SelectSingleNode("ProductUnitCodeStartX").InnerText);
                ProductUnitCodeEndX = int.Parse(configNode.SelectSingleNode("ProductUnitCodeEndX").InnerText);
                ProductUnitQuantStartX = int.Parse(configNode.SelectSingleNode("ProductUnitQuantStartX").InnerText);
                ProductUnitQuantEndX = int.Parse(configNode.SelectSingleNode("ProductUnitQuantEndX").InnerText);
                ProductTotalPriceStartX = int.Parse(configNode.SelectSingleNode("ProductTotalPriceStartX").InnerText);
                ProductTotalPriceEndX = int.Parse(configNode.SelectSingleNode("ProductTotalPriceEndX").InnerText);
                ProductItemUnitStartX = int.Parse(configNode.SelectSingleNode("ProductItemUnitStartX").InnerText);
                ProductItemUnitEndX = int.Parse(configNode.SelectSingleNode("ProductItemUnitEndX").InnerText);


                StopLinePhrase = configNode.SelectSingleNode("StopLinePhrase").InnerText;
                VAT = configNode.SelectSingleNode("VAT").InnerText;
                CR = configNode.SelectSingleNode("CR").InnerText;
                ReturnPolicy = configNode.SelectSingleNode("ReturnPolicy").InnerText;
                Store = configNode.SelectSingleNode("Store").InnerText;
                XMLFolder = configNode.SelectSingleNode("XMLFolder").InnerText;
                DefaultPhyPrinter = configNode.SelectSingleNode("DefaultPhyPrinter").InnerText;
                selectedPrinter = configNode.SelectSingleNode("selectedPrinter").InnerText;

                connectionString = configNode.SelectSingleNode("ConString").InnerText;
                dbFilePath = configNode.SelectSingleNode("DatabaseFile").InnerText;

                BuyerNameY = int.Parse(configNode.SelectSingleNode("BuyerNameY").InnerText);
                BuyerNameStartX = int.Parse(configNode.SelectSingleNode("BuyerNameStartX").InnerText);
                BuyerNameEndX = int.Parse(configNode.SelectSingleNode("BuyerNameEndX").InnerText);
                BuyerAddressY = int.Parse(configNode.SelectSingleNode("BuyerAddressY").InnerText);
                BuyerAddressStartX = int.Parse(configNode.SelectSingleNode("BuyerAddressStartX").InnerText);
                BuyerAddressEndX = int.Parse(configNode.SelectSingleNode("BuyerAddressEndX").InnerText);
                ProductStartPhrase = configNode.SelectSingleNode("ProductStartPhrase").InnerText;
                InvoiceNumberStartY = int.Parse(configNode.SelectSingleNode("InvoiceNumberStartY").InnerText);
                InvoiceNumberEndY = int.Parse(configNode.SelectSingleNode("InvoiceNumberEndY").InnerText);

                InvoiceStartX = int.Parse(configNode.SelectSingleNode("InvoiceStartX").InnerText);
                InvoiceEndX = int.Parse(configNode.SelectSingleNode("InvoiceEndX").InnerText);
                RetailerName = configNode.SelectSingleNode("RetailerName").InnerText;
                // JsonFilePath=configNode.SelectSingleNode("JsonFilePath").InnerText;
                POS = configNode.SelectSingleNode("POS").InnerText;
                Category = configNode.SelectSingleNode("Category").InnerText;
                CategoryId = int.Parse(configNode.SelectSingleNode("CategoryId").InnerText);
                Logo = configNode.SelectSingleNode("Logo").InnerText;
                StoreId = int.Parse(configNode.SelectSingleNode("StoreId").InnerText);
                StoreCode = configNode.SelectSingleNode("StoreCode").InnerText;
                CategoryCode = int.Parse(configNode.SelectSingleNode("CategoryCode").InnerText);
                NumOfQRCode = int.Parse(configNode.SelectSingleNode("NumOfQRCode").InnerText);
                commercialRegisterNumY = int.Parse(configNode.SelectSingleNode("commercialRegisterNumY").InnerText);
                commercialRegisterNumX = int.Parse(configNode.SelectSingleNode("commercialRegisterNumX").InnerText);

                vatAmountY = int.Parse(configNode.SelectSingleNode("vatAmountY").InnerText);
                vatAmountX = int.Parse(configNode.SelectSingleNode("vatAmountX").InnerText);

                discountAmountY = int.Parse(configNode.SelectSingleNode("discountAmountY").InnerText);
                discountAmountX = int.Parse(configNode.SelectSingleNode("discountAmountX").InnerText);

                salesNameY = int.Parse(configNode.SelectSingleNode("salesNameY").InnerText);
                salesNameX = int.Parse(configNode.SelectSingleNode("salesNameX").InnerText);


                totalQtyY = int.Parse(configNode.SelectSingleNode("totalQtyY").InnerText);
                totalQtyX = int.Parse(configNode.SelectSingleNode("totalQtyX").InnerText);
                
                branchNameStartY = int.Parse(configNode.SelectSingleNode("branchNameStartY").InnerText);
                branchNameEndY = int.Parse(configNode.SelectSingleNode("branchNameEndY").InnerText);
                branchNameX =int.Parse(configNode.SelectSingleNode("branchNameX").InnerText);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Console.WriteLine("An error occurred 1: " + ex.StackTrace);
        }
    }
}
