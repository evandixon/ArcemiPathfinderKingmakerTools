using Arcemi.Pathfinder.Kingmaker.GameData;
using Arcemi.Pathfinder.Kingmaker.GameData.Blueprints;
using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Arcemi.Pathfinder.Kingmaker.Infrastructure.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class GameResources : IGameResourcesProvider
    {
        public GameResources()
        {
        }

        public void SetConfig(string gameFolder, IPathfinderAppData pathfinderAppData)
        {
            this.AppData = pathfinderAppData ?? throw new ArgumentNullException(nameof(pathfinderAppData));
            this.Blueprints = BlueprintMetadata.Load(gameFolder);
            if (!string.IsNullOrEmpty(gameFolder))
            {
                var references = new References(this);
                this.BlueprintsRepository = new BlueprintsRepository(Path.Combine(gameFolder, "blueprints.zip"), references, this);
            }
        }

        public IPathfinderAppData AppData { get; private set; }
        public BlueprintMetadata Blueprints { get; set; }
        public BlueprintsRepository BlueprintsRepository { get; private set; }

        public IReadOnlyDictionary<PortraitCategory, IReadOnlyList<Portrait>> GetAvailablePortraits()
        {
            //var unknownUri = AppData.Portraits.GetUnknownUri();
            //var unmappedPortraits = Blueprints.GetEntries(BlueprintTypes.Portrait)
            //    .Where(e => e.Name.Original.IndexOf("BCT_", StringComparison.OrdinalIgnoreCase) < 0)
            //    .Where(e => !AppData.Portraits.Available.Any(p => string.Equals(p.Key, e.Id, StringComparison.Ordinal)))
            //    .Select(e => new Portrait(e.Id, unknownUri, PortraitCategory.Unmapped, name: e.DisplayName))
            //    .ToArray();

            var res = AppData.Portraits.Available
                .GroupBy(p => p.Category)
                .OrderBy(g => g.Key.Order)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<Portrait>)g.ToArray());

            //res.Add(PortraitCategory.Unmapped, unmappedPortraits);
            return res;
        }

        public string GetLeaderPortraitUri(string blueprint)
        {
            if (TryGetLeader(blueprint, out var leader))
            {
                return GetPortraitsUri(leader.Portrait);
            }
            return GetPortraitsUri(blueprint);
        }

        public string GetPortraitsUri(string id)
        {
            return AppData.Portraits.GetPortraitsUri(id);
        }

        public bool TryGetPortraitsUri(string key, out string uri)
        {
            return AppData.Portraits.TryGetPortraitsUri(key, out uri);
        }

        public string GetPortraitId(string blueprint)
        {
            if (string.IsNullOrEmpty(blueprint))
            {
                return null;
            }

            if (Mappings.Leaders.TryGetValue(blueprint, out var leader))
            {
                return leader.Portrait.OrIfEmpty("_c_" + leader.Name);
            }
            if (Mappings.Characters.TryGetValue(blueprint, out var character))
            {
                return "_c_" + character.Name;
            }
            return null;
        }

        public string GetCharacterPotraitIdentifier(string blueprint)
        {
            if (Mappings.Characters.TryGetValue(blueprint, out var character))
            {
                if (!string.IsNullOrEmpty(character.Portrait))
                {
                    return character.Portrait;
                }
            }
            return blueprint;
        }

        public string GetCharacterName(string blueprint)
        {
            return Mappings.Characters.TryGetValue(blueprint, out var character)
                ? character.Name
                : Blueprints.TryGetName(blueprint, out var name) ? name : "";
        }

        public string GetArmyUnitName(string blueprint)
        {
            return Blueprints.TryGetName(blueprint, out var name) ? name : blueprint;
        }

        public IEnumerable<IBlueprintMetadataEntry> GetAvailableArmyUnits()
        {
            var blueprints = Blueprints.GetEntries(BlueprintTypes.Unit);
            return blueprints.Where(b => b.Name.StartsWith("Army"));
        }

        public bool TryGetLeader(string blueprint, out LeaderDataMapping leader)
        {
            return Mappings.Leaders.TryGetValue(blueprint, out leader);
        }

        public string GetLeaderName(string blueprint)
        {
            if (string.IsNullOrEmpty(blueprint))
            {
                return "";
            }

            if (Mappings.Leaders.TryGetValue(blueprint, out var leader))
            {
                return leader.Name;
            }
            if (Mappings.Characters.TryGetValue(blueprint, out var character))
            {
                return character.Name;
            }
            if (Blueprints.TryGetName(blueprint, out var name))
            {
                return name;
            }
            return "";
        }

        public string GetRaceName(string id)
        {
            if (string.IsNullOrEmpty(id)) return "N/A";
            return Mappings.Races.TryGetValue(id, out var m) ? m.Name : Blueprints.TryGetName(id, out var name) ? name : id;
        }

        public string GetClassTypeName(string id)
        {
            return Mappings.Classes.TryGetValue(id, out var mapping)
                ? mapping.Name
                : Blueprints.TryGetName(id, out var name) ? name : id;
        }

        public string GetClassArchetypeName(IReadOnlyList<string> archetypes)
        {
            if (archetypes == null || archetypes.Count == 0) return null;

            var name = archetypes
                .Select(a => Mappings.Classes.TryGetValue(a, out var cls) ? cls.Name : Blueprints.TryGetName(a, out var n) ? n : null)
                .Where(a => a != null)
                .FirstOrDefault();

            if (name != null) return name;

            return archetypes.First();
        }

        public bool IsMythicClass(string blueprint)
        {
            return Mappings.Classes.TryGetValue(blueprint, out var cls) && (cls.Flags?.Contains('M') ?? false);
        }

        public string GetItemName(string blueprint)
        {
            return Blueprints.TryGetName(blueprint, out var name) ? name : null;
        }

        public async Task<FactItemModel> GetFeatTemplate(string blueprintId)
        {
            if (BlueprintsRepository == null)
            {
                return null;
            }

            var references = new References(this);
            var blueprintFeature = await BlueprintsRepository.GetBlueprint<BlueprintFeature>(blueprintId);
            var factTemplateRaw = new JObject();
            FeatureFactItemModel.Prepare(references, factTemplateRaw);
            var factTemplateAccessor = new ModelDataAccessor(factTemplateRaw, references, this);
            var factTemplate = FactItemModel.Factory(factTemplateAccessor);
            factTemplate.Blueprint = blueprintId;
            factTemplate.Context = new FactContextModel(new ModelDataAccessor(new JObject(), references, this))
            {
                AssociatedBlueprint = blueprintId
            };

            foreach (var component in blueprintFeature.Data.Components)
            {
                if (factTemplate.Components.ContainsKey(component.Name))
                {
                    continue;
                }

                // Not all components need to be added,
                // and some components need extra data
                // Luckily, the game corrects both these problems for us
                factTemplate.Components.AddNull(component.Name);
            }

            return factTemplate;
        }
    }
}
