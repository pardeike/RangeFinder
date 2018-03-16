using Verse;

namespace RangeFinder
{
	public class ObservedPawn
	{
		public Pawn pawn;
		public bool locked;
		public float lockedTime;

		public ObservedPawn(Pawn forPawn, bool locked)
		{
			pawn = forPawn;
			this.locked = locked;
		}
	}
}