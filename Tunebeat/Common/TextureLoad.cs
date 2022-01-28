using Amaoto;
using Tunebeat;

namespace Tunebeat.Common
{
    class TextureLoad
    {
        public static void Init()
        {
            const string DEFAULT = @"Graphic\";
            const string TITLE = @"\Title\";
            const string PLAYER = @"\Player\";
            const string MODE = @"\ModeSelect\";
            const string SONG = @"\SongSelect\";
            const string GAME = @"\Game\";
            const string RESULT = @"\Result\";

            #region Title
            Title_Background = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{TITLE}Background.png");
            Title_Text = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{TITLE}Title.png");
            Title_Text_Color = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{TITLE}Title_Color.png");
            #endregion
            #region Player
            #endregion
            #region ModeSelect
            #endregion
            #region SongSelect
            SongSelect_Background = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Background.png");
            SongSelect_Bar = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Bar.png");
            SongSelect_Clear = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Clear.png");
            SongSelect_Difficulty = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty.png");
            SongSelect_Difficulty_Base = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_Base.png");
            SongSelect_Difficulty_Course = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_Course.png");
            SongSelect_Difficulty_Cursor = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_Cursor.png");
            SongSelect_Difficulty_Level = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_Level.png");
            SongSelect_Difficulty_Number = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_Number.png");
            SongSelect_Difficulty_TJA = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_TJA.png");
            #endregion
            #region Game
            Game_Background = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Background.png");
            Game_Base = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Base.png");
            Game_Base_DP = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Base_DP.png");
            Game_Base_Info = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Base_Info.png");
            Game_Base_Info_DP = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Base_Info_DP.png");
            Game_Notes = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Notes.png");
            Game_Bar = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Bar.png");
            Game_Lane = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Lane.png");
            Game_Lane_Gogo = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Lane_Gogo.png");
            Game_Lane_Frame = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Lane_Frame.png");
            Game_Lane_Frame_DP = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Lane_Frame_DP.png");
            Game_Sudden = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Sudden.png");
            Game_HiSpeed = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}HiSpeed.png");
            Game_Don[0] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Don_L.png");
            Game_Don[1] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Don_R.png");
            Game_Ka[0] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Ka_L.png");
            Game_Ka[1] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Ka_R.png");
            Game_Don2P[0] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Don_L.png");
            Game_Don2P[1] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Don_R.png");
            Game_Ka2P[0] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Ka_L.png");
            Game_Ka2P[1] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Ka_R.png");
            Game_Gauge = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Gauge.png");
            Game_Gauge_Base = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Gauge_Base.png");
            Game_Judge = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Judge.png");
            Game_Judge_Data = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Judge.png");
            Game_Graph = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Graph.png");
            Game_Graph_Base = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Graph_Base.png");
            Game_Rank = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Rank.png");
            Game_Number = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Number.png");
            Game_Number_Mini = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Number_Mini.png");

            for (int i = 0; i < Game_Bomb.Length; i++)
            {
                Game_Bomb[i] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Bomb_{i}.png");
            }
            #endregion
            #region Result
            Result_Background = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{RESULT}Background.png");
            Result_Panel = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{RESULT}Panel.png");
            Result_FastRate = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{RESULT}FastRate.png");
            Result_Gauge = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{RESULT}Gauge.png");
            Result_Rank = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{RESULT}Rank.png");
            #endregion
        }

        #region Title
        public static Texture Title_Background,
            Title_Text,
            Title_Text_Color;
        #endregion
        #region Player
        #endregion
        #region ModeSelect
        #endregion
        #region SongSelect
        public static Texture SongSelect_Background,
            SongSelect_Bar,
            SongSelect_Clear,
            SongSelect_Difficulty,
            SongSelect_Difficulty_Base,
            SongSelect_Difficulty_Course,
            SongSelect_Difficulty_Cursor,
            SongSelect_Difficulty_Level,
            SongSelect_Difficulty_Number,
            SongSelect_Difficulty_TJA;
        #endregion
        #region Game
        public static Texture Game_Background,
            Game_Base,
            Game_Base_DP,
            Game_Base_Info,
            Game_Base_Info_DP,
            Game_Notes,
            Game_Bar,
            Game_Lane,
            Game_Lane_Gogo,
            Game_Lane_Frame,
            Game_Lane_Frame_DP,
            Game_Sudden,
            Game_HiSpeed,
            Game_Gauge,
            Game_Gauge_Base,
            Game_Judge,
            Game_Judge_Data,
            Game_Graph,
            Game_Graph_Base,
            Game_Rank,
            Game_Number,
            Game_Number_Mini;
        public static Texture[] Game_Don = new Texture[2],
            Game_Ka = new Texture[2],
            Game_Don2P = new Texture[2],
            Game_Ka2P = new Texture[2],
            Game_Bomb = new Texture[12];
        #endregion
        #region Result
        public static Texture Result_Background,
            Result_Panel,
            Result_FastRate,
            Result_Gauge,
            Result_Rank;
        #endregion
    }
}
