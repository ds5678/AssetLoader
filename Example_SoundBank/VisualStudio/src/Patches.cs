using Harmony;

namespace HomemadeSoupMod
{
	internal static class Patches
	{
		private const string TINDER_NAME = "GEAR_Tinder";
		private const string SOUND_NAME = "Play_CrinklePaper";

		//Changes the crafting sound of Tinder Plugs
		[HarmonyPatch(typeof(BlueprintDisplayItem), "Setup")]
		private static class ChangeTinderSound
		{
			internal static void Postfix(BlueprintDisplayItem __instance, BlueprintItem bpi)
			{
				//If the blueprint produces a tinder plug
				if (bpi?.m_CraftedResult?.name == TINDER_NAME)
				{
					//Change the crafting audio to our added sound
					bpi.m_CraftingAudio = SOUND_NAME; //This is the name of the EVENT I created in WWise
				}
			}
		}

		//Allows the player to trigger the sound with a keystroke
		[HarmonyPatch(typeof(GameManager),"Update")]
		private static class PlaySound
        {
			internal static void Postfix()
            {
                //If P has been pressed
				if (InputManager.GetKeyDown(InputManager.m_CurrentContext, UnityEngine.KeyCode.P))
                {
					//Play the sound
					GameAudioManager.PlaySound(SOUND_NAME, InterfaceManager.GetSoundEmitter());
                }
            }
        }
	}
}