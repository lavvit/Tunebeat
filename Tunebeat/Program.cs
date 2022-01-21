using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            SetMainWindowText("Tunebeat"); //ソフト名決める
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

            //画像読み込み
            TextureLoad.Init();
            //音源読み込み
            SoundLoad.Init();

            ScreenShot = new Counter(0, 2000, 1000, false);

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

                if (Key.IsPushed(KEY_INPUT_F12))
                {
                    DateTime time = DateTime.Now;
                    strTime = $"{time.Year:0000}{time.Month:00}{time.Day:00}{time.Hour:00}{time.Minute:00}{time.Second:00}";
                    SaveDrawScreenToPNG(0, 0, 1920, 1080, $@"Capture\{strTime}.png");
                    ScreenShot.Reset();
                    ScreenShot.Start();
                }
                ScreenShot.Tick();
                if (ScreenShot.Value == ScreenShot.End) ScreenShot.Stop();
                if (ScreenShot.State != 0)
                {
                    DrawBox(0, 1040, 480, 1080, 0x000000, TRUE);
                    DrawString(20, 1052, $"ScreenShot has been Saved! : {strTime}.png", 0xffffff);
                }
            }
        }

        public static void End()
        {
            //設定の保存
            PlayData.End();
            ScreenShot.Reset();

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
        public static Counter ScreenShot;
        public static string strTime;
    }
}
