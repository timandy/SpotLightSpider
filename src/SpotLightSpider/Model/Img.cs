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

        private string _md5;
        public string md5 { get { return this._md5 ?? (this._md5 = Hasher.MD5(this.sha256)); } }

        public void SetPath(string download, string folder)
        {
            string sub = FilePathUtil.GetAbsolutePath(folder, download);
            this.path = FilePathUtil.GetAbsolutePath(this.md5 + this.u.Substring(this.u.LastIndexOf(".")), sub);
            this.temp = this.path + TMP;
            if (!Directory.Exists(sub))
                Directory.CreateDirectory(sub);
        }
    }
}
