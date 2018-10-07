using IMinimalDAGInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalDAGImplementations.Serializer;
//using MinimalDAGImplementations.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MinimalDAGSearcher.Interfaces;

namespace MinimalDAGSearcher.Tests
{
    [TestClass()]
    public class MinimalDAGSearcherTests
    {
        static string _testDataDirectoryPath;
        static IMinimalDAG<char> _dawg;
        static IMinimalDAG<int?> _nullIntDAG;
        //todo: considerably more testing.
        //static IMinimalDAG<DateTime> _dateTimeDAG;
        //static IMinimalDAG<char> _turkishDAWG;
        //static IMinimalDAG<Coord> _coordDAG;
        static IMinimalDAGSearcher<char> _searcher;


        [ClassInitialize]
        public static void InitializeTest(TestContext testContext)
        {
            _testDataDirectoryPath = @"C:\Users\joelc\source\repos\ScrabbleSolver\TestData";
            _dawg = MinimalDAGSerializer.ReadCompressed<char>(Path.Combine(_testDataDirectoryPath, "ScrabbleJsonSerializedGZIP.gz"));
            _nullIntDAG = MinimalDAGSerializer.ReadCompressed<int?>(Path.Combine(_testDataDirectoryPath, "NullIntJsonSerializedGZIP.gz"));
            _searcher = new MinimalDAGSearcher<char>(_dawg);
        }

