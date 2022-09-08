using Arcemi.Pathfinder.Kingmaker.GameData;
using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Arcemi.Pathfinder.Kingmaker.Models;
using Arcemi.Pathfinder.SaveGameEditor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var extractedBlueprintsDirectory = args[0];
var gameDirectory = args[1];

var viewModel = new MainViewModel();
await viewModel.InitializeAsync();

var blueprintData = BlueprintMetadata.Load(gameDirectory);
var references = new References(viewModel.Resources);

var progressionTemplates = new List<ProgressionBlueprintModel>();
foreach (var progressionBlueprintReference in blueprintData.GetEntries(BlueprintTypes.Progression))
{
    Console.WriteLine(progressionBlueprintReference.DisplayName);
    var blueprintPath = Path.Combine(extractedBlueprintsDirectory, progressionBlueprintReference.Path);
    if (!File.Exists(blueprintPath))
    {
        Console.WriteLine("Doesn't exist");
        continue;
    }

    var blueprintJson = JObject.Parse(File.ReadAllText(blueprintPath));
    var blueprint = new Blueprint(new ModelDataAccessor(blueprintJson, references, viewModel.Resources));

    if (blueprint.Data is not BlueprintProgression blueprintProgression)
    {
        continue;
    }

    var progressionTemplate = new ProgressionBlueprintModel
    {
        BlueprintId = blueprint.AssetId
    };
    foreach (var progressionLevel in blueprintProgression.LevelEntries)
    {
        Console.WriteLine("- " + progressionLevel.Level.ToString());
        var level = new ProgressionBlueprintLevel
        {
            Level = progressionLevel.Level
        };

        foreach (var blueprintId in progressionLevel.GetFeatureBlueprintIds())
        {
            Console.WriteLine("    - " + viewModel.Resources.GetFeatTemplate(blueprintId)?.DisplayName ?? blueprintId);
            level.FeatureBlueprintIds.Add(blueprintId);
        }
        progressionTemplate.Levels.Add(level);
    }
    progressionTemplates.Add(progressionTemplate);
}
File.WriteAllText("Progressions.json", JsonConvert.SerializeObject(progressionTemplates));

var featTemplates = new List<JObject>();
foreach (var featureBlueprintReference in blueprintData.GetEntries(BlueprintTypes.Feature))
{
    Console.WriteLine(featureBlueprintReference.DisplayName);
    var blueprintPath = Path.Combine(extractedBlueprintsDirectory, featureBlueprintReference.Path);
    if (!File.Exists(blueprintPath))
    {
        Console.WriteLine("Doesn't exist");
        continue;
    }

    var blueprintJson = JObject.Parse(File.ReadAllText(blueprintPath));
    var blueprint = new Blueprint(new ModelDataAccessor(blueprintJson, references, viewModel.Resources));

    var factTemplateRaw = new JObject();
    FeatureFactItemModel.Prepare(references, factTemplateRaw);
    var factTemplateAccessor = new ModelDataAccessor(factTemplateRaw, new References(viewModel.Resources), viewModel.Resources);
    var factTemplate = FactItemModel.Factory(factTemplateAccessor);
    factTemplate.Blueprint = featureBlueprintReference.Id;
    factTemplate.Context = new FactContextModel(new ModelDataAccessor(new JObject(), references, viewModel.Resources));
    factTemplate.Context.AssociatedBlueprint = featureBlueprintReference.Id;
        
    if (blueprint.Data is not BlueprintFeature blueprintFeature)
    {
        continue;
    }
    foreach (var component in blueprintFeature.Components)
    {
        if (factTemplate.Components.ContainsKey(component.Name))
        {
            Console.WriteLine("Warning - Duplicate component: " + component.Name);
            continue;
        }

        // Not all components need to be added,
        // and some components need extra data
        // Luckily, the game corrects both these problems for us
        factTemplate.Components.AddNull(component.Name);
    }

    featTemplates.Add(factTemplateRaw);
}

File.WriteAllText("FeatTemplates.json", JsonConvert.SerializeObject(featTemplates));
