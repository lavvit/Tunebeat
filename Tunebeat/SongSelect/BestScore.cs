using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Amaoto;
using Tunebeat.Common;

namespace Tunebeat.SongSelect
{
    public class BestScore
    {
        public ScoreData ScoreData;
        public BestScore(string path)
        {
            if (string.IsNullOrEmpty(PlayData.Data.PlayerName)) return;

            ScoreData = new ScoreData();
            string scorepath = $@"{Path.GetDirectoryName(path)}\{Path.GetFileNameWithoutExtension(path)}.{PlayData.Data.PlayerName}.tbs";
            string oldscorepath = $@"{path}.{PlayData.Data.PlayerName}.scoredata";
            if (File.Exists(scorepath))
            {
                ScoreData = ConfigManager.GetConfig<ScoreData>(scorepath);
            }
            else if (File.Exists(oldscorepath))
            {
                ScoreData.Score = new Scores[5];
                for (int i = 0; i < 5; i++)
                {
                    ScoreData.Score[i] = new Scores();
                    ScoreData.Score[i].Course = i;
                }
                string str;
                using (StreamReader reader = new StreamReader(oldscorepath, Encoding.GetEncoding("Shift_JIS")))
                {
                    str = reader.ReadToEnd();
                }
                Load(str);
            }
            else
            {
                ScoreData = null;
            }
        }

