using log4net.Config;

namespace Logger
{
    public class cLog4net : iLogger
    {
        private log4net.ILog _Logger;

        public void Init()
        {
            BasicConfigurator.Configure();
        }

        public void Log(enLogLevel level, string message)
        {
            switch (level)
            {
                case enLogLevel.Debug:
                    _Logger.Debug(message); 
                    break;
                case enLogLevel.Info:
                    _Logger.Info(message);
                    break;
                case enLogLevel.Warn:
                    _Logger.Warn(message);
                    break;
                case enLogLevel.Error:
                    _Logger.Error(message);
                    break;
                case enLogLevel.Fatal:
                    _Logger.Fatal(message);
                    break;
            }
        }

        public void Log(enLogLevel level, string message, object arg1)
        {
            this.Log(level, string.Format(message, arg1));
        }

        public void Log(enLogLevel level, string message, object arg1, object arg2)
        {
            this.Log(level, string.Format(message, arg1, arg2));
        }

        public void Log(enLogLevel level, string message, object arg1, object arg2, object arg3)
        {
            this.Log(level, string.Format(message, arg1, arg2, arg3));
        }

        public cLog4net(System.Type Type)
        {
            _Logger = log4net.LogManager.GetLogger(Type);
            log4net.Util.SystemInfo.NullText = "";
        }
    }
}