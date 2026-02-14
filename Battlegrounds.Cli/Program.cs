using System;
using System.IO;
using System.Linq;
using System.Threading;
using HearthWatcher.LogReader;

class Program
{
    static void Main(string[] args)
    {
        // RUTA BASE (Ajustala a tu home o donde esté el juego)
        string baseLogPath = "/var/home/borgo/hearthstone-linux/hearthstone/Logs";

        // Buscamos la carpeta de fecha más reciente
        string? latestLogDir = Directory.GetDirectories(baseLogPath)
            .OrderByDescending(d => d) // Asumimos formato YYYY-MM-DD para que ordene bien
            .FirstOrDefault();

        if (latestLogDir == null || !Directory.Exists(latestLogDir))
        {
            Console.WriteLine($"[ERROR] No se encontró carpeta de logs en: {baseLogPath}");
            return;
        }

        var info = new LogWatcherInfo { Name = "Power" };
        var watcher = new LogFileWatcher(info, latestLogDir);
        var bgHandler = new BattlegroundsHandler();

        Console.WriteLine($"--- Backend Activo ---");
        Console.WriteLine($"[INFO] Monitoreando logs en: {latestLogDir}");
        
        watcher.Start(DateTime.Now, latestLogDir);

        while (true)
        {
            if (watcher.Lines.TryDequeue(out var line))
            {
                bgHandler.Handle(line.LineContent);
            }
            Thread.Sleep(10); 
        }
    }
}
