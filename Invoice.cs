//using AutoMapper;
class Invoice
{
    public string invoiceNo { get; set; }
    private string _date = string.Format("{0}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));//ToShortDateString();// { get; set; } 



    // public string vatAmount { get; set; }
    public decimal totalAmount { get; set; }
    // public string RetailerName { get; set; }
    // public string POS { get; set; }

    public string taxNumber { get; set; }
    public string transactionDateTime
    {
        get { return _date; }
        set { _date = value; }
    }

    public int categoryCode { get; set; }

    public string storeCode { get; set; }

    public string storeName { get; set; }

    public string terminalCode { get; set; }


    public string commercialRegisterNumber { get; set; }
    //  public string Id { get; set; }
    //public string Logo { get; set; }

    // public string CR { get; set; }


    // public string Store { get; set; }

    // public string Sales { get; set; }

    public string cashierName { get; set; }
    //public int CategoryId { get; set; }


    public decimal vatAmount { get; set; }

    public decimal discountAmount { get; set; }
    public string salesName { get; set; }

    public int totalQty { get; set; }
    public string branchName { get; set; }
    //public BuyerInfo BuyerInfo { get; set; }

    //public HashSet<Record> Lines { get; set; }
    //public String MacAdress { get; set; }
    public List<Line> transactionDetails { get; set; }

    public List<string> qrCodes { get; set; }
    public List<string> barCodes { get; set; }
    // public string ReturnPolicy { get; set; }


    // public override string ToString()
    // {
    // 	return string.Format("invoiceNo={0},Buyer Name={1}, Buyer Address = {2}", invoiceNo, BuyerInfo.Name, BuyerInfo.Address);
    // }
}
