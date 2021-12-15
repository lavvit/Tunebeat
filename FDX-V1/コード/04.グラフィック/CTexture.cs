using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using DxLibDLL;

namespace FDK
{
    public class CTexture : IDisposable
    {
        // プロパティ
        public bool b加算合成
        {
            get;
            set;
        }
        public bool b乗算合成
        {
            get;
            set;
        }
        public bool b減算合成
        {
            get;
            set;
        }
        public bool bスクリーン合成
        {
            get;
            set;
        }
        public float fZ軸中心回転
        {
            get;
            set;
        }
        public int Handle = -1;

        public int Opacity
        {
            get
            {
                return this._opacity;
            }
            set
            {
                if (value < 0)
                {
                    this._opacity = 0;
                }
                else if (value > 0xff)
                {
                    this._opacity = 0xff;
                }
                else
                {
                    this._opacity = value;
                }
            }
        }
        public Size szテクスチャサイズ
        {
            get
            {
                return new Size(this._size.Width <= 0 ? 1 : this._size.Width, this._size.Height <= 0 ? 1 : this._size.Height);
            }
            set
            {
                this._size = value;
            }
        }
        public PointF vc拡大縮小倍率;

        // コンストラクタ
        public CTexture(string Path)
        {
            try
            {
                this.Handle = DX.LoadGraph(Path);
                DX.GetGraphSizeF(this.Handle, out float SizeX, out float SizeY);
                this.szテクスチャサイズ = new Size((int)SizeX, (int)SizeY);
                this._opacity = 0xff;
                this.b加算合成 = false;
                this.fZ軸中心回転 = 0f;
                this.vc拡大縮小倍率 = new PointF(1,1);
                this.rc全画像 = new Rectangle(0, 0, this.szテクスチャサイズ.Width, this.szテクスチャサイズ.Height);
            }
            catch (Exception e)
            {
            }
        }
        public CTexture(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                var buf = ms.ToArray();

                unsafe
                {
                    fixed (byte* p = buf)
                    {
                        DX.SetDrawValidGraphCreateFlag(DX.TRUE);
                        DX.SetDrawValidAlphaChannelGraphCreateFlag(DX.TRUE);

                        Handle = DX.CreateGraphFromMem((IntPtr)p, buf.Length);
                        DX.GetGraphSizeF(this.Handle, out float SizeX, out float SizeY);
                        this.szテクスチャサイズ = new Size((int)SizeX, (int)SizeY);
                        this._opacity = 0xff;
                        this.b加算合成 = false;
                        this.fZ軸中心回転 = 0f;
                        this.vc拡大縮小倍率 = new PointF(1, 1);
                        this.rc全画像 = new Rectangle(0, 0, this.szテクスチャサイズ.Width, this.szテクスチャサイズ.Height);

                        DX.SetDrawValidGraphCreateFlag(DX.FALSE);
                        DX.SetDrawValidAlphaChannelGraphCreateFlag(DX.FALSE);
                    }
                }
            }
        }



        // メソッド

        // 2016.11.10 kairera0467 拡張
        // Rectangleを使う場合、座標調整のためにテクスチャサイズの値をそのまま使うとまずいことになるため、Rectragleから幅を取得して調整をする。
        public void t2D中心基準描画(int x, int y)
        {
            this.t2D描画(x - (this.szテクスチャサイズ.Width / 2), y - (this.szテクスチャサイズ.Height / 2), 1f, this.rc全画像);
        }
        public void t2D中心基準描画(int x, int y, Rectangle rc画像内の描画領域)
        {
            this.t2D描画(x - (rc画像内の描画領域.Width / 2), y - (rc画像内の描画領域.Height / 2), 1f, rc画像内の描画領域);
        }
        public void t2D中心基準描画(float x, float y)
        {
            this.t2D描画((int)x - (this.szテクスチャサイズ.Width / 2), (int)y - (this.szテクスチャサイズ.Height / 2), 1f, this.rc全画像);
        }
        public void t2D中心基準描画(float x, float y, float depth, Rectangle rc画像内の描画領域)
        {
            this.t2D描画((int)x - (rc画像内の描画領域.Width / 2), (int)y - (rc画像内の描画領域.Height / 2), depth, rc画像内の描画領域);
        }

        // 下を基準にして描画する(拡大率考慮)メソッドを追加。 (AioiLight)
        public void t2D拡大率考慮下基準描画(int x, int y)
        {
            this.t2D描画(x, y - (szテクスチャサイズ.Height * this.vc拡大縮小倍率.Y), 1f, this.rc全画像);
        }
        public void t2D拡大率考慮下基準描画(int x, int y, Rectangle rc画像内の描画領域)
        {
            this.t2D描画(x, y - (rc画像内の描画領域.Height * this.vc拡大縮小倍率.Y), 1f, rc画像内の描画領域);
        }
        public void t2D拡大率考慮下中心基準描画(int x, int y)
        {
            this.t2D描画(x - (this.szテクスチャサイズ.Width / 2), y - (szテクスチャサイズ.Height * this.vc拡大縮小倍率.Y), 1f, this.rc全画像);
        }
        public void t2D拡大率考慮下中心基準描画(float x, float y)
        {
            this.t2D拡大率考慮下中心基準描画((int)x, (int)y);
        }

