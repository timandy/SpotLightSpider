using System.IO;
using System.Threading;
using SpotLightSpider.Model;
using SpotLightSpider.Util;

namespace SpotLightSpider
{
    //下载
    class Program
    {
        static void Main(string[] args)
        {
            //删除日志
            try { File.Delete(Spider.LogPath); }
            catch { }

            //下载 
            while (true)
            {
                foreach (int pid in Spider.Interfaces)
                {
                    Spider.Download(new Result { pid = pid });
                    Thread.Sleep(500);
                }
            }
        }
    }
}
