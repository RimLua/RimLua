using HarmonyLib;
using RimWorld;
using Verse;

using UnityEngine;
using System.Collections.Generic;
using System;

using RimLua.Windows;

namespace RimLua {
    [HarmonyPatch(typeof(MainMenuDrawer), nameof(MainMenuDrawer.DoMainMenuControls))]
    public static class MainMenuMarker
    {
        public static bool drawing;

        static void Prefix() => drawing = true;
        static void Postfix() => drawing = false;
    }

    [HarmonyPatch(typeof(MainMenuDrawer), nameof(MainMenuDrawer.DoMainMenuControls))]
    public static class MainMenu_AddHeight
    {
        static void Prefix(ref Rect rect) => rect.height += 45f;
    }

    [HarmonyPatch(typeof(OptionListingUtility), nameof(OptionListingUtility.DrawOptionListing))]
    public static class MainMenuPatch
    {
        static void Prefix(Rect rect, List<ListableOption> optList)
        {
            if (!MainMenuMarker.drawing) return;

            if (Current.ProgramState == ProgramState.Entry)
            {
                int newColony = optList.FindIndex(opt => opt.label == "Mods".Translate());
                if (newColony != -1)
                {
                    optList.Insert(newColony + 1, new ListableOption("Addons".Translate(), () =>
                    {                
                        Find.WindowStack.Add(new Page_Addons());
                    }));
                }
            }
        }
    }
}