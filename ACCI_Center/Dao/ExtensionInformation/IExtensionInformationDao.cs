using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.Dto;

namespace ACCI_Center.Dao.ExtensionInformation
{
    public interface IExtensionInformationDao
    {
        public int AddExtensionInformation(Entity.ExtensionInformation extension);
        public PagedResult<Entity.ExtensionInformation> LoadExtendInformation(int pageSize, int currentPageNumber, FilterField.ExtensionInformationFilterObject filterObject);
        public int GetExtensionTime(int registerInformationId);
    }
}
