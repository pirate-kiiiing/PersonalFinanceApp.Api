namespace PirateKing.Email
{
    public class OutlookEmailClient : EmailClient
    {
        private const string host = "smtp-mail.outlook.com";
        private const int port = 587;

        public OutlookEmailClient() : base(host, port)
        { 
        }
    }
}
