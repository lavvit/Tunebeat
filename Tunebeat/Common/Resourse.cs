using SeaDrop;

namespace Tunebeat
{
    public class Resourse
    {
        public static void Init()
        {
            Tx.Init();
            Sfx.Init();
        }
    }

    public class Tx
    {
        public static void Init()
        {
            const string DEFAULT = @"Graphic\";
            const string TITLE = @"\Title\";
            //const string PLAYER = @"\Player\";
            //const string MODE = @"\ModeSelect\";
            const string SONG = @"\SongSelect\";
            const string GAME = @"\Game\";
            const string BMS = @"\Game\BMS\";
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
            SongSelect_Bar_Color = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Bar_Color.png");
            SongSelect_Bar_Cursor = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Bar_Cursor.png");
            SongSelect_Bar_Folder = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Bar_Folder.png");
            SongSelect_Bar_Folder_Color = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Bar_Folder_Color.png");
            SongSelect_Clear = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Clear.png");
            SongSelect_Difficulty = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty.png");
            SongSelect_Difficulty_Base = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_Base.png");
            SongSelect_Difficulty_Course = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_Course.png");
            SongSelect_Difficulty_Cursor = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_Cursor.png");
            SongSelect_Difficulty_Level = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_Level.png");
            SongSelect_Difficulty_Number = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_Number.png");
            SongSelect_Difficulty_TJA = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Difficulty_TJA.png");
            SongSelect_Number = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{SONG}Number.png");
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
            for (int i = 0; i < 5; i++)
            {
                Game_Don[i] = new Texture[2];
                Game_Ka[i] = new Texture[2];
                Game_Don[i][0] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Don_L.png");
                Game_Don[i][1] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Don_R.png");
                Game_Ka[i][0] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Ka_L.png");
                Game_Ka[i][1] = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{GAME}Ka_R.png");
            }
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
            #region BMS
            BMS_Background = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{BMS}Background.png");
            BMS_Lane = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{BMS}Lane.png");
            BMS_Notes = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{BMS}Notes.png");
            BMS_Long = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{BMS}Long.png");
            BMS_Long_Charge = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{BMS}Long_Charge.png");
            BMS_Long_Hell = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{BMS}Long_Hell.png");
            BMS_Long_Hell_Charge = new Texture($"{DEFAULT}{PlayData.Data.SkinName}{BMS}Long_Hell_Charge.png");
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
            SongSelect_Bar_Color,
            SongSelect_Bar_Cursor,
            SongSelect_Bar_Folder,
            SongSelect_Bar_Folder_Color,
            SongSelect_Clear,
            SongSelect_Difficulty,
            SongSelect_Difficulty_Base,
            SongSelect_Difficulty_Course,
            SongSelect_Difficulty_Cursor,
            SongSelect_Difficulty_Level,
            SongSelect_Difficulty_Number,
            SongSelect_Difficulty_TJA,
            SongSelect_Number;
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
        public static Texture[] Game_Bomb = new Texture[12];
        public static Texture[][] Game_Don = new Texture[5][],
            Game_Ka = new Texture[5][];
        #endregion
        #region BMS
        public static Texture BMS_Background,
            BMS_Lane,
            BMS_Notes,
            BMS_Long,
            BMS_Long_Charge,
            BMS_Long_Hell,
            BMS_Long_Hell_Charge;
        #endregion
        #region Result
        public static Texture Result_Background,
            Result_Panel,
            Result_FastRate,
            Result_Gauge,
            Result_Rank;
        #endregion
    }

    public class Sfx
    {
        public static void Init()
        {
            const string SOUND = @"Sound\";
            //const string BGM = @"BGM\";
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

            for (int i = 0; i < 8; i++) KeySound[i] = new Sound($@"{SOUND}{PlayData.Data.SoundName}\Key.ogg");
        }

        public static Sound Metronome, Metronome_Bar;
        public static Sound[] Don = new Sound[5], Ka = new Sound[5], DON = new Sound[5], KA = new Sound[5], Balloon = new Sound[5], Kusudama = new Sound[5], KeySound = new Sound[8];
    }
}
