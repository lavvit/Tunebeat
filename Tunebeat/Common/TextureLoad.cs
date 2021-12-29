using Amaoto;

namespace Tunebeat.Common
{
    class TextureLoad
    {
        public static void Init()
        {
            const string DEFAULT = @"Graphic\";
            const string GAME = @"Game\";

            #region Game
            Game_Background = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Background.png");
            Game_Base = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Base.png");
            Game_Base_DP = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Base_DP.png");
            Game_Notes = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Notes.png");
            Game_Bar = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Bar.png");
            Game_Lane = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Lane.png");
            Game_Lane_Frame = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Lane_Frame.png");
            Game_Lane_Frame_DP = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Lane_Frame_DP.png");
            Game_Don[0] = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Don_L.png");
            Game_Don[1] = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Don_R.png");
            Game_Ka[0] = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Ka_L.png");
            Game_Ka[1] = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Ka_R.png");
            Game_Don2P[0] = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Don_L.png");
            Game_Don2P[1] = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Don_R.png");
            Game_Ka2P[0] = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Ka_L.png");
            Game_Ka2P[1] = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Ka_R.png");
            Game_Gauge = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Gauge.png");
            Game_Gauge_Base = new Texture($@"{DEFAULT}{PlayData.SkinName}\{GAME}Gauge_Base.png");
            #endregion
        }

        #region Game
        public static Texture Game_Background,
            Game_Base,
            Game_Base_DP,
            Game_Notes,
            Game_Bar,
            Game_Lane,
            Game_Lane_Frame,
            Game_Lane_Frame_DP,
            Game_Gauge,
            Game_Gauge_Base;
        public static Texture[] Game_Don = new Texture[2],
            Game_Ka = new Texture[2],
            Game_Don2P = new Texture[2],
            Game_Ka2P = new Texture[2];
        #endregion
    }
}
