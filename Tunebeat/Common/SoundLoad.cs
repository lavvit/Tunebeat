using SeaDrop;

namespace Tunebeat.Common
{
    class SoundLoad
    {
        public static void Init()
        {
            const string SOUND = @"Sound\";
            const string BGM = @"BGM\";
            for (int i = 0; i < 5; i++)
            {
                Don[i] = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Don.ogg");
                Ka[i] = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Ka.ogg");

                if (System.IO.File.Exists($@"{SOUND}{PlayData.Data.SoundName}\Don_Large.ogg"))
                {
                    DON[i] = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Don_Large.ogg");
                }
                else
                {
                    DON[i] = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Don.ogg");
                }

                if (System.IO.File.Exists($@"{SOUND}{PlayData.Data.SoundName}\Ka_Large.ogg"))
                {
                    KA[i] = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Ka_Large.ogg");
                }
                else
                {
                    KA[i] = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Ka.ogg");
                }
                Balloon[i] = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Balloon.ogg");
                Kusudama[i] = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Kusudama.ogg");
            }
            Metronome = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Metronome.ogg");
            Metronome_Bar = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Metronome_Bar.ogg");
        }

        public static Sound Metronome, Metronome_Bar;
        public static Sound[] Don = new Sound[5], Ka = new Sound[5], DON = new Sound[5], KA = new Sound[5], Balloon = new Sound[5], Kusudama = new Sound[5];
    }
}
