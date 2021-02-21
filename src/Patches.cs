﻿using MelonLoader;
using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AssetLoader
{
    [HarmonyPatch()]
    internal class DefaultAssetBundleRef_LoadAsset_Texture2D
    {
        internal static bool Prefix(string name, ref Texture2D __result)
        {
            if (!ModAssetBundleManager.IsKnownAsset(name))
            {
                return true;
            }

            __result = ModAssetBundleManager.LoadAsset(name) as Texture2D;
            return __result == null;
        }

        internal static MethodBase TargetMethod()
        {
            MethodInfo[] methods = typeof(DefaultAssetBundleRef).GetMethods();
            foreach (MethodInfo eachMethod in methods)
            {
                if (eachMethod.Name == "LoadAsset" && eachMethod.GetGenericArguments().Length == 1)
                {
                    return eachMethod.MakeGenericMethod(typeof(Texture2D));
                }
            }

            Debug.LogWarning("Could not find target method for patch DefaultAssetBundleRef_LoadAsset_Texture2D.");

            // fallback is our own method, so harmony won't fail during load
            return typeof(DefaultAssetBundleRef_LoadAsset_Texture2D).GetMethod("Prefix");
        }
    }

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
            //AccessTools.Method(typeof(Resources)., "Load", new Type[] { typeof(string) });
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