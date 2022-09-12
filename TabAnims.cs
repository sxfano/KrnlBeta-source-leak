using System;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KrnlUI.Animations
{
	// Token: 0x02000011 RID: 17
	internal class TabAnims
	{
		// Token: 0x06000106 RID: 262 RVA: 0x00008EE6 File Offset: 0x000070E6
		public static void NewTabEnter(object sender, MouseEventArgs e)
		{
			((Label)sender).Foreground = new SolidColorBrush(Color.FromRgb(175, 175, 175));
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00008F0C File Offset: 0x0000710C
		public static void NewTabLeave(object sender, MouseEventArgs e)
		{
			((Label)sender).Foreground = new SolidColorBrush(Color.FromRgb(122, 122, 122));
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00008F29 File Offset: 0x00007129
		public static void AnimateNewTab(object sender)
		{
			Grid grid = (Grid)sender;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00008F34 File Offset: 0x00007134
		private static void AnimEnter(object sender, Color color, Color secondary)
		{
			TabAnims.<AnimEnter>d__5 <AnimEnter>d__;
			<AnimEnter>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimEnter>d__.sender = sender;
			<AnimEnter>d__.color = color;
			<AnimEnter>d__.secondary = secondary;
			<AnimEnter>d__.<>1__state = -1;
			<AnimEnter>d__.<>t__builder.Start<TabAnims.<AnimEnter>d__5>(ref <AnimEnter>d__);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00008F7C File Offset: 0x0000717C
		private static void AnimLeave(object sender, Color color, Color secondary)
		{
			TabAnims.<AnimLeave>d__6 <AnimLeave>d__;
			<AnimLeave>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimLeave>d__.sender = sender;
			<AnimLeave>d__.color = color;
			<AnimLeave>d__.secondary = secondary;
			<AnimLeave>d__.<>1__state = -1;
			<AnimLeave>d__.<>t__builder.Start<TabAnims.<AnimLeave>d__6>(ref <AnimLeave>d__);
		}

		// Token: 0x04000163 RID: 355
		private static int animTime = 20;

		// Token: 0x04000164 RID: 356
		private static int smooth = 5;
	}
}
