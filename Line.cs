//using AutoMapper;
class Line
{
	//public int Y { get; set; } 

	

	public string itemName { get; set; }
	public decimal quantity { get; set; }

	public decimal unitPrice { get; set; }

	// public string Unit { get; set; }

	public decimal totalPrice { get; set; }

	public string itemUnit { get; set; }
    public string itemId { get; set; }

    public override string ToString()
	{
		return string.Format("{6}-Product name={0}, Unit Price ={1} ,Quantity ={2},Total Price  = {3},item Unit ={4} ,Unit Id = {5}", itemName, unitPrice, quantity, totalPrice, itemUnit, itemId, 0);
	}
}