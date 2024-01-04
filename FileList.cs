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
        // JsonSerializer requires properties instead of fields
        //public List<FileInfo> items;
        private List<FileListInfo> items = new List<FileListInfo>();
        public List<FileListInfo> Items { get => items; set => items = value; }

        // Simple form {get; set; } leaves Items Null
        //public List<FileListInfo> Items { get; set; }



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
                Items.Add(new FileListInfo(Path.GetFileName(file), file, 0));
            }
            //
            //
            //
            //SettingManager.LoadSettings<FileList>(fileLdp)

            // TODO: Update FileList in async as per config
            // TODO: Implement some ways to update FileList manually
        }

        //public FileList(string dataPath)
        //{
        //    SettingManager.LoadSettings<FileList>(dataPath);
        //}



        public class FileListInfo
        {
            public string FileName { get; set; }
            public string FullPathName { get; set; }
            public int Priority { get; set; }

            public FileListInfo()
            {
                FileName = "";
                FullPathName = "";
                Priority = 0;
            }
            public FileListInfo(string fileName, string pathName, int priority)
            {
                FileName = fileName;
                FullPathName = pathName;
                Priority = priority;
            }
        }
    }
}
