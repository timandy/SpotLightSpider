using System.Collections.Generic;

namespace SpotLightSpider.Model
{
    public class Batchrsp
    {
        public string ver { get; set; }
        public List<Error> errors { get; set; }
        public List<Item> items { get; set; }

        public bool Success
        {
            get
            {
                return this.errors == null;
            }
        }
    }
}
