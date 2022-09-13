using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class ProgressionItemModel : Model
    {
        public ProgressionItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        /// <summary>
        /// Blueprint ID of the progression
        /// </summary>
        public string Key { get => A.Value<string>(); set => A.Value(value); }

        public ProgressionItemValueModel Value => A.Object(factory: a => new ProgressionItemValueModel(a));

        public static void Prepare(IReferences refs, JObject obj)
        {
        }
    }
}