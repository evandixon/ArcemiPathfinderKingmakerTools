using Arcemi.Pathfinder.Kingmaker.GameData;
using Arcemi.Pathfinder.Kingmaker.GameData.Blueprints;
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

Blueprint? LoadBlueprint(IBlueprintMetadataEntry blueprintMetadata)
{
    Console.WriteLine(blueprintMetadata.DisplayName);
    var blueprintPath = Path.Combine(extractedBlueprintsDirectory, blueprintMetadata.Path);
    if (!File.Exists(blueprintPath))
    {
        Console.WriteLine("Doesn't exist");
        return null;
    }

    var blueprintJson = JObject.Parse(File.ReadAllText(blueprintPath));
    return new Blueprint(new ModelDataAccessor(blueprintJson, references, viewModel.Resources));
}

string? GetBlueprintId(string? blueprintReferenceId)
{
    return blueprintReferenceId?.Replace("!bp_", "");
}

var featTemplates = new List<JObject>();
var featModels = new Dictionary<string, FeatureBlueprintModel>();
foreach (var blueprintMetadata in 
    blueprintData.GetEntries(BlueprintTypes.Feature)
    .Concat(blueprintData.GetEntries(BlueprintTypes.FeatureSelection)))
{
    var blueprint = LoadBlueprint(blueprintMetadata);
    if (blueprint?.Data is not BlueprintFeature blueprintFeature)
    {
        continue;
    }

    var factTemplateRaw = new JObject();
    FeatureFactItemModel.Prepare(references, factTemplateRaw);
    var factTemplateAccessor = new ModelDataAccessor(factTemplateRaw, new References(viewModel.Resources), viewModel.Resources);
    var factTemplate = FactItemModel.Factory(factTemplateAccessor);
    factTemplate.Blueprint = blueprintMetadata.Id;
    factTemplate.Context = new FactContextModel(new ModelDataAccessor(new JObject(), references, viewModel.Resources))
    {
        AssociatedBlueprint = blueprintMetadata.Id
    };

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

    var featModel = new FeatureBlueprintModel
    {
        Id = blueprintMetadata.Id,
        RemoveFeaturesIdOnApply = blueprintFeature.Components
            .Where(f => f is BlueprintComponentRemoveFeatureOnApply).Cast<BlueprintComponentRemoveFeatureOnApply>()
            .Select(f => GetBlueprintId(f.m_Feature))
            .ToList()
    };
    if (blueprintFeature is BlueprintFeatureSelection blueprintFeatureSelection)
    {
        featModel.IsSelection = true;
        featModel.SelectionFeatureIdOptions = blueprintFeatureSelection
            .m_AllFeatures
            .Select(f => GetBlueprintId(f))
            .ToList();
    }
    featModels.Add(blueprintMetadata.Id, featModel);
}
File.WriteAllText("FeatTemplates.json", JsonConvert.SerializeObject(featTemplates));

ProgressionLevelBlueprintModel ToModel(BlueprintProgressionLevel progressionLevel)
{
    Console.WriteLine("- " + progressionLevel.Level.ToString());
    var level = new ProgressionLevelBlueprintModel
    {
        Level = progressionLevel.Level
    };

    foreach (var blueprintId in progressionLevel.GetFeatureBlueprintIds())
    {
        Console.WriteLine("    - " + viewModel.Resources.GetFeatTemplate(blueprintId)?.DisplayName ?? blueprintId);
        var feature = featModels.GetValueOrDefault(blueprintId);
        if (feature != null)
        {
            level.Features.Add(feature);
        }
    }
    return level;
}

var progressionTemplates = new Dictionary<string, ProgressionBlueprintModel>();
foreach (var blueprintMetadata in blueprintData.GetEntries(BlueprintTypes.Progression))
{
    var blueprint = LoadBlueprint(blueprintMetadata);
    if (blueprint?.Data is not BlueprintProgression blueprintProgression)
    {
        continue;
    }

    progressionTemplates.Add(blueprint.AssetId, new ProgressionBlueprintModel
    {
        BlueprintId = blueprint.AssetId,
        Levels = blueprintProgression.LevelEntries.Select(l => ToModel(l)).ToList()
    });
}
File.WriteAllText("Progressions.json", JsonConvert.SerializeObject(progressionTemplates.Values));

var archetypeModels = new Dictionary<string, ClassArchetypeBlueprintModel>();
foreach (var blueprintMetadata in blueprintData.GetEntries(BlueprintTypes.Archetype))
{
    var blueprint = LoadBlueprint(blueprintMetadata);
    if (blueprint?.Data is not BlueprintArchetype blueprintArchetype)
    {
        continue;
    }

    archetypeModels.Add(blueprintMetadata.Id, new ClassArchetypeBlueprintModel
    {
        Id = blueprintMetadata.Id,
        ReplacementSpellbook = GetBlueprintId(blueprintArchetype.m_ReplaceSpellbook),
        RemoveSpellbook = blueprintArchetype.RemoveSpellbook,
        AddFeatures = blueprintArchetype.AddFeatures.Select(l => ToModel(l)).ToList(),
        RemoveFeatures = blueprintArchetype.RemoveFeatures.Select(l => ToModel(l)).ToList(),
    });
}

var classModels = new Dictionary<string, ClassBlueprintModel>();
foreach (var blueprintMetadata in blueprintData.GetEntries(BlueprintTypes.CharacterClass))
{
    var blueprint = LoadBlueprint(blueprintMetadata);
    if (blueprint?.Data is not BlueprintCharacterClass blueprintCharacterClass)
    {
        continue;
    }

    classModels.Add(blueprintMetadata.Id, new ClassBlueprintModel
    {
        Id = blueprintMetadata.Id,
        SpellbookId = GetBlueprintId(blueprintCharacterClass.m_Spellbook),
        IsMythic = blueprintCharacterClass.IsMythic,
        Archetypes = blueprintCharacterClass.m_Archetypes
            .Select(a => archetypeModels.GetValueOrDefault(GetBlueprintId(a.Replace("!bp_", ""))!))
            .Where(a => a != null)
            .ToList(),
        Progression = progressionTemplates.GetValueOrDefault(GetBlueprintId(blueprintCharacterClass.m_Progression)!)
    });
}
File.WriteAllText("ClassData.json", JsonConvert.SerializeObject(classModels.Values));
