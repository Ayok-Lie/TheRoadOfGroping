namespace RoadOfGroping.Host.Extensions
{
    public class Log4Extention
    {
        public static void InitLog4(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddFilter("System", LogLevel.Warning);
            loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//过滤掉系统默认的一些日志
            loggingBuilder.AddLog4Net(new Log4NetProviderOptions()
            {
                Log4NetConfigFileName = "Config/log4net.config",
                Watch = true
            });
        }
    }
}


// 配置日志
//builder.Host.UseSerilog((context, configuration) =>
//{
//    configuration
//        .MinimumLevel.Debug()
//        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
//        .MinimumLevel.Override("System", LogEventLevel.Warning)
//        .Enrich.FromLogContext()
//        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
//        .WriteTo.File(new CompactJsonFormatter(), "logs/log.txt", rollingInterval: RollingInterval.Day);
//});