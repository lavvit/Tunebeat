using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	/// <summary>
	/// <para>一定の間隔で処理を行うテンプレートパターンの定義。</para>
	/// <para>たとえば、t進行() で 5ms ごとに行う処理を前回のt進行()の呼び出しから 15ms 後に呼び出した場合は、処理が 3回 実行される。</para>
	/// </summary>
	public class C一定間隔処理 : IDisposable
	{
		public delegate void dg処理();
		public void t進行( long n間隔ms, dg処理 dg処理 )
		{

			// 初めての進行処理

			if( this.n前回の時刻 == -1 )
				this.n前回の時刻 = DxLibDLL.DX.GetNowCount();


			// タイマが一回りしてしまった時のため……

			if(DxLibDLL.DX.GetNowCount() < this.n前回の時刻 )
				this.n前回の時刻 = DxLibDLL.DX.GetNowCount();

	
			// 時間内の処理を実行。

			while( (DxLibDLL.DX.GetNowCount() - this.n前回の時刻 ) >= n間隔ms )
			{
				dg処理();

				this.n前回の時刻 += n間隔ms;
			}
		}

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
		}
		//-----------------
		#endregion

		#region [ protected ]
		//-----------------
		protected long n前回の時刻 = -1;
		//-----------------
		#endregion
	}
}
