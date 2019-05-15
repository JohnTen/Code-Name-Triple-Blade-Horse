using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JTUtility
{
	public enum DebugLog
	{
		FileOnly,
		FileAndUnityConsole,
		FileAndWarningInUnity,
		FileAndErrorInUnity
	}

	public class FileDebug
	{
		static FileStream _outputFile;
		static StreamWriter _writer;
		static StreamWriter Writer
		{
			get
			{
				if (_outputFile == null)
				{
					_outputFile = new FileStream(Application.dataPath + "/debug.log", FileMode.Create);
				}

				if (_writer == null)
				{
					_writer = new StreamWriter(_outputFile);
				}

				return _writer;
			}
		}
		public static void Write(object message, DebugLog log = DebugLog.FileOnly)
		{
			Writer.Write(message);
			Writer.Flush();

			switch (log)
			{
				case DebugLog.FileOnly: break;
				case DebugLog.FileAndUnityConsole:
					Debug.Log(message);
					break;
				case DebugLog.FileAndWarningInUnity:
					Debug.LogWarning(message);
					break;
				case DebugLog.FileAndErrorInUnity:
					Debug.LogError(message);
					break;
			}
		}

		public static void WriteLine(object message, DebugLog log = DebugLog.FileOnly)
		{
			Writer.WriteLine(message);
			Writer.Flush();

			switch (log)
			{
				case DebugLog.FileOnly: break;
				case DebugLog.FileAndUnityConsole:
					Debug.Log(message);
					break;
				case DebugLog.FileAndWarningInUnity:
					Debug.LogWarning(message);
					break;
				case DebugLog.FileAndErrorInUnity:
					Debug.LogError(message);
					break;
			}
		}
	}
}
