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
        }

        public static Sound Don, Ka;
    }
}
