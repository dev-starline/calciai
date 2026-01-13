using CalciAI.Models;

namespace CalciAI.Persistance
{
    public static class ErrorHelper
    {
        public static ProcessResult ToProcessResult(this Error error)
        {
            return ProcessResult.Fail(error.Code, error.Message);
        }
    }
}
