namespace AllTheBeans.Models;

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = "";
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public int BeanId { get; set; }
    public Bean? Bean { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
