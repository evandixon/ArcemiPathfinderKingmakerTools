#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class ClassModel : RefModel
    {
        public ClassModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string TypeName => A.Res.GetClassTypeName(CharacterClass);
        public string ArchetypeName => A.Res.GetClassArchetypeName(Archetypes);

        public int Level { get => A.Value<int>(); set => A.Value(value); }
        public string CharacterClass { get => A.Value<string>(); set => A.Value(value); }
        public ListValueAccessor<string> Archetypes => A.ListValue<string>();

        public bool IsMythic => A.Res.IsMythicClass(CharacterClass);
        public bool IsMythicChampion => string.Equals(CharacterClass, "247aa787806d5da4f89cfc3dff0b217f", StringComparison.Ordinal);


        public static void Prepare(IReferences refs, JObject obj)
        {
        }
    }
}