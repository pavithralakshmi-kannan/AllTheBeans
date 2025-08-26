namespace AllTheBeans.Models;

public class BotdAssignment
{
    public int Id { get; set; }
    // stored as date-only (time portion ignored)
    public DateTime Date { get; set; }
    public int BeanId { get; set; }
    public Bean? Bean { get; set; }
}
