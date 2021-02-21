using System;
using Xunit;
using Moq;

namespace BackendClassUnitTests
{
    public class TestBase
    {
        //create Mock of Database manager
        private Mock<DatabaseManager> mockDb = new Mock<DatabaseManager>();

        [TestInitialize]
        public void TestInitialize()
        {

        }
    }
}
