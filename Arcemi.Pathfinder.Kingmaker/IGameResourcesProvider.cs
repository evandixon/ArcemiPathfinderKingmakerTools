using Arcemi.Pathfinder.Kingmaker.GameData;
using Arcemi.Pathfinder.Kingmaker.GameData.Blueprints;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker
{
    public interface IGameResourcesProvider
    {
        IPathfinderAppData AppData { get; }
        BlueprintMetadata Blueprints { get; }
        BlueprintsRepository BlueprintsRepository { get; }

        string GetPortraitId(string blueprint);
        string GetCharacterPotraitIdentifier(string blueprint);
        string GetLeaderPortraitUri(string blueprint);
        string GetPortraitsUri(string id);
        bool TryGetPortraitsUri(string characterBlueprint, out string uri);
        IReadOnlyDictionary<PortraitCategory, IReadOnlyList<Portrait>> GetAvailablePortraits();

        string GetCharacterName(string blueprint);
        string GetArmyUnitName(string blueprint);
        IEnumerable<IBlueprintMetadataEntry> GetAvailableArmyUnits();
        bool TryGetLeader(string blueprint, out LeaderDataMapping leader);
        string GetLeaderName(string blueprint);

        string GetRaceName(string id);
        string GetClassTypeName(string id);
        string GetClassArchetypeName(IReadOnlyList<string> archetypes);
        bool IsMythicClass(string blueprint);
        string GetItemName(string blueprint);

        Task<FactItemModel> GetFeatTemplate(string blueprint);
    }
}
