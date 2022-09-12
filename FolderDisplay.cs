using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace KrnlUI.Controls
{
	// Token: 0x0200000D RID: 13
	public class FolderDisplay : UserControl, IComponentConnector
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00007E5F File Offset: 0x0000605F
		// (set) Token: 0x060000D8 RID: 216 RVA: 0x00007E67 File Offset: 0x00006067
		public bool isFile
		{
			get
			{
				return this.isFileRaw;
			}
			set
			{
				this.isFileRaw = value;
				if (value)
				{
					this.svg240.Visibility = Visibility.Collapsed;
					return;
				}
				this.svg240.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00007E8C File Offset: 0x0000608C
		public FolderDisplay()
		{
			this.InitializeComponent();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00007EA8 File Offset: 0x000060A8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/KrnlUI;component/controls/folderdisplay.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00007ED8 File Offset: 0x000060D8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.EntryName = (Label)target;
				return;
			case 2:
				this.svg240 = (Canvas)target;
				return;
			case 3:
				this.path209 = (Path)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400013C RID: 316
		public string Path = "";

		// Token: 0x0400013D RID: 317
		private bool isFileRaw;

		// Token: 0x0400013E RID: 318
		internal Label EntryName;

		// Token: 0x0400013F RID: 319
		internal Canvas svg240;

		// Token: 0x04000140 RID: 320
		internal Path path209;

		// Token: 0x04000141 RID: 321
		private bool _contentLoaded;
	}
}
