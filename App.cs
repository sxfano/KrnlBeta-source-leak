using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

namespace KrnlUI
{
	// Token: 0x02000003 RID: 3
	public class App : Application
	{
		// Token: 0x06000007 RID: 7 RVA: 0x000020C8 File Offset: 0x000002C8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
			Uri resourceLocator = new Uri("/KrnlUI;component/app.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002109 File Offset: 0x00000309
		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}

		// Token: 0x04000001 RID: 1
		private bool _contentLoaded;
	}
}
