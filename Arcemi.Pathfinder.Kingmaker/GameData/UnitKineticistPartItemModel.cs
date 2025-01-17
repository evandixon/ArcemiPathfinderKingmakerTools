﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class UnitKineticistPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Class.Kineticist.UnitPartKineticist, Assembly-CSharp";
        public UnitKineticistPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int AcceptedBurn { get => A.Value<int>(); set => A.Value(value); }
    }
}