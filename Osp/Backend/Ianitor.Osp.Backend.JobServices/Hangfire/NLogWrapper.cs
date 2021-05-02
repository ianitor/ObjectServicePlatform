using System;
using Hangfire.Logging;
using NLog;
using LogLevel = NLog.LogLevel;

namespace Ianitor.Osp.Backend.JobServices.Hangfire
{
    internal class NLogWrapper : ILog
    {
        private readonly Logger _targetLogger;

        public NLogWrapper(Logger targetLogger)
        {
            _targetLogger = targetLogger ?? throw new ArgumentNullException(nameof(targetLogger));
        }

        public bool Log(global::Hangfire.Logging.LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            var targetLogLevel = ToTargetLogLevel(logLevel);

            // When messageFunc is null, Hangfire.Logging
            // just determines is logging enabled.
            if (messageFunc == null)
            {
                return _targetLogger.IsEnabled(targetLogLevel);
            }

            _targetLogger.Log(targetLogLevel, exception, () => messageFunc());
            return true;
        }

        private static LogLevel ToTargetLogLevel(global::Hangfire.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case global::Hangfire.Logging.LogLevel.Trace:
                    return LogLevel.Trace;
                case global::Hangfire.Logging.LogLevel.Debug:
                    return LogLevel.Debug;
                case global::Hangfire.Logging.LogLevel.Info:
                    return LogLevel.Info;
                case global::Hangfire.Logging.LogLevel.Warn:
                    return LogLevel.Warn;
                case global::Hangfire.Logging.LogLevel.Error:
                    return LogLevel.Error;
                case global::Hangfire.Logging.LogLevel.Fatal:
                    return LogLevel.Fatal;
            }

            return LogLevel.Off;
        }
    }
}
