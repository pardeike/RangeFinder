using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RangeFinder
{
	class RangeFinder : Mod
	{
		public static RangeFinderSettings Settings;

		public RangeFinder(ModContentPack content) : base(content)
		{
			Settings = GetSettings<RangeFinderSettings>();

			var harmony = new Harmony("net.pardeike.rimworld.mods.rangefinder");
			harmony.PatchAll();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			RangeFinderSettings.DoWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return "RangeFinder";
		}
	}

	// initialize on map load
	//
	[HarmonyPatch(typeof(Map), nameof(Map.FinalizeLoading))]
	static class Map_FinalizeLoading_Patch
	{
		public static void Postfix()
		{
			Controller.Reset();
			ModCounter.Trigger();
		}
	}

	// handle events early
	//
	[HarmonyPatch(typeof(MainTabsRoot), nameof(MainTabsRoot.HandleLowPriorityShortcuts))]
	static class MainTabsRoot_HandleLowPriorityShortcuts_Patch
	{
		public static void Postfix()
		{
			Controller.Instance().HandleEvents();
		}
	}

	// handle drawing
	//
	[HarmonyPatch(typeof(SelectionDrawer), nameof(SelectionDrawer.DrawSelectionOverlays))]
	static class SelectionDrawer_DrawSelectionOverlays_Patch
	{
		public static void Prefix()
		{
			Controller.HandleDrawing();
		}
	}
}
