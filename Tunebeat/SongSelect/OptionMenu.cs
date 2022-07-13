using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJAParse;
using SeaDrop;

namespace Tunebeat
{
    public class OptionMenu
    {
        #region Function
        public static bool Enable, Is2P;
        public static int Cursor;
        #endregion

        public static void Draw()
        {
            Drawing.Box(80, 640, 280, 360, 0);
            Drawing.Text(100, 660, "OPTION");
            Drawing.Text(200, 660, $"{Cursor}");
            for (int i = 0; i <= (int)EOption.Back; i++)
            {
                Drawing.Text(100, 700 + 30 * i, StrOption(i));
                if (Cursor == i) Drawing.Text(90 + (Is2P ? 20 + Drawing.TextWidth(StrOption(i)) : 0), 700 + 30 * i, Is2P ? "<" : ">", (Is2P ? 0x00ffff : 0xff0000));
            }
        }

        public static string StrOption(int type)
        {
            if (SongData.NowSong.Type == EType.BMScore)
            {
                switch ((EBOption)type)
                {
                    case EBOption.Play2P:
                        return $"プレイ人数  : {(PlayData.Data.IsPlay2P ? "2人" : "1人")}";
                    case EBOption.Auto:
                        return $"オート      : {(PlayData.Data.Auto[Is2P ? 1 : 0] ? "ON" : "OFF")}";
                    case EBOption.Gauge:
                        return $"ゲージ      : {(EGaugeJP)PlayData.Data.GaugeType[Is2P ? 1 : 0]}";
                    case EBOption.GaugeAutoShift:
                        return $"ゲージ遷移  : {(EGaugeAutoShiftJP)PlayData.Data.GaugeAutoShift[Is2P ? 1 : 0]}";
                    case EBOption.Random:
                        return $"ランダム    : {(BMSParse.EOption)PlayData.Data.Option[Is2P ? 1 : 0]}";
                    case EBOption.Reload:
                        return "再読み込み";
                    case EBOption.Back:
                        return "とじる";
                }
            }
            else
            {
                switch ((EOption)type)
                {
                    case EOption.Play2P:
                        return $"プレイ人数  : {(PlayData.Data.IsPlay2P ? "2人" : "1人")}";
                    case EOption.Auto:
                        return $"オート      : {(PlayData.Data.Auto[Is2P ? 1 : 0] ? "ON" : "OFF")}";
                    case EOption.Gauge:
                        return $"ゲージ      : {(EGaugeJP)PlayData.Data.GaugeType[Is2P ? 1 : 0]}";
                    case EOption.GaugeAutoShift:
                        return $"ゲージ遷移  : {(EGaugeAutoShiftJP)PlayData.Data.GaugeAutoShift[Is2P ? 1 : 0]}";
                    case EOption.Random:
                        return $"ランダム    : {(PlayData.Data.Random[Is2P ? 1 : 0] ? "ON" : "OFF")}";
                    case EOption.Mirror:
                        return $"ミラー      : {(PlayData.Data.Mirror[Is2P ? 1 : 0] ? "ON" : "OFF")}";
                    case EOption.Stelth:
                        return $"ステルス    : {(PlayData.Data.Stelth[Is2P ? 1 : 0] ? "ON" : "OFF")}";
                    case EOption.NotesChange:
                        return $"音符変化    : {(EChange)PlayData.Data.NotesChange[Is2P ? 1 : 0]}";
                    case EOption.Reload:
                        return "再読み込み";
                    case EOption.Back:
                        return "とじる";
                }
            }
            return "";
        }

