using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using DxLibDLL;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏DrumsチップファイアD : CActivity
	{
		// コンストラクタ

		public CAct演奏DrumsチップファイアD()
		{
			base.b活性化してない = true;
		}
		
		
		// メソッド

        /// <summary>
        /// 大音符の花火エフェクト
        /// </summary>
        /// <param name="nLane"></param>
        public virtual void Start( int nLane, int nPlayer )
        {
            nY座標P2 = new int[] { 548, 612, 670, 712, 730, 780, 725, 690, 640 };
            if( TJAPlayer3.Tx.Effects_Hit_FireWorks != null && TJAPlayer3.Tx.Effects_Hit_FireWorks != null )
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 45; j++)
                    {
                        if (!this.st大音符花火[j].b使用中)
                        {
                            this.st大音符花火[j].b使用中 = true;
                            this.st大音符花火[j].ct進行 = new CCounter(0, 40, 18, TJAPlayer3.Timer); // カウンタ
                            this.st大音符花火[j].fX = this.nX座標[ i ]; //X座標
                            this.st大音符花火[j].fY = nPlayer == 0 ? this.nY座標[ i ] : this.nY座標P2[ i ];

                            switch(nLane)
                            {
                                case 0:
                                    this.st大音符花火[j].nColor = 0;
                                    break;
                                case 1:
                                    this.st大音符花火[j].nColor = 1;
                                    break;
                            }

                            switch( i )
                            {
                                case 0:
                                    this.st大音符花火[ j ].n開始フレーム = 0;
                                    this.st大音符花火[ j ].n終了フレーム = 16;
                                    break;
                                case 1:
                                    this.st大音符花火[ j ].n開始フレーム = 3;
                                    this.st大音符花火[ j ].n終了フレーム = 19;
                                    break;
                                case 2:
                                    this.st大音符花火[ j ].n開始フレーム = 6;
                                    this.st大音符花火[ j ].n終了フレーム = 22;
                                    break;
                                case 3:
                                    this.st大音符花火[ j ].n開始フレーム = 9;
                                    this.st大音符花火[ j ].n終了フレーム = 25;
                                    break;
                                case 4:
                                    this.st大音符花火[ j ].n開始フレーム = 12;
                                    this.st大音符花火[ j ].n終了フレーム = 28;
                                    break;
                                case 5:
                                    this.st大音符花火[ j ].n開始フレーム = 15;
                                    this.st大音符花火[ j ].n終了フレーム = 31;
                                    break;
                                case 6:
                                    this.st大音符花火[ j ].n開始フレーム = 18;
                                    this.st大音符花火[ j ].n終了フレーム = 34;
                                    break;
                                case 7:
                                    this.st大音符花火[ j ].n開始フレーム = 21;
                                    this.st大音符花火[ j ].n終了フレーム = 37;
                                    break;
                                case 8:
                                    this.st大音符花火[ j ].n開始フレーム = 24;
                                    this.st大音符花火[ j ].n終了フレーム = 40;
                                    break;
                            }



                            break;
                        }
                    }
                }
            }
        }

        public virtual void Start( int nLane, E判定 judge, int player )
		{
            for (int j = 0; j < 3 * 4; j++)
            {
                if( !this.st状態[ j ].b使用中 )
			    //for( int n = 0; n < 1; n++ )
			    {
                    this.st状態[ j ].b使用中 = true;
		    		//this.st状態[ n ].ct進行 = new CCounter( 0, 9, 20, CDTXMania.Timer );
	    			this.st状態[ j ].ct進行 = new CCounter( 0, 6, 25, TJAPlayer3.Timer );
    				this.st状態[ j ].judge = judge;
                    this.st状態[ j ].nPlayer = player;
                    this.st状態_大[ j ].nPlayer = player;

                    switch( nLane )
                    {
                        case 0x11:
                        case 0x12:
                            this.st状態[ j ].nIsBig = 0;
                            break;
                        case 0x13:
                        case 0x14:
                        case 0x1A:
                        case 0x1B:
                            this.st状態_大[ j ].ct進行 = new CCounter( 0, 9, 20, TJAPlayer3.Timer );
                            this.st状態_大[ j ].judge = judge;
                            this.st状態_大[ j ].nIsBig = 1;
                            break;
                    }
                    break;
			    }
            }
		}
		public void Start紙吹雪()
		{
            return;
            /*
            if (this.tx紙吹雪 != null)
            {
                for (int i = 0; i < 256; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        if (!this.st紙吹雪[j].b使用中)
                        {
                            this.st紙吹雪[j].b使用中 = true;
                            int n回転初期値 = CDTXMania.Random.Next(360);
                            int nX拡散方向 = CDTXMania.Random.Next(10);
                            int n拡散の大きさ = CDTXMania.Random.Next( 50, 1400 );
                            int n重力加速 = CDTXMania.Random.Next( 6, 100 );
                            double num7 = ( n拡散の大きさ / 1000.0 ) + (1 / 100.0); // 拡散の大きさ
                            //double num7 = 0.9 + ( ( (double) CDTXMania.Random.Next( 40 ) ) / 100.0 );
                            this.st紙吹雪[ j ].nGraphic = CDTXMania.Random.Next(3);
                            this.st紙吹雪[ j ].nColor = CDTXMania.Random.Next(3);
                            this.st紙吹雪[j].ct進行 = new CCounter(0, 500, 5, CDTXMania.Timer); // カウンタ
                            this.st紙吹雪[j].fX = 1000; //X座標(仮)

                            this.st紙吹雪[j].fY = ((( 470 + (((float)Math.Sin((double)this.st紙吹雪[j].f半径)) * this.st紙吹雪[j].f半径)) )); //Y座標
                            //this.st紙吹雪[j].f加速度X = (float)(num7 * Math.Cos((Math.PI * 2 * n回転初期値) / 360.0));
                            //this.st紙吹雪[ j ].f加速度X = (float)( ( num7 * Math.Cos((Math.PI * 2 * n回転初期値) / 360.0)) > 0.005 ? Math.Abs( num7 * Math.Cos((Math.PI * 2 * n回転初期値) / 360.0)) : num7 * Math.Cos((Math.PI * 2 * n回転初期値) / 360.0) );
                            this.st紙吹雪[ j ].f加速度X = (float)Math.Abs(num7 * Math.Cos((Math.PI * 2 * n回転初期値) / 360.0)) - ( nX拡散方向 / 20.0f );
                            this.st紙吹雪[j].f加速度Y = (float)-Math.Abs( num7 * (Math.Sin((Math.PI * 2 * n回転初期値) / 360.0))) - 0.05f;
                            //this.st紙吹雪[j].f加速度Y = (float)( num7 * (Math.Sin((Math.PI * 2 * n回転初期値) / 360.0))) - 0.05f;
                            this.st紙吹雪[j].f加速度の加速度X = 1.009f + (float)(num7 / 1000);
                            this.st紙吹雪[j].f加速度の加速度Y = 0.989f + (float)(num7 / 1000);
                            //this.st紙吹雪[j].f重力加速度 = 0.0100f;
                            this.st紙吹雪[j].f重力加速度 = n重力加速 / 10000.0f;
                            this.st紙吹雪[j].f半径 = (float)(0.5 + (((double)CDTXMania.Random.Next(40)) / 100.0));

                            
                            break;
                        }
                    }
                }
            }  */
		}

		// CActivity 実装

		public override void On活性化()
		{
            for( int i = 0; i < 3 * 4; i++ )
			{
				this.st状態[ i ].ct進行 = new CCounter();
                this.st状態[ i ].b使用中 = false;
                this.st状態_大[ i ].ct進行 = new CCounter();
			}
			for( int i = 0; i < 256; i++ )
			{
				this.st紙吹雪[ i ] = new ST紙吹雪();
				this.st紙吹雪[ i ].b使用中 = false;
				this.st紙吹雪[ i ].ct進行 = new CCounter();
			}
            base.On活性化();
		}
		public override void On非活性化()
		{
            for( int i = 0; i < 3 * 4; i++ )
			{
				this.st状態[ i ].ct進行 = null;
                this.st状態_大[ i ].ct進行 = null;
			}
			for( int i = 0; i < 256; i++ )
			{
				this.st紙吹雪[ i ].ct進行 = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
    //            this.txアタックエフェクトUpper = CDTXMania.tテクスチャの生成Af( CSkin.Path( @"Graphics\7_explosion_upper.png" ) );
    //            this.txアタックエフェクトUpper_big = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_explosion_upper_big.png" ) );
				//if( this.txアタックエフェクトUpper != null )
				//{
				//	this.txアタックエフェクトUpper.b加算合成 = true;
				//}
    //            this.tx大音符花火[0] = CDTXMania.tテクスチャの生成Af( CSkin.Path( @"Graphics\7_explosion_bignotes_red.png" ) );
    //            this.tx大音符花火[0].b加算合成 = true;
    //            this.tx大音符花火[1] = CDTXMania.tテクスチャの生成Af( CSkin.Path( @"Graphics\7_explosion_bignotes_blue.png" ) );
    //            this.tx大音符花火[1].b加算合成 = true;
                //this.tx紙吹雪 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_particle paper.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				//CDTXMania.tテクスチャの解放( ref this.txアタックエフェクトUpper );
				//CDTXMania.tテクスチャの解放( ref this.txアタックエフェクトUpper_big );
    //            CDTXMania.tテクスチャの解放( ref this.tx大音符花火[ 0 ] );
    //            CDTXMania.tテクスチャの解放( ref this.tx大音符花火[ 1 ] );
                //CDTXMania.tテクスチャの解放( ref this.tx紙吹雪 );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
                for( int i = 0; i < 3 * 4; i++ )
			    {
                    if( this.st状態[ i ].b使用中 )
                    {
				        if( !this.st状態[ i ].ct進行.b停止中 )
				        {
                            this.st状態[ i ].ct進行.t進行();
					        if( this.st状態[ i ].ct進行.b終了値に達した )
					        {
						        this.st状態[ i ].ct進行.t停止();
                                this.st状態[ i ].b使用中 = false;
					        }

					        // (When performing calibration, reduce visual distraction
					        // and current judgment feedback near the judgment position.)
					        if( TJAPlayer3.Tx.Effects_Hit_Explosion != null && !TJAPlayer3.IsPerformingCalibration )
					        {
                                int n = this.st状態[ i ].nIsBig == 1 ? 520 : 0;
                                int nX = ( TJAPlayer3.Skin.nScrollFieldX[ this.st状態[ i ].nPlayer ] ) - ( (TJAPlayer3.Tx.Effects_Hit_Explosion.szテクスチャサイズ.Width / 7 ) / 2 );
                                int nY = ( TJAPlayer3.Skin.nJudgePointY[ this.st状態[ i ].nPlayer ] ) - ( (TJAPlayer3.Tx.Effects_Hit_Explosion.szテクスチャサイズ.Height / 4 ) / 2 );

                                switch( st状態[ i ].judge )
                                {
                                    case E判定.Perfect:
                                    case E判定.Great:
                                    case E判定.Auto:
                                        if (!this.st状態_大[i].ct進行.b停止中 && TJAPlayer3.Tx.Effects_Hit_Explosion_Big != null && this.st状態_大[i].nIsBig == 1)  
                                                TJAPlayer3.Tx.Effects_Hit_Explosion.t2D描画(nX, nY, new Rectangle(this.st状態[i].ct進行.n現在の値 * 260, n + 520, 260, 260));
                                        else
                                            TJAPlayer3.Tx.Effects_Hit_Explosion.t2D描画(nX, nY, new Rectangle(this.st状態[i].ct進行.n現在の値 * 260, n, 260, 260));
                                        break;                                    
                                    case E判定.Good:
                                        if (!this.st状態_大[i].ct進行.b停止中 && TJAPlayer3.Tx.Effects_Hit_Explosion_Big != null && this.st状態_大[i].nIsBig == 1)
                                            TJAPlayer3.Tx.Effects_Hit_Explosion.t2D描画( nX, nY, new Rectangle( this.st状態[ i ].ct進行.n現在の値 * 260, n + 780, 260, 260 ) );
                                        else
                                            TJAPlayer3.Tx.Effects_Hit_Explosion.t2D描画(nX, nY, new Rectangle(this.st状態[i].ct進行.n現在の値 * 260, n + 260, 260, 260));
                                        break;
                                    case E判定.Miss:
                                    case E判定.Bad:
                                        break;
                                }
					        }
				        }
                    }
                }

                for( int i = 0; i < 3 * 4; i++ )
			    {
				    if( !this.st状態_大[ i ].ct進行.b停止中 )
				    {
                        this.st状態_大[ i ].ct進行.t進行();
					    if( this.st状態_大[ i ].ct進行.b終了値に達した )
					    {
						    this.st状態_大[ i ].ct進行.t停止();
					    }
					    if(TJAPlayer3.Tx.Effects_Hit_Explosion_Big != null && this.st状態_大[ i ].nIsBig == 1 )
					    {

                            switch( st状態_大[ i ].judge )
                            {
                                case E判定.Perfect:
                                case E判定.Great:
                                case E判定.Auto:
                                    if( this.st状態_大[ i ].nIsBig == 1 )
                                    {
                                        //float fY = 257 - ((this.txアタックエフェクトUpper_big.szテクスチャサイズ.Height * this.txアタックエフェクトUpper_big.vc拡大縮小倍率.Y ) / 2.0f);
                                        int nX = (TJAPlayer3.Skin.nScrollFieldX[this.st状態_大[i].nPlayer]) + ((TJAPlayer3.Tx.Effects_Hit_Explosion_Big.szテクスチャサイズ.Height) / 999992);
                                        int nY = (TJAPlayer3.Skin.nJudgePointY[this.st状態_大[i].nPlayer]) + ((TJAPlayer3.Tx.Effects_Hit_Explosion_Big.szテクスチャサイズ.Height) / 999992);

                                        ////7
                                        float f倍率 = 0.5f + ( (this.st状態_大[ i ].ct進行.n現在の値 * 0.5f) / 10.0f);
                                   
                                        TJAPlayer3.Tx.Effects_Hit_Explosion_Big.vc拡大縮小倍率 = new PointF(f倍率, f倍率);
                                        TJAPlayer3.Tx.Effects_Hit_Explosion_Big.Opacity = 255;
                                        TJAPlayer3.Tx.Effects_Hit_Explosion_Big.t2D拡大率考慮中央基準描画(nX, nY);
                                    }
                                    break;
                                    
                                case E判定.Good:
                                    break;

                                case E判定.Miss:
                                case E判定.Bad:
                                    break;
                            }
					    }
				    }
                }
			}
			return 0;
		}
		

		// その他

		#region [ private ]
		//-----------------
        //private CTextureAf txアタックエフェクトUpper;
        //private CTexture txアタックエフェクトUpper_big;
        //private CTextureAf[] tx大音符花火 = new CTextureAf[2];
        //private CTexture tx紙吹雪;

        protected STSTATUS[] st状態 = new STSTATUS[ 3 * 4 ];
        protected STSTATUS_B[] st状態_大 = new STSTATUS_B[ 3 * 4 ];
        private ST大音符花火[] st大音符花火 = new ST大音符花火[45];

        protected int[] nX座標 = new int[] { 450, 521, 596, 686, 778, 863, 970, 1070, 1150 };
        protected int[] nY座標 = new int[] { 172, 108,  50,   8, -10, -60,  -5,   30,   90 };
        protected int[] nY座標P2 = new int[] { 172, 108,  50,   8, -10, -60,  -5,   30,   90 };

        [StructLayout(LayoutKind.Sequential)]
        protected struct STSTATUS
        {
            public bool b使用中;
            public CCounter ct進行;
            public E判定 judge;
            public int nIsBig;
            public int n透明度;
            public int nPlayer;
        }
        [StructLayout(LayoutKind.Sequential)]
        protected struct STSTATUS_B
        {
            public CCounter ct進行;
            public E判定 judge;
            public int nIsBig;
            public int n透明度;
            public int nPlayer;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ST大音符花火
        {
            public int nColor;
            public bool b使用中;
            public CCounter ct進行;
            public int n前回のValue;
            public float fX;
            public float fY;
            public int n開始フレーム;
            public int n終了フレーム;
        }

        private ST紙吹雪[] st紙吹雪 = new ST紙吹雪[ 256 ];
		[StructLayout( LayoutKind.Sequential )]
		private struct ST紙吹雪
		{
            public int nGraphic;
            public int nColor;
			public bool b使用中;
			public CCounter ct進行;
			public int n前回のValue;
			public float fX;
			public float fY;
			public float f加速度X;
			public float f加速度Y;
			public float f加速度の加速度X;
			public float f加速度の加速度Y;
			public float f重力加速度;
			public float f半径;
            public float f角度;
		}
		//-----------------
		#endregion
	}
}
　
