using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class SingleReferenceModel : Model
    {
        public SingleReferenceModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Ref => A.Value<string>("m_Ref");
    }
}