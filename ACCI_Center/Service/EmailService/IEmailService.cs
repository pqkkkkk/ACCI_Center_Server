using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ACCI_Center.Service.EmailService
{
   public interface IEmailService
   {
       bool SendEmail(Entity.CandidateInformation candidate);
   }
}
