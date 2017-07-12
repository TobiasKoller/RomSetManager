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
        public BehaviourType Behaviour { get; set; }
        public int Position { get; set; }

        public NamePart(string name, string value, string system, string type, string description, int position, string behaviour)
        {
            NamePartType namePartType;
            BehaviourType behaviourType;

            if (!Enum.TryParse(type, true, out namePartType))
                namePartType = NamePartType.Property;

            if (!Enum.TryParse(behaviour, true, out behaviourType))
                behaviourType = BehaviourType.DontCare;
            
            SetValue(name, value, system, namePartType, description, position, behaviourType);
        }

        public NamePart(string name, string value, string system, NamePartType type, string description, int position, BehaviourType behaviour)
        {
           SetValue(name,value,system,type,description, position, behaviour);
        }

        public NamePart()
        {
            
        }

        private void SetValue(string name, string value, string system, NamePartType type, string description, int position,BehaviourType behaviour)
        {
            Name = name;
            Value = value;
            System = system;
            Type = type;
            Description = description;
            Position = position;
            Behaviour = behaviour;
        }
    }
}