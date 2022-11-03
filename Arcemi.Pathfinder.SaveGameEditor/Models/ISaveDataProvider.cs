using Arcemi.Pathfinder.Kingmaker.GameData;
using Arcemi.Pathfinder.Kingmaker.GameData.Player;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public interface ISaveDataProvider
    {
        bool CanEdit { get; }
        PlayerModel Player { get; }
        PartyModel Party { get; }
        UnitEntityModel PlayerEntity { get; set; }
        IEnumerable<UnitEntityModel> Characters { get; }
    }
}