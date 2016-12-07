

namespace YouduSDK.EntApp.Exceptions
{
    public class UnexpectedException : System.Exception
    {
        public UnexpectedException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
