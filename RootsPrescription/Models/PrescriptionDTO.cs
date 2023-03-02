namespace RootsPrescription.Models;

public class PrescriptionDTO
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int DocNo { get; set; }
    public DateTime PrescriptionDate { get; set; }
    public string Medication { get; set; }
    public string Filename { get; set; }
}

