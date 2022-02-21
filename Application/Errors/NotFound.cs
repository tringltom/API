namespace Application.Errors
{
    public class NotFound : RestException
    {
        public NotFound(string message) : base(System.Net.HttpStatusCode.NotFound, new { error = message })
        {

        }
    }
}
