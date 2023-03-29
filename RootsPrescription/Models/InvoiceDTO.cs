namespace RootsPrescription.Models;
public class InvoiceDTO
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int InvoiceNo { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Filename { get; set; }
}

