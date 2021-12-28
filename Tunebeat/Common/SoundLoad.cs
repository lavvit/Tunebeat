using Amaoto;

namespace Tunebeat.Common
{
    class SoundLoad
    {
        public static void Init()
        {
            const string DEFAULT = @"Resourses\Sound\";
            Don = new Sound($"{DEFAULT}Don.ogg");
            Ka = new Sound($"{DEFAULT}Ka.ogg");

            if (System.IO.File.Exists($"{DEFAULT}Don_Large.ogg"))
            {
                DON = new Sound($"{DEFAULT}Don_Large.ogg");
            }
            else
            {
                DON = new Sound($"{DEFAULT}Don.ogg");
            }
            
            if (System.IO.File.Exists($"{DEFAULT}Ka_Large.ogg"))
            {
                KA = new Sound($"{DEFAULT}Ka_Large.ogg");
            }
            else
            {
                KA = new Sound($"{DEFAULT}Ka.ogg");
            }
            Balloon = new Sound($"{DEFAULT}Balloon.ogg");
            Kusudama = new Sound($"{DEFAULT}Kusudama.ogg");

            Don2P = new Sound($"{DEFAULT}Don.ogg");
            Ka2P = new Sound($"{DEFAULT}Ka.ogg");

            if (System.IO.File.Exists($"{DEFAULT}Don_Large.ogg"))
            {
                DON2P = new Sound($"{DEFAULT}Don_Large.ogg");
            }
            else
            {
                DON2P = new Sound($"{DEFAULT}Don.ogg");
            }

            if (System.IO.File.Exists($"{DEFAULT}Ka_Large.ogg"))
            {
                KA2P = new Sound($"{DEFAULT}Ka_Large.ogg");
            }
            else
            {
                KA2P = new Sound($"{DEFAULT}Ka.ogg");
            }

            Balloon2P = new Sound($"{DEFAULT}Balloon.ogg");
            Kusudama2P = new Sound($"{DEFAULT}Kusudama.ogg");
        }

        public static Sound Don, Ka, DON, KA, Balloon, Kusudama, Don2P, Ka2P, DON2P, KA2P, Balloon2P, Kusudama2P;
    }
}
