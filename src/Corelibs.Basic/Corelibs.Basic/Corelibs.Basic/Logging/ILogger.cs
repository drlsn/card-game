namespace Corelibs.Basic.Logging
{
    public interface ILogger
    {
        void Log(string message);
        void Log(object @object);
        void Log(object @object, string message);
    }
}
