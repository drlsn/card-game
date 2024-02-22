using Corelibs.Basic.Collections;

namespace Corelibs.Basic.Tests.Collections;

public class EnumerableExtensionsTests
{
    [Test]
    public void IndexOf()
    {
        List<int> list = [3, 4, 6];

        var index = list.IndexOf(i => i == 4);

        Assert.That(index, Is.EqualTo(1));
    }
}
