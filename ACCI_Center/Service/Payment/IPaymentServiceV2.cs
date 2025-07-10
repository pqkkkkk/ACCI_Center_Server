using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;

namespace ACCI_Center.Service.Payment
{
    public interface IPaymentServiceV2
    {
        public CreateExamScheduleResponse CreateInvoice(CreateInvoiceRequest request);
        public UpdateInvoiceResponse UpdateInvoice(UpdateInvoiceRequest request);
    }
}
