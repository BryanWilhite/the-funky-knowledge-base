using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using Songhay.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Publications.Tests
{
    public class FunkyKBTests
    {
        public FunkyKBTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Theory, InlineData("../../../../../_data/kb/entries/", "../../../../../posts/")]
        public void ShouldLoadEntries(string entriesRoot, string postsRoot)
        {
            entriesRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, entriesRoot);
            this._testOutputHelper.WriteLine($"loading entries from `{entriesRoot}`");

            postsRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, postsRoot);
            this._testOutputHelper.WriteLine($"writing entries to `{postsRoot}`");

            var entriesRootInfo = new DirectoryInfo(entriesRoot);
            foreach (var entryInfo in entriesRootInfo.GetFiles("*.json"))
            {
                this._testOutputHelper.WriteLine($"reading {entryInfo.Name}...");
                var jO = JObject.Parse(File.ReadAllText(entryInfo.FullName));

                var md = GetMarkdown(jO);

                var mdPath = FrameworkFileUtility.GetCombinedPath(postsRoot, entryInfo.Name.Replace(".json", ".md"));

                this._testOutputHelper.WriteLine($"writing `{mdPath}`...");
                File.WriteAllText(mdPath, md);
            }
        }

        [Theory, ProjectFileData(typeof(FunkyKBTests),
            "../../../../../_data/kb/entries/kb-19901868.json",
            "../../../../../posts/kb-19901868.md")]
        public void ShouldLoadEntry(FileInfo entryInfo, FileInfo mdInfo)
        {
            var jO = JObject.Parse(File.ReadAllText(entryInfo.FullName));

            var md = GetMarkdown(jO);

            this._testOutputHelper.WriteLine($"writing {entryInfo.Name}...");
            File.WriteAllText(mdInfo.FullName, md);
        }

        public static string GetMarkdown(JObject input)
        {
            var md = string.Empty;

            string ConvertToCamelCase(string s)
            { // TODO: move to Core
                return $"{s[0].ToString().ToLowerInvariant()}{s.Substring(1)}";
            }

            var documentJson = input["Document"]?.Value<JObject>()
                .Properties()
                .Select(i => $"\"{ConvertToCamelCase(i.Name)}\": \"{i.Value}\"")
                .Aggregate((a, i) => $"{a},{i}");
            var document = JObject.Parse($"{{{documentJson}}}");
            var frontMatter = $"---json{Environment.NewLine}{document}{Environment.NewLine}---";
            var content = input["Content"]?.Value<string>();

            md = $"{frontMatter}{Environment.NewLine}{Environment.NewLine}{content}{Environment.NewLine}";

            return md;
        }

        ITestOutputHelper _testOutputHelper;
    }
}