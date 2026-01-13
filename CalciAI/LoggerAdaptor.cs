using Microsoft.Extensions.Logging;
using System;

namespace CalciAI
{
    public interface ILoggerAdaptor<T>
    {
        void LogTrace(string message);

        void LogTrace<T0>(string message, T0 arg0);

        void LogTrace<T0, T1>(string message, T0 arg0, T1 arg1);

        void LogTrace<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

        void LogTrace<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);


        void LogDebug(string message);

        void LogDebug<T0>(string message, T0 arg0);

        void LogDebug<T0, T1>(string message, T0 arg0, T1 arg1);

        void LogDebug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

        void LogDebug<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);



        void LogInformation(string message);

        void LogInformation<T0>(string message, T0 arg0);

        void LogInformation<T0, T1>(string message, T0 arg0, T1 arg1);

        void LogInformation<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

        void LogInformation<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);

        void LogInformation<T0, T1, T2, T3, T4>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        void LogInformation<T0, T1, T2, T3, T4, T5>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

        void LogInformation<T0, T1, T2, T3, T4, T5, T6>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);



        void LogWarning(string message);

        void LogWarning<T0>(string message, T0 arg0);

        void LogWarning<T0, T1>(string message, T0 arg0, T1 arg1);

        void LogWarning<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

        void LogWarning<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);



        void LogError(string message);

        void LogError<T0>(string message, T0 arg0);

        void LogError<T0, T1>(string message, T0 arg0, T1 arg1);

        void LogError<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

        void LogError<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);


        void LogCritical(string message);

        void LogCritical<T0>(string message, T0 arg0);

        void LogCritical<T0, T1>(string message, T0 arg0, T1 arg1);

        void LogCritical<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

        void LogCritical<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);

    }

    public class LoggerAdaptor<T> : ILoggerAdaptor<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerAdaptor(ILogger<T> logger)
        {
            _logger = logger;
        }

        #region LogTrace
        /// <summary>
        /// For Trace logging
        /// </summary>
        /// <param name="message">message</param>
        public void LogTrace(string message)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message);
            }
        }
        /// <summary>
        /// For Trace logging
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        public void LogTrace<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message, arg0);
            }
        }
        /// <summary>
        /// For Trace logging
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        public void LogTrace<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message, arg0, arg1);
            }
        }
        /// <summary>
        /// For Trace logging
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        public void LogTrace<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message, arg0, arg1, arg2);
            }
        }
        /// <summary>
        /// For Trace logging
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        /// <param name="arg3">arg4</param>
        public void LogTrace<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message, arg0, arg1, arg2, arg3);
            }
        }
        #endregion

        #region LogDebug

        /// <summary>
        /// to add Debug logs
        /// </summary>
        /// <param name="message"> static message</param>
        public void LogDebug(string message)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message);
            }
        }
        /// <summary>
        /// to add Debug logs
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        public void LogDebug<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0);
            }
        }
        /// <summary>
        /// to add Debug logs
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        public void LogDebug<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0, arg1);
            }
        }
        /// <summary>
        /// to add Debug logs
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        public void LogDebug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0, arg1, arg2);
            }
        }
        /// <summary>
        /// to add Debug logs
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="message">messsage</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        /// <param name="arg3">arg4</param>
        public void LogDebug<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0, arg1, arg2, arg3);
            }
        }
        #endregion

        #region LogInformation
        /// <summary>
        /// for Information Logging
        /// </summary>
        /// <param name="message"> log message</param>
        /// <exception cref="NotImplementedException"></exception>
        public void LogInformation(string message)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message);
            }
        }
        /// <summary>
        /// For log information with 1 argument
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">dynamic arg1</param>
        public void LogInformation<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0);
            }
        }
        /// <summary>
        /// For log information with 2 argument
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">dynamic arg1</param>
        /// <param name="arg1">dynamic arg2</param>
        public void LogInformation<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0, arg1);
            }
        }
        /// <summary>
        /// For log information with 3 argument
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">dynamic arg1</param>
        /// <param name="arg1">dynamic arg2</param>
        /// <param name="arg2">dynamic arg3</param>
        public void LogInformation<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0, arg1, arg2);
            }
        }
        /// <summary>
        /// For log information with 4 argument
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="message">messsage</param>
        /// <param name="arg0">dynamic arg1</param>
        /// <param name="arg1">dynamic arg2</param>
        /// <param name="arg2">dynamic arg3</param>
        /// <param name="arg3">dynamic arg4</param>
        public void LogInformation<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0, arg1, arg2, arg3);
            }
        }
        /// <summary>
        /// For log information with 5 argument
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        /// <param name="arg3">arg4</param>
        /// <param name="arg4">arg5</param>
        public void LogInformation<T0, T1, T2, T3, T4>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0, arg1, arg2, arg3, arg4);
            }
        }

        public void LogInformation<T0, T1, T2, T3, T4, T5>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0, arg1, arg2, arg3, arg4, arg5);
            }
        }

        public void LogInformation<T0, T1, T2, T3, T4, T5, T6>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }
        #endregion

        #region LogWarning
        /// <summary>
        /// For Logging warning
        /// </summary>
        /// <param name="message">message</param>
        public void LogWarning(string message)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(message);
            }
        }
        /// <summary>
        /// For Logging warning
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        public void LogWarning<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(message, arg0);
            }
        }
        /// <summary>
        /// For Logging warning
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        public void LogWarning<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(message, arg0, arg1);
            }
        }
        /// <summary>
        /// For Logging warning
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        public void LogWarning<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(message, arg0, arg1, arg2);
            }
        }
        /// <summary>
        /// For Logging warning
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        /// <param name="arg3">arg4</param>
        public void LogWarning<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(message, arg0, arg1, arg2, arg3);
            }
        }
        #endregion

        #region LogError
        /// <summary>
        /// For logging error
        /// </summary>
        /// <param name="message">message</param>
        public void LogError(string message)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message);
            }
        }
        /// <summary>
        /// For logging error
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        public void LogError<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message, arg0);
            }
        }
        /// <summary>
        /// For logging error
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        public void LogError<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message, arg0, arg1);
            }
        }
        /// <summary>
        /// For logging error
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        public void LogError<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message, arg0, arg1, arg2);
            }
        }
        /// <summary>
        /// For logging error
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        /// <param name="arg3">arg4</param>
        public void LogError<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message, arg0, arg1, arg2, arg3);
            }
        }
        #endregion

        #region LogCritical
        /// <summary>
        /// To log critical error
        /// </summary>
        /// <param name="message">message</param>
        public void LogCritical(string message)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message);
            }
        }
        /// <summary>
        /// To log critical error
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        public void LogCritical<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message, arg0);
            }
        }
        /// <summary>
        /// To log critical error
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        public void LogCritical<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message, arg0, arg1);
            }
        }
        /// <summary>
        /// To log critical error
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        public void LogCritical<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message, arg0, arg1, arg2);
            }
        }
        /// <summary>
        /// To log critical error
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="message">message</param>
        /// <param name="arg0">arg1</param>
        /// <param name="arg1">arg2</param>
        /// <param name="arg2">arg3</param>
        /// <param name="arg3">arg4</param>
        public void LogCritical<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message, arg0, arg1, arg2, arg3);
            }
        }


        #endregion
    }
}
