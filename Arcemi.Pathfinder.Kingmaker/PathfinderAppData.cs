#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion


using Arcemi.Pathfinder.Kingmaker.GameData;

namespace Arcemi.Pathfinder.Kingmaker
{
    public interface IPathfinderAppData
    {
        string Directory { get; }
        Portraits Portraits { get; }
        string PortraitsDirectory { get; }
        string SavedGamesDirectory { get; }
    }

    public class PathfinderAppData : IPathfinderAppData
    {
        private readonly ResourceProvider resourceProvider;

        public string Directory => resourceProvider.Directory;
        public string PortraitsDirectory => resourceProvider.PortraitsDirectory;
        public string SavedGamesDirectory => resourceProvider.SavedGamesDirectory;
        public Portraits Portraits { get; }

        public PathfinderAppData(ResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;
            Portraits = new Portraits(resourceProvider);
        }
    }
}