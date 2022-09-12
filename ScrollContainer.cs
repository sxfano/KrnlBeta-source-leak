using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace KrnlUI.Controls
{
	// Token: 0x0200000E RID: 14
	public class ScrollContainer : UserControl, IComponentConnector
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00007F2C File Offset: 0x0000612C
		// (set) Token: 0x060000DD RID: 221 RVA: 0x00007F98 File Offset: 0x00006198
		public List<UIElement> Children
		{
			get
			{
				List<UIElement> list = new List<UIElement>();
				foreach (object obj in ((Grid)base.Content).Children)
				{
					UIElement item = (UIElement)obj;
					list.Add(item);
				}
				return list;
			}
			set
			{
				foreach (UIElement element in value)
				{
					((Grid)base.Content).Children.Add(element);
				}
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00007FF8 File Offset: 0x000061F8
		public ScrollContainer()
		{
			this.InitializeComponent();
			Storyboard.SetTargetProperty(this.ScrollPosAnimator, new PropertyPath("Margin", Array.Empty<object>()));
			this.ScrollStoryboard.Children.Add(this.ScrollPosAnimator);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00008088 File Offset: 0x00006288
		private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (this.VisualChildrenCount == 1 && base.IsLoaded)
			{
				ScrollViewer scrollViewer = (ScrollViewer)base.Content;
				Storyboard.SetTarget(this.ScrollPosAnimator, scrollViewer);
				this.ScrollStoryboard.Stop();
				MessageBox.Show(scrollViewer.ViewportHeight.ToString());
				int num = this.ScrollLength * (e.Delta / 120);
				this.AccPad += num;
				if (this.AccPad > 0)
				{
					this.ScrollPosAnimator.To = new Thickness?(new Thickness(0.0, 0.0, 0.0, 0.0));
					this.AccPad = 0;
				}
				else
				{
					this.ScrollPosAnimator.To = new Thickness?(new Thickness(0.0, (double)this.AccPad, 0.0, 0.0));
				}
				this.ScrollStoryboard.Begin();
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00008190 File Offset: 0x00006390
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/KrnlUI;component/controls/scrollcontainer.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000081C0 File Offset: 0x000063C0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((ScrollContainer)target).PreviewMouseWheel += this.UserControl_PreviewMouseWheel;
				return;
			}
			this._contentLoaded = true;
		}

		// Token: 0x04000142 RID: 322
		private int AccPad;

		// Token: 0x04000143 RID: 323
		public int ScrollLength = 40;

		// Token: 0x04000144 RID: 324
		private Storyboard ScrollStoryboard = new Storyboard();

		// Token: 0x04000145 RID: 325
		private ThicknessAnimation ScrollPosAnimator = new ThicknessAnimation
		{
			Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)),
			DecelerationRatio = 1.0
		};

		// Token: 0x04000146 RID: 326
		private bool _contentLoaded;
	}
}
