﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class ArmorComponentModel : RefModel
    {
        public ArmorComponentModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Blueprint { get => A.Value<string>("m_Blueprint"); set => A.Value(value, "m_Blueprint"); }
        public bool IsIdentified { get => A.Value<bool>(); set => A.Value(value); }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }

        public PartsContainerModel Parts => A.Object(factory: a => new PartsContainerModel(a));
        public FactsContainerModel Facts => A.Object(factory: a => new FactsContainerModel(a));
    }
}