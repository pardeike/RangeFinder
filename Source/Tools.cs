using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RangeFinder
{
	static class Tools
	{
		public static bool IsModKeyHeld(this ModifierKey key)
		{
			switch (key)
			{
				case ModifierKey.Alt:
					return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
				case ModifierKey.Ctrl:
					return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				case ModifierKey.Shift:
					return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
				case ModifierKey.Meta:
					return Input.GetKey(KeyCode.LeftWindows) || Input.GetKey(KeyCode.RightWindows)
						|| Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)
						|| Input.GetKey(KeyCode.LeftApple) || Input.GetKey(KeyCode.RightApple);
			}
			return false;
		}

		public static bool IsModKeyDown(this ModifierKey key)
		{
			switch (key)
			{
				case ModifierKey.Alt:
					return Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);
				case ModifierKey.Ctrl:
					return Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
				case ModifierKey.Shift:
					return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
				case ModifierKey.Meta:
					return Input.GetKeyDown(KeyCode.LeftWindows) || Input.GetKeyDown(KeyCode.RightWindows)
						|| Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.RightCommand)
						|| Input.GetKeyDown(KeyCode.LeftApple) || Input.GetKeyDown(KeyCode.RightApple);
			}
			return false;
		}

		public static bool IsModKeyUp(this ModifierKey key)
		{
			switch (key)
			{
				case ModifierKey.Alt:
					return Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt);
				case ModifierKey.Ctrl:
					return Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl);
				case ModifierKey.Shift:
					return Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift);
				case ModifierKey.Meta:
					return Input.GetKeyUp(KeyCode.LeftWindows) || Input.GetKeyUp(KeyCode.RightWindows)
						|| Input.GetKeyUp(KeyCode.LeftCommand) || Input.GetKeyUp(KeyCode.RightCommand)
						|| Input.GetKeyUp(KeyCode.LeftApple) || Input.GetKeyUp(KeyCode.RightApple);
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
			_ = listing.Label((name + "Explained").Translate());
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

		public static void IntegerLabeled(this Listing_Standard listing, string name, ref int value, string tooltip = null)
		{
			var startHeight = listing.CurHeight;

			var rect = listing.GetRect(Text.LineHeight + listing.verticalSpacing);

			Text.Font = GameFont.Small;
			GUI.color = Color.white;

			var savedAnchor = Text.Anchor;

			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect, (name + "Title").Translate());

			rect.xMin += rect.width * 2 / 3;
			var newValue = Widgets.TextField(rect, value.ToString());
			if (int.TryParse(newValue, out var newInteger))
				value = newInteger;

			Text.Anchor = savedAnchor;

			var key = name + "Explained";
			if (key.CanTranslate())
			{
				Text.Font = GameFont.Tiny;
				listing.ColumnWidth -= 34;
				GUI.color = Color.gray;
				_ = listing.Label(key.Translate());
				listing.ColumnWidth += 34;
			}

			rect = listing.GetRect(0);
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
				_ = listing.Label(key.Translate());
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
