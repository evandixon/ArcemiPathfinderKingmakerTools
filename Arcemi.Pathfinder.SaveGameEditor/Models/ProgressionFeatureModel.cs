using Arcemi.Pathfinder.Kingmaker;
using Arcemi.Pathfinder.Kingmaker.GameData;
using Arcemi.Pathfinder.Kingmaker.GameData.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class ProgressionFeatureModel
    {
        public static async Task<List<ProgressionFeatureModel>> FromProgression(ProgressionItemModel progression, UnitEntityModel unit, IGameResourcesProvider resources, CharacterLevelManipulator characterLevelManipulator) 
        {
            var viewModels = new List<ProgressionFeatureModel>();

            var progressionData = await resources.BlueprintsRepository.GetBlueprint<BlueprintProgression>(progression.Key);

            // Multiple selection values for a single feature means there's multiple features for this level
            // The first feature uses the first one, the second uses the second, etc
            var selectionIndexes = new Dictionary<(string selectionId, int level), int>();

            var progressionLevels = progressionData.Data.LevelEntries.Where(l => l.Level <= progression.Value.Level);
            foreach (var progressionLevelData in progressionLevels)
            {
                foreach (var featureData in progressionLevelData.m_Features)
                {
                    if (!unit.Facts.Items.Any(f => f.Blueprint == featureData.Id))
                    {
                        continue;
                    }

                    string selectionValue = null;
                    int? selectionIndex = null;
                    bool isSelection = false;
                    var selectionFeatures = new List<BlueprintReference>();
                    if (featureData.Blueprint.Data is BlueprintFeatureSelection blueprintFeatureSelection)
                    {
                        isSelection = true;
                        foreach (var featureReference in blueprintFeatureSelection.m_AllFeatures)
                        {
                            selectionFeatures.Add(featureReference);
                        }

                        var selection = unit.Descriptor.Progression.Selections.FirstOrDefault(s => s.Key == featureData.Id && s.Value.ByLevel.ContainsKey(progressionLevelData.Level.ToString()));
                        if (selection != null)
                        {
                            var selectionKey = (selectionId: featureData.Id, level: progressionLevelData.Level);
                            if (!selectionIndexes.ContainsKey(selectionKey))
                            {
                                selectionIndexes.Add(selectionKey, 0);
                            }

                            selectionIndex = selectionIndexes[selectionKey]++;
                            var selectionList = selection.Value.ByLevel[progressionLevelData.Level.ToString()];
                            selectionValue = selectionList.Count > selectionIndex ? selectionList[selectionIndex.Value] : null;
                        }
                    }

                    var viewModel = new ProgressionFeatureModel(unit, characterLevelManipulator, resources)
                    {
                        ProgressionBlueprintId = progressionData.AssetId,
                        FeatureBlueprintId = featureData.Id,
                        Level = progressionLevelData.Level,
                        DisplayName = resources.Blueprints.GetNameOrBlueprint(featureData.Id),
                        IsSelection = isSelection,
                        SelectionOptions = selectionFeatures
                            .ToDictionary(featureId => featureId, featureId => resources.Blueprints.GetNameOrBlueprint(featureId)),
                        SelectionValue = selectionValue
                    };
                    viewModels.Add(viewModel);
                }
            }

            return viewModels;
        }

        public ProgressionFeatureModel(UnitEntityModel unitEntityModel, CharacterLevelManipulator characterLevelManipulator, IGameResourcesProvider gameResourcesProvider)
        {
            this.unitEntityModel = unitEntityModel ?? throw new ArgumentNullException(nameof(unitEntityModel));
            this.characterLevelManipulator = characterLevelManipulator ?? throw new ArgumentNullException(nameof(characterLevelManipulator));
            this.gameResourcesProvider = gameResourcesProvider ?? throw new ArgumentNullException(nameof(gameResourcesProvider));
        }

        private readonly UnitEntityModel unitEntityModel;
        private readonly CharacterLevelManipulator characterLevelManipulator;
        private readonly IGameResourcesProvider gameResourcesProvider;

        public string ProgressionBlueprintId { get; set; }
        public string FeatureBlueprintId { get; set; }
        public int Level { get; set; }
        public string DisplayName { get; set; }
        public bool IsSelection { get; set; }
        public Dictionary<BlueprintReference, string> SelectionOptions { get; set; }
        public string SelectionValue { get; set; }

        /// <summary>
        /// If <see cref="SelectionValue"/> is a feature that has another selection, the view model representing that selection feature, or null if it is not
        /// </summary>
        /// <returns></returns>
        public async Task<ProgressionFeatureModel> GetSubSelection()
        {
            if (string.IsNullOrEmpty(SelectionValue))
            {
                return null;
            }

            var blueprint = await gameResourcesProvider.BlueprintsRepository.GetBlueprint(SelectionValue);
            if (blueprint?.Data is not BlueprintFeatureSelection selectionData)
            {
                return null;
            }

            string subSelectionValue = null;
            var selectionFeatures = new List<BlueprintReference>();
            foreach (var featureReference in selectionData.m_AllFeatures)
            {
                selectionFeatures.Add(featureReference);
            }

            var selection = unitEntityModel.Descriptor.Progression.Selections.FirstOrDefault(s => s.Key == blueprint.AssetId && s.Value.ByLevel.ContainsKey(Level.ToString()));
            if (selection != null)
            {
                // Let's assume for now that we're not going to have selections of selections
                // One example includes backgrounds, like Wanderer -> Hunter. We don't expect having multiple Wanderer values.
                const int selectionIndex = 0;

                var selectionList = selection.Value.ByLevel[Level.ToString()];
                subSelectionValue = selectionList.Count > selectionIndex ? selectionList[selectionIndex] : null;
            }

            return new ProgressionFeatureModel(unitEntityModel, characterLevelManipulator, gameResourcesProvider)
            {
                ProgressionBlueprintId = ProgressionBlueprintId,
                FeatureBlueprintId = SelectionValue,
                Level = Level,
                DisplayName = gameResourcesProvider.Blueprints.GetNameOrBlueprint(subSelectionValue),
                IsSelection = true,
                SelectionOptions = selectionFeatures
                        .ToDictionary(featureId => featureId, featureId => gameResourcesProvider.Blueprints.GetNameOrBlueprint(featureId)),
                SelectionValue = subSelectionValue
            };
        }

        public async Task ChangeSelectionValue(string value)
        {
            var subSelection = await GetSubSelection();
            if (subSelection != null)
            {
                await subSelection.ChangeSelectionValue(null);
            }

            unitEntityModel.Descriptor.Progression.ReplaceSelection(FeatureBlueprintId, ProgressionBlueprintId, Level, SelectionValue, value);
            if (!string.IsNullOrEmpty(SelectionValue))
            {
                characterLevelManipulator.RemoveFeatureByBlueprint(SelectionValue, Level, ProgressionBlueprintId);
            }
            if (!string.IsNullOrEmpty(value))
            {
                await characterLevelManipulator.AddFeature(Level, ProgressionBlueprintId, value);
            }
            SelectionValue = value;
        }
    }
}
