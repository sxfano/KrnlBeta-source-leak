using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using KrnlUI.Controls;
using Microsoft.Win32;

namespace KrnlUI
{
	// Token: 0x02000005 RID: 5
	public class MainWindow : Window, IComponentConnector
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000C RID: 12 RVA: 0x0000212C File Offset: 0x0000032C
		// (set) Token: 0x0600000D RID: 13 RVA: 0x00002134 File Offset: 0x00000334
		public ChromiumWebBrowser browser { get; set; }

		// Token: 0x0600000E RID: 14 RVA: 0x00002140 File Offset: 0x00000340
		public void WriteScript(string script, bool tabPrompt)
		{
			this.TabChanging = tabPrompt;
			if (this.browser.IsLoaded)
			{
				this.browser.EvaluateScriptAsync("SetText(`" + script.Replace("`", "\\`").Replace("\\", "\\\\") + "`)", null, false);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021A8 File Offset: 0x000003A8
		public string ReadScript()
		{
			if (!this.browser.IsLoaded)
			{
				return "";
			}
			string text = this.browser.EvaluateScriptAsync("(function() { return GetText() })();", null, false).GetAwaiter().GetResult().Result.ToString();
			if (text != null)
			{
				return text;
			}
			return "";
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002204 File Offset: 0x00000404
		public MainWindow()
		{
			this.InitializeComponent();
			this.MainMenu.Visibility = Visibility.Hidden;
			this.InitBrowser();
			this.Initialize();
			this.InitHotkeys();
			this.InitTabs();
			this.LoadCommunity();
			this.MainDirDisplay.isFile = true;
			this.MainDirDisplay.Path = "Scripts";
			this.MainMenu.Margin = new Thickness(0.0, 37.0, 0.0, 0.0);
			this.InitRecents();
			this.ActivateRecent();
			this.AutoAttach();
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
			if (registryKey.OpenSubKey("Krnl") == null)
			{
				RegistryKey registryKey2 = registryKey.CreateSubKey("Krnl", true);
				registryKey2.SetValue("AutoAttach", false);
				registryKey2.SetValue("AutoLaunch", false);
				registryKey2.SetValue("Topmost", false);
				registryKey2.SetValue("UnlockFPS", false);
			}
			RegistryKey krnlSubkey = this.getKrnlSubkey();
			object value = krnlSubkey.GetValue("Topmost");
			object value2 = krnlSubkey.GetValue("AutoAttach");
			object value3 = krnlSubkey.GetValue("UnlockFPS");
			if (value != null && value.ToString() == "true")
			{
				this.TopmostOpt_MouseLeftButtonUp(null, null);
			}
			if (value2 != null && value2.ToString() == "true")
			{
				this.AutoAttachOpt_MouseLeftButtonUp(null, null);
			}
			if (value3 != null && value3.ToString() == "true")
			{
				this.UnlockFPSOpt_MouseLeftButtonUp(null, null);
			}
			foreach (Process process in Process.GetProcessesByName("KrnlUI"))
			{
				if (Process.GetCurrentProcess().Id != process.Id)
				{
					process.Kill();
				}
			}
			this.BringDownMenu();
			this.MenuDown = true;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000024B0 File Offset: 0x000006B0
		private void InitTabs()
		{
			if (!Directory.Exists("Data"))
			{
				Directory.CreateDirectory("Data");
				return;
			}
			if (!Directory.Exists("Data\\SavedTabs"))
			{
				Directory.CreateDirectory("Data\\SavedTabs");
				return;
			}
			if (File.Exists("Data\\SavedTabs\\tabs.config"))
			{
				foreach (string str in File.ReadAllLines("Data\\SavedTabs\\tabs.config"))
				{
					string text = "Data\\SavedTabs\\" + str;
					if (Directory.Exists(text))
					{
						this.CreateTab(System.IO.Path.GetFileName(text), File.ReadAllText(text + "\\script.lua"), File.ReadAllText(text + "\\tab.config"));
						Directory.Delete(text, true);
					}
				}
				File.Delete("Data\\SavedTabs\\tabs.config");
			}
			if (this.TabFlow.Children.Count == 1)
			{
				this.CreateTab("Untitled", "", "");
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002594 File Offset: 0x00000794
		private void InitHotkeys()
		{
			RoutedCommand routedCommand = new RoutedCommand();
			routedCommand.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
			RoutedCommand routedCommand2 = new RoutedCommand();
			routedCommand2.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
			RoutedCommand routedCommand3 = new RoutedCommand();
			routedCommand3.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
			RoutedCommand routedCommand4 = new RoutedCommand();
			routedCommand4.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
			base.CommandBindings.Add(new CommandBinding(routedCommand, new ExecutedRoutedEventHandler(this.CreateTabHotkey)));
			base.CommandBindings.Add(new CommandBinding(routedCommand2, new ExecutedRoutedEventHandler(this.OpenHotkey)));
			base.CommandBindings.Add(new CommandBinding(routedCommand3, new ExecutedRoutedEventHandler(this.SaveHotkey)));
			base.CommandBindings.Add(new CommandBinding(routedCommand4, new ExecutedRoutedEventHandler(this.SaveAsHotkey)));
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002681 File Offset: 0x00000881
		private void CreateTabHotkey(object sender, ExecutedRoutedEventArgs e)
		{
			this.CreateTab("", "", "").Select();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000269D File Offset: 0x0000089D
		private void OpenHotkey(object sender, ExecutedRoutedEventArgs e)
		{
			this.PromptOpenFile();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000026A5 File Offset: 0x000008A5
		private void SaveHotkey(object sender, ExecutedRoutedEventArgs e)
		{
			this.PromptSaveFile();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000026AD File Offset: 0x000008AD
		private void SaveAsHotkey(object sender, ExecutedRoutedEventArgs e)
		{
			this.PromptSaveAsFile();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000026B8 File Offset: 0x000008B8
		private void AutoAttach()
		{
			MainWindow.<AutoAttach>d__21 <AutoAttach>d__;
			<AutoAttach>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AutoAttach>d__.<>4__this = this;
			<AutoAttach>d__.<>1__state = -1;
			<AutoAttach>d__.<>t__builder.Start<MainWindow.<AutoAttach>d__21>(ref <AutoAttach>d__);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000026F0 File Offset: 0x000008F0
		private void LoadCommunity()
		{
			if (Directory.Exists("Community"))
			{
				foreach (string text in Directory.GetDirectories("Community"))
				{
					string str = AppDomain.CurrentDomain.BaseDirectory + "\\" + text;
					CommunityEntry communityEntry = new CommunityEntry();
					communityEntry.EntryPreview.Source = new BitmapImage(new Uri(str + "\\preview.png"));
					communityEntry.EntryCreatorIcon.ImageSource = new BitmapImage(new Uri(str + "\\profile.png"));
					communityEntry.EntryName.Content = text.Split(new char[]
					{
						'\\'
					})[1];
					communityEntry.EntryCreator.Content = File.ReadAllText(str + "\\card.config");
					communityEntry.Script = File.ReadAllText(str + "\\script.lua");
					if (File.Exists(str + "\\tags.config"))
					{
						communityEntry.Tags = File.ReadAllText(str + "\\tags.config").Split(new char[]
						{
							','
						}).ToList<string>();
					}
					communityEntry.MouseRightButtonUp += this.CardHolder_MouseRightButtonUp;
					communityEntry.RunBorder.MouseLeftButtonUp += this.CommunityCard_MouseLeftButtonUp;
					communityEntry.Width = 242.0;
					communityEntry.Height = 185.0;
					this.CommunityCards.Add(communityEntry);
				}
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002878 File Offset: 0x00000A78
		private void Initialize()
		{
			if (!Directory.Exists("Scripts"))
			{
				Directory.CreateDirectory("Scripts");
			}
			if (!Directory.Exists("Recent"))
			{
				Directory.CreateDirectory("Recent");
			}
			else
			{
				new DirectoryInfo("Recent").Delete(true);
				Directory.CreateDirectory("Recent");
			}
			this.Introduct.Content = "Welcome " + Environment.UserName + "!";
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000028F0 File Offset: 0x00000AF0
		private void InitBrowser()
		{
			CefSettings cefSettings = new CefSettings();
			cefSettings.SetOffScreenRenderingBestPerformanceArgs();
			cefSettings.MultiThreadedMessageLoop = true;
			cefSettings.DisableGpuAcceleration();
			Cef.Initialize(cefSettings);
			string currentDirectory = Directory.GetCurrentDirectory();
			this.browser = new ChromiumWebBrowser(currentDirectory + "\\Monaco\\Monaco.html");
			this.browser.BrowserSettings.WindowlessFrameRate = 144;
			this.browser.IsBrowserInitializedChanged += this.Browser_IsBrowserInitializedChanged;
			this.browser.JavascriptMessageReceived += this.Browser_JavascriptMessageReceived;
			this.browser.AllowDrop = false;
			this.Editor.Children.Add(this.browser);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000299D File Offset: 0x00000B9D
		private void Browser_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
		{
			if (!this.TabChanging)
			{
				base.Dispatcher.Invoke(delegate()
				{
					Common.SelectedTab.IsSaved = false;
				});
			}
			this.TabChanging = false;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000029D8 File Offset: 0x00000BD8
		private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000029DA File Offset: 0x00000BDA
		private void Button_MouseDown(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000029DC File Offset: 0x00000BDC
		private void ExitButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			base.Hide();
			this.Window_Closing(null, null);
			this.disable_auto_launch();
			Cef.Shutdown();
			Environment.Exit(1);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000029FD File Offset: 0x00000BFD
		private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			base.WindowState = WindowState.Minimized;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002A08 File Offset: 0x00000C08
		private void MaximizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (base.WindowState == WindowState.Maximized)
			{
				base.WindowState = WindowState.Normal;
				this.svg356.Visibility = Visibility.Visible;
				this.svg223.Visibility = Visibility.Hidden;
				base.BorderThickness = new Thickness(0.0);
				return;
			}
			base.WindowState = WindowState.Maximized;
			this.svg356.Visibility = Visibility.Hidden;
			this.svg223.Visibility = Visibility.Visible;
			base.BorderThickness = new Thickness(7.0);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002A85 File Offset: 0x00000C85
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002A8D File Offset: 0x00000C8D
		private Grid selectedTab { get; set; }

		// Token: 0x06000023 RID: 35 RVA: 0x00002A96 File Offset: 0x00000C96
		private void NewTabButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.CreateTab("", "", "").Select();
			this.MyScrollViewer.ScrollToRightEnd();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002AC0 File Offset: 0x00000CC0
		private string DynamicTabName()
		{
			string text = "Untitled ";
			List<int> list = new List<int>();
			foreach (object obj in this.TabFlow.Children)
			{
				Tab tab = ((UIElement)obj) as Tab;
				if (tab != null && tab.TabName.StartsWith(text))
				{
					list.Add(tab.DefaultedNameTabNr);
				}
			}
			list.Sort();
			return text + Enumerable.Range(1, list.Count + 1).ToList<int>().Except(list).ToList<int>()[0].ToString();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002B84 File Offset: 0x00000D84
		private Tab CreateTab(string Name, string Content, string path = "")
		{
			this.DragAvailable = false;
			Tab tab = new Tab(this.TabFlow, this);
			tab.TabName = ((Name == "") ? this.DynamicTabName() : Name);
			tab.Script = Content;
			tab.Path = path;
			this.TabFlow.Children.Insert(this.TabFlow.Children.Count - 1, tab);
			this.MyScrollViewer.ScrollToRightEnd();
			this.DragAvailable = true;
			return tab;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002C08 File Offset: 0x00000E08
		private void Tab_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Grid grid = (Grid)sender;
			if (this.selectedTab != grid)
			{
				grid.Background = new SolidColorBrush(Color.FromRgb(44, 44, 44));
				((Label)grid.Children[0]).Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
				this.selectedTab.Background = new SolidColorBrush(Color.FromRgb(34, 34, 34));
				((Label)this.selectedTab.Children[0]).Foreground = new SolidColorBrush(Color.FromRgb(122, 122, 122));
				this.selectedTab = grid;
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002CBC File Offset: 0x00000EBC
		private void KrnlWindow_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Point position = Mouse.GetPosition(this.ManagedWrapper);
			if (this.FileContextMenu.Margin.Left >= position.X || this.FileContextMenu.Margin.Left + this.FileContextMenu.Width <= position.X || this.FileContextMenu.Margin.Top >= position.Y || this.FileContextMenu.Margin.Top + this.FileContextMenu.Height <= position.Y)
			{
				this.FileContextMenu.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002D69 File Offset: 0x00000F69
		private void appIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002D6B File Offset: 0x00000F6B
		private void appIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.MenuDown)
			{
				this.BringUpMenu(false);
				this.MenuDown = false;
				return;
			}
			this.BringDownMenu();
			this.MenuDown = true;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002D91 File Offset: 0x00000F91
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00002D99 File Offset: 0x00000F99
		public Tab LastTab { get; set; }

		// Token: 0x0600002C RID: 44 RVA: 0x00002DA4 File Offset: 0x00000FA4
		public void BringDownMenu()
		{
			this.LastTab = Common.SelectedTab;
			if (Common.SelectedTab != null)
			{
				Common.SelectedTab.Script = this.ReadScript();
				Common.SelectedTab.Deselect();
			}
			this.MainMenu.Visibility = Visibility.Visible;
			this.appIconAnimDown.To = new Color?(Color.FromRgb(48, 48, 48));
			this.appIconAnimUp.To = new Color?(Color.FromRgb(44, 44, 44));
			this.appIconAnimEnterBack.To = new Color?(Color.FromRgb(44, 44, 44));
			this.appIconAnimLeaveBack.To = new Color?(Color.FromRgb(44, 44, 44));
			base.BeginStoryboard(this.appIconAnimDownSB);
			base.BeginStoryboard(this.appIconAnimUpSB);
			base.BeginStoryboard(this.appIconAnimEnterBackSB);
			base.BeginStoryboard(this.appIconAnimLeaveBackSB);
			if (this.MainDirDisplay.EntryName.Content != "Community")
			{
				if (this.MainDirDisplay.Name == "MainDirDisplay")
				{
					this.CurrentDraftPath = "Scripts";
					this.LayDirPath("");
					this.MainDirDisplay.isFile = true;
					if (this.MainDirDisplay.EntryName.Content == "Scripts")
					{
						this.InitDrafts(this.MainDirDisplay.Path);
					}
					else if (this.MainDirDisplay.EntryName.Content == "Recent")
					{
						this.InitRecents();
					}
				}
				else
				{
					this.CurrentDraftPath = this.MainDirDisplay.Path;
					this.LayDirPath(this.MainDirDisplay.Path);
					this.InitDrafts(this.MainDirDisplay.Path);
				}
			}
			this.MenuDown = true;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002F5C File Offset: 0x0000115C
		public void BringUpMenu(bool isTabCaller)
		{
			if (this.TabFlow.Children.Count > 1)
			{
				if (!isTabCaller)
				{
					if (Common.PreviousTab != null)
					{
						Common.PreviousTab.Select();
					}
					else
					{
						((Tab)this.TabFlow.Children[0]).Select();
					}
				}
				this.MainMenu.Visibility = Visibility.Hidden;
				this.appIconAnimDown.To = new Color?(Color.FromRgb(48, 48, 48));
				this.appIconAnimUp.To = new Color?(Color.FromRgb(39, 39, 39));
				this.appIconAnimEnterBack.To = new Color?(Color.FromRgb(39, 39, 39));
				this.appIconAnimLeaveBack.To = new Color?(Color.FromRgb(34, 34, 34));
				base.BeginStoryboard(this.appIconAnimDownSB);
				base.BeginStoryboard(this.appIconAnimUpSB);
				base.BeginStoryboard(this.appIconAnimEnterBackSB);
				base.BeginStoryboard(this.appIconAnimLeaveBackSB);
				this.MenuDown = false;
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000305F File Offset: 0x0000125F
		private void appIcon_MouseEnter(object sender, MouseEventArgs e)
		{
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003061 File Offset: 0x00001261
		private void appIcon_MouseLeave(object sender, MouseEventArgs e)
		{
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003063 File Offset: 0x00001263
		private void MovableForm_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.DragAvailable)
			{
				base.DragMove();
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003073 File Offset: 0x00001273
		private void injectOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (Pipes.PipeActive())
			{
				this.DisplayNotification("Already injected");
				return;
			}
			if (Process.GetProcessesByName("RobloxPlayerBeta").Length != 0)
			{
				this.Inject();
				return;
			}
			this.DisplayNotification("No Roblox process found");
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000030A8 File Offset: 0x000012A8
		private void Inject()
		{
			if (File.Exists(this.DllPath) && File.Exists(Directory.GetCurrentDirectory() + string.Format("\\\\{0}", "injector.dll")))
			{
				this.DisplayNotification("Injecting");
				Task.Run(delegate()
				{
					Injector.inject_status status = Injector.inject(this.DllPath);
					base.Dispatcher.Invoke(delegate()
					{
						switch (status)
						{
						case Injector.inject_status.failure:
							this.DisplayNotification("Failed to inject :(");
							return;
						case Injector.inject_status.success:
							this.DisplayNotification("Injected");
							return;
						case Injector.inject_status.loadimage_fail:
							MessageBox.Show("Failed to access dll file.\n\nKrnl is most likely already injected, or your anti-virus is on!", "krnl", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							return;
						case Injector.inject_status.no_rbx_proc:
							this.DisplayNotification("No Roblox process found");
							return;
						default:
							return;
						}
					});
				});
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000030FF File Offset: 0x000012FF
		private void AnimateInjecting()
		{
			this.LoadBar.Visibility = Visibility.Visible;
			this.LoaderAnim.To = new Color?(Color.FromArgb(byte.MaxValue, 77, 146, byte.MaxValue));
			this.LoaderAnimStoryboard.Begin();
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003140 File Offset: 0x00001340
		private void AnimateInjected()
		{
			MainWindow.<AnimateInjected>d__56 <AnimateInjected>d__;
			<AnimateInjected>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimateInjected>d__.<>4__this = this;
			<AnimateInjected>d__.<>1__state = -1;
			<AnimateInjected>d__.<>t__builder.Start<MainWindow.<AnimateInjected>d__56>(ref <AnimateInjected>d__);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003177 File Offset: 0x00001377
		private void executeOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.TryExecuteScript(this.ReadScript());
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003186 File Offset: 0x00001386
		private bool TryExecuteScript(string script)
		{
			if (Pipes.PipeActive())
			{
				return Pipes.PassString(script);
			}
			this.DisplayNotification("Krnl is not injected");
			return false;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000031A2 File Offset: 0x000013A2
		private void menuOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.HBOpts.Visibility == Visibility.Hidden)
			{
				this.HBOpts.Visibility = Visibility.Visible;
				return;
			}
			this.HBOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000031CB File Offset: 0x000013CB
		private void FileHBOpt_MouseEnter(object sender, MouseEventArgs e)
		{
			this.FileOpts.Visibility = Visibility.Visible;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.PrefOpts.Visibility = Visibility.Hidden;
			this.ViewOpts.Visibility = Visibility.Hidden;
			this.FileHBOptOpen = 0;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003204 File Offset: 0x00001404
		private void FileHBOpt_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.FileHBOptOpen != 1)
			{
				this.FileOpts.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000321B File Offset: 0x0000141B
		private void FileHBOptsGate_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.FileHBOptOpen == 0)
			{
				this.FileHBOptOpen = 1;
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000322C File Offset: 0x0000142C
		private void FileOpts_MouseEnter(object sender, MouseEventArgs e)
		{
			this.FileHBOptOpen = 2;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003235 File Offset: 0x00001435
		private void FileOpts_MouseLeave(object sender, MouseEventArgs e)
		{
			this.FileHBOptOpen = 0;
			this.FileOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000324A File Offset: 0x0000144A
		private void EditHBOpt_MouseEnter(object sender, MouseEventArgs e)
		{
			this.EditOpts.Visibility = Visibility.Visible;
			this.FileOpts.Visibility = Visibility.Hidden;
			this.PrefOpts.Visibility = Visibility.Hidden;
			this.ViewOpts.Visibility = Visibility.Hidden;
			this.EditHBOptOpen = 0;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003283 File Offset: 0x00001483
		private void EditHBOpt_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.EditHBOptOpen != 1)
			{
				this.EditOpts.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000329A File Offset: 0x0000149A
		private void EditHBOptGate_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.EditHBOptOpen == 0)
			{
				this.EditHBOptOpen = 1;
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000032AB File Offset: 0x000014AB
		private void EditOpts_MouseEnter(object sender, MouseEventArgs e)
		{
			this.EditHBOptOpen = 2;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000032B4 File Offset: 0x000014B4
		private void EditOpts_MouseLeave(object sender, MouseEventArgs e)
		{
			this.EditHBOptOpen = 0;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000032CC File Offset: 0x000014CC
		private void CloseHBOpt_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
			if (processesByName.Length != 0)
			{
				Process[] array = processesByName;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Kill();
				}
			}
			this.HBOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000330C File Offset: 0x0000150C
		private void ActivateRecent()
		{
			this.RecentTab1.To = new Color?(Color.FromRgb(24, 160, 251));
			this.RecentTab2.To = new Color?(Color.FromRgb(24, 160, 251));
			this.RecentTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.RecentTab4.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.RecentTab5.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.RecentTab6.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.RecentTab7.To = new Color?(Color.FromRgb(24, 160, 251));
			this.RecentTab8.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.RecentTab9.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.RecentTab10.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003478 File Offset: 0x00001678
		private void DeactivateRecent()
		{
			this.RecentTab1.To = new Color?(Color.FromRgb(38, 38, 38));
			this.RecentTab2.To = new Color?(Color.FromRgb(34, 34, 34));
			this.RecentTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.RecentTab4.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTab5.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTab6.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTab7.To = new Color?(Color.FromRgb(38, 38, 38));
			this.RecentTab8.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTab9.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTab10.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTabLabel.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.circle19.Fill = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path21.Stroke = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			base.BeginStoryboard(this.RecentTab2SB);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000035F8 File Offset: 0x000017F8
		private void ActivateDrafts()
		{
			this.DraftsTab1.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab2.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab4.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab5.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab6.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab7.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab8.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab9.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab10.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab12.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab13.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab14.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab15.To = new Color?(Color.FromRgb(24, 160, 251));
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000037E8 File Offset: 0x000019E8
		private void DeactivateDrafts()
		{
			this.DraftsTab1.To = new Color?(Color.FromRgb(38, 38, 38));
			this.DraftsTab2.To = new Color?(Color.FromRgb(34, 34, 34));
			this.DraftsTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab4.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab5.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab6.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab7.To = new Color?(Color.FromRgb(38, 38, 38));
			this.DraftsTab8.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab9.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab10.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab12.To = new Color?(Color.FromRgb(38, 38, 38));
			this.DraftsTab13.To = new Color?(Color.FromRgb(34, 34, 34));
			this.DraftsTab14.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab15.To = new Color?(Color.FromRgb(38, 38, 38));
			this.DraftsTabLabel.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path208.Stroke = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path56.Stroke = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			base.BeginStoryboard(this.DraftsTab2SB);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000039D8 File Offset: 0x00001BD8
		private void ActivateCommunity()
		{
			this.CommunityTab1.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CommunityTab2.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CommunityTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CommunityTab4.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.CommunityTab5.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.CommunityTab6.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CommunityTab7.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.CommunityTab8.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003AFC File Offset: 0x00001CFC
		private void DeactivateCommunity()
		{
			this.CommunityTab1.To = new Color?(Color.FromRgb(38, 38, 38));
			this.CommunityTab2.To = new Color?(Color.FromRgb(34, 34, 34));
			this.CommunityTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CommunityTab4.To = new Color?(Color.FromRgb(125, 125, 125));
			this.CommunityTab5.To = new Color?(Color.FromRgb(125, 125, 125));
			this.CommunityTab6.To = new Color?(Color.FromRgb(38, 38, 38));
			this.CommunityTab7.To = new Color?(Color.FromRgb(125, 125, 125));
			this.CommunityTab8.To = new Color?(Color.FromRgb(125, 125, 125));
			this.CommunityTabLabel.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path20.Fill = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			base.BeginStoryboard(this.CommunityTab2SB);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003C2C File Offset: 0x00001E2C
		private void ActivatePlan()
		{
			this.UpgradeTab1.To = new Color?(Color.FromRgb(24, 160, 251));
			this.UpgradeTab2.To = new Color?(Color.FromRgb(24, 160, 251));
			this.UpgradeTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.UpgradeTab4.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.UpgradeTab5.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.UpgradeTab6.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.UpgradeTab7.To = new Color?(Color.FromRgb(24, 160, 251));
			this.UpgradeTab8.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.UpgradeTab9.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.UpgradeTab10.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003D98 File Offset: 0x00001F98
		private void DeactivatePlan()
		{
			this.UpgradeTab1.To = new Color?(Color.FromRgb(38, 38, 38));
			this.UpgradeTab2.To = new Color?(Color.FromRgb(34, 34, 34));
			this.UpgradeTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.UpgradeTab4.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTab5.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTab6.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTab7.To = new Color?(Color.FromRgb(38, 38, 38));
			this.UpgradeTab8.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTab9.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTab10.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTabLabel.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path42.Fill = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path44.Fill = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			base.BeginStoryboard(this.UpgradeTab2SB);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003F16 File Offset: 0x00002116
		private void RecentTab_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DisplayRecent();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003F20 File Offset: 0x00002120
		private void DisplayRecent()
		{
			Keyboard.ClearFocus();
			this.SearchInput.Text = "Search";
			this.FolderDisplayer.Children.RemoveRange(1, this.FolderDisplayer.Children.Count);
			this.MainDirDisplay.isFile = true;
			this.InitRecents();
			this.ActivateRecent();
			this.DeactivateDrafts();
			this.DeactivateCommunity();
			this.DeactivatePlan();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003F8D File Offset: 0x0000218D
		private void DraftsTab_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DisplayDrafts();
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003F98 File Offset: 0x00002198
		private void DisplayDrafts()
		{
			Keyboard.ClearFocus();
			this.SearchInput.Text = "Search";
			this.FolderDisplayer.Children.RemoveRange(1, this.FolderDisplayer.Children.Count);
			this.MainDirDisplay.isFile = true;
			this.InitDrafts("Scripts");
			this.DeactivateRecent();
			this.ActivateDrafts();
			this.DeactivateCommunity();
			this.DeactivatePlan();
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000400A File Offset: 0x0000220A
		public void SaveRecent(string Name, string Content)
		{
			File.WriteAllText("Recent\\" + Name, Content);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004020 File Offset: 0x00002220
		private void InitRecents()
		{
			this.CardHolder.Children.Clear();
			this.MainDirDisplay.EntryName.Content = "Recent";
			foreach (string path in Directory.GetFiles("Recent"))
			{
				string script = File.ReadAllText(path);
				string fileName = System.IO.Path.GetFileName(path);
				TimeSpan creationDate = DateTime.Now - File.GetLastWriteTime(path);
				this.CreateExplorerCard(fileName, script, creationDate, path);
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000040A0 File Offset: 0x000022A0
		private void InitDrafts(string Dire)
		{
			this.CardHolder.Children.Clear();
			this.MainDirDisplay.EntryName.Content = "Scripts";
			string[] files = Directory.GetFiles(Dire);
			foreach (string path in Directory.GetDirectories(Dire))
			{
				string fileName = System.IO.Path.GetFileName(path);
				TimeSpan creationDate = DateTime.Now - File.GetLastWriteTime(path);
				this.CreateExplorerCard(fileName, "", creationDate, path).SetFolderTheme();
			}
			foreach (string path2 in files)
			{
				string fileName2 = System.IO.Path.GetFileName(path2);
				if (fileName2 != "temp.bin")
				{
					string script = File.ReadAllText(path2);
					TimeSpan creationDate2 = DateTime.Now - File.GetLastWriteTime(path2);
					this.CreateExplorerCard(fileName2, script, creationDate2, path2).SetLuaTheme();
				}
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000417C File Offset: 0x0000237C
		private void InitCommunity()
		{
			this.MainDirDisplay.EntryName.Content = "Community";
			this.CardHolder.Children.Clear();
			foreach (CommunityEntry element in this.CommunityCards)
			{
				this.CardHolder.Children.Add(element);
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004200 File Offset: 0x00002400
		private void CommunityTab_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Keyboard.ClearFocus();
			this.SearchInput.Text = "Search";
			this.FolderDisplayer.Children.RemoveRange(1, this.FolderDisplayer.Children.Count);
			this.MainDirDisplay.isFile = true;
			this.InitCommunity();
			this.DeactivateRecent();
			this.DeactivateDrafts();
			this.ActivateCommunity();
			this.DeactivatePlan();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00004270 File Offset: 0x00002470
		private CommunityEntry CreateCommunityCard(string Title, string Script, string CreatorName)
		{
			CommunityEntry communityEntry = new CommunityEntry();
			communityEntry.EntryName.Content = Title;
			communityEntry.Script = Script;
			communityEntry.EntryCreator.Content = CreatorName;
			communityEntry.MouseRightButtonUp += this.CardHolder_MouseRightButtonUp;
			communityEntry.Width = 242.0;
			communityEntry.Height = 185.0;
			this.CardHolder.Children.Add(communityEntry);
			communityEntry.RunBorder.MouseLeftButtonUp += this.CommunityCard_MouseLeftButtonUp;
			return communityEntry;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000042FC File Offset: 0x000024FC
		private void CommunityCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CommunityEntry communityEntry = (CommunityEntry)((Grid)((Grid)((Border)sender).Parent).Parent).Parent;
			if (this.TryExecuteScript(communityEntry.Script))
			{
				this.DisplayNotification("File executed");
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00004347 File Offset: 0x00002547
		// (set) Token: 0x06000057 RID: 87 RVA: 0x0000434F File Offset: 0x0000254F
		public ExplorerEntry SelectedExpEntry { get; set; }

		// Token: 0x06000058 RID: 88 RVA: 0x00004358 File Offset: 0x00002558
		private ExplorerEntry CreateExplorerCard(string Title, string Script, TimeSpan CreationDate, string Path)
		{
			ExplorerEntry explorerEntry = new ExplorerEntry(this);
			explorerEntry.EntryEdit.PreviewKeyDown += this.Entry_PreviewKeyDown;
			explorerEntry.EntryName.Content = Title;
			explorerEntry.Script = Script;
			explorerEntry.EntryEditstamp.Content = this.TranslateDate(CreationDate);
			explorerEntry.Path = Path;
			explorerEntry.MouseDoubleClick += this.CardHolder_MouseDoubleClick;
			explorerEntry.MouseRightButtonUp += this.CardHolder_MouseRightButtonUp;
			explorerEntry.Width = 242.0;
			explorerEntry.Height = 185.0;
			this.CardHolder.Children.Add(explorerEntry);
			return explorerEntry;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004408 File Offset: 0x00002608
		private string TranslateDate(TimeSpan Date)
		{
			if (Date.Days > 0)
			{
				if (Date.Days != 1)
				{
					return string.Format("Edited {0} days ago", Date.Days);
				}
				return string.Format("Edited {0} day ago", Date.Days);
			}
			else if (Date.Hours > 0)
			{
				if (Date.Hours != 1)
				{
					return string.Format("Edited {0} hours ago", Date.Hours);
				}
				return string.Format("Edited {0} hour ago", Date.Hours);
			}
			else if (Date.Minutes > 0)
			{
				if (Date.Minutes != 1)
				{
					return string.Format("Edited {0} minutes ago", Date.Minutes);
				}
				return string.Format("Edited {0} minute ago", Date.Minutes);
			}
			else
			{
				if (Date.Seconds <= 0)
				{
					return "Edited now";
				}
				if (Date.Seconds != 1)
				{
					return string.Format("Edited {0} seconds ago", Date.Seconds);
				}
				return string.Format("Edited {0} second ago", Date.Seconds);
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004522 File Offset: 0x00002722
		private void AddRecent(string Name, string Content)
		{
			if (!File.Exists("Recent\\" + Name))
			{
				File.WriteAllText("Recent\\" + Name, Content);
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004548 File Offset: 0x00002748
		private void CardHolder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				ExplorerEntry explorerEntry = (ExplorerEntry)sender;
				if (explorerEntry.isFile)
				{
					if (File.Exists(explorerEntry.Path))
					{
						explorerEntry.Script = File.ReadAllText(explorerEntry.Path);
					}
					this.AddRecent(explorerEntry.EntryName.Content.ToString(), explorerEntry.Script);
					Tab tab = this.CreateTab(explorerEntry.EntryName.Content.ToString(), explorerEntry.Script, "");
					tab.Path = explorerEntry.Path;
					this.BringUpMenu(false);
					tab.Select();
					return;
				}
				if (Directory.Exists(explorerEntry.Path))
				{
					this.FolderDisplayer.Children.RemoveRange(1, this.FolderDisplayer.Children.Count);
					this.MainDirDisplay.isFile = false;
					this.CurrentDraftPath = explorerEntry.Path;
					this.LayDirPath(explorerEntry.Path);
					this.InitDrafts(explorerEntry.Path);
				}
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004644 File Offset: 0x00002844
		private void LayDirPath(string RelativePath)
		{
			string[] array = RelativePath.Split(new char[]
			{
				'\\'
			});
			string text = "Drafts\\";
			for (int i = 1; i < array.Length - 1; i++)
			{
				FolderDisplay folderDisplay = new FolderDisplay();
				folderDisplay.EntryName.Content = array[i];
				folderDisplay.EntryName.Foreground = new SolidColorBrush(Color.FromRgb(163, 163, 163));
				folderDisplay.isFile = false;
				folderDisplay.MouseLeftButtonUp += this.FolderDisplay_MouseLeftButtonUp;
				folderDisplay.MouseEnter += this.FolderDisplay_MouseEnter;
				folderDisplay.MouseLeave += this.FolderDisplay_MouseLeave;
				text += array[i];
				folderDisplay.Path = text;
				this.FolderDisplayer.Children.Add(folderDisplay);
			}
			FolderDisplay folderDisplay2 = new FolderDisplay();
			folderDisplay2.EntryName.Content = array[array.Length - 1];
			folderDisplay2.EntryName.Foreground = new SolidColorBrush(Color.FromRgb(163, 163, 163));
			folderDisplay2.isFile = true;
			folderDisplay2.Path = RelativePath;
			folderDisplay2.MouseLeftButtonUp += this.FolderDisplay_MouseLeftButtonUp;
			folderDisplay2.MouseEnter += this.FolderDisplay_MouseEnter;
			folderDisplay2.MouseLeave += this.FolderDisplay_MouseLeave;
			this.FolderDisplayer.Children.Add(folderDisplay2);
			this.CardHolder.Children.Clear();
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000047C4 File Offset: 0x000029C4
		private void FolderDisplay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.FolderDisplayer.Children.RemoveRange(1, this.FolderDisplayer.Children.Count);
			FolderDisplay folderDisplay = (FolderDisplay)sender;
			if (folderDisplay.EntryName.Content != "Community")
			{
				if (folderDisplay.Name == "MainDirDisplay")
				{
					this.CurrentDraftPath = "Scripts";
					this.LayDirPath("");
					this.MainDirDisplay.isFile = true;
					if (folderDisplay.EntryName.Content == "Scripts")
					{
						this.InitDrafts(folderDisplay.Path);
						return;
					}
					if (folderDisplay.EntryName.Content == "Recent")
					{
						this.InitRecents();
						return;
					}
				}
				else
				{
					this.CurrentDraftPath = folderDisplay.Path;
					this.LayDirPath(folderDisplay.Path);
					this.InitDrafts(folderDisplay.Path);
				}
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x0000489E File Offset: 0x00002A9E
		private void FolderDisplay_MouseEnter(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Hand;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000048AA File Offset: 0x00002AAA
		private void FolderDisplay_MouseLeave(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Arrow;
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000048B6 File Offset: 0x00002AB6
		// (set) Token: 0x06000061 RID: 97 RVA: 0x000048BE File Offset: 0x00002ABE
		private UIElement FileContextSelected { get; set; }

		// Token: 0x06000062 RID: 98 RVA: 0x000048C8 File Offset: 0x00002AC8
		private void CardHolder_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			Point position = Mouse.GetPosition(this.ManagedWrapper);
			this.FileContextMenu.Margin = new Thickness(position.X, position.Y, this.FileContextMenu.Margin.Right, this.FileContextMenu.Margin.Bottom);
			if (sender is ExplorerEntry)
			{
				this.FileContextMenu.Height = 157.0;
			}
			else if (sender is CommunityEntry)
			{
				this.FileContextMenu.Height = 41.0;
			}
			this.FileContextMenu.Visibility = Visibility.Visible;
			this.FileContextSelected = (UIElement)sender;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004977 File Offset: 0x00002B77
		private void UpgradeTab_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DeactivateRecent();
			this.DeactivateDrafts();
			this.DeactivateCommunity();
			this.ActivatePlan();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004994 File Offset: 0x00002B94
		private void MyScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewer scrollViewer = sender as ScrollViewer;
			if (e.Delta > 0)
			{
				scrollViewer.LineLeft();
			}
			else
			{
				scrollViewer.LineRight();
			}
			e.Handled = true;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000049C6 File Offset: 0x00002BC6
		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000049C8 File Offset: 0x00002BC8
		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000049CA File Offset: 0x00002BCA
		private void SearchGlass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Keyboard.ClearFocus();
			this.SearchInput.Focus();
			Keyboard.Focus(this.SearchInput);
			this.SearchInput.SelectAll();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000049F4 File Offset: 0x00002BF4
		private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (base.IsLoaded && this.Searchable)
			{
				if (this.MainDirDisplay.EntryName.Content == "Scripts")
				{
					this.InitDrafts("Scripts");
				}
				else if (this.MainDirDisplay.EntryName.Content == "Recent")
				{
					this.InitRecents();
				}
				else if (this.MainDirDisplay.EntryName.Content == "Community")
				{
					this.InitCommunity();
				}
				if (this.SearchInput.Text != "")
				{
					Console.WriteLine(this.SearchInput.Text);
					List<UIElement> list = new List<UIElement>();
					if (this.MainDirDisplay.EntryName.Content == "Recent" || this.MainDirDisplay.EntryName.Content == "Scripts")
					{
						using (IEnumerator enumerator = this.CardHolder.Children.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object obj = enumerator.Current;
								ExplorerEntry explorerEntry = (ExplorerEntry)obj;
								if (CultureInfo.CurrentUICulture.CompareInfo.IndexOf(explorerEntry.EntryName.Content.ToString(), this.SearchInput.Text, CompareOptions.IgnoreCase) >= 0)
								{
									list.Add(explorerEntry);
								}
							}
							goto IL_23C;
						}
					}
					if (this.MainDirDisplay.EntryName.Content == "Community")
					{
						foreach (object obj2 in this.CardHolder.Children)
						{
							CommunityEntry communityEntry = (CommunityEntry)obj2;
							if (communityEntry.EntryName.Content.ToString().ToLower().IndexOf(this.SearchInput.Text.ToLower()) >= 0)
							{
								list.Add(communityEntry);
							}
							else if (communityEntry.Tags.Count > 0)
							{
								for (int i = 0; i < communityEntry.Tags.Count; i++)
								{
									if (communityEntry.Tags[i].ToString().ToLower().StartsWith(this.SearchInput.Text.ToLower()))
									{
										list.Add(communityEntry);
										break;
									}
								}
							}
						}
					}
					IL_23C:
					this.CardHolder.Children.Clear();
					foreach (UIElement element in list)
					{
						this.CardHolder.Children.Add(element);
					}
				}
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004CB4 File Offset: 0x00002EB4
		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004CB6 File Offset: 0x00002EB6
		private void SearchInput_GotFocus(object sender, RoutedEventArgs e)
		{
			this.SearchInput.Clear();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004CC3 File Offset: 0x00002EC3
		private void Window_DpiChanged(object sender, DpiChangedEventArgs e)
		{
			base.Activate();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004CCC File Offset: 0x00002ECC
		private void OpenFileOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.PromptOpenFile();
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00004CD4 File Offset: 0x00002ED4
		private void PromptOpenFile()
		{
			this.HBOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.DefaultExt = "lua";
			bool? flag = openFileDialog.ShowDialog();
			bool flag2 = true;
			if (flag.GetValueOrDefault() == flag2 & flag != null)
			{
				string text = File.ReadAllText(openFileDialog.FileName);
				Tab tab = this.CreateTab(openFileDialog.SafeFileName, text, "");
				tab.Path = openFileDialog.FileName;
				tab.fileWatcher.filePath = tab.Path;
				tab.fileWatcher.Start();
				tab.Select();
				this.WriteScript(text, false);
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004D82 File Offset: 0x00002F82
		private void SaveFileOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.PromptSaveAsFile();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004D8C File Offset: 0x00002F8C
		private void PromptSaveAsFile()
		{
			this.HBOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
			if (Common.SelectedTab != null)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\Scripts";
				saveFileDialog.FileName = Common.SelectedTab.TabName;
				saveFileDialog.DefaultExt = "lua";
				saveFileDialog.Filter = "Lua files (*.lua)|*.lua";
				bool? flag = saveFileDialog.ShowDialog();
				bool flag2 = true;
				if (flag.GetValueOrDefault() == flag2 & flag != null)
				{
					File.WriteAllText(saveFileDialog.FileName, this.ReadScript());
					Common.SelectedTab.IsSaved = true;
					Common.SelectedTab.Path = saveFileDialog.FileName;
					Common.SelectedTab.fileWatcher.filePath = saveFileDialog.FileName;
					Common.SelectedTab.fileWatcher.Start();
				}
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00004E6C File Offset: 0x0000306C
		private void DisplayNotification(string Text)
		{
			MainWindow.<DisplayNotification>d__125 <DisplayNotification>d__;
			<DisplayNotification>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisplayNotification>d__.<>4__this = this;
			<DisplayNotification>d__.Text = Text;
			<DisplayNotification>d__.<>1__state = -1;
			<DisplayNotification>d__.<>t__builder.Start<MainWindow.<DisplayNotification>d__125>(ref <DisplayNotification>d__);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004EAB File Offset: 0x000030AB
		private void FileContextMenu_LostFocus(object sender, RoutedEventArgs e)
		{
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004EBC File Offset: 0x000030BC
		private void OpenHBOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null)
			{
				this.CreateTab(explorerEntry.EntryName.Content.ToString(), explorerEntry.Script, "");
			}
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004F06 File Offset: 0x00003106
		private void RenameHBOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004F14 File Offset: 0x00003114
		private void DeleteHBOpt_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null)
			{
				File.GetAttributes(explorerEntry.Path);
				if (explorerEntry.isFile)
				{
					if (File.Exists(explorerEntry.Path))
					{
						File.Delete(explorerEntry.Path);
					}
				}
				else if (Directory.Exists(explorerEntry.Path))
				{
					new DirectoryInfo(explorerEntry.Path).Delete(true);
				}
				this.CardHolder.Children.Remove(explorerEntry);
			}
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004F9C File Offset: 0x0000319C
		private void ExplorerHBOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null)
			{
				Process.Start("explorer.exe", "/select, \"" + explorerEntry.Path + "\"");
			}
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004FE4 File Offset: 0x000031E4
		private void OpenHBOpt_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null && explorerEntry.isFile && File.Exists(explorerEntry.Path))
			{
				Tab tab = this.CreateTab(explorerEntry.EntryName.Content.ToString(), explorerEntry.Script, "");
				tab.Path = explorerEntry.Path;
				tab.Select();
				this.BringUpMenu(true);
			}
			CommunityEntry communityEntry = this.FileContextSelected as CommunityEntry;
			if (communityEntry != null)
			{
				this.CreateTab(communityEntry.EntryName.Content.ToString(), communityEntry.Script, "").Select();
				this.BringUpMenu(true);
			}
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005098 File Offset: 0x00003298
		private void ExecuteHBOpt_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			string script = "";
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null)
			{
				script = explorerEntry.Script;
			}
			else
			{
				CommunityEntry communityEntry = this.FileContextSelected as CommunityEntry;
				if (communityEntry != null)
				{
					script = communityEntry.Script;
				}
			}
			if (this.TryExecuteScript(script))
			{
				this.DisplayNotification("File executed");
			}
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000050FC File Offset: 0x000032FC
		private void RenameHBOpt_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null)
			{
				explorerEntry.EntryName.Visibility = Visibility.Hidden;
				explorerEntry.EntryEdit.Text = explorerEntry.EntryName.Content.ToString();
				explorerEntry.EntryEdit.Visibility = Visibility.Visible;
				explorerEntry.Select();
				explorerEntry.Focus();
				explorerEntry.EntryEdit.Focus();
				Keyboard.Focus(explorerEntry.EntryEdit);
				explorerEntry.EntryEdit.SelectAll();
				this.FileContextMenu.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00005188 File Offset: 0x00003388
		private void Entry_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				ExplorerEntry explorerEntry = (ExplorerEntry)((Grid)((Border)((Grid)((Grid)((TextBox)sender).Parent).Parent).Parent).Parent).Parent;
				if (explorerEntry.EntryEdit.Text == "")
				{
					explorerEntry.EntryName.Visibility = Visibility.Visible;
					explorerEntry.EntryEdit.Visibility = Visibility.Hidden;
					return;
				}
				explorerEntry.EntryName.Content = explorerEntry.EntryEdit.Text;
				explorerEntry.EntryName.Visibility = Visibility.Visible;
				explorerEntry.EntryEdit.Visibility = Visibility.Hidden;
				string text = System.IO.Path.GetDirectoryName(explorerEntry.Path) + string.Format("\\{0}", explorerEntry.EntryName.Content);
				if (explorerEntry.isFile)
				{
					if (!File.Exists(text))
					{
						File.Move(explorerEntry.Path, text);
					}
				}
				else if (!Directory.Exists(text))
				{
					Directory.Move(explorerEntry.Path, text);
				}
				explorerEntry.Path = text;
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005297 File Offset: 0x00003497
		private void SaveOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.PromptSaveFile();
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000052A0 File Offset: 0x000034A0
		private void PromptSaveFile()
		{
			this.HBOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
			if (Common.SelectedTab != null)
			{
				if (Common.SelectedTab.Path == "" || !File.Exists(Common.SelectedTab.Path))
				{
					SaveFileDialog saveFileDialog = new SaveFileDialog();
					saveFileDialog.DefaultExt = "lua";
					saveFileDialog.Filter = "Lua files (*.lua)|*.lua";
					bool? flag = saveFileDialog.ShowDialog();
					bool flag2 = true;
					if (flag.GetValueOrDefault() == flag2 & flag != null)
					{
						Common.SelectedTab.TabName = saveFileDialog.SafeFileName;
						Common.SelectedTab.Path = saveFileDialog.FileName;
						File.WriteAllText(saveFileDialog.FileName, this.ReadScript());
						Common.SelectedTab.isSaved = false;
						Common.SelectedTab.IsSaved = true;
						return;
					}
				}
				else
				{
					Common.SelectedTab.isSaved = false;
					Common.SelectedTab.IsSaved = true;
					Common.SelectedTab.TabName = Common.SelectedTab.TabName;
					Common.SelectedTab.watchSave = true;
					File.WriteAllText(Common.SelectedTab.Path, this.ReadScript());
				}
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000053C7 File Offset: 0x000035C7
		private void NewDraft_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.MainDirDisplay.EntryName.Content == "Scripts")
			{
				this.CreateDraft();
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000053E8 File Offset: 0x000035E8
		private void CreateDraft()
		{
			ExplorerEntry explorerEntry = this.CreateExplorerCard("Untitled", "", new TimeSpan(0, 0, 0), this.CurrentDraftPath + "\\temp.bin");
			File.WriteAllText(this.CurrentDraftPath + "\\temp.bin", "");
			explorerEntry.EntryName.Visibility = Visibility.Hidden;
			explorerEntry.EntryEdit.Text = explorerEntry.EntryName.Content.ToString();
			explorerEntry.EntryEdit.Visibility = Visibility.Visible;
			explorerEntry.EntryEdit.PreviewKeyDown += this.Entry_PreviewKeyDown;
			Keyboard.ClearFocus();
			explorerEntry.EntryEdit.SelectAll();
			explorerEntry.EntryEdit.Focus();
			UIElement relativeTo = VisualTreeHelper.GetParent(explorerEntry) as UIElement;
			explorerEntry.TranslatePoint(new Point(0.0, 0.0), relativeTo);
			this.CardHolderScroller.ScrollToBottom();
			explorerEntry.Select();
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000054DC File Offset: 0x000036DC
		private void UndoOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.browser.EvaluateScriptAsync("Undo();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000552C File Offset: 0x0000372C
		private void RedoOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.browser.EvaluateScriptAsync("Redo();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x0000557C File Offset: 0x0000377C
		private void MinimapOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.MinimapEnabled = !this.MinimapEnabled;
			this.svg2422.Visibility = (this.MinimapEnabled ? Visibility.Visible : Visibility.Hidden);
			this.browser.EvaluateScriptAsync("SwitchMinimap(" + this.MinimapEnabled.ToString().ToLower() + ");", null, false).GetAwaiter().GetResult();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000055F1 File Offset: 0x000037F1
		private void PrefHBOpt_MouseEnter(object sender, MouseEventArgs e)
		{
			this.PrefOpts.Visibility = Visibility.Visible;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
			this.PrefHBOptOpen = 0;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000561E File Offset: 0x0000381E
		private void PrefHBOpt_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.PrefHBOptOpen != 1)
			{
				this.PrefOpts.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005635 File Offset: 0x00003835
		private void PrefHBOptGate_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.PrefHBOptOpen == 0)
			{
				this.PrefHBOptOpen = 1;
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005646 File Offset: 0x00003846
		private void PrefOpts_MouseEnter(object sender, MouseEventArgs e)
		{
			this.PrefHBOptOpen = 2;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000564F File Offset: 0x0000384F
		private void PrefOpts_MouseLeave(object sender, MouseEventArgs e)
		{
			this.PrefHBOptOpen = 0;
			this.PrefOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00005664 File Offset: 0x00003864
		private void TopmostOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			base.Topmost = !base.Topmost;
			this.getKrnlSubkey().SetValue("Topmost", base.Topmost ? "true" : "false");
			this.svg242_Copy.Visibility = (base.Topmost ? Visibility.Visible : Visibility.Hidden);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000056BC File Offset: 0x000038BC
		private void UnlockFPSOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			RegistryKey krnlSubkey = this.getKrnlSubkey();
			object value = krnlSubkey.GetValue("UnlockFPS");
			bool flag = value == null || !(value.ToString() == "true");
			if (sender != null)
			{
				krnlSubkey.SetValue("UnlockFPS", flag ? "true" : "false", RegistryValueKind.String);
			}
			else
			{
				flag = !flag;
			}
			this.TryExecuteScript("setfpscap(144)");
			this.svg242_Copy1.Visibility = (flag ? Visibility.Visible : Visibility.Hidden);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000573C File Offset: 0x0000393C
		private RegistryKey getKrnlSubkey()
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE");
			RegistryKey registryKey2 = null;
			try
			{
				registryKey2 = registryKey.OpenSubKey("Krnl", true);
			}
			catch
			{
				registryKey2 = registryKey2.CreateSubKey("Krnl", true);
			}
			return registryKey2;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000578C File Offset: 0x0000398C
		private void ViewHBOpt_MouseEnter(object sender, MouseEventArgs e)
		{
			this.ViewOpts.Visibility = Visibility.Visible;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
			this.PrefOpts.Visibility = Visibility.Hidden;
			this.ViewHBOptOpen = 0;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000057C5 File Offset: 0x000039C5
		private void ViewHBOpt_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.ViewHBOptOpen != 1)
			{
				this.ViewOpts.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000057DC File Offset: 0x000039DC
		private void ViewHBOptGate_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.ViewHBOptOpen == 0)
			{
				this.ViewHBOptOpen = 1;
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000057ED File Offset: 0x000039ED
		private void ViewOpts_MouseEnter(object sender, MouseEventArgs e)
		{
			this.ViewHBOptOpen = 2;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000057F6 File Offset: 0x000039F6
		private void ViewOpts_MouseLeave(object sender, MouseEventArgs e)
		{
			this.ViewHBOptOpen = 0;
			this.ViewOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000580C File Offset: 0x00003A0C
		private void AutoAttachOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.AutoAttachEnabled = !this.AutoAttachEnabled;
			this.getKrnlSubkey().SetValue("AutoAttach", this.AutoAttachEnabled ? "true" : "false");
			this.svg242.Visibility = (this.AutoAttachEnabled ? Visibility.Visible : Visibility.Hidden);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005863 File Offset: 0x00003A63
		private void AutoLaunchOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.AutoLaunchEnabled = !this.AutoLaunchEnabled;
			if (this.AutoLaunchEnabled)
			{
				this.enable_auto_launch();
				return;
			}
			this.disable_auto_launch();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000588C File Offset: 0x00003A8C
		private void enable_auto_launch()
		{
			string path = System.IO.Path.Combine(this.roblox_path, "XInput1_4.dll");
			if (!File.Exists(path))
			{
				this.svg3.Visibility = Visibility.Visible;
				if (File.Exists(this.DllPath))
				{
					File.WriteAllBytes(path, File.ReadAllBytes(this.DllPath));
				}
				else
				{
					this.DisplayNotification("DLL not found, please close the UI and relaunch the bootstrapper");
				}
			}
			this.auto_launch_mutex = new Mutex(false, "RJ_AL_MTX0001");
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000058FC File Offset: 0x00003AFC
		private void disable_auto_launch()
		{
			if (this.auto_launch_mutex != null)
			{
				this.auto_launch_mutex.Dispose();
			}
			bool flag;
			Mutex mutex = new Mutex(true, "RJ_AL_MTX0001", ref flag);
			if (flag)
			{
				mutex.ReleaseMutex();
			}
			mutex.Dispose();
			string path = System.IO.Path.Combine(this.roblox_path, "XInput1_4.dll");
			if (File.Exists(path))
			{
				try
				{
					File.Delete(path);
					this.svg3.Visibility = Visibility.Hidden;
				}
				catch (Exception)
				{
					this.svg3.Visibility = Visibility.Visible;
					MessageBox.Show("Please close Roblox before trying to disable auto launch.", "Krnl", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005998 File Offset: 0x00003B98
		private void SearchInput_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.SearchInput.Text == "")
			{
				this.Searchable = false;
				this.SearchInput.Text = "Search";
				this.Searchable = true;
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000059D0 File Offset: 0x00003BD0
		private void CutOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.browser.EvaluateScriptAsync("Cut();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005A20 File Offset: 0x00003C20
		private void CopyOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.browser.EvaluateScriptAsync("Copy();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005A70 File Offset: 0x00003C70
		private void PasteOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.browser.EvaluateScriptAsync("Paste();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005AC0 File Offset: 0x00003CC0
		private void DeleteOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.browser.EvaluateScriptAsync("Delete();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005B10 File Offset: 0x00003D10
		private void SelectAllOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.browser.EvaluateScriptAsync("SelectAll();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005B5D File Offset: 0x00003D5D
		private void MainMenu_MouseDown(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005B5F File Offset: 0x00003D5F
		private void Workspace_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.Workspace.Focus();
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005B6D File Offset: 0x00003D6D
		private void HBOpts_LostFocus(object sender, RoutedEventArgs e)
		{
			this.HBOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005B7B File Offset: 0x00003D7B
		private void OpenKrnlOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Process.Start(Directory.GetCurrentDirectory());
			this.HBOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005BA0 File Offset: 0x00003DA0
		private void SearchBorder_MouseEnter(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.IBeam;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005BAD File Offset: 0x00003DAD
		private void SearchBorder_MouseLeave(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.Arrow;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005BBC File Offset: 0x00003DBC
		private void Editor_Drop(object sender, DragEventArgs e)
		{
			DataObject dataObject = (DataObject)e.Data;
			if (dataObject.ContainsFileDropList())
			{
				StringCollection fileDropList = dataObject.GetFileDropList();
				Tab tab = null;
				foreach (string path in fileDropList)
				{
					try
					{
						tab = this.CreateTab(System.IO.Path.GetFileName(path), File.ReadAllText(path), "");
					}
					catch (Exception ex)
					{
						tab = this.CreateTab("<ERROR> " + System.IO.Path.GetFileName(path), "Couldn't access the file\nComputer produced error; " + ex.Message, "");
					}
				}
				if (tab == null)
				{
					MessageBox.Show("FATAL CODE FAULT . Some code in the application produced unexpected output . 1941");
					return;
				}
				tab.Select();
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005C94 File Offset: 0x00003E94
		private void TabContainer_Drop(object sender, DragEventArgs e)
		{
			DataObject dataObject = (DataObject)e.Data;
			if (dataObject.ContainsFileDropList())
			{
				StringCollection fileDropList = dataObject.GetFileDropList();
				Tab tab = null;
				foreach (string path in fileDropList)
				{
					tab = this.CreateTab(System.IO.Path.GetFileName(path), File.ReadAllText(path), path);
				}
				if (tab != null)
				{
					tab.Select();
				}
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005D18 File Offset: 0x00003F18
		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (!Directory.Exists("Data"))
			{
				Directory.CreateDirectory("Data");
				if (!Directory.Exists("Data\\SavedTabs"))
				{
					Directory.CreateDirectory("Data\\SavedTabs");
				}
			}
			List<string> list = new List<string>();
			for (int i = 0; i < this.TabFlow.Children.Count - 1; i++)
			{
				Tab tab = (Tab)this.TabFlow.Children[i];
				Directory.CreateDirectory("Data\\SavedTabs\\" + tab.TabName);
				File.WriteAllText("Data\\SavedTabs\\" + tab.TabName + "\\tab.config", tab.Path);
				File.WriteAllText("Data\\SavedTabs\\" + tab.TabName + "\\script.lua", tab.Script);
				list.Add(tab.TabName);
			}
			File.WriteAllText("Data\\SavedTabs\\tabs.config", string.Join("\n", list));
			if (Common.SelectedTab != null)
			{
				Directory.CreateDirectory("Data\\SavedTabs\\" + Common.SelectedTab.TabName);
				File.WriteAllText("Data\\SavedTabs\\" + Common.SelectedTab.TabName + "\\tab.config", Common.SelectedTab.Path);
				File.WriteAllText("Data\\SavedTabs\\" + Common.SelectedTab.TabName + "\\script.lua", this.ReadScript());
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005E74 File Offset: 0x00004074
		private void Window_Closed(object sender, EventArgs e)
		{
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00005E78 File Offset: 0x00004078
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/KrnlUI;component/mainwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005EA8 File Offset: 0x000040A8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00005EB4 File Offset: 0x000040B4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((MainWindow)target).SizeChanged += this.Window_SizeChanged;
				((MainWindow)target).DpiChanged += this.Window_DpiChanged;
				((MainWindow)target).MouseDown += this.KrnlWindow_MouseDown;
				((MainWindow)target).Closing += this.Window_Closing;
				((MainWindow)target).Closed += this.Window_Closed;
				return;
			case 2:
				this.Workspace = (Grid)target;
				this.Workspace.MouseDown += this.Workspace_MouseDown;
				return;
			case 3:
				this.KrnlWindow = (Border)target;
				return;
			case 4:
				this.Workspace1 = (Grid)target;
				return;
			case 5:
				this.TabContainer = (StackPanel)target;
				this.TabContainer.MouseLeftButtonDown += this.MovableForm_MouseDown;
				this.TabContainer.Drop += this.TabContainer_Drop;
				return;
			case 6:
				this.MyScrollViewer = (ScrollViewer)target;
				this.MyScrollViewer.PreviewMouseWheel += this.MyScrollViewer_PreviewMouseWheel;
				return;
			case 7:
				this.TabFlow = (StackPanel)target;
				return;
			case 8:
				this.NewTabButton = (Grid)target;
				this.NewTabButton.MouseLeftButtonUp += this.NewTabButton_MouseLeftButtonUp;
				return;
			case 9:
				this.svg454 = (Canvas)target;
				return;
			case 10:
				this.path452 = (System.Windows.Shapes.Path)target;
				return;
			case 11:
				this.MinimizeButton = (StackPanel)target;
				this.MinimizeButton.MouseLeftButtonUp += this.MinimizeButton_MouseLeftButtonUp;
				return;
			case 12:
				this.svg376 = (Canvas)target;
				return;
			case 13:
				this.path374 = (System.Windows.Shapes.Path)target;
				return;
			case 14:
				this.MaximizeButton = (StackPanel)target;
				this.MaximizeButton.MouseLeftButtonUp += this.MaximizeButton_MouseLeftButtonUp;
				return;
			case 15:
				this.svg356 = (Canvas)target;
				return;
			case 16:
				this.rect354 = (Rectangle)target;
				return;
			case 17:
				this.svg223 = (Canvas)target;
				return;
			case 18:
				this.path202 = (System.Windows.Shapes.Path)target;
				return;
			case 19:
				this.ExitButton = (StackPanel)target;
				this.ExitButton.MouseLeftButtonUp += this.ExitButton_MouseLeftButtonUp;
				return;
			case 20:
				this.svg276 = (Canvas)target;
				return;
			case 21:
				this.path274 = (System.Windows.Shapes.Path)target;
				return;
			case 22:
				this.appIcon = (Grid)target;
				this.appIcon.MouseLeftButtonUp += this.appIcon_MouseLeftButtonUp;
				this.appIcon.MouseEnter += this.appIcon_MouseEnter;
				this.appIcon.MouseLeave += this.appIcon_MouseLeave;
				this.appIcon.MouseLeftButtonDown += this.appIcon_MouseLeftButtonDown;
				return;
			case 23:
				this.appIconAnimEnterIconSB = (Storyboard)target;
				return;
			case 24:
				this.appIconAnimEnterIcon = (ColorAnimation)target;
				return;
			case 25:
				this.appIconAnimEnterBackSB = (Storyboard)target;
				return;
			case 26:
				this.appIconAnimEnterBack = (ColorAnimation)target;
				return;
			case 27:
				this.appIconAnimLeaveIconSB = (Storyboard)target;
				return;
			case 28:
				this.appIconAnimLeaveIcon = (ColorAnimation)target;
				return;
			case 29:
				this.appIconAnimLeaveBackSB = (Storyboard)target;
				return;
			case 30:
				this.appIconAnimLeaveBack = (ColorAnimation)target;
				return;
			case 31:
				this.appIconAnimDownSB = (Storyboard)target;
				return;
			case 32:
				this.appIconAnimDown = (ColorAnimation)target;
				return;
			case 33:
				this.appIconAnimUpSB = (Storyboard)target;
				return;
			case 34:
				this.appIconAnimUp = (ColorAnimation)target;
				return;
			case 35:
				this.path2 = (System.Windows.Shapes.Path)target;
				return;
			case 36:
				this.Editor = (Grid)target;
				this.Editor.Drop += this.Editor_Drop;
				return;
			case 37:
				this.LoadBar = (Grid)target;
				return;
			case 38:
				this.LoaderAnimStoryboard = (Storyboard)target;
				return;
			case 39:
				this.LoaderAnim = (ColorAnimation)target;
				return;
			case 40:
				this.HBOpts = (Grid)target;
				this.HBOpts.LostFocus += this.HBOpts_LostFocus;
				return;
			case 41:
				this.svg233 = (Canvas)target;
				return;
			case 42:
				this.path212 = (System.Windows.Shapes.Path)target;
				return;
			case 43:
				this.PrefHBOpt = (Grid)target;
				this.PrefHBOpt.MouseEnter += this.PrefHBOpt_MouseEnter;
				this.PrefHBOpt.MouseLeave += this.PrefHBOpt_MouseLeave;
				return;
			case 44:
				this.TabTitle9 = (Label)target;
				return;
			case 45:
				this.svg321_Copy5 = (Canvas)target;
				return;
			case 46:
				this.path9 = (System.Windows.Shapes.Path)target;
				return;
			case 47:
				this.PrefHBOptGate = (Grid)target;
				this.PrefHBOptGate.MouseEnter += this.PrefHBOptGate_MouseEnter;
				return;
			case 48:
				this.FileHBOpt = (Grid)target;
				this.FileHBOpt.MouseEnter += this.FileHBOpt_MouseEnter;
				this.FileHBOpt.MouseLeave += this.FileHBOpt_MouseLeave;
				return;
			case 49:
				this.TabTitle6 = (Label)target;
				return;
			case 50:
				this.svg2 = (Canvas)target;
				return;
			case 51:
				this.path6 = (System.Windows.Shapes.Path)target;
				return;
			case 52:
				this.FileHBOptsGate = (Grid)target;
				this.FileHBOptsGate.MouseEnter += this.FileHBOptsGate_MouseEnter;
				return;
			case 53:
				this.EditHBOpt = (Grid)target;
				this.EditHBOpt.MouseEnter += this.EditHBOpt_MouseEnter;
				this.EditHBOpt.MouseLeave += this.EditHBOpt_MouseLeave;
				return;
			case 54:
				this.TabTitle7 = (Label)target;
				return;
			case 55:
				this.svg321_Copy3 = (Canvas)target;
				return;
			case 56:
				this.path7 = (System.Windows.Shapes.Path)target;
				return;
			case 57:
				this.EditHBOptGate = (Grid)target;
				this.EditHBOptGate.MouseEnter += this.EditHBOptGate_MouseEnter;
				return;
			case 58:
				this.ViewHBOpt = (Grid)target;
				this.ViewHBOpt.MouseEnter += this.ViewHBOpt_MouseEnter;
				this.ViewHBOpt.MouseLeave += this.ViewHBOpt_MouseLeave;
				return;
			case 59:
				this.TabTitle8 = (Label)target;
				return;
			case 60:
				this.svg321_Copy4 = (Canvas)target;
				return;
			case 61:
				this.path8 = (System.Windows.Shapes.Path)target;
				return;
			case 62:
				this.ViewHBOptGate = (Grid)target;
				this.ViewHBOptGate.MouseEnter += this.ViewHBOptGate_MouseEnter;
				return;
			case 63:
				this.CloseHBOpt = (Grid)target;
				this.CloseHBOpt.MouseUp += this.CloseHBOpt_MouseUp;
				return;
			case 64:
				this.TabTitle5 = (Label)target;
				return;
			case 65:
				this.svg1 = (Canvas)target;
				return;
			case 66:
				this.path5 = (System.Windows.Shapes.Path)target;
				return;
			case 67:
				this.FileOpts = (Border)target;
				this.FileOpts.MouseEnter += this.FileOpts_MouseEnter;
				this.FileOpts.MouseLeave += this.FileOpts_MouseLeave;
				return;
			case 68:
				this.OpenFileOpt = (Grid)target;
				this.OpenFileOpt.MouseLeftButtonUp += this.OpenFileOpt_MouseLeftButtonUp;
				return;
			case 69:
				this.TabTitle2 = (Label)target;
				return;
			case 70:
				this.TabTitle22_Copy1 = (Label)target;
				return;
			case 71:
				this.SaveOpt = (Grid)target;
				this.SaveOpt.MouseLeftButtonUp += this.SaveOpt_MouseLeftButtonUp;
				return;
			case 72:
				this.TabTitle11 = (Label)target;
				return;
			case 73:
				this.TabTitle22_Copy2 = (Label)target;
				return;
			case 74:
				this.SaveFileOpt = (Grid)target;
				this.SaveFileOpt.MouseLeftButtonUp += this.SaveFileOpt_MouseLeftButtonUp;
				return;
			case 75:
				this.TabTitle3 = (Label)target;
				return;
			case 76:
				this.TabTitle22_Copy3 = (Label)target;
				return;
			case 77:
				this.OpenKrnlOpt = (Grid)target;
				this.OpenKrnlOpt.MouseLeftButtonUp += this.OpenKrnlOpt_MouseLeftButtonUp;
				return;
			case 78:
				this.TabTitle25 = (Label)target;
				return;
			case 79:
				this.menuOpt = (Grid)target;
				this.menuOpt.MouseLeftButtonUp += this.menuOpt_MouseLeftButtonUp;
				return;
			case 80:
				this.svg95 = (Canvas)target;
				return;
			case 81:
				this.path93 = (System.Windows.Shapes.Path)target;
				return;
			case 82:
				this.svg42 = (Canvas)target;
				return;
			case 83:
				this.path40 = (System.Windows.Shapes.Path)target;
				return;
			case 84:
				this.g81 = (Canvas)target;
				return;
			case 85:
				this.path70 = (System.Windows.Shapes.Path)target;
				return;
			case 86:
				this.injectOpt = (Grid)target;
				this.injectOpt.MouseLeftButtonUp += this.injectOpt_MouseLeftButtonUp;
				return;
			case 87:
				this.svg296 = (Canvas)target;
				return;
			case 88:
				this.connect = (System.Windows.Shapes.Path)target;
				return;
			case 89:
				this.executeOpt = (Grid)target;
				this.executeOpt.MouseLeftButtonUp += this.executeOpt_MouseLeftButtonUp;
				return;
			case 90:
				this.svg396 = (Canvas)target;
				return;
			case 91:
				this.path394 = (System.Windows.Shapes.Path)target;
				return;
			case 92:
				this.MainMenu = (Grid)target;
				this.MainMenu.MouseDown += this.MainMenu_MouseDown;
				return;
			case 93:
				this.menuOpt1 = (Grid)target;
				return;
			case 94:
				this.svg304 = (Canvas)target;
				return;
			case 95:
				this.path300 = (System.Windows.Shapes.Path)target;
				return;
			case 96:
				this.path302 = (System.Windows.Shapes.Path)target;
				return;
			case 97:
				this.Introduct = (Label)target;
				return;
			case 98:
				this.SearchBorder = (Border)target;
				this.SearchBorder.MouseEnter += this.SearchBorder_MouseEnter;
				this.SearchBorder.MouseLeave += this.SearchBorder_MouseLeave;
				return;
			case 99:
				((Grid)target).MouseLeftButtonDown += this.Grid_MouseLeftButtonDown;
				return;
			case 100:
				this.SearchInput = (TextBox)target;
				this.SearchInput.TextChanged += this.SearchInput_TextChanged;
				this.SearchInput.GotFocus += this.SearchInput_GotFocus;
				this.SearchInput.LostFocus += this.SearchInput_LostFocus;
				return;
			case 101:
				this.SearchGlass = (Grid)target;
				this.SearchGlass.MouseLeftButtonUp += this.SearchGlass_MouseLeftButtonUp;
				return;
			case 102:
				this.svg28 = (Canvas)target;
				return;
			case 103:
				this.circle24 = (Ellipse)target;
				return;
			case 104:
				this.path26 = (System.Windows.Shapes.Path)target;
				return;
			case 105:
				this.FolderDisplayer = (StackPanel)target;
				return;
			case 106:
				this.MainDirDisplay = (FolderDisplay)target;
				return;
			case 107:
				this.RecentTab = (Grid)target;
				this.RecentTab.MouseLeftButtonDown += this.RecentTab_MouseDown;
				return;
			case 108:
				this.RecentTab1 = (ColorAnimation)target;
				return;
			case 109:
				this.RecentTab2SB = (Storyboard)target;
				return;
			case 110:
				this.RecentTab2 = (ColorAnimation)target;
				return;
			case 111:
				this.RecentTab3 = (ColorAnimation)target;
				return;
			case 112:
				this.RecentTab4 = (ColorAnimation)target;
				return;
			case 113:
				this.RecentTab5 = (ColorAnimation)target;
				return;
			case 114:
				this.RecentTab6 = (ColorAnimation)target;
				return;
			case 115:
				this.RecentTab7 = (ColorAnimation)target;
				return;
			case 116:
				this.RecentTab8 = (ColorAnimation)target;
				return;
			case 117:
				this.RecentTab9 = (ColorAnimation)target;
				return;
			case 118:
				this.RecentTab10 = (ColorAnimation)target;
				return;
			case 119:
				this.svg23 = (Canvas)target;
				return;
			case 120:
				this.circle19 = (System.Windows.Shapes.Path)target;
				return;
			case 121:
				this.path21 = (System.Windows.Shapes.Path)target;
				return;
			case 122:
				this.RecentTabLabel = (Label)target;
				return;
			case 123:
				this.DraftsTab = (Grid)target;
				this.DraftsTab.MouseLeftButtonDown += this.DraftsTab_MouseDown;
				return;
			case 124:
				this.DraftsTab1 = (ColorAnimation)target;
				return;
			case 125:
				this.DraftsTab12 = (ColorAnimation)target;
				return;
			case 126:
				this.DraftsTab2SB = (Storyboard)target;
				return;
			case 127:
				this.DraftsTab2 = (ColorAnimation)target;
				return;
			case 128:
				this.DraftsTab13 = (ColorAnimation)target;
				return;
			case 129:
				this.DraftsTab3 = (ColorAnimation)target;
				return;
			case 130:
				this.DraftsTab14 = (ColorAnimation)target;
				return;
			case 131:
				this.DraftsTab4 = (ColorAnimation)target;
				return;
			case 132:
				this.DraftsTab5 = (ColorAnimation)target;
				return;
			case 133:
				this.DraftsTab6 = (ColorAnimation)target;
				return;
			case 134:
				this.DraftsTab7 = (ColorAnimation)target;
				return;
			case 135:
				this.DraftsTab15 = (ColorAnimation)target;
				return;
			case 136:
				this.DraftsTab8 = (ColorAnimation)target;
				return;
			case 137:
				this.DraftsTab9 = (ColorAnimation)target;
				return;
			case 138:
				this.DraftsTab10 = (ColorAnimation)target;
				return;
			case 139:
				this.svg210 = (Canvas)target;
				return;
			case 140:
				this.path208 = (System.Windows.Shapes.Path)target;
				return;
			case 141:
				this.DraftsTabLabel = (Label)target;
				return;
			case 142:
				this.CommunityTab = (Grid)target;
				this.CommunityTab.MouseLeftButtonDown += this.CommunityTab_MouseDown;
				return;
			case 143:
				this.CommunityTab1 = (ColorAnimation)target;
				return;
			case 144:
				this.CommunityTab2SB = (Storyboard)target;
				return;
			case 145:
				this.CommunityTab2 = (ColorAnimation)target;
				return;
			case 146:
				this.CommunityTab3 = (ColorAnimation)target;
				return;
			case 147:
				this.CommunityTab4 = (ColorAnimation)target;
				return;
			case 148:
				this.CommunityTab5 = (ColorAnimation)target;
				return;
			case 149:
				this.CommunityTab6 = (ColorAnimation)target;
				return;
			case 150:
				this.CommunityTab7 = (ColorAnimation)target;
				return;
			case 151:
				this.CommunityTab8 = (ColorAnimation)target;
				return;
			case 152:
				this.svg22 = (Canvas)target;
				return;
			case 153:
				this.path20 = (System.Windows.Shapes.Path)target;
				return;
			case 154:
				this.CommunityTabLabel = (Label)target;
				return;
			case 155:
				this.UpgradeTab = (Grid)target;
				this.UpgradeTab.MouseLeftButtonDown += this.UpgradeTab_MouseDown;
				return;
			case 156:
				this.UpgradeTab1 = (ColorAnimation)target;
				return;
			case 157:
				this.UpgradeTab2SB = (Storyboard)target;
				return;
			case 158:
				this.UpgradeTab2 = (ColorAnimation)target;
				return;
			case 159:
				this.UpgradeTab3 = (ColorAnimation)target;
				return;
			case 160:
				this.UpgradeTab4 = (ColorAnimation)target;
				return;
			case 161:
				this.UpgradeTab5 = (ColorAnimation)target;
				return;
			case 162:
				this.UpgradeTab6 = (ColorAnimation)target;
				return;
			case 163:
				this.UpgradeTab7 = (ColorAnimation)target;
				return;
			case 164:
				this.UpgradeTab8 = (ColorAnimation)target;
				return;
			case 165:
				this.UpgradeTab9 = (ColorAnimation)target;
				return;
			case 166:
				this.UpgradeTab10 = (ColorAnimation)target;
				return;
			case 167:
				this.svg46 = (Canvas)target;
				return;
			case 168:
				this.path42 = (System.Windows.Shapes.Path)target;
				return;
			case 169:
				this.path44 = (System.Windows.Shapes.Path)target;
				return;
			case 170:
				this.UpgradeTabLabel = (Label)target;
				return;
			case 171:
				this.NewDraft = (Grid)target;
				this.NewDraft.MouseLeftButtonUp += this.NewDraft_MouseLeftButtonUp;
				return;
			case 172:
				this.svg58 = (Canvas)target;
				return;
			case 173:
				this.path56 = (System.Windows.Shapes.Path)target;
				return;
			case 174:
				this.ManagedWrapper = (Grid)target;
				return;
			case 175:
				this.CardHolderScroller = (ScrollViewer)target;
				this.CardHolderScroller.MouseDown += this.KrnlWindow_MouseDown;
				this.CardHolderScroller.PreviewMouseWheel += this.ScrollViewer_PreviewMouseWheel;
				return;
			case 176:
				this.ObjectHolder = (Grid)target;
				return;
			case 177:
				this.CardHolder = (WrapPanel)target;
				return;
			case 178:
				this.FileContextMenu = (Border)target;
				this.FileContextMenu.LostFocus += this.FileContextMenu_LostFocus;
				return;
			case 179:
				this.OpenHBOpt = (Grid)target;
				this.OpenHBOpt.MouseLeftButtonUp += this.OpenHBOpt_MouseLeftButtonUp_1;
				return;
			case 180:
				this.TabTitle1 = (Label)target;
				return;
			case 181:
				this.RenameHBOpt = (Grid)target;
				this.RenameHBOpt.MouseLeftButtonUp += this.RenameHBOpt_MouseLeftButtonUp_1;
				return;
			case 182:
				this.TabTitle4 = (Label)target;
				return;
			case 183:
				this.DeleteHBOpt = (Grid)target;
				this.DeleteHBOpt.MouseLeftButtonUp += this.DeleteHBOpt_MouseLeftButtonUp_1;
				return;
			case 184:
				this.TabTitle10 = (Label)target;
				return;
			case 185:
				this.ExplorerHBOpt = (Grid)target;
				this.ExplorerHBOpt.MouseLeftButtonUp += this.ExplorerHBOpt_MouseLeftButtonUp;
				return;
			case 186:
				this.TabTitle23 = (Label)target;
				return;
			case 187:
				this.ExecuteHBOpt = (Grid)target;
				this.ExecuteHBOpt.MouseLeftButtonUp += this.ExecuteHBOpt_MouseLeftButtonUp_1;
				return;
			case 188:
				this.TabTitle26 = (Label)target;
				return;
			case 189:
				this.EditOpts = (Border)target;
				this.EditOpts.MouseEnter += this.EditOpts_MouseEnter;
				this.EditOpts.MouseLeave += this.EditOpts_MouseLeave;
				return;
			case 190:
				this.UndoOpt = (Grid)target;
				this.UndoOpt.MouseLeftButtonUp += this.UndoOpt_MouseLeftButtonUp;
				return;
			case 191:
				this.TabTitle12 = (Label)target;
				return;
			case 192:
				this.TabTitle12_Copy = (Label)target;
				return;
			case 193:
				this.RedoOpt = (Grid)target;
				this.RedoOpt.MouseLeftButtonUp += this.RedoOpt_MouseLeftButtonUp;
				return;
			case 194:
				this.TabTitle13 = (Label)target;
				return;
			case 195:
				this.TabTitle12_Copy1 = (Label)target;
				return;
			case 196:
				this.CutOpt = (Grid)target;
				this.CutOpt.MouseLeftButtonUp += this.CutOpt_MouseLeftButtonUp;
				return;
			case 197:
				this.TabTitle17 = (Label)target;
				return;
			case 198:
				this.TabTitle12_Copy2 = (Label)target;
				return;
			case 199:
				this.CopyOpt = (Grid)target;
				this.CopyOpt.MouseLeftButtonUp += this.CopyOpt_MouseLeftButtonUp;
				return;
			case 200:
				this.TabTitle18 = (Label)target;
				return;
			case 201:
				this.TabTitle12_Copy3 = (Label)target;
				return;
			case 202:
				this.PasteOpt = (Grid)target;
				this.PasteOpt.MouseLeftButtonUp += this.PasteOpt_MouseLeftButtonUp;
				return;
			case 203:
				this.TabTitle19 = (Label)target;
				return;
			case 204:
				this.TabTitle12_Copy4 = (Label)target;
				return;
			case 205:
				this.DeleteOpt = (Grid)target;
				this.DeleteOpt.MouseLeftButtonUp += this.DeleteOpt_MouseLeftButtonUp;
				return;
			case 206:
				this.TabTitle20 = (Label)target;
				return;
			case 207:
				this.TabTitle12_Copy5 = (Label)target;
				return;
			case 208:
				this.SelectAllOpt = (Grid)target;
				this.SelectAllOpt.MouseLeftButtonUp += this.SelectAllOpt_MouseLeftButtonUp;
				return;
			case 209:
				this.TabTitle21 = (Label)target;
				return;
			case 210:
				this.TabTitle12_Copy6 = (Label)target;
				return;
			case 211:
				this.PrefOpts = (Border)target;
				this.PrefOpts.MouseEnter += this.PrefOpts_MouseEnter;
				this.PrefOpts.MouseLeave += this.PrefOpts_MouseLeave;
				return;
			case 212:
				this.AutoAttachOpt = (Grid)target;
				this.AutoAttachOpt.MouseLeftButtonUp += this.AutoAttachOpt_MouseLeftButtonUp;
				return;
			case 213:
				this.TabTitle16 = (Label)target;
				return;
			case 214:
				this.svg242 = (Canvas)target;
				return;
			case 215:
				this.path230 = (System.Windows.Shapes.Path)target;
				return;
			case 216:
				this.AutoLaunchOpt = (Grid)target;
				this.AutoLaunchOpt.MouseLeftButtonUp += this.AutoLaunchOpt_MouseLeftButtonUp;
				return;
			case 217:
				this.TabTitle24 = (Label)target;
				return;
			case 218:
				this.svg3 = (Canvas)target;
				return;
			case 219:
				this.path4 = (System.Windows.Shapes.Path)target;
				return;
			case 220:
				this.TopmostOpt = (Grid)target;
				this.TopmostOpt.MouseLeftButtonUp += this.TopmostOpt_MouseLeftButtonUp;
				return;
			case 221:
				this.TabTitle15 = (Label)target;
				return;
			case 222:
				this.svg242_Copy = (Canvas)target;
				return;
			case 223:
				this.path1 = (System.Windows.Shapes.Path)target;
				return;
			case 224:
				this.UnlockFPSOpt = (Grid)target;
				this.UnlockFPSOpt.MouseLeftButtonUp += this.UnlockFPSOpt_MouseLeftButtonUp;
				return;
			case 225:
				this.TabTitle22 = (Label)target;
				return;
			case 226:
				this.svg242_Copy1 = (Canvas)target;
				return;
			case 227:
				this.path3 = (System.Windows.Shapes.Path)target;
				return;
			case 228:
				this.ViewOpts = (Border)target;
				this.ViewOpts.MouseEnter += this.ViewOpts_MouseEnter;
				this.ViewOpts.MouseLeave += this.ViewOpts_MouseLeave;
				return;
			case 229:
				this.MinimapOpt = (Grid)target;
				this.MinimapOpt.MouseLeftButtonUp += this.MinimapOpt_MouseLeftButtonUp;
				return;
			case 230:
				this.svg2422 = (Canvas)target;
				return;
			case 231:
				this.path2340 = (System.Windows.Shapes.Path)target;
				return;
			case 232:
				this.TabTitle14 = (Label)target;
				return;
			case 233:
				this.FileExecNotf = (Border)target;
				return;
			case 234:
				this.svg224 = (Canvas)target;
				return;
			case 235:
				this.path205 = (System.Windows.Shapes.Path)target;
				return;
			case 236:
				this.FileNotfText = (Label)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000002 RID: 2
		private string SiteUrl = "https://cdn.krnl.place/";

		// Token: 0x04000004 RID: 4
		private List<CommunityEntry> CommunityCards = new List<CommunityEntry>();

		// Token: 0x04000005 RID: 5
		private bool TabChanging;

		// Token: 0x04000006 RID: 6
		private string DllPath = Directory.GetCurrentDirectory() + string.Format("\\\\{0}", "krnl.dll");

		// Token: 0x04000007 RID: 7
		private bool DragAvailable = true;

		// Token: 0x04000008 RID: 8
		private bool AutoAttachEnabled;

		// Token: 0x04000009 RID: 9
		private bool isAutoAttached;

		// Token: 0x0400000A RID: 10
		private bool AutoLaunchEnabled = true;

		// Token: 0x0400000C RID: 12
		private int TabCount = 2;

		// Token: 0x0400000D RID: 13
		public bool MenuDown;

		// Token: 0x0400000F RID: 15
		private int FileHBOptOpen;

		// Token: 0x04000010 RID: 16
		private int EditHBOptOpen;

		// Token: 0x04000011 RID: 17
		private List<ValueTuple<string, string, string>> Scripts = new List<ValueTuple<string, string, string>>
		{
			new ValueTuple<string, string, string>("OpenGui", "loadstring(game:HttpGet('https://pastebin.com/raw/UXmbai5q', true))()", "stickmasterluke")
		};

		// Token: 0x04000013 RID: 19
		private string CurrentDraftPath = "Scripts";

		// Token: 0x04000015 RID: 21
		private bool isAnimatingNotf;

		// Token: 0x04000016 RID: 22
		private bool MinimapEnabled;

		// Token: 0x04000017 RID: 23
		private int PrefHBOptOpen;

		// Token: 0x04000018 RID: 24
		private int ViewHBOptOpen;

		// Token: 0x04000019 RID: 25
		private string roblox_path = Directory.GetParent(Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("ROBLOX Corporation").OpenSubKey("Environments").OpenSubKey("roblox-player").GetValue("").ToString()).FullName;

		// Token: 0x0400001A RID: 26
		private Mutex auto_launch_mutex;

		// Token: 0x0400001B RID: 27
		private bool Searchable = true;

		// Token: 0x0400001C RID: 28
		internal Grid Workspace;

		// Token: 0x0400001D RID: 29
		internal Border KrnlWindow;

		// Token: 0x0400001E RID: 30
		internal Grid Workspace1;

		// Token: 0x0400001F RID: 31
		internal StackPanel TabContainer;

		// Token: 0x04000020 RID: 32
		internal ScrollViewer MyScrollViewer;

		// Token: 0x04000021 RID: 33
		internal StackPanel TabFlow;

		// Token: 0x04000022 RID: 34
		internal Grid NewTabButton;

		// Token: 0x04000023 RID: 35
		internal Canvas svg454;

		// Token: 0x04000024 RID: 36
		internal System.Windows.Shapes.Path path452;

		// Token: 0x04000025 RID: 37
		internal StackPanel MinimizeButton;

		// Token: 0x04000026 RID: 38
		internal Canvas svg376;

		// Token: 0x04000027 RID: 39
		internal System.Windows.Shapes.Path path374;

		// Token: 0x04000028 RID: 40
		internal StackPanel MaximizeButton;

		// Token: 0x04000029 RID: 41
		internal Canvas svg356;

		// Token: 0x0400002A RID: 42
		internal Rectangle rect354;

		// Token: 0x0400002B RID: 43
		internal Canvas svg223;

		// Token: 0x0400002C RID: 44
		internal System.Windows.Shapes.Path path202;

		// Token: 0x0400002D RID: 45
		internal StackPanel ExitButton;

		// Token: 0x0400002E RID: 46
		internal Canvas svg276;

		// Token: 0x0400002F RID: 47
		internal System.Windows.Shapes.Path path274;

		// Token: 0x04000030 RID: 48
		internal Grid appIcon;

		// Token: 0x04000031 RID: 49
		internal Storyboard appIconAnimEnterIconSB;

		// Token: 0x04000032 RID: 50
		internal ColorAnimation appIconAnimEnterIcon;

		// Token: 0x04000033 RID: 51
		internal Storyboard appIconAnimEnterBackSB;

		// Token: 0x04000034 RID: 52
		internal ColorAnimation appIconAnimEnterBack;

		// Token: 0x04000035 RID: 53
		internal Storyboard appIconAnimLeaveIconSB;

		// Token: 0x04000036 RID: 54
		internal ColorAnimation appIconAnimLeaveIcon;

		// Token: 0x04000037 RID: 55
		internal Storyboard appIconAnimLeaveBackSB;

		// Token: 0x04000038 RID: 56
		internal ColorAnimation appIconAnimLeaveBack;

		// Token: 0x04000039 RID: 57
		internal Storyboard appIconAnimDownSB;

		// Token: 0x0400003A RID: 58
		internal ColorAnimation appIconAnimDown;

		// Token: 0x0400003B RID: 59
		internal Storyboard appIconAnimUpSB;

		// Token: 0x0400003C RID: 60
		internal ColorAnimation appIconAnimUp;

		// Token: 0x0400003D RID: 61
		internal System.Windows.Shapes.Path path2;

		// Token: 0x0400003E RID: 62
		internal Grid Editor;

		// Token: 0x0400003F RID: 63
		internal Grid LoadBar;

		// Token: 0x04000040 RID: 64
		internal Storyboard LoaderAnimStoryboard;

		// Token: 0x04000041 RID: 65
		internal ColorAnimation LoaderAnim;

		// Token: 0x04000042 RID: 66
		internal Grid HBOpts;

		// Token: 0x04000043 RID: 67
		internal Canvas svg233;

		// Token: 0x04000044 RID: 68
		internal System.Windows.Shapes.Path path212;

		// Token: 0x04000045 RID: 69
		internal Grid PrefHBOpt;

		// Token: 0x04000046 RID: 70
		internal Label TabTitle9;

		// Token: 0x04000047 RID: 71
		internal Canvas svg321_Copy5;

		// Token: 0x04000048 RID: 72
		internal System.Windows.Shapes.Path path9;

		// Token: 0x04000049 RID: 73
		internal Grid PrefHBOptGate;

		// Token: 0x0400004A RID: 74
		internal Grid FileHBOpt;

		// Token: 0x0400004B RID: 75
		internal Label TabTitle6;

		// Token: 0x0400004C RID: 76
		internal Canvas svg2;

		// Token: 0x0400004D RID: 77
		internal System.Windows.Shapes.Path path6;

		// Token: 0x0400004E RID: 78
		internal Grid FileHBOptsGate;

		// Token: 0x0400004F RID: 79
		internal Grid EditHBOpt;

		// Token: 0x04000050 RID: 80
		internal Label TabTitle7;

		// Token: 0x04000051 RID: 81
		internal Canvas svg321_Copy3;

		// Token: 0x04000052 RID: 82
		internal System.Windows.Shapes.Path path7;

		// Token: 0x04000053 RID: 83
		internal Grid EditHBOptGate;

		// Token: 0x04000054 RID: 84
		internal Grid ViewHBOpt;

		// Token: 0x04000055 RID: 85
		internal Label TabTitle8;

		// Token: 0x04000056 RID: 86
		internal Canvas svg321_Copy4;

		// Token: 0x04000057 RID: 87
		internal System.Windows.Shapes.Path path8;

		// Token: 0x04000058 RID: 88
		internal Grid ViewHBOptGate;

		// Token: 0x04000059 RID: 89
		internal Grid CloseHBOpt;

		// Token: 0x0400005A RID: 90
		internal Label TabTitle5;

		// Token: 0x0400005B RID: 91
		internal Canvas svg1;

		// Token: 0x0400005C RID: 92
		internal System.Windows.Shapes.Path path5;

		// Token: 0x0400005D RID: 93
		internal Border FileOpts;

		// Token: 0x0400005E RID: 94
		internal Grid OpenFileOpt;

		// Token: 0x0400005F RID: 95
		internal Label TabTitle2;

		// Token: 0x04000060 RID: 96
		internal Label TabTitle22_Copy1;

		// Token: 0x04000061 RID: 97
		internal Grid SaveOpt;

		// Token: 0x04000062 RID: 98
		internal Label TabTitle11;

		// Token: 0x04000063 RID: 99
		internal Label TabTitle22_Copy2;

		// Token: 0x04000064 RID: 100
		internal Grid SaveFileOpt;

		// Token: 0x04000065 RID: 101
		internal Label TabTitle3;

		// Token: 0x04000066 RID: 102
		internal Label TabTitle22_Copy3;

		// Token: 0x04000067 RID: 103
		internal Grid OpenKrnlOpt;

		// Token: 0x04000068 RID: 104
		internal Label TabTitle25;

		// Token: 0x04000069 RID: 105
		internal Grid menuOpt;

		// Token: 0x0400006A RID: 106
		internal Canvas svg95;

		// Token: 0x0400006B RID: 107
		internal System.Windows.Shapes.Path path93;

		// Token: 0x0400006C RID: 108
		internal Canvas svg42;

		// Token: 0x0400006D RID: 109
		internal System.Windows.Shapes.Path path40;

		// Token: 0x0400006E RID: 110
		internal Canvas g81;

		// Token: 0x0400006F RID: 111
		internal System.Windows.Shapes.Path path70;

		// Token: 0x04000070 RID: 112
		internal Grid injectOpt;

		// Token: 0x04000071 RID: 113
		internal Canvas svg296;

		// Token: 0x04000072 RID: 114
		internal System.Windows.Shapes.Path connect;

		// Token: 0x04000073 RID: 115
		internal Grid executeOpt;

		// Token: 0x04000074 RID: 116
		internal Canvas svg396;

		// Token: 0x04000075 RID: 117
		internal System.Windows.Shapes.Path path394;

		// Token: 0x04000076 RID: 118
		internal Grid MainMenu;

		// Token: 0x04000077 RID: 119
		internal Grid menuOpt1;

		// Token: 0x04000078 RID: 120
		internal Canvas svg304;

		// Token: 0x04000079 RID: 121
		internal System.Windows.Shapes.Path path300;

		// Token: 0x0400007A RID: 122
		internal System.Windows.Shapes.Path path302;

		// Token: 0x0400007B RID: 123
		internal Label Introduct;

		// Token: 0x0400007C RID: 124
		internal Border SearchBorder;

		// Token: 0x0400007D RID: 125
		internal TextBox SearchInput;

		// Token: 0x0400007E RID: 126
		internal Grid SearchGlass;

		// Token: 0x0400007F RID: 127
		internal Canvas svg28;

		// Token: 0x04000080 RID: 128
		internal Ellipse circle24;

		// Token: 0x04000081 RID: 129
		internal System.Windows.Shapes.Path path26;

		// Token: 0x04000082 RID: 130
		internal StackPanel FolderDisplayer;

		// Token: 0x04000083 RID: 131
		internal FolderDisplay MainDirDisplay;

		// Token: 0x04000084 RID: 132
		internal Grid RecentTab;

		// Token: 0x04000085 RID: 133
		internal ColorAnimation RecentTab1;

		// Token: 0x04000086 RID: 134
		internal Storyboard RecentTab2SB;

		// Token: 0x04000087 RID: 135
		internal ColorAnimation RecentTab2;

		// Token: 0x04000088 RID: 136
		internal ColorAnimation RecentTab3;

		// Token: 0x04000089 RID: 137
		internal ColorAnimation RecentTab4;

		// Token: 0x0400008A RID: 138
		internal ColorAnimation RecentTab5;

		// Token: 0x0400008B RID: 139
		internal ColorAnimation RecentTab6;

		// Token: 0x0400008C RID: 140
		internal ColorAnimation RecentTab7;

		// Token: 0x0400008D RID: 141
		internal ColorAnimation RecentTab8;

		// Token: 0x0400008E RID: 142
		internal ColorAnimation RecentTab9;

		// Token: 0x0400008F RID: 143
		internal ColorAnimation RecentTab10;

		// Token: 0x04000090 RID: 144
		internal Canvas svg23;

		// Token: 0x04000091 RID: 145
		internal System.Windows.Shapes.Path circle19;

		// Token: 0x04000092 RID: 146
		internal System.Windows.Shapes.Path path21;

		// Token: 0x04000093 RID: 147
		internal Label RecentTabLabel;

		// Token: 0x04000094 RID: 148
		internal Grid DraftsTab;

		// Token: 0x04000095 RID: 149
		internal ColorAnimation DraftsTab1;

		// Token: 0x04000096 RID: 150
		internal ColorAnimation DraftsTab12;

		// Token: 0x04000097 RID: 151
		internal Storyboard DraftsTab2SB;

		// Token: 0x04000098 RID: 152
		internal ColorAnimation DraftsTab2;

		// Token: 0x04000099 RID: 153
		internal ColorAnimation DraftsTab13;

		// Token: 0x0400009A RID: 154
		internal ColorAnimation DraftsTab3;

		// Token: 0x0400009B RID: 155
		internal ColorAnimation DraftsTab14;

		// Token: 0x0400009C RID: 156
		internal ColorAnimation DraftsTab4;

		// Token: 0x0400009D RID: 157
		internal ColorAnimation DraftsTab5;

		// Token: 0x0400009E RID: 158
		internal ColorAnimation DraftsTab6;

		// Token: 0x0400009F RID: 159
		internal ColorAnimation DraftsTab7;

		// Token: 0x040000A0 RID: 160
		internal ColorAnimation DraftsTab15;

		// Token: 0x040000A1 RID: 161
		internal ColorAnimation DraftsTab8;

		// Token: 0x040000A2 RID: 162
		internal ColorAnimation DraftsTab9;

		// Token: 0x040000A3 RID: 163
		internal ColorAnimation DraftsTab10;

		// Token: 0x040000A4 RID: 164
		internal Canvas svg210;

		// Token: 0x040000A5 RID: 165
		internal System.Windows.Shapes.Path path208;

		// Token: 0x040000A6 RID: 166
		internal Label DraftsTabLabel;

		// Token: 0x040000A7 RID: 167
		internal Grid CommunityTab;

		// Token: 0x040000A8 RID: 168
		internal ColorAnimation CommunityTab1;

		// Token: 0x040000A9 RID: 169
		internal Storyboard CommunityTab2SB;

		// Token: 0x040000AA RID: 170
		internal ColorAnimation CommunityTab2;

		// Token: 0x040000AB RID: 171
		internal ColorAnimation CommunityTab3;

		// Token: 0x040000AC RID: 172
		internal ColorAnimation CommunityTab4;

		// Token: 0x040000AD RID: 173
		internal ColorAnimation CommunityTab5;

		// Token: 0x040000AE RID: 174
		internal ColorAnimation CommunityTab6;

		// Token: 0x040000AF RID: 175
		internal ColorAnimation CommunityTab7;

		// Token: 0x040000B0 RID: 176
		internal ColorAnimation CommunityTab8;

		// Token: 0x040000B1 RID: 177
		internal Canvas svg22;

		// Token: 0x040000B2 RID: 178
		internal System.Windows.Shapes.Path path20;

		// Token: 0x040000B3 RID: 179
		internal Label CommunityTabLabel;

		// Token: 0x040000B4 RID: 180
		internal Grid UpgradeTab;

		// Token: 0x040000B5 RID: 181
		internal ColorAnimation UpgradeTab1;

		// Token: 0x040000B6 RID: 182
		internal Storyboard UpgradeTab2SB;

		// Token: 0x040000B7 RID: 183
		internal ColorAnimation UpgradeTab2;

		// Token: 0x040000B8 RID: 184
		internal ColorAnimation UpgradeTab3;

		// Token: 0x040000B9 RID: 185
		internal ColorAnimation UpgradeTab4;

		// Token: 0x040000BA RID: 186
		internal ColorAnimation UpgradeTab5;

		// Token: 0x040000BB RID: 187
		internal ColorAnimation UpgradeTab6;

		// Token: 0x040000BC RID: 188
		internal ColorAnimation UpgradeTab7;

		// Token: 0x040000BD RID: 189
		internal ColorAnimation UpgradeTab8;

		// Token: 0x040000BE RID: 190
		internal ColorAnimation UpgradeTab9;

		// Token: 0x040000BF RID: 191
		internal ColorAnimation UpgradeTab10;

		// Token: 0x040000C0 RID: 192
		internal Canvas svg46;

		// Token: 0x040000C1 RID: 193
		internal System.Windows.Shapes.Path path42;

		// Token: 0x040000C2 RID: 194
		internal System.Windows.Shapes.Path path44;

		// Token: 0x040000C3 RID: 195
		internal Label UpgradeTabLabel;

		// Token: 0x040000C4 RID: 196
		internal Grid NewDraft;

		// Token: 0x040000C5 RID: 197
		internal Canvas svg58;

		// Token: 0x040000C6 RID: 198
		internal System.Windows.Shapes.Path path56;

		// Token: 0x040000C7 RID: 199
		internal Grid ManagedWrapper;

		// Token: 0x040000C8 RID: 200
		internal ScrollViewer CardHolderScroller;

		// Token: 0x040000C9 RID: 201
		internal Grid ObjectHolder;

		// Token: 0x040000CA RID: 202
		internal WrapPanel CardHolder;

		// Token: 0x040000CB RID: 203
		internal Border FileContextMenu;

		// Token: 0x040000CC RID: 204
		internal Grid OpenHBOpt;

		// Token: 0x040000CD RID: 205
		internal Label TabTitle1;

		// Token: 0x040000CE RID: 206
		internal Grid RenameHBOpt;

		// Token: 0x040000CF RID: 207
		internal Label TabTitle4;

		// Token: 0x040000D0 RID: 208
		internal Grid DeleteHBOpt;

		// Token: 0x040000D1 RID: 209
		internal Label TabTitle10;

		// Token: 0x040000D2 RID: 210
		internal Grid ExplorerHBOpt;

		// Token: 0x040000D3 RID: 211
		internal Label TabTitle23;

		// Token: 0x040000D4 RID: 212
		internal Grid ExecuteHBOpt;

		// Token: 0x040000D5 RID: 213
		internal Label TabTitle26;

		// Token: 0x040000D6 RID: 214
		internal Border EditOpts;

		// Token: 0x040000D7 RID: 215
		internal Grid UndoOpt;

		// Token: 0x040000D8 RID: 216
		internal Label TabTitle12;

		// Token: 0x040000D9 RID: 217
		internal Label TabTitle12_Copy;

		// Token: 0x040000DA RID: 218
		internal Grid RedoOpt;

		// Token: 0x040000DB RID: 219
		internal Label TabTitle13;

		// Token: 0x040000DC RID: 220
		internal Label TabTitle12_Copy1;

		// Token: 0x040000DD RID: 221
		internal Grid CutOpt;

		// Token: 0x040000DE RID: 222
		internal Label TabTitle17;

		// Token: 0x040000DF RID: 223
		internal Label TabTitle12_Copy2;

		// Token: 0x040000E0 RID: 224
		internal Grid CopyOpt;

		// Token: 0x040000E1 RID: 225
		internal Label TabTitle18;

		// Token: 0x040000E2 RID: 226
		internal Label TabTitle12_Copy3;

		// Token: 0x040000E3 RID: 227
		internal Grid PasteOpt;

		// Token: 0x040000E4 RID: 228
		internal Label TabTitle19;

		// Token: 0x040000E5 RID: 229
		internal Label TabTitle12_Copy4;

		// Token: 0x040000E6 RID: 230
		internal Grid DeleteOpt;

		// Token: 0x040000E7 RID: 231
		internal Label TabTitle20;

		// Token: 0x040000E8 RID: 232
		internal Label TabTitle12_Copy5;

		// Token: 0x040000E9 RID: 233
		internal Grid SelectAllOpt;

		// Token: 0x040000EA RID: 234
		internal Label TabTitle21;

		// Token: 0x040000EB RID: 235
		internal Label TabTitle12_Copy6;

		// Token: 0x040000EC RID: 236
		internal Border PrefOpts;

		// Token: 0x040000ED RID: 237
		internal Grid AutoAttachOpt;

		// Token: 0x040000EE RID: 238
		internal Label TabTitle16;

		// Token: 0x040000EF RID: 239
		internal Canvas svg242;

		// Token: 0x040000F0 RID: 240
		internal System.Windows.Shapes.Path path230;

		// Token: 0x040000F1 RID: 241
		internal Grid AutoLaunchOpt;

		// Token: 0x040000F2 RID: 242
		internal Label TabTitle24;

		// Token: 0x040000F3 RID: 243
		internal Canvas svg3;

		// Token: 0x040000F4 RID: 244
		internal System.Windows.Shapes.Path path4;

		// Token: 0x040000F5 RID: 245
		internal Grid TopmostOpt;

		// Token: 0x040000F6 RID: 246
		internal Label TabTitle15;

		// Token: 0x040000F7 RID: 247
		internal Canvas svg242_Copy;

		// Token: 0x040000F8 RID: 248
		internal System.Windows.Shapes.Path path1;

		// Token: 0x040000F9 RID: 249
		internal Grid UnlockFPSOpt;

		// Token: 0x040000FA RID: 250
		internal Label TabTitle22;

		// Token: 0x040000FB RID: 251
		internal Canvas svg242_Copy1;

		// Token: 0x040000FC RID: 252
		internal System.Windows.Shapes.Path path3;

		// Token: 0x040000FD RID: 253
		internal Border ViewOpts;

		// Token: 0x040000FE RID: 254
		internal Grid MinimapOpt;

		// Token: 0x040000FF RID: 255
		internal Canvas svg2422;

		// Token: 0x04000100 RID: 256
		internal System.Windows.Shapes.Path path2340;

		// Token: 0x04000101 RID: 257
		internal Label TabTitle14;

		// Token: 0x04000102 RID: 258
		internal Border FileExecNotf;

		// Token: 0x04000103 RID: 259
		internal Canvas svg224;

		// Token: 0x04000104 RID: 260
		internal System.Windows.Shapes.Path path205;

		// Token: 0x04000105 RID: 261
		internal Label FileNotfText;

		// Token: 0x04000106 RID: 262
		private bool _contentLoaded;
	}
}
