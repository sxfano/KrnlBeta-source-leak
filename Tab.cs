using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace KrnlUI.Controls
{
	// Token: 0x0200000F RID: 15
	public class Tab : UserControl, IComponentConnector
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x000081E5 File Offset: 0x000063E5
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x000081F0 File Offset: 0x000063F0
		public bool IsSaved
		{
			get
			{
				return this.isSaved;
			}
			set
			{
				if (value && !this.isSaved)
				{
					if (this.TabName[0] == '*')
					{
						this.TabName = this.TabName.Remove(0, 1);
					}
				}
				else if (this.isSaved)
				{
					this.TabName = "*" + this.TabName;
				}
				this.isSaved = value;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00008254 File Offset: 0x00006454
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x00008288 File Offset: 0x00006488
		public string TabName
		{
			get
			{
				string text = this.TabTitle.Content.ToString();
				if (text[0] == '*')
				{
					text = text.Remove(0, 1);
				}
				return text;
			}
			set
			{
				this.isSaved = false;
				this.IsSaved = true;
				FormattedText formattedText = new FormattedText(value, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(this.TabTitle.FontFamily, this.TabTitle.FontStyle, this.TabTitle.FontWeight, this.TabTitle.FontStretch), this.TabTitle.FontSize, Brushes.Black, new NumberSubstitution(), 1.0);
				this.passWidth = 78;
				if (formattedText.Width > 42.0)
				{
					if (formattedText.Width < 122.0 || base.Width < 122.0)
					{
						int num = (int)formattedText.Width - 42;
						this.passWidth = 78 + num;
						this.TabTitle.Width += (double)num;
					}
					else
					{
						this.passWidth = 122;
						this.TabTitle.Width += 122.0;
					}
				}
				Storyboard storyboard = new Storyboard();
				DoubleAnimation doubleAnimation = new DoubleAnimation();
				storyboard.Children.Add(doubleAnimation);
				doubleAnimation.From = new double?(base.Width);
				doubleAnimation.To = new double?((double)this.passWidth);
				doubleAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
				doubleAnimation.DecelerationRatio = 0.4;
				Storyboard.SetTarget(doubleAnimation, this);
				Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Width", Array.Empty<object>()));
				storyboard.Begin();
				int defaultedNameTabNr;
				if (value.StartsWith("Untitled ") && value.Length > 9 && int.TryParse(value.Remove(0, 9), out defaultedNameTabNr))
				{
					this.DefaultedNameTabNr = defaultedNameTabNr;
				}
				this.TabTitle.Content = value;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00008444 File Offset: 0x00006644
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x0000844C File Offset: 0x0000664C
		public new StackPanel Parent { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00008455 File Offset: 0x00006655
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x0000845D File Offset: 0x0000665D
		public MainWindow MainInstance { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00008466 File Offset: 0x00006666
		// (set) Token: 0x060000EB RID: 235 RVA: 0x0000846E File Offset: 0x0000666E
		public FileWatcher fileWatcher { get; set; }

		// Token: 0x060000EC RID: 236 RVA: 0x00008478 File Offset: 0x00006678
		public Tab(StackPanel Parent, MainWindow Instance)
		{
			this.InitializeComponent();
			this.Parent = Parent;
			this.MainInstance = Instance;
			this.fileWatcher = new FileWatcher(this);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000084C8 File Offset: 0x000066C8
		public void Select()
		{
			this.isSelected = true;
			if (this.MainInstance.MenuDown)
			{
				this.MainInstance.LastTab = this;
			}
			this.MainInstance.BringUpMenu(true);
			if (Common.SelectedTab != null)
			{
				if (Common.SelectedTab != this)
				{
					Common.SelectedTab.TabTitle.Visibility = Visibility.Visible;
					Common.SelectedTab.EntryEditName.Visibility = Visibility.Hidden;
				}
				Common.SelectedTab.Script = this.MainInstance.ReadScript().ToString();
				Common.SelectedTab.Deselect();
			}
			this.MainInstance.WriteScript(this.Script, true);
			Common.SelectedTab = this;
			Storyboard storyboard = new Storyboard();
			ColorAnimation colorAnimation = new ColorAnimation();
			storyboard.Children.Add(colorAnimation);
			colorAnimation.From = new Color?(Color.FromRgb(122, 122, 122));
			colorAnimation.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			colorAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
			colorAnimation.DecelerationRatio = 0.4;
			Storyboard.SetTarget(colorAnimation, this.TabTitle);
			Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("Foreground.Color", Array.Empty<object>()));
			this.MouseEnterBackground.To = new Color?(Color.FromRgb(44, 44, 44));
			this.MouseLeaveBackground.To = new Color?(Color.FromRgb(44, 44, 44));
			this.MouseEnterBackgroundAnim.Begin();
			storyboard.Begin();
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00008644 File Offset: 0x00006844
		public void Deselect()
		{
			this.isSelected = false;
			Common.SelectedTab = null;
			Common.PreviousTab = this;
			Storyboard storyboard = new Storyboard();
			ColorAnimation colorAnimation = new ColorAnimation();
			storyboard.Children.Add(colorAnimation);
			colorAnimation.From = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			colorAnimation.To = new Color?(Color.FromRgb(122, 122, 122));
			colorAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
			colorAnimation.DecelerationRatio = 0.4;
			Storyboard.SetTarget(colorAnimation, this.TabTitle);
			Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("Foreground.Color", Array.Empty<object>()));
			this.MouseEnterBackground.To = new Color?(Color.FromRgb(39, 39, 39));
			this.MouseLeaveBackground.To = new Color?(Color.FromRgb(34, 34, 34));
			this.MouseLeaveBackgroundAnim.Begin();
			storyboard.Begin();
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000873C File Offset: 0x0000693C
		private void MainTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point position = e.GetPosition(this.svg434);
			if (position.X < 0.0 || position.X >= 24.0 || position.Y < 0.0 || position.Y >= 24.0)
			{
				this.Select();
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x000087A4 File Offset: 0x000069A4
		public bool PromptSaveDialog(Tab tab)
		{
			tab.Cancelled = false;
			if (tab.TabTitle.Content.ToString().IndexOf('*') != 0 || !(tab.TabName != tab.TabTitle.Content.ToString()))
			{
				return true;
			}
			MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to save the changes you made to " + tab.TabName + "?\n\nYour changes will be lost if you don't save them.", "KRNL", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
			if (messageBoxResult == MessageBoxResult.Cancel)
			{
				tab.Cancelled = true;
				return false;
			}
			if (messageBoxResult == MessageBoxResult.No)
			{
				return true;
			}
			this.MainInstance.HBOpts.Visibility = Visibility.Hidden;
			this.MainInstance.FileOpts.Visibility = Visibility.Hidden;
			if (tab.Path == "" || !File.Exists(tab.Path))
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.DefaultExt = "lua";
				saveFileDialog.Filter = "Lua files (*.lua)|*.lua";
				bool? flag = saveFileDialog.ShowDialog();
				bool flag2 = true;
				if (flag.GetValueOrDefault() == flag2 & flag != null)
				{
					tab.TabName = saveFileDialog.SafeFileName;
					tab.Path = saveFileDialog.FileName;
					File.WriteAllText(saveFileDialog.FileName, tab.Script);
					tab.isSaved = false;
					tab.IsSaved = true;
				}
			}
			else
			{
				File.WriteAllText(tab.Path, tab.Script);
			}
			return true;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x000088F4 File Offset: 0x00006AF4
		public void svg434_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!this.PromptSaveDialog(this))
			{
				if (e != null)
				{
					e.Handled = true;
				}
				return;
			}
			this.isDeleted = true;
			if (Common.SelectedTab == this)
			{
				this.Script = this.MainInstance.ReadScript().ToString();
			}
			if (e != null && Common.SelectedTab == this)
			{
				int num = this.Parent.Children.IndexOf(this);
				if (this.Parent.Children.Count > 2)
				{
					if (num > 0)
					{
						((Tab)this.Parent.Children[num - 1]).Select();
					}
					else if (this.Parent.Children.Count - num > 2)
					{
						((Tab)this.Parent.Children[num + 1]).Select();
					}
				}
			}
			if (Common.SelectedTab == this)
			{
				this.MainInstance.SaveRecent(this.TabName, this.MainInstance.ReadScript());
			}
			else
			{
				this.MainInstance.SaveRecent(this.TabName, this.Script);
			}
			this.MainInstance.TabFlow.Children.Remove(this);
			if (this.MainInstance.TabFlow.Children.Count <= 1)
			{
				this.MainInstance.BringDownMenu();
			}
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00008A38 File Offset: 0x00006C38
		private void TabCanvas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.EntryEditName.Focus();
			this.EntryEditName.SelectAll();
			Keyboard.Focus(this.EntryEditName);
			this.TabTitle.Visibility = Visibility.Hidden;
			this.EntryEditName.Visibility = Visibility.Visible;
			this.EntryEditName.Text = this.TabName;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00008A94 File Offset: 0x00006C94
		private void EntryEditName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.TabTitle.Visibility = Visibility.Visible;
				this.EntryEditName.Visibility = Visibility.Hidden;
				this.TabName = "*" + this.EntryEditName.Text;
				return;
			}
			if (e.Key == Key.Escape)
			{
				this.TabTitle.Visibility = Visibility.Visible;
				this.EntryEditName.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00008B00 File Offset: 0x00006D00
		private void EntryEditName_LostFocus(object sender, RoutedEventArgs e)
		{
			this.TabTitle.Visibility = Visibility.Visible;
			this.EntryEditName.Visibility = Visibility.Hidden;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00008B1A File Offset: 0x00006D1A
		private void MainTab_GotFocus(object sender, RoutedEventArgs e)
		{
			if (this.EntryEditName.Visibility == Visibility.Visible)
			{
				this.EntryEditName.Focus();
			}
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00008B38 File Offset: 0x00006D38
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/KrnlUI;component/controls/tab.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00008B68 File Offset: 0x00006D68
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.TabCanvas = (Tab)target;
				this.TabCanvas.MouseDoubleClick += this.TabCanvas_MouseDoubleClick;
				return;
			case 2:
				this.MainTab = (Grid)target;
				this.MainTab.MouseLeftButtonDown += this.MainTab_MouseLeftButtonDown;
				this.MainTab.GotFocus += this.MainTab_GotFocus;
				return;
			case 3:
				this.TabTitle = (Label)target;
				return;
			case 4:
				this.svg434 = (Canvas)target;
				this.svg434.MouseLeftButtonUp += this.svg434_MouseLeftButtonUp;
				return;
			case 5:
				this.path432 = (System.Windows.Shapes.Path)target;
				return;
			case 6:
				this.EntryEditName = (TextBox)target;
				this.EntryEditName.PreviewKeyDown += this.EntryEditName_PreviewKeyDown;
				this.EntryEditName.LostFocus += this.EntryEditName_LostFocus;
				return;
			case 7:
				this.MouseEnterClose = (ColorAnimation)target;
				return;
			case 8:
				this.MouseEnterBackgroundAnim = (Storyboard)target;
				return;
			case 9:
				this.MouseEnterBackground = (ColorAnimation)target;
				return;
			case 10:
				this.MouseLeaveClose = (ColorAnimation)target;
				return;
			case 11:
				this.MouseLeaveBackgroundAnim = (Storyboard)target;
				return;
			case 12:
				this.MouseLeaveBackground = (ColorAnimation)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000147 RID: 327
		public bool isSelected;

		// Token: 0x04000148 RID: 328
		public bool Cancelled;

		// Token: 0x04000149 RID: 329
		public string Script = "\"\"";

		// Token: 0x0400014A RID: 330
		public string Path = "";

		// Token: 0x0400014B RID: 331
		public bool isWatched;

		// Token: 0x0400014C RID: 332
		public bool watchSave;

		// Token: 0x0400014D RID: 333
		public bool isDeleted;

		// Token: 0x0400014E RID: 334
		public int DefaultedNameTabNr;

		// Token: 0x0400014F RID: 335
		private int passWidth;

		// Token: 0x04000150 RID: 336
		public bool isSaved = true;

		// Token: 0x04000154 RID: 340
		internal Tab TabCanvas;

		// Token: 0x04000155 RID: 341
		internal Grid MainTab;

		// Token: 0x04000156 RID: 342
		internal Label TabTitle;

		// Token: 0x04000157 RID: 343
		internal Canvas svg434;

		// Token: 0x04000158 RID: 344
		internal System.Windows.Shapes.Path path432;

		// Token: 0x04000159 RID: 345
		internal TextBox EntryEditName;

		// Token: 0x0400015A RID: 346
		internal ColorAnimation MouseEnterClose;

		// Token: 0x0400015B RID: 347
		internal Storyboard MouseEnterBackgroundAnim;

		// Token: 0x0400015C RID: 348
		internal ColorAnimation MouseEnterBackground;

		// Token: 0x0400015D RID: 349
		internal ColorAnimation MouseLeaveClose;

		// Token: 0x0400015E RID: 350
		internal Storyboard MouseLeaveBackgroundAnim;

		// Token: 0x0400015F RID: 351
		internal ColorAnimation MouseLeaveBackground;

		// Token: 0x04000160 RID: 352
		private bool _contentLoaded;
	}
}
