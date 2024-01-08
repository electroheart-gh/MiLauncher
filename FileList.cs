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
        // TODO: Update FileList in async as per config
        // TODO: Implement some ways to update FileList manually

        // Variables and Properties
        // JsonSerializer requires properties instead of fields
        private List<FileListInfo> items = new List<FileListInfo>();
        public List<FileListInfo> Items { get => items; set => items = value; }

        public FileList()
        {
            //
            // for pre-release
            //
            string folderPath = @"C:\Users\JUNJI\Desktop\tools"; // フォルダのパスを指定
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                // Console.WriteLine(Path.GetFileName(file));
                Items.Add(new FileListInfo(Path.GetFileName(file), file, 0));
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

        //
        // Method for test to create sample File List
        //
        public static FileList FileListForTest()
        {
            FileList fileList = new FileList();
            //string folderPath = @"C:\Users\JUNJI\Desktop\tools";
            string folderPath = @"C:\Users\JUNJI\Desktop\";
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                // Console.WriteLine(Path.GetFileName(file));
                fileList.Items.Add(new FileListInfo(Path.GetFileName(file), file, 0));
            }
            return fileList;
        }
    }
}
