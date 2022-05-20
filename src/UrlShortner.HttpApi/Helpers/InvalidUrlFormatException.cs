namespace UrlShortner.HttpApi.Endpoints
{
    public partial class CreateShortUrl
    {
        public class InvalidUrlFormatException : Exception
        {
            public InvalidUrlFormatException(string? message) : base(message)
            {
            }
        }
    }
}
