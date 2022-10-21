﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class KeyValuePairObjectModel<TModel> : Model
        where TModel : Model
    {
        private readonly Func<ModelDataAccessor, TModel> _factory;

        public KeyValuePairObjectModel(ModelDataAccessor accessor, Func<ModelDataAccessor, TModel> factory) : base(accessor)
        {
            _factory = factory;
        }

        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Key);
        public string Key { get => A.Value<string>(); set => A.Value(value); }
        public TModel Value { get => A.Object(factory: _factory); }
    }
}