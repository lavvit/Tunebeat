using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using DxLibDLL;
using FDK;
using System.Reflection;

namespace TJAPlayer3
{
	internal class TJAPlayer3 : IDisposable
	{
        // プロパティ
        #region [ properties ]
        public static readonly string VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Length - 2);

        public static readonly string SLIMDXDLL = "c_net20x86_Jun2010";
		public static readonly string D3DXDLL = "d3dx9_43.dll";		// June 2010
        //public static readonly string D3DXDLL = "d3dx9_42.dll";	// February 2010
        //public static readonly string D3DXDLL = "d3dx9_41.dll";	// March 2009

		public static TJAPlayer3 app
		{
			get;
			private set;
		}
		public static C文字コンソール act文字コンソール
		{ 
			get;
			private set;
		}
		public static bool bコンパクトモード
		{
			get;
			private set;
		}
		public static CConfigIni ConfigIni
		{
			get; 
			private set;
		}
		public static CDTX DTX
		{
			get
			{
				return dtx[ 0 ];
			}
			set
			{
				if( ( dtx[ 0 ] != null ) && ( app != null ) )
				{
					dtx[ 0 ].On非活性化();
					app.listトップレベルActivities.Remove( dtx[ 0 ] );
				}
				dtx[ 0 ] = value;
				if( ( dtx[ 0 ] != null ) && ( app != null ) )
				{
					app.listトップレベルActivities.Add( dtx[ 0 ] );
				}
			}
		}
		public static CDTX DTX_2P
		{
			get
			{
				return dtx[ 1 ];
			}
			set
			{
				if( ( dtx[ 1 ] != null ) && ( app != null ) )
				{
					dtx[ 1 ].On非活性化();
					app.listトップレベルActivities.Remove( dtx[ 1 ] );
				}
				dtx[ 1 ] = value;
				if( ( dtx[ 1 ] != null ) && ( app != null ) )
				{
					app.listトップレベルActivities.Add( dtx[ 1 ] );
				}
			}
		}

	    public static bool IsPerformingCalibration;

		public static CFPS FPS
		{ 
			get; 
			private set;
		}
		public static CInput管理 Input管理 
		{
			get;
			private set;
		}
		#region [ 入力範囲ms ]
		public static int nPerfect範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX ) ) && ( c曲リストノード.nPerfect範囲ms >= 0 ) )
					{
						return c曲リストノード.nPerfect範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Perfect;
			}
		}
		public static int nGreat範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX ) ) && ( c曲リストノード.nGreat範囲ms >= 0 ) )
					{
						return c曲リストノード.nGreat範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Great;
			}
		}
		public static int nGood範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX ) ) && ( c曲リストノード.nGood範囲ms >= 0 ) )
					{
						return c曲リストノード.nGood範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Good;
			}
		}
		public static int nPoor範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX ) ) && ( c曲リストノード.nPoor範囲ms >= 0 ) )
					{
						return c曲リストノード.nPoor範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Poor;
			}
		}
		#endregion
		public static CPad Pad 
		{
			get;
			private set;
		}
		public static Random Random
		{
			get;
			private set;
		}
		public static CSkin Skin
		{
			get; 
			private set;
		}
		public static CSongs管理 Songs管理 
		{
			get;
			set;	// 2012.1.26 yyagi private解除 CStage起動でのdesirialize読み込みのため
		}
		public static CEnumSongs EnumSongs
		{
			get;
			private set;
		}
		public static CActEnumSongs actEnumSongs
		{
			get;
			private set;
		}
		public static CActScanningLoudness actScanningLoudness
		{
			get;
			private set;
		}
		public static CActFlushGPU actFlushGPU
		{
			get;
			private set;
		}
		public static CStage起動 stage起動 
		{
			get; 
			private set;
		}
		public static CStageタイトル stageタイトル
		{
			get;
			private set;
		}
