using System;
using System.Collections.Generic;
using System.Text;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏スクロール速度 : CActivity
	{
		// プロパティ

		public STDGBVALUE<double> db現在の譜面スクロール速度;


		// コンストラクタ

		public CAct演奏スクロール速度()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装

		public override void On活性化()
		{
			for( int i = 0; i < 3; i++ )
			{
				this.db現在の譜面スクロール速度[ i ] = (double) TJAPlayer3.ConfigIni.n譜面スクロール速度[ i ];
				this.n速度変更制御タイマ[ i ] = -1;
			}
			base.On活性化();
		}
		public override unsafe int On進行描画()
		{
			if( !base.b活性化してない )
			{
				if( base.b初めての進行描画 )
				{
					this.n速度変更制御タイマ.Drums = this.n速度変更制御タイマ.Guitar = this.n速度変更制御タイマ.Bass = (long)(TJAPlayer3.stage演奏ドラム画面.ct演奏用タイマー.db現在の値 * (((double)TJAPlayer3.ConfigIni.n演奏速度) / 20.0));
				}
				long n現在時刻 = (long)TJAPlayer3.stage演奏ドラム画面.ct演奏用タイマー.db現在の値;
				for( int i = 0; i < 3; i++ )
				{
					double db譜面スクロールスピード = (double) TJAPlayer3.ConfigIni.n譜面スクロール速度[ i ];

					if (n現在時刻 < this.nスクロールタイマ) // 念のため
						this.nスクロールタイマ = n現在時刻;

					const int nアニメ間隔 = 2;
					while ((n現在時刻 - this.nスクロールタイマ) >= nアニメ間隔)
					{
						if ( this.db現在の譜面スクロール速度[ i ] < db譜面スクロールスピード )				// Config.iniのスクロール速度を変えると、それに追いつくように実画面のスクロール速度を変える
						{
							this.db現在の譜面スクロール速度[ i ] += 0.012;

							if( this.db現在の譜面スクロール速度[ i ] > db譜面スクロールスピード )
							{
								this.db現在の譜面スクロール速度[ i ] = db譜面スクロールスピード;
							}
						}
						else if ( this.db現在の譜面スクロール速度[ i ] > db譜面スクロールスピード )
						{
							this.db現在の譜面スクロール速度[ i ] -= 0.012;

							if( this.db現在の譜面スクロール速度[ i ] < db譜面スクロールスピード )
							{
								this.db現在の譜面スクロール速度[ i ] = db譜面スクロールスピード;
							}
						}
						this.n速度変更制御タイマ[ i ] -= 20;
						this.nスクロールタイマ += nアニメ間隔;
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private long nスクロールタイマ;
		private STDGBVALUE<double> n速度変更制御タイマ;
		//-----------------
		#endregion
	}
}
