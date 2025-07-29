namespace ACCI_Center.Dto.Response
{
    public class AvailableRoomsResponse
    {
        public List<Entity.Room> data { get; set; } = new List<Entity.Room>();
        public int statusCode { get; set; } = StatusCodes.Status200OK;
        public string message { get; set; } = "Success";
    }
}
