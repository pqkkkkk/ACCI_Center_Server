using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Dao.Invoice
{
    public interface IInvoiceDao
    {
        public int AddInvoice(Entity.Invoice invoice);
    }
}
