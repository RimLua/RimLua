using Verse;
using System;
using System.Reflection;
using System.IO;

using HarmonyLib;

using System.Collections.Generic;

namespace RimLua
{
    public class AddonInfo 
    {
        public String RootDir;
        public bool Active;
        public String Name;
        
        public AddonInfo(String _RootDir, String _Name, bool _Active) {
            RootDir = _RootDir;
            Name = _Name;

            Active = _Active;
        }
    }
}