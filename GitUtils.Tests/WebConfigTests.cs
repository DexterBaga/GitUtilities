using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GitUtils.Tests
{
    [TestClass]
    public class WebConfigTests
    {
        [TestMethod]
        public void ResetCredentialsForFile()
        {
            var configFile = @".\Resources\web.config";
            var updatedConnectionStrings = WebConfigUtilities.ResetCredentials(configFile,"FakeTestUserName","FakeTestPassword");
            var updatedConfig = File.ReadAllText(configFile);
            ApprovalTests.Approvals.Verify(updatedConfig);
            var actualCount = updatedConnectionStrings.Count;
            var expectedCount = 2;
            Assert.IsTrue(expectedCount == actualCount,$"Expected count {expectedCount} but received {actualCount}");
        }
        [TestMethod]
        public void ResetCredentialsForFileByConnectionStringName()
        {
            var configFile = @".\Resources\web.config";
            WebConfigUtilities.ResetCredentials(@".\Resources\web.config", "BigAppConnectionString", "RealTestUserName", "RealTestPassword");
            var updatedConfig = File.ReadAllText(configFile);
            ApprovalTests.Approvals.Verify(updatedConfig);
        }
    }
}
