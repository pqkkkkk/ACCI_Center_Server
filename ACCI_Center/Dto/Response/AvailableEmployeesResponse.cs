namespace ACCI_Center.Dto.Response
{
    public class AvailableEmployeesResponse
    {
        public List<Entity.Employee> data { get; set; } = new List<Entity.Employee>();
        public int statusCode { get; set; } = StatusCodes.Status200OK;
        public string message { get; set; } = "Success";
    }
}
