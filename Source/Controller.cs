using HarmonyLib;
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

		public static Color GetColor(int n) => colors[n % colors.Length];
		public static Color[] colors = new[]
		{
			Color.red,
			Color.green,
			Color.blue,
			Color.yellow,
			Color.cyan,
			Color.magenta
		};

		public static SimpleColor GetSimpleColor(int n) => simpleColors[n % simpleColors.Length];
		public static SimpleColor[] simpleColors = new[]
		{
			SimpleColor.Red,
			SimpleColor.Green,
			SimpleColor.Blue,
			SimpleColor.Yellow,
			SimpleColor.Cyan,
			SimpleColor.Magenta
		};

		public static Controller controller;
		public static Controller Instance()
		{
			if (controller == null) controller = new Controller();
			return controller;
		}

		public static void Reset()
		{
			observedPawns = new HashSet<ObservedPawn>();
			observedTargetSearchers = new HashSet<ObservedTargetSearcher>();
		}

		public static void HandleDrawing()
		{
			var currentMap = Find.CurrentMap;
			var colorIndex = 0;

			var pawns = observedPawns
				.Select(observed => observed.pawn)
				.Where(pawn => pawn.Map == currentMap && pawn.Spawned && pawn.Dead == false && pawn.Downed == false)
				.ToHashSet();

			pawns.Do(pawn =>
				{
					var verb = pawn.equipment?.PrimaryEq?.PrimaryVerb;
					if (verb != null && verb.verbProps.IsMeleeAttack == false)
					{
						var range = verb.verbProps.range;
						if (range > 0 && range < RangeFinder.Settings.maxRange)
						{
							var color = RangeFinder.Settings.useColorCoding == BooleanKey.Yes ? GetColor(colorIndex) : Color.white;
							var lineColor = GetSimpleColor(colorIndex++);

							var pos = RangeFinder.Settings.showRangeAtMouseKey.IsModKeyHeld() ? UI.MouseCell() : pawn.Position;
							GenDraw.DrawRadiusRing(pos, range, color);

							if (RangeFinder.Settings.useColorCoding == BooleanKey.Yes)
							{
								GenDraw.DrawCircleOutline(pawn.DrawPos, 0.75f, lineColor);
								GenDraw.DrawCircleOutline(pawn.DrawPos, 0.75f, lineColor);
							}
						}
					}
				});

			observedTargetSearchers
				.Select(observed => observed.targetSearcher)
				.Where(targetSearcher => targetSearcher.Thing != null && targetSearcher.Thing.Map == currentMap && targetSearcher.Thing.Spawned && pawns.Contains(targetSearcher.Thing) == false)
				.Do(targetSearcher =>
				{
					var verb = targetSearcher.CurrentEffectiveVerb;
					if (verb != null)
					{
						var range = verb.verbProps.range;
						if (range > 0 && range < RangeFinder.Settings.maxRange)
						{
							var color = RangeFinder.Settings.useColorCoding == BooleanKey.Yes ? GetColor(colorIndex) : Color.white;
							var lineColor = GetSimpleColor(colorIndex++);

							var pos = RangeFinder.Settings.showRangeAtMouseKey.IsModKeyHeld() ? UI.MouseCell() : targetSearcher.Thing.Position;
							GenDraw.DrawRadiusRing(pos, range, color);

							if (RangeFinder.Settings.useColorCoding == BooleanKey.Yes)
							{
								var rect = targetSearcher.Thing.OccupiedRect();
								var size = Mathf.Max(rect.Width, rect.Height) / 2f + 0.25f;
								GenDraw.DrawCircleOutline(targetSearcher.Thing.DrawPos, size, lineColor);
								GenDraw.DrawCircleOutline(targetSearcher.Thing.DrawPos, size, lineColor);
							}
						}
					}
				});
		}

		private bool isPressed;
		private float lastPressedTime;
		public void HandleEvents()
		{
			if (RangeFinder.Settings.showWeaponRangeKey.IsModKeyDown())
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
						_ = observedPawns.Add(new ObservedPawn(pawn, locked));
					else
						observedPawns.DoIf(c => c.pawn == pawn, c => c.locked = locked);
				}

				foreach (var targetSearcher in Tools.GetSelectedTargetSearchers())
				{
					var observed = observedTargetSearchers.FirstOrDefault(c => c.targetSearcher == targetSearcher);
					if (observed == null)
						_ = observedTargetSearchers.Add(new ObservedTargetSearcher(targetSearcher, locked));
					else
						observedTargetSearchers.DoIf(c => c.targetSearcher == targetSearcher, c => c.locked = locked);
				}
			}

			if (RangeFinder.Settings.showWeaponRangeKey.IsModKeyUp())
			{
				if (isPressed == false)
					return;
				isPressed = false;

				foreach (var pawn in Tools.GetSelectedPawns())
				{
					var observed = observedPawns.FirstOrDefault(c => c.pawn == pawn);
					if (observed != null && observed.locked == false)
						_ = observedPawns.RemoveWhere(c => c.pawn == pawn);
				}

				foreach (var targetSearcher in Tools.GetSelectedTargetSearchers())
				{
					var observed = observedTargetSearchers.FirstOrDefault(c => c.targetSearcher == targetSearcher);
					if (observed != null && observed.locked == false)
						_ = observedTargetSearchers.RemoveWhere(c => c.targetSearcher == targetSearcher);
				}
			}
		}
	}
}
