using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using SonghayCore.xUnit;
using System;
using System.IO;
using System.Linq;
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

        [Theory, InlineData("../../../../../_data/kb/entries/")]
        public void ShouldLoadEntries(string root)
        {
            root = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly);
            this._testOutputHelper.WriteLine(root);
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