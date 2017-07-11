using System;
using Model.Constants;

namespace Model
{
    public class NamePart
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string System { get; set; }
        public NamePartType Type { get; set; }
        public IncludeType Include { get; set; }
        public int Position { get; set; }

        public NamePart(string name, string value, string system, string type, string description, int position, string include)
        {
            NamePartType namePartType;
            IncludeType includeType;

            if (!Enum.TryParse(type, true, out namePartType))
                namePartType = NamePartType.Property;

            if (!Enum.TryParse(include, true, out includeType))
                includeType = IncludeType.Yes;
            
            SetValue(name, value, system, namePartType, description, position, includeType);
        }

        public NamePart(string name, string value, string system, NamePartType type, string description, int position, IncludeType include)
        {
           SetValue(name,value,system,type,description, position, include);
        }

        public NamePart()
        {
            
        }

        private void SetValue(string name, string value, string system, NamePartType type, string description, int position,IncludeType include)
        {
            Name = name;
            Value = value;
            System = system;
            Type = type;
            Description = description;
            Position = position;
            Include = include;
        }
    }
}