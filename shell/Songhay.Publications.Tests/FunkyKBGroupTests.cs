using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Songhay.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Publications.Tests
{
    public class FunkyKBGroupTests
    {
        public FunkyKBGroupTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Theory, InlineData("../../../../../_data/kb/group")]
        public void ShouldSortGroupsByInceptDate(string groupRoot)
        {
            groupRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, groupRoot);
            this._testOutputHelper.WriteLine($"loading entries from `{groupRoot}`");

            var groupRootInfo = new DirectoryInfo(groupRoot);
            foreach (var groupInfo in groupRootInfo.GetFiles("*.json"))
            {
                var jO = JObject.Parse(File.ReadAllText(groupInfo.FullName));
                var jA = jO["Documents"]?.Value<JArray>();

                Assert.NotNull(jA);
                Assert.True(jA.Any());
                jO["Documents"] = JArray.FromObject(jA.OrderByDescending(i => i["CreateDate"].Value<DateTime>()));

                File.WriteAllText(groupInfo.FullName, jO.ToString());
            }
        }

        [Theory, ProjectFileData(typeof(FunkyKBEntryTests), "../../../../../_data/kb/index.json")]
        public void ShouldSortGroups(FileInfo indexInfo)
        {
            var jO = JObject.Parse(File.ReadAllText(indexInfo.FullName));
            var jA = jO["ChildSegments"]?.Value<JArray>();

            Assert.NotNull(jA);
            Assert.True(jA.Any());
            jO["ChildSegments"] = JArray.FromObject(jA.OrderBy(i => i["SegmentName"].Value<string>()));

            File.WriteAllText(indexInfo.FullName, jO.ToString());
        }

        ITestOutputHelper _testOutputHelper;
    }
}