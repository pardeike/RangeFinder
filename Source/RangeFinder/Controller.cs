using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;
using Verse;

namespace RangeFinder
{
	public class Controller
	{
		public static HashSet<ObservedPawn> observedPawns = new HashSet<ObservedPawn>();

		public static Controller controller;
		public static Controller getInstance()
		{
			if (controller == null) controller = new Controller();
			return controller;
		}

		public void Reset()
		{
			observedPawns = new HashSet<ObservedPawn>();
		}

		public void HandleDrawing()
		{
			var currentMap = Find.VisibleMap;
			observedPawns
				.Select(observed => observed.pawn)
				.Where(pawn => pawn.Map == currentMap && pawn.Spawned && pawn.Dead == false && pawn.Downed == false)
				.Do(pawn =>
				{
					var verb = pawn.equipment?.PrimaryEq?.PrimaryVerb;
					if (verb != null && verb.verbProps.MeleeRange == false)
					{
						var range = verb.verbProps.range;
						if (range < 90f)
							GenDraw.DrawRadiusRing(pawn.Position, range);
					}
				});
		}

		private bool isPressed;
		private float lastPressedTime;
		public void HandleEvents()
		{
			if (Event.current.type == EventType.KeyDown && Tools.IsModKey(Event.current.keyCode))
			{
				if (isPressed)
					return;

				isPressed = true;
				var now = Time.realtimeSinceStartup;
				var locked = now - lastPressedTime <= 0.25f;
				lastPressedTime = now;

				foreach (var pawn in Tools.GetSelectedPawns())
				{
					var observed = observedPawns.FirstOrDefault(c => c.pawn == pawn);
					if (observed == null)
						observedPawns.Add(new ObservedPawn(pawn, locked));
					else
						observedPawns.DoIf(c => c.pawn == pawn, c => c.locked = locked);
				}
			}

			if (Event.current.type == EventType.KeyUp && Tools.IsModKey(Event.current.keyCode))
			{
				if (isPressed == false)
					return;

				isPressed = false;
				foreach (var pawn in Tools.GetSelectedPawns())
				{
					var observed = observedPawns.FirstOrDefault(c => c.pawn == pawn);
					if (observed != null && observed.locked == false)
						observedPawns.RemoveWhere(c => c.pawn == pawn);
				}
			}
		}
	}
}