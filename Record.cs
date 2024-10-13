using System.Text;
//using AutoMapper;
class Record
{
	protected int y;
	protected PriorityQueue<XText> product;
	protected decimal quantity;
	protected decimal price;
	protected decimal totalPrice;
	protected bool arabicText;

	protected string? unit;

	protected string? code;
	protected string? productName0;

	protected int pageNumber;
	public Record(int y)
	{
		this.y = y;
		this.product = new PriorityQueue<XText>(new XTextComparer());
	}
	public Record(int y,int pageNumber)
	{
		this.y = y;
		this.pageNumber=pageNumber;
		this.product = new PriorityQueue<XText>(new XTextComparer());
	}
	public Record(int y, PriorityQueue<XText> product, string code , string unit, decimal quantity, decimal price, decimal totalPrice)
	{
		this.y = y;
		this.product = product;
		this.code = code;
		this.unit = unit;	
		this.price = price;
		this.quantity = quantity;
		this.totalPrice = totalPrice;
		Console.WriteLine("Constructor created");
	}
public string? Unit
	{
		get
		{
			return unit;
		}

		set
		{
			unit = value;
		}
	}
	public string? ProductName0
	{
		get
		{
			return productName0;
		}

		set
		{
			productName0 = value;
		}
	}
	public string? Code
	{
		get
		{
			return code;
		}

		set
		{
			code = value;
		}
	}
	public int Y
	{
		get
		{
			return y;
		}
	}

	public PriorityQueue<XText> Product
	{
		get
		{
			return product;
		}
	}

	public decimal UnitPrice
	{
		get
		{
			return price;
		}

		set
		{
			price = value;
		}
	}


	public decimal Quantity
	{
		get
		{
			return quantity;
		}

		set
		{
			quantity = value;
		}
	}

public int PageNumber
	{
		get
		{
			return pageNumber;
		}

		set
		{
			pageNumber = value;
		}
	}
	public decimal TotalPrice
	{
		get
		{
			return totalPrice;
		}

		set
		{
			totalPrice = value;
		}
	}

	public bool Reversed
	{
		get
		{
			return arabicText;
		}

		set
		{
			arabicText = value;
		}
	}

	public string ProductName
	{
		get
		{
			return GetName();
		}
	}

	private string GetName()
	{
		StringBuilder sb = new StringBuilder();
		// if (!arabicText)
		// {
		// 	foreach (string p1 in product)
		// 	{
		// 		sb.Append(p1);
		// 	}
		// }
		// else
		// {
		// 	Array ar = product.ToArray();
		// 	for (int i = ar.Length - 1; i >= 0; i--)
		// 	{
		// 		sb.Append(ar.GetValue(i));
		// 	}
		// }
		// while (product.Count > 0)
		// {
		// 	XText item = product.Dequeue();
		// 	sb.Append(item.Text);
		// 	Console.WriteLine($"x: {item.x}, Text: {item.Text}, y: {y}");
		// }
		foreach (var p1 in product)
			{
				sb.Append(p1.Text);
				// Console.WriteLine("-- {0}:{1} --", p1.x,p1.Text);
			}
		// Console.WriteLine(sb.ToString());

		return sb.ToString();

	}

	public void AddProductChunck(XText nameChunck)
	{
		this.product.Enqueue(nameChunck);
	}

	public override int GetHashCode()
	{
		return y.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return obj is Record && Equals((Record)obj);
	}

	public bool Equals(Record r)
	{
		return y == r.y && pageNumber==r.pageNumber;
	}

	public override string ToString()
	{
		//return string.Format("{4}-Product name={0},Unit Price={1}, Quantity = {2},Total Price={3}", ProductName, price, quantity, totalPrice, y);
        return string.Format("{4}-Product name={0}, Unit Id  ={1}  Unit = {2} ,Unit Price={3}, Quantity = {4},Total Price={5}", ProductName, Code, unit ,price, quantity, totalPrice, y);
    }
}
