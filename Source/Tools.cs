using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using System;
using RimWorld;
using Verse.AI;

namespace RangeFinder
{
	static class Tools
	{
		public static bool IsModKey(KeyCode keyCode)
		{
			switch (RangeFinder.Settings.showWeaponRangeKey)
			{
				case RangeFinderModKey.Alt:
					return keyCode == KeyCode.LeftAlt || keyCode == KeyCode.RightAlt;
				case RangeFinderModKey.Ctrl:
					return keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl;
				case RangeFinderModKey.Shift:
					return keyCode == KeyCode.LeftShift || keyCode == KeyCode.RightShift;
				case RangeFinderModKey.Meta:
					return keyCode == KeyCode.LeftWindows || keyCode == KeyCode.RightWindows
						|| keyCode == KeyCode.LeftCommand || keyCode == KeyCode.RightCommand
						|| keyCode == KeyCode.LeftApple || keyCode == KeyCode.RightApple;
			}
			return false;
		}

		public static List<Pawn> GetSelectedPawns()
		{
			return Find.Selector.SelectedObjects.OfType<Pawn>()
				.Where(pawn =>
				{
					if (pawn.Spawned == false || pawn.Downed) return false;
					var verb = pawn.equipment?.PrimaryEq?.PrimaryVerb;
					return (verb != null && verb.verbProps.IsMeleeAttack == false);
				})
				.ToList();
		}

		public static List<IAttackTargetSearcher> GetSelectedTargetSearchers()
		{
			return Find.Selector.SelectedObjects.Where(obj => (obj as IAttackTargetSearcher) != null).Cast<IAttackTargetSearcher>()
				.Where(targetSearcher =>
				{
					if (targetSearcher.Thing == null || targetSearcher.Thing.Spawned == false) return false;
					var verb = targetSearcher.CurrentEffectiveVerb;
					return (verb != null);
				})
				.ToList();
		}

		public static void CheckboxEnhanced(this Listing_Standard listing, string name, ref bool value, string tooltip = null)
		{
			var startHeight = listing.CurHeight;

			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			listing.CheckboxLabeled((name + "Title").Translate(), ref value);

			Text.Font = GameFont.Tiny;
			listing.ColumnWidth -= 34;
			GUI.color = Color.gray;
			listing.Label((name + "Explained").Translate());
			listing.ColumnWidth += 34;

			var rect = listing.GetRect(0);
			rect.height = listing.CurHeight - startHeight;
			rect.y -= rect.height;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(rect, tooltip);
			}

			listing.Gap();
		}

		public static void ValueLabeled<T>(this Listing_Standard listing, string name, ref T value, string tooltip = null)
		{
			var startHeight = listing.CurHeight;

			var rect = listing.GetRect(Text.LineHeight + listing.verticalSpacing);

			Text.Font = GameFont.Small;
			GUI.color = Color.white;

			var savedAnchor = Text.Anchor;

			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect, (name + "Title").Translate());

			Text.Anchor = TextAnchor.MiddleRight;
			if (typeof(T).IsEnum)
				Widgets.Label(rect, (typeof(T).Name + "Option" + value.ToString()).Translate());
			else
				Widgets.Label(rect, value.ToString());

			Text.Anchor = savedAnchor;

			var key = name + "Explained";
			if (key.CanTranslate())
			{
				Text.Font = GameFont.Tiny;
				listing.ColumnWidth -= 34;
				GUI.color = Color.gray;
				listing.Label(key.Translate());
				listing.ColumnWidth += 34;
			}

			rect = listing.GetRect(0);
			rect.height = listing.CurHeight - startHeight;
			rect.y -= rect.height;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(rect, tooltip);

				if (Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
				{
					var keys = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
					for (var i = 0; i < keys.Length; i++)
					{
						var newValue = keys[(i + 1) % keys.Length];
						if (keys[i].ToString() == value.ToString())
						{
							value = newValue;
							break;
						}
					}
					Event.current.Use();
				}
			}

			listing.Gap();
		}

	}
}