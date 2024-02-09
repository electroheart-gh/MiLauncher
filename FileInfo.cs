using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLauncher
{
    internal class FileInfo
    {
        public string FullPathName { get; }
        public string FileName { get; }

        public FileInfo()
        {
        }
        public FileInfo(string pathName)
        {
            FullPathName = pathName;
            FileName = Path.GetFileName(pathName);
        }
    }

}
