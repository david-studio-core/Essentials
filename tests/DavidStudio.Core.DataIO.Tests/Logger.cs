using Microsoft.Extensions.Logging;

namespace DavidStudio.Core.DataIO.Tests;

public static class Logger
{
    public static readonly ILoggerFactory TestsLoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
}