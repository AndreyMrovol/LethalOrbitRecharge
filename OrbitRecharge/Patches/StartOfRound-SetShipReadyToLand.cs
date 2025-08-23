using System;
using System.Diagnostics;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace OrbitRecharge
{
	[HarmonyPatch(typeof(StartOfRound))]
	public static class StartOfRoundPatch
	{
		private static Logger Logger = new("Debug", MrovLib.LoggingType.Debug);

		private static void RechargeItem(GrabbableObject itemToCharge, string playerHeldBy = "on ship")
		{
			if (!StartOfRound.Instance.IsHost)
			{
				Plugin.logger.LogDebug("Not a host!!");
				return;
			}

			Logger.LogDebug(
				$"Recharging {itemToCharge.itemProperties.itemName} ({playerHeldBy}) - {Math.Ceiling(itemToCharge.insertedBattery.charge) * 100}%"
			);

			itemToCharge.SyncBatteryServerRpc(100);
		}

		[HarmonyPatch("SetShipReadyToLand")]
		[HarmonyPrefix]
		public static void Prefix(StartOfRound __instance)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			GrabbableObject[] Items = GameObject.FindObjectsOfType<GrabbableObject>();

			Logger.LogCustom(
				$"Time to find all Grabbables: {stopwatch.ElapsedMilliseconds}ms",
				BepInEx.Logging.LogLevel.Debug,
				MrovLib.LoggingType.Developer
			);

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
					catch
					{
						Logger.LogWarning($"Failed to turn off flashlight");
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
						Logger.LogWarning($"Failed to turn off walkie");
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
						Logger.LogWarning("Failed to turn off radar booster");
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
						Logger.LogWarning("Failed to turn off spray paint");
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
						Logger.LogWarning("Failed to turn off spray paint");
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
					apparatus.gameObject.GetComponentInChildren<Light>().intensity = 5;
				}
				catch
				{
					Logger.LogWarning("Failed to turn off lung");
					continue;
				}
			}

			Logger.LogCustom(
				$"Time to finish the function: {stopwatch.ElapsedMilliseconds}ms",
				BepInEx.Logging.LogLevel.Debug,
				MrovLib.LoggingType.Developer
			);

			stopwatch.Stop();
		}
	}
}
