using IMinimalDAGInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalDAGImplementations.Serializer;
using MinimalDAGImplementations.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinimalDAGSearcher.Tests
{
    [TestClass()]
    public class MinimalDAGSearcherTests
    {
        static string _testDataDirectoryPath;
        static IMinimalDAG<char> _dawg;
        static IMinimalDAG<int?> _nullIntDAG;
        //todo: considerably more testing.
        static IMinimalDAG<DateTime> _dateTimeDAG;
        static IMinimalDAG<char> _turkishDAWG;
        static IMinimalDAG<Coord> _coordDAG;


        [ClassInitialize]
        public static void InitializeTest(TestContext testContext)
        {
            _testDataDirectoryPath = @"C:\Users\joelc\source\repos\ScrabbleSolver\TestData";// Directory.GetCurrentDirectory();
            _dawg = MinimalDAGSerializer.ReadCompressed<char>(Path.Combine(_testDataDirectoryPath, "ScrabbleJsonSerializedGZIP.gz"));
            _nullIntDAG = MinimalDAGSerializer.ReadCompressed<int?>(Path.Combine(_testDataDirectoryPath, "NullIntJsonSerializedGZIP.gz"));
        }

        [TestMethod()]
        public void FindMatchingNullIntSequencesTest()
        {
            MinimalDAGSearcher<int?> DagSearcher = new MinimalDAGSearcher<int?>(_nullIntDAG);
            List<int?> ValuePool = new List<int?>() { 1, 2, 9, 8 };
            int?[] ExistingValues = new int?[] { -1, 5, 1, -1, -1 };


            var matches = DagSearcher.FindMatchingSequences(ValuePool, ExistingValues, -1, 0).Select(x => x.MatchingSequence.ToList()).ToList();

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

            var Actual = minimalDAGSearcher.FindMatchingSequences(CharPool, existing, default(char), 6).Select(x => string.Concat(x.MatchingSequence)).ToList();

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

            var Actual = minimalDAGSearcher.FindMatchingSequences(CharPool, existing, default(char), 14).Select(x => string.Concat(x.MatchingSequence)).ToList();
            Assert.IsTrue(Actual.Count > 0);
        }

        [TestMethod]
        public void ContainingIndexTest()
        {
            MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
            List<char> CharPool = new List<char>() { 'a', 's', 'd', 'o'}; /*{ '*', '*', '*', '*', '*', '*' };*/
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

            var Actual = minimalDAGSearcher.FindMatchingSequencesContainingIndex(CharPool, existing, default(char), 0, 10).Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CornerCase()
        {
            List<char> pool = new List<char>() { 'b', 'i', 'd', 'a', 'r', 'x' };
            MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
            var results = minimalDAGSearcher.FindMatchingSequences(pool, "\0\0\0\0\0\0\0doctor\0\0".ToArray(), default(char), 1).Select(x => string.Concat(x.MatchingSequence)).ToList();

            Assert.IsTrue(results.Count > 0);
        }

        //[TestMethod]
        //public void AdoTest()
        //{
        //    MinimalDAGSearcher<char> minimalDAGSearcher = new MinimalDAGSearcher<char>(_dawg);
        //    List<char> CharPool = new List<char>() { 'a'};
        //    var existing = new char[] { default(char), 'd', 'o', default(char) };
        //    var ExpectedMatches = new List<string>() { "do", "ado", "doe", "dog", "dot", "don" };

        //    var Actual = minimalDAGSearcher.FindMatchingSequences(CharPool, existing, default(char), 1).Select(x => string.Concat(x)).ToList();

        //    CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        //}

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