using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using DxLibDLL;

namespace FDK
{
	public class CAvi : CTexture, IDisposable
	{
		// コンストラクタ

		/// <param name="fileName">ファイル名。</param>
		public CAvi(string filename )
			: base(filename)
		{
			
		}


		// メソッド

		public void t再生()
		{
			DX.PlayMovieToGraph(Handle);
		}
		public void t停止()
		{
			DX.PauseMovieToGraph(Handle);
		}
	}
}
