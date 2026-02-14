using System;

namespace HearthWatcher.LogReader
{
    public class LogLine
    {
        public string LineContent { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Namespace { get; set; } = string.Empty;

        public LogLine()
        {
        }

        public LogLine(string ns, string line)
        {
            Namespace = ns;
            LineContent = line;
        }
    }
}
