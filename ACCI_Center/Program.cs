using ACCI_Center.Configuraion;
using ACCI_Center.Configuration;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.ExtensionInformation;
using ACCI_Center.Dao.Invoice;
using ACCI_Center.Dao.RegisterInformation;
using ACCI_Center.Service.ExamSchedule;
using ACCI_Center.Service.Payment;
using ACCI_Center.Service.RegisterInformation;
using ACCI_Center.Service.TTDangKy;
using ACCI_Center.Service.TTGiaHan;
using Microsoft.Data.SqlClient;
using ACCI_Center.Service.EmailService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Add daos and services to the container.
builder.Services.AddSingleton<IDataClient, DataClient>();
builder.Services.AddSingleton<IExamScheduleDao, ExamScheduleSqlDao>();
builder.Services.AddSingleton<IRegisterInformationDao, RegisterInformationSqlDao>();
builder.Services.AddSingleton<IInvoiceDao, InvoiceSqlDao>();
builder.Services.AddSingleton<IExtensionInformationDao, ExtensionInformationSqlDao>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddSingleton<IExamScheduleService, ExamScheduleService>();
builder.Services.AddSingleton<IRegisterInformationService, RegisterInformationService>();
builder.Services.AddSingleton<IOrganizationRegisterInformationService, OrganizationRegisterInformationService>();
builder.Services.AddSingleton<IExtensionInformationService, ExtensionInformationService>();
builder.Services.AddSingleton<IEmailService, EmailService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
