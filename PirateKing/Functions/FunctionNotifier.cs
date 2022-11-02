using System;
using System.Threading.Tasks;
using PirateKing.Email;
using Microsoft.Extensions.Logging;

namespace PirateKing.Functions
{
    public static class FunctionNotifier
    {
        public static async Task SendEmailAsync(
            IPirateKingEmailClient emailClient,
            ILogger log,
            string subject,
            string fromName,
            string body)
        {
            try
            {
                await emailClient.SendAsync(
                    subject: subject,
                    fromName: fromName,
                    body: body);
            }
            catch (Exception e)
            {
                log.LogError($"An error occurred while sending an email: {e.Message}");
            }
        }
    }
}
