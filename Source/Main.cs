using HarmonyLib;
using RimWorld;
using System.Reflection;
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
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Settings.DoWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return "RangeFinder";
		}
	}

	// initialize on map load
	//
	[HarmonyPatch(typeof(Map))]
	[HarmonyPatch("FinalizeLoading")]
	static class Map_FinalizeLoading_Patch
	{
		static void Postfix()
		{
			Controller.Instance().Reset();
			ModCounter.Trigger();
		}
	}

	// handle events early
	//
	[HarmonyPatch(typeof(MainTabsRoot))]
	[HarmonyPatch("HandleLowPriorityShortcuts")]
	static class MainTabsRoot_HandleLowPriorityShortcuts_Patch
	{
		static void Postfix()
		{
			Controller.Instance().HandleEvents();
		}
	}

	// handle drawing
	//
	[HarmonyPatch(typeof(SelectionDrawer))]
	[HarmonyPatch("DrawSelectionOverlays")]
	static class SelectionDrawer_DrawSelectionOverlays_Patch
	{
		static void Prefix()
		{
			Controller.Instance().HandleDrawing();
		}
	}
}