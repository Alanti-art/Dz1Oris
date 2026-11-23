using CustomHttpServer.Framework.Attributes;
using MailKit.Net.Smtp;
using MailKit.Security;
using MailKit.Security;
using MimeKit;
using MimeKit;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.Аttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Controlers
{
    [HttpController("home-work")]
    public class SendController : ControllerBase
    {
        [GET("send")]
        public void Send(string to)
        {


            string login = "Sveta22052006@yandex.ru";
            string password = "gsekdnviqkrsfvcn";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sveta", login));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = $"Проверка связи";


            BodyBuilder builder = new BodyBuilder
            {
                HtmlBody = $"<h2>Hello World!</h2>" +
                $" <p>Группа: 11-541 </p>" +
                $" <p>ФИО: Лахнова Светлана Александровна </p>"

            };
            builder.Attachments.Add("ServerZip.zip");
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            client.Connect("smtp.yandex.ru", 465, SecureSocketOptions.SslOnConnect);

            client.Authenticate(login, password);
            client.Send(message);
            client.Disconnect(true);
            Console.WriteLine($"Письмо отправлено на {to}"); ;
        }

    }
}
