namespace ACCI_Center.Dao.ExamSchedule
{
    public interface IExamScheduleDaoV2
    {
        public int AddExamSchedule(Entity.ExamSchedule examSchedule,
                                    List<int> supervisorIds);
        public int UpdateExamSchedule(Entity.ExamSchedule examSchedule,
                                   List<int> supervisorIds);

        public List<Entity.Room> GetAllEmptyRoomIds(DateTime desiredExamTime, int testId);
        public List<Entity.Employee> GetAllFreeEmployeeIds(DateTime desiredExamTime, int testId);

    }
}
