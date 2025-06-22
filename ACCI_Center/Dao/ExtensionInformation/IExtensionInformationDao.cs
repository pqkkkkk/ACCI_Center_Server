using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Dao.ExtensionInformation
{
    public interface IExtensionInformationDao
    {
        public int AddExtensionInformation(Entity.ExtensionInformation extension);
        public List<Entity.ExtensionInformation> LoadExtendInformation(int pageSize, int currentPageNumber, FilterField.ExtensionInformationFilterObject filterObject);
    }
}
