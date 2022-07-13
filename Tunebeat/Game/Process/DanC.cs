using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaDrop;
using TJAParse;

namespace Tunebeat
{
    public class DanC
    {
        #region Function
        public static List<Chip> LastChip;
        #endregion
        public static void Draw()
        {
            if (NewGame.Dan == null) return;

            Drawing.Text(0, 500, $"{NewGame.Dan.Title} Lv.{NewGame.Dan.Level} {NewGame.Dan.TotalNotes} Notes");
            for (int i = 0; i < NewScore.DanScore.Length; i++)
            {
                if (DanCourse.SongNumber >= i)
                {
                    Drawing.Text(160 * (i % 5), 520 + 240 * (i / 5), "Song");
                    Drawing.Text(160 * (i % 5) + Drawing.TextWidth("Song "), 520 + 240 * (i / 5), $"{i + 1}/{NewScore.DanScore.Length}", i == NewScore.DanScore.Length - 1 ? 0xff0000 : 0xffffff);
                    Drawing.Text(160 * (i % 5), 540 + 240 * (i / 5), $"{NewGame.Dan.Courses[i].Title}");
                    Drawing.Text(160 * (i % 5), 560 + 240 * (i / 5), $"{(ECourse)NewGame.Dan.Courses[i].Course} Lv.{NewGame.Dan.Courses[i].Level}");
                    Drawing.Text(160 * (i % 5), 580 + 240 * (i / 5), $"SC:{NewScore.DanScore[i].EXScore}");
                    Drawing.Text(160 * (i % 5), 600 + 240 * (i / 5), $"PG:{NewScore.DanScore[i].Perfect}");
                    Drawing.Text(160 * (i % 5), 620 + 240 * (i / 5), $"GR:{NewScore.DanScore[i].Great}");
                    Drawing.Text(160 * (i % 5), 640 + 240 * (i / 5), $"GD:{NewScore.DanScore[i].Good}");
                    Drawing.Text(160 * (i % 5), 660 + 240 * (i / 5), $"BD:{NewScore.DanScore[i].Bad}");
                    Drawing.Text(160 * (i % 5), 680 + 240 * (i / 5), $"PR:{NewScore.DanScore[i].Poor}");
                    Drawing.Text(160 * (i % 5), 700 + 240 * (i / 5), $"AT:{NewScore.DanScore[i].Auto}");
                    Drawing.Text(160 * (i % 5), 720 + 240 * (i / 5), $"RL:{NewScore.DanScore[i].Roll + NewScore.DanScore[i].AutoRoll}{NewScore.DanScore[i].RollDetail}");
                    Drawing.Text(160 * (i % 5), 740 + 240 * (i / 5), $"CB:{NewScore.DanScore[i].MaxCombo}");
                }
            }
            if (NewGame.Dan.Gauge.RedNumber + NewGame.Dan.Gauge.GoldNumber > 0.0)
            {
                Drawing.Text(960, 560, $"{(NewGame.Dan.Gauge.GaugeType == 0 ? EGauge.Normal : (EGauge)NewGame.Dan.Gauge.GaugeType + 2)} : {NewGame.Dan.Gauge.RedNumber}%/{NewGame.Dan.Gauge.GoldNumber}%");
                Drawing.Box(1160, 550, 500, 40, 0x404040);
                double perred = NewScore.Gauge[0] > NewGame.Dan.Gauge.RedNumber ? 1.0 : NewScore.Gauge[0] / NewGame.Dan.Gauge.RedNumber;
                Drawing.Box(1160, 550, 500 * perred, 40, 0xff0000);
                double pergold = NewScore.Gauge[0] > NewGame.Dan.Gauge.GoldNumber ? 1.0 : (NewScore.Gauge[0] < NewGame.Dan.Gauge.RedNumber ? 0.0 : (NewScore.Gauge[0] - NewGame.Dan.Gauge.RedNumber) / (NewGame.Dan.Gauge.GoldNumber - NewGame.Dan.Gauge.RedNumber));
                Drawing.Box(1160, 550, 500 * pergold, 40, 0xffff00);
                NewGame.SmallNumber.Draw(1180, 560, NewScore.Gauge[0], 0);
            }

            for (int i = 0; i < NewGame.Dan.Exams.Count; i++)
            {
                Drawing.Text(960, 660 + 60 * i, $"{NewGame.Dan.Exams[i].Name} : {(NewGame.Dan.Exams[i].RedNumberSong != null ? NewGame.Dan.Exams[i].RedNumberSong[DanCourse.SongNumber] : NewGame.Dan.Exams[i].RedNumber)}/{(NewGame.Dan.Exams[i].GoldNumberSong != null ? NewGame.Dan.Exams[i].GoldNumberSong[DanCourse.SongNumber] : NewGame.Dan.Exams[i].GoldNumber)}{(NewGame.Dan.Exams[i].isLess ? "未満" : "以上")}");
                Drawing.Box(1160, 650 + 60 * i, 500, 40, 0x404040);
                Drawing.Box(1160, 650 + 60 * i, 500 * ExamPercent(NewGame.Dan.Exams[i], false), 40, 0xff0000);
                Drawing.Box(1160, 650 + 60 * i, 500 * ExamPercent(NewGame.Dan.Exams[i], true), 40, 0xffff00);
                ESuccess c = ExamSuccess(NewGame.Dan.Exams[i]);
                if (c == ESuccess.Failed) Drawing.Box(1160, 650 + 60 * i, 500, 40, 0, 128);
                Drawing.Text(1680, 660 + 60 * i, $"{c}", CrearColor(c));
                NewGame.SmallNumber.Draw(1180, 660 + 60 * i, ExamValueV(NewGame.Dan.Exams[i]), 0);
            }

            Drawing.Text(1180, 520, $"{ExamSuccess(NewGame.Dan.Exams)}", CrearColor(ExamSuccess(NewGame.Dan.Exams)));

            Drawing.Text(1800, 560, $"{NotesRemain(false)} : {NotesRemain(true)}");
        }

