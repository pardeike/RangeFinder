using RimWorld;
using Verse;
using Verse.AI;

namespace RangeFinder
{
	public class ObservedPawn(Pawn forPawn, bool locked)
	{
		public Pawn pawn = forPawn;
		public bool locked = locked;
	}

	public class ObservedTargetSearcher(IAttackTargetSearcher forTargetSearcher, bool locked)
	{
		public IAttackTargetSearcher targetSearcher = forTargetSearcher;
		public bool locked = locked;
	}
}