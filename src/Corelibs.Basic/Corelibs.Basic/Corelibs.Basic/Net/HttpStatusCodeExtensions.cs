using System.Net;

namespace Corelibs.Basic.Net
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccess(this HttpStatusCode statusCode)
        {
            int numericValue = (int) statusCode;
            return numericValue >= 200 && numericValue <= 299;
        }
    }
}
