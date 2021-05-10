using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using MoonSharp.Interpreter;
using Verse;

namespace RimLua
{
    public class AddonManager 
    {
        private List<string> _excludedDirectories= new List<string>() { "core" };
        public static Script environment;

        static bool isExcluded(List<string> exludedDirList, string target)
        {
            return exludedDirList.Any(d => new DirectoryInfo(target).Name.Equals(d));
        }

        public static Script GetEnvironment() {
            return environment;
        }

        public void Initialize(String addonsPath, String corePath) 
        {
            environment = new Script();
            
            environment.Globals.RegisterModuleType<RimLuaFunctions>();
            
            UserData.RegisterType<Verse.Def>();
            UserData.RegisterType<Verse.ThingDef>();
            UserData.RegisterType<RimWorld.InspirationDef>();

            UserData.RegisterType<Verse.Map>();

            UserData.RegisterType<RimWorld.Plant>();
            UserData.RegisterType<RimWorld.Inspiration>();

            string[] files = Directory.GetFiles(corePath, "*.lua");
            foreach (string file in files)
            {
                try
	            {
                    string code = System.IO.File.ReadAllText(file);
                    environment.DoString(code);
                }
                catch (ScriptRuntimeException ex)
                {
                    Log.Message("[RimLua Core] An error occured! " + ex.DecoratedMessage);
                }
            }

            var directories = Directory.GetDirectories(addonsPath).Where(d => !isExcluded(_excludedDirectories, d));
            foreach (string directory in directories) {                
                Addon addon = new Addon(directory, environment);
                addon.Load();
            }

            Table hookTable = environment.Globals.Get("hook").Table;
            environment.Call(hookTable.Get("Call"), "Initialize");
        }
    }
}