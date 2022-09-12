using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KrnlUI.Controls
{
	// Token: 0x0200000B RID: 11
	public class CommunityEntry : UserControl, IComponentConnector
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x000077EF File Offset: 0x000059EF
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x000077F7 File Offset: 0x000059F7
		public string Script { get; set; }

		// Token: 0x060000C4 RID: 196 RVA: 0x00007800 File Offset: 0x00005A00
		public CommunityEntry()
		{
			this.InitializeComponent();
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000781C File Offset: 0x00005A1C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/KrnlUI;component/controls/communityentry.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000784C File Offset: 0x00005A4C
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
				this.EntryCreatorIcon = (ImageBrush)target;
				return;
			case 3:
				this.EntryCreator = (Label)target;
				return;
			case 4:
				this.RunBorder = (Border)target;
				return;
			case 5:
				this.svg22 = (Canvas)target;
				return;
			case 6:
				this.RunIcon = (Path)target;
				return;
			case 7:
				this.RunLabel = (Label)target;
				return;
			case 8:
				this.Preview = (Border)target;
				return;
			case 9:
				this.EntryPreview = (Image)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000111 RID: 273
		public List<string> Tags = new List<string>();

		// Token: 0x04000112 RID: 274
		internal Label EntryName;

		// Token: 0x04000113 RID: 275
		internal ImageBrush EntryCreatorIcon;

		// Token: 0x04000114 RID: 276
		internal Label EntryCreator;

		// Token: 0x04000115 RID: 277
		internal Border RunBorder;

		// Token: 0x04000116 RID: 278
		internal Canvas svg22;

		// Token: 0x04000117 RID: 279
		internal Path RunIcon;

		// Token: 0x04000118 RID: 280
		internal Label RunLabel;

		// Token: 0x04000119 RID: 281
		internal Border Preview;

		// Token: 0x0400011A RID: 282
		internal Image EntryPreview;

		// Token: 0x0400011B RID: 283
		private bool _contentLoaded;
	}
}
