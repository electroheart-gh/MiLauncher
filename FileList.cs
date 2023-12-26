using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLauncher
{
    internal class FileList
    {
        //public List<FileInfo> items;
        public List<FileListInfo> items = new List<FileListInfo>();


        public FileList()
        {
            //
            // for pre-release
            //
            string folderPath = @"C:\Users\JUNJI\Desktop\tools"; // フォルダのパスを指定
            string[] files = Directory.GetFiles(folderPath);

            // ファイル名をコンソールに出力する例
            foreach (string file in files)
            {
                // Console.WriteLine(Path.GetFileName(file));
                items.Add(new FileListInfo(Path.GetFileName(file), file, 0));
            }
            //
            //
            //
        }


        public class FileListInfo
        {
            public string FileName { get; set; }
            public string FullPathName { get; set; }
            public int Priority { get; set; }

            public FileListInfo(string fileName, string pathName, int priority)
            {
                this.FileName = fileName;
                this.FullPathName = pathName;
                this.Priority = priority;
            }

        }
    }
}
