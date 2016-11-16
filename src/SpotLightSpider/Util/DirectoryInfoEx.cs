using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SpotLightSpider.Util
{
    /// <summary>
    /// 文件系统扩展
    /// </summary>
    public static class DirectoryInfoEx
    {
        //遍历文件
        public static IEnumerable<FileInfo> VisitFiles(this DirectoryInfo dirInfo, Func<DirectoryInfo, bool> folderPredicate, Func<FileInfo, bool> filePredicate)
        {
            Debug.Assert(dirInfo != null);

            DirectoryInfo subDirInfo;
            FileInfo subFileInfo;
            foreach (FileSystemInfo subSysInfo in dirInfo.EnumerateFileSystemInfos())
            {
                if ((subDirInfo = subSysInfo as DirectoryInfo) != null)
                {
                    if (folderPredicate == null || folderPredicate(subDirInfo))
                        foreach (FileInfo subSubFileInfo in subDirInfo.VisitFiles(folderPredicate, filePredicate))
                            yield return subSubFileInfo;
                }
                else if ((subFileInfo = subSysInfo as FileInfo) != null)
                {
                    if (filePredicate == null || filePredicate(subFileInfo))
                        yield return subFileInfo;
                }
            }
        }
    }
}
