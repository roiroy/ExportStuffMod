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
                    recipes = building.ExportRecipes(),
                }).Where(b => b.recipes.Count > 0).ToList(),
            };
            string json = JsonConvert.SerializeObject(exports, Formatting.Indented);
            Debug.Log(json);
            //File.WriteAllText("D:\\Project\\roi-tools\\calc\\exports.json", json);
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
            RecipeUser[] users = building.GetComponents<RecipeUser>();
            return users.Length > 0 ? Recipes(users[0]) : new List<string>();
        }

        private static List<String> Recipes(RecipeUser user)
        {
            return user.availableRecipes?.Select(recipe => recipe.name)?.ToList();
        }
    }
}
