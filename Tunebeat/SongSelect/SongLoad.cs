using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TJAParse;
using Amaoto;
using Tunebeat.Common;

namespace Tunebeat.SongSelect
{
    public class SongLoad
    {
        public static void Init()
        {
            SongData = new List<SongData>();
            Load(SongData, PlayData.Data.PlayFolder);
        }
        public static void Dispose()
        {
            SongData = null;
        }
        public static void Load(List<SongData> data, List<string> allpath)
        {
            foreach (string path in allpath)
            {
                foreach (string item in Directory.EnumerateFiles(path, "*.tja")) //root
                {
                    TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
                    SongData songdata = new SongData()
                    {
                        Path = item,
                        Time = File.GetLastWriteTime(item),
                        Header = parse.Header,
                        Course = parse.Courses,
                        Type = EType.Score,
                    };
                    data.Add(songdata);
                }

                foreach (string folder in Directory.EnumerateDirectories(path, "*")) //folder
                {
                    foreach (string item in Directory.EnumerateFiles(folder, "*.tja"))
                    {
                        TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
                        SongData songdata = new SongData()
                        {
                            Path = item,
                            Time = File.GetLastWriteTime(item),
                            Header = parse.Header,
                            Course = parse.Courses,
                            Type = EType.Score
                        };
                        data.Add(songdata);
                    }
                }
                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Prev = data[(i + (data.Count - 1)) % data.Count];
                    data[i].Next = data[(i + 1) % data.Count];
                }
            }
        }

        public static List<SongData> SongData { get; set; }
    }

    public class SongData
    {
        public string Path;
        public DateTime Time;
        public Header Header;
        public Course[] Course = new Course[5];
        public EType Type;
        public SongData Prev;
        public SongData Next;
    }

    public enum EType
    {
        Score,
        Folder,
        Back,
        Random
    }
}
