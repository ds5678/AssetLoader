using Harmony;
using System;
using System.Reflection;
using UnityEngine;

namespace AssetLoader
{
    class ResourcePatches
    {
        // Hinterland loads assets by calling Resources.Load which ignores external AssetBundles
        // so we need to patch Resources.Load to redirect specific calls to load from the AssetBundle instead
        [HarmonyPatch]
        internal class Resources_Load
        {
            static MethodBase TargetMethod()
            {
                MethodInfo[] methods = typeof(Resources).GetMethods();
                foreach (MethodInfo m in methods)
                {
                    if (m.Name == "Load" && m.ReturnType == typeof(UnityEngine.Object) && !m.IsGenericMethod && m.GetParameters().Length == 1)
                    {
                        return m;
                    }

                }
                Implementation.Log("Resources.Load not found for patch.");
                return null;
            }
            internal static bool Prefix(string path, ref UnityEngine.Object __result)
            {
                if (Implementation.LOG_ALL_ASSET_CALLS && !path.ToLower().StartsWith("gear")) Implementation.Log("Resources.Load is loading '{0}'", path);
                if (!ModAssetBundleManager.IsKnownAsset(path))
                {
                    //Implementation.Log(path);
                    return true;
                }
                //Implementation.Log("Resources.Load is loading '{0}'", path);
                __result = ModAssetBundleManager.LoadAsset(path);
                if (__result == null)
                {
                    Implementation.Log("Resources.Load failed to load the external asset");
                }
                return false;
            }
        }

        //Hinterland stores many of its assets in asset bundles
        //This allows us to enable external asset loading in key locations
        //For example, paperdoll textures are loaded from asset bundles
        [HarmonyPatch(typeof(UnityEngine.AssetBundle), "LoadAsset", new System.Type[] { typeof(string), typeof(Il2CppSystem.Type) })]
        internal class AssetBundle_LoadAsset
        {
            /*static MethodBase TargetMethod()
            {
                MethodInfo[] methods = typeof(UnityEngine.AssetBundle).GetMethods();
                foreach (MethodInfo m in methods)
                {
                    if (m.Name == "LoadAsset" && !m.IsGenericMethod && m.GetParameters().Length == 2)
                    {
                        //Implementation.Log("Found it");
                        return m;
                    }

                }
                Implementation.Log("AssetBundle.LoadAsset not found for patch.");
                return null;
            }*/
            private static bool Prefix(string name, ref UnityEngine.Object __result)
            {
                if (Implementation.LOG_ALL_ASSET_CALLS) Implementation.Log("AssetBundle.Load: '{0}'", name);
                if (ModAssetBundleManager.loadingFromExternalBundle || !ModAssetBundleManager.IsKnownAsset(name))
                {
                    //Implementation.Log("AssetBundle.Load skipping '{0}'", name);
                    return true;
                }
                //Implementation.Log("AssetBundle.Load loading '{0}'", name);
                __result = ModAssetBundleManager.LoadAsset(name);
                if (__result == null)
                {
                    Implementation.LogWarning("AssetBundle.LoadAsset failed to load the external asset '{0}'", name);
                }
                return false;
            }
        }

        /*[HarmonyPatch(typeof(UnityEngine.AssetBundle), "LoadAssetAsync", new System.Type[] { typeof(string), typeof(Il2CppSystem.Type) })]
        internal class AssetBundle_LoadAssetAsync
        {
            private static void Postfix(string name, Il2CppSystem.Type type,AssetBundle __instance)
            {
                Implementation.Log("Tried to asyncronously load '{0}' of type '{1}' from '{2}'", name, type.ToString(),__instance.name);
            }
        }

        [HarmonyPatch(typeof(UnityEngine.AssetBundle), "LoadAllAssets", new System.Type[] { typeof(Il2CppSystem.Type) })]
        internal class AssetBundle_LoadAllAssets
        {
            private static void Postfix(Il2CppSystem.Type type, AssetBundle __instance, UnhollowerBaseLib.Il2CppReferenceArray<UnityEngine.Object> __result)
            {
                Implementation.Log("Tried to load all assets of type '{0}' from '{1}'", type.ToString(), __instance.name);
                foreach(var obj in __result)
                {
                    Implementation.Log("Loaded '{0}'", obj.name);
                }
            }
        }*/
    }
}
