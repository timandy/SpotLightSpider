using System.IO;
using SpotLightSpider.Util;

namespace SpotLightSpider.Model
{
    public class Img
    {
        public const string TMP = ".tmp";

        public string u { get; set; }

        public string sha256 { get; set; }

        public string fileSize { get; set; }

        public string path { get; private set; }

        public string temp { get; private set; }

        public void SetPath(string download)
        {
            string trimHttp = this.u.Substring(this.u.IndexOf("//") + 2);
            string com = FilePathUtil.GetAbsolutePath(trimHttp.Substring(0, trimHttp.IndexOf(".com/") + 4).Replace("/", "@"), download);
            this.path = FilePathUtil.GetAbsolutePath(trimHttp.Substring(trimHttp.IndexOf(".com/") + 5).Replace("/", "-"), com);
            this.temp = this.path + TMP;
            if (!Directory.Exists(com))
                Directory.CreateDirectory(com);
        }
    }
}
