using MelonLoader;
using System.IO;
using UnityEngine;

namespace AssetLoader
{
    public class Implementation : MelonMod
    {
        public const bool LOG_ALL_ASSET_CALLS = false;

        public override void OnApplicationStart()
        {
            Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<SaveAtlas>();
        }

        internal static void Log(string message) => MelonLogger.Log(message);

        internal static void Log(string message, params object[] parameters) => MelonLogger.Log(message, parameters);

        internal static void LogWarning(string message) => MelonLogger.LogWarning(message);

        internal static void LogWarning(string message, params object[] parameters) => MelonLogger.LogWarning(message, parameters);

        internal static string getModsFolderPath()
        {
            return Path.GetFullPath(typeof(MelonMod).Assembly.Location + @"\..\..\Mods");
        }
    }
}