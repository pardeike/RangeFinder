﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RangeFinder
{
	public class Controller
	{
		public static HashSet<ObservedPawn> observedPawns = new HashSet<ObservedPawn>();
		public static HashSet<ObservedTargetSearcher> observedTargetSearchers = new HashSet<ObservedTargetSearcher>();

		public static Controller controller;
		public static Controller Instance()
		{
			if (controller == null) controller = new Controller();
			return controller;
		}

		public void Reset()
		{
			observedPawns = new HashSet<ObservedPawn>();
			observedTargetSearchers = new HashSet<ObservedTargetSearcher>();
		}

		public void HandleDrawing()
		{
			var currentMap = Find.CurrentMap;

			observedPawns
				.Select(observed => observed.pawn)
				.Where(pawn => pawn.Map == currentMap && pawn.Spawned && pawn.Dead == false && pawn.Downed == false)
				.Do(pawn =>
				{
					var verb = pawn.equipment?.PrimaryEq?.PrimaryVerb;
					if (verb != null && verb.verbProps.IsMeleeAttack == false)
					{
						var range = verb.verbProps.range;
						if (range < 90f)
							GenDraw.DrawRadiusRing(pawn.Position, range);
					}
				});

			observedTargetSearchers
				.Select(observed => observed.targetSearcher)
				.Where(targetSearcher => targetSearcher.Thing != null && targetSearcher.Thing.Map == currentMap && targetSearcher.Thing.Spawned)
				.Do(targetSearcher =>
				{
					var verb = targetSearcher.CurrentEffectiveVerb;
					if (verb != null)
					{
						var range = verb.verbProps.range;
						if (range < 90f)
							GenDraw.DrawRadiusRing(targetSearcher.Thing.Position, range);
					}
				});
		}

		private bool isPressed;
		private float lastPressedTime;
		public void HandleEvents()
		{
			if (Tools.IsModKeyDown())
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

				foreach (var targetSearcher in Tools.GetSelectedTargetSearchers())
				{
					var observed = observedTargetSearchers.FirstOrDefault(c => c.targetSearcher == targetSearcher);
					if (observed == null)
						observedTargetSearchers.Add(new ObservedTargetSearcher(targetSearcher, locked));
					else
						observedTargetSearchers.DoIf(c => c.targetSearcher == targetSearcher, c => c.locked = locked);
				}
			}

			if (Tools.IsModKeyUp())
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

				foreach (var targetSearcher in Tools.GetSelectedTargetSearchers())
				{
					var observed = observedTargetSearchers.FirstOrDefault(c => c.targetSearcher == targetSearcher);
					if (observed != null && observed.locked == false)
						observedTargetSearchers.RemoveWhere(c => c.targetSearcher == targetSearcher);
				}
			}
		}
	}
}