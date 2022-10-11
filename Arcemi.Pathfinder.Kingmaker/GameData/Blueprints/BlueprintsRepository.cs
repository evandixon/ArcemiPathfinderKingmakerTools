using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
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

        public async Task<Blueprint> GetBlueprint(string blueprintId)
        {
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
                    try
                    {
                        entryStream = entry.Open();

                    }
                    catch (InvalidDataException)
                    {
                        // WOTR ships with a slightly corrupt zip file that can still be read if we try hard enough
                        // Seems to be a result of missing the zip64 header that specifies compressed size, while having a zip32 compressed size indicating to use zip64

                        // ZipArchiveEntry actually contains the very method we need, but it's private
                        // OpenInReadMode(bool checkOpenable)
                        // https://github.com/dotnet/runtime/blob/2d8f10528f91461344ede350f6a37283c87d581d/src/libraries/System.IO.Compression/src/System/IO/Compression/ZipArchiveEntry.cs#L681
                        entryStream = (Stream)entry
                            .GetType()
                            .GetMethod("OpenInReadMode", BindingFlags.NonPublic | BindingFlags.Instance, new[] { typeof(bool) })
                            .Invoke(entry, new object[] { false /* checkOpenable */ });
                    }

                    using var streamReader = new StreamReader(entryStream);
                    blueprintContent = await streamReader.ReadToEndAsync();
                }
                finally
                {
                    entryStream?.Dispose();
                }

                if (entry.CompressedLength == uint.MaxValue)
                {
                    entry.GetType().GetField("_compressedSize", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(entry, 0);
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
            return new Blueprint(accessor);
        }

        public void Dispose()
        {
            this.zipArchive?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
