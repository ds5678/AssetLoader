using UnityEngine;
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
            MelonLogger.Log(message);
        }

        internal static void Log(string message, params object[] parameters)
        {
            Log(string.Format(message, parameters));
        }

        internal static void LogWarning(string message)
        {
            MelonLogger.LogWarning(message);
        }

        internal static void LogWarning(string message, params object[] parameters)
        {
            LogWarning(string.Format(message, parameters));
        }

        internal static string getModsFolderPath()
        {
            return Path.GetFullPath(typeof(MelonMod).Assembly.Location + @"\..\..\Mods");
        }
    }
}