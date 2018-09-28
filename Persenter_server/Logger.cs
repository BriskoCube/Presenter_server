using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persenter_server
{
    static class Logger
    {
        private static NLog.Logger _logger;

        /// <summary>
        /// Init NLog
        /// </summary>
        public static void Init()
        {
            // Config only if exists
            if (_logger != null)
                return;

            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            _logger = NLog.LogManager.GetCurrentClassLogger();

            Logger.Log().Info("Program Started");
        }

        public static NLog.Logger Log()
        {
            return _logger;
        }

    }
}
