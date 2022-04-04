namespace Application.Errors
{
    public class BadRequest : RestException
    {
        public BadRequest(string message) : base(System.Net.HttpStatusCode.BadRequest, new { error = message })
        {

        }
    }
}
