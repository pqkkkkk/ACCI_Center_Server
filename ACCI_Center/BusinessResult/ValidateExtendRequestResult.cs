using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.BusinessResult
{
    public enum ValidateExtendRequestResult
    {
        Ok,
        TooLate,
        ExceedExtendTimeLimit,
        DesiredExamDateNotAvailable,
    }
}
