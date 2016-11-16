using System;
using SpotLightSpider.Model;
using SpotLightSpider.Util;

namespace SpotLightScanner
{
    //扫描
    class Program
    {
        static void Main(string[] args)
        {
            //准备
            Spider.DeleteLog();
            Spider.DeleteTemp();

            //扫描
            for (int i = 200000; i <= 300000; i++)
                Spider.ScanInterface(new Result { pid = i });
            Console.ReadLine();
        }
    }
}
