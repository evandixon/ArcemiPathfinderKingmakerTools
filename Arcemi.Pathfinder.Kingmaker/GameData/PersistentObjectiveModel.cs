﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class PersistentObjectiveModel : RefModel
    {
        public PersistentObjectiveModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string State { get => A.Value<string>("m_State"); set => A.Value(value, "m_State"); }
        public bool IsVisible { get => A.Value<bool>("m_IsVisible"); set => A.Value(value, "m_IsVisible"); }
        public bool IsCollapse { get => A.Value<bool>("m_IsCollapse"); set => A.Value(value, "m_IsCollapse"); }
        public bool NeedToAttention { get => A.Value<bool>("m_NeedToAttention"); set => A.Value(value, "m_NeedToAttention"); }
        public bool IsActive { get => A.Value<bool>(); set => A.Value(value); }
        public TimeSpan ObjectiveStartTime { get => A.Value<TimeSpan>("m_ObjectiveStartTime"); set => A.Value(value, "m_ObjectiveStartTime"); }
        public int Order { get => A.Value<int>(); set => A.Value(value); }
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
        public TimeSpan AttachTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
    }
}