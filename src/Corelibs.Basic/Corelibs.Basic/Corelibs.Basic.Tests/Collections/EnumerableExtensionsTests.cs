using Corelibs.Basic.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Common.Basic.Tests.Collections
{
    internal class EnumerableExtensionsTests
    {
        private static readonly Node[] Nodes = new Node[]
        {
            new Node()
            {
                Children = new List<Node>()
                {
                    new Node()
                    {

                    },

                    new Node()
                    {
                        Children = new List<Node>()
                        {
                            new Node()
                            {

                            },

                            new Node()
                            {

                            }
                        }
                    }
                }
            }
        };

        [Test]
        public void FlattenEnumerable_RecursionCount_ShouldBe_Fine()
        {
            var nodesFlattened = Nodes.Flatten(n => n.Children, out int recursionCount).ToArray();
            Assert.AreEqual(nodesFlattened.Count(), 5);
            Assert.AreEqual(recursionCount, 2);
        }

        [Test]
        public void FlattenEnumerable_RecursionCountLimit_Should_Work()
        {
            var nodesFlattened = Nodes.Flatten(n => n.Children, out int recursionCount, 1).ToArray();
            Assert.AreEqual(nodesFlattened.Count(), 3);
            Assert.AreEqual(recursionCount, 1);
        }

        internal class Node
        {
            public List<Node> Children { get; set; } = new List<Node>();
        }
    }
}