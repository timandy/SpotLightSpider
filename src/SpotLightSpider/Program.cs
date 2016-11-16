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
            //准备
            Spider.DeleteLog();
            Spider.DeleteTemp();

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
