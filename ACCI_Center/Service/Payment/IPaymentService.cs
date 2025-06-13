using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.FilterField;

namespace ACCI_Center.Service.Payment
{
    public interface IPaymentService
    {
        public ValidatePaymentRequestResult ValidatePaymentRequest(int maTTDangKy);
        public int PayForIndividualRegistration(int maTTDangKy);
        public int PayForOrganizationRegistration(int maTTDangKy);
        public int PayForExtensionFee(int maTTGiaHan);
        public List<Entity.Invoice> LoadInvoices();
        public List<Entity.Invoice> GetInvoiceById(int maHoaDon);
        public List<Entity.Invoice> LoadInvoices(int pageSize, int currentPageNumber, InvoiceFilterObject invoiceFilterObject);
    }
}
