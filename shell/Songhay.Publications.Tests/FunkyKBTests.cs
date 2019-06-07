using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using SonghayCore.xUnit;
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

        [Theory, ProjectFileData(typeof(FunkyKBTests),
            "../../../../../_data/kb/entries/kb-19901868.json",
            "../../../../../posts/kb-19901868.md")]
        public void ShouldLoadEntry(FileInfo entryInfo, FileInfo mdInfo)
        {
            var jO = JObject.Parse(File.ReadAllText(entryInfo.FullName));
            var md = string.Empty;

            string ConvertToCamelCase(string input)
            { // TODO: move to Core
                return $"{input[0].ToString().ToLowerInvariant()}{input.Substring(1)}";
            }

            var documentJson = jO["Document"]?.Value<JObject>()
                .Properties()
                .Select(i => $"\"{ConvertToCamelCase(i.Name)}\": \"{i.Value}\"")
                .Aggregate((a, i) => $"{a},{i}");
            var document = JObject.Parse($"{{{documentJson}}}");

            this._testOutputHelper.WriteLine(document.ToString());

            // File.WriteAllText(mdInfo.FullName, md);
        }

        ITestOutputHelper _testOutputHelper;
    }
}