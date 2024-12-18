using BepInEx.Configuration;

namespace OrbitRecharge
{
	public class ConfigManager
	{
		public static ConfigFile configFile;
		public static ConfigManager Instance { get; internal set; }

		public static ConfigEntry<bool> TurnOffFlashlights;
		public static ConfigEntry<bool> TurnOffWalkies;
		public static ConfigEntry<bool> TurnOffBoosters;

		public static ConfigEntry<bool> RefillSprayCans;
		public static ConfigEntry<bool> RefillTZPs;

		public static ConfigEntry<bool> Debug;

		public static void Init(ConfigFile config)
		{
			Instance = new ConfigManager(config);
		}

		public ConfigManager(ConfigFile config)
		{
			configFile = config;

			TurnOffFlashlights = configFile.Bind("General", "TurnOffFlashlights", true, "Turn off flashlights at the end of the round");
			TurnOffWalkies = configFile.Bind("General", "TurnOffWalkies", true, "Turn off walkies at the end of the round");
			TurnOffBoosters = configFile.Bind("General", "TurnOffBoosters", true, "Turn off boosters at the end of the round");

			RefillSprayCans = configFile.Bind("General", "RefillSprayCans", true, "Refill spray cans at the end of the round");
			RefillTZPs = configFile.Bind("General", "RefillTZPs", true, "Refill TZPs at the end of the round");

			Debug = configFile.Bind("Debug", "Debug", false, "Enable debug logging");
		}
	}
}
