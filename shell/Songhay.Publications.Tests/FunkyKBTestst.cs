using SonghayCore.xUnit;
using System;
using System.IO;
using Xunit;

namespace Songhay.Publications.Tests
{
    public class FunkyKBTests
    {
        [Theory, ProjectFileData("../../../../../_data/kb/entries/kb-19901868.json")]
        public void ShouldLoadEntry(FileInfo entryInfo)
        {
        }
    }
}
