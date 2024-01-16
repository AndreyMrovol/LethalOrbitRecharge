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

        internal static ManualLogSource logger;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            logger = Logger;

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(StartOfRound))]
    public static class StartOfRound_SetShipReadyToLand_Patch
    {

        private static void RechargeItem(GrabbableObject itemToCharge, string playerHeldBy = "on ship")
        {
            Plugin.logger.LogDebug($"Recharging {itemToCharge.itemProperties.itemName} ({playerHeldBy}) - {Math.Ceiling(itemToCharge.insertedBattery.charge) * 100}%");

            itemToCharge.insertedBattery = new Battery(false, 1f);
            itemToCharge.SyncBatteryServerRpc(100);
        }

        [HarmonyPatch("SetShipReadyToLand")]
        [HarmonyPrefix]
        public static void Prefix(StartOfRound __instance)
        {
            GameObject ship = GameObject.Find("/Environment/HangarShip");
            var ItemsOnShip = ship.GetComponentsInChildren<GrabbableObject>();

            foreach (var item in ItemsOnShip)
            {
                if (item.insertedBattery != null)
                {
                    RechargeItem(item);
                }
            }

            var Players = __instance.allPlayerScripts;
            foreach (var player in Players)
            {
                GrabbableObject[] ItemsOnPlayer = player.ItemSlots;

                foreach (var item in ItemsOnPlayer)
                {
                    if (item == null) continue;

                    if (item.insertedBattery != null)
                    {
                        RechargeItem(item, player.name);
                    }
                }
            }


            FlashlightItem[] flashlights = GameObject.FindObjectsOfType<FlashlightItem>();
            foreach (var flashlight in flashlights)
            {
                try
                {
                    flashlight.SwitchFlashlight(false);
                    flashlight.flashlightBulb.enabled = false;
                    flashlight.flashlightBulbGlow.enabled = false;

                    flashlight.isBeingUsed = false;
                }
                catch (Exception e)
                {
                    Plugin.logger.LogDebug("Failed to turn off flashlight");
                    Plugin.logger.LogDebug(e);
                    continue;
                }

            }

            WalkieTalkie[] walkies = GameObject.FindObjectsOfType<WalkieTalkie>();
            foreach (var walkie in walkies)
            {
                try
                {
                    walkie.SwitchWalkieTalkieOn(false);
                }
                catch
                {
                    Plugin.logger.LogDebug("Failed to turn off walkie");
                    continue;
                }
            }
        }
    }
}