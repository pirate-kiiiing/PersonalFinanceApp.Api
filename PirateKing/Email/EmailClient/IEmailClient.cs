using System.Threading.Tasks;

namespace PirateKing.Email
{
    public interface IEmailClient
    {
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
        Task SendAsync(
            string subject,
            string fromAddress,
            string fromPassword,
            string fromName,
            string toAddress,
            string toName,
            string body);

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
        Task SendAttachmentAsync(
            string subject,
            string fromAddress,
            string fromPassword,
            string fromName,
            string toAddress,
            string toName,
            string body,
            string attachment,
            string fileName);
    }
}
