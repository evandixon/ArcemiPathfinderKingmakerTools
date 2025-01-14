﻿namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public interface IBlueprintMetadataEntry
    {
        string Id { get; }
        BlueprintName Name { get; }
        BlueprintType Type { get; }
        string DisplayName { get; }
        string Path { get; }
    }
}
