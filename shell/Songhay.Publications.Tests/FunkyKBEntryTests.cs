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
    public class FunkyKBEntryTests
    {
        public FunkyKBEntryTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Theory, InlineData("../../../../../entry/")]
        public void ShouldEditEntryDates(string entryRoot)
        {
            entryRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, entryRoot);
            this._testOutputHelper.WriteLine($"loading entries from `{entryRoot}`");

            var entriesRootInfo = new DirectoryInfo(entryRoot);
            foreach (var entryInfo in entriesRootInfo.GetFiles("*.md"))
            {
                this._testOutputHelper.WriteLine($"reading `{entryInfo.Name}`...");

                var lines = File.ReadLines(entryInfo.FullName);

                var mdFrontMatterLines = lines.TakeWhile(i => !i.EqualsInvariant("---"));

                var frontMatterJson = mdFrontMatterLines
                    .Where(i => !i.Contains("---json"))
                    .Aggregate((a, i) => $"{a}{i}")
                    .Trim();

                var jO = JObject.Parse(frontMatterJson);

                string getUtcDate(string propertyName)
                {
                    var date = jO[propertyName]?.Value<DateTime>();
                    Assert.NotNull(date);

                    var utcDate = date.Value
                        .ToUniversalTime()
                        .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                    return utcDate;
                }

                new [] { "date", "modificationDate" }.ForEachInEnumerable(i => jO[i] = getUtcDate(i));

                frontMatterJson = jO.ToString();

                var frontMatter = $@"---json{Environment.NewLine}{frontMatterJson}{Environment.NewLine}---";

                this._testOutputHelper.WriteLine($"{nameof(frontMatter)}:{Environment.NewLine}{frontMatter}");

                var md = $"{frontMatter}{Environment.NewLine}{lines.SkipWhile(i => !i.EqualsInvariant("---")).Skip(1).Aggregate((a, i) => $"{a}{Environment.NewLine}{i}")}";

                File.WriteAllText(entryInfo.FullName, md);
            }
        }

        [Theory, ProjectFileData(typeof(FunkyKBEntryTests), "../../../../../entry/kb904850341.md")]
        public void ShouldLoadEntry(FileInfo mdInfo)
        {
            var mdFrontMatterLines = File
                .ReadLines(mdInfo.FullName)
                .TakeWhile(i => !i.EqualsInvariant("---"));

            var frontMatterJson = mdFrontMatterLines
                .Where(i => !i.Contains("---json"))
                .Aggregate((a, i) => $"{a}{i}")
                .Trim();

            Assert.False(string.IsNullOrEmpty(frontMatterJson), "The expected Front Matter JSON is not here.");

            this._testOutputHelper.WriteLine(frontMatterJson);
        }

        ITestOutputHelper _testOutputHelper;
    }
}