using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Dto.Request
{
    public class ExtensionRequest
    {
        public Entity.ExtensionInformation extensionInformation { get; set; }
        public int newExamScheduleId { get; set; }
    }
}
