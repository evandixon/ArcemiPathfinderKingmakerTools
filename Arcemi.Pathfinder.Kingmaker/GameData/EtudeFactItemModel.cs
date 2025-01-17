﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class EtudeFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.AreaLogic.Etudes.Etude, Assembly-CSharp";
        public EtudeFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public bool IsCompleted { get => A.Value<bool>(); set => A.Value(value); }
        public bool CompletionInProgress { get => A.Value<bool>(); set => A.Value(value); }

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            obj.Add(nameof(IsCompleted), false);
            obj.Add(nameof(CompletionInProgress), false);
            FactItemModel.Prepare(refs, obj);
            obj[nameof(IsActive)] = false;
        }
    }
}