using HarmonyLib;
using RimWorld;

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