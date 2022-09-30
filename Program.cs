using System.Text.Json;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers.Zip;

var json = File.ReadAllText("whitelist.json");
var whiteList = JsonSerializer.Deserialize<List<string>>(json);

var winSxsPath = args.Length > 0
    ? new DirectoryInfo(args.First())
    : new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "WinSxS"));

var foldersToCopy = whiteList?
    .SelectMany(pattern => winSxsPath.EnumerateDirectories($"*{pattern}*", SearchOption.TopDirectoryOnly))
    .DistinctBy(folder => folder.FullName);

var filesToCopy = foldersToCopy?
    .SelectMany(folder => folder.EnumerateFiles("*.*", SearchOption.AllDirectories))
    .DistinctBy(file => file.FullName)
    .ToList();


using var zipFile = ZipArchive.Create();
foreach (var folder in filesToCopy)
{
    zipFile.AddEntry(folder.FullName.Replace(winSxsPath.FullName, ""), folder.FullName);
}

zipFile.SaveTo($"WinSxS_{filesToCopy?.Count}-files_{DateTime.Now:yyyy-MM-dd}.zip", CompressionType.Deflate);