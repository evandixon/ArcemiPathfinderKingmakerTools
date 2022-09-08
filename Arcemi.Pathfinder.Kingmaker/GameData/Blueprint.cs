﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    /// <summary>
    /// A model for a JBP blueprint file 
    /// </summary>
    public class Blueprint : Model
    {
        public Blueprint(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string AssetId { get => A.Value<string>("AssetId"); set => A.Value(value); }

        public BlueprintData Data => A.Object(factory: BlueprintData.Factory);

    }
}
