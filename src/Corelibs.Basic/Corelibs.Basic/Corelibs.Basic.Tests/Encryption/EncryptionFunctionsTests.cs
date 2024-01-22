using Corelibs.Basic.Encryption;
using NUnit.Framework;

namespace Corelibs.Basic.Tests.Encryption
{
    internal class EncryptionFunctionsTests
    {
        [Test]
        public void GenerateGuidHash()
        {
            var result = EncryptionFunctions.GenerateGuidHash("1", "2");
            Assert.AreEqual(result, "6e51ce52-dde2-4201-8032-eff584a7219b");
        }
    }
}
