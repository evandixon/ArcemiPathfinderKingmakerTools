﻿using Arcemi.Pathfinder.Kingmaker.GameData;
using Arcemi.Pathfinder.Kingmaker.Infrastructure.Extensions;
using System.Collections.Generic;
using Xunit;

namespace Arcemi.Pathfinder.Tests
{
    public class StringExtensionsTests
    {
        public static IEnumerable<object[]> AsDisplayableData()
        {
            yield return new object[] { "ShortswordPlus4Item", "Shortsword +4" };
            yield return new object[] { "ShieldItemPlus1", "Shield +1" };
        }

        [Theory]
        [MemberData(nameof(AsDisplayableData))]
        public void AsDisplayable(string value, string expected)
        {
            var actual = value.AsDisplayable();
            Assert.Equal(expected, actual);
        }
    }
}
