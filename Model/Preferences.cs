using System.Collections.Generic;

namespace Model
{
    public class Preferences
    {
        public List<NamePart> NameParts { get; set; }

        public Preferences()
        {
            NameParts = new List<NamePart>();
        }
    }
}
