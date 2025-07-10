namespace ACCI_Center.Dto.Request
{
    public class ApproveOrganizationRegisterRequest
    {
        public List<int> supervisorIds { get; set; } = [];
        public int roomId { get; set; } = -1;
    }
}
