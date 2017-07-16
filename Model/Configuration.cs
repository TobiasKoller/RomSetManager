using System.Collections.Generic;

namespace Model
{
    public class Configuration
    {
        public BestMatch BestMatch { get; set; }
        public List<System> Systems { get; set; }

        public Configuration()
        {
            Systems = new List<System>();
        }

    }
}