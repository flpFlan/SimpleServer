using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleServer
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var f = new FolderBrowserDialog();
            f.Description = "选择一个文件夹作为网站根目录";
            if (f.ShowDialog() == DialogResult.OK && f.SelectedPath != "")
            {
                var path = f.SelectedPath;
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.Start();
                cmd.StandardInput.AutoFlush = true;
                cmd.StandardInput.WriteLine("ipconfig" + "&exit");
                var ipr = Regex.Match(cmd.StandardOutput.ReadToEnd(), @"(?<=IPv4 地址.+)\d+\.\d+\.\d+\.\d+");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"当前ip地址:{ipr.Value} 服务器端口号:2333\n可以使用 http://{ipr.Value}:2333 访问网站。(注意只能内网访问，就是连接了同一个网络的设备)");
                Console.ForegroundColor = ConsoleColor.Gray;
                cmd.Dispose();
                var file = File.Open("start.bat", FileMode.OpenOrCreate);
                using (var sw = new StreamWriter(file))
                {
                    var pyPath = Path.GetFullPath("python.exe");
                    if (path.StartsWith("D"))
                    {
                        sw.WriteLine($@"cd \d {path}");
                    }
                    else
                    {
                        sw.WriteLine($"cd {path}");
                    }
                    sw.WriteLine($"{pyPath} -m http.server --cgi 2333");
                }
                file.Dispose();
                Console.Write("\n使用当前目录下的start.bat启动服务器\n\n现在可以关闭该窗口");
                Console.ReadKey();
            }
        }
    }
}