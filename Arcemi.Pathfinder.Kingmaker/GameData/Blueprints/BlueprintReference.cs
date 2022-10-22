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

        public Blueprint Dereference(IBlueprintsRepository blueprintsRepository)
        {
            return blueprintsRepository.GetBlueprint(Id);
        }

        public async Task<Blueprint> DereferenceAsync(IBlueprintsRepository blueprintsRepository)
        {
            return await blueprintsRepository.GetBlueprintAsync(Id);
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

        public new Blueprint<TBlueprintData> Dereference(IBlueprintsRepository blueprintsRepository)
        {
            return blueprintsRepository.GetBlueprint<TBlueprintData>(Id);
        }

        public new async Task<Blueprint<TBlueprintData>> DereferenceAsync(IBlueprintsRepository blueprintsRepository)
        {
            return await blueprintsRepository.GetBlueprintAsync<TBlueprintData>(Id);
        }

        public static implicit operator string(BlueprintReference<TBlueprintData> blueprintIdReference)
        {
            if (blueprintIdReference == null)
            {
                return null;
            }

            return blueprintIdReference.Id;
        }

        public static implicit operator BlueprintReference<TBlueprintData>(string blueprintId)
        {
            if (string.IsNullOrEmpty(blueprintId))
            {
                return null;
            }

            return new BlueprintReference<TBlueprintData>(blueprintId);
        }
    }
}
