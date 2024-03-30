using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RangeFinder
{
	public enum ModifierKey
	{
		None,
		Alt,
		Ctrl,
		Shift,
		Meta
	}

	public enum BooleanKey
	{
		No,
		Yes
	}

	public class RangeFinderSettings : ModSettings
	{
		public ModifierKey showRangeAtMouseKey = ModifierKey.Shift;
		public ModifierKey showWeaponRangeKey = ModifierKey.Ctrl;
		public BooleanKey useColorCoding = BooleanKey.Yes;
		public int maxRange = 96;
		public List<Color> customColors = [];
		public List<Material> customColorMaterials = [];

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref showRangeAtMouseKey, "showWeaponRangeKey", ModifierKey.Ctrl, true);
			Scribe_Values.Look(ref showWeaponRangeKey, "showRangeAtMouseKey", ModifierKey.Shift, true);
			Scribe_Values.Look(ref useColorCoding, "useColorCoding", BooleanKey.Yes, true);
			Scribe_Values.Look(ref maxRange, "maxRange", 96, true);
			Scribe_Collections.Look(ref customColors, "customColors");
			customColors ??= [];
			customColorMaterials = customColors.Select(color => (Material)null).ToList();
		}

		public static Vector2 scrollPosition = Vector2.zero;
		public static void DoWindowContents(Rect canvas)
		{
			var list = new Listing_Standard { ColumnWidth = (canvas.width - Listing.ColumnSpacing) / 2 };
			list.Begin(canvas);
			list.Gap();
			list.ValueLabeled("ShowWeaponRangeKey", ref RangeFinder.Settings.showWeaponRangeKey);
			list.Gap(24);
			list.ValueLabeled("ShowRangeAtMouseKey", ref RangeFinder.Settings.showRangeAtMouseKey);
			list.Gap(24);
			list.ValueLabeled("UseColorCoding", ref RangeFinder.Settings.useColorCoding);
			list.Gap(24);
			list.IntegerLabeled("MaxRange", ref RangeFinder.Settings.maxRange);
			list.NewColumn();

			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			var savedAnchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			_ = list.Label("CustomColors".Translate());
			Text.Anchor = savedAnchor;

			const float padding = 20f;
			var colors = RangeFinder.Settings.customColors;
			var colorMaterials = RangeFinder.Settings.customColorMaterials;

			var listRect = list.GetRect(canvas.height - list.curY - 24 - 12 - padding);
			var innerWidth = listRect.width - (colors.Count > 16 ? 16 : 0);
			var innerHeight = colors.Count == 0 ? 100 : colors.Count * (24 + 6) + 24;
			var innerRect = new Rect(0f, 0f, innerWidth, innerHeight);
			Widgets.BeginScrollView(listRect, ref scrollPosition, innerRect, true);
			var innerList = new Listing_Standard();
			innerList.Begin(innerRect);

			if (Widgets.ButtonText(innerList.GetRect(24), "AddColor".Translate()))
			{
				var newColor = new Color(Random.value, Random.value, Random.value);
				colors.Add(newColor);
				colorMaterials.Add(MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, newColor));
			}

			for (var i = 0; i < colors.Count; i++)
			{
				innerList.Gap(6);
				var rect = innerList.GetRect(24);

				if (Widgets.ButtonImage(rect.RightPartPixels(24f), Widgets.CheckboxOffTex))
				{
					colors.RemoveAt(i);
					colorMaterials.RemoveAt(i);
					if (colors.NullOrEmpty())
						break;
					if (i > 0)
						i--;
				}

				rect.xMax -= padding + 24f;
				Widgets.DrawBoxSolid(rect.LeftPartPixels(40), colors[i]);
				rect.xMin += 40 + padding;
				var col = (rect.xMax - rect.xMin - 2 * padding) / 3;

				var r = Tools.HorizontalSlider(rect.LeftPartPixels(col), colors[i].r, 0f, 1f, true, "Red".Translate());
				rect.xMin += col + padding;
				var g = Tools.HorizontalSlider(rect.LeftPartPixels(col), colors[i].g, 0f, 1f, true, "Green".Translate());
				rect.xMin += col + padding;
				var b = Tools.HorizontalSlider(rect.LeftPartPixels(col), colors[i].b, 0f, 1f, true, "Blue".Translate());

				var oldColor = colors[i];
				colors[i] = new Color(r, g, b);
				if (colorMaterials[i] == null || colors[i] != oldColor)
					colorMaterials[i] = MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, colors[i]);
			}
			if (colors.Count == 0)
			{
				innerList.Gap();
				Text.Font = GameFont.Tiny;
				_ = innerList.Label("AddingCustomColors".Translate());
			}

			innerList.End();
			Widgets.EndScrollView();

			list.End();
		}
	}
}
