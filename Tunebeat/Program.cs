using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static DxLibDLL.DX;
using Amaoto;
using Tunebeat.Common;

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

            SetOutApplicationLogValidFlag(FALSE); //ログ出力するか
            ChangeWindowMode(PlayData.Data.FullScreen ? FALSE : TRUE);  //ウィンドウモード切替
            SetGraphMode(1920, 1080, 32); //ゲームサイズ決める
            SetWindowSize(1920, 1080); //ウィンドウサイズを決める
            string Version = "";//AssemblyInfo?そんな回りくどいことはせんよ
            #if DEBUG
            #else
            Version = "  Ver.Beta 0.3";
            #endif
            SetMainWindowText("Tunebeat" + Version); //ソフト名決める
            SetWindowStyleMode(7); //画面最大化できるようにする
            SetWindowSizeChangeEnableFlag(TRUE); //ウィンドウサイズ変えれるようにする
            SetAlwaysRunFlag(TRUE); //ソフトがアクティブじゃなくても処理続行するようにする
            SetWindowSizeExtendRate(0.75f); //起動時のウィンドウサイズを設定 ( 1 = 100%)
            SetUseMaskScreenFlag(TRUE); //書かなくても良い。マスク使うときだけ書こう
            SetWaitVSyncFlag(TRUE); //垂直同期にするかどうか
            SetDoubleStartValidFlag(TRUE); //複数起動ができるようにするかどうか
            SetMultiThreadFlag(TRUE); //マルチスレッドで処理できるようにするかどうか
            SetUseASyncLoadFlag(TRUE); //非同期で画像を読み込むかどうか
            SetBackgroundColor(0, 0, 0); //バックグラウンドの色指定
            if (DxLib_Init() < 0) return;
            SetDrawScreen(DX_SCREEN_BACK);

            //ChangeFont("Arial");

            //画像読み込み
            TextureLoad.Init();
            //音源読み込み
            SoundLoad.Init();

            DrawLog.Init();

            SceneChange(new Title.Title());
        }

        static void Update()
        {
            while(ProcessMessage() == 0 && ScreenFlip() == 0 && ClearDrawScreen() == 0)
            {
                Key.Update();
                Mouse.Update();
                NowScene?.Draw();
                NowScene?.Update();
                DrawLog.Update();

                if (PlayData.Data.FullScreen) DrawCircle(Mouse.X, Mouse.Y, 4, Mouse.IsPushing(MouseButton.Left)? (uint)0xffff00 : 0xff0000);

                if (Key.IsPushed(PlayData.Data.ScreenShot))
                {
                    DateTime time = DateTime.Now;
                    strTime = $"{time.Year:0000}{time.Month:00}{time.Day:00}{time.Hour:00}{time.Minute:00}{time.Second:00}";
                    if (!Directory.Exists("Capture")) Directory.CreateDirectory("Capture");
                    SaveDrawScreenToPNG(0, 0, 1920, 1080, $@"Capture\{strTime}.png");
                    DrawLog.Draw($"スクリーンショットが保存されました! : {strTime}.png", 2000);
                }
            }
        }

        public static void End()
        {
            //設定の保存
            PlayData.End();

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

        public static Scene NowScene, OldScene;
        public static string strTime;
    }
}
