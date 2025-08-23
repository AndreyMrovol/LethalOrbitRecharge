using BepInEx.Logging;
using MrovLib;

namespace OrbitRecharge
{
	public class Logger : MrovLib.Logger
	{
		public Logger(string SourceName, LoggingType defaultLoggingType = LoggingType.Debug)
			: base(SourceName, defaultLoggingType)
		{
			ModName = SourceName;
			LogSource = BepInEx.Logging.Logger.CreateLogSource("TestingTools");
			_name = SourceName;
		}
	}
}
