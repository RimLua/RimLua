using HarmonyLib;
using RimWorld;
using Verse;

namespace RimLua
{
    [HarmonyPatch( typeof(DefGenerator), nameof( DefGenerator.GenerateImpliedDefs_PreResolve ) )]
    public class HarmonyPatch_GenerateImpliedDefs_PreResolve
    {
        public static void Prefix()
        {
            
        }
    }
}