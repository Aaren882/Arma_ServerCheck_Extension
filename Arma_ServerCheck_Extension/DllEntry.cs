using RGiesecke.DllExport;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Arma_ServerCheck_Extension
{
	public class DllEntry
	{

		#region Misc RVExtension Requirements
#if IS_x64
		[DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]
#else
		[DllExport("_RVExtensionVersion@8", CallingConvention = CallingConvention.Winapi)]
#endif
		public static void RvExtensionVersion(StringBuilder output, int outputSize)
		{
				outputSize--;
				output.Append("1.0.0");
		}

#if IS_x64
		[DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
#else
		[DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
#endif
		public static void RvExtension(StringBuilder output, int outputSize,
			[MarshalAs(UnmanagedType.LPStr)] string path)
		{
				outputSize--;
				if (path == "")
						path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + DateTime.Now.ToString("h:mm:ss tt");
				output.Append(new ScreenCapture().TakeScreenshot(path));
		}

#if IS_x64
		[DllExport("RVExtensionArgs", CallingConvention = CallingConvention.Winapi)]
#else
		[DllExport("_RVExtensionArgs@20", CallingConvention = CallingConvention.Winapi)]
#endif
		#endregion
		public static int RvExtensionArgs(StringBuilder output, int outputSize,
			[MarshalAs(UnmanagedType.LPStr)] string function,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 4)] string[] args, int argCount)
		{
			//- Check Server Status
			try
			{
				var server = GetSteamServer(function, args, output);

				//- Setup Serialization
				string jsonString = JsonSerializer.Serialize(server[0],
					new JsonSerializerOptions
					{
						Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, //- Output all Text
						WriteIndented = true //- Better Formatting
					}
				);
				byte[] UTF = Encoding.UTF32.GetBytes(jsonString);

				//- Return Values
				output.Append($"[{string.Join(",", UTF)}]");
				return 0;
			}
			catch (Exception _)
			{
				output.Append(_);
				return -1;
			}
		}

		private static dynamic[] GetSteamServer(string function, string[] input, StringBuilder output)
		{
			string[] info = input[0].Split(':');

			if (function == "PlayersAsync") //- [IP, Steam Port, Timeout(ms)];
			{
				return [SteamServer.QueryPlayersAsync(info[0], int.Parse(info[1]) + 1, 3000).Result, 1];
			}
			else
			{
				return [SteamServer.QueryServerAsync(info[0], int.Parse(info[1]) + 1, 3000).Result, 0];
			}
		}
	}
}
