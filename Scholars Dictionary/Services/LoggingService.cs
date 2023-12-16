using NLog;
using System;
using System.IO;
using Scholars_Dictionary.Constants;

namespace Scholars_Dictionary.Services
{
    /// <summary>
    /// The LoggingHelper class provides utility methods for logging messages and exceptions.
    /// </summary>
    public static class LoggingService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Logs an informational message to the log file.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        public static void Info(string message)
        {
            EnsureLogsFolderExists();
            logger.Info(message);
        }

        /// <summary>
        /// Logs a warning message to the log file and, if configuration is set to, sends a feedback to the FeedbackSender.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        /// <param name="exception">The exception associated with the error, if any.</param>
        public static void Warning(string message, Exception? exception = null)
        {
            EnsureLogsFolderExists();

            if (exception == null)
                logger.Warn(message);
            else
                logger.Warn($"{message}\n\tError: {exception.Message}\n\tSource: {exception.Source}\n\tTargetSite: {exception.TargetSite}\n\tStackTrace:\n{exception.StackTrace}", exception);
        }

        /// <summary>
        /// Logs an error message to the log file and sends a feedback to the FeedbackSender.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        /// <param name="exception">The exception associated with the error, if any.</param>
        public static void Error(string message, Exception? exception = null)
        {
            EnsureLogsFolderExists();

            if (exception == null)
                logger.Error(message);
            else
                logger.Error($"{message}\n\tError: {exception.Message}\n\tSource: {exception.Source}\n\tTargetSite: {exception.TargetSite}\n\tStackTrace:\n{exception.StackTrace}", exception);
        }

        /// <summary>
        /// Creates a logs folder in the current application base directory if it does not already exist.
        /// </summary>
        private static void EnsureLogsFolderExists()
        {
            string logFolderName = "logs";
            string logFolderPath = Path.Combine(PathConstants.AppReleasePath, logFolderName);

            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }
        }

        /// <summary>
        /// Gets the path to the log file for today.
        /// </summary>
        /// <param name="fileIsCopy">Indicates whether the returned path is for a copy of the log file.</param>
        /// <returns>The path to the log file for today.</returns>
        public static string GetTodaysLogPath()
        {
            string logFolderName = "logs";
            string logFolderPath = Path.Combine(PathConstants.AppReleasePath, logFolderName);
            string todaysLogPath = Path.Combine(logFolderPath, DateTime.Now.ToString("yyyy-MM-dd.log"));
            return todaysLogPath;
        }

        /// <summary>
        /// The `GetTodaysLogName` method gets the name of the log file for today.
        /// </summary>
        /// <returns>A string with the name of the log file for today as value.</returns>
        public static string GetTodaysLogName()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + ".log";
        }
    }
}
