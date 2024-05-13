using EquinoxsModUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreativeChests.Patches
{
    internal class ChestDefinitionPatch
    {
        [HarmonyPatch(typeof(MachineDefinition<ChestInstance, ChestDefinition>), "OnBuild")]
        [HarmonyPostfix]
        static void AddParameters(MachineInstanceRef<ChestInstance> instRef) {
            ModUtils.AddCustomDataForMachine(instRef.instanceId, ChestProperties.isCreative, false);
            ModUtils.AddCustomDataForMachine(instRef.instanceId, ChestProperties.lastResId, -1);
        }

        [HarmonyPatch(typeof(MachineDefinition<ChestInstance, ChestDefinition>), "OnDeconstruct")]
        [HarmonyPrefix]
        static void RemoveFromCreativeList(ref ChestInstance erasedInstance) {
            if (CreativeChestsPlugin.creativeChestIndexes.Contains(erasedInstance.commonInfo.index)) {
                ref Inventory inventory = ref erasedInstance.GetInventory(0);
                for(int i = 0; i < inventory.myStacks.Length; i++) {
                    inventory.myStacks[i] = ResourceStack.CreateEmptyStack();
                }

                CreativeChestsPlugin.creativeChestIndexes.Remove(erasedInstance.commonInfo.index);
                ModUtils.UpdateCustomDataForMachine(erasedInstance.commonInfo.instanceId, ChestProperties.isCreative, false);
            }
        }
    }
}
