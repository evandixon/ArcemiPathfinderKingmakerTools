using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class VendorTableModel : RefModel
    {
        public VendorTableModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Table);
        public string Table { get => A.Value<string>(); set => A.Value(value); }
        public ListAccessor<VendorTableEntryModel> Entries => A.List(factory: a => new VendorTableEntryModel(a));
        public ListValueAccessor<string> Loot => A.ListValue<string>();
        public ListAccessor<KeyValuePairModel<int>> KnownItems => A.List(factory: a => new KeyValuePairModel<int>(a));

        public bool HasItem(string blueprint)
        {
            return Entries.Any(e => string.Equals(e.Item, blueprint, StringComparison.Ordinal));
        }
    }
}