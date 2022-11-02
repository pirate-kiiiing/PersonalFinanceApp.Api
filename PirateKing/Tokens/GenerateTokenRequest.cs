using System;

namespace PirateKing.Tokens
{
    public class GenerateTokenRequest
    {
        public string TenantId { get; set; }

        public string UserId { get; set; }

        public string Role { get; set; }

        public TimeSpan TimeSpan { get; set; }
    }
}
