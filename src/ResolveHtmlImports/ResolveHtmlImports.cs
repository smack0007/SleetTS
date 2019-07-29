using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace ChocolateTS
{
    public class ResolveHtmlImports : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string MainPath { get; set; }

        [Required]
        public string SourceDirectory { get; set; }

        [Output]
        public string Output { get; set;}

        public override bool Execute()
        {
            LogMessage($"{nameof(MainPath)} = {MainPath}");
            LogMessage($"{nameof(SourceDirectory)} = {SourceDirectory}");            

            Output = LoadHtml(MainPath);

            return true;
        }

        private void LogMessage(string message)
        {
            Log.LogMessage(MessageImportance.Normal, message);
        }

        private string LoadHtml(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var fullPath = System.IO.Path.Combine(SourceDirectory, path);
            LogMessage($"Loading \"{fullPath}\"");
            var html = File.ReadAllText(fullPath);

            html = Regex.Replace(html, @"\<import(\s+)src\=\""([\w, \/, \\, \.]+)\""\>(.*)\<\/import\>", (match) => {
                return LoadHtml(Path.Combine(directory, match.Groups[2].Value));
            });

            return html;
        }
    }
}