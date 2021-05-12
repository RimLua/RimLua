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
        
        private static Script environment;
        public static List<AddonInfo> InstalledAddons = new List<AddonInfo>();

        static bool isExcluded(List<string> exludedDirList, string target)
        {
            return exludedDirList.Any(d => new DirectoryInfo(target).Name.Equals(d));
        }

        public static Script GetEnvironment() {
            return environment;
        }

        public static void RegisterAddon(AddonInfo info) {
            InstalledAddons.Add(info);
        }

        // TODO: Construction for path
        private static void LoadFolders(String path) {
            var directories = Directory.GetDirectories(path).Where(d => !isExcluded(new List<string>() { "core" }, d));

            foreach (string directory in directories) {                
                Addon addon = new Addon(directory, environment);
                addon.Load();

                AddonManager.RegisterAddon(addon.Info);
            }
        }

        private static void LoadWithExtension(String path) {
            string[] files = Directory.GetFiles(path, "*.rwa");

            foreach (string file in files)
            {
                Addon addon = new Addon(file, environment);
                addon.LoadZip();

                AddonManager.RegisterAddon(addon.Info);              
            }
        }

        public void Initialize(String addonsPath, String corePath) 
        {
            environment = LuaEnvironment.boundScript();

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

            // TODO: Better?
            AddonManager.LoadFolders(addonsPath);
            AddonManager.LoadWithExtension(addonsPath);

            Table hookTable = environment.Globals.Get("hook").Table;
            environment.Call(hookTable.Get("Call"), "Initialize");
        }
    }
}