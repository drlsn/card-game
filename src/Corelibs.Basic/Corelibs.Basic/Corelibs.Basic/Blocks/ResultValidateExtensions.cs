namespace Corelibs.Basic.Blocks
{
    public static class ResultValidateExtensions
    {
        public static bool ValidateSuccessAndValues(this Result result)
        {
            if (!result.IsSuccess)
                return false;

            if (!result.Values.Any(v => v == null))
                return true;

            result.AddError("Result contains empty values");
            return false;
        }
    }
}
