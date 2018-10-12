using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalDAGSearcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations.Tests
{
    [TestClass]
    public class PatternTests
    {
        static Pattern<char> _pattern;
        [TestInitialize]
        public void InitializeTest()
        {
            HashSet<char>[] inputData = new HashSet<char>[]
            {
                null,
                new HashSet<char>(){default(char)},
                null,
                new HashSet<char>(){'f', 'b'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'r', '\0'},
                null,
                null
            };
            _pattern = new Pattern<char>(inputData);
        }

        [TestMethod]
        public void IsIndexForcedEmptyTests()
        {
            Assert.IsTrue(_pattern.IsIndexForcedEmpty(1));
            Assert.IsFalse(_pattern.IsIndexForcedEmpty(0));
        }

        [TestMethod]
        public void IsIndexPotentiallyEmptyTests()
        {
            Assert.IsTrue(_pattern.IsIndexPotentiallyEmpty(5));
            Assert.IsTrue(_pattern.IsIndexPotentiallyEmpty(6));
            Assert.IsFalse(_pattern.IsIndexPotentiallyEmpty(4));
            Assert.IsFalse(_pattern.IsIndexPotentiallyEmpty(3));

        }
        [TestMethod]
        public void IsIndexConcreteTests()
        {
            Assert.IsTrue(_pattern.IsIndexConcreteValue(4));
            Assert.IsFalse(_pattern.IsIndexConcreteValue(3));
            Assert.IsFalse(_pattern.IsIndexConcreteValue(2));
        }

        [TestMethod]
        public void Concrete_ZeroIndex_MaximalPrefixTest()
        {
            //var charPat = new char[] { default(char), default(char), 'f', 'o', 'o', default(char), default(char) };
            HashSet<char>[] inputData = new HashSet<char>[]
            {
                new HashSet<char>(){'t' },
                new HashSet<char>(){default(char)},
                null,
                new HashSet<char>(){'f', 'b'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'r', '\0'},
                null,
                null
            };
            Pattern<char> pattern = new Pattern<char>(inputData);
            Assert.IsTrue(pattern.IsPrefixMaximalAtIndex(0));
        }

        [TestMethod]
        public void NonConcrete_ZeroIndex_MaximalPrefixTest()
        {
            HashSet<char>[] inputData = new HashSet<char>[]
{
                new HashSet<char>(){'t', 's'},
                new HashSet<char>(){default(char)},
                null,
                new HashSet<char>(){'f', 'b'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'r', '\0'},
                null,
                null
};
            Pattern<char> pattern = new Pattern<char>(inputData);
            Assert.IsTrue(pattern.IsPrefixMaximalAtIndex(0));
        }

        [TestMethod]
        public void Empty_ZeroIndex_MaximalPrefixTest()
        {
            HashSet<char>[] inputData = new HashSet<char>[]
{
                new HashSet<char>(){'\0' },
                new HashSet<char>(){default(char)},
                null,
                new HashSet<char>(){'f', 'b'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'r', '\0'},
                null,
                null
};
            Pattern<char> pattern = new Pattern<char>(inputData);
            Assert.IsFalse(pattern.IsPrefixMaximalAtIndex(0)); //is invalid place to start
        }

        [TestMethod]
        public void EmptyPreceeding_MaximalPrefixTest()
        {
            HashSet<char>[] inputData = new HashSet<char>[]
{
                new HashSet<char>(){'\0' },
                new HashSet<char>(){default(char)},
                null,
                new HashSet<char>(){'f', 'b'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'r', '\0'},
                null,
                null
            };
            Pattern<char> pattern = new Pattern<char>(inputData);
            Assert.IsTrue(pattern.IsPrefixMaximalAtIndex(1));
        }


        [TestMethod]
        public void ConcretePreceding_MaximalPrefixTest()
        {
            HashSet<char>[] inputData = new HashSet<char>[]
            {
                new HashSet<char>(){'t' },
                new HashSet<char>(){default(char)},
                null,
                new HashSet<char>(){'f', 'b'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'r', '\0'},
                null,
                null
            };
            Pattern<char> pattern = new Pattern<char>(inputData);
            Assert.IsTrue(pattern.IsPrefixMaximalAtIndex(1));
        }

        [TestMethod]
        public void NonConcretePreceeding_MaximalPrefixTest()
        {
            HashSet<char>[] inputData = new HashSet<char>[]
            {
                new HashSet<char>(){'t', 's'},
                new HashSet<char>(){default(char)},
                null,
                new HashSet<char>(){'f', 'b'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'r', '\0'},
                null,
                null
            };
            Pattern<char> pattern = new Pattern<char>(inputData);
            Assert.IsTrue(pattern.IsPrefixMaximalAtIndex(1));
        }

        [TestMethod]
        public void PossiblyEmptyPreceeding_MaximalPrefixTest()
        {
            HashSet<char>[] inputData = new HashSet<char>[]
{
                new HashSet<char>(){'t', 's', '\0'},
                new HashSet<char>(){default(char)},
                null,
                new HashSet<char>(){'f', 'b'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'r', '\0'},
                null,
                null
};
            Pattern<char> pattern = new Pattern<char>(inputData);
            Assert.IsFalse(pattern.IsPrefixMaximalAtIndex(1));
        }

        [TestMethod]
        public void FullConcrete_PatternBoundaryTest()
        {
            HashSet<char>[] concrete = new HashSet<char>[]
            {
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'}
            };
            var ConcretePattern = new Pattern<char>(concrete);
            Assert.IsTrue(ConcretePattern.SequenceBoundaries.Count == 1);
            CollectionAssert.AreEquivalent(ConcretePattern.SequenceBoundaries[0], new int[] { 0, 6 });
        }
        [TestMethod]
        public void Concrete_And_SinglePossiblyEmpty_PatternBoundaryTest()
        {
            HashSet<char>[] concrete = new HashSet<char>[]
{
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o', '\0'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'}
};
            var ConcretePattern = new Pattern<char>(concrete);
            Assert.IsTrue(ConcretePattern.SequenceBoundaries.Count == 2);
            CollectionAssert.AreEquivalent(ConcretePattern.SequenceBoundaries[0], new int[] { 0, 2 });
            CollectionAssert.AreEquivalent(ConcretePattern.SequenceBoundaries[1], new int[] { 4, 6 });
        }
        [TestMethod]
        public void SinglePossiblyEmptyPattern_BoundaryTest()
        {
            HashSet<char>[] concrete = new HashSet<char>[]
            {
                null,
                null,
                null,
                new HashSet<char>(){'o', '\0'},
                null,
                null,
                null
            };
            var ConcretePattern = new Pattern<char>(concrete);
            Assert.IsTrue(ConcretePattern.SequenceBoundaries.Count == 1);
            CollectionAssert.AreEquivalent(ConcretePattern.SequenceBoundaries[0], new int[] { 3, 3 });
        }

        [TestMethod]
        public void PossiblyEmptyPattern_BoundaryTest()
        {
            HashSet<char>[] concrete = new HashSet<char>[]
            {
                new HashSet<char>(){'o', '\0'},
                new HashSet<char>(){'o', '\0'},
                new HashSet<char>(){'o', '\0'},
                new HashSet<char>(){'o', '\0'},
                new HashSet<char>(){'o', '\0'},
                new HashSet<char>(){'o', '\0'},
                new HashSet<char>(){'o', '\0'},
            };
            var ConcretePattern = new Pattern<char>(concrete);
            Assert.IsTrue(ConcretePattern.SequenceBoundaries.Count == 6);
            //CollectionAssert.AreEquivalent(ConcretePattern.SequenceBoundaries[0], new int[] { 3, 3 });
        }

        [TestMethod]
        public void PossiblyEmptyAndConcrete_BoundaryTest()
        {
            HashSet<char>[] concrete = new HashSet<char>[]
            {
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o', '\0'},
                new HashSet<char>(){'o', '\0'},
                new HashSet<char>(){'o', '\0'},
                new HashSet<char>(){'o'},
                new HashSet<char>(){'o'},
            };
            var ConcretePattern = new Pattern<char>(concrete);
            Assert.IsTrue(ConcretePattern.SequenceBoundaries.Count == 3);
            //CollectionAssert.AreEquivalent(ConcretePattern.SequenceBoundaries[0], new int[] { 3, 3 });
        }
    }
}
