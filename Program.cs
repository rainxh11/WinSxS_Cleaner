using System.Text.Json;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

var json = File.ReadAllText("whitelist.json");
var whiteList = JsonSerializer.Deserialize<List<string>>(json);

var winSxsPath = args.Length > 0
    ? new DirectoryInfo(args[0])
    : new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "WinSxS"));

var filesToCopy = whiteList?
    .SelectMany(pattern => winSxsPath.EnumerateDirectories($"*{pattern}*", SearchOption.TopDirectoryOnly))
    .DistinctBy(folder => folder.FullName)
    .SelectMany(folder => folder.EnumerateFiles("*.*", SearchOption.AllDirectories))
    .DistinctBy(file => file.FullName)
    .OrderBy(file => file.Extension)
    .ThenBy(file => file.Length)
    .ToList();

using var zipFile = ZipArchive.Create();
foreach (var file in filesToCopy)
{
    Console.WriteLine($"Adding File: {file.FullName}");
    zipFile.AddEntry(file.FullName.Replace(winSxsPath.FullName, ""), file.FullName);
}

zipFile.SaveTo($"WinSxS_{filesToCopy?.Count}-files_{DateTime.Now:yyyy-MM-dd}.zip", CompressionType.Deflate);