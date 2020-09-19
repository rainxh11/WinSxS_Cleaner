using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Globalization;


namespace WinSxS_Cleaner
{
    class Program
    {
        private static void RetrieveFoldersPath(string dirToSearch, List<string> foldersToSearchFor, List<DirectoryInfo> folderToCopy)
        {
            DirectoryInfo dir = new DirectoryInfo(dirToSearch);
            try
            {
                Parallel.ForEach(dir.GetDirectories("*", SearchOption.TopDirectoryOnly), (DirectoryInfo folder) =>
                //foreach (DirectoryInfo folder in dir.GetDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    Console.WriteLine($"Processing {folder.Name} on thread {Thread.CurrentThread.ManagedThreadId}");
                    foreach (string filter in foldersToSearchFor)
                    {
                        if (folder.Name.Contains(filter))
                        {
                            folderToCopy.Add(folder);
                        }
                    }
                //}
                });
            }
            catch { }
        }
        /// ----------------------------------------------------------------------//
        static void CopyFolder(string destFolder, DirectoryInfo sourceFolder)
        {
            Process robocopyProcess = new Process();
            robocopyProcess.StartInfo.RedirectStandardOutput = true;
            robocopyProcess.StartInfo.CreateNoWindow = true;
            robocopyProcess.StartInfo.UseShellExecute = false;
            robocopyProcess.StartInfo.Arguments = $@"{sourceFolder.FullName} {destFolder}\{sourceFolder.Name} /E";
            robocopyProcess.StartInfo.FileName = "robocopy.exe";
            try
            {
                robocopyProcess.Start();
            }
            catch { }
        }
        /// ----------------------------------------------------------------------//
        static void Main(string[] args)
        {
            string currentDir = AppContext.BaseDirectory;
			string destFolder = currentDir + @"\Sakonito_WinSxS_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            List<string> foldersToSearchFor = new List<string>()
            {
                "amd64_microsoft-windows-s..cingstack.resources",
                "amd64_microsoft-windows-servicingstack",
                "amd64_microsoft.vc80",
                "amd64_microsoft.vc90",
                "amd64_microsoft.windows.c..-controls.resources",
                "amd64_microsoft.windows.c..-controls.resources",
                "amd64_microsoft.windows.common-controls",
                "amd64_microsoft.windows.common-controls",
                "amd64_microsoft.windows.gdiplus",
                "amd64_microsoft.windows.gdiplus",
                "Catalogs",
                "InstallTemp",
                "Manifests",
                "x86_microsoft.vc80",
                "x86_microsoft.vc90",
                "x86_microsoft.windows.c..-controls.resources",
                "x86_microsoft.windows.c..-controls.resources",
                "x86_microsoft.windows.common-controls",
                "x86_microsoft.windows.common-controls",
                "x86_microsoft.windows.gdiplus",
                "x86_microsoft.windows.gdiplus"
            };
            List<DirectoryInfo> foldersToCopy = new List<DirectoryInfo>();

            RetrieveFoldersPath(args[0], foldersToSearchFor, foldersToCopy);
			
			if (args.Length == 2)
			{
				destFolder = args[1];
			}
			
            if (!Directory.Exists(destFolder))
            {
				try
                {
                    Directory.CreateDirectory(destFolder);
                }
                catch {}
            }

            foreach(DirectoryInfo folder in foldersToCopy)
            {
                CopyFolder(destFolder, folder);
            }
        }
    }
}
