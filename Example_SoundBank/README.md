# Sound Bank Example

Asset Loader can be used to add additional sounds to the game. To illustrate its use and give a small tutorial, I made a small mod changing the sound of tinder crafting and allowing the user to play the sound by pressing `P`.

## The Initial Sound File

Sound files can be recorded or obtained open source. Their names do not matter at all. However, they need to be in WAV format. If a sound file is not in WAV format, it can probably be converted easily using [Audacity](https://www.audacityteam.org/download/).

## Creating a Sound Bank

Sound Banks are created with [Wwise](https://www.audiokinetic.com/download/) version `2018.1.11`. No other version will work.

Audiokinetic gives an excellent [tutorial](https://www.audiokinetic.com/courses/wwise101/?source=wwise101&id=quick_start_from_silence_to_sound#read) on using Wwise. For this example, it's only necessary to do the first lesson.

The most important thing to remember is to name your event correctly because that's the string that's used to play your sound. Most events are named with this format: `"Play_YourSoundName"`

I have included my pregenerated sound bank. It goes in the `ExternalAssets` folder within your `Mods` folder.

## Visual Studio Project

All sound banks must be registered in `OnApplicationStart`. This loads all the sounds into the in-game audio manager. After this, it is indistinguishable from vanilla sounds.