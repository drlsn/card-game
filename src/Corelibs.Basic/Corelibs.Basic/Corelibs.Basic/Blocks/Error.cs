using System;

namespace Corelibs.Basic.Blocks
{
    public class Error
    {
        public Exception Exception { get; }
        private string _text { get; }

        public static Error Create(string text)
        {
            return new Error(text);
        }

        public static Error Create(Exception exception)
        {
            return new Error(exception);
        }

        public static Error Create()
        {
            return new Error();
        }

        public Error()
        {

        }

        public Error(string text)
        {
            _text = text;
        }

        public Error(Exception exception)
        {
            Exception = exception;
        }

        public override string ToString()
        {
            if (HasException)
                return Exception.ToString();

            if (!string.IsNullOrEmpty(_text))
                return _text;

            return string.Empty;
        }

        public bool HasException => Exception != null;
        public bool HasMessage => HasException || !string.IsNullOrEmpty(_text);
    }
}
