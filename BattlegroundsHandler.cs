using System;
using System.Text.RegularExpressions;

namespace HearthWatcher.LogReader
{
    public class BattlegroundsHandler
    {
        private static readonly Regex TagChangeRegex = new Regex(@"TAG_CHANGE Entity=(?<ent>.+) tag=(?<tag>\w+) value=(?<val>\w+)");

        public int CurrentTavernTier { get; private set; } = -1;
        public bool IsBattlegrounds { get; private set; } = false;
        private string _lastStep = "";

        public void Handle(string logLine)
        {
            var match = TagChangeRegex.Match(logLine);
            if (match.Success)
            {
                string tag = match.Groups["tag"].Value;
                string value = match.Groups["val"].Value;
                string entity = match.Groups["ent"].Value;

                // 1. Solo detectar Battlegrounds la primera vez
                if (tag == "BACON_USE_COIN_BASED_BUDDY_METER" && value == "1" && !IsBattlegrounds)
                {
                    IsBattlegrounds = true;
                    Console.WriteLine("\n[SISTEMA] ¡Partida de Battlegrounds Detectada!");
                }

                // 2. Filtrar el nivel de taberna: Solo si CAMBIÓ y es un nivel válido (1-7)
                if (tag == "PLAYER_TECH_LEVEL")
                {
                    if (int.TryParse(value, out int newTier) && newTier != CurrentTavernTier && newTier > 0)
                    {
                        CurrentTavernTier = newTier;
                        Console.WriteLine($"[TABERNA] Cambio de estado: Nivel {CurrentTavernTier}");
                    }
                }
                
                // 3. Fase de compra: Solo avisar si entramos de nuevo a la fase
                if (tag == "NEXT_STEP" && value == "MAIN_ACTION" && _lastStep != "MAIN_ACTION")
                {
                     _lastStep = "MAIN_ACTION";
                     Console.WriteLine("[FASE] Fase de Reclutamiento (Tienda abierta)");
                }
                else if (tag == "NEXT_STEP" && value != "MAIN_ACTION")
                {
                    _lastStep = value;
                }
            }
        }
    }
}
