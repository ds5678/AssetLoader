using MelonLoader;
using AssetLoader;
using UnityEngine;

namespace CrinklePaperSoundExample
{
    public class Implementation : MelonMod
    {
        //Position of the sound bank within the mods folder
        public const string SOUND_BANK_PATH = @"ExternalAssets\SoundBankExample.bnk";
        public override void OnApplicationStart()
        {
            Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");

            //Sound banks need to be registered. This adds all the events to the in-game audio manager.
            ModSoundBankManager.RegisterSoundBank(SOUND_BANK_PATH);
        }
    }
}