        public static int NotesRemain(bool isAll)
        {
            ScoreBoard sc = !isAll ? NewScore.DanScore[DanCourse.SongNumber] : NewScore.Scores[0];
            int total = !isAll ? NewGame.NowCourse[0].TotalNotes : NewGame.Dan.TotalNotes;
            return total - (sc.Hit() + sc.BadPoor());
        }

        public static double ExamValue(Exam exam)
        {
            ScoreBoard sc = exam.RedNumberSong != null ? NewScore.DanScore[DanCourse.SongNumber] : NewScore.Scores[0];
            switch (exam.Name)
            {
                case EExam.Perfect:
                default:
                    return sc.Perfect + sc.Auto;
                case EExam.Great:
                    return sc.Great;
                case EExam.Good:
                    return sc.Good;
                case EExam.Bad:
                    return sc.Bad;
                case EExam.Poor:
                    return sc.Poor;
                case EExam.Light:
                    return sc.Perfect + sc.Great + sc.Auto;
                case EExam.Miss:
                    return sc.Bad + sc.Poor;
                case EExam.Score:
                    return sc.EXScore + sc.Auto * 2;
                case EExam.OldScore:
                    return 0;
                case EExam.Roll:
                    return sc.Roll + sc.AutoRoll;
                case EExam.Hit:
                    return sc.Perfect + sc.Great + sc.Good + sc.Roll + sc.Auto + sc.AutoRoll;
                case EExam.Combo:
                    return sc.MaxCombo;
            }
        }
        public static double ExamValueV(Exam exam)
        {
            if (exam.isLess)
            {
                int red = exam.RedNumberSong != null ? exam.RedNumberSong[DanCourse.SongNumber] : exam.RedNumber;
                return red - ExamValue(exam) > 0.0 ? red - ExamValue(exam) : 0;
            }
            else return ExamValue(exam);
        }

