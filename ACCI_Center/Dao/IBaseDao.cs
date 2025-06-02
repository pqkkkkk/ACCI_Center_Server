using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Dao
{
    public interface IBaseDao<T>
        where T : class
    {
        List<T> LoadData(int itemsPerPage, int currentPage);
    }
}
