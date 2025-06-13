using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.ExtensionInformation;
using ACCI_Center.Dao.Invoice;
using ACCI_Center.Dao.RegisterInformation;
using ACCI_Center.FilterField;
using ACCI_Center.Entity;

namespace ACCI_Center.Service.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IInvoiceDao invoiceDao;
        private readonly IRegisterInformationDao registerInformationDao;
        private readonly IExtensionInformationDao extensionInformationDao;

        public PaymentService(
            IInvoiceDao invoiceDao,
            IRegisterInformationDao registerInformationDao,
            IExtensionInformationDao extensionInformationDao)
        {
            this.invoiceDao = invoiceDao;
            this.registerInformationDao = registerInformationDao;
            this.extensionInformationDao = extensionInformationDao;
        }
        public List<Invoice> GetInvoiceById(int maHoaDon)
        {
            throw new NotImplementedException();
        }

        public List<Invoice> LoadInvoices()
        {
            throw new NotImplementedException();
        }
        public int PayForExtensionFee(int maTTGiaHan)
        {
            throw new NotImplementedException();
        }

        public int PayForOrganizationRegistration(int maTTDangKy)
        {
            throw new NotImplementedException();
        }

        public int PayForIndividualRegistration(int maTTDangKy)
        {
            throw new NotImplementedException();
        }

        public ValidatePaymentRequestResult ValidatePaymentRequest(int maTTDangKy)
        {
            throw new NotImplementedException();
        }

        public List<Invoice> LoadInvoices(int pageSize, int currentPageNumber, InvoiceFilterObject invoiceFilterObject)
        {
            throw new NotImplementedException();
        }
    }
}
