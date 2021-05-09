using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using MoonSharp.Interpreter;
using Verse;

using System.Text;

using UnityEngine;

namespace RimLua 
{
    public class LuaDefController {
        public List<ThingDef> defs = new List<ThingDef>();

        public void AddLuaDef(ThingDef def) {
            defs.Append(def);
        }

        public List<ThingDef> GetLuaDefs() {
            return defs;
        }
    }

    public class LuaDef {
        public int GetDamageAmount(DamageDef damageDef, int damageAmountBase, float weaponDamageMultiplier, StringBuilder explanation = null)
		{
			int num;
			if (damageAmountBase != -1)
			{
				num = damageAmountBase;
			}
			else
			{
				if (damageDef == null)
				{
					Log.ErrorOnce("Failed to find sane damage amount", 91094882, false);
					return 1;
				}
				num = damageDef.defaultDamage;
			}
			if (explanation != null)
			{
				explanation.AppendLine("StatsReport_BaseValue".Translate() + ": " + num);
				explanation.AppendLine();
				explanation.Append("StatsReport_QualityMultiplier".Translate() + ": " + weaponDamageMultiplier.ToStringPercent());
			}
			num = Mathf.RoundToInt((float)num * weaponDamageMultiplier);
			if (explanation != null)
			{
				explanation.AppendLine();
				explanation.AppendLine();
				explanation.Append("StatsReport_FinalValue".Translate() + ": " + num);
			}
			return num;
		}

        private Table content;
        private Script environment;
        private LuaDefController defController;

        public LuaDef(Table defContent, Script luaEnvironment, LuaDefController controller) {
            content = defContent;
            environment = luaEnvironment;
            defController = controller;
        }

        public void Register() {
            ThingDef def = new ThingDef();

            def.defName = content.Get("name").ToString();

            var thingClass = content.Get("thingClass");
            if (thingClass.IsNotNil()) {
                def.thingClass = Type.GetType(thingClass.ToString());
            }

            var label = content.Get("label");
            if (label.IsNotNil()) {
                def.label = label.ToString();
            }

            Table graphicContent = (Table) content.Get("graphicData").ToObject(MoonSharp.Interpreter.DataType.Table.GetType());
            
            var texPath = graphicContent.Get("texPath");
            if (texPath.IsNotNil()) {
                def.graphicData.texPath = texPath.ToString();
            }

            var graphicClass = graphicContent.Get("graphicClass");
            if (graphicClass.IsNotNil()) {
                def.graphicData.graphicClass = Type.GetType(graphicClass.ToString());
            }

            Table projectileContent = (Table) content.Get("projectile").ToObject(MoonSharp.Interpreter.DataType.Table.GetType());

            var flyOverhead = projectileContent.Get("flyOverhead");
            if (flyOverhead.IsNotNil()) {
                def.projectile.flyOverhead = flyOverhead.CastToBool();
            }

            var damageDef = projectileContent.Get("damageDef");
            if (damageDef.IsNotNil()) {
                def.projectile.damageDef = DefDatabase<DamageDef>.GetNamed(damageDef.ToString());
            }

            // HELP!
            //var damageAmountBase = projectileContent.Get("damageAmountBase")
            //if (damageAmountBase.IsNotNil()) {
            //    def.projectile.GetDamageAmount = GetDamageAmount(DefDatabase<DamageDef>.GetNamed(damageDef.ToString()), damageAmountBase);
            //}
        }
    }
}