using System.Collections.Generic;

namespace SpotLightSpider.Model
{
    public class Result
    {
        public int pid { get; set; }

        public bool valid { get; set; }

        public List<Img> imgs { get; private set; }

        public int index = -1;

        public Result()
        {
            this.imgs = new List<Img>();
        }
    }
}
