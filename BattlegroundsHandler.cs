using System;
using System.Text.RegularExpressions;

namespace HearthWatcher.LogReader
{
    public class BattlegroundsHandler
    {
        private static readonly Regex TagChangeRegex = new Regex(@"TAG_CHANGE Entity=(?<ent>.+) tag=(?<tag>\w+) value=(?<val>\w+)");

        public int CurrentTavernTier { get; private set; } = -1;
        public bool IsBattlegrounds { get; private set; }
        public bool IsRecruitmentPhase { get; private set; }

        private string _lastStep = string.Empty;
        private string? _localPlayerEntity;
        private int? _localPlayerEntityId;

        public event Action? BattlegroundsDetected;
        public event Action<int>? TavernTierChanged;
        public event Action? RecruitmentPhaseStarted;
        public event Action<string>? StepChanged;

        public void Handle(string logLine)
        {
            var match = TagChangeRegex.Match(logLine);
            if (!match.Success)
                return;

            string tag = match.Groups["tag"].Value;
            string value = match.Groups["val"].Value;
            string entity = match.Groups["ent"].Value;

            if (tag == "BACON_USE_COIN_BASED_BUDDY_METER" && value == "1" && !IsBattlegrounds)
            {
                IsBattlegrounds = true;
                BattlegroundsDetected?.Invoke();
            }

            if (tag == "LOCAL_PLAYER" && value == "1")
            {
                _localPlayerEntity = NormalizeEntity(entity);
                _localPlayerEntityId = ExtractEntityId(entity);
            }

            if (tag == "PLAYER_TECH_LEVEL" && IsLocalPlayerEntity(entity))
            {
                if (int.TryParse(value, out var newTier) && newTier > 0 && newTier != CurrentTavernTier)
                {
                    CurrentTavernTier = newTier;
                    TavernTierChanged?.Invoke(CurrentTavernTier);
                }
            }

            if (tag == "NEXT_STEP" && IsGameEntity(entity))
            {
                if (_lastStep != value)
                {
                    _lastStep = value;
                    StepChanged?.Invoke(value);
                }

                if (value == "MAIN_ACTION")
                {
                    if (!IsRecruitmentPhase)
                    {
                        IsRecruitmentPhase = true;
                        RecruitmentPhaseStarted?.Invoke();
                    }
                }
                else
                {
                    IsRecruitmentPhase = false;
                }
            }
        }

        private bool IsLocalPlayerEntity(string entity)
        {
            var entityId = ExtractEntityId(entity);
            if (_localPlayerEntityId.HasValue && entityId.HasValue)
                return _localPlayerEntityId.Value == entityId.Value;

            return _localPlayerEntity != null
                && NormalizeEntity(entity).Equals(_localPlayerEntity, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsGameEntity(string entity)
        {
            return NormalizeEntity(entity).StartsWith("GameEntity", StringComparison.OrdinalIgnoreCase);
        }

        private static int? ExtractEntityId(string entity)
        {
            const string idMarker = " id=";
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
