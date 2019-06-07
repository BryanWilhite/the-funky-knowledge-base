using Newtonsoft.Json.Linq;
using SonghayCore.xUnit;
using System;
using System.IO;
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

            File.WriteAllText(mdInfo.FullName, md);
        }

        ITestOutputHelper _testOutputHelper;
    }
}
