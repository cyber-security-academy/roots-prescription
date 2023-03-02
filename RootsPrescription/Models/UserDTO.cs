namespace RootsPrescription.Models;

public class UserDTO
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string NationalIdNumber { get; set; }
    public string Name { get; set; }
    public string PostalAddress { get; set; }

    public PrescriptionDTO[] Prescriptions { get; set; }
    public InvoiceDTO[] Invoices{ get; set; }

}
