using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace KrnlUI.Animations
{
	// Token: 0x02000010 RID: 16
	internal class ButtonAnims
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x00008CDF File Offset: 0x00006EDF
		public static void ButtonExitEnter(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimEnter(sender, Color.FromRgb(231, 16, 34), Color.FromRgb(200, 200, 200));
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00008D09 File Offset: 0x00006F09
		public static void ButtonExitLeave(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimLeave(sender, Color.FromRgb(34, 34, 34), Color.FromRgb(122, 122, 122));
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00008D27 File Offset: 0x00006F27
		public static void ButtonExitDown(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimDown(sender, Color.FromRgb(194, 16, 29));
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00008D3D File Offset: 0x00006F3D
		public static void ButtonExitUp(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimUp(sender, Color.FromRgb(231, 16, 34));
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00008D53 File Offset: 0x00006F53
		public static void ButtonTopEnter(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimEnter(sender, Color.FromRgb(57, 57, 57), Color.FromRgb(200, 200, 200));
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00008D7A File Offset: 0x00006F7A
		public static void ButtonTopLeave(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimLeave(sender, Color.FromRgb(34, 34, 34), Color.FromRgb(122, 122, 122));
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00008D98 File Offset: 0x00006F98
		public static void ButtonTopDown(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimDown(sender, Color.FromRgb(74, 74, 74));
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00008DAB File Offset: 0x00006FAB
		public static void ButtonTopUp(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimUp(sender, Color.FromRgb(57, 57, 57));
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00008DC0 File Offset: 0x00006FC0
		private static void AnimEnter(object sender, Color color, Color secondary)
		{
			ButtonAnims.<AnimEnter>d__10 <AnimEnter>d__;
			<AnimEnter>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimEnter>d__.sender = sender;
			<AnimEnter>d__.color = color;
			<AnimEnter>d__.secondary = secondary;
			<AnimEnter>d__.<>1__state = -1;
			<AnimEnter>d__.<>t__builder.Start<ButtonAnims.<AnimEnter>d__10>(ref <AnimEnter>d__);
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00008E08 File Offset: 0x00007008
		private static void AnimLeave(object sender, Color color, Color secondary)
		{
			ButtonAnims.<AnimLeave>d__11 <AnimLeave>d__;
			<AnimLeave>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimLeave>d__.sender = sender;
			<AnimLeave>d__.color = color;
			<AnimLeave>d__.secondary = secondary;
			<AnimLeave>d__.<>1__state = -1;
			<AnimLeave>d__.<>t__builder.Start<ButtonAnims.<AnimLeave>d__11>(ref <AnimLeave>d__);
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00008E50 File Offset: 0x00007050
		private static void AnimDown(object sender, Color color)
		{
			ButtonAnims.<AnimDown>d__12 <AnimDown>d__;
			<AnimDown>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimDown>d__.sender = sender;
			<AnimDown>d__.color = color;
			<AnimDown>d__.<>1__state = -1;
			<AnimDown>d__.<>t__builder.Start<ButtonAnims.<AnimDown>d__12>(ref <AnimDown>d__);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00008E90 File Offset: 0x00007090
		private static void AnimUp(object sender, Color color)
		{
			ButtonAnims.<AnimUp>d__13 <AnimUp>d__;
			<AnimUp>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimUp>d__.sender = sender;
			<AnimUp>d__.color = color;
			<AnimUp>d__.<>1__state = -1;
			<AnimUp>d__.<>t__builder.Start<ButtonAnims.<AnimUp>d__13>(ref <AnimUp>d__);
		}

		// Token: 0x04000161 RID: 353
		private static int animTime = 20;

		// Token: 0x04000162 RID: 354
		private static int smooth = 5;
	}
}
