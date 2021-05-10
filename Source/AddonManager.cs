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

        static bool isExcluded(List<string> exludedDirList, string target)
        {
            return exludedDirList.Any(d => new DirectoryInfo(target).Name.Equals(d));
        }

        public void Initialize(String addonsPath, String corePath) 
        {
            Script script = new Script();
            script.Globals.RegisterModuleType<RimLuaFunctions>();

            string[] files = Directory.GetFiles(corePath, "*.lua");
            foreach (string file in files)
            {
                try
	            {
                    string code = System.IO.File.ReadAllText(file);
                    script.DoString(code);
                }
                catch (ScriptRuntimeException ex)
                {
                    Log.Message("[RimLua Core] An error occured! " + ex.DecoratedMessage);
                }
            }

            var directories = Directory.GetDirectories(addonsPath).Where(d => !isExcluded(_excludedDirectories, d));
            foreach (string directory in directories) {                
                Addon addon = new Addon(directory, script);
                addon.Load();
            }

            Table hookTable = script.Globals.Get("hook").Table;
            script.Call(hookTable.Get("Call"), "Initialize");
        }
    }
}