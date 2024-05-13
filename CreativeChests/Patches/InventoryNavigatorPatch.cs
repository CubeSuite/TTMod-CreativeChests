using EquinoxsModUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CreativeChests.Patches
{
    internal class InventoryNavigatorPatch
    {
        [HarmonyPatch(typeof(InventoryNavigator), "OnOpen")]
        [HarmonyPrefix]
        static void ShowGUI(InventoryNavigator __instance) {
            CreativeChestsPlugin.showGUI = true;
            ChestInstance chest = GetAimedAtChest();
            CreativeChestsPlugin.currentChestId = chest.commonInfo.instanceId;
            CreativeChestsPlugin.currentChestIndex = chest.commonInfo.index;
        }

        [HarmonyPatch(typeof(InventoryNavigator), "OnClose")]
        [HarmonyPrefix]
        static void HideGui() {
            CreativeChestsPlugin.showGUI = false;
        }

        // Private Functions

        private static ChestInstance GetAimedAtChest() {
            GenericMachineInstanceRef machine = (GenericMachineInstanceRef)ModUtils.GetPrivateField("targetMachineRef", Player.instance.interaction);
            return MachineManager.instance.Get<ChestInstance, ChestDefinition>(machine.index, MachineTypeEnum.Chest);
        }
    }
}
