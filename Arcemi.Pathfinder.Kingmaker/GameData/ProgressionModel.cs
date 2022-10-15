#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Arcemi.Pathfinder.Kingmaker.GameData.Blueprints;
using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class ProgressionModel : RefModel
    {
        public ProgressionModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<ClassModel> Classes => A.List(factory: a => new ClassModel(a));
        public FeaturesModel Features => A.Object(factory: a => new FeaturesModel(a));
        public ListAccessor<ProgressionItemModel> Items => A.List("m_Progressions", a => new ProgressionItemModel(a));
        public ListAccessor<ProgressionSelectionModel> Selections => A.List("m_Selections", a => new ProgressionSelectionModel(a));
        public int Experience { get => A.Value<int>(); set => A.Value(value); }
        public int MythicExperience { get => A.Value<int>(); set => A.Value(value); }
        public string Race { get => A.Value<string>("m_Race"); set => A.Value(value, "m_Race"); }
        public string RaceName => A.Res.GetRaceName(Race);
        public int CurrentLevel => Classes?.Where(c => !c.IsMythic).Sum(c => c.Level) ?? 0;

        public async Task<ProgressionItemModel> GetProgression(string characterClassBlueprintId, IGameResourcesProvider gameResourcesProvider)
        {
            var classData = await gameResourcesProvider.BlueprintsRepository.GetBlueprint<BlueprintCharacterClass>(characterClassBlueprintId);
            var progressionId = classData?.Data?.m_Progression?.Id;
            if (progressionId == null)
            {
                return null;
            }

            return Items.FirstOrDefault(p => p.Key == progressionId);
        }

        public ProgressionItemModel GetBasicFeatsProgression()
        {
            // This progression is a bit of a special case
            // Most progressions are tied to classes or features
            // But this one everyone has
            return Items.FirstOrDefault(p => p.Key == "5b72dd2ca2cb73b49903806ee8986325");
        }

        public bool ReplaceSelection(string featureSelectionBlueprintId, string sourceBlueprintId, int level, string oldValue, string newValue)
        {
            var levelStr = level.ToString();
            var selection = Selections.FirstOrDefault(s => s.Key == featureSelectionBlueprintId);
            if (selection == null)
            {
                selection = Selections.Add(ProgressionSelectionModel.Prepare);
                selection.Key = featureSelectionBlueprintId;
                selection.Value.Source.Blueprint = sourceBlueprintId;
            }

            if (!selection.Value.ByLevel.ContainsKey(levelStr))
            {
                selection.Value.ByLevel.Add(levelStr);
            }

            var index = selection.Value.ByLevel[levelStr].ToList().IndexOf(oldValue);
            if (index != -1)
            {
                selection.Value.ByLevel[levelStr][index] = newValue;
            }
            else
            {
                selection.Value.ByLevel[levelStr].Add(newValue);
            }
            return true;
        }
    }
}