        [TestMethod()]
        public void FindMatchingNullIntSequencesTest()
        {
            MinimalDAGSearcher<int?> DagSearcher = new MinimalDAGSearcher<int?>(_nullIntDAG);
            List<int?> ValuePool = new List<int?>() { 1, 2, 9, 8 };
            int?[] ExistingValues = new int?[] { -1, 5, 1, -1, -1 };


            var matches = DagSearcher.FindMatchingSequences(ValuePool, new Pattern<int?>(ExistingValues, -1),
                                                            0).Select(x => x.MatchingSequence.ToList()).ToList();

            var ExpectedMatches = new List<List<int?>>()
            {
                new List<int?>()
                {
                    5,
                    1,
                    9,
                    8
                },
                new List<int?>()
                {
                    5,
                    1,
                    2
                }
            };

            NestedSequenceCompare(ExpectedMatches, matches);

        }
        [TestMethod]
        public void FindMatchingEnglishWordsTest()
        {
            MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
            List<char> CharPool = new List<char>();
            var existing = new char[] { default(char), default(char), default(char), 'i', 'z', 'z', 'a', 'r', default(char), default(char), default(char) };
            var ExpectedMatches = new List<string>()
            {
                "blizzard",
                "blizzardly",
                "blizzards",
                "blizzardy",
                "gizzard",
                "gizzards",
                "izzard",
                "izzards",
            };

            var Actual = minimalDAGSearcher.FindMatchingSequences(CharPool, new Pattern<char>(existing, default(char)),
                                                                   6).Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void FindMatchingEnglishWordsOptionTest()
        {
            MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
            List<char> CharPool = new List<char>();
            //var existing = new char[] { default(char), default(char), default(char), 'i', 'z', 'z', 'a', 'r', default(char), default(char), default(char) };
            var existing = new HashSet<char>[]{
                null,
                null,
                new HashSet<char>(){'l', 'g', default(char)},
                new HashSet<char>(){'i'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'a'},
                new HashSet<char>(){'r'},
                null,
                null,
                null
            };
            var ExpectedMatches = new List<string>()
            {
                "blizzard",
                "blizzardly",
                "blizzards",
                "blizzardy",
                "gizzard",
                "gizzards",
                "izzard",
                "izzards",
            };

            var Actual = minimalDAGSearcher.FindMatchingSequences(CharPool, new Pattern<char>(existing, default(char)),
                                                                                6).Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void EnglishWordRestrictedOptionTest()
        {
            MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
            List<char> CharPool = new List<char>();
            //var existing = new char[] { default(char), default(char), default(char), 'i', 'z', 'z', 'a', 'r', default(char), default(char), default(char) };
            var existing = new HashSet<char>[]{
                null,
                null,
                new HashSet<char>(){'l', 'g'},
                new HashSet<char>(){'i'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'a'},
                new HashSet<char>(){'r'},
                null,
                null,
                null
            };
            var ExpectedMatches = new List<string>()
            {
                "blizzard",
                "blizzardly",
                "blizzards",
                "blizzardy",
                "gizzard",
                "gizzards",
            };

            var Actual = minimalDAGSearcher.FindMatchingSequences(CharPool, new Pattern<char>(existing, default(char)),
                                                                                            6).Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void SecondRestrictedOptionEnglishTest()
        {
            MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
            List<char> CharPool = new List<char>();
            //var existing = new char[] { default(char), default(char), default(char), 'i', 'z', 'z', 'a', 'r', default(char), default(char), default(char) };
            var existing = new HashSet<char>[]{
                null,
                null,
                new HashSet<char>(){'l', default(char)},
                new HashSet<char>(){'i'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'a'},
                new HashSet<char>(){'r'},
                null,
                null,
                null
            };
            var ExpectedMatches = new List<string>()
            {
                "blizzard",
                "blizzardly",
                "blizzards",
                "blizzardy",
                "gizzard",
                "gizzards",
                "izzard",
                "izzards",
            };

            var Actual = minimalDAGSearcher.FindMatchingSequences(CharPool, new Pattern<char>(existing, default(char)),
                                                                                              6).Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        }

        [TestMethod]
        public void SimpleBenchmarkTest()
        {
            MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
            List<char> CharPool = new List<char>();

            var existing = new char[]
            {
                default(char),default(char),default(char),default(char),'e',default(char),default(char),
                default(char),default(char),default(char),default(char),default(char),default(char),default(char),
                default(char)
            };

            var Actual = minimalDAGSearcher.FindMatchingSequences(CharPool, new Pattern<char>(existing, default(char)), 14).Select(x => string.Concat(x.MatchingSequence)).ToList();
            Assert.IsTrue(Actual.Count > 0);
        }

        //[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        //public void SimpleBenchMarkTEst()
        //{

        //}

        [TestMethod]
        public void ContainingIndexTest()
        {
            MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
            List<char> CharPool = new List<char>() { 'a', 's', 'd', 'o' }; /*{ '*', '*', '*', '*', '*', '*' };*/
            var existing = new char[] { default(char), default(char), default(char), 'i', 'z', 'z', 'a', 'r', default(char), default(char), 'd', default(char) };
            var ExpectedMatches = new List<string>()
            {
                "do",
                "ado",
                "ad",
                "ads",
                "add",
                "odd",
                "od",
                "ods",
                "oda"


            };

            var contians = _dawg.Contains("ado");
            List<string> Actual = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                Actual = minimalDAGSearcher.FindMatchingSequencesContainingIndex(CharPool, new Pattern<char>(existing, default(char)),
                                                                                    0, 10).Select(x => string.Concat(x.MatchingSequence)).ToList();
            }


            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        }

        [TestMethod]
        public void ContainingPossibleIndexTest()
        {
            MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
            List<char> CharPool = new List<char>();
            //var existing = new char[] { default(char), default(char), default(char), 'i', 'z', 'z', 'a', 'r', default(char), default(char), default(char) };
            var existing = new HashSet<char>[]{
                null,
                null,
                new HashSet<char>(){'l', 'g'},
                new HashSet<char>(){'i'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'a'},
                new HashSet<char>(){'r'},
                null,
                null,
                null
            };
            var ExpectedMatches = new List<string>()
            {
                "blizzard",
                "blizzardly",
                "blizzards",
                "blizzardy",
                "gizzard",
                "gizzards",
            };

            var Actual = minimalDAGSearcher.FindMatchingSequencesContainingIndex(CharPool, new Pattern<char>(existing, default(char)),
                                                                                            6, 2).Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        }

        [TestMethod]
        public void FailedTest()
        {
            HashSet<char>[] Pattern = new HashSet<char>[15];
            Pattern[6] = new HashSet<char>() { '\0', 'a', 'i' };
            Pattern[7] = new HashSet<char>() { 'r' };
            Pattern[8] = new HashSet<char>() { '\0' };
            var Pool = new List<char>() { 'b', 'i', 'd', 'a', 'r', 'x' };
            Pattern<char> newPattern = new Pattern<char>(Pattern, default(char));
            var matches = _searcher.FindMatchingSequencesContainingIndex(Pool, newPattern, 0, 7).ToList();

            Assert.IsTrue(matches.Count > 0);

            matches = _searcher.FindMatchingSequences(Pool, newPattern, 0).ToList();
            Assert.IsTrue(matches.Count > 0);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CornerCase()
        {
            List<char> pool = new List<char>() { 'b', 'i', 'd', 'a', 'r', 'x' };
            var results = _searcher.FindMatchingSequences(pool, new Pattern<char>("\0\0\0\0\0\0\0doctor\0\0".ToArray(), default(char)),
                                                            1).Select(x => string.Concat(x.MatchingSequence)).ToList();

            Assert.IsTrue(results.Count() > 0);
        }

        //[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        //public void CornerCaseReverse()
        //{
        //    List<char> pool = new List<char>() { 'b', 'i', 'd', 'a', 'r', 'x' };
        //    //MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
        //    var results = _reverseSearcher.FindMatchingSequences(pool, "\0\0\0\0\0\0\0doctor\0\0".ToArray(), default(char), 1).Select(x => string.Concat(x.MatchingSequence)).ToList();

        //    Assert.IsTrue(results.Count() > 0);
        //}

        [TestMethod]
        public void ContainsBenchmark()
        {
            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(_searcher.Contains("pie"));
                Assert.IsFalse(_searcher.Contains("pxe"));
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void SingleLetterSearchBenchMark()
        {
            List<char> pool = new List<char>();
            var searchspace = "p\0e".ToArray();
            //var Pattern = ;
            for (int i = 0; i < 1000; i++)
            {
                var list = _searcher.FindMatchingSequences(pool, new Pattern<char>(searchspace, default(char)), 1).ToList();
                Assert.IsTrue(list.Count > 0);
            }
        }

        //[TestMethod]
        //public void SingleLetterReverseBenchMark()
        //{
        //    List<char> pool = new List<char>();
        //    var searchspace = "p\0e".ToArray();

        //    for (int i = 0; i < 1000; i++)
        //    {
        //        var list = _reverseSearcher.FindMatchingSequences(pool, searchspace, default(char), 1).ToList();
        //        Assert.IsTrue(list.Count > 0);
        //    }
        //}

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void IndexBlankTest()
        {
            List<char> pool = new List<char>() { 't', 'h', 'y', 'k', 'd', 'l' };
            int BlankCount = 1;
            char[] existing = new char[] { '\0', '\0', 'e', 'a', '\0', '\0', '\0', 's', 'e', 'i', 'z', 'o', 'r', '\0', '\0' };
            MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
            var results = minimalDAGSearcher.FindMatchingSequencesContainingIndex(pool, new Pattern<char>(existing, default(char)), BlankCount, 2).ToList();
            Assert.IsTrue(results.Count > 0);
            //FindMatchingSequencesContainingIndex(IEnumerable<T> valuePool, T[] existingValues, T emptyValue, int wildCardCount, int index)
        }

        [TestMethod]
        public void StartAndEndTest()
        {
            var Letters = new List<char>() { 'a', 'e', 's' };
            char[] existing = new char[] { 'c', 'a', 'r', default(char) };

            var results = _searcher.FindMatchingSequences(Letters, new Pattern<char>(existing, default(char)), 0).ToList();

            var Expected = new List<string>() { "care", "car", "cars" };//, "scar","scare" };

            var Actual = results.Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(Expected, Actual);

            Letters = new List<char>() { 'a', 'e', 's' };
            existing = new char[] { default(char), 'c', 'a', 'r', default(char) };

            results = _searcher.FindMatchingSequences(Letters, new Pattern<char>(existing, default(char)), 0).ToList();

            Expected = new List<string>() { "care", "car", "cars", "scar","scare" };

            Actual = results.Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(Expected, Actual);

        }
        internal void NestedSequenceCompare<T>(List<List<T>> expected, List<List<T>> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                CollectionAssert.AreEquivalent(expected[i], actual[i]);
            }
        }
    }
}