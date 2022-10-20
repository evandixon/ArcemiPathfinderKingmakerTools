using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintReference
    {
        public BlueprintReference(string id)
        {
            this.id = id;
        }

        private readonly string id;

        public string Id => id?.Replace("!bp_", "");

        public bool IsBlueprintLoaded { get; private set; }

        public Blueprint Blueprint
        {
            get
            {
                if (!IsBlueprintLoaded)
                {
                    throw new InvalidOperationException();
                }
                return _blueprint;
            }
            private set => _blueprint = value;
        }
        private Blueprint _blueprint;
        
        public async Task LoadBlueprint(IBlueprintsRepository blueprints)
        {
            if (string.IsNullOrEmpty(this.Id))
            {
                return;
            }

            Blueprint = await blueprints.GetBlueprint(this.Id);
            if (_blueprint == null)
            {
                throw new Exception($"Failed to load blueprint {this.Id}");
            }
            IsBlueprintLoaded = true;
        }

        protected Blueprint<TBlueprintData> GetBlueprint<TBlueprintData>() where TBlueprintData : BlueprintData
        {
            return new Blueprint<TBlueprintData>(Blueprint);
        }

        public static implicit operator string(BlueprintReference blueprintIdReference)
        {
            if (blueprintIdReference == null)
            {
                return null;
            }

            return blueprintIdReference.Id;
        }

        public static implicit operator BlueprintReference(string blueprintId)
        {
            if (string.IsNullOrEmpty(blueprintId))
            {
                return null;
            }

            return new BlueprintReference(blueprintId);
        }

        public static implicit operator JToken(BlueprintReference blueprintIdReference)
        {
            return (JToken)blueprintIdReference.Id;
        }
    }

    public class BlueprintReference<TBlueprintData> : BlueprintReference where TBlueprintData : BlueprintData
    {
        public BlueprintReference(string id) : base(id)
        {
        }

        public new Blueprint<TBlueprintData> Blueprint => GetBlueprint<TBlueprintData>();
    }
}
