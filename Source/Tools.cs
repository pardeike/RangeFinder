using RimWorld;
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
			return key switch
			{
				ModifierKey.Alt => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt),
				ModifierKey.Ctrl => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl),
				ModifierKey.Shift => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift),
				ModifierKey.Meta => Input.GetKey(KeyCode.LeftWindows) || Input.GetKey(KeyCode.RightWindows)
|| Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)
|| Input.GetKey(KeyCode.LeftApple) || Input.GetKey(KeyCode.RightApple),
				_ => false,
			};
		}

		public static bool IsModKeyDown(this ModifierKey key)
		{
			return key switch
			{
				ModifierKey.Alt => Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt),
				ModifierKey.Ctrl => Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl),
				ModifierKey.Shift => Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift),
				ModifierKey.Meta => Input.GetKeyDown(KeyCode.LeftWindows) || Input.GetKeyDown(KeyCode.RightWindows)
|| Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.RightCommand)
|| Input.GetKeyDown(KeyCode.LeftApple) || Input.GetKeyDown(KeyCode.RightApple),
				_ => false,
			};
		}

		public static bool IsModKeyUp(this ModifierKey key)
		{
			return key switch
			{
				ModifierKey.Alt => Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt),
				ModifierKey.Ctrl => Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl),
				ModifierKey.Shift => Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift),
				ModifierKey.Meta => Input.GetKeyUp(KeyCode.LeftWindows) || Input.GetKeyUp(KeyCode.RightWindows)
|| Input.GetKeyUp(KeyCode.LeftCommand) || Input.GetKeyUp(KeyCode.RightCommand)
|| Input.GetKeyUp(KeyCode.LeftApple) || Input.GetKeyUp(KeyCode.RightApple),
				_ => false,
			};
		}

		public static List<Pawn> GetSelectedPawns()
		{
			return Find.Selector.SelectedObjects.OfType<Pawn>()
				.Where(pawn =>
				{
					if (pawn.Spawned == false || pawn.Downed)
						return false;
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
					if (targetSearcher.Thing == null || targetSearcher.Thing.Spawned == false)
						return false;
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
				if (!tooltip.NullOrEmpty())
					TooltipHandler.TipRegion(rect, tooltip);
			}
		}

		public static void IntegerLabeled(this Listing_Standard listing, string name, ref int value, string tooltip = null)
		{
			var startHeight = listing.CurHeight;

			var rect = listing.GetRect("Hg".GetHeightCached() + listing.verticalSpacing);

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
				if (!tooltip.NullOrEmpty())
					TooltipHandler.TipRegion(rect, tooltip);
			}
		}

		public static void ValueLabeled<T>(this Listing_Standard listing, string name, ref T value, string tooltip = null)
		{
			var startHeight = listing.CurHeight;

			var rect = listing.GetRect("Hg".GetHeightCached() + listing.verticalSpacing);

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
				if (!tooltip.NullOrEmpty())
					TooltipHandler.TipRegion(rect, tooltip);

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
		}

		public static float HorizontalSlider(Rect rect, float value, float leftValue, float rightValue, bool middleAlignment = false, string label = null, string leftAlignedLabel = null, string rightAlignedLabel = null, float roundTo = -1f)
		{
			if (middleAlignment || !label.NullOrEmpty())
				rect.y += Mathf.Round((rect.height - 16f) / 2f);
			if (!label.NullOrEmpty())
				rect.y += 5f;
			float num = GUI.HorizontalSlider(rect, value, leftValue, rightValue);
			if (!label.NullOrEmpty() || !leftAlignedLabel.NullOrEmpty() || !rightAlignedLabel.NullOrEmpty())
			{
				TextAnchor anchor = Text.Anchor;
				GameFont font = Text.Font;
				Text.Font = GameFont.Tiny;
				float num2 = (label.NullOrEmpty() ? 18f : Text.CalcSize(label).y);
				rect.y = rect.y - num2 + 3f;
				if (!leftAlignedLabel.NullOrEmpty())
				{
					Text.Anchor = TextAnchor.UpperLeft;
					Widgets.Label(rect, leftAlignedLabel);
				}
				if (!rightAlignedLabel.NullOrEmpty())
				{
					Text.Anchor = TextAnchor.UpperRight;
					Widgets.Label(rect, rightAlignedLabel);
				}
				if (!label.NullOrEmpty())
				{
					Text.Anchor = TextAnchor.UpperCenter;
					Widgets.Label(rect, label);
				}
				Text.Anchor = anchor;
				Text.Font = font;
			}
			if (roundTo > 0f)
				num = Mathf.RoundToInt(num / roundTo) * roundTo;
			return num;
		}

	}
}
