using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SpotLightSpider.Model;

namespace SpotLightSpider.Util
{
    public class Spider
    {
        private const string URL = "http://arc.msn.com/v3/Delivery/Cache?fmt=json&rafb=0&ctry=CN&lc=zh-Hans-CN&pid=";
        private static object m_Lock = new object();
        private static string m_InterfacesPath = FilePathUtil.GetAbsolutePath("interfaces.txt");
        private static string m_LogPath = FilePathUtil.GetAbsolutePath("error.log");
        private static string m_DownloadPath = FilePathUtil.GetAbsolutePath("download");
        private static SortedSet<int> m_Interfaces;
        public static SortedSet<int> Interfaces
        {
            get
            {
                lock (m_Lock)
                {
                    if (m_Interfaces == null)
                    {
                        if (!File.Exists(m_InterfacesPath))
                            File.Create(m_InterfacesPath);

                        string text = File.ReadAllText(m_InterfacesPath, Encoding.UTF8);
                        int temp;
                        int[] pids = text.Split(new char[] { '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(a => int.TryParse(a, out temp)).Select(a => int.Parse(a)).ToArray();
                        SortedSet<int> set = new SortedSet<int>();
                        foreach (int pid in pids)
                            set.Add(pid);
                        m_Interfaces = set;
                    }
                    return m_Interfaces;
                }
            }
        }

        //删除日志
        public static void DeleteLog()
        {
            try
            {
                File.Delete(m_LogPath);
            }
            catch
            {
            }
        }

        //删除缓存
        public static void DeleteTemp()
        {
            DirectoryInfo download = new DirectoryInfo(m_DownloadPath);
            if (!download.Exists)
                return;

            var files = download.VisitFiles(null, file => file.Name.EndsWith(Img.TMP, StringComparison.OrdinalIgnoreCase));
            foreach (FileInfo file in files)
            {
                try { file.Delete(); }
                catch { }
            }
        }

        //扫描服务
        public static void ScanInterface(Result result)
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                //下载字符串完成
                client.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e) =>
                {
                    if (e.Error != null)
                    {
                        WriteError(result, "访问服务失败:" + e.Error);
                        return;
                    }
                    //
                    ArcResponse response;
                    string errorResponse;
                    if (!JsonSerializer.Deserialize<ArcResponse>(e.Result, out response, out errorResponse))
                    {
                        WriteError(result, "反序列化响应数据失败:" + errorResponse);
                        return;
                    }
                    if (response == null)
                    {
                        WriteError(result, "反序列化结果为 null");
                        return;
                    }

                    if (result.pid % 1000 == 0)
                        WriteInfo(result, "扫描中");

                    //
                    result.valid = response.batchrsp.Success;
                    if (!result.valid)
                    {
                        return;
                    }

                    //收集
                    foreach (Item item in response.batchrsp.items)
                    {
                        ItemReal itemReal;
                        string errorItem;
                        if (!JsonSerializer.Deserialize<ItemReal>(item.item, out itemReal, out errorItem))
                        {
                            WriteError(result, "反序列化 Item 失败:" + errorItem);
                            continue;
                        }
                        //
                        Img landscape = itemReal.ad.image_fullscreen_001_landscape;
                        Img portrait = itemReal.ad.image_fullscreen_001_portrait;
                        if (landscape != null || portrait != null)
                        {
                            lock (m_Lock)
                            {
                                client.Dispose();
                                Interfaces.Add(result.pid);
                                WriteInfo(result, "扫描到接口");
                                File.WriteAllText(m_InterfacesPath, string.Join("\r\n", Interfaces), Encoding.UTF8);
                                return;
                            }
                        }
                    }
                    client.Dispose();
                };
                //开始请求
                client.DownloadStringAsync(new Uri(URL + result.pid));
            }
            catch (Exception exp)
            {
                WriteError(result, exp);
            }
        }

