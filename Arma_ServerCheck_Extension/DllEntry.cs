using RGiesecke.DllExport;
using SteamServerQuery;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

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
			[MarshalAs(UnmanagedType.LPStr)] string function)
		{
				outputSize--;
				output.Append(function);
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
				var server = GetSteamServer(function, args[0]);
				if (server == "")
				{
					output.Append("No function be found");
					return -1;
				}

				//- Setup Serialization
				string jsonString = JsonSerializer.Serialize(server[0],
					new JsonSerializerOptions
					{
						Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, //- Output all Text
						WriteIndented = true //- Better Formatting
					}
				);
				int[] UTF = Tools.StringToCode32(jsonString);

				//- Return Values (in 32Bit Unicode)
				output.Append($"[{string.Join(",", UTF)}]");
				return 0;
			}
			catch (Exception e)
			{
				Tools.Logger(e);
				return -1;
			}
		}

		private static dynamic GetSteamServer(string function, string input)
		{
			string[] info = input.Trim('\"').Split(':');
			Tools.Logger(null, info[1]);
			Tools.Logger(null, $"{Int16.Parse(info[1])}");

            if (function == "ServerAsync") //- [IP, Steam Port, Timeout(ms)];
				return SteamServer.QueryServerAsync(info[0], Int16.Parse(info[1]) + 1, 3000).Result;
			if (function == "PlayersAsync") //- [IP, Steam Port, Timeout(ms)];
				return SteamServer.QueryPlayersAsync(info[0], Int16.Parse(info[1]) + 1, 3000).Result;
			
			return "";
		}
	}
}
