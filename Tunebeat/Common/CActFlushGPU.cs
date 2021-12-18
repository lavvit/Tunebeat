using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using FDK;

namespace TJAPlayer3
{
	/// <summary>
	/// 描画フレーム毎にGPUをフラッシュして、描画遅延を防ぐ。
	/// DirectX9の、Occlusion Queryを用いる。(Flush属性付きでGetDataする)
	/// Device Lost対策のため、QueueをCActivitiyのManagedリソースとして扱う。
	/// On進行描画()を呼び出すことで、GPUをフラッシュする。
	/// </summary>
	internal class CActFlushGPU : CActivity
	{
		// CActivity 実装

		public override void OnManagedリソースの作成()
		{
			if ( !base.b活性化してない )
			{
				base.OnManagedリソースの作成();
			}
		}
		public override void  OnManagedリソースの解放()
		{
			base.OnManagedリソースの解放();
		}
		public override int On進行描画()
		{
			if ( !base.b活性化してない )
			{
			}
			return 0;
		}

		// その他

		#region [ private ]
		//-----------------
		//-----------------
		#endregion
	}
}
