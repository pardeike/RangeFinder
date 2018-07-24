using UnityEngine;
using Verse;

namespace RangeFinder
{
	public enum ModKey
	{
		None,
		Alt,
		Ctrl,
		Shift,
		Meta
	}

	public class RangeFinderSettings : ModSettings
	{
		public ModKey showWeaponRangeKey = ModKey.Ctrl;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref showWeaponRangeKey, "showWeaponRangeKey", ModKey.Ctrl, true);
		}

		public void DoWindowContents(Rect canvas)
		{
			var list = new Listing_Standard { ColumnWidth = canvas.width / 2 };
			list.Begin(canvas);
			list.Gap();
			list.ValueLabeled("ShowWeaponRangeKey", ref RangeFinder.Settings.showWeaponRangeKey);
			list.End();
		}
	}
}