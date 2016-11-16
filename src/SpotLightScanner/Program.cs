using System;
using System.IO;
using SpotLightSpider.Model;
using SpotLightSpider.Util;

namespace SpotLightScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            //删除日志
            try { File.Delete(Spider.LogPath); }
            catch { }

            //扫描
            for (int i = 200000; i <= 300000; i++)
                Spider.ScanInterface(new Result { pid = i });
            Console.ReadLine();
        }
    }
}
