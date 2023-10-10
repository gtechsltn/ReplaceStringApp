using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ReplaceStringApp
{
    internal class Program
    {
        /// <summary>
        /// https://localhost/3s/default.aspx
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            var hasError = false;
            var fileInfos = new DirectoryInfo(@"D:\D2\lite_portal").GetFiles("*.js", SearchOption.AllDirectories);
            var sb = new StringBuilder();
            var sbSuccess = new StringBuilder();
            var lst = new List<string>();
            foreach (var fileInfo in fileInfos)
            {
                var fullName = fileInfo.FullName;
                if (!fullName.Contains(@"\.")
                 && !fullName.Contains(@"-cli-")
                 && !fullName.Contains(@"\.history\")
                 && !fullName.Contains(@"\bower_components\")
                 && !fullName.Contains(@"\node_modules\")
                 && !fullName.Contains(@"\dist\")
                 && !fullName.Contains(@"\dists\")
                 && !fullName.Contains(@"\test\")
                 && !fullName.Contains(@"\tests\")
                 && !fullName.Contains(@"\vendor\")
                )
                {
                    sb.AppendLine(fullName);
                    lst.Add(fullName);
                }
            }
            var allText = sb.ToString();
            File.WriteAllText("Output.txt", allText, Encoding.UTF8);
            Process.Start("Output.txt");

            var dict = new Dictionary<int, string>();
            var arrUnique = new HashSet<string>();
            for (int i = 1; i <= 5; i++)
            {
                var cfgKey = $"StringReplace{i}1";
                var oldValue = ConfigurationManager.AppSettings[cfgKey];
                arrUnique.Add(oldValue);
                dict.Add(i, oldValue);
            }

            sb.Length = 0;
            foreach (var filePath in lst)
            {
                try
                {
                    var fileContent = File.ReadAllText(filePath, Encoding.UTF8);
                    foreach (var hs in arrUnique)
                    {
                        if (fileContent.Contains(hs) && dict.ContainsValue(hs))
                        {
                            var key = dict.FirstOrDefault(x => x.Value == hs).Key;
                            var cfgKey = $"StringReplace{key}2";
                            var newValue = ConfigurationManager.AppSettings[cfgKey];
                            if (!string.IsNullOrWhiteSpace(newValue) && !newValue.Equals(hs))
                            {
                                var str = fileContent.Replace(hs, newValue);
                                File.WriteAllText(filePath, str, Encoding.UTF8);
                                sbSuccess.AppendLine(filePath);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    hasError = true;
                    sb.AppendLine(ex.ToString());
                    //throw;
                }
            }
            if (hasError && sb.Length > 0)
            {
                File.WriteAllText("Error.txt", sb.ToString(), Encoding.UTF8);
                Process.Start("Error.txt");
            }

            if (sbSuccess.Length > 0)
            {
                File.WriteAllText("Success.txt", sbSuccess.ToString(), Encoding.UTF8);
                Process.Start("Success.txt");
            }
        }
    }
}