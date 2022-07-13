using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaDrop;
using TJAParse;
using BMSParse;

namespace Tunebeat
{
    public class SongData
    {
        public static List<Song> Song, AllSong, FolderSong;
        public static List<Folder> Folder, AllFolder;
        public static List<string> FolderPath, Ini;
        public static int FolderFloor;

        public static Song NowSong;
        public static TJA[] NowTJA;
        public static BMS[] NowBMS;
    }
}
