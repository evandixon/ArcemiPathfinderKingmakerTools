using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Arcemi.Pathfinder.Kingmaker.Infrastructure.Extensions;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class FeatureFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Feature, Assembly-CSharp";
        public FeatureFactItemModel(ModelDataAccessor accessor) : base(accessor) { }

        public override string DisplayName
        {
            get
            {
                if (Param != null)
                {
                    var displayName = base.DisplayName;
                    if (Param.Any())
                    {
                        displayName += $"({string.Join(", ", Param.Values)})";
                    }
                    return displayName;
                }

                return base.DisplayName;
            }
        }

        public bool IsRanked => RankToSource?.Count > 0;
        public int Rank { get => A.Value<int>() > 0 ? A.Value<int>() : IsRanked ? 1 : 0; set => A.Value(value); }
        public int SourceLevel { get => A.Value<int>(); set => A.Value(value); }
        public string Source { get => A.Value<string>(); set => A.Value(value); }
        public bool IgnorePrerequisites { get => A.Value<bool>(); set => A.Value(value); }
        public ListAccessor<FeatureRankToSourceModel> RankToSource => A.List("m_RankToSource", a => new FeatureRankToSourceModel(a));
        public DictionaryOfValueAccessor<string> Param => A.DictionaryOfValue<string>();

        public void SetParameter(string parameterType, string parameterValue)
        {
            if (Param == null)
            {
                A.UnderlyingObject.Add(nameof(Param), JToken.FromObject(new { }));
            }

            Param[parameterType] = parameterValue;
        }

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            obj.Add(nameof(RankToSource), new JArray());
            FactItemModel.Prepare(refs, obj);
        }

        public override string Export()
        {
            return A.ExportCode(o =>
            {
                o.RemovePath("m_Context", "m_OwnerRef");
                o.Remove("Source");
                o.Remove("SourceLevel");
                o.Remove("UniqueId");
                o.Remove("AttachTime");
                o.Remove("IgnorePrerequisites");
                o.Remove("IsActive");
            });
        }
    }
}