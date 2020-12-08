using System;

namespace Logger
{
    public enum enLogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
        Fatal = 4   
    }

    public interface iLogger
    {
        void Init();
        void Log(enLogLevel level, string message);
        void Log(enLogLevel level, string message, object arg1);
        void Log(enLogLevel level, string message, object arg1, object arg2);
        void Log(enLogLevel level, string message, object arg1, object arg2, object arg3);
    }
}
