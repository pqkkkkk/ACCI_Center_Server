using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ACCI_Center.Entity;
using MimeKit;
using MailKit.Security;
using RazorLight;
using ACCI_Center.Configuration;
using PuppeteerSharp;
using PuppeteerSharp.Media;
namespace ACCI_Center.Service.EmailService
{
   public class EmailService : IEmailService
   {       
       private readonly MailSettings _mailSettings;
       public EmailService(IOptions<MailSettings> mailSettings)
       {
           _mailSettings = mailSettings.Value;
       }
       public bool SendEmail(CandidateInformation candidate)
       {
           // 1. Render Razor HTML
           var engine = new RazorLightEngineBuilder()
               .UseFileSystemProject(Directory.GetCurrentDirectory())
               .UseMemoryCachingProvider()
               .Build();
           string templatePath = "ExamRegisterForm.cshtml";
           string htmlContent = engine.CompileRenderAsync(templatePath, candidate).Result;


           // 2. Tạo PDF
           new BrowserFetcher().DownloadAsync().GetAwaiter().GetResult();            
           using var browser = Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }).GetAwaiter().GetResult();
           using var page = browser.NewPageAsync().GetAwaiter().GetResult();


           page.SetContentAsync(htmlContent).GetAwaiter().GetResult();
           var pdfBytes = page.PdfDataAsync(new PdfOptions
           {
               Format = PaperFormat.A4, // dùng enum cho bản cũ
               PrintBackground = true
           }).GetAwaiter().GetResult();


           // 3. Tạo email và đính kèm PDF
           var email = new MimeMessage();
           // email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
           email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
           email.To.Add(MailboxAddress.Parse("qthong2004@gmail.com"));
           email.Subject = "Phiếu Dự Thi";
           var builder = new BodyBuilder
           {
               HtmlBody = $"""
                   <p>Xin chào {candidate.HoTen},</p>
                   <p>Trung tâm thông tin đến bạn Phiếu dự thi cho kì thi sắp tới</p>
                   <p>Vui lòng in phiếu dự thi và mang theo cùng giấy tờ cá nhân khi đi thi.</p>
                   <p>Trân trọng,<br/>ACCI_CENTER</p>
               """
           };
           builder.Attachments.Add("ExamRegisterForm.pdf", pdfBytes, new ContentType("application", "pdf"));
           email.Body = builder.ToMessageBody();


           using var smtp = new MailKit.Net.Smtp.SmtpClient();
           try
           {
               smtp.Connect(_mailSettings.Host, int.Parse(_mailSettings.Port), SecureSocketOptions.StartTls);
               smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
               smtp.Send(email);
               smtp.Disconnect(true);
               return true;
           }
           catch (Exception e)
           {
               Console.WriteLine(e.Message);
               return false;
           }
       }
   }
}