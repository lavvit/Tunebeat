﻿using Amaoto;

namespace Tunebeat.Common
{
    class PlayData
    {
        public static void Init()
        {
            PlayFile = @"Songs/水天神術・時雨.tja";
            PlayCourse = 3;

            Auto = false;
        }

        public static string PlayFile;
        public static int PlayCourse;

        public static bool Auto;
    }
}