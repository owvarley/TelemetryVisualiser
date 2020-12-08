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
        void Log(enLogLevel Level, string Message);
        void Log(enLogLevel Level, string Message, object Arg1);
        void Log(enLogLevel Level, string Message, object Arg1, object Arg2);
        void Log(enLogLevel Level, string Message, object Arg1, object Arg2, object Arg3);
    }
}