//		public static CStageオプション stageオプション
//		{ 
//			get;
//			private set;
//		}
		public static CStageコンフィグ stageコンフィグ 
		{ 
			get; 
			private set;
		}
		public static CStage選曲 stage選曲
		{
			get;
			private set;
		}
		public static CStage曲読み込み stage曲読み込み
		{
			get;
			private set;
		}
		public static CStage演奏ドラム画面 stage演奏ドラム画面
		{
			get;
			private set;
		}
		public static CStage結果 stage結果
		{
			get;
			private set;
		}
		public static CStageChangeSkin stageChangeSkin
		{
			get;
			private set;
		}
		public static CStage終了 stage終了
		{
			get;
			private set;
		}
		public static CStage r現在のステージ = null;
		public static CStage r直前のステージ = null;
		public static string strEXEのあるフォルダ 
		{
			get;
			private set;
		}
		public static string strコンパクトモードファイル
		{ 
			get; 
			private set;
		}
		public static int Timer
		{
			get;
			private set;
		}
		internal static IPluginActivity act現在入力を占有中のプラグイン = null;
		public bool bApplicationActive
		{
			get; 
			private set;
		}
		public bool b次のタイミングで垂直帰線同期切り替えを行う
		{
			get; 
			set;
		}
		public bool b次のタイミングで全画面_ウィンドウ切り替えを行う
		{
			get;
			set;
		}
		public CPluginHost PluginHost
		{
			get;
			private set;
		}
		public List<STPlugin> listプラグイン = new List<STPlugin>();
		public struct STPlugin
		{
			public IPluginActivity plugin;
			public string strプラグインフォルダ;
			public string strアセンブリ簡易名;
			public Version Version;
		}
		private static Size currentClientSize		// #23510 2010.10.27 add yyagi to keep current window size
		{
			get;
			set;
		}
		public static CDTXVmode DTXVmode			// #28821 2014.1.23 yyagi
		{
			get;
			set;
		}

        #endregion

        // コンストラクタ

        public TJAPlayer3()
		{
			TJAPlayer3.app = this;
		}


		// メソッド

		public void t全画面_ウィンドウモード切り替え()
		{
			DX.ChangeWindowMode(DX.FALSE);
		}

		#region [ #24609 リザルト画像をpngで保存する ]		// #24609 2011.3.14 yyagi; to save result screen in case BestRank or HiSkill.
		/// <summary>
		/// リザルト画像のキャプチャと保存。
		/// </summary>
		/// <param name="strFilename">保存するファイル名(フルパス)</param>
		public bool SaveResultScreen(string strFullPath)
		{
			string strSavePath = Path.GetDirectoryName(strFullPath);
			if (!Directory.Exists(strSavePath))
			{
				try
				{
					Directory.CreateDirectory(strSavePath);
				}
				catch (Exception e)
				{
					Trace.TraceError(e.ToString());
					Trace.TraceError("例外が発生しましたが処理を継続します。 (0bfe6bff-2a56-4df4-9333-2df26d9b765b)");
					return false;
				}
			}

			DX.SaveDrawScreenToPNG(0, 0, 1280, 720, strFullPath);
			return true;
		}
		#region [ #24609 リザルト画像をpngで保存する ]		// #24609 2011.3.14 yyagi; to save result screen in case BestRank or HiSkill.
		/// <summary>
		/// リザルト画像のキャプチャと保存。
		/// 自動保存モード時は、ランク更新or演奏型スキル更新時に自動保存。
		/// 手動保存モード時は、ランクに依らず保存。
		/// </summary>
		/// <param name="bIsAutoSave">true=自動保存モード, false=手動保存モード</param>
		private void CheckAndSaveResultScreen(bool bIsAutoSave)
		{
			string path = @"ScreenShot\";
			string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
			if (bIsAutoSave)
			{
				string strFullPath = path + "." + datetime + ".png";
				SaveResultScreen(strFullPath);
			}
		}
		#endregion
		#endregion

		// Game 実装
		public void Initialize()
		{
			if ( this.listトップレベルActivities != null )
			{
				foreach( CActivity activity in this.listトップレベルActivities )
					activity.OnManagedリソースの作成();
			}

			foreach( STPlugin st in this.listプラグイン )
			{
				Directory.SetCurrentDirectory( st.strプラグインフォルダ );
				st.plugin.OnManagedリソースの作成();
				Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
			}
		}
		public void LoadContent()
		{
			if ( ConfigIni.bウィンドウモード )
			{
				if( !this.bマウスカーソル表示中 )
				{
					Cursor.Show();
					this.bマウスカーソル表示中 = true;
				}
			}
			else if( this.bマウスカーソル表示中 )
			{
				Cursor.Hide();
				this.bマウスカーソル表示中 = false;
			}
		
			if( this.listトップレベルActivities != null )
			{
				foreach( CActivity activity in this.listトップレベルActivities )
					activity.OnUnmanagedリソースの作成();
			}

			foreach( STPlugin st in this.listプラグイン )
			{
				Directory.SetCurrentDirectory( st.strプラグインフォルダ );
				st.plugin.OnUnmanagedリソースの作成();
				Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
			}
		}
		public void UnloadContent()
		{
			if( this.listトップレベルActivities != null )
			{
				foreach( CActivity activity in this.listトップレベルActivities )
					activity.OnUnmanagedリソースの解放();
			}

			foreach( STPlugin st in this.listプラグイン )
			{
				Directory.SetCurrentDirectory( st.strプラグインフォルダ );
				st.plugin.OnUnmanagedリソースの解放();
				Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
			}
		}
		public void OnExiting( EventArgs e )
		{
			CPowerManagement.tEnableMonitorSuspend();		// スリープ抑止状態を解除
			this.t終了処理();
		}
		protected void Update()
		{
		}
		public void Draw()
		{
			while (DX.ProcessMessage() == 0 && DX.ScreenFlip() == 0 && DX.ClearDrawScreen() == 0)
			{
				Input管理?.Update();
                FPS?.tカウンタ更新();

				if (Input管理.IsPushedKey(DX.KEY_INPUT_F12)) CheckAndSaveResultScreen(true);
			if ( this.bApplicationActive )
				CPowerManagement.tDisableMonitorSuspend();

			#region [ スリープ ]
			if ( ConfigIni.nフレーム毎スリープms >= 0 )			// #xxxxx 2011.11.27 yyagi
			{
				Thread.Sleep( ConfigIni.nフレーム毎スリープms );
			}
			#endregion

			if( r現在のステージ != null )
			{
				this.n進行描画の戻り値 = ( r現在のステージ != null ) ? r現在のステージ.On進行描画() : 0;

				#region [ プラグインの進行描画 ]
				//---------------------
				foreach( STPlugin sp in this.listプラグイン )
				{
					Directory.SetCurrentDirectory( sp.strプラグインフォルダ );

					Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
				}
				//---------------------
				#endregion


				CScoreIni scoreIni = null;

				if ( Control.IsKeyLocked( Keys.CapsLock ) )				// #30925 2013.3.11 yyagi; capslock=ON時は、EnumSongsしないようにして、起動負荷とASIOの音切れの関係を確認する
				{														// → songs.db等の書き込み時だと音切れするっぽい
					actEnumSongs.On非活性化();
					EnumSongs.SongListEnumCompletelyDone();
					TJAPlayer3.stage選曲.bIsEnumeratingSongs = false;
				}
				#region [ 曲検索スレッドの起動/終了 ]					// ここに"Enumerating Songs..."表示を集約
				if ( !TJAPlayer3.bコンパクトモード )
				{
					actEnumSongs.On進行描画();							// "Enumerating Songs..."アイコンの描画
				}
				switch ( r現在のステージ.eステージID )
				{
					case CStage.Eステージ.タイトル:
					case CStage.Eステージ.コンフィグ:
					case CStage.Eステージ.選曲:
					case CStage.Eステージ.曲読み込み:
						if ( EnumSongs != null )
						{
							#region [ (特定条件時) 曲検索スレッドの起動_開始 ]
							if ( r現在のステージ.eステージID == CStage.Eステージ.タイトル &&
								 r直前のステージ.eステージID == CStage.Eステージ.起動 &&
								 this.n進行描画の戻り値 == (int) CStageタイトル.E戻り値.継続 &&
								 !EnumSongs.IsSongListEnumStarted )
							{
								actEnumSongs.On活性化();
								TJAPlayer3.stage選曲.bIsEnumeratingSongs = true;
								EnumSongs.Init( TJAPlayer3.Songs管理.listSongsDB, TJAPlayer3.Songs管理.nSongsDBから取得できたスコア数 );	// songs.db情報と、取得した曲数を、新インスタンスにも与える
								EnumSongs.StartEnumFromDisk();		// 曲検索スレッドの起動_開始
								if ( TJAPlayer3.Songs管理.nSongsDBから取得できたスコア数 == 0 )	// もし初回起動なら、検索スレッドのプライオリティをLowestでなくNormalにする
								{
									EnumSongs.ChangeEnumeratePriority( ThreadPriority.Normal );
								}
							}
							#endregion
							
							#region [ 曲検索の中断と再開 ]
							if ( r現在のステージ.eステージID == CStage.Eステージ.選曲 && !EnumSongs.IsSongListEnumCompletelyDone )
							{
								switch ( this.n進行描画の戻り値 )
								{
									case 0:		// 何もない
										//if ( CDTXMania.stage選曲.bIsEnumeratingSongs )
										if ( !TJAPlayer3.stage選曲.bIsPlayingPremovie )
										{
											EnumSongs.Resume();						// #27060 2012.2.6 yyagi 中止していたバックグランド曲検索を再開
											EnumSongs.IsSlowdown = false;
										}
										else
										{
											// EnumSongs.Suspend();					// #27060 2012.3.2 yyagi #PREMOVIE再生中は曲検索を低速化
											EnumSongs.IsSlowdown = true;
										}
										actEnumSongs.On活性化();
										break;

									case 2:		// 曲決定
										EnumSongs.Suspend();						// #27060 バックグラウンドの曲検索を一時停止
										actEnumSongs.On非活性化();
										break;
								}
							}
							#endregion

							#region [ 曲探索中断待ち待機 ]
							if ( r現在のステージ.eステージID == CStage.Eステージ.曲読み込み && !EnumSongs.IsSongListEnumCompletelyDone &&
								EnumSongs.thDTXFileEnumerate != null )							// #28700 2012.6.12 yyagi; at Compact mode, enumerating thread does not exist.
							{
								EnumSongs.WaitUntilSuspended();									// 念のため、曲検索が一時中断されるまで待機
							}
							#endregion

							#region [ 曲検索が完了したら、実際の曲リストに反映する ]
							// CStage選曲.On活性化() に回した方がいいかな？
							if ( EnumSongs.IsSongListEnumerated )
							{
								actEnumSongs.On非活性化();
								TJAPlayer3.stage選曲.bIsEnumeratingSongs = false;

								bool bRemakeSongTitleBar = ( r現在のステージ.eステージID == CStage.Eステージ.選曲 ) ? true : false;
								TJAPlayer3.stage選曲.Refresh( EnumSongs.Songs管理, bRemakeSongTitleBar );
								EnumSongs.SongListEnumCompletelyDone();
							}
							#endregion
						}
						break;
				}
				#endregion



				switch ( r現在のステージ.eステージID )
				{
					case CStage.Eステージ.何もしない:
						break;

					case CStage.Eステージ.起動:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							if( !bコンパクトモード )
							{
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ タイトル" );
								stageタイトル.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageタイトル;
							}
							else
							{
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 曲読み込み" );
								stage曲読み込み.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage曲読み込み;

							}
							foreach( STPlugin pg in this.listプラグイン )
							{
								Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
								pg.plugin.Onステージ変更();
								Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
							}

							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.タイトル:
						#region [ *** ]
						//-----------------------------
						switch( this.n進行描画の戻り値 )
						{
							case (int)CStageタイトル.E戻り値.GAMESTART:
								#region [ 選曲処理へ ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 選曲" );
								stage選曲.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;
								//-----------------------------
								#endregion
								break;

							#region [ OPTION: 廃止済 ]
//							case 2:									// #24525 OPTIONとCONFIGの統合に伴い、OPTIONは廃止
//								#region [ *** ]
//								//-----------------------------
//								r現在のステージ.On非活性化();
//								Trace.TraceInformation( "----------------------" );
//								Trace.TraceInformation( "■ オプション" );
//								stageオプション.On活性化();
//								r直前のステージ = r現在のステージ;
//								r現在のステージ = stageオプション;
//								//-----------------------------
//								#endregion
							//								break;
							#endregion

							case (int)CStageタイトル.E戻り値.CONFIG:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ コンフィグ" );
								stageコンフィグ.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageコンフィグ;
								//-----------------------------
								#endregion
								break;

							case (int)CStageタイトル.E戻り値.EXIT:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 終了" );
								stage終了.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage終了;
								//-----------------------------
								#endregion
								break;
						}

						foreach( STPlugin pg in this.listプラグイン )
						{
							Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
							pg.plugin.Onステージ変更();
							Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
						}

						//this.tガベージコレクションを実行する();		// #31980 2013.9.3 yyagi タイトル画面でだけ、毎フレームGCを実行して重くなっていた問題の修正
						//-----------------------------
						#endregion
						break;

//					case CStage.Eステージ.オプション:
						#region [ *** ]
//						//-----------------------------
//						if( this.n進行描画の戻り値 != 0 )
//						{
//							switch( r直前のステージ.eステージID )
//							{
//								case CStage.Eステージ.タイトル:
//									#region [ *** ]
//									//-----------------------------
//									r現在のステージ.On非活性化();
//									Trace.TraceInformation( "----------------------" );
//									Trace.TraceInformation( "■ タイトル" );
//									stageタイトル.On活性化();
//									r直前のステージ = r現在のステージ;
//									r現在のステージ = stageタイトル;
//						
//									foreach( STPlugin pg in this.listプラグイン )
//									{
//										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
//										pg.plugin.Onステージ変更();
//										Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
//									}
//						
//									this.tガベージコレクションを実行する();
//									break;
//								//-----------------------------
//									#endregion
//
//								case CStage.Eステージ.選曲:
//									#region [ *** ]
//									//-----------------------------
//									r現在のステージ.On非活性化();
//									Trace.TraceInformation( "----------------------" );
//									Trace.TraceInformation( "■ 選曲" );
//									stage選曲.On活性化();
//									r直前のステージ = r現在のステージ;
//									r現在のステージ = stage選曲;
//
//									foreach( STPlugin pg in this.listプラグイン )
//									{
//										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
//										pg.plugin.Onステージ変更();
//										Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
//									}
//
//									this.tガベージコレクションを実行する();
//									break;
//								//-----------------------------
//									#endregion
//							}
//						}
//						//-----------------------------
						#endregion
//						break;

					case CStage.Eステージ.コンフィグ:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							switch( r直前のステージ.eステージID )
							{
								case CStage.Eステージ.タイトル:
									#region [ *** ]
									//-----------------------------
									r現在のステージ.On非活性化();
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ タイトル" );
									stageタイトル.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stageタイトル;

									foreach( STPlugin pg in this.listプラグイン )
									{
										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
									}

									this.tガベージコレクションを実行する();
									break;
								//-----------------------------
									#endregion

								case CStage.Eステージ.選曲:
									#region [ *** ]
									//-----------------------------
									r現在のステージ.On非活性化();
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ 選曲" );
									stage選曲.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									foreach( STPlugin pg in this.listプラグイン )
									{
										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
									}

									this.tガベージコレクションを実行する();
									break;
								//-----------------------------
									#endregion
							}
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.選曲:
						#region [ *** ]
						//-----------------------------
						switch( this.n進行描画の戻り値 )
						{
							case (int) CStage選曲.E戻り値.タイトルに戻る:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ タイトル" );
								stageタイトル.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageタイトル;

								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
								}

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
								#endregion

							case (int) CStage選曲.E戻り値.選曲した:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 曲読み込み" );
								stage曲読み込み.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage曲読み込み;

								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
								}

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
								#endregion

//							case (int) CStage選曲.E戻り値.オプション呼び出し:
								#region [ *** ]
//								//-----------------------------
//								r現在のステージ.On非活性化();
//								Trace.TraceInformation( "----------------------" );
//								Trace.TraceInformation( "■ オプション" );
//								stageオプション.On活性化();
//								r直前のステージ = r現在のステージ;
//								r現在のステージ = stageオプション;
//
//								foreach( STPlugin pg in this.listプラグイン )
//								{
//									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
//									pg.plugin.Onステージ変更();
//									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
//								}
//
//								this.tガベージコレクションを実行する();
//								break;
//							//-----------------------------
								#endregion

							case (int) CStage選曲.E戻り値.コンフィグ呼び出し:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ コンフィグ" );
								stageコンフィグ.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageコンフィグ;

								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
								}

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
								#endregion

							case (int) CStage選曲.E戻り値.スキン変更:

								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ スキン切り替え" );
								stageChangeSkin.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageChangeSkin;
								break;
							//-----------------------------
								#endregion
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.曲読み込み:
						#region [ *** ]
						//-----------------------------
						DTXVmode.Refreshed = false;		// 曲のリロード中に発生した再リロードは、無視する。
						if( this.n進行描画の戻り値 != 0 )
						{
							r現在のステージ.On非活性化();
							#region [ ESC押下時は、曲の読み込みを中止して選曲画面に戻る ]
							if ( this.n進行描画の戻り値 == (int) E曲読込画面の戻り値.読込中止 )
							{
								//DTX.t全チップの再生停止();
								if( DTX != null )
                                    DTX.On非活性化();
								Trace.TraceInformation( "曲の読み込みを中止しました。" );
								this.tガベージコレクションを実行する();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 選曲" );
								stage選曲.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;
								foreach ( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
								}
								break;
							}
							#endregion

							Trace.TraceInformation( "----------------------" );
							Trace.TraceInformation( "■ 演奏（ドラム画面）" );
#if false		// #23625 2011.1.11 Config.iniからダメージ/回復値の定数変更を行う場合はここを有効にする 087リリースに合わせ機能無効化
for (int i = 0; i < 5; i++)
{
	for (int j = 0; j < 2; j++)
	{
		stage演奏ドラム画面.fDamageGaugeDelta[i, j] = ConfigIni.fGaugeFactor[i, j];
	}
}
for (int i = 0; i < 3; i++) {
	stage演奏ドラム画面.fDamageLevelFactor[i] = ConfigIni.fDamageLevelFactor[i];
}		
#endif
							r直前のステージ = r現在のステージ;
							r現在のステージ = stage演奏ドラム画面;
							foreach( STPlugin pg in this.listプラグイン )
							{
								Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
								pg.plugin.Onステージ変更();
								Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
							}

							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.演奏:
						#region [ *** ]
						//-----------------------------
						//long n1 = FDK.null.nシステム時刻ms;
						//long n2 = FDK.CSound管理.SoundDevice.n経過時間ms;
						//long n3 = FDK.CSound管理.SoundDevice.tmシステムタイマ.nシステム時刻ms;
						//long n4 = FDK.ct演奏用タイマー.db現在の値;
						//long n5 = FDK.CSound管理.SoundDevice.n経過時間を更新したシステム時刻ms;

						//swlist1.Add( Convert.ToInt32(n1) );
						//swlist2.Add( Convert.ToInt32(n2) );
						//swlist3.Add( Convert.ToInt32( n3 ) );
						//swlist4.Add( Convert.ToInt32( n4 ) );
						//swlist5.Add( Convert.ToInt32( n5 ) );

						#region [ DTXVモード中にDTXCreatorから指示を受けた場合の処理 ]
						if ( DTXVmode.Enabled && DTXVmode.Refreshed )
						{
							DTXVmode.Refreshed = false;

							if ( DTXVmode.Command == CDTXVmode.ECommand.Stop )
							{
								TJAPlayer3.stage演奏ドラム画面.t停止();
								if ( previewSound != null )
								{
									this.previewSound.Stop();
									this.previewSound.t解放する();
									this.previewSound = null;
								}
								//{
								//    int lastd = 0;
								//    int f = 0;
								//    for ( int i = 0; i < swlist1.Count; i++ )
								//    {
								//        int d1 = swlist1[ i ];
								//        int d2 = swlist2[ i ];
								//        int d3 = swlist3[ i ];
								//        int d4 = swlist4[ i ];
								//        int d5 = swlist5[ i ];

								//        int dif = d1 - lastd;
								//        string s = "";
								//        if ( 16 <= dif && dif <= 17 )
								//        {
								//        }
								//        else
								//        {
								//            s = "★";
								//        }
								//        Trace.TraceInformation( "frame {0:D4}: {1:D3} ( {2:D3}, {3:D3} - {7:D3}, {4:D3} ) {5}, n現在時刻={6}", f, dif, d1, d2, d3, s, d4, d5 );
								//        lastd = d1;
								//        f++;
								//    }
								//    swlist1.Clear();
								//    swlist2.Clear();
								//    swlist3.Clear();
								//    swlist4.Clear();
								//    swlist5.Clear();

								//}
							}
							else if ( DTXVmode.Command == CDTXVmode.ECommand.Play )
							{
								if ( DTXVmode.NeedReload )
								{
									TJAPlayer3.stage演奏ドラム画面.t再読込();

									TJAPlayer3.ConfigIni.bTimeStretch = DTXVmode.TimeStretch;
									if ( TJAPlayer3.ConfigIni.b垂直帰線待ちを行う != DTXVmode.VSyncWait )
									{
										TJAPlayer3.ConfigIni.b垂直帰線待ちを行う = DTXVmode.VSyncWait;
										TJAPlayer3.app.b次のタイミングで垂直帰線同期切り替えを行う = true;
									}
								}
								else
								{
									TJAPlayer3.stage演奏ドラム画面.t演奏位置の変更( TJAPlayer3.DTXVmode.nStartBar, 0 );
								}
							}
						}
						#endregion

						switch( this.n進行描画の戻り値 )
						{
							case (int) E演奏画面の戻り値.再読込_再演奏:
								#region [ DTXファイルを再読み込みして、再演奏 ]
								DTX.t全チップの再生停止();
								DTX.On非活性化();
								r現在のステージ.On非活性化();
								stage曲読み込み.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage曲読み込み;
								this.tガベージコレクションを実行する();
								break;
								#endregion

							//case (int) E演奏画面の戻り値.再演奏:
							#region [ 再読み込み無しで、再演奏 ]
							#endregion
							//	break;

							case (int) E演奏画面の戻り値.継続:
								break;

							case (int) E演奏画面の戻り値.演奏中断:
								#region [ 演奏キャンセル ]
								//-----------------------------
								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新( "Play canceled" );

								//int lastd = 0;
								//int f = 0;
								//for (int i = 0; i < swlist1.Count; i++)
								//{
								//    int d1 = swlist1[ i ];
								//    int d2 = swlist2[ i ];
								//    int d3 = swlist3[ i ];
								//    int d4 = swlist4[ i ];

								//    int dif = d1 - lastd;
								//    string s = "";
								//    if ( 16 <= dif && dif <= 17 )
								//    {
								//    }
								//    else
								//    {
								//        s = "★";
								//    }
								//    Trace.TraceInformation( "frame {0:D4}: {1:D3} ( {2:D3}, {3:D3}, {4:D3} ) {5}, n現在時刻={6}", f, dif, d1, d2, d3, s, d4 );
								//    lastd = d1;
								//    f++;
								//}
								//swlist1.Clear();
								//swlist2.Clear();
								//swlist3.Clear();
								//swlist4.Clear();
		
								#region [ プラグイン On演奏キャンセル() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.On演奏キャンセル( scoreIni );
									Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
								}
								//---------------------
								#endregion

								DTX.t全チップの再生停止();
								DTX.On非活性化();
								r現在のステージ.On非活性化();
								if( bコンパクトモード )
								{
								}
								else
								{
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ 選曲" );
									stage選曲.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									#region [ プラグイン Onステージ変更() の呼び出し ]
									//---------------------
									foreach( STPlugin pg in this.listプラグイン )
									{
										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
									}
									//---------------------
									#endregion

									this.tガベージコレクションを実行する();
								}
                                this.tガベージコレクションを実行する();
                                break;
								//-----------------------------
								#endregion

							case (int) E演奏画面の戻り値.ステージ失敗:
								#region [ 演奏失敗(StageFailed) ]
								//-----------------------------
								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新( "Stage failed" );

								#region [ プラグイン On演奏失敗() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.On演奏失敗( scoreIni );
									Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
								}
								//---------------------
								#endregion

								DTX.t全チップの再生停止();
								DTX.On非活性化();
								r現在のステージ.On非活性化();
								if( bコンパクトモード )
								{
								}
								else
								{
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ 選曲" );
									stage選曲.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									#region [ プラグイン Onステージ変更() の呼び出し ]
									//---------------------
									foreach( STPlugin pg in this.listプラグイン )
									{
										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
									}
									//---------------------
									#endregion

									this.tガベージコレクションを実行する();
								}
								break;
								//-----------------------------
								#endregion

							case (int) E演奏画面の戻り値.ステージクリア:
								#region [ 演奏クリア ]
								//-----------------------------
								CScoreIni.C演奏記録 c演奏記録_Drums;
								stage演奏ドラム画面.t演奏結果を格納する( out c演奏記録_Drums );

                                double ps = 0.0, gs = 0.0;
								if ( !c演奏記録_Drums.b全AUTOである && c演奏記録_Drums.n全チップ数 > 0) {
									ps = c演奏記録_Drums.db演奏型スキル値;
									gs = c演奏記録_Drums.dbゲーム型スキル値;
								}
								string str = "Cleared";
								switch( CScoreIni.t総合ランク値を計算して返す( c演奏記録_Drums, null, null ) )
								{
									case (int)CScoreIni.ERANK.SS:
										str = string.Format( "Cleared (SS: {0:F2})", ps );
										break;

									case (int) CScoreIni.ERANK.S:
										str = string.Format( "Cleared (S: {0:F2})", ps );
										break;

									case (int) CScoreIni.ERANK.A:
										str = string.Format( "Cleared (A: {0:F2})", ps );
										break;

									case (int) CScoreIni.ERANK.B:
										str = string.Format( "Cleared (B: {0:F2})", ps );
										break;

									case (int) CScoreIni.ERANK.C:
										str = string.Format( "Cleared (C: {0:F2})", ps );
										break;

									case (int) CScoreIni.ERANK.D:
										str = string.Format( "Cleared (D: {0:F2})", ps );
										break;

									case (int) CScoreIni.ERANK.E:
										str = string.Format( "Cleared (E: {0:F2})", ps );
										break;

									case (int)CScoreIni.ERANK.UNKNOWN:	// #23534 2010.10.28 yyagi add: 演奏チップが0個のとき
										str = "Cleared (No chips)";
										break;
								}

								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新( str );

								#region [ プラグイン On演奏クリア() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.On演奏クリア( scoreIni );
									Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
								}
								//---------------------
								#endregion

								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 結果" );
								stage結果.st演奏記録.Drums = c演奏記録_Drums;
								stage結果.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage結果;

								#region [ プラグイン Onステージ変更() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
								}
								//---------------------
								#endregion

								break;
								//-----------------------------
								#endregion
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.結果:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							//DTX.t全チップの再生一時停止();
                            DTX.t全チップの再生停止とミキサーからの削除();
                            DTX.On非活性化();
							r現在のステージ.On非活性化();
                            this.tガベージコレクションを実行する();
                            if ( !bコンパクトモード )
							{
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 選曲" );
								stage選曲.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;

								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
								}

								this.tガベージコレクションを実行する();
							}
							else
							{
							}
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.ChangeSkin:
						#region [ *** ]
						//-----------------------------
						if ( this.n進行描画の戻り値 != 0 )
						{
							r現在のステージ.On非活性化();
							Trace.TraceInformation( "----------------------" );
							Trace.TraceInformation( "■ 選曲" );
							stage選曲.On活性化();
							r直前のステージ = r現在のステージ;
							r現在のステージ = stage選曲;
							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.終了:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
						}
						//-----------------------------
						#endregion
						break;
				}

                // オーバレイを描画する(テクスチャの生成されていない起動ステージは例外
                if(r現在のステージ != null && r現在のステージ.eステージID != CStage.Eステージ.起動 && TJAPlayer3.Tx.Overlay != null)
                {
                    TJAPlayer3.Tx.Overlay.t2D描画(0, 0);
                }
			}
