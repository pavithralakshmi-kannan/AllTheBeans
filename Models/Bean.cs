namespace AllTheBeans.Models;

public class Bean
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = "";
    public int Index { get; set; }
    public bool IsBOTD { get; set; }
    public string Name { get; set; } = "";
    public string Colour { get; set; } = "";
    public string Country { get; set; } = "";
    public string Description { get; set; } = "";
    public string Image { get; set; } = "";
    public decimal Cost { get; set; }
}
