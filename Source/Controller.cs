using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RangeFinder
{
	[StaticConstructorOnStartup]
	public class Controller
	{
		public static List<ObservedPawn> observedPawns = [];
		public static List<ObservedTargetSearcher> observedTargetSearchers = [];

		public static Color GetColor(int n)
		{
			var customColors = RangeFinder.Settings.customColors;
			if (customColors.NullOrEmpty())
				return colors[n % colors.Length];
			return customColors[n % customColors.Count];
		}
		public static Material GetMaterial(int n)
		{
			var customColorMaterials = RangeFinder.Settings.customColorMaterials;
			if (customColorMaterials.NullOrEmpty())
				return materials[n % materials.Length];
			var idx = n % customColorMaterials.Count;
			if (customColorMaterials[idx] == null)
				customColorMaterials[idx] = MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, RangeFinder.Settings.customColors[idx]);
			return customColorMaterials[idx];
		}

		public static Material whiteMaterial = MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.white);

		public static Color[] colors =
		[
			Color.red,
			Color.green,
			Color.blue,
			Color.yellow,
			Color.cyan,
			Color.magenta
		];
		public static Material[] materials =
		[
			MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.red),
			MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.green),
			MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.blue),
			MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.yellow),
			MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.cyan),
			MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.magenta)
		];

		public static Controller controller;
		public static Controller Instance()
		{
			controller ??= new Controller();
			return controller;
		}

		public static void Reset()
		{
			observedPawns = [];
			observedTargetSearchers = [];
		}

		public static void HandleDrawing()
		{
			if (Find.UIRoot.screenshotMode.active)
				return;

			var currentMap = Find.CurrentMap;
			var colorIndex = -1;

			var pawns = observedPawns
				.Select(observed => observed.pawn)
				.Where(pawn => pawn.Map == currentMap && pawn.Spawned && pawn.Dead == false && pawn.Downed == false);

			var pawnsWithRanges = new HashSet<Pawn>();

			pawns
				.Where(pawn =>
				{
					var verb = pawn.equipment?.PrimaryEq?.PrimaryVerb;
					if (verb == null || verb.verbProps.IsMeleeAttack)
						return false;
					var range = verb.verbProps.range;
					return range > 0 && range < RangeFinder.Settings.maxRange;
				})
				.Do(pawn =>
				{
					var color = RangeFinder.Settings.useColorCoding == BooleanKey.Yes ? GetColor(++colorIndex) : Color.white;
					GenDraw.DrawRadiusRing(pawn.Position, pawn.equipment.PrimaryEq.PrimaryVerb.verbProps.range, color);
					_ = pawnsWithRanges.Add(pawn);

					if (RangeFinder.Settings.useColorCoding == BooleanKey.Yes)
					{
						var lineColor = GetColor(colorIndex);
						var mat = GetMaterial(colorIndex);
						GenDraw.DrawCircleOutline(pawn.DrawPos, 0.75f, mat);
						GenDraw.DrawCircleOutline(pawn.DrawPos, 0.75f, mat);
					}
				});

			if (RangeFinder.Settings.showRangeAtMouseKey.IsModKeyHeld())
			{
				var selectedPawns = Find.Selector.SelectedPawns
					.Where(pawn =>
					{
						var verb = pawn.equipment?.PrimaryEq?.PrimaryVerb;
						if (verb == null || verb.verbProps.IsMeleeAttack)
							return false;
						var range = verb.verbProps.range;
						return range > 0 && range < RangeFinder.Settings.maxRange;
					})
					.Except(pawnsWithRanges);

				if (selectedPawns.Any())
				{
					var mouseCell = UI.MouseCell();
					GenDraw.DrawCircleOutline(mouseCell.ToVector3Shifted(), 0.25f, whiteMaterial);
					GenDraw.DrawCircleOutline(mouseCell.ToVector3Shifted(), 0.25f, whiteMaterial);

					selectedPawns
						.Do(pawn =>
						{
							var color = RangeFinder.Settings.useColorCoding == BooleanKey.Yes ? GetColor(++colorIndex) : Color.white;
							GenDraw.DrawRadiusRing(mouseCell, pawn.equipment.PrimaryEq.PrimaryVerb.verbProps.range, color);

							if (RangeFinder.Settings.useColorCoding == BooleanKey.Yes)
							{
								var lineColor = GetColor(colorIndex);
								var mat = GetMaterial(colorIndex);
								GenDraw.DrawCircleOutline(pawn.DrawPos, 0.75f, mat);
								GenDraw.DrawCircleOutline(pawn.DrawPos, 0.75f, mat);
							}
						});
				}
			}

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
							var color = RangeFinder.Settings.useColorCoding == BooleanKey.Yes ? GetColor(++colorIndex) : Color.white;
							GenDraw.DrawRadiusRing(targetSearcher.Thing.Position, range, color);

							if (RangeFinder.Settings.useColorCoding == BooleanKey.Yes)
							{
								var rect = targetSearcher.Thing.OccupiedRect();
								var size = Mathf.Max(rect.Width, rect.Height) / 2f + 0.25f;

								var lineColor = GetColor(colorIndex);
								var mat = GetMaterial(colorIndex);
								GenDraw.DrawCircleOutline(targetSearcher.Thing.DrawPos, size, mat);
								GenDraw.DrawCircleOutline(targetSearcher.Thing.DrawPos, size, mat);
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

			if (RangeFinder.Settings.showWeaponRangeKey.IsModKeyUp())
			{
				if (isPressed == false)
					return;
				isPressed = false;

				foreach (var pawn in Tools.GetSelectedPawns())
				{
					var observed = observedPawns.FirstOrDefault(c => c.pawn == pawn);
					if (observed != null && observed.locked == false)
						_ = observedPawns.RemoveAll(c => c.pawn == pawn);
				}

				foreach (var targetSearcher in Tools.GetSelectedTargetSearchers())
				{
					var observed = observedTargetSearchers.FirstOrDefault(c => c.targetSearcher == targetSearcher);
					if (observed != null && observed.locked == false)
						_ = observedTargetSearchers.RemoveAll(c => c.targetSearcher == targetSearcher);
				}
			}
		}
	}
}
