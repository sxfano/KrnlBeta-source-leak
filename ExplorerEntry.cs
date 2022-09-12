using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace KrnlUI.Controls
{
	// Token: 0x0200000C RID: 12
	public class ExplorerEntry : UserControl, IComponentConnector
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00007903 File Offset: 0x00005B03
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x0000790B File Offset: 0x00005B0B
		private MainWindow window { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00007914 File Offset: 0x00005B14
		// (set) Token: 0x060000CA RID: 202 RVA: 0x0000791C File Offset: 0x00005B1C
		public string Script { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00007925 File Offset: 0x00005B25
		// (set) Token: 0x060000CC RID: 204 RVA: 0x0000792D File Offset: 0x00005B2D
		public string Path { get; set; }

		// Token: 0x060000CD RID: 205 RVA: 0x00007936 File Offset: 0x00005B36
		public ExplorerEntry(MainWindow window)
		{
			this.InitializeComponent();
			this.window = window;
			this.Script = "";
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00007960 File Offset: 0x00005B60
		public void SetLuaTheme()
		{
			this.FolderIcon.Visibility = Visibility.Hidden;
			this.FolderIcon2.Visibility = Visibility.Hidden;
			this.LuaIcon.Visibility = Visibility.Visible;
			this.LuaIcon2.Visibility = Visibility.Visible;
			this.BorderBack.Background = new SolidColorBrush(Color.FromRgb(24, 160, 251));
			this.isFile = true;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000079C8 File Offset: 0x00005BC8
		public void SetFolderTheme()
		{
			this.FolderIcon.Visibility = Visibility.Visible;
			this.FolderIcon2.Visibility = Visibility.Visible;
			this.LuaIcon.Visibility = Visibility.Hidden;
			this.LuaIcon2.Visibility = Visibility.Hidden;
			this.BorderBack.Background = new SolidColorBrush(Color.FromRgb(44, 44, 44));
			this.isFile = false;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00007A27 File Offset: 0x00005C27
		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.Hand;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00007A34 File Offset: 0x00005C34
		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.Arrow;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00007A41 File Offset: 0x00005C41
		private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.window.FileContextMenu.Visibility = Visibility.Hidden;
			this.Select();
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00007A5C File Offset: 0x00005C5C
		public void Select()
		{
			if (this.window.SelectedExpEntry != null)
			{
				this.window.SelectedExpEntry.CardBorderDown.To = new Color?(Color.FromArgb(17, byte.MaxValue, byte.MaxValue, byte.MaxValue));
				this.window.SelectedExpEntry.CardBorderUp.To = new Color?(Color.FromArgb(17, byte.MaxValue, byte.MaxValue, byte.MaxValue));
				this.window.SelectedExpEntry.CardBorderDownSB.Begin();
			}
			this.CardBorderDown.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CardBorderUp.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CardBorderDownSB.Begin();
			this.window.SelectedExpEntry = this;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00007B44 File Offset: 0x00005D44
		private void EntryEdit_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.EntryEdit.Text == "")
			{
				this.EntryName.Visibility = Visibility.Visible;
				this.EntryEdit.Visibility = Visibility.Hidden;
				return;
			}
			this.EntryName.Content = this.EntryEdit.Text;
			this.EntryName.Visibility = Visibility.Visible;
			this.EntryEdit.Visibility = Visibility.Hidden;
			string text = System.IO.Path.GetDirectoryName(this.Path) + string.Format("\\{0}", this.EntryName.Content);
			if (this.isFile)
			{
				if (!File.Exists(text))
				{
					File.Move(this.Path, text);
				}
			}
			else if (!Directory.Exists(text))
			{
				Directory.Move(this.Path, text);
			}
			this.Path = text;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00007C10 File Offset: 0x00005E10
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/KrnlUI;component/controls/explorerentry.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00007C40 File Offset: 0x00005E40
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((ExplorerEntry)target).MouseDown += this.UserControl_MouseLeftButtonUp;
				return;
			case 2:
				this.CardBorderDownSB = (Storyboard)target;
				return;
			case 3:
				this.CardBorderDown = (ColorAnimation)target;
				return;
			case 4:
				this.CardBorderUpSB = (Storyboard)target;
				return;
			case 5:
				this.CardBorderUp = (ColorAnimation)target;
				return;
			case 6:
				this.CardBorder = (Border)target;
				return;
			case 7:
				this.BorderBack = (Border)target;
				return;
			case 8:
				this.LuaIcon2 = (Canvas)target;
				return;
			case 9:
				this.circle24 = (Ellipse)target;
				return;
			case 10:
				this.path26 = (System.Windows.Shapes.Path)target;
				return;
			case 11:
				this.circle28 = (Ellipse)target;
				return;
			case 12:
				this.FolderIcon2 = (Canvas)target;
				return;
			case 13:
				this.g1 = (Canvas)target;
				return;
			case 14:
				this.rect1 = (Rectangle)target;
				return;
			case 15:
				this.g2 = (Canvas)target;
				return;
			case 16:
				this.path1 = (System.Windows.Shapes.Path)target;
				return;
			case 17:
				this.FolderIcon = (Canvas)target;
				return;
			case 18:
				this.g177 = (Canvas)target;
				return;
			case 19:
				this.rect166 = (Rectangle)target;
				return;
			case 20:
				this.g210 = (Canvas)target;
				return;
			case 21:
				this.path199 = (System.Windows.Shapes.Path)target;
				return;
			case 22:
				this.EntryName = (Label)target;
				return;
			case 23:
				this.EntryEditstamp = (Label)target;
				return;
			case 24:
				this.LuaIcon = (Canvas)target;
				return;
			case 25:
				this.circle1 = (Ellipse)target;
				return;
			case 26:
				this.path2 = (System.Windows.Shapes.Path)target;
				return;
			case 27:
				this.circle2 = (Ellipse)target;
				return;
			case 28:
				this.EntryEdit = (TextBox)target;
				this.EntryEdit.LostFocus += this.EntryEdit_LostFocus;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400011F RID: 287
		public bool isFile = true;

		// Token: 0x04000120 RID: 288
		internal Storyboard CardBorderDownSB;

		// Token: 0x04000121 RID: 289
		internal ColorAnimation CardBorderDown;

		// Token: 0x04000122 RID: 290
		internal Storyboard CardBorderUpSB;

		// Token: 0x04000123 RID: 291
		internal ColorAnimation CardBorderUp;

		// Token: 0x04000124 RID: 292
		internal Border CardBorder;

		// Token: 0x04000125 RID: 293
		internal Border BorderBack;

		// Token: 0x04000126 RID: 294
		internal Canvas LuaIcon2;

		// Token: 0x04000127 RID: 295
		internal Ellipse circle24;

		// Token: 0x04000128 RID: 296
		internal System.Windows.Shapes.Path path26;

		// Token: 0x04000129 RID: 297
		internal Ellipse circle28;

		// Token: 0x0400012A RID: 298
		internal Canvas FolderIcon2;

		// Token: 0x0400012B RID: 299
		internal Canvas g1;

		// Token: 0x0400012C RID: 300
		internal Rectangle rect1;

		// Token: 0x0400012D RID: 301
		internal Canvas g2;

		// Token: 0x0400012E RID: 302
		internal System.Windows.Shapes.Path path1;

		// Token: 0x0400012F RID: 303
		internal Canvas FolderIcon;

		// Token: 0x04000130 RID: 304
		internal Canvas g177;

		// Token: 0x04000131 RID: 305
		internal Rectangle rect166;

		// Token: 0x04000132 RID: 306
		internal Canvas g210;

		// Token: 0x04000133 RID: 307
		internal System.Windows.Shapes.Path path199;

		// Token: 0x04000134 RID: 308
		internal Label EntryName;

		// Token: 0x04000135 RID: 309
		internal Label EntryEditstamp;

		// Token: 0x04000136 RID: 310
		internal Canvas LuaIcon;

		// Token: 0x04000137 RID: 311
		internal Ellipse circle1;

		// Token: 0x04000138 RID: 312
		internal System.Windows.Shapes.Path path2;

		// Token: 0x04000139 RID: 313
		internal Ellipse circle2;

		// Token: 0x0400013A RID: 314
		internal TextBox EntryEdit;

		// Token: 0x0400013B RID: 315
		private bool _contentLoaded;
	}
}
