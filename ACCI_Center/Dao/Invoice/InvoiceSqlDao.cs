using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.Configuraion;

namespace ACCI_Center.Dao.Invoice
{
    public class InvoiceSqlDao : IInvoiceDao
    {
        private readonly IDataClient dataClient;

        public InvoiceSqlDao(IDataClient dataClient)
        {
            this.dataClient = dataClient;
        }
        private DbParameter[] BuildParametersForAddInvoice(Entity.Invoice invoice, DbConnection dbConnection)
        {
            var parameters = new List<DbParameter>();
            
            var thoiDiemTaoParam = dbConnection.CreateCommand().CreateParameter();
            thoiDiemTaoParam.ParameterName = "@ThoiDiemTao";
            thoiDiemTaoParam.Value = invoice.ThoiDiemTao;
            parameters.Add(thoiDiemTaoParam);

            var thoiDiemThanhToanParam = dbConnection.CreateCommand().CreateParameter();
            thoiDiemThanhToanParam.ParameterName = "@ThoiDiemThanhToan";
            thoiDiemThanhToanParam.Value = invoice.ThoiDiemThanhToan == null ? DBNull.Value : invoice.ThoiDiemThanhToan;
            parameters.Add(thoiDiemThanhToanParam);

            var tongTienParam = dbConnection.CreateCommand().CreateParameter();
            tongTienParam.ParameterName = "@TongTien";
            tongTienParam.Value = invoice.TongTien;
            parameters.Add(tongTienParam);

            var trangThaiParam = dbConnection.CreateCommand().CreateParameter();
            trangThaiParam.ParameterName = "@TrangThai";
            trangThaiParam.Value = invoice.TrangThai;
            parameters.Add(trangThaiParam);

            var loaiHoaDonParam = dbConnection.CreateCommand().CreateParameter();
            loaiHoaDonParam.ParameterName = "@LoaiHoaDon";
            loaiHoaDonParam.Value = invoice.LoaiHoaDon;
            parameters.Add(loaiHoaDonParam);

            var maTTDangKyParam = dbConnection.CreateCommand().CreateParameter();
            maTTDangKyParam.ParameterName = "@MaTTDangKy";
            maTTDangKyParam.Value = invoice.MaTTDangKy;
            parameters.Add(maTTDangKyParam);

            var maTTGiaHanParam = dbConnection.CreateCommand().CreateParameter();
            maTTGiaHanParam.ParameterName = "@MaTTGiaHan";
            maTTGiaHanParam.Value = invoice.MaTTGiaHan <= 0 ? DBNull.Value : invoice.MaTTGiaHan;
            parameters.Add(maTTGiaHanParam);

            return parameters.ToArray();
        }
        public int AddInvoice(Entity.Invoice invoice)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                try
                {
                    string sql = """
                INSERT INTO ACCI_Center.dbo.HOADON (ThoiDiemTao, ThoiDiemThanhToan, TongTien, TrangThai, LoaiHoaDon, MaTTDangKy, MaTTGiaHan)
                VALUES (@ThoiDiemTao, @ThoiDiemThanhToan, @TongTien, @TrangThai, @LoaiHoaDon, @MaTTDangKy, @MaTTGiaHan);
                SELECT CAST(SCOPE_IDENTITY() AS int);
                """;

                    DbParameter[] parameters = BuildParametersForAddInvoice(invoice, dbConnection);

                    if (dbConnection.State != System.Data.ConnectionState.Open)
                    {
                        dbConnection.Open();
                    }
                    using (var command = dbConnection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.AddRange(parameters);

                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while adding invoice: " + ex.Message, ex);
                }
            }
        }
    }
}
