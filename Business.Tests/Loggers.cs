using System;
using log4net;

namespace Business.Tests
{
    public class CustomLogerAdapter : Logger.ILogger
    {
        public void LogError(Exception ex)
        {
            Logger.Controller.LogError(ex);
        }

        public void LogInfo(string infoMessage)
        {
            Logger.Controller.LogInfo(infoMessage);
        }
    }

    public class Log4NetAdapter : Logger.ILogger
    {
        readonly ILog logger = LogManager.GetLogger("Default");

        public void LogError(Exception ex)
        {
            logger.Error(ex);
        }

        public void LogInfo(string infoMessage)
        {
            logger.Info(infoMessage);
        }

    }
      
}