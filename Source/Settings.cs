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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref showRangeAtMouseKey, "showWeaponRangeKey", ModifierKey.Ctrl, true);
			Scribe_Values.Look(ref showWeaponRangeKey, "showRangeAtMouseKey", ModifierKey.Shift, true);
			Scribe_Values.Look(ref useColorCoding, "useColorCoding", BooleanKey.Yes, true);
			Scribe_Values.Look(ref maxRange, "maxRange", 96, true);
		}

		public static void DoWindowContents(Rect canvas)
		{
			var list = new Listing_Standard { ColumnWidth = canvas.width / 2 };
			list.Begin(canvas);
			list.Gap();
			list.ValueLabeled("ShowWeaponRangeKey", ref RangeFinder.Settings.showWeaponRangeKey);
			list.Gap();
			list.ValueLabeled("ShowRangeAtMouseKey", ref RangeFinder.Settings.showRangeAtMouseKey);
			list.Gap();
			list.ValueLabeled("UseColorCoding", ref RangeFinder.Settings.useColorCoding);
			list.Gap();
			list.IntegerLabeled("MaxRange", ref RangeFinder.Settings.maxRange);
			list.End();
		}
	}
}
