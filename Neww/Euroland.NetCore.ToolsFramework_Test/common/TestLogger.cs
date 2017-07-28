using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Test
{
    public class DummmyDisposable : IDisposable
    {
        public void Dispose()
        {
            
        }
    }
    public class TestLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return new DummmyDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            
        }
    }
}
