using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace KrnlUI.Properties
{
	// Token: 0x02000009 RID: 9
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000BA RID: 186 RVA: 0x000077A4 File Offset: 0x000059A4
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x0400010D RID: 269
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
