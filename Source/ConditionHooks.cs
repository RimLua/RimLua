using HarmonyLib;
using RimWorld;
using Verse;

using MoonSharp.Interpreter;

namespace RimLua {
    [HarmonyPatch(typeof(GameCondition), nameof(GameCondition.Init))]
    static class Patch_GameCondition_Init
    {
        [HarmonyPostfix]
        static void Patch_GameCondition_Init_Void(GameCondition __instance)
        {
            Script env = AddonManager.GetEnvironment();
            Table hookTable = env.Globals.Get("hook").Table;
            env.Call(hookTable.Get("Call"), "GameConditionChanged", __instance.def.defName);
        }
    }

    [HarmonyPatch(typeof(GameCondition), nameof(GameCondition.End))]
    static class Patch_GameCondition_End
    {
        [HarmonyPostfix]
        static void Patch_GameCondition_End_Void(GameCondition __instance)
        {            
            Script env = AddonManager.GetEnvironment();
            Table hookTable = env.Globals.Get("hook").Table;
            env.Call(hookTable.Get("Call"), "GameConditionEnd", __instance.def.defName);
        }
    }
}