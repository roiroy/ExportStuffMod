using System.Collections.Generic;
using ProjectAutomata;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

namespace ExportStuffMod
{
    [Serializable]
    public class ExportedStuff
    {
        public DateTime timestamp = DateTime.UtcNow;
        public List<ExportedRecipe> recipes;
        public List<ExportedProducer> producers;
    }

    [Serializable]
    public class ExportedRecipe
    {
        public string name;
        public float days;
        public List<ExportedProduct> inputs;
        public List<ExportedProduct> outputs;
    }

    [Serializable]
    public class ExportedProduct
    {
        public string name;
        public int amount;
        public string priceFormula;
    }

    [Serializable]
    public class ExportedProducer
    {
        public string name;
        public float cost;
        public float? harvesterCost;
        public List<String> recipes;
    }

    [Serializable]
    public class ExportedField
    {
        public string name;
        public string hubName;
    }

    public class ExportStuffMod : Mod
    {
        public override void OnAllModsLoaded()
        {
            ExportedStuff exports = new ExportedStuff
            {
                recipes = GameData.instance.GetAssets<Recipe>().Select(formula => new ExportedRecipe
                {
                    name = formula.name,
                    days = formula.gameDays,
                    inputs = formula.ingredients.Export().ToList(),
                    outputs = formula.result.Export().ToList()
                }).ToList(),

                producers = GameData.instance.GetAssets<Building>().Select(building => new ExportedProducer
                {
                    name = building.name,
                    cost = building.baseCost,
                    harvesterCost = building.ExportHarvesterCost(),
                    recipes = building.ExportRecipes(),
                }).Where(b => b.recipes != null && b.recipes.Count > 0).ToList(),
            };
            string json = JsonConvert.SerializeObject(exports, Formatting.Indented);
            Debug.Log(json);
        }
    }

    static class ExportHelpers
    {
        public static IEnumerable<ExportedProduct> Export(this ProductList products)
        {
            return products.Select(product => new ExportedProduct
            {
                amount = product.amount,
                name = product.definition?.name,
                priceFormula = product.definition?.price?.formula,
            });
        }

        public static List<String> ExportRecipes(this Building building)
        {
            return building.recipeUser?.availableRecipes?.Select(recipe => recipe.name)?.ToList();
        }

        // I couldn't get that information from game data, should certainly be there
        private static readonly Dictionary<string, string> HARVESTERS = new Dictionary<string, string>
        {
            { "CoalGatherer", "CoalMineHarvester" },
            { "CopperGatherer", "CopperMineHarvester" },
            { "CropFarm", "WheatField" },
            { "FishingGatherer", "FishNetHarvester" },
            { "GasGatherer", "GasPumpHarvester" },
            { "IronGatherer", "IronMineHarvester" },
            { "LivestockFarm", "ChickenField" },
            { "LumberyardGatherer", "LumberyardHarvester" },
            { "OilSeaGatherer", "OilSeaHarvester" },
            { "OilGatherer", "OilDrillHarvester" },
            { "OrchardFarm", "ApplesField" },
            { "PlantationFarm", "BerryField" },
            { "SandGatherer", "SandHarvester" },
            { "WaterGatherer", "WaterHarvester" },
            { "WaterWell", "WaterWellHarvester" },
        };

        public static float? ExportHarvesterCost(this Building building)
        {
            var hub = building.GetComponent<GathererHub>();
            if (hub != null)
            {
                var harvesterName = HARVESTERS[hub.name];
                return GameData.instance.GetAssets<Building>()
                    .Where(b => b.harvester != null && b.name == harvesterName)
                    .FirstOrDefault()
                    ?.baseCost;
            }
            return null;
        }
    }
}
