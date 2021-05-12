using System.IO;

using MoonSharp.Interpreter;
using Verse;

using ICSharpCode.SharpZipLib.Zip;

namespace RimLua 
{
    public class Addon {
        public AddonInfo Info;
        private Script environment;

        public Addon(string addonPath, Script env) {
            DirectoryInfo dirInfo = new DirectoryInfo(addonPath);
            Info = new AddonInfo(addonPath, dirInfo.Name, true);

            environment = env;
        }
        public void Load() {
            

            Log.Message("[RimLua] " + Info.Name + " addon was loaded");
            environment.Options.DebugPrint = s => Log.Message("["+Info.Name+"] " + s);
            
            string[] files = Directory.GetFiles(Info.RootDir, "*.lua");
            foreach (string file in files)
            {
                try
	            {
                    string code = System.IO.File.ReadAllText(file);
                    environment.DoString(code);
                }
                catch (ScriptRuntimeException ex)
                {
                    Log.Message("[RimLua " + Info.Name + "] An error occured! " + ex.DecoratedMessage);
                }
            }
        }

        public void LoadZip() {
            DirectoryInfo dirInfo = new DirectoryInfo(Info.RootDir);
            Info.Name = dirInfo.Name;  

            Log.Message("[RimLua] " + Info.Name + " addon was loaded");
            environment.Options.DebugPrint = s => Log.Message("["+Info.Name+"] " + s);
            
            var zip = new ZipInputStream(File.OpenRead(Info.RootDir));
            var filestream = new FileStream(Info.RootDir, FileMode.Open, FileAccess.Read);

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
                        Log.Message("[RimLua " + Info.Name + "] An error occured! " + ex.DecoratedMessage);
                    }
                }
            }
        }
    }
}