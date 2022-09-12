using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Markup;

namespace XamlGeneratedNamespace
{
	// Token: 0x02000002 RID: 2
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class GeneratedInternalTypeHelper : InternalTypeHelper
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		protected override object CreateInstance(Type type, CultureInfo culture)
		{
			return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, culture);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002060 File Offset: 0x00000260
		protected override object GetPropertyValue(PropertyInfo propertyInfo, object target, CultureInfo culture)
		{
			return propertyInfo.GetValue(target, BindingFlags.Default, null, null, culture);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000206D File Offset: 0x0000026D
		protected override void SetPropertyValue(PropertyInfo propertyInfo, object target, object value, CultureInfo culture)
		{
			propertyInfo.SetValue(target, value, BindingFlags.Default, null, null, culture);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000207C File Offset: 0x0000027C
		protected override Delegate CreateDelegate(Type delegateType, object target, string handler)
		{
			return (Delegate)target.GetType().InvokeMember("_CreateDelegate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, target, new object[]
			{
				delegateType,
				handler
			}, null);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020B4 File Offset: 0x000002B4
		protected override void AddEventHandler(EventInfo eventInfo, object target, Delegate handler)
		{
			eventInfo.AddEventHandler(target, handler);
		}
	}
}
