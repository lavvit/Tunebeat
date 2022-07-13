using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaDrop;
using TJAParse;

namespace Tunebeat
{
    public class DanMenu
    {
        #region Function
        public static bool Enable, InLayer, ListSelected, Selecting;
        public static int Cursor, Layer, Listnum;
        public static DanOrder Order = PlayData.Data.DanSetting;
        #endregion

        public static void Draw()
        {
            Drawing.Box(80, 640, 400, 360, 0);
            Drawing.Text(100, 660, "DAN SETTING");
            Drawing.Text(240, 660, $"{Cursor}");
            Drawing.Text(90, 700 + 30 * Cursor, ">", 0xff0000);
            for (int i = 0; i <= (int)EDan.Enter; i++)
            {
                Drawing.Text(100, 700 + 30 * i, $"{(EDan)i}");
            }
            if (InLayer)
            {
                Drawing.Text(170, 700 + 30 * Layer, ">", 0xff0000);
                for (int i = 0; i <= (int)EDan.Enter; i++)
                {
                    Drawing.Text(100, 700 + 30 * i, $"{(EDan)i}");
                }
            }
            switch ((EDan)Cursor)
            {
                case EDan.Song:
                    for (int i = 0; i < Order.Songs.Count; i++)
                    {
                        if (ListSelected) Drawing.Text(180, 700 + 30 * i, $"{i}:{(ECourse)Order.Songs[i].Course}, LV.{Order.Songs[i].Level.Item1}~{Order.Songs[i].Level.Item2}, {Order.Songs[i].Combo.Item1}~{Order.Songs[i].Combo.Item2}Combo");
                        else Drawing.Text(180, 700 + 30 * i, $"{i}:{(ECourse)Order.Songs[i].Course}" +
                            $"{(Order.Songs[i].Level != (1, 22) ? $", LV.{Order.Songs[i].Level.Item1}~{Order.Songs[i].Level.Item2}" : "")}{(Order.Songs[i].Combo != (1, 99999) ? $", {Order.Songs[i].Combo.Item1}~{Order.Songs[i].Combo.Item2}Combo" : "")}");
                    }
                    Drawing.Text(180, 700 + 30 * Order.Songs.Count, "Add");
                    break;
                case EDan.Gauge:
                    //lay = 4;
                    break;
                case EDan.Exam:
                    for (int i = 0; i < Order.Exams.Count; i++)
                    {
                        Drawing.Text(180, 700 + 30 * i, $"{i}");
                    }
                    Drawing.Text(180, 700 + 30 * Order.Exams.Count, "Add");
                    break;
            }
        }

        public static void Update()
        {
            if (Selecting)
            {
                if (Mouse.IsPushed(MouseButton.Left) || Key.IsPushed(EKey.Esc)) Selecting = !Selecting;
            }
            if (ListSelected)
            {
                if (Mouse.IsPushed(MouseButton.Left) || Key.IsPushed(EKey.Esc)) ListSelected = !ListSelected;
            }
            else if (InLayer)
            {
                if (Mouse.IsPushed(MouseButton.Left) || Key.IsPushed(EKey.Esc)) InLayer = !InLayer;
                int lay = 0;
                switch ((EDan)Cursor)
                {
                    case EDan.Song:
                        lay = 3;
                        break;
                    case EDan.Exam:
                        lay = Order.Exams.Count;
                        break;
                }
                if (Key.IsHolding(EKey.Left, 500, 50)) if (Listnum-- <= 0) Listnum = lay;
                if (Key.IsHolding(EKey.Right, 500, 50)) if (Listnum++ >= lay) Listnum = 0;
                if (Key.IsPushed(EKey.Enter))
                {
                    Selecting = !Selecting;
                }
            }
            else
            {
                if (Mouse.IsPushed(MouseButton.Left) || Key.IsPushed(EKey.Esc)) Enable = !Enable;
                if (Key.IsHolding(EKey.Up, 500, 50)) if (Cursor-- <= 0) Cursor = (int)EDan.Enter;
                if (Key.IsHolding(EKey.Down, 500, 50)) if (Cursor++ >= (int)EDan.Enter) Cursor = 0;
                if (Key.IsPushed(EKey.Enter))
                {
                    if (Cursor == (int)EDan.Enter)
                    {
                        Enable = !Enable;
                        SongSelect.DanEnter();
                    }

                    else InLayer = !InLayer;
                }
            }
        }
    }

    public class SongOrder
    {
        public int Course = PlayData.Data.PlayCourse[0];
        public (int, int) Level = (1, 22);//Min,Max
        public (int, int) Combo = (1, 99999);
    }
    public class DanOrder
    {
        public List<SongOrder> Songs = new List<SongOrder>();
        public GaugeExam Gauge = new GaugeExam();
        public List<Exam> Exams = new List<Exam>();
    }

    public enum EDan
    {
        Song,
        Gauge,
        Exam,
        Enter
    }
}
