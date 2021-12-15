using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Threading;
using DxLibDLL;


namespace FDK
{
    public class CSound
    {
        /// <summary>
        /// サウンドを生成します。
        /// </summary>
        public CSound(string fileName)
        {
            Handle = DX.LoadSoundMem(fileName);
            FileName = fileName;
            Volume = 1.0f;
        }
        public void t解放する()
        {
            DX.DeleteSoundMem(Handle);
            Handle = -1;
        }

        public void t再生を開始する(long time = 0)
        {
            if (IsPlaying) Stop();

            DX.SetSoundCurrentTime((int)time, Handle);
            DX.PlaySoundMem(Handle, DX.DX_PLAYTYPE_BACK, DX.FALSE);
        }

        public void Stop()
        {
            DX.StopSoundMem(Handle);
        }

        public string FileName;
        public int Handle;
        public bool IsPlaying
        {
            get {return DX.CheckSoundMem(Handle) != 0; }
        }
        public int Pan
        {
            get
            {
                return _pan;
            }
            set
            {
                _pan = value;
                DX.ChangePanSoundMem(value, Handle);
            }
        }
        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = (int)(value * 255f);
                DX.ChangeVolumeSoundMem(_volume, Handle);
            }
        }
        public double Time
        {
            get
            {
                return DX.GetSoundCurrentTime(Handle);
            }
            set
            {
                DX.SetSoundCurrentTime((int)value, Handle);
            }
        }
        /// <summary>
        /// Amaotoからイン用
        /// </summary>
        public double PlaySpeed
        {
            get
            {
                return _ratio;
            }
            set
            {
                _ratio = value;
                DX.ResetFrequencySoundMem(Handle);
                var freq = DX.GetFrequencySoundMem(Handle);
                var speed = value * freq;
                DX.SetFrequencySoundMem((int)speed, Handle);
            }
        }
        private int _pan;
        private int _volume;
        private double _ratio;
    }
}