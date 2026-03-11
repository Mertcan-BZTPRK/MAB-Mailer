using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MAB_Mailer
{
    public class MailService
    {
        public async Task<bool> SendEmailAsync(string to, string subject, string body, string cc = "", string customFromEmail = null)
        {
            try
            {
                string htmlBody = body.Replace("\n", "<br/>");

                using (var smtp = new SmtpClient(GlobalSettings.SmtpHost, 587))
                {
                    smtp.EnableSsl = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Credentials = new NetworkCredential(GlobalSettings.UserMail, GlobalSettings.UserPass);

                    var mail = new MailMessage();

                    // --- GÖNDEREN AYARI (SPOOFING) ---
                    string fromAddress = string.IsNullOrEmpty(customFromEmail)
                                         ? GlobalSettings.UserMail
                                         : customFromEmail;

                    string senderName = string.IsNullOrEmpty(GlobalSettings.UserDisplayName)
                                        ? "MAB TECH Mailer"
                                        : GlobalSettings.UserDisplayName;

                    mail.From = new MailAddress(fromAddress, senderName);
                    // ---------------------------------

                    foreach (var address in to.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        mail.To.Add(address.Trim());
                    }

                    if (!string.IsNullOrEmpty(cc))
                    {
                        foreach (var address in cc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            mail.CC.Add(address.Trim());
                        }
                    }

                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    mail.Body = $@"
                        <div style='font-family: Arial, sans-serif; font-size: 14px; line-height: 1.6; color: #333;'>
                            {htmlBody}
                        </div>";

                    await smtp.SendMailAsync(mail);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}