using AltaSemester.Data.Dtos;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;

namespace AltaSemester.Service.Utils.Mailer
{
    public class Mail
    {
        public static async Task<ModelResult> SendMailResetPassword(string email, string password, IConfiguration _config)
        {
            var _result = new ModelResult();
            try
            {
                var mail = new MimeMessage();
                mail.From.Add(MailboxAddress.Parse(_config["EmailConfig:Email"]));
                mail.To.Add(MailboxAddress.Parse(email));
                mail.Subject = "Reset password";
                var pass = $"{password}";
                mail.Body = new TextPart(TextFormat.Html)
                {
                    Text = $"Your password is: {pass}"
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config["EmailConfig:smtp"], int.Parse(_config["EmailConfig:SmtpPort"]), SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config["EmailConfig:Email"], _config["EmailConfig:EmailPassword"]);

                await smtp.SendAsync(mail);
                await smtp.DisconnectAsync(true);

                _result.Success = true;
                _result.Message = "Email sent successfully";
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Message = ex.ToString();
            }
            return _result;
        }
    }
}
