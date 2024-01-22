namespace Corelibs.Basic.Blocks
{
    public class Remark
    {
        private string _text;

        public static Remark Create(string message)
        {
            return new Remark(message);
        }

        public Remark(string text)
        {
            _text = text;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_text))
                return string.Empty;

            return _text;
        }
    }
}
