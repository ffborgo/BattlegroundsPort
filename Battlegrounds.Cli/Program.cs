using System;
using System.IO;
using System.Linq;
using System.Threading;
using HearthWatcher.LogReader;

class Program
{
    static void Main(string[] args)
    {
        var baseLogPath = args.FirstOrDefault() ?? "/var/home/borgo/hearthstone-linux/hearthstone/Logs";

        if (!Directory.Exists(baseLogPath))
        {
            Console.WriteLine($"[ERROR] No existe el directorio base de logs: {baseLogPath}");
            return;
        }

        string? latestLogDir = Directory.GetDirectories(baseLogPath)
            .OrderByDescending(d => d)
            .FirstOrDefault();

        if (latestLogDir == null || !Directory.Exists(latestLogDir))
        {
            Console.WriteLine($"[ERROR] No se encontró carpeta de logs en: {baseLogPath}");
            return;
        }

        var info = new LogWatcherInfo { Name = "Power" };
        var watcher = new LogFileWatcher(info, latestLogDir);
        var bgHandler = new BattlegroundsHandler();

        bgHandler.BattlegroundsDetected += () => Console.WriteLine("\n[SISTEMA] ¡Partida de Battlegrounds Detectada!");
        bgHandler.TavernTierChanged += tier => Console.WriteLine($"[TABERNA] Cambio de estado: Nivel {tier}");
        bgHandler.RecruitmentPhaseStarted += () => Console.WriteLine("[FASE] Fase de Reclutamiento (Tienda abierta)");

        Console.WriteLine("--- Backend Activo ---");
        Console.WriteLine($"[INFO] Monitoreando logs en: {latestLogDir}");
        Console.WriteLine("[INFO] Presioná Ctrl+C para salir");

        watcher.Start(DateTime.Now, latestLogDir);

        var keepRunning = true;
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            keepRunning = false;
            watcher.Stop();
        };

        while (keepRunning)
        {
            if (watcher.Lines.TryDequeue(out var line))
                bgHandler.Handle(line.LineContent);

            Thread.Sleep(10);
        }

        Console.WriteLine("[INFO] Backend detenido.");
    }
}
