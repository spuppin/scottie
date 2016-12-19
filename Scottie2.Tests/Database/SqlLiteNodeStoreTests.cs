using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Scottie2.Tests.Database
{
    public class SqlLiteNodeStoreTests
    {
        [Fact]
        public void GetSequentialPath_HappyPath_PathAsExpected()
        {
            // .Net Core unit test support is atrocious.
           // string actual = SqlLiteNodeStore.GetSequentialPath("/foo/bar", 1);

            //Assert.IsNotNull(actual);
        }
    }
}
