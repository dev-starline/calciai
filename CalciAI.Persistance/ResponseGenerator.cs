using System;
using CalciAI.Models;

namespace CalciAI.Persistance
{
    public static class ResponseGenerator
    {
        public static ProcessResult Generator(string result, string errorCode)
        {
            if (result != "")
            {
                var splitedresult = result.Split("~");

                if (splitedresult.Length == 4)
                {
                    if (splitedresult[0].ToString() == "TRUE")
                    {
                        return ProcessResult.Success(Convert.ToInt32(splitedresult[1]), Convert.ToInt64(splitedresult[2]), splitedresult[3].ToString());
                    }

                    return ProcessResult.Fail(errorCode, splitedresult[3].ToString());
                }
                else
                {
                    return ProcessResult.Fail(errorCode, "Invalid Response !!");
                }
            }

            return ProcessResult.Fail(errorCode, "Result not found!!");
        }
    }
}