        //下载
        public static void Download(Result result)
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                //下载字符串完成
                client.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e) =>
                {
                    if (e.Error != null)
                    {
                        WriteError(result, "访问服务失败:" + e.Error);
                        return;
                    }
                    //
                    ArcResponse response;
                    string errorResponse;
                    if (!JsonSerializer.Deserialize<ArcResponse>(e.Result, out response, out errorResponse))
                    {
                        WriteError(result, "反序列化响应数据失败:" + errorResponse);
                        return;
                    }
                    if (response == null)
                    {
                        WriteError(result, "反序列化结果为 null");
                        return;
                    }
                    //
                    result.valid = response.batchrsp.Success;
                    if (!result.valid)
                    {
                        WriteInfo(result, "无效 pid");
                        return;
                    }

                    //收集
                    foreach (Item item in response.batchrsp.items)
                    {
                        ItemReal itemReal;
                        string errorItem;
                        if (!JsonSerializer.Deserialize<ItemReal>(item.item, out itemReal, out errorItem))
                        {
                            WriteError(result, "反序列化 Item 失败:" + errorItem);
                            continue;
                        }
                        //
                        Img landscape = itemReal.ad.image_fullscreen_001_landscape;
                        string folder = null;
                        if (landscape != null)
                        {
                            folder += landscape.md5;
                            result.imgs.Add(landscape);
                        }
                        folder += "-";
                        //
                        Img portrait = itemReal.ad.image_fullscreen_001_portrait;
                        if (portrait != null)
                        {

                            folder += portrait.md5;
                            result.imgs.Add(portrait);
                            portrait.SetPath(m_DownloadPath, folder);
                        }
                        //
                        if (landscape != null)
                            landscape.SetPath(m_DownloadPath, folder);
                    }
                    if (result.imgs.Count <= 0)
                        return;

                    //下载一个
                    DownloadNext(client, result);
                };
                //下载文件完成
                client.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                {
                    if (e.Error != null)
                    {
                        WriteError(result, "下载图片失败:" + e.Error);
                        return;
                    }
                    //
                    Img current = result.imgs[result.index];
                    if (!File.Exists(current.temp))
                    {
                        WriteError(result, "下载 " + current.u + " 失败");
                        return;
                    }
                    if (new FileInfo(current.temp).Length.ToString() != current.fileSize)
                    {
                        WriteError(result, "校验 " + current.u + " 失败");
                        return;
                    }
                    try
                    {
                        File.Move(current.temp, current.path);
                    }
                    catch
                    {
                        WriteError(result, "重命名 " + current.u + " 失败");
                        return;
                    }
                    WriteInfo(result, "已下载 " + current.u);

                    //下载一个
                    DownloadNext(client, result);
                };
                //开始请求
                client.DownloadStringAsync(new Uri(URL + result.pid));
            }
            catch (Exception exp)
            {
                WriteError(result, exp);
            }
        }

        //下载下一个
        public static void DownloadNext(WebClient client, Result result)
        {
            while (true)
            {
                //下载完
                if (++result.index >= result.imgs.Count)
                {
                    client.Dispose();
                    return;
                }

                //当前
                Img current = result.imgs[result.index];
                if (File.Exists(current.path) && new FileInfo(current.path).Length.ToString() == current.fileSize)
                    continue;

                //缓存
                if (File.Exists(current.temp))
                    continue;

                //下载
                client.DownloadFileAsync(new Uri(current.u), current.temp);
                return;
            }
        }

        //写信息日志
        public static void WriteInfo(Result result, object msg)
        {
            Console.WriteLine(string.Format("{0}\r\n{1}", result.pid, msg));
        }

        //写错误日志
        public static void WriteError(Result result, object msg)
        {
            lock (m_Lock)
            {
                Console.WriteLine(string.Format("{0}\r\n{1}", result.pid, msg));
                File.AppendAllText(FilePathUtil.GetAbsolutePath("error.log"), string.Format("{0}\r\n{1}\r\n", result.pid, msg), Encoding.UTF8);
            }
        }
    }
}
