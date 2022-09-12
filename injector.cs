using System;
using System.Runtime.InteropServices;

namespace KrnlUI
{
	// Token: 0x02000004 RID: 4
	public class Injector
	{
		// Token: 0x0600000A RID: 10
		[DllImport("injector.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern Injector.inject_status inject(string dll_path);

		// Token: 0x02000013 RID: 19
		public enum inject_status
		{
			// Token: 0x04000166 RID: 358
			failure = -1,
			// Token: 0x04000167 RID: 359
			success,
			// Token: 0x04000168 RID: 360
			loadimage_fail,
			// Token: 0x04000169 RID: 361
			no_rbx_proc
		}
	}
}
