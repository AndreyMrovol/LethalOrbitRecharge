using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace OrbitRecharge
{
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	public class Plugin : BaseUnityPlugin
	{
		internal static ManualLogSource logger = new("OrbitRecharge");

		private void Awake()
		{
			// Plugin startup logic
			Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

			ConfigManager.Init(Config);

			var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
			harmony.PatchAll();
		}
	}
}
