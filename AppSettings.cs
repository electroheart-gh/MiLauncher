using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLauncher
{
    internal class AppSettings
    {
        //public List<NameModifier> NameModifiers { get; set; }
        public List<string> TargetFolders{ get; set; }

        //// Configuration class to modify display of lblTaskName
        //// If Pattern match, replace it with Substitution and set the Forecolor
        //public class NameModifier
        //{
        //    public string Pattern { get; set; }
        //    public string Substitution { get; set; }
        //    public string ForeColor { get; set; }

        //    public NameModifier()
        //    {
        //        Pattern = string.Empty;
        //        Substitution = string.Empty;
        //        ForeColor = string.Empty;
        //    }
        //}

        public AppSettings()
        {
            //NameModifiers = new List<NameModifier>();

            TargetFolders = new List<string>();
            // TODO: configuration for Keymap 
            // TODO: configuration for specific application to open file, such as sakura


        }
    }
}
