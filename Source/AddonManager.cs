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
            
            UserData.RegisterType<Verse.IntVec2>();
            UserData.RegisterType<Verse.IntVec3>();
            UserData.RegisterType<UnityEngine.Vector2>();
            UserData.RegisterType<UnityEngine.Vector2Int>();
            UserData.RegisterType<UnityEngine.Vector3>();
            UserData.RegisterType<UnityEngine.Vector3Int>();
            UserData.RegisterType<UnityEngine.Vector4>();

            environment.Globals["IntVec2"] = typeof(Verse.IntVec2);
            environment.Globals["IntVec3"] = typeof(Verse.IntVec3);
            environment.Globals["Vector2"] = typeof(UnityEngine.Vector2);
            environment.Globals["Vector2Int"] = typeof(UnityEngine.Vector2Int);
            environment.Globals["Vector3"] = typeof(UnityEngine.Vector3);
            environment.Globals["Vector3Int"] = typeof(UnityEngine.Vector3Int);
            environment.Globals["Vector4"] = typeof(UnityEngine.Vector4);

            UserData.RegisterType<Verse.Def>();
            UserData.RegisterType<RimWorld.TraitDegreeData>();
            UserData.RegisterType<RimWorld.SkillDef>();
            UserData.RegisterType<Verse.WorkTypeDef>();

            UserData.RegisterType<Verse.WorkTags>();
            UserData.RegisterType<Verse.DestroyMode>();

            UserData.RegisterType<Verse.Thing>();
            UserData.RegisterType<Verse.ThingDef>();
            UserData.RegisterType<Verse.HediffDef>();
            UserData.RegisterType<RimWorld.InspirationDef>();
            UserData.RegisterType<RimWorld.TraitDef>();

            UserData.RegisterType<Verse.Explosion>();

            environment.Globals["HediffDef"] = typeof(HediffDef);
            environment.Globals["ThingDef"] = typeof(ThingDef);
            environment.Globals["InspirationDef"] = typeof(RimWorld.InspirationDef);
            environment.Globals["TraitDef"] = typeof(RimWorld.TraitDef);
            environment.Globals["TraitDegreeData"] = typeof(RimWorld.TraitDegreeData);
            environment.Globals["SkillDef"] = typeof(RimWorld.SkillDef);
            environment.Globals["WorkTypeDef"] = typeof(Verse.WorkTypeDef);
            environment.Globals["WorkTags"] = typeof(Verse.WorkTags);
            environment.Globals["DestroyMode"] = typeof(Verse.DestroyMode);
            environment.Globals["Explosion"] = typeof(Verse.Explosion);

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