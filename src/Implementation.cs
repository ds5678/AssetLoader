﻿using UnityEngine;
using System.Reflection;
using MelonLoader;
using System.IO;

namespace AssetLoader
{
    public class Implementation : MelonMod
    {
        private const string NAME = "AssetLoader";

        public override void OnApplicationStart()
        {
            Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<SaveAtlas>();
        }

        internal static void Log(string message)
        {
            Debug.Log("[" + NAME + "]: " + message);
        }

        internal static void Log(string message, params object[] parameters)
        {
            string preformattedMessage = string.Format("[" + NAME + "] {0}", message);
            Debug.Log(preformattedMessage); //, parameters);
        }

        internal static string getModsFolderPath()
        {
            return Path.GetFullPath(typeof(MelonMod).Assembly.Location + @"\..\..\Mods");
        }
    }
}