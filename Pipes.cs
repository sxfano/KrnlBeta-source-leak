using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KrnlUI
{
	// Token: 0x02000006 RID: 6
	internal class Pipes
	{
		// Token: 0x060000A6 RID: 166
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool WaitNamedPipe(string name, int timeout);

		// Token: 0x060000A7 RID: 167 RVA: 0x000075E0 File Offset: 0x000057E0
		public static bool PipeActive()
		{
			bool result;
			try
			{
				if (!Pipes.WaitNamedPipe(Path.GetFullPath("\\\\.\\pipe\\" + Pipes.PipeName), 0))
				{
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00007628 File Offset: 0x00005828
		public static bool PassString(string input)
		{
			bool result = false;
			Task.Run(delegate()
			{
				if (Pipes.PipeActive())
				{
					try
					{
						using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", "krnlpipe", PipeDirection.Out))
						{
							namedPipeClientStream.Connect();
							byte[] bytes = Encoding.UTF8.GetBytes(input);
							namedPipeClientStream.Write(bytes, 0, bytes.Length);
							namedPipeClientStream.Dispose();
						}
						return;
					}
					catch (Exception)
					{
						result = false;
						return;
					}
				}
				result = false;
			}).GetAwaiter().GetResult();
			return result;
		}

		// Token: 0x04000107 RID: 263
		private static string PipeName = "krnlpipe";
	}
}
