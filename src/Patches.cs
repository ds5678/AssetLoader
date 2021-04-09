using Harmony;
using System;
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