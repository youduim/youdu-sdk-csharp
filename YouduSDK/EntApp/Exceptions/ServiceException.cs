

namespace YouduSDK.EntApp.Exceptions
{
    public class ServiceException : System.Exception
    {
        public ServiceException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
