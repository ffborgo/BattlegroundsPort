using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HearthWatcher.LogReader
{
	public class LogFileWatcher
	{
		internal readonly LogWatcherInfo Info;
		private string _logDir;
		private ConcurrentQueue<LogLine> _lines = new ConcurrentQueue<LogLine>();
		public ConcurrentQueue<LogLine> Lines => _lines;
		private long _offset;
		private bool _running;
		private bool _stop;
		private Thread? _thread;

		public LogFileWatcher(LogWatcherInfo info, string logDirectory)
		{
			Info = info;
			_logDir = logDirectory;
		}

		public void Start(DateTime startingPoint, string logDirectory)
		{
			_logDir = logDirectory;
			_stop = false;
			_offset = 0;
			_thread = new Thread(ReadLogFile) { IsBackground = true };
			_thread.Start();
		}

		public void Stop() => _stop = true;

		private void ReadLogFile()
		{
			_running = true;
			while (!_stop)
			{
				var filePath = Path.Combine(_logDir, Info.Name + ".log");
				if (File.Exists(filePath))
				{
					using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						if (fs.Length > _offset)
						{
							fs.Seek(_offset, SeekOrigin.Begin);
							using (var sr = new StreamReader(fs))
							{
								string? line;
								while ((line = sr.ReadLine()) != null)
								{
									_lines.Enqueue(new LogLine(Info.Name, line));
								}
								_offset = fs.Position;
							}
						}
					}
				}
				Thread.Sleep(100);
			}
			_running = false;
		}
	}
}
