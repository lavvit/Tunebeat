namespace SeaDrop
{
    /// <summary>
    /// FPSを計測するクラス。
    /// </summary>
    public class FPSCount
    {
        /// <summary>
        /// FPSを計測するクラス。
        /// </summary>
        public FPSCount()
        {
            NowFPS = 0;
            FPS = 0;
            Counter = new Counter(0, 999, 1000, true);
            Counter.Looped += Counter_Looped;
        }

        private void Counter_Looped(object sender, System.EventArgs e)
        {
            // 一秒経過
            FPS = NowFPS;
            NowFPS = 0;
        }

        /// <summary>
        /// FPSカウンターを更新します。
        /// </summary>
        public void Update()
        {
            if (Counter.State == TimerState.Stopped)
            {
                Counter.Start();
            }

            Counter.Tick();
            NowFPS++;
        }

        /// <summary>
        /// 現在のFPS。
        /// </summary>
        public int FPS { get; private set; }

        private int NowFPS;
        private readonly Counter Counter;
    }
}