using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.Dao;

namespace ACCI_Center.Service
{
    public class PaginationService<T>
        where T : class
    {
        private readonly IBaseDao<T> dao;
        public List<T> LoadData(int itemsPerPage, int currentPage)
        {
            List<T> result = dao.LoadData(itemsPerPage, currentPage);

            return result;
        }
    }
}
