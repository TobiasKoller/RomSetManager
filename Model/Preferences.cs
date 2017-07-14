using System.Collections.Generic;

namespace Model
{
    public class Preferences
    {
        public bool IgnoreMustHaveForOneRom { get; set; }
        public bool IgnoreNeverUseForOneRom { get; set; }
        public List<NamePart> NameParts { get; set; }

        public Preferences()
        {
            NameParts = new List<NamePart>();
        }
    }
}
