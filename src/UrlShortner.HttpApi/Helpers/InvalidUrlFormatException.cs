namespace UrlShortner.HttpApi.Endpoints
{
    public class InvalidUrlFormatException : Exception
    {
        public InvalidUrlFormatException(string? message) : base(message)
        {
        }
    }
}
