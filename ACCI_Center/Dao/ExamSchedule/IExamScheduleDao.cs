using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.FilterField;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACCI_Center.Dao.ExamSchedule
{
    public interface IExamScheduleDao
    {
        public Dto.PagedResult<Entity.Test> GetTests(
            int pageSize,
            int currentPageNumber,
            TestFilterObject testFilterObject);
    }
}
