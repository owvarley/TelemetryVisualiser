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

        public void Log(enLogLevel Level, string Message)
        {
            switch (Level)
            {
                case enLogLevel.Debug:
                    _Logger.Debug(Message); 
                    break;
                case enLogLevel.Info:
                    _Logger.Info(Message);
                    break;
                case enLogLevel.Warn:
                    _Logger.Warn(Message);
                    break;
                case enLogLevel.Error:
                    _Logger.Error(Message);
                    break;
                case enLogLevel.Fatal:
                    _Logger.Fatal(Message);
                    break;
            }
        }

        public void Log(enLogLevel Level, string Message, object Arg1)
        {
            this.Log(Level, string.Format(Message, Arg1));
        }

        public void Log(enLogLevel Level, string Message, object Arg1, object Arg2)
        {
            this.Log(Level, string.Format(Message, Arg1, Arg2));
        }

        public void Log(enLogLevel Level, string Message, object Arg1, object Arg2, object Arg3)
        {
            this.Log(Level, string.Format(Message, Arg1, Arg2, Arg3));
        }

        public cLog4net(System.Type Type)
        {
            _Logger = log4net.LogManager.GetLogger(Type);
            log4net.Util.SystemInfo.NullText = "";
        }
    }
}