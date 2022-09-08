﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class FactItemModel : RefModel, ITypedModel
    {
        public FactItemModel(ModelDataAccessor accessor) : base(accessor) { }
        public virtual string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
        public TimeSpan AttachTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public bool IsActive { get => A.Value<bool>(); set => A.Value(value); }
        public FactContextModel Context { get => A.Object("m_Context", a => new FactContextModel(a)); set => A.Value(value.GetAccessor().UnderlyingObject.Root, "m_Context"); }
        public DictionaryAccessor<ComponentModel> Components => A.Dictionary(factory: ComponentModel.Factory, createIfNull: true);
        public ParentContextModel ParentContext => A.Object(factory: a => new ParentContextModel(a), createIfNull: true);

        public static FactItemModel Factory(ModelDataAccessor accessor)
        {
            var type = accessor.TypeValue();
            if (string.Equals(type, FeatureFactItemModel.TypeRef, StringComparison.Ordinal))
            {
                return new FeatureFactItemModel(accessor);
            }
            if (string.Equals(type, EnchantmentFactItemModel.TypeRef, StringComparison.Ordinal))
            {
                return new EnchantmentFactItemModel(accessor);
            }
            if (string.Equals(type, QuestFactItemModel.TypeRef, StringComparison.Ordinal))
            {
                return new QuestFactItemModel(accessor);
            }
            if (string.Equals(type, BuffFactItemModel.TypeRef, StringComparison.Ordinal))
            {
                return new BuffFactItemModel(accessor);
            }
            if (string.Equals(type, AbilityFactItemModel.TypeRef, StringComparison.Ordinal))
            {
                return new AbilityFactItemModel(accessor);
            }
            if (string.Equals(type, ActivatableAbilityFactItemModel.TypeRef, StringComparison.Ordinal))
            {
                return new ActivatableAbilityFactItemModel(accessor);
            }
            if (string.Equals(type, EtudeFactItemModel.TypeRef, StringComparison.Ordinal))
            {
                return new EtudeFactItemModel(accessor);
            }
            return new FactItemModel(accessor);
        }

        public static Action<IReferences, JObject> GetPreparation<T>()
        {
            var type = typeof(T);
            if (type == typeof(FeatureFactItemModel))
            {
                return FeatureFactItemModel.Prepare;
            }
            if (type == typeof(EnchantmentFactItemModel))
            {
                return EnchantmentFactItemModel.Prepare;
            }
            if (type == typeof(QuestFactItemModel))
            {
                return QuestFactItemModel.Prepare;
            }
            if (type == typeof(BuffFactItemModel))
            {
                return BuffFactItemModel.Prepare;
            }
            if (type == typeof(AbilityFactItemModel))
            {
                return AbilityFactItemModel.Prepare;
            }
            if (type == typeof(EtudeFactItemModel))
            {
                return EtudeFactItemModel.Prepare;
            }
            return Prepare;
        }

        protected static void Prepare(IReferences refs, JObject obj)
        {
            obj.Add(nameof(AttachTime), AttachTimes.GameStart.ToString());
            obj.Add(nameof(UniqueId), Guid.NewGuid().ToString());
            obj.Add(nameof(IsActive), true);
            obj.Add(nameof(Components), new JObject());
        }

        public virtual string Export()
        {
            return A.ExportCode();
        }

        public void Import(string code)
        {
            A.ImportCode(code);
        }

        public void Import(FactItemModel obj)
        {
            A.ImportCode(obj.Export());
        }
    }
}