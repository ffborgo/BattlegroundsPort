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
        private string? _localPlayerEntity;
        private int? _localPlayerEntityId;

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
                if (tag == "LOCAL_PLAYER" && value == "1")
                {
                    _localPlayerEntity = NormalizeEntity(entity);
                    _localPlayerEntityId = ExtractEntityId(entity);
                }

                if (tag == "PLAYER_TECH_LEVEL" && IsLocalPlayerEntity(entity))
                {
                    if (int.TryParse(value, out int newTier) && newTier != CurrentTavernTier && newTier > 0)
                    {
                        CurrentTavernTier = newTier;
                        Console.WriteLine($"[TABERNA] Cambio de estado: Nivel {CurrentTavernTier}");
                    }
                }
                
                // 3. Fase de compra: Solo avisar si entramos de nuevo a la fase
                if (tag == "NEXT_STEP" && IsGameEntity(entity) && value == "MAIN_ACTION" && _lastStep != "MAIN_ACTION")
                {
                     _lastStep = "MAIN_ACTION";
                     Console.WriteLine("[FASE] Fase de Reclutamiento (Tienda abierta)");
                }
                else if (tag == "NEXT_STEP" && IsGameEntity(entity) && value != "MAIN_ACTION")
                {
                    _lastStep = value;
                }
            }
        }

        private bool IsLocalPlayerEntity(string entity)
        {
            var entityId = ExtractEntityId(entity);
            if (_localPlayerEntityId.HasValue && entityId.HasValue)
                return _localPlayerEntityId.Value == entityId.Value;

            return _localPlayerEntity != null && NormalizeEntity(entity).Equals(_localPlayerEntity, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsGameEntity(string entity)
        {
            return NormalizeEntity(entity).StartsWith("GameEntity", StringComparison.OrdinalIgnoreCase);
        }


        private static int? ExtractEntityId(string entity)
        {
            var idMarker = " id=";
            var idx = entity.IndexOf(idMarker, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
                return null;

            var start = idx + idMarker.Length;
            var end = start;
            while (end < entity.Length && char.IsDigit(entity[end]))
                end++;

            if (start == end)
                return null;

            return int.TryParse(entity[start..end], out var entityId) ? entityId : null;
        }

        private static string NormalizeEntity(string entity)
        {
            var normalized = entity.Trim();
            var splitAt = normalized.IndexOf(" ", StringComparison.Ordinal);
            return splitAt > 0 ? normalized[..splitAt] : normalized;
        }
    }
}
