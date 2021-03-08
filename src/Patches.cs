using MelonLoader;
using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AssetLoader
{
    [HarmonyPatch(typeof(GameAudioManager), "Start")]
    internal class GameAudioManager_LoadSoundBanksPath
    {
        internal static void Postfix()
        {
            ModSoundBankManager.RegisterPendingSoundBanks();
        }
    }

    // Hinterland loads assets by calling Resources.Load which ignores external AssetBundles
    // so we need to patch Resources.Load to redirect specific calls to load from the AssetBundle instead
    [HarmonyPatch]
    internal class Resources_Load
    {
        static MethodBase TargetMethod()
        {
            MethodInfo[] methods = typeof(Resources).GetMethods();
            foreach(MethodInfo m in methods)
            {
                if(m.Name == "Load" && m.ReturnType == typeof(UnityEngine.Object) && !m.IsGenericMethod && m.GetParameters().Length == 1)
                {
                    return m;
                }
                
            }
            Implementation.Log("Resources.Load not found for patch.");
            return null;
        }
        internal static bool Prefix(string path, ref UnityEngine.Object __result)
        {
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
    [HarmonyPatch]
    internal class AssetBundle_LoadAsset
    {
        static MethodBase TargetMethod()
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
        }
        private static bool Prefix(string name, ref UnityEngine.Object __result)
        {
            //Implementation.Log("AssetBundle.Load: '{0}'", name);
            if (ModAssetBundleManager.loadingFromExternalBundle || !ModAssetBundleManager.IsKnownAsset(name))
            {
                //Implementation.Log("AssetBundle.Load skipping '{0}'", name);
                return true;
            }
            //Implementation.Log("AssetBundle.Load loading '{0}'", name);
            __result = ModAssetBundleManager.LoadAsset(name);
            if (__result == null)
            {
                Implementation.LogWarning("AssetBundle.LoadAsset failed to load the external asset '{0}'",name);
            }
            return false;
        }
    }

    /*[HarmonyPatch(typeof(AssetBundle), "Contains")]
    internal class UnityEngine_AssetBundle_Contains
    {
        private static void Postfix()
        {
            Implementation.Log("Working!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }*/

    [HarmonyPatch(typeof(GameManager),"Update")]
    internal class LoadWaitingLocalizations
    {
        private static void Postfix()
        {
            if( LocalizationManager.localizationWaitlistAssets.Count > 0 && Localization.IsInitialized())
            {
                LocalizationManager.LoadPendingAssets();
            }
        }
    }

    public class SaveAtlas : MonoBehaviour
    {
        public UIAtlas original;

        public SaveAtlas(IntPtr intPtr) : base(intPtr) { }
    }

    [HarmonyPatch(typeof(UISprite), "SetAtlasSprite")]
    internal class UISprite_set_spriteName
    {
        internal static void Postfix(UISprite __instance)
        {
            UIAtlas atlas = AssetUtils.GetRequiredAtlas(__instance, __instance.mSpriteName);
            if (__instance.atlas == atlas)
            {
                return;
            }
            
            AssetUtils.SaveOriginalAtlas(__instance);
            __instance.atlas = atlas;
        }
    }
}