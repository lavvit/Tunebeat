using System;
using System.IO;
using static DxLibDLL.DX;
using SeaDrop;

namespace Tunebeat
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();
            Update();
            End();
        }

        static void Init()
        {
            //設定読み込み
            PlayData.Init();

            #region Discordの処理
            //Discord.Initialize("895240560552591400");
            //StartupTime = Discord.GetUnixTime();
            //Discord.UpdatePresence("", Properties.Discord.Title, StartupTime);
            #endregion

            SetOutApplicationLogValidFlag(FALSE); //ログ出力するか
            ChangeWindowMode(PlayData.Data.FullScreen ? FALSE : TRUE);  //ウィンドウモード切替
            SetGraphMode(1920, 1080, 32); //ゲームサイズ決める
            SetWindowSize(PlayData.Data.Size.Item1, PlayData.Data.Size.Item2); //ウィンドウサイズを決める
            Version = "";//AssemblyInfo?そんな回りくどいことはせんよ
            #if DEBUG
            #else
            Version = "  Ver.Pre 1.0";
            #endif
            SetMainWindowText("Tunebeat" + Version); //ソフト名決める
            SetWindowStyleMode(7); //画面最大化できるようにする
            SetWindowSizeChangeEnableFlag(TRUE); //ウィンドウサイズ変えれるようにする
            SetAlwaysRunFlag(TRUE); //ソフトがアクティブじゃなくても処理続行するようにする
            SetWindowSizeExtendRate(1.0f); //起動時のウィンドウサイズを設定 ( 1 = 100%)
            SetUseMaskScreenFlag(TRUE); //書かなくても良い。マスク使うときだけ書こう
            SetWaitVSyncFlag(PlayData.Data.VSync ? TRUE : FALSE); //垂直同期にするかどうか
            SetDoubleStartValidFlag(TRUE); //複数起動ができるようにするかどうか
            SetMultiThreadFlag(TRUE); //マルチスレッドで処理できるようにするかどうか
            SetUseASyncLoadFlag(TRUE); //非同期で画像を読み込むかどうか
            SetBackgroundColor(0, 0, 0); //バックグラウンドの色指定
            SetDragFileValidFlag(TRUE); //ファイルをD&Dで入れられるようにする
            if (DxLib_Init() < 0) return;
            SetDrawScreen(DX_SCREEN_BACK);

            ChangeFont(PlayData.Data.FontName);

            //画像音源読み込み
            Resourse.Init();

            SeaDrop.SeaDrop.Init();

            SceneChange(new Title());
            FPS = new FPSCount();
        }

        static void Update()
        {
            while(ProcessMessage() == 0 && ScreenFlip() == 0 && ClearDrawScreen() == 0)
            {
                NowScene?.Draw();
                NowScene?.Update();
                SeaDrop.SeaDrop.Update();

                if (PlayData.Data.FullScreen) Cursor = true;
                if (Cursor) DrawCircle(Mouse.X, Mouse.Y, 4, Mouse.IsPushing(MouseButton.Left)? (uint)0xffff00 : 0xff0000);

                if (Key.IsPushed(PlayData.Data.ScreenShot))
                {
                    DateTime time = DateTime.Now;
                    strTime = $"{time.Year:0000}{time.Month:00}{time.Day:00}{time.Hour:00}{time.Minute:00}{time.Second:00}";
                    if (!Directory.Exists("Capture")) Directory.CreateDirectory("Capture");
                    SaveDrawScreenToPNG(0, 0, 1920, 1080, $@"Capture\{strTime}.png");
                    TextLog.Draw($"スクリーンショットが保存されました! : {strTime}.png", 2000);
                }

#if DEBUG
                FPS.Update();
                Drawing.Text(1880, 0, $"{FPS.FPS}", 0x00ff00);
#endif
            }
        }

        public static void End()
        {
            //設定の保存
            PlayData.End();

            //Discord.Shutdown();
            DxLib_End();            
        }

        public static void SceneChange(Scene scene)
        {
            GC.Collect();
            scene?.Enable();
            OldScene = NowScene;
            OldScene?.Disable();
            NowScene = scene;
        }

        public static bool Cursor;
        public static Scene NowScene, OldScene;
        public static string strTime, Version;
        public static FPSCount FPS;
    }
}
