using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace OrbitRecharge
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(StartOfRound))]
    public static class StartOfRound_SetShipReadyToLand_Patch
    {
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
                    item.insertedBattery = new Battery(false, 1f);
                    item.SyncBatteryServerRpc(100);
                }
            }
        }
    }
}