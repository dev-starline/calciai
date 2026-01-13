using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    public class ProcessResult<T, E> where T : IModel where E : IException
    {
        public static ProcessResult<T, E> Success(T data)
        {
            return new ProcessResult<T, E>
            {
                Data = data
            };
        }

        public static ProcessResult<T, E> Success(T data, long returnId, int action, string successMessage)
        {
            return new ProcessResult<T, E>
            {
                Data = data,
                ReturnID = returnId,
                Action = action,
                SuccessMessage = successMessage
            };
        }

        public static ProcessResult<T, E> Fail(E exception)
        {
            return new ProcessResult<T, E>
            {
                Exception = exception
            };
        }

        public static ProcessResult<T, E> Fail(string code, string message)
        {
            return new ProcessResult<T, E>
            {
                Exception = (E)Activator.CreateInstance(typeof(E), new object[] { code, message })
            };
        }

        public T Data { get; set; }

        [JsonPropertyName("exception")]
        public E Exception { get; set; }

        public string SuccessMessage { get; set; }
        
        public long ReturnID { get; set; }
        
        public int Action { get; set; }
        
        public bool IsSuccess { get { return Data != null; } }
    }

    public class ProcessResult<T> where T : IModel
    {
        public static ProcessResult<T> Success(T data)
        {
            return new ProcessResult<T>
            {
                Data = data
            };
        }

        public static ProcessResult<T> Success(T data, long returnId, int action, string successMessage)
        {
            return new ProcessResult<T>
            {
                Data = data,
                ReturnID = returnId,
                Action = action,
                SuccessMessage = successMessage
            };
        }

        public static ProcessResult<T> Fail(Error exception)
        {
            return new ProcessResult<T>
            {
                Exception = exception
            };
        }

        public static ProcessResult<T> Fail(string code, string message)
        {
            return new ProcessResult<T>
            {
                Exception = new Error(code, message)
            };
        }

        public T Data { get; set; }

        [JsonPropertyName("exception")]

        public Error Exception { get; set; }

        public string SuccessMessage { get; set; }

        public long ReturnID { get; set; }
        
        public int Action { get; set; }

        public bool IsSuccess { get { return Data != null; } }
    }

    public class ProcessResult : IModel
    {
        public static ProcessResult Success()
        {
            return new ProcessResult
            {
                Data = new ResultModel(true)
            };
        }

        public static ProcessResult Success(int action, long returnId, string successMessage)
        {
            return new ProcessResult
            {
                Data = new ResultModel(true),
                ReturnID = returnId,
                Action = action,
                SuccessMessage = successMessage
            };
        }

        public static ProcessResult Success(IModel model)
        {
            return new ProcessResult
            {
                Data = model
            };
        }

        public static ProcessResult Success(object model)
        {
            return new ProcessResult
            {
                Data = model
            };
        }

        public static ProcessResult Success(IModel model, long returnId, int action, string successMessage)
        {
            return new ProcessResult
            {
                Data = model,
                ReturnID = returnId,
                Action = action,
                SuccessMessage = successMessage
            };
        }

        public static ProcessResult Fail(Error exception)
        {
            return new ProcessResult
            {
                Exception = exception
            };
        }

        public static ProcessResult FailObject(object exception)
        {
            return new ProcessResult
            {
                Exception = new Error(),
                Data = exception
            };
        }

        public static ProcessResult Fail(string code, string message)
        {
            return new ProcessResult
            {
                Exception = new Error(code, message)
            };
        }

        public object Data { get; set; }


        [JsonPropertyName("exception")]
        public Error Exception { get; set; }

        public string SuccessMessage { get; set; }

        public long ReturnID { get; set; }

        public int Action { get; set; }

        public bool IsSuccess { get { return Exception == null; } }


        public override string ToString()
        {
            if (IsSuccess)
            {
                return "Success";
            }
            else
            {
                return $"ERROR: {Exception}";
            }
        }
    }

    public class ValidationResult
    {
        public ValidationResult()
        {
            Errors = new Dictionary<string, List<string>>();
        }

        public Dictionary<string, List<string>> Errors { get; set; }

        public bool IsValid()
        {
            return Errors == null || Errors.Count == 0;
        }

        public void AddError(string code, string message)
        {
            if (!Errors.ContainsKey(code))
            {
                Errors.Add(code, new List<string>());
            }

            Errors[code].Add(message);
        }
    }
}
