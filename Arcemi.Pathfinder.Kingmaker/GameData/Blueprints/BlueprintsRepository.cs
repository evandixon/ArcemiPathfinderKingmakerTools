using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintsRepository : IDisposable
    {
        public BlueprintsRepository(string filename, IReferences refs, IGameResourcesProvider res)
        {
            this.refs = refs ?? throw new ArgumentNullException(nameof(refs));
            this.res = res ?? throw new ArgumentNullException(nameof(res));
            
            this.zipArchive = new ZipArchive(File.OpenRead(filename), ZipArchiveMode.Read, leaveOpen: false);
        }

        private readonly IReferences refs;
        private readonly IGameResourcesProvider res;
        private readonly ZipArchive zipArchive;
        private readonly SemaphoreSlim zipSemaphore = new(1, 1);

        private readonly ConcurrentDictionary<string, Blueprint> blueprintCache = new();

        public async Task<Blueprint> GetBlueprint(string blueprintId)
        {
            if (blueprintCache.TryGetValue(blueprintId, out var cachedBlueprint))
            {
                return cachedBlueprint;
            }

            var filePath = res.Blueprints.Get(blueprintId)?.Path?.Replace(@"\", "/")?.Replace("Blueprints/", "");
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            await zipSemaphore.WaitAsync();

            string blueprintContent = null;
            try
            {
                var entry = zipArchive.GetEntry(filePath);
                if (entry == null)
                {
                    return null;
                }

                Stream entryStream = null;
                try
                {
                    // WOTR ships with a slightly corrupt zip file that can still be read if we try hard enough
                    // Seems to be a result of missing the zip64 header that specifies compressed size, while having a zip32 compressed size indicating to use zip64

                    // Some entries have a compressed size that's too small
                    // .Net uses this as an upper bound for the data it can read, and increasing it lets us read up to the actual end of the file,
                    // but it also uses it as validation to prevent reading a file that extends beyond the end of the file
                    entry.GetType()
                        .GetField("_compressedSize", BindingFlags.NonPublic | BindingFlags.Instance)
                        .SetValue(entry, uint.MaxValue);

                    // ZipArchiveEntry.Open checks _compressedSize to ensure it doesn't extend beyond the end of the zip file.
                    // Some entries are like this already, others are only like this because we increased it to fix a different issue.
                    // ZipArchiveEntry contains a different method that lets us bypass errors related to size, but it's private.
                    // ZipArchiveEntry.OpenInReadMode(bool checkOpenable). https://github.com/dotnet/runtime/blob/2d8f10528f91461344ede350f6a37283c87d581d/src/libraries/System.IO.Compression/src/System/IO/Compression/ZipArchiveEntry.cs#L681
                    entryStream = (Stream)entry
                        .GetType()
                        .GetMethod("OpenInReadMode", BindingFlags.NonPublic | BindingFlags.Instance, new[] { typeof(bool) })
                        .Invoke(entry, new object[] { false /* checkOpenable */ });

                    using var streamReader = new StreamReader(entryStream);
                    blueprintContent = await streamReader.ReadToEndAsync();
                }
                finally
                {
                    entryStream?.Dispose();
                }
            }
            finally
            {
                zipSemaphore.Release();
            }

            if (string.IsNullOrEmpty(blueprintContent))
            {
                return null;
            }

            var accessor = new ModelDataAccessor(JObject.Parse(blueprintContent), refs, res);
            var blueprint = new Blueprint(accessor);
            blueprintCache[blueprintId] = blueprint;
            return blueprint;
        }

        public void Dispose()
        {
            this.zipArchive?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
