namespace ACCI_Center.Dto.Response
{
    public class UpdateRegisterInformationResponse
    {
        public Entity.RegisterInformation registerInformation { get; set; }
        public int statusCode { get; set; } = 500;
        public string message { get; set; } = "An error occurred while updating the registration information.";
    }
}
