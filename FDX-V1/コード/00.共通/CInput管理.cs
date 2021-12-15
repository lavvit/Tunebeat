using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;

namespace FDK
{
    public class CInput管理
    {
        /// <summary>
        /// 入力を管理するクラス。
        /// </summary>
        public CInput管理()
        {
            Keys = new byte[256];
            Buffer = new byte[256];
            Count = new int[256];
            Time = new (double, double, double)[256];
        }

        /// <summary>
        /// キー入力の状態をチェックします。毎フレーム呼び出す必要があります。
        /// </summary>
        public void Update()
        {

            DX.GetHitKeyStateAll(Buffer);
            for (int i = 0; i < 256; i++)
            {
                Time[i].time = DX.GetNowCount() - Time[i].start;

                if (Buffer[i] == 1)
                {
                    if (Keys[i] == 0) Keys[i] = 1;
                    else if (Keys[i] == 1) Keys[i] = 2;
                }
                else
                {
                    Keys[i] = 0;
                }
            }
        }

        /// <summary>
        /// そのキーが入力されたかどうかチェックします。
        /// </summary>
        /// <param name="key">キーコード。</param>
        /// <returns>入力されたかどうか。</returns>
        public bool IsPushedKey(int key)
        {
            return Keys[key] == 1;
        }

        /// <summary>
        /// そのキーが入力されているかどうかチェックします。
        /// </summary>
        /// <param name="key">キーコード。</param>
        /// <returns>入力されているかどうか。</returns>
        public bool IsPushingKey(int key)
        {
            return Keys[key] > 0;
        }

        /// <summary>
        /// そのキーを長押しした際にどう信号を送るか
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsLongPress(int key, double start_intert進行Loopal, double intert進行Loopal)
        {
            if (Keys[key] > 0)
            {
                if (Time[key].start == -1)
                {
                    switch (Count[key])
                    {
                        case 0:
                            Time[key].end = start_intert進行Loopal;
                            Time[key].start = DX.GetNowCount();
                            Count[key]++;
                            return true;
                        default:
                            Time[key].end = intert進行Loopal;
                            Time[key].start = DX.GetNowCount();
                            Count[key]++;
                            return true;
                    }
                }
                if (Time[key].time >= Time[key].end) Time[key].start = -1;
            }
            else { Time[key].start = -1; Count[key] = 0; }

            return false;
        }

        private readonly byte[] Keys;
        private readonly byte[] Buffer;
        private readonly int[] Count;
        private readonly (double start, double time, double end)[] Time;
    }
}