        public void t2D拡大率考慮下中心基準描画(int x, int y, Rectangle rc画像内の描画領域)
        {
            this.t2D描画(x - ((rc画像内の描画領域.Width / 2)), y - (rc画像内の描画領域.Height * this.vc拡大縮小倍率.Y), 1f, rc画像内の描画領域);
        }
        public void t2D拡大率考慮下中心基準描画(float x, float y, Rectangle rc画像内の描画領域)
        {
            this.t2D拡大率考慮下中心基準描画((int)x, (int)y, rc画像内の描画領域);
        }
        public void t2D下中央基準描画(int x, int y)
        {
            this.t2D描画(x - (this.szテクスチャサイズ.Width / 2), y - (szテクスチャサイズ.Height), this.rc全画像);
        }
        public void t2D下中央基準描画(int x, int y, Rectangle rc画像内の描画領域)
        {
            this.t2D描画(x - (rc画像内の描画領域.Width / 2), y - (rc画像内の描画領域.Height), rc画像内の描画領域);
            //this.t2D描画(devicek x, y, rc画像内の描画領域;
        }


        public void t2D拡大率考慮中央基準描画(int x, int y)
        {
            this.t2D描画(x - (this.szテクスチャサイズ.Width / 2 * this.vc拡大縮小倍率.X), y - (szテクスチャサイズ.Height / 2 * this.vc拡大縮小倍率.Y), 1f, this.rc全画像);
        }
        public void t2D拡大率考慮中央基準描画(float x, float y)
        {
            this.t2D拡大率考慮中央基準描画((int)x, (int)y);
        }

        public void t2D拡大率考慮中央基準描画(float x, float y, Rectangle rc画像内の描画領域)
        {
            this.t2D描画((int)x - (rc画像内の描画領域.Width / 2 * this.vc拡大縮小倍率.X), y - (rc画像内の描画領域.Height / 2 * this.vc拡大縮小倍率.Y), rc画像内の描画領域);
        }


        /// <summary>
        /// テクスチャを 2D 画像と見なして描画する。
        /// </summary>
        /// <param name="device">Direct3D9 デバイス。</param>
        /// <param name="x">描画位置（テクスチャの左上位置の X 座標[dot]）。</param>
        /// <param name="y">描画位置（テクスチャの左上位置の Y 座標[dot]）。</param>
        public void t2D描画(int x, int y)
        {
            this.t2D描画(x, y, 1f, this.rc全画像);
        }
        public void t2D描画(float x, float y)
        {
            this.t2D描画(x, y, 1f, this.rc全画像);
        }
        public void t2D描画(int x, int y, Rectangle rc画像内の描画領域)
        {
            this.t2D描画(x, y, 1f, rc画像内の描画領域);
        }
        public void t2D描画(float x, float y, Rectangle rectangle)
        {
            this.t2D描画((int)x, (int)y, 1f, rectangle);
        }
        public void t2D描画(float x, float y, float depth, Rectangle rc画像内の描画領域)
        {
            if (this.Handle == -1)
                return;

            this.tレンダリングステートの設定();

            DX.SetDrawBright(color4.R, color4.G, color4.B);
            DX.DrawRectRotaGraph3F(x, y, rc画像内の描画領域.X, rc画像内の描画領域.Y, rc画像内の描画領域.Width, rc画像内の描画領域.Height, 0, 0, this.vc拡大縮小倍率.X, this.vc拡大縮小倍率.Y, -fZ軸中心回転, Handle, DX.TRUE);
            DX.SetDrawBright(255, 255, 255);
        }
        public void t2D上下反転描画(int x, int y)
        {
            this.t2D上下反転描画(x, y, 1f, this.rc全画像);
        }
        public void t2D上下反転描画(int x, int y, Rectangle rc画像内の描画領域)
        {
            this.t2D上下反転描画(x, y, 1f, rc画像内の描画領域);
        }
        public void t2D上下反転描画(int x, int y, float depth, Rectangle rc画像内の描画領域)
        {
            if (this.Handle == -1)
                return;

            this.tレンダリングステートの設定();

            DX.SetDrawBright(color4.R, color4.G, color4.B);
            DX.DrawRectRotaGraph3F(x, y + rc画像内の描画領域.Height, rc画像内の描画領域.X, rc画像内の描画領域.Y , rc画像内の描画領域.Width, rc画像内の描画領域.Height, 0, 0, this.vc拡大縮小倍率.X, -this.vc拡大縮小倍率.Y, -fZ軸中心回転, Handle, DX.TRUE);
            DX.SetDrawBright(255, 255, 255);
        }
        public void t2D上下反転描画(Point pt)
        {
            this.t2D上下反転描画(pt.X, pt.Y, 1f, this.rc全画像);
        }
        public void t2D上下反転描画(Point pt, Rectangle rc画像内の描画領域)
        {
            this.t2D上下反転描画(pt.X, pt.Y, 1f, rc画像内の描画領域);
        }
        public void t2D上下反転描画(Point pt, float depth, Rectangle rc画像内の描画領域)
        {
            this.t2D上下反転描画(pt.X, pt.Y, depth, rc画像内の描画領域);
        }
        #region [ IDisposable 実装 ]
        //-----------------
        public void Dispose()
        {
            if (!this.bDispose完了済み)
            {

                this.bDispose完了済み = true;
            }
        }
        //-----------------
        #endregion


        // その他

        #region [ private ]
        //-----------------
        private int _opacity;
        private bool bDispose完了済み;

        /// <summary>
        /// どれか一つが有効になります。
        /// </summary>
        /// <param name="device">Direct3Dのデバイス</param>
		private void tレンダリングステートの設定()
        {
            if (this.b加算合成)
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ADD, Opacity);
            }
            else if (this.b乗算合成)
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_MUL, Opacity);
            }
            else if (this.b減算合成)
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_SUB, Opacity);
            }
            else if (this.bスクリーン合成)
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ADD, Opacity);
            }
            else
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, Opacity);
            }
        }


        // 2012.3.21 さらなる new の省略作戦

        protected Rectangle rc全画像;                              // テクスチャ作ったらあとは不変
        public Color color4 = Color.FromArgb(255, 255, 255);
        private Size _size;


        #endregion
    }
}
