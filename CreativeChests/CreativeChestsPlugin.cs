using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CreativeChests.Patches;
using EquinoxsModUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using UnityEngine;

namespace CreativeChests
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class CreativeChestsPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.equinox.CreativeChests";
        private const string PluginName = "CreativeChests";
        private const string VersionString = "1.0.1";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        // Objects & Variables
        public static bool showGUI;
        public static uint currentChestId;
        public static int currentChestIndex;

        public static List<int> creativeChestIds = new List<int>();
        public static List<int> creativeChestIndexes = new List<int>();

        private static Texture2D border;
        private static Texture2D borderHover;

        // Config Entries

        public static ConfigEntry<float> buttonXOffset;
        public static ConfigEntry<float> buttonYOffset;

        // Unity Functions

        private void Awake() {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            ApplyPatches();
            CreateConfigEntries();

            ModUtils.SaveStateLoaded += OnSaveStateLoaded;

            ModUtils.AddCustomDataForMachine(0, ChestProperties.creativeChestIndexes, "");

            border = ModUtils.LoadTexture2DFromFile("CreativeChests.Assets.Border240x40.png");
            borderHover = ModUtils.LoadTexture2DFromFile("CreativeChests.Assets.BorderHover240x40.png");

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        private void FixedUpdate() {
            if (!ModUtils.hasGameLoaded) return;

            bool refreshIndexes = false;

            MachineInstanceList<ChestInstance, ChestDefinition> chests = MachineManager.instance.GetMachineList<ChestInstance, ChestDefinition>(MachineTypeEnum.Chest);
            foreach(int index in creativeChestIndexes) {
                if (index >= chests.curCount || chests.unusedIndices.Contains(index)) {
                    refreshIndexes = true;
                    break;
                }

                ChestInstance chest = chests.GetIndex(index);
                ref Inventory inventory = ref chest.GetInventory(0);
                if (inventory.myStacks[0].isEmpty) {
                    for (int i = 0; i < inventory.myStacks.Length; i++) {
                        inventory.myStacks[i] = ResourceStack.CreateEmptyStack();
                    }

                    return;
                }

                int resID = inventory.myStacks[0].id;
                if (resID == ModUtils.GetCustomDataForMachine<int>(chest.commonInfo.instanceId, ChestProperties.lastResId)) return;
                int max = SaveState.GetResInfoFromId(resID).maxStackCount;

                for(int i = 0; i < inventory.myStacks.Length; i++) {
                    inventory.myStacks[i].id = resID;
                    inventory.myStacks[i].maxStack = max;
                    inventory.myStacks[i].count = max;
                }
            }

            if (refreshIndexes) {
                creativeChestIndexes.Clear();
                foreach(ChestInstance chest in chests.myArray) {
                    if (chest.commonInfo.instanceId == 0 && chest.commonInfo.index == 0) continue;
                    if (ModUtils.CustomDataExistsForMachine(chest.commonInfo.instanceId)) {
                        if (ModUtils.GetCustomDataForMachine<bool>(chest.commonInfo.instanceId, ChestProperties.isCreative)) {
                            creativeChestIndexes.Add(chest.commonInfo.index);
                        }
                    }
                }

                SetCreativeChestsList();
            }
        }

        private void OnGUI() {
            if (!showGUI) return;
            if(!ModUtils.hasGameLoaded) return;

            float xPos = (Screen.width / 2.0f) + buttonXOffset.Value;
            float yPos = (Screen.height / 2.0f) + buttonYOffset.Value;

            GUIStyle buttonStyle = new GUIStyle() {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.yellow, background = border },
                hover = { textColor = Color.black, background = borderHover },
            };

            if(GUI.Button(new Rect(xPos, yPos, 200, 40), "Set To Creative", buttonStyle)) {
                ModUtils.UpdateCustomDataForMachine(currentChestId, ChestProperties.isCreative, true);
                creativeChestIndexes.Add(currentChestIndex);
                SetCreativeChestsList();
            }
        }

        // Events

        private void OnSaveStateLoaded(object sender, EventArgs e) {
            GetCreativeChestList();
        }

        // Private Functions

        private void ApplyPatches() {
            Harmony.CreateAndPatchAll(typeof(ChestDefinitionPatch));
            Harmony.CreateAndPatchAll(typeof(ChestInstancePatch));
            Harmony.CreateAndPatchAll(typeof(InventoryNavigatorPatch));
        }

        private void CreateConfigEntries() {
            buttonXOffset = Config.Bind("General", "Button Horizontal Offset", 120f, new ConfigDescription("Controls the horizontal position of the 'Set To Creative' button"));
            buttonYOffset = Config.Bind("General", "Button Vertical Offset", 320f, new ConfigDescription("Controls the vertical position of the 'Set To Creative' button"));
        }

        private void GetCreativeChestList() {
            string savedList = ModUtils.GetCustomDataForMachine<string>(0, ChestProperties.creativeChestIndexes);
            Debug.Log($"Creative Chest List: {savedList}");
            if (string.IsNullOrEmpty(savedList)) return;

            List<int> indexes = new List<int>();
            foreach(string index in savedList.Split(',')) {
                indexes.Add(int.Parse(index));
            }

            creativeChestIndexes = indexes;
        }

        private void SetCreativeChestsList() {
            List<string> indexStrings = new List<string>();
            foreach(int index in creativeChestIndexes) {
                indexStrings.Add(index.ToString());
            }

            Debug.Log($"Creative Chest List: {string.Join(",", creativeChestIndexes)}");
            ModUtils.UpdateCustomDataForMachine(0, ChestProperties.creativeChestIndexes, string.Join(",", creativeChestIndexes));
        }
    }
}
