using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CreativeChests.Patches;
using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using UnityEngine;

namespace CreativeChests
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class CreativeChestsPlugin : BaseUnityPlugin
    {
        internal const string MyGUID = "com.equinox.CreativeChests";
        private const string PluginName = "CreativeChests";
        private const string VersionString = "2.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        // Members

        public static List<uint> creativeChestIDs = new List<uint>();

        // Unity Functions

        private void Awake() {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            ApplyPatches();
            CreateConfigEntries();
            ContentAdder.AddContent();

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        private void FixedUpdate() {
            foreach(uint id in creativeChestIDs) {
                if (MachineManager.instance.GetRefFromId(id, out MachineInstanceRef<ChestInstance> chest)) {
                    ref Inventory inventory = ref chest.GetInventory(0);
                    if (inventory.myStacks[0].id == inventory.myStacks[1].id) continue;

                    if (inventory.myStacks[0].isEmpty) {
                        for (int i = 0; i < inventory.myStacks.Length; i++) {
                            inventory.myStacks[i] = ResourceStack.CreateEmptyStack();
                        }

                        continue;
                    }

                    int resID = inventory.myStacks[0].id;
                    int max = SaveState.GetResInfoFromId(resID).maxStackCount;

                    EDT.PacedLog($"General", $"Filling chest #{id} with {max} res#{resID}");
                    for (int i = 0; i < inventory.myStacks.Length; i++) {
                        inventory.myStacks[i].id = resID;
                        inventory.myStacks[i].maxStack = max;
                        inventory.myStacks[i].count = max;
                    }
                }
            }
        }

        // Events

        private void OnSaveStateLoaded(object sender, EventArgs e) {
            
        }

        // Private Functions

        private void ApplyPatches() {
            Harmony.CreateAndPatchAll(typeof(MachineDefinitionPatch));
        }

        private void CreateConfigEntries() {
        
        }

        private void GetCreativeChestList() {
        
        }

        private void SetCreativeChestsList() {
        
        }
    }
}
