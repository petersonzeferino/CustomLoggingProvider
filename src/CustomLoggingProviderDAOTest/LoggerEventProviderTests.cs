using Xunit;
using Moq;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using System;
using CustomLoggingProviderLibrary;

namespace CustomLoggingProviderDAOTest
{
    /// <summary>
    /// Unit tests for the LoggerEventProvider class.
    /// These tests verify that each public logging method
    /// correctly calls ILogger with the expected LogLevel and message.
    /// </summary>
    public class LoggerEventProviderTests
    {
        /// <summary>
        /// Tests that all public logging methods call ILogger.Log
        /// with the correct log level and message.
        /// </summary>
        /// <param name="level">The expected log level for the method.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="methodName">The name of the LoggerEventProvider method being tested.</param>
        [Theory]
        [InlineData(LogLevel.Information, "info message", nameof(LoggerEventProvider.LogInfo))]
        [InlineData(LogLevel.Error, "error message", nameof(LoggerEventProvider.LogError))]
        [InlineData(LogLevel.Warning, "warning message", nameof(LoggerEventProvider.LogWarning))]
        [InlineData(LogLevel.Debug, "debug message", nameof(LoggerEventProvider.LogDebug))]
        [InlineData(LogLevel.Critical, "critical message", nameof(LoggerEventProvider.LogCritical))]
        public void LogMethods_ShouldCallILoggerWithCorrectLevel(LogLevel level, string message, string methodName)
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var provider = CreateInstanceWithLogger(mockLogger.Object, level);

            // Act
            var methodInfo = typeof(LoggerEventProvider).GetMethod(methodName);
            methodInfo.Invoke(provider, new object[] { message });

            // Assert
            mockLogger.Verify(
                l => l.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Creates a LoggerEventProvider instance without invoking its constructor,
        /// injecting a mocked ILogger and setting necessary private fields via reflection.
        /// This approach avoids triggering real dependencies like EventLogHelper or LoggerProviderFactory.
        /// </summary>
        /// <param name="logger">The mocked ILogger to inject.</param>
        /// <param name="level">The log level to set for the instance.</param>
        /// <returns>A LoggerEventProvider instance with injected dependencies.</returns>
        private LoggerEventProvider CreateInstanceWithLogger(ILogger logger, LogLevel level)
        {
            // Create instance without running the constructor
            var instance = (LoggerEventProvider)FormatterServices
                .GetUninitializedObject(typeof(LoggerEventProvider));

            // Inject required private fields
            typeof(LoggerEventProvider)
                .GetField("_logger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(instance, logger);

            typeof(LoggerEventProvider)
                .GetField("_logLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(instance, level);

            typeof(LoggerEventProvider)
                .GetField("_callerName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(instance, "TestClass");

            typeof(LoggerEventProvider)
                .GetField("_applicationName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(instance, "TestApp");

            typeof(LoggerEventProvider)
                .GetField("_enableWriteLogToFile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(instance, false);

            return instance;
        }
    }
}