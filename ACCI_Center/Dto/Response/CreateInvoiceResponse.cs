namespace ACCI_Center.Dto.Response
{
    public class CreateInvoiceResponse
    {
        public Entity.Invoice invoice { get; set; }
        public int statusCode { get; set; } = 500;
        public string message { get; set; } = "An error occurred while creating the invoice.";
    }
}