        public void Load(string strAll)
        {
            string[] delimiter = { "\n" };
            string[] strSingleLine = strAll.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in strSingleLine)
            {
                string str = s.Replace('\t', ' ').TrimStart(new char[] { '\t', ' ' });
                if ((str.Length != 0) && (str[0] != ';'))
                {
                    string str3;
                    string str4;
                    string[] strArray = str.Split(new char[] { '=' });
                    if (strArray.Length == 2)
                    {
                        str3 = strArray[0].Trim();
                        str4 = strArray[1].Trim();
                        if (str3.Equals("Title"))
                        {
                            ScoreData.Title = str4;
                        }
                        #region Crown
                        if (str3.Equals("Crown1"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    ScoreData.Score[0].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    ScoreData.Score[0].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    ScoreData.Score[0].ClearLamp = (int)EClear.Clear;
                                    break;
                                case 0:
                                    ScoreData.Score[0].ClearLamp = (int)EClear.NoPlay;
                                    break;
                            }
                        }
                        if (str3.Equals("Crown2"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    ScoreData.Score[1].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    ScoreData.Score[1].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    ScoreData.Score[1].ClearLamp = (int)EClear.Clear;
                                    break;
                                case 0:
                                    ScoreData.Score[1].ClearLamp = (int)EClear.NoPlay;
                                    break;
                            }
                        }
                        if (str3.Equals("Crown3"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    ScoreData.Score[2].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    ScoreData.Score[2].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    ScoreData.Score[2].ClearLamp = (int)EClear.Clear;
                                    break;
                                case 0:
                                    ScoreData.Score[2].ClearLamp = (int)EClear.NoPlay;
                                    break;
                            }
                        }
                        if (str3.Equals("Crown4"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    ScoreData.Score[3].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    ScoreData.Score[3].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    ScoreData.Score[3].ClearLamp = (int)EClear.Clear;
                                    break;
                                case 0:
                                    ScoreData.Score[3].ClearLamp = (int)EClear.NoPlay;
                                    break;
                            }
                        }
                        if (str3.Equals("Crown5"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    ScoreData.Score[4].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    ScoreData.Score[4].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    ScoreData.Score[4].ClearLamp = (int)EClear.Clear;
                                    break;
                                case 0:
                                    ScoreData.Score[4].ClearLamp = (int)EClear.NoPlay;
                                    break;
                            }
                        }
                        #endregion
                        #region CrownHard
                        if (str3.Equals($"CrownHard1"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.HardClear)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.HardClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownHard2"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.HardClear)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.HardClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownHard3"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.HardClear)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.HardClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownHard4"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.HardClear)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.HardClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownHard5"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.HardClear)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.HardClear;
                                    break;
                            }
                        }
                        #endregion
                        #region CrownEXH
                        if (str3.Equals($"CrownEXH1"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.EXHardClear)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.EXHardClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownEXH2"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.EXHardClear)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.EXHardClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownEXH3"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.EXHardClear)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.EXHardClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownEXH4"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.EXHardClear)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.EXHardClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownEXH5"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.EXHardClear)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.EXHardClear;
                                    break;
                            }
                        }
                        #endregion
                        #region CrownEasy
                        if (str3.Equals($"CrownEasy1"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.EasyClear)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.EasyClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownEasy2"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.EasyClear)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.EasyClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownEasy3"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.EasyClear)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.EasyClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownEasy4"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.EasyClear)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.EasyClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownEasy5"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.EasyClear)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.EasyClear;
                                    break;
                            }
                        }
                        #endregion
                        #region CrownAssist
                        if (str3.Equals($"CrownAssist1"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[0].ClearLamp < (int)EClear.AssistClear)
                                        ScoreData.Score[0].ClearLamp = (int)EClear.AssistClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownAssist2"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[1].ClearLamp < (int)EClear.AssistClear)
                                        ScoreData.Score[1].ClearLamp = (int)EClear.AssistClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownAssist3"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[2].ClearLamp < (int)EClear.AssistClear)
                                        ScoreData.Score[2].ClearLamp = (int)EClear.AssistClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownAssist4"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[3].ClearLamp < (int)EClear.AssistClear)
                                        ScoreData.Score[3].ClearLamp = (int)EClear.AssistClear;
                                    break;
                            }
                        }
                        if (str3.Equals($"CrownAssist5"))
                        {
                            switch (int.Parse(str4))
                            {
                                case 3:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.AllGreat)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.AllGreat;
                                    break;
                                case 2:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.FullCombo)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.FullCombo;
                                    break;
                                case 1:
                                    if (ScoreData.Score[4].ClearLamp < (int)EClear.AssistClear)
                                        ScoreData.Score[4].ClearLamp = (int)EClear.AssistClear;
                                    break;
                            }
                        }
                        #endregion
                        #region AllNotes
                        if (str3.Equals($"AllNotes1"))
                        {
                            if (int.Parse(str4) > 0 && ScoreData.Score[0].ClearLamp == (int)EClear.NoPlay)
                                ScoreData.Score[0].ClearLamp = (int)EClear.Failed;
                        }
                        if (str3.Equals($"AllNotes2"))
                        {
                            if (int.Parse(str4) > 0 && ScoreData.Score[1].ClearLamp == (int)EClear.NoPlay)
                                ScoreData.Score[1].ClearLamp = (int)EClear.Failed;
                        }
                        if (str3.Equals($"AllNotes3"))
                        {
                            if (int.Parse(str4) > 0 && ScoreData.Score[2].ClearLamp == (int)EClear.NoPlay)
                                ScoreData.Score[2].ClearLamp = (int)EClear.Failed;
                        }
                        if (str3.Equals($"AllNotes4"))
                        {
                            if (int.Parse(str4) > 0 && ScoreData.Score[3].ClearLamp == (int)EClear.NoPlay)
                                ScoreData.Score[3].ClearLamp = (int)EClear.Failed;
                        }
                        if (str3.Equals($"AllNotes5"))
                        {
                            if (int.Parse(str4) > 0 && ScoreData.Score[4].ClearLamp == (int)EClear.NoPlay)
                                ScoreData.Score[4].ClearLamp = (int)EClear.Failed;
                        }
                        #endregion
                        #region Gage
                        if (str3.Equals($"Gage1"))
                        {
                            ScoreData.Score[0].Gauge = double.Parse(str4);
                        }
                        if (str3.Equals($"Gage2"))
                        {
                            ScoreData.Score[1].Gauge = double.Parse(str4);
                        }
                        if (str3.Equals($"Gage3"))
                        {
                            ScoreData.Score[2].Gauge = double.Parse(str4);
                        }
                        if (str3.Equals($"Gage4"))
                        {
                            ScoreData.Score[3].Gauge = double.Parse(str4);
                        }
                        if (str3.Equals($"Gage5"))
                        {
                            ScoreData.Score[4].Gauge = double.Parse(str4);
                        }
                        #endregion
                        #region GageMode
                        if (str3.Equals($"GageMode1"))
                        {
                            ScoreData.Score[0].GaugeType = int.Parse(str4);
                        }
                        if (str3.Equals($"GageMode2"))
                        {
                            ScoreData.Score[1].GaugeType = int.Parse(str4);
                        }
                        if (str3.Equals($"GageMode3"))
                        {
                            ScoreData.Score[2].GaugeType = int.Parse(str4);
                        }
                        if (str3.Equals($"GageMode4"))
                        {
                            ScoreData.Score[3].GaugeType = int.Parse(str4);
                        }
                        if (str3.Equals($"GageMode5"))
                        {
                            ScoreData.Score[4].GaugeType = int.Parse(str4);
                        }
                        #endregion
                        #region TechnicalPerfect
                        if (str3.Equals($"TechnicalPerfect1"))
                        {
                            ScoreData.Score[0].Perfect = int.Parse(str4);
                        }
                        if (str3.Equals($"TechnicalPerfect2"))
                        {
                            ScoreData.Score[1].Perfect = int.Parse(str4);
                        }
                        if (str3.Equals($"TechnicalPerfect3"))
                        {
                            ScoreData.Score[2].Perfect = int.Parse(str4);
                        }
                        if (str3.Equals($"TechnicalPerfect4"))
                        {
                            ScoreData.Score[3].Perfect = int.Parse(str4);
                        }
                        if (str3.Equals($"TechnicalPerfect5"))
                        {
                            ScoreData.Score[4].Perfect = int.Parse(str4);
                        }
                        #endregion
                        #region Perfect
                        if (str3.Equals($"Perfect1"))
                        {
                            ScoreData.Score[0].Great = int.Parse(str4) - ScoreData.Score[0].Perfect;
                            ScoreData.Score[0].Score = 2 * ScoreData.Score[0].Perfect + ScoreData.Score[0].Great;
                            if (ScoreData.Score[0].Great == 0 && ScoreData.Score[0].ClearLamp == (int)EClear.AllGreat)
                                ScoreData.Score[0].ClearLamp = (int)EClear.AllPerfect;
                        }
                        if (str3.Equals($"Perfect2"))
                        {
                            ScoreData.Score[1].Great = int.Parse(str4) - ScoreData.Score[1].Perfect;
                            ScoreData.Score[1].Score = 2 * ScoreData.Score[1].Perfect + ScoreData.Score[1].Great;
                            if (ScoreData.Score[1].Great == 0 && ScoreData.Score[1].ClearLamp == (int)EClear.AllGreat)
                                ScoreData.Score[1].ClearLamp = (int)EClear.AllPerfect;
                        }
                        if (str3.Equals($"Perfect3"))
                        {
                            ScoreData.Score[2].Great = int.Parse(str4) - ScoreData.Score[2].Perfect;
                            ScoreData.Score[2].Score = 2 * ScoreData.Score[2].Perfect + ScoreData.Score[2].Great;
                            if (ScoreData.Score[2].Great == 0 && ScoreData.Score[2].ClearLamp == (int)EClear.AllGreat)
                                ScoreData.Score[2].ClearLamp = (int)EClear.AllPerfect;
                        }
                        if (str3.Equals($"Perfect4"))
                        {
                            ScoreData.Score[3].Great = int.Parse(str4) - ScoreData.Score[3].Perfect;
                            ScoreData.Score[3].Score = 2 * ScoreData.Score[3].Perfect + ScoreData.Score[3].Great;
                            if (ScoreData.Score[3].Great == 0 && ScoreData.Score[3].ClearLamp == (int)EClear.AllGreat)
                                ScoreData.Score[3].ClearLamp = (int)EClear.AllPerfect;
                        }
                        if (str3.Equals($"Perfect5"))
                        {
                            ScoreData.Score[4].Great = int.Parse(str4) - ScoreData.Score[4].Perfect;
                            ScoreData.Score[4].Score = 2 * ScoreData.Score[4].Perfect + ScoreData.Score[4].Great;
                            if (ScoreData.Score[4].Great == 0 && ScoreData.Score[4].ClearLamp == (int)EClear.AllGreat)
                                ScoreData.Score[4].ClearLamp = (int)EClear.AllPerfect;
                        }
                        #endregion
                        #region Good
                        if (str3.Equals($"Good1"))
                        {
                            ScoreData.Score[0].Good = int.Parse(str4);
                        }
                        if (str3.Equals($"Good2"))
                        {
                            ScoreData.Score[1].Good = int.Parse(str4);
                        }
                        if (str3.Equals($"Good3"))
                        {
                            ScoreData.Score[2].Good = int.Parse(str4);
                        }
                        if (str3.Equals($"Good4"))
                        {
                            ScoreData.Score[3].Good = int.Parse(str4);
                        }
                        if (str3.Equals($"Good5"))
                        {
                            ScoreData.Score[4].Good = int.Parse(str4);
                        }
                        #endregion
                        #region But
                        if (str3.Equals($"But1"))
                        {
                            ScoreData.Score[0].Bad = int.Parse(str4);
                        }
                        if (str3.Equals($"But2"))
                        {
                            ScoreData.Score[1].Bad = int.Parse(str4);
                        }
                        if (str3.Equals($"But3"))
                        {
                            ScoreData.Score[2].Bad = int.Parse(str4);
                        }
                        if (str3.Equals($"But4"))
                        {
                            ScoreData.Score[3].Bad = int.Parse(str4);
                        }
                        if (str3.Equals($"But5"))
                        {
                            ScoreData.Score[4].Bad = int.Parse(str4);
                        }
                        #endregion
                        #region Roll
                        if (str3.Equals($"Roll1"))
                        {
                            ScoreData.Score[0].Roll = int.Parse(str4);
                        }
                        if (str3.Equals($"Roll2"))
                        {
                            ScoreData.Score[1].Roll = int.Parse(str4);
                        }
                        if (str3.Equals($"Roll3"))
                        {
                            ScoreData.Score[2].Roll = int.Parse(str4);
                        }
                        if (str3.Equals($"Roll4"))
                        {
                            ScoreData.Score[3].Roll = int.Parse(str4);
                        }
                        if (str3.Equals($"Roll5"))
                        {
                            ScoreData.Score[4].Roll = int.Parse(str4);
                        }
                        #endregion
                        #region MaxCombo
                        if (str3.Equals($"MaxCombo1"))
                        {
                            ScoreData.Score[0].MaxCombo = int.Parse(str4);
                        }
                        if (str3.Equals($"MaxCombo2"))
                        {
                            ScoreData.Score[1].MaxCombo = int.Parse(str4);
                        }
                        if (str3.Equals($"MaxCombo3"))
                        {
                            ScoreData.Score[2].MaxCombo = int.Parse(str4);
                        }
                        if (str3.Equals($"MaxCombo4"))
                        {
                            ScoreData.Score[3].MaxCombo = int.Parse(str4);
                        }
                        if (str3.Equals($"MaxCombo5"))
                        {
                            ScoreData.Score[4].MaxCombo = int.Parse(str4);
                        }
                        #endregion
                        continue;
                    }
                }
            }
        }
    }

    public class ScoreData
    {
        public string Title;
        public Scores[] Score;
    }

    public class Scores
    {
        public int Course;
        public int PlayCount;
        public string Time;
        public int ClearLamp;
        public int Score;
        public int MaxCombo;
        public int GaugeType;
        public double Gauge;
        public int Perfect;
        public int Great;
        public int Good;
        public int Bad;
        public int Poor;
        public int Roll;
        public string BestScore;
    }

    public enum EClear
    {
        NoPlay,
        Failed,
        AssistClear,
        EasyClear,
        Clear,
        HardClear,
        EXHardClear,
        FullCombo,
        AllGreat,
        AllPerfect
    }
}
