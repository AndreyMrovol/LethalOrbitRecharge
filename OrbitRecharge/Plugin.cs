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
		internal static MrovLib.Logger logger;

		private void Awake()
		{
			// Plugin startup logic
			Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

			logger = new("OrbitRecharge", ConfigManager.Debug);

			ConfigManager.Init(Config);

			var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
			harmony.PatchAll();
		}
	}

	[HarmonyPatch(typeof(StartOfRound))]
	public static class StartOfRound_SetShipReadyToLand_Patch
	{
		private static void RechargeItem(GrabbableObject itemToCharge, string playerHeldBy = "on ship")
		{
			if (!StartOfRound.Instance.IsHost)
			{
				Plugin.logger.LogDebug("Not a host!!");
				return;
			}

			Plugin.logger.LogDebug(
				$"Recharging {itemToCharge.itemProperties.itemName} ({playerHeldBy}) - {Math.Ceiling(itemToCharge.insertedBattery.charge) * 100}%"
			);

			itemToCharge.SyncBatteryServerRpc(100);
		}

		[HarmonyPatch("SetShipReadyToLand")]
		[HarmonyPrefix]
		public static void Prefix(StartOfRound __instance)
		{
			GrabbableObject[] Items = GameObject.FindObjectsOfType<GrabbableObject>();

			foreach (var item in Items)
			{
				if (item == null)
				{
					continue;
				}

				if (item.insertedBattery != null)
				{
					RechargeItem(item);
				}
			}

			if (ConfigManager.TurnOffFlashlights.Value)
			{
				FlashlightItem[] flashlights = GameObject.FindObjectsOfType<FlashlightItem>();
				foreach (var flashlight in flashlights)
				{
					try
					{
						if (!flashlight.heldByPlayerOnServer)
						{
							flashlight.SwitchFlashlight(false);
							flashlight.flashlightBulb.enabled = false;
							flashlight.flashlightBulbGlow.enabled = false;

							flashlight.isBeingUsed = false;
						}
					}
					catch (Exception e)
					{
						Plugin.logger.LogDebug("Failed to turn off flashlight");
						Plugin.logger.LogDebug(e);
						continue;
					}
				}
			}

			if (ConfigManager.TurnOffWalkies.Value)
			{
				WalkieTalkie[] walkies = GameObject.FindObjectsOfType<WalkieTalkie>();
				foreach (var walkie in walkies)
				{
					try
					{
						if (!walkie.heldByPlayerOnServer)
						{
							walkie.SwitchWalkieTalkieOn(false);
						}
					}
					catch
					{
						Plugin.logger.LogDebug("Failed to turn off walkie");
						continue;
					}
				}
			}

			if (ConfigManager.TurnOffBoosters.Value)
			{
				RadarBoosterItem[] radarBoosters = GameObject.FindObjectsOfType<RadarBoosterItem>();
				foreach (var radarBooster in radarBoosters)
				{
					try
					{
						radarBooster.EnableRadarBooster(false);
					}
					catch
					{
						Plugin.logger.LogDebug("Failed to turn off radar booster");
						continue;
					}
				}
			}

			if (ConfigManager.RefillSprayCans.Value)
			{
				SprayPaintItem[] sprayPaints = GameObject.FindObjectsOfType<SprayPaintItem>();
				foreach (var sprayPaint in sprayPaints)
				{
					try
					{
						sprayPaint.sprayCanTank = 100f;
					}
					catch
					{
						Plugin.logger.LogDebug("Failed to turn off spray paint");
						continue;
					}
				}
			}

			if (ConfigManager.RefillTZPs.Value)
			{
				TetraChemicalItem[] tetraChemicals = GameObject.FindObjectsOfType<TetraChemicalItem>();
				foreach (var tetraChemical in tetraChemicals)
				{
					try
					{
						tetraChemical.fuel = 100f;
					}
					catch
					{
						Plugin.logger.LogDebug("Failed to turn off spray paint");
						continue;
					}
				}
			}

			// sometimes (somehow) apparatus turn itself on after being disconnected - this is a workaround
			LungProp[] apparatuses = GameObject.FindObjectsOfType<LungProp>();
			foreach (var apparatus in apparatuses)
			{
				try
				{
					apparatus.gameObject.GetComponent<AudioSource>().Stop();
				}
				catch
				{
					Plugin.logger.LogDebug("Failed to turn off lung");
					continue;
				}
			}
		}
	}
}
