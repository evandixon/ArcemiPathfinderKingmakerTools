﻿using Arcemi.Pathfinder.Kingmaker;
using Arcemi.Pathfinder.Kingmaker.GameData;
using Arcemi.Pathfinder.Kingmaker.GameData.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class CharacterLevelManipulator
    {
        public CharacterLevelManipulator(UnitEntityModel unit, IGameResourcesProvider resources)
        {
            Unit = unit;
            Resources = resources;
            if (unit == null) {
                return;
            }
            ProgressionBlueprints = unit.Descriptor.Progression.Classes.Select(cls => {
                var blueprints = (IReadOnlyList<IBlueprintMetadataEntry>)unit.Facts.Items
                    .OfType<FeatureFactItemModel>()
                    .Select(f => {
                        if (!resources.Blueprints.TryGet(f.Blueprint, out var bp)) return null;
                        return bp.Type == BlueprintTypes.Progression
                            ? bp
                            : null;
                    })
                    .Where(bp => bp != null)
                    .ToArray();
                return (cls.CharacterClass, blueprints);
            })
            .ToDictionary(x => x.CharacterClass, x => x.blueprints, StringComparer.Ordinal);
        }

        public UnitEntityModel Unit { get; }
        public IGameResourcesProvider Resources { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<IBlueprintMetadataEntry>> ProgressionBlueprints { get; }

        public bool CanDowngrade(ClassModel cls)
        {
            return true;
        }

        public async Task DowngradeClass(ClassModel cls)
        {
            var classData = await Resources.BlueprintsRepository.GetBlueprint<BlueprintCharacterClass>(cls.CharacterClass);
            if (!ProgressionBlueprints.TryGetValue(cls.CharacterClass, out var blueprints))
            {
                blueprints = new List<IBlueprintMetadataEntry>
                {
                    Resources.Blueprints.Get(classData.Data.m_Progression.Id)
                };
            }

            var progression = Unit.Descriptor.Progression;

            // Some advanced classes is advancing base classes, so we find those progression blueprints as well
            var progressionLookup = new HashSet<string>(blueprints.Select(x => x.Id), StringComparer.Ordinal);
            var selectionProgressions = progression.Selections
                .Where(x => progressionLookup.Contains(x.Value.Source.Blueprint))
                .SelectMany(x => x.Value.ByLevel.Values.SelectMany(blList => blList.Select(v => Resources.Blueprints.TryGet(v, out var bp) ? bp : null)))
                .Where(x => x != null);
            foreach (var selectionProgression in selectionProgressions) {
                progressionLookup.Add(selectionProgression.Id);
            }

            var level = progression.CurrentLevel;
            var clsLevel = cls.Level;
            var clsBlueprints = cls.Archetypes?.Any() ?? false
                ? new HashSet<string>(cls.Archetypes, StringComparer.Ordinal)
                : new HashSet<string>(StringComparer.Ordinal);
            clsBlueprints.Add(cls.CharacterClass);

            // Remove things selected when leveling up to the level we're removing
            var clsLevelStr = clsLevel.ToString();
            var levelStr = level.ToString();
            var selection = progression.Selections.FirstOrDefault(s => s.Value.Source.Blueprint == classData.Data.m_Progression.Id);
            if (selection != null)
            {
                RemoveSelection(progression, selection, clsLevel);
            }

            //for (var i = progression.Selections.Count - 1; i >= 0; i--) {
            //    var selection = progression.Selections[i];

            //    // Remove class selections

            //    if (progressionLookup.Contains(selection.Value.Source.Blueprint))
            //    {
            //        RemoveSelection(progression, i, clsLevel);
            //    }

            //    // Remove character selections
            //    // Was this originally done for feat progressions?
            //    //if (!cls.IsMythic)
            //    //{
            //    //    RemoveSelection(progression, i, level);
            //    //}
            //}

            // Remove features added by the class itself
            await RemoveClassFeatures(cls);

            // Decrease the level on the class
            var progressionsDecreased = new List<ProgressionItemModel>();
            foreach (var item in progression.Items.Where(p => p.Key == classData.Data.m_Progression.Id))
            {
                // Lower the overall character level
                if (!cls.IsMythic)
                {
                    if (item.Value.Level == level)
                    {
                        item.Value.Level--;
                        progressionsDecreased.Add(item);
                        continue;
                    }
                }

                // Lower the individual class level
                if (!progressionLookup.Contains(item.Key)) continue;
                if (item.Value.Level == clsLevel)
                {
                    item.Value.Level--;
                    progressionsDecreased.Add(item);
                }
            }
            cls.Level = clsLevel - 1;
            await RefreshClassFeatures(cls);

            // Re-add any features that were superceded by ones we just removed
            foreach (var decreasedProgression in progressionsDecreased)
            {
                await AddMissingFeatures(decreasedProgression, decreasedProgression.Value.Level);
            }

            // Remove the class if it's been set to level 0
            if (cls.Level < 1)
            {
                progression.Classes.Remove(cls);
            }
        }

        public async Task RefreshClassFeatures(ClassModel cls)
        {
            var classData = await Resources.BlueprintsRepository.GetBlueprint<BlueprintCharacterClass>(cls.CharacterClass);
            for (int level = 1; level <= cls.Level; level++)
            {
                var progression = Unit.Descriptor.Progression.Items.FirstOrDefault(p => p.Key == classData.Data.m_Progression.Id);
                if (progression == null)
                {
                    progression = Unit.Descriptor.Progression.Items.Add(ProgressionItemModel.Prepare);
                }

                // Reset the progression and reapply it
                progression.Value.Level = 0;
                await UpgradeProgression(progression, level, classData);
            }
        }

        private void RemoveSelection(ProgressionModel progression, ProgressionSelectionModel selection, int level)
        {
            var levelStr = level.ToString();
            var progressionBlueprintId = selection.Value.Source.Blueprint;

            if (selection.Value.ByLevel.ContainsKey(levelStr))
            {
                foreach (var featureBlueprintId in selection.Value.ByLevel[levelStr])
                {
                    RemoveFeature(featureBlueprintId, level, progressionBlueprintId);
                }

                selection.Value.ByLevel.Remove(levelStr);
                if (selection.Value.ByLevel.Count == 0)
                {
                    progression.Selections.Remove(selection);
                }
            }
        }

        private async Task RemoveClassFeatures(ClassModel cls)
        {
            var classData = await Resources.BlueprintsRepository.GetBlueprint<BlueprintCharacterClass>(cls.CharacterClass);
            if (classData == null)
            {
                return;
            }

            var toRemove = Unit.Facts.Items.Where(fact => fact is FeatureFactItemModel feature
                && feature.Source == classData.Data.m_Progression.Id
                && feature.SourceLevel == cls.Level).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeature(fact, cls.Level, classData.Data.m_Progression.Id);
            }
        }

        public bool RemoveFeatureByBlueprint(string blueprintId, int sourceLevel, string sourceBlueprintId)
        {
            var feature = GetFeature(blueprintId, sourceLevel, sourceBlueprintId);
            if (feature == null)
            {
                return false;
            }

            return RemoveFeature(feature, sourceLevel, sourceBlueprintId);
        }

        private void RemoveFeaturesById(string id)
        {
            var toRemove = Unit.Facts.Items.Where(f => f.Id == id).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeature(fact);
            }
        }

        public bool RemoveFeaturesByBlueprint(string blueprintId)
        {
            var toRemove = Unit.Facts.Items.Where(fact => fact.Blueprint == blueprintId).ToList();
            var results = new List<bool>();
            foreach (var fact in toRemove)
            {
                RemoveFeature(fact);
            }
            return !results.Any() || results.All(r => r == true);
        }

        private bool RemoveFeature(string featureBlueprintId, int sourceLevel, string sourceBlueprintId)
        {
            var feature = GetFeature(featureBlueprintId, sourceLevel, sourceBlueprintId);
            if (feature == null)
            {
                return false;
            }

            return RemoveFeature(feature, sourceLevel, sourceBlueprintId);
        }

        private bool RemoveFeature(FactItemModel fact, int sourceLevel = -1, string sourceBlueprintId = null)
        {
            if (fact == null)
            {
                return false;
            }

            // Remove facts added by this fact
            foreach (var component in fact.Components)
            {
                if (component.Value is AddFactsComponentModel addFactsComponent)
                {
                    foreach (var addedFact in addFactsComponent.Data.AppliedFacts)
                    {
                        if (addedFact is ActivatableAbilityFactItemModel activatableAbilityFact && activatableAbilityFact.m_AppliedBuff != null)
                        {
                            RemoveFeaturesById(activatableAbilityFact.m_AppliedBuff.Id);
                        }

                        RemoveFeature(addedFact);
                    }
                }
            }

            if (!string.IsNullOrEmpty(sourceBlueprintId) && sourceLevel > -1
                && fact is FeatureFactItemModel feature && feature.Rank > 1)
            {
                // Simply remove one rank since it came from multiple sources
                feature.Rank -= 1;
                var sourcesToRemove = feature.RankToSource.Where(r => r.Level == sourceLevel && r.Blueprint == sourceBlueprintId).ToList();
                foreach (var source in sourcesToRemove)
                {
                    feature.RankToSource.Remove(source);
                }
                return true;
            }
            else
            {
                // Remove the fact itself
                var result = Unit.Facts.Items.Remove(fact);
                Unit.Descriptor.UISettings.m_AlreadyAutomaticallyAdded.Remove(fact.Blueprint);
                return result;
            }
        }

        public async Task AddClass(string blueprintId)
        {
            // Class
            var classData = await Resources.BlueprintsRepository.GetBlueprint<BlueprintCharacterClass>(blueprintId);
            var characterClass = Unit.Descriptor.Progression.Classes.FirstOrDefault(c => c.CharacterClass == blueprintId);
            if (characterClass != null)
            {
                // Add a level to the class
                characterClass.Level += 1;
            }
            else
            {
                // Add the class
                characterClass = Unit.Descriptor.Progression.Classes.Add(ClassModel.Prepare);
                characterClass.CharacterClass = blueprintId;
                characterClass.Level = 1;
            }

            // Progression
            var progression = Unit.Descriptor.Progression.Items.FirstOrDefault(p => p.Key == classData.Data.m_Progression.Id);
            if (progression == null)
            { 
                progression = Unit.Descriptor.Progression.Items.Add(ProgressionItemModel.Prepare);
                progression.Key = classData.Data.m_Progression.Id;
                progression.Value.Level = 0;
            }

            await UpgradeProgression(progression, characterClass.Level, classData);
        }

        public async Task AddClassArchetype(string classId, string archetypeId)
        {
            var characterClass = Unit.Descriptor.Progression.Classes.First(c => c.CharacterClass == classId);
            if (characterClass.Archetypes.Any(a => a == archetypeId))
            {
                return;
            }

            characterClass.Archetypes.Add(archetypeId);

            for (int i = 1; i <= characterClass.Level; i++)
            {
                await ApplyArchetypeLevel(classId, i);
            }
        }

        private async Task ApplyArchetypeLevel(string classId, int level)
        {
            var characterClass = Unit.Descriptor.Progression.Classes.FirstOrDefault(c => c.Id == classId);
            if (characterClass == null)
            {
                return;
            }

            var classData = await Resources.BlueprintsRepository.GetBlueprint<BlueprintCharacterClass>(classId);
            foreach (var archetype in characterClass.Archetypes ?? Enumerable.Empty<string>())
            {
                var archetypeReference = classData.Data.m_Archetypes.FirstOrDefault(a => a.Id == archetype);
                var archetypeData = archetypeReference.Blueprint;
                foreach (var featureToRemove in archetypeData.Data.RemoveFeatures
                    .FirstOrDefault(f => f.Level == level)?.m_Features ?? Enumerable.Empty<BlueprintReference>())
                {
                    RemoveFeaturesByBlueprint(featureToRemove.Id);
                }
                foreach (var featureToAdd in archetypeData.Data.AddFeatures
                    .FirstOrDefault(f => f.Level == level)?.m_Features ?? Enumerable.Empty<BlueprintReference>())
                {
                    await AddFeature(level, classData.Data.m_Progression.Id, featureToAdd);
                }
            }
        }

        private async Task UpgradeProgression(ProgressionItemModel progression, int targetLevel, Blueprint<BlueprintCharacterClass> classData)
        {
            for (int currentLevel = progression.Value.Level + 1; currentLevel <= targetLevel; currentLevel++)
            {
                var classLevel = classData.Data.m_Progression.Blueprint.Data.LevelEntries.FirstOrDefault(l => l.Level == currentLevel);
                if (classLevel == null)
                {
                    return;
                }

                foreach (var feature in classLevel.m_Features)
                {
                    // Relying on side effects:
                    // 1. Not adding an already-existing feature
                    // 2. Removing any features this one replaces
                    await AddFeature(currentLevel, classData.Data.m_Progression.Id, feature);
                }

                await ApplyArchetypeLevel(classData.AssetId, currentLevel);

                progression.Value.Level = currentLevel;
            }
        }

        private async Task AddReplacedClassFeatures(string classId, int removedLevel, string removedFeatureId)
        {
            var classData = await Resources.BlueprintsRepository.GetBlueprint<BlueprintCharacterClass>(classId);
            var removedFeature = classData.Data.m_Progression.Blueprint.Data
                .LevelEntries
                .FirstOrDefault(l => l.Level == removedLevel)
                ?.m_Features.FirstOrDefault(f => f.Id == removedFeatureId);
            if (removedFeature == null)
            {
                return;
            }
            foreach (var component in removedFeature.Blueprint.Data.Components)
            {
                if (component is BlueprintComponentRemoveFeatureOnApply blueprintComponentRemoveFeatureOnApply)
                {
                    await AddReplacedClassFeature(classData, blueprintComponentRemoveFeatureOnApply.m_Feature);
                }
            }
        }

        private async Task AddReplacedClassFeature(Blueprint<BlueprintCharacterClass> classData, string featureToAdd)
        {
            foreach (var level in classData.Data.m_Progression.Blueprint.Data.LevelEntries)
            {
                var feature = level.m_Features.FirstOrDefault(f => f.Id == featureToAdd);
                if (feature == null)
                {
                    continue;
                }

                await AddFeature(level.Level, classData.Data.m_Progression.Id, feature);
            }
        }

        public async Task AddMissingFeatures(ProgressionItemModel progression, int level)
        {
            var blueprint = await Resources.BlueprintsRepository.GetBlueprint<BlueprintProgression>(progression.Key);
            if (blueprint == null)
            {
                return;
            }

            var newLevel = blueprint.Data.LevelEntries.FirstOrDefault(l => l.Level == level);
            if (newLevel == null)
            {
                return;
            }
            foreach (var feature in newLevel.m_Features)
            {
                if (FeatureExists(feature.Id, level, progression.Key))
                {
                    continue;
                }

                await AddFeature(level, progression.Key, feature);
            }
        }

        public async Task AddFeature(int level, string progressionId, string featureBlueprintId)
        {
            if (FeatureExists(featureBlueprintId, level, progressionId))
            {
                return;
            }

            var addedFeature = Unit.Facts.Items.FirstOrDefault(f => f.Blueprint == featureBlueprintId)
                as FeatureFactItemModel;
            if (addedFeature == null)
            {
                // Doesn't exist. Create and add it.
                var template = await Resources.GetFeatTemplate(featureBlueprintId);
                if (template == null)
                {
                    return;
                }

                addedFeature = (FeatureFactItemModel)Unit.Facts.Items.Add(FeatureFactItemModel.Prepare);
                addedFeature.Import(template);
            }
            else
            {
                // It does exist. Increase its rank if applicable
                if (addedFeature.RankToSource != null)
                {
                    addedFeature.Rank += 1;
                    var rankSource = addedFeature.RankToSource.Add(FeatureRankToSourceModel.Prepare);
                    rankSource.Blueprint = progressionId;
                    rankSource.Level = level;
                }
            }

            addedFeature.Source = progressionId;
            addedFeature.SourceLevel = level;
            addedFeature.Context.OwnerRef = Unit.UniqueId;
        }

        public FeatureFactItemModel GetFeature(string featureBlueprintId, int level, string sourceBlueprintId)
        {
            return Unit.Facts.Items
                .FirstOrDefault(fact => fact is FeatureFactItemModel feature
                    && feature.Blueprint == featureBlueprintId
                    && (
                            (feature.Source == sourceBlueprintId && feature.SourceLevel == level)
                            || (feature.RankToSource != null && feature.RankToSource.Any(r => r.Blueprint == sourceBlueprintId && r.Level == level))
                        )
                    ) as FeatureFactItemModel;
        }

        public bool FeatureExists(string featureBlueprintId, int level, string sourceBlueprintId)
        {
            return GetFeature(featureBlueprintId, level, sourceBlueprintId) != null;
        }

        public async Task AddFeature(int level, string progressionId, Blueprint<BlueprintFeature> featureData)
        {
            await AddFeature(level, progressionId, featureData.AssetId);

            foreach (var component in featureData.Data.Components)
            {
                if (component is BlueprintComponentRemoveFeatureOnApply featureToRemove)
                {
                    RemoveFeatureByBlueprint(featureToRemove.m_Feature.Id, level, progressionId);
                }
            }
        }
    }
}
