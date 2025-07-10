namespace ACCI_Center.Dao.ExamSchedule
{
    public interface IExamScheduleDaoV2
    {
        public int AddExamSchedule(Entity.ExamSchedule examSchedule,
                                    List<int> supervisorIds);
    }
}
