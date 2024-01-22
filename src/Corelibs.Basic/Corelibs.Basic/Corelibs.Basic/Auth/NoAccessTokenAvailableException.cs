namespace Corelibs.Basic
{
    [Serializable]
    public class NoAccessTokenAvailableException : Exception
    {
        public NoAccessTokenAvailableException(string? message) : base(message)
        {
        }
    }
}
