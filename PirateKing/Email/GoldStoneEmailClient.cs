using System;
using System.Threading.Tasks;
using PirateKing.Guards;
using PirateKing.Models;

namespace PirateKing.Email
{
    public class PirateKingEmailClient : IPirateKingEmailClient
    {
        private readonly IEmailClient emailClient;
        private readonly EmailSecrets emailSecrets;

        public PirateKingEmailClient(
            IEmailClient emailClient,
            EmailSecrets emailSecrets)
        {
            Validate.NotNull(emailClient, nameof(emailClient));
            Validate.NotNull(emailSecrets, nameof(emailSecrets));

            this.emailClient = emailClient;
            this.emailSecrets = emailSecrets;
        }

        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="fromName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task SendAsync(
            string subject,
            string fromName,
            string body)
        {
            Validate.NotNullOrEmpty(subject, nameof(subject));
            Validate.NotNullOrEmpty(fromName, nameof(fromName));

            await emailClient.SendAsync(
                subject: subject,
                fromAddress: emailSecrets.FromAddress,
                fromPassword: emailSecrets.FromPassword,
                fromName: fromName,
                toAddress: emailSecrets.ToAddress,
                toName: emailSecrets.ToName,
                body: body);
        }

        /// <summary>
        /// Sends email with an attachment
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="fromName"></param>
        /// <param name="body"></param>
        /// <param name="attachment"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task SendAttachmentAsync(
            string subject,
            string fromName,
            string body,
            string attachment,
            string fileName)
        {
            Validate.NotNullOrEmpty(subject, nameof(subject));
            Validate.NotNullOrEmpty(fromName, nameof(fromName));
            Validate.NotNullOrEmpty(attachment, nameof(attachment));
            Validate.NotNullOrEmpty(fileName, nameof(fileName));

            await emailClient.SendAttachmentAsync(
                subject: subject,
                fromAddress: emailSecrets.FromAddress,
                fromPassword: emailSecrets.FromPassword,
                fromName: fromName,
                toAddress: emailSecrets.ToAddress,
                toName: emailSecrets.ToName,
                body: body,
                attachment: attachment,
                fileName: fileName);
        }
    }

    public interface IPirateKingEmailClient
    {
        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="fromName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task SendAsync(
            string subject,
            string fromName,
            string body);

        /// <summary>
        /// Sends email with an attachment
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="fromName"></param>
        /// <param name="body"></param>
        /// <param name="attachment"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task SendAttachmentAsync(
            string subject,
            string fromName,
            string body,
            string attachment,
            string fileName);
    }
}
