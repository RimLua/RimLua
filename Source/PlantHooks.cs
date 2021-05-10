using HarmonyLib;
using RimWorld;
using Verse;

using MoonSharp.Interpreter;

namespace RimLua {
    [HarmonyPatch(typeof(Plant), nameof(Plant.PostMapInit))]
    static class Patch_Plant_PostMapInit
    {
        [HarmonyPostfix]
        static void Patch_Plant_PostMapInit_Void(Plant __instance)
        {            
            Script env = AddonManager.GetEnvironment();
            Table hookTable = env.Globals.Get("hook").Table;
            env.Call(hookTable.Get("Call"), "PlantPostMapInit", __instance);
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.SpawnSetup))]
    static class Patch_Plant_SpawnSetup
    {
        [HarmonyPostfix]
        static void Patch_Plant_SpawnSetup_Void(Plant __instance, Map map, bool respawningAfterLoad)
        {            
            Script env = AddonManager.GetEnvironment();
            Table hookTable = env.Globals.Get("hook").Table;
            env.Call(hookTable.Get("Call"), "PlantSpawnSetup", UserData.Create(__instance), map, respawningAfterLoad);
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.PlantCollected))]
    static class Patch_Plant_PlantCollected
    {
        [HarmonyPostfix]
        static void Patch_Plant_PlantCollected_Void(Plant __instance)
        {            
            Script env = AddonManager.GetEnvironment();
            Table hookTable = env.Globals.Get("hook").Table;
            env.Call(hookTable.Get("Call"), "PlantCollected", UserData.Create(__instance));
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.CanYieldNow))]
    static class Patch_Plant_CanYieldNow
    {
        [HarmonyPrefix]
        static bool Patch_Plant_CanYieldNow_Bool(Plant __instance, ref bool __result)
        {            
            Script env = AddonManager.GetEnvironment();
            Table hookTable = env.Globals.Get("hook").Table;
            __result = env.Call(hookTable.Get("Call"), "PlantCanYieldNow", UserData.Create(__instance)).CastToBool();

            return false;
        }
    }
}