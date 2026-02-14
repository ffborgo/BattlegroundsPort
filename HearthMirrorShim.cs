using System.Collections.Generic;

namespace HearthMirror.Objects
{
    public enum SelectedBattlegroundsGameMode
    {
        UNKNOWN = 0,
        SOLO = 1,
        DUOS = 2
    }

    public sealed class BattlegroundsTeammateBoardState
    {
        public bool ViewingTeammate { get; set; }
        public List<string> MulliganHeroes { get; set; } = new();
        public List<BattlegroundsTeammateBoardStateEntity> Entities { get; set; } = new();
    }

    public sealed class BattlegroundsTeammateBoardStateEntity
    {
        public int Id { get; set; }
        public string? CardId { get; set; }
    }
}

namespace HearthMirror.Enums
{
    public enum FindGameState
    {
        UNKNOWN = 0,
        CLIENT_UNKNOWN = 1,
        CLIENT_ERROR = 2,
        CLIENT_NOT_READY = 3,
        CLIENT_BUSY = 4,
        CLIENT_QUEUE_CANCELED = 5,
        CLIENT_QUEUE_TIMEOUT = 6,
        SERVER_UNKNOWN = 7,
        SERVER_ERROR = 8,
        SERVER_QUEUE_CANCELED = 9,
        SERVER_NO_GAME_SERVER = 10,
        SERVER_GAME_STARTED = 11,
        SERVER_GAME_CANCELED = 12,
        SERVER_CONNECT_TIMEOUT = 13,
        SERVER_CONNECT_ERROR = 14,
        SERVER_DISCONNECT = 15,
        SERVER_CHALLENGE_CANCELED = 16
    }
}
