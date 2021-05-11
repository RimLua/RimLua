using System.Collections.Generic;
using MoonSharp.Interpreter;
using Verse;

namespace RimLua
{
    public static class LuaEnvironment
    {
        private static Dictionary<string, DynValue> dict = new Dictionary<string, DynValue>();

        static LuaEnvironment()
        {
            // Type classes
            UserData.RegisterType<Verse.IntVec2>();
            UserData.RegisterType<Verse.IntVec3>();
            UserData.RegisterType<UnityEngine.Vector2>();
            UserData.RegisterType<UnityEngine.Vector2Int>();
            UserData.RegisterType<UnityEngine.Vector3>();
            UserData.RegisterType<UnityEngine.Vector3Int>();
            UserData.RegisterType<UnityEngine.Vector4>();

            // Defs and defs dependencies
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

            // Other
            UserData.RegisterType<Verse.Map>();
            UserData.RegisterType<Verse.Explosion>();

            UserData.RegisterType<RimWorld.Plant>();
            UserData.RegisterType<RimWorld.Inspiration>();
            
        }
        public static Script boundScript()
        {
            Script environment = new Script();

            environment.Globals.RegisterModuleType<LuaEnvironmentFunctions>();

            // Types
            environment.Globals["IntVec2"] = typeof(Verse.IntVec2);
            environment.Globals["IntVec3"] = typeof(Verse.IntVec3);
            environment.Globals["Vector2"] = typeof(UnityEngine.Vector2);
            environment.Globals["Vector2Int"] = typeof(UnityEngine.Vector2Int);
            environment.Globals["Vector3"] = typeof(UnityEngine.Vector3);
            environment.Globals["Vector3Int"] = typeof(UnityEngine.Vector3Int);
            environment.Globals["Vector4"] = typeof(UnityEngine.Vector4);

            // Defs and defs dep.
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

            return environment;
        }

        public static DynValue Get(Script script, string key)
        {
            if (dict.ContainsKey(key))
            {
                if (dict[key].Type == DataType.Table)
                {
                    DynValue t = DynValue.NewTable(script);
                    foreach (TablePair pair in dict[key].Table.Pairs)
                    {
                        t.Table.Set(pair.Key, pair.Value);
                    }
                    return t;
                }
                else
                {
                    return dict[key];
                }
            }
            return null;
        }

        public static void Set(Script script, string key, DynValue value)
        {
            dict[key] = value;
        }

        public static void Clear()
        {
            dict.Clear();
        }
    }
}