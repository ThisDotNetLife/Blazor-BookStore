﻿using Blazor_BookStore_API.Contracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_BookStore_API.Services {
    public class LoggerService : ILoggerService {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public void LogDebug(string message) {
            logger.Debug(message);
        }

        public void LogError(string message) {
            logger.Debug(message);
        }

        public void LogInfo(string message) {
            logger.Debug(message);
        }

        public void LogWarn(string message) {
            logger.Debug(message);
        }
    }
}
