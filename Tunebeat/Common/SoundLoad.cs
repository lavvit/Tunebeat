using Amaoto;

namespace Tunebeat.Common
{
    class SoundLoad
    {
        public static void Init()
        {
            const string SOUND = @"Sound\";
            const string BGM = @"BGM\";
            Don = new Sound($@"{SOUND}{PlayData.SoundName}\Don.ogg");
            Ka = new Sound($@"{SOUND}{PlayData.SoundName}\Ka.ogg");

            if (System.IO.File.Exists($@"{SOUND}{PlayData.SoundName}\Don_Large.ogg"))
            {
                DON = new Sound($@"{SOUND}{PlayData.SoundName}\Don_Large.ogg");
            }
            else
            {
                DON = new Sound($@"{SOUND}{PlayData.SoundName}\Don.ogg");
            }
            
            if (System.IO.File.Exists($@"{SOUND}{PlayData.SoundName}\Ka_Large.ogg"))
            {
                KA = new Sound($@"{SOUND}{PlayData.SoundName}\Ka_Large.ogg");
            }
            else
            {
                KA = new Sound($@"{SOUND}{PlayData.SoundName}\Ka.ogg");
            }
            Balloon = new Sound($@"{SOUND}{PlayData.SoundName}\Balloon.ogg");
            Kusudama = new Sound($@"{SOUND}{PlayData.SoundName}\Kusudama.ogg");

            Don2P = new Sound($@"{SOUND}{PlayData.SoundName}\Don.ogg");
            Ka2P = new Sound($@"{SOUND}{PlayData.SoundName}\Ka.ogg");

            if (System.IO.File.Exists($@"{SOUND}{PlayData.SoundName}\Don_Large.ogg"))
            {
                DON2P = new Sound($@"{SOUND}{PlayData.SoundName}\Don_Large.ogg");
            }
            else
            {
                DON2P = new Sound($@"{SOUND}{PlayData.SoundName}\Don.ogg");
            }

            if (System.IO.File.Exists($@"{SOUND}{PlayData.SoundName}\Ka_Large.ogg"))
            {
                KA2P = new Sound($@"{SOUND}{PlayData.SoundName}\Ka_Large.ogg");
            }
            else
            {
                KA2P = new Sound($@"{SOUND}{PlayData.SoundName}\Ka.ogg");
            }

            Balloon2P = new Sound($@"{SOUND}{PlayData.SoundName}\Balloon.ogg");
            Kusudama2P = new Sound($@"{SOUND}{PlayData.SoundName}\Kusudama.ogg");
        }

        public static Sound Don, Ka, DON, KA, Balloon, Kusudama, Don2P, Ka2P, DON2P, KA2P, Balloon2P, Kusudama2P;
    }
}
