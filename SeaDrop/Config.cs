using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SeaDrop
{
    /// <summary>
    /// 設定ファイル入出力クラス。(Json形式)
    /// </summary>
    public static class ConfigJson
    {
        private static readonly JsonSerializerSettings Settings =
            new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.Include,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new StringEnumConverter[] { new StringEnumConverter() }
            };

        /// <summary>
        /// 設定ファイルの読み込みを行います。ファイルが存在しなかった場合、そのクラスの新規インスタンスを返します。
        /// </summary>
        /// <typeparam name="T">シリアライズしたクラス。</typeparam>
        /// <param name="filePath">ファイル名。</param>
        /// <returns>デシリアライズ結果。</returns>
        public static T GetConfig<T>(string filePath) where T : new()
        {
            var json = "";
            if (!File.Exists(filePath))
            {
                // ファイルが存在しないので
                return new T();
            }
            using (var stream = new StreamReader(filePath, Encoding.UTF8))
            {
                json = stream.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        /// <summary>
        /// 設定ファイルの書き込みを行います。
        /// </summary>
        /// <param name="obj">シリアライズするインスタンス。</param>
        /// <param name="filePath">ファイル名。</param>
        public static void SaveConfig(object obj, string filePath)
        {
            using (var stream = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                stream.Write(JsonConvert.SerializeObject(obj, Formatting.Indented, Settings));
            }
        }
    }

    /// <summary>
    /// ファイル入出力クラス。(テキスト)
    /// </summary>
    public class ConfigIni
    {
        public ConfigIni()
        {
            TextLine = null;
            TextList = new List<string>();
        }

        /// <summary>
        /// テキストを追加します。(改行しない)
        /// </summary>
        /// <param name="text">テキスト</param>
        public void AddText(string text)
        {
            TextLine += text;
        }
        /// <summary>
        /// テキストを追加し改行します。
        /// </summary>
        /// <param name="text">テキスト</param>
        public void AddLine(string text)
        {
            TextLine += text;
            TextList.Add(TextLine);
            TextLine = null;
        }
        /// <summary>
        /// テキストを複数追加します。
        /// </summary>
        /// <param name="text">テキスト</param>
        public void AddList(string[] text)
        {
            TextList.AddRange(text);
        }
        /// <summary>
        /// テキストを複数追加します。
        /// </summary>
        /// <param name="text">テキスト</param>
        public void AddList(List<string> text)
        {
            TextList.AddRange(text);
        }

        /// <summary>
        /// ファイルの読み込みを行います。
        /// </summary>
        /// <param name="path">ファイル名</param>
        /// <param name="trim">空白を削る</param>
        /// <param name="code">文字コード</param>
        public void Load(string path, bool trim = true, string code = "Shift_JIS")
        {
            if (!File.Exists(path)) return;
            string strAll;
            using (StreamReader reader = new StreamReader(path, Encoding.GetEncoding(code)))
            {
                strAll = reader.ReadToEnd();
            }
            string[] delimiter = { "\n" };
            string[] strSingleLine = strAll.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in strSingleLine)
            {
                TextList.Add(trim ? s.Trim() : s);
            }
        }
        /// <summary>
        /// 設定ファイルの書き込みを行います。
        /// </summary>
        /// <param name="path">ファイル名</param>
        /// <param name="path">ファイル名</param>
        /// <param name="code">文字コード</param>
        public void SaveConfig(string path, string code = "Shift_JIS")
        {
            using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.GetEncoding(code)))
            {
                foreach (string str in TextList)
                {
                    streamWriter.WriteLine(str);
                }
            }
        }

        public string TextLine;
        public List<string> TextList;
    }
}