        public static void Update()
        {
            int cur = SongData.NowSong.Type == EType.BMScore ? (int)EBOption.Back : (int)EOption.Back;
            if (Mouse.IsPushed(MouseButton.Left)|| Key.IsPushed(EKey.Esc) || (Cursor == cur && Key.IsPushed(EKey.Enter))) Enable = !Enable;
            if (Key.IsHolding(EKey.Up, 500, 50)) if (Cursor-- <= 0) Cursor = cur;
            if (Key.IsHolding(EKey.Down, 500, 50)) if (Cursor++ >= cur) Cursor = 0;
            if (Key.IsPushed(EKey.Left) || (!PlayData.Data.IsPlay2P && Is2P)) Is2P = false;
            if (Key.IsPushed(EKey.Right) && PlayData.Data.IsPlay2P) Is2P = true;

            if (Key.IsPushed(EKey.Enter))
            {
                if (SongData.NowSong.Type == EType.BMScore)
                {
                    switch ((EBOption)Cursor)
                    {
                        case EBOption.Play2P:
                            PlayData.Data.IsPlay2P = !PlayData.Data.IsPlay2P;
                            break;
                        case EBOption.Auto:
                            PlayData.Data.Auto[Is2P ? 1 : 0] = !PlayData.Data.Auto[Is2P ? 1 : 0];
                            break;
                        case EBOption.Gauge:
                            if (PlayData.Data.GaugeType[Is2P ? 1 : 0]++ >= (int)EGauge.EXHard) PlayData.Data.GaugeType[Is2P ? 1 : 0] = (int)EGauge.Normal;
                            break;
                        case EBOption.GaugeAutoShift:
                            if (PlayData.Data.GaugeAutoShift[Is2P ? 1 : 0]++ >= (int)EGaugeAutoShift.LessBest) PlayData.Data.GaugeAutoShift[Is2P ? 1 : 0] = (int)EGaugeAutoShift.None;
                            break;
                        case EBOption.Random:
                            if (PlayData.Data.Option[Is2P ? 1 : 0]++ >= (int)BMSParse.EOption.AllScrach) PlayData.Data.Option[Is2P ? 1 : 0] = (int)BMSParse.EOption.None;
                            break;
                        case EBOption.Reload:
                            PlayData.Init();
                            SongLoad.Init();
                            Enable = !Enable;
                            break;
                    }
                }
                else
                {
                    switch ((EOption)Cursor)
                    {
                        case EOption.Play2P:
                            PlayData.Data.IsPlay2P = !PlayData.Data.IsPlay2P;
                            break;
                        case EOption.Auto:
                            PlayData.Data.Auto[Is2P ? 1 : 0] = !PlayData.Data.Auto[Is2P ? 1 : 0];
                            break;
                        case EOption.Gauge:
                            if (PlayData.Data.GaugeType[Is2P ? 1 : 0]++ >= (int)EGauge.EXHard) PlayData.Data.GaugeType[Is2P ? 1 : 0] = (int)EGauge.Normal;
                            break;
                        case EOption.GaugeAutoShift:
                            if (PlayData.Data.GaugeAutoShift[Is2P ? 1 : 0]++ >= (int)EGaugeAutoShift.LessBest) PlayData.Data.GaugeAutoShift[Is2P ? 1 : 0] = (int)EGaugeAutoShift.None;
                            break;
                        case EOption.Random:
                            PlayData.Data.Random[Is2P ? 1 : 0] = !PlayData.Data.Random[Is2P ? 1 : 0];
                            break;
                        case EOption.Mirror:
                            PlayData.Data.Mirror[Is2P ? 1 : 0] = !PlayData.Data.Mirror[Is2P ? 1 : 0];
                            break;
                        case EOption.Stelth:
                            PlayData.Data.Stelth[Is2P ? 1 : 0] = !PlayData.Data.Stelth[Is2P ? 1 : 0];
                            break;
                        case EOption.NotesChange:
                            if (PlayData.Data.NotesChange[Is2P ? 1 : 0]++ >= (int)EChange.AllBlue) PlayData.Data.NotesChange[Is2P ? 1 : 0] = (int)EChange.None;
                            break;
                        case EOption.Reload:
                            PlayData.Init();
                            SongLoad.Init();
                            Enable = !Enable;
                            break;
                    }
                }
                PlayData.End();
            }
        }

        public enum EOption
        {
            Play2P,
            Auto,
            Gauge,
            GaugeAutoShift,
            Random,
            Mirror,
            Stelth,
            NotesChange,
            Reload,
            Back
        }
        public enum EBOption
        {
            Play2P,
            Auto,
            Gauge,
            GaugeAutoShift,
            Random,
            Reload,
            Back
        }

        public enum EGaugeJP
        {
            ノーマル,
            Aイージー,
            イージー,
            ハード,
            EXハード
        }

        public enum EGaugeAutoShiftJP
        {
            終了,
            続行,
            リトライ,
            N移行,
            ベスト,
            Lベスト
        }
    }
}