#if !GPUFlushAfterPresent
			actFlushGPU?.On進行描画();		// Flush GPU	// EndScene()～Present()間 (つまりVSync前) でFlush実行
#endif

			#region [ 全画面_ウインドウ切り替え ]
			if ( this.b次のタイミングで全画面_ウィンドウ切り替えを行う )
			{
				ConfigIni.b全画面モード = !ConfigIni.b全画面モード;
				app.t全画面_ウィンドウモード切り替え();
				this.b次のタイミングで全画面_ウィンドウ切り替えを行う = false;
			}
			#endregion
			#region [ 垂直基線同期切り替え ]
			if ( this.b次のタイミングで垂直帰線同期切り替えを行う )
			{
				
			}
				#endregion
			}
		}

		// その他

		#region [ 汎用ヘルパー ]
		//-----------------
		public static CTexture tテクスチャの生成( string fileName )
		{
			return tテクスチャの生成( fileName, false );
		}
		public static CTexture tテクスチャの生成( string fileName, bool b黒を透過する )
		{
			if ( app == null )
			{
				return null;
			}
			try
			{
				return new CTexture(fileName);
			}
			catch
			{
				Trace.TraceError( "テクスチャの生成に失敗しました。({0})", fileName );
				return null;
			}
		}
		public static void tテクスチャの解放(ref CTexture tx )
		{
			TJAPlayer3.t安全にDisposeする( ref tx );
		}
		public static CTexture tテクスチャの生成( Bitmap bitmap )
		{
			return tテクスチャの生成( bitmap, false );
		}
		public static CTexture tテクスチャの生成( Bitmap bitmap, bool b黒を透過する )
		{
			if ( app == null )
			{
				return null;
			}
            if (bitmap == null)
            {
                Trace.TraceError("テクスチャの生成に失敗しました。(bitmap==null)");
                return null;
            }
            try
			{
				return new CTexture(bitmap);
			}
			catch
			{
				Trace.TraceError( "テクスチャの生成に失敗しました。(txData)" );
				return null;
			}
		}

		/// <summary>プロパティ、インデクサには ref は使用できないので注意。</summary>
		public static void t安全にDisposeする<T>(ref T obj)
        {
            if (obj == null)
                return;

            var d = obj as IDisposable;

            if (d != null)
                d.Dispose();

            obj = default(T);
        }

        /// <summary>
        /// そのフォルダの連番画像の最大値を返す。
        /// </summary>
        public static int t連番画像の枚数を数える(string ディレクトリ名, string プレフィックス = "", string 拡張子 = ".png")
        {
            int num = 0;
            while(File.Exists(ディレクトリ名 + プレフィックス + num + 拡張子))
            {
                num++;
            }
            return num;
        }

        /// <summary>
        /// 曲名テクスチャの縮小倍率を返す。
        /// </summary>
        /// <param name="cTexture">曲名テクスチャ。</param>
        /// <param name="samePixel">等倍で表示するピクセル数の最大値(デフォルト値:645)</param>
        /// <returns>曲名テクスチャの縮小倍率。そのテクスチャがnullならば一倍(1f)を返す。</returns>
        public static float GetSongNameXScaling(ref CTexture cTexture, int samePixel = 660)
        {
            if (cTexture == null) return 1f;
            float scalingRate = (float)samePixel / (float)cTexture.szテクスチャサイズ.Width;
            if (cTexture.szテクスチャサイズ.Width <= samePixel)
                scalingRate = 1.0f;
            return scalingRate;
        }

        /// <summary>
        /// 難易度を表す数字を列挙体に変換します。
        /// </summary>
        /// <param name="number">難易度を表す数字。</param>
        /// <returns>Difficulty 列挙体</returns>
        public static Difficulty DifficultyNumberToEnum(int number)
        {
            switch (number)
            {
                case 0:
                    return Difficulty.Easy;
                case 1:
                    return Difficulty.Normal;
                case 2:
                    return Difficulty.Hard;
                case 3:
                    return Difficulty.Oni;
                case 4:
                    return Difficulty.Edit;
                case 5:
                    return Difficulty.Tower;
                case 6:
                    return Difficulty.Dan;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        //-----------------
        #endregion

        #region [ private ]
        //-----------------
        private bool bマウスカーソル表示中 = true;
		private bool b終了処理完了済み;
		private static CDTX[] dtx = new CDTX[ 4 ];

        public static TextureLoader Tx = new TextureLoader();

		private List<CActivity> listトップレベルActivities;
		private int n進行描画の戻り値;
		private MouseButtons mb = System.Windows.Forms.MouseButtons.Left;
		private string strWindowTitle
		{
			get
			{
				if ( DTXVmode.Enabled )
				{
					return "DTXViewer release " + VERSION;
				}
				else
				{
					return "TJAPlayer3 feat.DTXMania";
				}
			}
		}
		private CSound previewSound;
        public static long StartupTime
        {
            get;
            private set;
        }

		/// <summary>
		/// 2021.2.14.akasoko
		/// </summary>
		public void LoadConigini()
		{
			#region [ Config.ini の読込み ]
			//---------------------
			ConfigIni = new CConfigIni();
			string path = strEXEのあるフォルダ + "Config.ini";
			if (File.Exists(path))
			{
				try
				{
					ConfigIni.tファイルから読み込み(path);
				}
				catch (Exception e)
				{
					//ConfigIni = new CConfigIni();	// 存在してなければ新規生成
					Trace.TraceError(e.ToString());
					Trace.TraceError("例外が発生しましたが処理を継続します。 (b8d93255-bbe4-4ca3-8264-7ee5175b19f3)");
				}
			}
			//---------------------
			#endregion
		}
		public void t起動処理()
		{
			#region [ strEXEのあるフォルダを決定する ]
			//-----------------
// BEGIN #23629 2010.11.13 from: デバッグ時は Application.ExecutablePath が ($SolutionDir)/bin/x86/Debug/ などになり System/ の読み込みに失敗するので、カレントディレクトリを採用する。（プロジェクトのプロパティ→デバッグ→作業ディレクトリが有効になる）
#if DEBUG
			strEXEのあるフォルダ = Environment.CurrentDirectory + @"\";
#else
			strEXEのあるフォルダ = Path.GetDirectoryName( Application.ExecutablePath ) + @"\";	// #23629 2010.11.9 yyagi: set correct pathname where DTXManiaGR.exe is.
#endif
			// END #23629 2010.11.13 from
			//-----------------
			#endregion

			LoadConigini();
			#region [ ログ出力開始 ]
			//---------------------
			Trace.AutoFlush = true;
			if( ConfigIni.bログ出力 )
			{
				try
				{
					Trace.Listeners.Add( new CTraceLogListener( new StreamWriter( System.IO.Path.Combine( strEXEのあるフォルダ, "TJAPlayer3.log" ), false, Encoding.GetEncoding( "Shift_JIS" ) ) ) );
				}
				catch ( System.UnauthorizedAccessException )			// #24481 2011.2.20 yyagi
				{
					int c = (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja")? 0 : 1;
					string[] mes_writeErr = {
						"DTXManiaLog.txtへの書き込みができませんでした。書き込みできるようにしてから、再度起動してください。",
						"Failed to write DTXManiaLog.txt. Please set it writable and try again."
					};
					MessageBox.Show( mes_writeErr[c], "DTXMania boot error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					Environment.Exit(1);
				}
			}
			Trace.WriteLine("");
			Trace.WriteLine( "DTXMania powered by YAMAHA Silent Session Drums" );
			Trace.WriteLine( string.Format( "Release: {0}", VERSION ) );
			Trace.WriteLine( "" );
			Trace.TraceInformation( "----------------------" );
			Trace.TraceInformation( "■ アプリケーションの初期化" );
			Trace.TraceInformation( "OS Version: " + Environment.OSVersion );
			Trace.TraceInformation( "ProcessorCount: " + Environment.ProcessorCount.ToString() );
			Trace.TraceInformation( "CLR Version: " + Environment.Version.ToString() );
			//---------------------
			#endregion
			#region [ DTXVmodeクラス の初期化 ]
			//---------------------
			//Trace.TraceInformation( "DTXVモードの初期化を行います。" );
			//Trace.Indent();
			try
			{
				DTXVmode = new CDTXVmode();
				DTXVmode.Enabled = false;
				//Trace.TraceInformation( "DTXVモードの初期化を完了しました。" );
			}
			finally
			{
				//Trace.Unindent();
			}
			//---------------------
			#endregion


			#region [ ウィンドウ初期化 ]
			//---------------------
			DX.SetOutApplicationLogValidFlag(DX.FALSE); //ログ出力するか

			DX.ChangeWindowMode(DX.TRUE);  //ウィンドウモード切替
			DX.SetGraphMode(1280, 720, 32); //ウィンドウサイズ決める
			DX.SetMainWindowText("TJAPlayer3-FDX"); //ソフト名決める
			DX.SetWindowStyleMode(7); //画面最大化できるようにする
			DX.SetWindowSizeChangeEnableFlag(DX.TRUE); //ウィンドウサイズ変えれるようにする
			DX.SetAlwaysRunFlag(DX.TRUE); //ソフトがアクティブじゃなくても処理続行するようにする
			DX.SetWindowSizeExtendRate(1f); //起動時のウィンドウサイズを設定 ( 1 = 100%)
			DX.SetUseMaskScreenFlag(DX.TRUE); //書かなくても良い。マスク使うときだけ書こう
			DX.SetWaitVSyncFlag(DX.FALSE);
			// 2021.2.10ここでDX関数を呼び出しちゃう
			switch (TJAPlayer3.ConfigIni.nSoundDeviceType)
			{
				case 0: DX.SetEnableWASAPIFlag(DX.FALSE); break;
				case 1: DX.SetEnableASIOFlag(DX.TRUE); break;
				case 2: DX.SetEnableWASAPIFlag(DX.TRUE, DX.TRUE); break;
				case 3: DX.SetEnableWASAPIFlag(DX.TRUE, DX.FALSE); break;
				default:
					goto case 3;
			}
			if (DX.DxLib_Init() < 0) return;
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			DX.SetTransColor(221, 12, 1);
			DX.SetBackgroundColor(0, 0, 0);
			#endregion

			DTX = null;

			#region [ Skin の初期化 ]
			//---------------------
			Trace.TraceInformation( "スキンの初期化を行います。" );
			Trace.Indent();
			try
			{
				Skin = new CSkin( TJAPlayer3.ConfigIni.strSystemSkinSubfolderFullName, false);
				TJAPlayer3.ConfigIni.strSystemSkinSubfolderFullName = TJAPlayer3.Skin.GetCurrentSkinSubfolderFullName( true );	// 旧指定のSkinフォルダが消滅していた場合に備える
				Trace.TraceInformation( "スキンの初期化を完了しました。" );
			}
			catch (Exception e)
			{
				Trace.TraceInformation( "スキンの初期化に失敗しました。" );
				throw;
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			//-----------
			//-----------

			#region [ FPS カウンタの初期化 ]
			//---------------------
			Trace.TraceInformation( "FPSカウンタの初期化を行います。" );
			Trace.Indent();
			try
			{
				FPS = new CFPS();
				Trace.TraceInformation( "FPSカウンタを生成しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ act文字コンソールの初期化 ]
			//---------------------
			Trace.TraceInformation( "文字コンソールの初期化を行います。" );
			Trace.Indent();
			try
			{
				act文字コンソール = new C文字コンソール();
				Trace.TraceInformation( "文字コンソールを生成しました。" );
				act文字コンソール.On活性化();
				Trace.TraceInformation( "文字コンソールを活性化しました。" );
				Trace.TraceInformation( "文字コンソールの初期化を完了しました。" );
			}
			catch( Exception exception )
			{
				Trace.TraceError( exception.ToString() );
				Trace.TraceError( "文字コンソールの初期化に失敗しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Input管理 の初期化 ]
			//---------------------
			Trace.TraceInformation( "DirectInput, MIDI入力の初期化を行います。" );
			Trace.Indent();
			try
			{
				bool bUseMIDIIn = !DTXVmode.Enabled;
				Input管理 = new CInput管理();
				Trace.TraceInformation( "DirectInput の初期化を完了しました。" );
			}
			catch( Exception exception2 )
			{
				Trace.TraceError( "DirectInput, MIDI入力の初期化に失敗しました。" );
				throw;
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			
			#region [ Songs管理 の初期化 ]
			//---------------------
			Trace.TraceInformation( "曲リストの初期化を行います。" );
			Trace.Indent();
			try
			{
				Songs管理 = new CSongs管理();
//				Songs管理_裏読 = new CSongs管理();
				EnumSongs = new CEnumSongs();
				actEnumSongs = new CActEnumSongs();
				Trace.TraceInformation( "曲リストの初期化を完了しました。" );
			}
			catch( Exception e )
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "曲リストの初期化に失敗しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Random の初期化 ]
			//---------------------
			Random = new Random( (int)DX.GetNowCount());
			//---------------------
			#endregion
			#region [ ステージの初期化 ]
			//---------------------
			r現在のステージ = null;
			r直前のステージ = null;
			stage起動 = new CStage起動();
			stageタイトル = new CStageタイトル();
//			stageオプション = new CStageオプション();
			stageコンフィグ = new CStageコンフィグ();
			stage選曲 = new CStage選曲();
			stage曲読み込み = new CStage曲読み込み();
			stage演奏ドラム画面 = new CStage演奏ドラム画面();
			stage結果 = new CStage結果();
			stageChangeSkin = new CStageChangeSkin();
			stage終了 = new CStage終了();
			this.listトップレベルActivities = new List<CActivity>();
			this.listトップレベルActivities.Add( actEnumSongs );
			this.listトップレベルActivities.Add( act文字コンソール );
			this.listトップレベルActivities.Add( stage起動 );
			this.listトップレベルActivities.Add( stageタイトル );
//			this.listトップレベルActivities.Add( stageオプション );
			this.listトップレベルActivities.Add( stageコンフィグ );
			this.listトップレベルActivities.Add( stage選曲 );
			this.listトップレベルActivities.Add( stage曲読み込み );
			this.listトップレベルActivities.Add( stage演奏ドラム画面 );
			this.listトップレベルActivities.Add( stage結果 );
			this.listトップレベルActivities.Add( stageChangeSkin );
			this.listトップレベルActivities.Add( stage終了 );
			this.listトップレベルActivities.Add( actFlushGPU );
			//---------------------
			#endregion
			#region [ プラグインの検索と生成 ]
			//---------------------
			PluginHost = new CPluginHost();

			Trace.TraceInformation( "プラグインの検索と生成を行います。" );
			Trace.Indent();
			try
			{
				this.tプラグイン検索と生成();
				Trace.TraceInformation( "プラグインの検索と生成を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ プラグインの初期化 ]
			//---------------------
			if( this.listプラグイン != null && this.listプラグイン.Count > 0 )
			{
				Trace.TraceInformation( "プラグインの初期化を行います。" );
				Trace.Indent();
				try
				{
					foreach( STPlugin st in this.listプラグイン )
					{
						Directory.SetCurrentDirectory( st.strプラグインフォルダ );
						st.plugin.On初期化((IPluginHost)this.PluginHost );
						st.plugin.OnManagedリソースの作成();
						st.plugin.OnUnmanagedリソースの作成();
						Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
					}
					Trace.TraceInformation( "すべてのプラグインの初期化を完了しました。" );
				}
				catch
				{
					Trace.TraceError( "プラグインのどれかの初期化に失敗しました。" );
					throw;
				}
				finally
				{
					Trace.Unindent();
				}
			}

            //---------------------
            #endregion

            #region Discordの処理
            Discord.Initialize("428233983025741855");
            StartupTime = Discord.GetUnixTime();
            Discord.UpdatePresence("", Properties.Discord.Stage_StartUp, StartupTime);
            #endregion


            Trace.TraceInformation( "アプリケーションの初期化を完了しました。" );


            #region [ 最初のステージの起動 ]
            //---------------------
            Trace.TraceInformation( "----------------------" );
			Trace.TraceInformation( "■ 起動" );

			if ( TJAPlayer3.bコンパクトモード )
			{
				r現在のステージ = stage曲読み込み;
			}
			else
			{
				r現在のステージ = stage起動;
			}
			r現在のステージ.On活性化();

			//---------------------
			#endregion
		}

		public void ShowWindowTitleWithSoundType()
		{
			
		}

		public void t終了処理()
		{
			DX.DxLib_End();
		}
		private CScoreIni tScoreIniへBGMAdjustとHistoryとPlayCountを更新(string str新ヒストリ行)
		{
			bool bIsUpdatedDrums, bIsUpdatedGuitar, bIsUpdatedBass;
			string strFilename = DTX.strファイル名の絶対パス + ".score.ini";
			CScoreIni ini = new CScoreIni( strFilename );
			if( !File.Exists( strFilename ) )
			{
				ini.stファイル.Title = DTX.TITLE;
				ini.stファイル.Name = DTX.strファイル名;
				ini.stファイル.Hash = CScoreIni.tファイルのMD5を求めて返す( DTX.strファイル名の絶対パス );
				for( int i = 0; i < 6; i++ )
				{
					ini.stセクション[ i ].nPerfectになる範囲ms = nPerfect範囲ms;
					ini.stセクション[ i ].nGreatになる範囲ms = nGreat範囲ms;
					ini.stセクション[ i ].nGoodになる範囲ms = nGood範囲ms;
					ini.stセクション[ i ].nPoorになる範囲ms = nPoor範囲ms;
				}
			}
			ini.stファイル.BGMAdjust = DTX.nBGMAdjust;
			CScoreIni.t更新条件を取得する( out bIsUpdatedDrums, out bIsUpdatedGuitar, out bIsUpdatedBass );
			if( bIsUpdatedDrums || bIsUpdatedGuitar || bIsUpdatedBass )
			{
				if( bIsUpdatedDrums )
				{
					ini.stファイル.PlayCountDrums++;
				}
				if( bIsUpdatedGuitar )
				{
					ini.stファイル.PlayCountGuitar++;
				}
				if( bIsUpdatedBass )
				{
					ini.stファイル.PlayCountBass++;
				}
				ini.tヒストリを追加する( str新ヒストリ行 );
				if( !bコンパクトモード )
				{
					stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Drums = ini.stファイル.PlayCountDrums;
					stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Guitar = ini.stファイル.PlayCountGuitar;
					stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Bass = ini.stファイル.PlayCountBass;
					for( int j = 0; j < ini.stファイル.History.Length; j++ )
					{
						stage選曲.r現在選択中のスコア.譜面情報.演奏履歴[ j ] = ini.stファイル.History[ j ];
					}
				}
			}
			if( ConfigIni.bScoreIniを出力する )
			{
				ini.t書き出し( strFilename );
			}

			return ini;
		}
		private void tガベージコレクションを実行する()
		{
			GC.Collect(GC.MaxGeneration);
			GC.WaitForPendingFinalizers();
			GC.Collect(GC.MaxGeneration);
		}
		private void tプラグイン検索と生成()
		{
			this.listプラグイン = new List<STPlugin>();

			string strIPluginActivityの名前 = typeof( IPluginActivity ).FullName;
			string strプラグインフォルダパス = strEXEのあるフォルダ + "Plugins\\";

			this.t指定フォルダ内でのプラグイン検索と生成( strプラグインフォルダパス, strIPluginActivityの名前 );

			if( this.listプラグイン.Count > 0 )
				Trace.TraceInformation( this.listプラグイン.Count + " 個のプラグインを読み込みました。" );
		}

        public void RefleshSkin()
        {
            Trace.TraceInformation("スキン変更:" + TJAPlayer3.Skin.GetCurrentSkinSubfolderFullName(false));

            TJAPlayer3.act文字コンソール.On非活性化();

            TJAPlayer3.Skin.Dispose();
            TJAPlayer3.Skin = null;
            TJAPlayer3.Skin = new CSkin(TJAPlayer3.ConfigIni.strSystemSkinSubfolderFullName, false);


            TJAPlayer3.Tx.DisposeTexture();
            TJAPlayer3.Tx.LoadTexture();

            TJAPlayer3.act文字コンソール.On活性化();
        }
		#region [ Windowイベント処理 ]
		private void t指定フォルダ内でのプラグイン検索と生成( string strプラグインフォルダパス, string strプラグイン型名 )
		{
			// 指定されたパスが存在しないとエラー
			if( !Directory.Exists( strプラグインフォルダパス ) )
			{
				Trace.TraceWarning( "プラグインフォルダが存在しません。(" + strプラグインフォルダパス + ")" );
				return;
			}

			// (1) すべての *.dll について…
			string[] strDLLs = System.IO.Directory.GetFiles( strプラグインフォルダパス, "*.dll" );
			foreach( string dllName in strDLLs )
			{
				try
				{
					// (1-1) dll をアセンブリとして読み込む。
					System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFrom( dllName );

					// (1-2) アセンブリ内のすべての型について、プラグインとして有効か調べる
					foreach( Type t in asm.GetTypes() )
					{
						//  (1-3) ↓クラスであり↓Publicであり↓抽象クラスでなく↓IPlugin型のインスタンスが作れる　型を持っていれば有効
						if( t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface( strプラグイン型名 ) != null )
						{
							// (1-4) クラス名からインスタンスを作成する
							var st = new STPlugin() {
								plugin = (IPluginActivity) asm.CreateInstance( t.FullName ),
								strプラグインフォルダ = Path.GetDirectoryName( dllName ),
								strアセンブリ簡易名 = asm.GetName().Name,
								Version = asm.GetName().Version,
							};

							// (1-5) プラグインリストへ登録
							this.listプラグイン.Add( st );
							Trace.TraceInformation( "プラグイン {0} ({1}, {2}, {3}) を読み込みました。", t.FullName, Path.GetFileName( dllName ), st.strアセンブリ簡易名, st.Version.ToString() );
						}
					}
				}
				catch (Exception e)
				{
					Trace.TraceError( e.ToString() );
					Trace.TraceInformation( dllName + " からプラグインを生成することに失敗しました。スキップします。" );
				}
			}

			// (2) サブフォルダがあれば再帰する
			string[] strDirs = Directory.GetDirectories( strプラグインフォルダパス, "*" );
			foreach( string dir in strDirs )
				this.t指定フォルダ内でのプラグイン検索と生成( dir + "\\", strプラグイン型名 );
		}
		//-----------------
		private void Window_ApplicationActivated( object sender, EventArgs e )
		{
			this.bApplicationActive = true;
		}
		private void Window_ApplicationDeactivated( object sender, EventArgs e )
		{
			this.bApplicationActive = false;
		}
		private void Window_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.KeyCode == Keys.Menu )
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else if ( ( e.KeyCode == Keys.Return ) && e.Alt )
			{
				if ( ConfigIni != null )
				{
					ConfigIni.bウィンドウモード = !ConfigIni.bウィンドウモード;
					this.t全画面_ウィンドウモード切り替え();
				}
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else
			{
				for ( int i = 0; i < 0x10; i++ )
				{
				
				}
			}
		}
		private void Window_MouseUp( object sender, MouseEventArgs e )
		{
			mb = e.Button;
		}

		private void Window_MouseDoubleClick( object sender, MouseEventArgs e)	// #23510 2010.11.13 yyagi: to go full screen mode
		{
			if ( mb.Equals(MouseButtons.Left) && ConfigIni.bIsAllowedDoubleClickFullscreen )	// #26752 2011.11.27 yyagi
			{
				ConfigIni.bウィンドウモード = false;
				this.t全画面_ウィンドウモード切り替え();
			}
		}
		private void Window_ResizeEnd(object sender, EventArgs e)				// #23510 2010.11.20 yyagi: to get resized window size
		{
		}

        public void Dispose()
        {
           
        }
        #endregion
        #endregion
    }
}
