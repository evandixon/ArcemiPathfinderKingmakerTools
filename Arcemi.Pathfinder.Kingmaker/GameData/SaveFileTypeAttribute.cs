using System;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SaveFileTypeAttribute : Attribute
    {
        public SaveFileTypeAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
