using Verse;
using RimWorld;
using MoonSharp.Interpreter;
using System;
using System.Reflection;
using System.IO;

using HarmonyLib;

namespace RimLua
{
    [StaticConstructorOnStartup]
    public class RimLua : Mod 
    {
        public RimLua(ModContentPack content) : base(content)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var addonsPath = Path.Combine(path, "RimLua-addons");
            var corePath = Path.Combine(addonsPath, "core");

            System.IO.Directory.CreateDirectory(addonsPath);
            System.IO.Directory.CreateDirectory(corePath);
            
            AddonManager manager = new AddonManager();
            manager.Initialize(addonsPath, corePath);

            var harmony = new Harmony( "rorkh.rimlua" );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );

            Log.Message("[RimLua] Loaded");
        }
    }
}
