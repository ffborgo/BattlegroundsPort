using System.Collections.Generic;
using System.Linq;
using HearthMirror.Objects;

namespace HearthWatcher.EventArgs
{
	public class BattlegroundsTeammateBoardStateArgs : System.EventArgs
	{
		public bool IsViewingTeammate { get; }

		public List<string> MulliganHeroes { get; }

		public List<BattlegroundsTeammateBoardStateEntity> Entities { get; }

		public BattlegroundsTeammateBoardStateArgs(
			bool isViewingTeammate, List<string> mulliganHeroes, List<BattlegroundsTeammateBoardStateEntity> entities
		)
		{
			IsViewingTeammate = isViewingTeammate;
			MulliganHeroes = mulliganHeroes;
			Entities = entities;
		}

		public override bool Equals(object? obj)
		{
			return obj is BattlegroundsTeammateBoardStateArgs args
			       && IsViewingTeammate == args.IsViewingTeammate
			       && MulliganHeroes.SequenceEqual(args.MulliganHeroes)
			       && Entities.SequenceEqual(args.Entities);
		}

		public override int GetHashCode()
		{
			var hashCode = IsViewingTeammate.GetHashCode();
			foreach(var hero in MulliganHeroes)
				hashCode = (hashCode * 397) ^ (hero?.GetHashCode() ?? 0);
			foreach(var entity in Entities)
				hashCode = (hashCode * 397) ^ (entity?.GetHashCode() ?? 0);
			return hashCode;
		}
	}
}
