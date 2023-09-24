using Vonage.Request;

namespace Vonage
{
    internal class ApiRequest : Credentials
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}