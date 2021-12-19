using Amaoto;

namespace Tunebeat.Common
{
    class TextureLoad
    {
        public static void Init()
        {
            const string DEFAULT = @"Resourses\Image\";
            const string GAME = @"Game\";

            #region Game
            Game_Notes = new Texture($"{DEFAULT}{GAME}Notes.png");
            Game_Bar = new Texture($"{DEFAULT}{GAME}Bar.png");
            #endregion
        }

        #region Game
        public static Texture Game_Notes;
        public static Texture Game_Bar;
        #endregion
    }
}
