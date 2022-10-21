﻿#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{

    public class JsonPartSaveGameFile
    {
        private static readonly IEnumerable<SaveFileTypeMetadata> Metadatas =
            typeof(JsonPartSaveGameFile).Assembly.GetTypes()
            .Select(x => new SaveFileTypeMetadata(x))
            .Where(x => x.Attribute != null)
            .ToArray();

        private static readonly Dictionary<string, SaveFileTypeMetadata> TypeIdLookup = Metadatas
            .ToDictionary(x => x.Attribute.Id, StringComparer.Ordinal);

        private static readonly Dictionary<Type, SaveFileTypeMetadata> TypesLookup = Metadatas
            .ToDictionary(x => x.Type);

        private readonly string _path;
        private readonly JObject _json;
        private readonly IGameResourcesProvider _res;
        private readonly References _refs;
        private readonly Dictionary<string, List<Model>> _types;

        private Model _root;

        public JsonPartSaveGameFile(string path, JObject json, IGameResourcesProvider res)
        {
            _path = path;
            _json = json;
            _res = res;
            _refs = new References(res);
            _types = new Dictionary<string, List<Model>>(StringComparer.Ordinal);
            VisitTree(null, json);
        }

        public T GetRoot<T>(Func<ModelDataAccessor, T> factory = null)
            where T : Model
        {
            if (_root != null)
            {
                return (T)_root;
            }

            var accessor = new ModelDataAccessor(_json, _refs, _res);
            var root = (factory ?? Mappings.GetFactory<T>()).Invoke(accessor);
            _root = root;
            return root;
        }

        public IReadOnlyList<T> GetAllOf<T>()
        {
            if (!TypesLookup.TryGetValue(typeof(T), out var metadata))
            {
                throw new ArgumentException("Type is not registered");
            }
            if (!_types.TryGetValue(metadata.Attribute.Id, out var list)) return Array.Empty<T>();

            return list.Cast<T>().ToArray();
        }

        private void AddTypeRef(string typeId, JObject obj)
        {
            if (!TypeIdLookup.TryGetValue(typeId, out var metadata))
            {
                return;
            }

            if (!_types.TryGetValue(typeId, out var list))
            {
                list = new List<Model>();
                _types.Add(typeId, list);
            }

            var model = (Model)_refs.CreateObject(obj, metadata.CreateInstance);
            list.Add(model);
        }

        private void VisitTree(JToken parent, JToken node)
        {
            if (node is JArray arr)
            {
                for (var i = 0; i < arr.Count; i++)
                {
                    VisitTree(arr, arr[i]);
                }
                return;
            }

            if (node is JObject obj)
            {
                var type = obj.Property("$type");
                if (type != null && type.Value != null)
                {
                    var typeValue = type.Value.Value<string>();
                    AddTypeRef(typeValue, obj);
                }
                foreach (var property in obj.Properties())
                {
                    if (string.Equals(property.Name, "$id", StringComparison.Ordinal))
                    {
                        var id = property.Value.Value<string>();
                        _refs.Add(id, obj);
                    }
                    else if (string.Equals(property.Name, "$ref", StringComparison.Ordinal))
                    {
                        var id = property.Value.Value<string>();
                        _refs.ReferTo(parent, obj, id);
                    }

                    VisitTree(obj, property.Value);
                }
            }
        }

        public void Save()
        {
            using (var stream = new FileStream(_path, FileMode.Create, FileAccess.Write))
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var writer = new JsonTextWriter(streamWriter))
                    {
                        _json.WriteTo(writer);
                    }
                }
            }
        }
    }
}