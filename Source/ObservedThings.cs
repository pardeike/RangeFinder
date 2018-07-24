using RimWorld;
using Verse;
using Verse.AI;

namespace RangeFinder
{
	public class ObservedPawn
	{
		public Pawn pawn;
		public bool locked;

		public ObservedPawn(Pawn forPawn, bool locked)
		{
			pawn = forPawn;
			this.locked = locked;
		}
	}

	public class ObservedTargetSearcher
	{
		public IAttackTargetSearcher targetSearcher;
		public bool locked;

		public ObservedTargetSearcher(IAttackTargetSearcher forTargetSearcher, bool locked)
		{
			targetSearcher = forTargetSearcher;
			this.locked = locked;
		}
	}
}