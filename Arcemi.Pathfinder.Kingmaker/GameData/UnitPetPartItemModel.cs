﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class UnitPetPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.UnitPartPet, Assembly-CSharp";
        public UnitPetPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public SingleReferenceModel MasterRef => A.Object("m_MasterRef", a => new SingleReferenceModel(a));
    }
}