        public static double ExamPercent(Exam exam, bool isgold)
        {
            double value = ExamValue(exam);
            int red = exam.RedNumberSong != null ? exam.RedNumberSong[DanCourse.SongNumber] : exam.RedNumber;
            int gold = exam.GoldNumberSong != null ? exam.GoldNumberSong[DanCourse.SongNumber] : exam.GoldNumber;
            if (!isgold)
            {
                if (red == 0) return 1.0;
            }
            else
            {
                if (gold - red == 0) return 1.0;
            }
            if (!exam.isLess)
            {
                if (!isgold)
                {
                    return value > red ? 1.0 : value / red;
                }
                else
                {
                    return value > gold ? 1.0 : (value < red ? 0.0 : (value - red) / (gold - red));
                }
            }
            else
            {
                double r = red - value;
                double g = gold - value;
                if (isgold)
                {
                    return value >= gold ? 0.0 : g / gold;
                }
                else
                {
                    return value >= red ? 0.0 : (value <= gold ? 1.0 : r / (red - gold));
                }
            }
        }

        public static ESuccess ExamSuccess(Exam exam)
        {
            double value = ExamValue(exam);
            int red = exam.RedNumberSong != null ? exam.RedNumberSong[DanCourse.SongNumber] : exam.RedNumber;
            int gold = exam.GoldNumberSong != null ? exam.GoldNumberSong[DanCourse.SongNumber] : exam.GoldNumber;
            int total = exam.RedNumberSong != null ? NewGame.NowCourse[0].TotalNotes : NewGame.Dan.TotalNotes;
            if (!exam.isLess)
            {
                switch (exam.Name)
                {
                    case EExam.Perfect:
                    case EExam.Great:
                    case EExam.Good:
                    case EExam.Bad:
                    case EExam.Poor:
                    case EExam.Light:
                    case EExam.Miss:
                        if (value + NotesRemain(exam.RedNumberSong != null) < red)
                            return ESuccess.Failed;
                        break;
                    case EExam.Score:
                        if (value + (2 * NotesRemain(exam.RedNumberSong != null)) < red) return ESuccess.Failed;
                        break;
                }

                if (value >= gold) return ESuccess.Gold;
                else if (value >= red) return ESuccess.Red;
                else
                {
                    Chip chip = new Chip();
                    List<Chip> list = exam.RedNumberSong != null ? NewGame.Chips[0] : LastChip;
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (list[i].ENote > ENote.Space)
                        {
                            chip = list[i];
                            break;
                        }
                    }
                    if (NewGame.Timer.Value >= chip.Time && (exam.RedNumberSong != null || DanCourse.SongNumber == NewGame.Dan.Courses.Count - 1)) return ESuccess.Failed;
                }
            }
            else
            {
                if (value < gold) return ESuccess.Gold;
                else if (value < red) return ESuccess.Red;
                else return ESuccess.Failed;
            }
            return ESuccess.None;
        }
        public static ESuccess ExamSuccess(List<Exam> exams)
        {
            ESuccess success = ESuccess.Gold;
            if (NewGame.NowState > EState.None)
            {
                foreach (Exam exam in exams)
                {
                    ESuccess es = exam.Success;
                    if (es != ESuccess.None && es < success) success = es;
                }
                foreach (Exam exam in exams)
                {
                    if (ExamSuccess(exam) != ESuccess.None && ExamSuccess(exam) < success) success = ExamSuccess(exam);
                }
            }
            return success;
        }

        public static void SaveSuccess(Exam exam)
        {
            ESuccess success = ExamSuccess(exam);
            if (exam.RedNumberSong != null)
            {
                if (success < exam.Success) exam.Success = success;
            }
            else exam.Success = success;
        }

        public static int CrearColor(ESuccess clear)
        {
            switch (clear)
            {
                case ESuccess.Failed:
                    return 0x0000ff;
                case ESuccess.Red:
                    return 0xff0000;
                case ESuccess.Gold:
                    return 0xffff00;
                default:
                    return 0xffffff;
            }
        }
    }

    public enum EDanFailedType
    {
        End,
        Retry,
        Song,
        All
    }
}
