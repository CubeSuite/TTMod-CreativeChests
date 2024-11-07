using EquinoxsDebuggingTools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CreativeChests.Patches
{
    internal class MachineDefinitionPatch
    {
        [HarmonyPatch(typeof(MachineDefinition<ChestInstance, ChestDefinition>), nameof(MachineDefinition<ChestInstance, ChestDefinition>.OnBuild))]
        [HarmonyPostfix]
        static void AddCreativeChestToList(MachineInstanceRef<ChestInstance> instRef) {
            if (instRef.Get().myDef.displayName != "Creative Chest") return;

            CreativeChestsPlugin.creativeChestIDs.Add(instRef.GetCommonInfo().instanceId);
            EDT.Log("General", $"Added chest #{instRef.GetCommonInfo().instanceId} to creativeChestIDs");
        }
    }
}
