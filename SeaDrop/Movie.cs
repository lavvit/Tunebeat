using DxLibDLL;

namespace SeaDrop
{
    /// <summary>
    /// 動画再生クラス。
    /// </summary>
    public class Movie : Texture, IPlayable
    {
        /// <summary>
        /// 動画ファイルのオープンを行います。
        /// </summary>
        /// <param name="fileName">ファイル名。</param>
        public Movie(string fileName)
            : base(fileName)
        {
            Volume = 1.0;
        }

        /// <summary>
        /// 映像のみ再生をループモードで開始します。
        /// </summary>
        /// <param name="startTime">再生開始時間</param>
        /// <param name="truetime">等倍での時間か</param>
        public void PlayGraphLoop(double startTime, bool truetime = false)
        {
            if (IsEnable && !IsPlaying)
            {
                if (truetime) TrueTime = startTime;
                else Time = startTime;
                Volume = 0;
                Play(false);
            }
        }
        /// <summary>
        /// 映像のみ再生をループモードで開始します。
        /// </summary>
        public void PlayGraphLoop(bool playFromBegin = true)
        {
            if (IsEnable && !IsPlaying)
            {
                Play(playFromBegin);
                Volume = 0;
            }
        }
        /// <summary>
        /// 映像のみ再生を開始します。
        /// </summary>
        /// <param name="startTime">再生開始時間</param>
        /// <param name="truetime">等倍での時間か</param>
        public void PlayGraph(double startTime, bool truetime = false)
        {
            if (IsEnable)
            {
                if (truetime) TrueTime = startTime;
                else Time = startTime;
                Volume = 0;
                Play(false);
            }
        }
        /// <summary>
        /// 映像のみ再生を開始します。
        /// </summary>
        public void PlayGraph(bool playFromBegin = true)
        {
            if (IsEnable)
            {
                Play(playFromBegin);
                Volume = 0;
            }
        }
        /// <summary>
        /// 再生をループモードで開始します。
        /// </summary>
        /// <param name="startTime">再生開始時間</param>
        /// <param name="truetime">等倍での時間か</param>
        public void PlayLoop(double startTime, bool truetime = false)
        {
            if (IsEnable && !IsPlaying)
            {
                if (truetime) TrueTime = startTime;
                else Time = startTime;
                Play(false);
            }
        }
        /// <summary>
        /// 再生をループモードで開始します。
        /// </summary>
        public void PlayLoop(bool playFromBegin = true)
        {
            if (IsEnable && !IsPlaying)
            {
                Play(playFromBegin);
            }
        }
        /// <summary>
        /// 再生を開始します。
        /// </summary>
        /// <param name="startTime">再生開始時間</param>
        /// <param name="truetime">等倍での時間か</param>
        public void Play(double startTime, bool truetime = false)
        {
            if (IsEnable)
            {
                if (truetime) TrueTime = startTime;
                else Time = startTime;
                Play(false);
            }
        }
        /// <summary>
        /// 再生を開始します。
        /// </summary>
        public void Play(bool playFromBegin = true)
        {
            if (playFromBegin)
            {
                Time = 0;
            }
            DX.PlayMovieToGraph(ID);
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop()
        {
            DX.PauseMovieToGraph(ID);
        }

        /// <summary>
        /// 動画の音量。
        /// </summary>
        public double Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = (int)(value * 255);
                DX.ChangeMovieVolumeToGraph(_volume, ID);
            }
        }

        /// <summary>
        /// 時間。単位はミリ秒。
        /// </summary>
        public double Time
        {
            get
            {
                return (int)(DX.TellMovieToGraph(ID) / PlaySpeed);
            }
            set
            {
                DX.SeekMovieToGraph(ID, (int)(value * PlaySpeed));
            }
        }
        /// <summary>
        /// 再生速度を考慮しない時間。単位はミリ秒。
        /// </summary>
        public double TrueTime
        {
            get
            {
                return DX.TellMovieToGraph(ID);
            }
            set
            {
                DX.SeekMovieToGraph(ID, (int)value);
            }
        }

        /// <summary>
        /// 再生中かどうか。
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return DX.GetMovieStateToGraph(ID) == 1;
            }
        }

        /// <summary>
        /// 再生速度を倍率で変更する。
        /// </summary>
        public double PlaySpeed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
                DX.SetPlaySpeedRateMovieToGraph(ID, value);
            }
        }

        /// <summary>
        /// 長さ。
        /// </summary>
        public double Length
        {
            get
            {
                return DX.GetMovieTotalFrameToGraph(ID);
            }
        }

        private int _volume;
        private double _speed;
    }
}