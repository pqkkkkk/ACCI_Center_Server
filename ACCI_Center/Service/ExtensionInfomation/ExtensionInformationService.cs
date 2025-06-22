using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.Invoice;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.RegisterInformation;
using ACCI_Center.Dao.ExtensionInformation;
using ACCI_Center.FilterField;
using ACCI_Center.Entity;

namespace ACCI_Center.Service.TTGiaHan
{
    public class ExtensionInformationService : IExtensionInformationService
    {
        private IExtensionInformationDao TTGiaHanDao;
        private IRegisterInformationDao TTDangKyDao;
        private IExamScheduleDao LichThiDao;
        private IInvoiceDao HoaDonDao;

        public ExtensionInformationService(IExtensionInformationDao ttGiaHanDao, IRegisterInformationDao ttDangKyDao, IExamScheduleDao lichThiDao, IInvoiceDao hoaDonDao)
        {
            TTGiaHanDao = ttGiaHanDao;
            TTDangKyDao = ttDangKyDao;
            LichThiDao = lichThiDao;
            HoaDonDao = hoaDonDao;
        }
        public int ExtendExamTimeFree(Entity.ExtensionInformation TTGiaHan)
        {
            throw new NotImplementedException();
        }

        public int ExtendExamTimePaid(Entity.ExtensionInformation TTGiaHan, int MaLichThiMoi)
        {
            if (TTGiaHan == null || MaLichThiMoi <= 0) { return 0; }
            int maTTGiaHan = TTGiaHanDao.AddExtensionInformation(TTGiaHan);
            if (maTTGiaHan <= 0) { return 0; }

            Invoice invoice = new Invoice
            {
                ThoiDiemTao = DateTime.Now,
                TongTien = TTGiaHan.PhiGiaHan,
                TrangThai = "Chưa thanh toán",
                LoaiHoaDon = "Gia hạn thi",
                MaTTDangKy = TTGiaHan.MaTTDangKy,
                MaTTGiaHan = maTTGiaHan
            };
            if (TTDangKyDao.UpdateExamSchedule(TTGiaHan.MaTTDangKy, MaLichThiMoi) > 0
                && HoaDonDao.AddInvoice(invoice) > 0) return 1;
            return 0;
        }

        public List<Entity.ExtensionInformation> LoadExtendInformation()
        {
            throw new NotImplementedException();
        }

        public List<ExtensionInformation> LoadExtendInformation(int pageSize, int currentPageNumber, ExtensionInformationFilterObject extensionInformationFilterObject)
        {
            return TTGiaHanDao.LoadExtendInformation(pageSize, currentPageNumber, extensionInformationFilterObject);
        }

        public Entity.ExtensionInformation LoadExtendInformationById(int maTTGiaHan)
        {
            throw new NotImplementedException();
        }

        public ValidateExtendRequestResult ValidateExtensionRequest(int maTTDangKy, DateTime desiredExamDate)
        {
            throw new NotImplementedException();
        }
    }
}
