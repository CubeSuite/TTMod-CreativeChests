using EquinoxsModUtils;
using EquinoxsModUtils.Additions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CreativeChests
{
    public static class ContentAdder
    {
        public static void AddContent() {
            NewResourceDetails details = new NewResourceDetails() {
                name = "Creative Chest",
                description = "Fills itself with the item in the first slot.",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Logistics",
                subHeaderTitle = "Utility",
                maxStackCount = 50,
                sortPriority = 999,
                unlockName = EMU.Names.Unlocks.BasicLogistics,
                parentName = EMU.Names.Resources.Container
            };

            ChestDefinition definition = ScriptableObject.CreateInstance<ChestDefinition>();
            EMUAdditions.AddNewMachine(definition, details, true);

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = CreativeChestsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.1f,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.IronIngot,
                        quantity = 1
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = "Creative Chest",
                        quantity = 1
                    }
                },
                sortPriority = 10,
                unlockName = EMU.Names.Unlocks.BasicLogistics
            });
        }
    }
}
