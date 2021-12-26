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
            SetOutApplicationLogValidFlag(FALSE); //ログ出力するか
            ChangeWindowMode(TRUE);  //ウィンドウモード切替
            SetGraphMode(1920, 1080, 32); //ゲームサイズ決める
            SetWindowSize(1280, 720); //ウィンドウサイズを決める
            SetMainWindowText("Tunebeat"); //ソフト名決める
            SetWindowStyleMode(7); //画面最大化できるようにする
            SetWindowSizeChangeEnableFlag(TRUE); //ウィンドウサイズ変えれるようにする
            SetAlwaysRunFlag(TRUE); //ソフトがアクティブじゃなくても処理続行するようにする
            SetWindowSizeExtendRate(1f); //起動時のウィンドウサイズを設定 ( 1 = 100%)
            SetUseMaskScreenFlag(TRUE); //書かなくても良い。マスク使うときだけ書こう
            SetWaitVSyncFlag(TRUE); //垂直同期にするかどうか
            SetDoubleStartValidFlag(TRUE); //複数起動ができるようにするかどうか
            SetMultiThreadFlag(TRUE); //マルチスレッドで処理できるようにするかどうか
            SetUseASyncLoadFlag(TRUE); //非同期で画像を読み込むかどうか
            SetBackgroundColor(0, 0, 0); //バックグラウンドの色指定
            if (DxLib_Init() < 0) return;
            SetDrawScreen(DX_SCREEN_BACK);

            //データ読み込み
            PlayData.Init();
            //画像読み込み
            TextureLoad.Init();
            //音源読み込み
            SoundLoad.Init();

            SceneChange(new SongSelect.SongSelect());
        }

        static void Update()
        {
            while(ProcessMessage() == 0 && ScreenFlip() == 0 && ClearDrawScreen() == 0)
            {
                Key.Update();
                Mouse.Update();
                NowScene?.Draw();
                NowScene?.Update();
            }
        }

        public static void End()
        {
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
    }
}
