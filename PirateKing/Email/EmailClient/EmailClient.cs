using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using PirateKing.Guards;

namespace PirateKing.Email
{
    public abstract class EmailClient : IEmailClient
    {
        private readonly SmtpClient smtp;

        public EmailClient(
            string host, 
            int port,
            bool enableSsl = true,
            SmtpDeliveryMethod deliveryMethod = SmtpDeliveryMethod.Network,
            int timeout = 20000,
            bool UseDefaultCredentials = false)
        {
            smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = enableSsl,
                DeliveryMethod = deliveryMethod,
                Timeout = timeout,
                UseDefaultCredentials = UseDefaultCredentials,
            };
        }

        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="fromAddress"></param>
        /// <param name="fromPassword"></param>
        /// <param name="fromName"></param>
        /// <param name="toAddress"></param>
        /// <param name="toName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public Task SendAsync(
            string subject, 
            string fromAddress, 
            string fromPassword, 
            string fromName,
            string toAddress, 
            string toName, 
            string body)
        {
            Validate.NotNullOrEmpty(subject, nameof(subject));
            Validate.NotNullOrEmpty(fromAddress, nameof(fromAddress));
            Validate.NotNullOrEmpty(fromPassword, nameof(fromPassword));
            Validate.NotNullOrEmpty(fromName, nameof(fromName));
            Validate.NotNullOrEmpty(toAddress, nameof(toAddress));
            Validate.NotNullOrEmpty(toName, nameof(toName));

            return Task.FromResult(Send(subject, fromAddress, fromPassword, fromName, toAddress, toName, body));
        }

        private bool Send(
            string subject,
            string fromAddress,
            string fromPassword,
            string fromName,
            string toAddress,
            string toName,
            string body)
        {
            var fromMailAddress = new MailAddress(fromAddress, fromName);
            var toMailAddress = new MailAddress(toAddress, toName);

            smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);

            var message = new MailMessage(fromMailAddress, toMailAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            smtp.Send(message);
            
            return true;
        }

        /// <summary>
        /// Sends email with an attachment
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="fromAddress"></param>
        /// <param name="fromPassword"></param>
        /// <param name="fromName"></param>
        /// <param name="toAddress"></param>
        /// <param name="toName"></param>
        /// <param name="body"></param>
        /// <param name="attachment"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Task SendAttachmentAsync(
            string subject,
            string fromAddress,
            string fromPassword,
            string fromName,
            string toAddress,
            string toName,
            string body,
            string attachment,
            string fileName)
        {
            Validate.NotNullOrEmpty(subject, nameof(subject));
            Validate.NotNullOrEmpty(fromAddress, nameof(fromAddress));
            Validate.NotNullOrEmpty(fromPassword, nameof(fromPassword));
            Validate.NotNullOrEmpty(fromName, nameof(fromName));
            Validate.NotNullOrEmpty(toAddress, nameof(toAddress));
            Validate.NotNullOrEmpty(toName, nameof(toName));
            Validate.NotNullOrEmpty(attachment, nameof(attachment));
            Validate.NotNullOrEmpty(fileName, nameof(fileName));

            return Task.FromResult<bool>(SendAttachment(subject, fromAddress, fromPassword, fromName, toAddress, toName, body, attachment, fileName));
        }

        private bool SendAttachment(
            string subject, 
            string fromAddress, 
            string fromPassword, 
            string fromName,
            string toAddress, 
            string toName, 
            string body, 
            string attachment, 
            string fileName)
        {
            var fromMailAddress = new MailAddress(fromAddress, fromName);
            var toMailAddress = new MailAddress(toAddress, toName);

            smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);

            using (var message = new MailMessage(fromMailAddress, toMailAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                var memoryStream = new MemoryStream(ASCIIEncoding.Default.GetBytes(attachment));
                var data = new Attachment(memoryStream, fileName, MediaTypeNames.Text.Html);

                message.IsBodyHtml = true;
                message.Attachments.Add(data);

                smtp.Send(message);
            }

            return true;
        }
    }
}
