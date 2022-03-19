namespace TJAParse
{
    public class DanCourse
    {
        public string Title, SubTitle;
        public int Level = 0, TotalNotes;
        public Course[] Courses;
        public GaugeExam Gauge;
        public Exam[] Exams;
    }

    public class Exam
    {
        public EExam[] Name = new EExam[3];
        public int RedNumber;
        public int GoldNumber;
        public int[] RedNumberSong;
        public int[] GoldNumberSong;
        public bool isLess;
    }
    public class GaugeExam
    {
        public int GaugeType;//0:None,1:Hard,2:EXHard
        public double RedNumber;
        public double GoldNumber;
        public double Total;
    }

    public enum EExam
    {
        Perfect,
        Great,
        Good,
        Bad,
        Poor,
        Light,//Perfect+Great
        Miss,//Bad+Poor
        Score,
        Roll,
        Hit,
        Combo
    }
}
