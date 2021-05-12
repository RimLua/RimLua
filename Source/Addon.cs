using System.IO;

using MoonSharp.Interpreter;
using Verse;

using ICSharpCode.SharpZipLib.Zip;

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

        public void LoadZip() {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            string addonName = dirInfo.Name;  

            Log.Message("[RimLua] " + addonName + " addon was loaded");
            environment.Options.DebugPrint = s => Log.Message("["+addonName+"] " + s);
            
            var zip = new ZipInputStream(File.OpenRead(path));
            var filestream = new FileStream(path, FileMode.Open, FileAccess.Read);

            ZipFile zipfile = new ZipFile(filestream);
            ZipEntry item;
            
            while ((item = zip.GetNextEntry()) != null)
            {
                using (StreamReader s = new StreamReader(zipfile.GetInputStream(item)))
                {
                    try
	                {
                    environment.DoString(s.ReadToEnd());
                    }
                    catch (ScriptRuntimeException ex) {
                        Log.Message("[RimLua " + addonName + "] An error occured! " + ex.DecoratedMessage);
                    }
                }
            }
        }
    }
}