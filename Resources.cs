using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace KrnlUI.Properties
{
	// Token: 0x02000008 RID: 8
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	public class Resources
	{
		// Token: 0x060000B5 RID: 181 RVA: 0x0000774B File Offset: 0x0000594B
		internal Resources()
		{
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00007753 File Offset: 0x00005953
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("KrnlUI.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x0000777F File Offset: 0x0000597F
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x00007786 File Offset: 0x00005986
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x0000778E File Offset: 0x0000598E
		public static string _default
		{
			get
			{
				return Resources.ResourceManager.GetString("_default", Resources.resourceCulture);
			}
		}

		// Token: 0x0400010B RID: 267
		private static ResourceManager resourceMan;

		// Token: 0x0400010C RID: 268
		private static CultureInfo resourceCulture;
	}
}
