using Arcemi.Pathfinder.Kingmaker;
using Arcemi.Pathfinder.Kingmaker.GameData;
using Arcemi.Pathfinder.Kingmaker.GameData.Blueprints;
using Arcemi.Pathfinder.Kingmaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void DowngradeClass(ClassModel cls, bool preserveFeatures = false)
        {
            if (!ProgressionBlueprints.TryGetValue(cls.CharacterClass, out var blueprints))
            {
                var classData = Resources.GetClassData(cls.CharacterClass);
                blueprints = new List<IBlueprintMetadataEntry>
                {
                    Resources.Blueprints.Get(classData.Progression.BlueprintId)
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

            // Decrease the level on the class
            var progressionsDecreased = new List<ProgressionItemModel>();
            foreach (var item in progression.Items) {
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
                if (item.Value.Level == clsLevel) {
                    item.Value.Level--;
                    progressionsDecreased.Add(item);
                }
            }
            cls.Level = clsLevel - 1;

            // Remove things selected when leveling up to the level we're removing
            var clsLevelStr = clsLevel.ToString();
            var levelStr = level.ToString();
            for (var i = progression.Selections.Count - 1; i >= 0; i--) {
                var selection = progression.Selections[i];

                // Remove class selections
                if (progressionLookup.Contains(selection.Value.Source.Blueprint))
                {
                    RemoveSelection(progression, i, clsLevelStr, preserveFeatures);
                }

                // Remove character selections
                if (!cls.IsMythic)
                {
                    RemoveSelection(progression, i, levelStr, preserveFeatures);
                }
            }

            // If enabled, re-add any features that were superceded by ones we just removed
            if (!preserveFeatures)
            {
                foreach (var decreasedProgression in progressionsDecreased)
                {
                    AddClassLevelFeatures(decreasedProgression, decreasedProgression.Value.Level);
                }
            }

            // Remove the class if it's been set to level 0
            if (cls.Level < 1)
            {
                progression.Classes.Remove(cls);
            }
        }

        private void RemoveSelection(ProgressionModel progression, int i, string levelStr, bool preserveFeatures)
        {
            var selection = progression.Selections[i];

            if (selection.Value.ByLevel.ContainsKey(levelStr))
            {
                if (!preserveFeatures)
                {
                    RemoveClassFeatures(selection.Value.Source.Blueprint, int.Parse(levelStr));
                }

                selection.Value.ByLevel.Remove(levelStr);
                if (selection.Value.ByLevel.Count == 0)
                {
                    progression.Selections.RemoveAt(i);
                }
            }
        }

        private void RemoveClassFeatures(string classProgressionBlueprintId, int level)
        {
            var classData = Resources.ClassData.FirstOrDefault(c => c.Progression.BlueprintId == classProgressionBlueprintId);
            if (classData == null)
            {
                return;
            }

            var toRemove = Unit.Facts.Items.Where(fact => fact is FeatureFactItemModel feature
                && feature.Source == classProgressionBlueprintId
                && feature.SourceLevel == level).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeatureAndRestoreReplaced(fact, classData.Id, level);
            }
        }

        private void RemoveFeatureByBlueprint(string blueprintId)
        {
            var toRemove = Unit.Facts.Items.Where(fact => fact.Blueprint == blueprintId).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeature(fact);
            }
        }

        private void RemoveFeatureByBlueprintAndRestoreReplaced(string blueprintId, string classId, int removedLevel)
        {
            var toRemove = Unit.Facts.Items.Where(fact => fact.Blueprint == blueprintId).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeatureAndRestoreReplaced(fact, classId, removedLevel);
            }
        }

        private void RemoveFeatureById(string id)
        {
            var toRemove = Unit.Facts.Items.Where(f => f.Id == id).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeature(fact);
            }
        }
        private void RemoveFeatureByIdAndRestoreReplaced(string id, string classId, int removedLevel)
        {
            var toRemove = Unit.Facts.Items.Where(f => f.Id == id).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeatureAndRestoreReplaced(fact, classId, removedLevel);
            }
        }

        private void RemoveFeature(FactItemModel fact)
        {
            // Remove facts added by this fact
            foreach (var component in fact.Components)
            {
                if (component.Value is AddFactsComponentModel addFactsComponent)
                {
                    foreach (var addedFact in addFactsComponent.Data.AppliedFacts)
                    {
                        if (addedFact is ActivatableAbilityFactItemModel activatableAbilityFact)
                        {
                            RemoveFeatureById(activatableAbilityFact.m_AppliedBuff.Id);
                        }

                        RemoveFeatureByBlueprint(addedFact.Blueprint);
                    }
                }
            }

            // Remove the fact itself
            Unit.Facts.Items.Remove(fact);
            Unit.Descriptor.UISettings.m_AlreadyAutomaticallyAdded.Remove(fact.Blueprint);
        }

        private void RemoveFeatureAndRestoreReplaced(FactItemModel fact, string classId, int removedLevel)
        {
            // Remove facts added by this fact
            foreach (var component in fact.Components)
            {
                if (component.Value is AddFactsComponentModel addFactsComponent)
                {
                    foreach (var addedFact in addFactsComponent.Data.AppliedFacts)
                    {
                        if (addedFact is ActivatableAbilityFactItemModel activatableAbilityFact)
                        {
                            RemoveFeatureByIdAndRestoreReplaced(activatableAbilityFact.m_AppliedBuff.Id, classId, removedLevel);
                        }

                        RemoveFeatureByBlueprintAndRestoreReplaced(addedFact.Blueprint, classId, removedLevel);
                    }
                }
            }

            // Remove the fact itself
            Unit.Facts.Items.Remove(fact);
            Unit.Descriptor.UISettings.m_AlreadyAutomaticallyAdded.Remove(fact.Blueprint);

            AddReplacedClassFeatures(classId, removedLevel, fact.Blueprint);
        }

        public void AddClass(string blueprintId)
        {
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

            var classData = Resources.GetClassData(blueprintId);
            var classLevel = classData.Progression.Levels.FirstOrDefault(l => l.Level == characterClass.Level);
            if (classLevel == null)
            {
                return;
            }

            foreach (var feature in classLevel.Features)
            {
                AddFeature(characterClass.Level, classData.Progression.BlueprintId, feature.Id);
                foreach (var featureToRemove in feature.RemoveFeaturesIdOnApply)
                {
                    RemoveFeatureByBlueprint(featureToRemove);
                }
            }

            ApplyArchetypeLevel(characterClass.Id, characterClass.Level);
        }

        public void AddClassArchetype(string classId, string archetypeId)
        {
            var characterClass = Unit.Descriptor.Progression.Classes.First(c => c.CharacterClass == classId);
            if (characterClass.Archetypes.Any(a => a == archetypeId))
            {
                return;
            }

            characterClass.Archetypes.Add(archetypeId);

            for (int i = 1; i <= characterClass.Level; i++)
            {
                ApplyArchetypeLevel(classId, i);
            }
        }

        private void ApplyArchetypeLevel(string classId, int level)
        {
            var characterClass = Unit.Descriptor.Progression.Classes.FirstOrDefault(c => c.Id == classId);
            if (characterClass == null)
            {
                return;
            }

            var classData = Resources.GetClassData(characterClass.Id);
            foreach (var archetype in characterClass.Archetypes ?? Enumerable.Empty<string>())
            {
                var archetypeData = classData.Archetypes.FirstOrDefault(a => a.Id == archetype);
                foreach (var featureToRemove in archetypeData.RemoveFeatures
                    .FirstOrDefault(f => f.Level == level)?.Features ?? Enumerable.Empty<FeatureBlueprintModel>())
                {
                    RemoveFeatureByBlueprint(featureToRemove.Id);
                }
                foreach (var featureToAdd in archetypeData.AddFeatures
                    .FirstOrDefault(f => f.Level == level)?.Features ?? Enumerable.Empty<FeatureBlueprintModel>())
                {
                    AddFeature(level, classData.Progression.BlueprintId, featureToAdd.Id);
                }
            }
        }

        private void AddReplacedClassFeatures(string classId, int removedLevel, string removedFeatureId)
        {
            var classData = Resources.GetClassData(classId);
            var removedFeature = classData.Progression
                .Levels.FirstOrDefault(l => l.Level == removedLevel)
                ?.Features.FirstOrDefault(f => f.Id == removedFeatureId);
            if (removedFeature == null)
            {
                return;
            }
            foreach (var replacedFeature in removedFeature.RemoveFeaturesIdOnApply ?? Enumerable.Empty<string>())
            {
                AddReplacedClassFeature(classData, replacedFeature);
            }
        }

        private void AddReplacedClassFeature(ClassBlueprintModel classData, string featureToAdd)
        {
            foreach (var level in classData.Progression.Levels)
            {
                var feature = level.Features.FirstOrDefault(f => f.Id == featureToAdd);
                if (feature == null)
                {
                    continue;
                }

                AddFeature(level.Level, classData.Progression.BlueprintId, feature.Id);
            }
        }

        public void AddClassLevelFeatures(ProgressionItemModel progression, int level)
        {
            var blueprint = Resources.GetProgression(progression.Key);
            if (blueprint == null)
            {
                return;
            }

            var newLevel = blueprint.Levels.FirstOrDefault(l => l.Level == level);
            if (newLevel == null)
            {
                return;
            }
            foreach (var feature in newLevel.Features)
            {
                if (Unit.Facts.Items.Any(f => f.Blueprint == feature.Id))
                {
                    continue;
                }

                AddFeature(level, progression.Key, feature.Id);
            }
        }

        private void AddFeature(int level, string progressionId, string featureId)
        {
            var template = Resources.GetFeatTemplate(featureId);
            if (template == null)
            {
                return;
            }

            var feature = (FeatureFactItemModel)Unit.Facts.Items.Add(FeatureFactItemModel.Prepare);
            feature.Import(template);
            feature.Source = progressionId;
            feature.SourceLevel = level;
            feature.Context.OwnerRef = Unit.UniqueId;
        }
    }
}
