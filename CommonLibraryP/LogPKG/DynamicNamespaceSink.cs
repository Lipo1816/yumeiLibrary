using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.LogPKG
{
    internal class DynamicNamespaceSink : ILogEventSink
    {
        private readonly ConcurrentDictionary<string, ILogEventSink> _sinkCache = new();

        void ILogEventSink.Emit(LogEvent logEvent)
        {
            // 從日誌事件中獲取 SourceContext
            if (!logEvent.Properties.TryGetValue("SourceContext", out var sourceContext))
            {
                sourceContext = new ScalarValue("General");
            }

            // 動態生成文件路徑（包含日期）
            var namespaceName = sourceContext.ToString().Replace('.', '_');
            var currentDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var logFilePath = $"logs/{namespaceName}_{currentDate}.log";

            // 緩存並創建 FileSink
            var sink = _sinkCache.GetOrAdd(logFilePath, path =>
                new Serilog.Sinks.File.FileSink(path, new Serilog.Formatting.Display.MessageTemplateTextFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"), null));

            // 寫入日誌事件
            sink.Emit(logEvent);

        }
    }
}
