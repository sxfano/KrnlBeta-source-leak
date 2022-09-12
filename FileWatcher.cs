using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using KrnlUI.Controls;

namespace KrnlUI
{
	// Token: 0x02000007 RID: 7
	public class FileWatcher
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060000AB RID: 171 RVA: 0x0000767F File Offset: 0x0000587F
		// (set) Token: 0x060000AC RID: 172 RVA: 0x00007687 File Offset: 0x00005887
		private Tab Tab { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00007690 File Offset: 0x00005890
		// (set) Token: 0x060000AE RID: 174 RVA: 0x00007698 File Offset: 0x00005898
		public string filePath { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000AF RID: 175 RVA: 0x000076A1 File Offset: 0x000058A1
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x000076A9 File Offset: 0x000058A9
		private bool isWatching { get; set; }

		// Token: 0x060000B1 RID: 177 RVA: 0x000076B2 File Offset: 0x000058B2
		public FileWatcher(Tab tab)
		{
			this.Tab = tab;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000076C1 File Offset: 0x000058C1
		public void Start()
		{
			if (!this.isWatching)
			{
				this.isWatching = true;
				new Task(new Action(this.Initialize)).Start();
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000076E8 File Offset: 0x000058E8
		private void Initialize()
		{
			Application.Current.Dispatcher.Invoke<Task>(delegate()
			{
				FileWatcher.<<Initialize>b__14_0>d <<Initialize>b__14_0>d;
				<<Initialize>b__14_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Initialize>b__14_0>d.<>4__this = this;
				<<Initialize>b__14_0>d.<>1__state = -1;
				<<Initialize>b__14_0>d.<>t__builder.Start<FileWatcher.<<Initialize>b__14_0>d>(ref <<Initialize>b__14_0>d);
				return <<Initialize>b__14_0>d.<>t__builder.Task;
			});
		}
	}
}
