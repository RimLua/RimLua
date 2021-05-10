using HarmonyLib;
using RimWorld;
using Verse;

using MoonSharp.Interpreter;

namespace RimLua {

    [HarmonyPatch(typeof(Inspiration), nameof(Inspiration.PostStart))]
    static class Patch_Inspiration_PostStart
    {
        [HarmonyPostfix]
        static void Patch_Inspiration_PostStart_void(Inspiration __instance)
        {            
            Script env = AddonManager.GetEnvironment();
            Table hookTable = env.Globals.Get("hook").Table;
            env.Call(hookTable.Get("Call"), "InspirationGained", __instance.def.defName);
        }
    }

    [HarmonyPatch(typeof(Inspiration), nameof(Inspiration.PostEnd))]
    static class Patch_Inspiration_PostEnd
    {
        [HarmonyPostfix]
        static void Patch_Inspiration_PostEnd_void(Inspiration __instance)
        {            
            Script env = AddonManager.GetEnvironment();
            Table hookTable = env.Globals.Get("hook").Table;
            env.Call(hookTable.Get("Call"), "InspirationEnded", __instance.def.defName);
        }
    }
}