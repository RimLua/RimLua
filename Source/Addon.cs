using System.IO;

using MoonSharp.Interpreter;
using Verse;

namespace RimLua 
{
    public class Addon {
        private string path;
        private Script environment;

        public Addon(string addonPath, Script env) {
            path = addonPath;
            environment = env;
        }
        public void Load() {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            string addonName = dirInfo.Name;  

            Log.Message("[RimLua] " + addonName + " addon was loaded");
            environment.Options.DebugPrint = s => Log.Message("["+addonName+"] " + s);

            string[] files = Directory.GetFiles(path, "*.lua");
            foreach (string file in files)
            {
                try
	            {
                    string code = System.IO.File.ReadAllText(file);
                    environment.DoString(code);
                }
                catch (ScriptRuntimeException ex)
                {
                    Log.Message("[RimLua " + addonName + "] An error occured! " + ex.DecoratedMessage);
                }
            }
        }
    }
}