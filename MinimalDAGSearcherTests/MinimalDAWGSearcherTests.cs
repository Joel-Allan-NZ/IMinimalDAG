using IMinimalDAGInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalDAGImplementations;
using MinimalDAGImplementations.Serializer;
using MinimalDAGSearcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGSearcher.Tests
{
    [TestClass]
    public class MinimalDAWGSearcherTests
    {
        static string _testDataDirectoryPath;
        static IMinimalDAG<char> _dawg;
        static MinimalDAWGSearcher _searcher;
        //static MinimalDAWGSearcherAlternate _alt;

        [ClassInitialize]
        public static void InitializeTest(TestContext testContext)
        {
            _testDataDirectoryPath = @"C:\Users\joelc\source\repos\ScrabbleSolver\TestData";
            _dawg = MinimalDAGSerializer.ReadCompressed<char>(Path.Combine(_testDataDirectoryPath, "ScrabbleJsonSerializedGZIP.gz"));
            _searcher = new MinimalDAWGSearcher((MinimalDAG<char>)_dawg);
            //_alt = new MinimalDAWGSearcherAlternate((MinimalDAG<char>)_dawg);
        }

        [TestMethod]
        public void NullValueTest()
        {
            HashSet<char>[] SearchSpace = new HashSet<char>[]
            {
                null, null, null,
                new HashSet<char>(){'i'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'a'},
                new HashSet<char>(){'r'},
                null, null, null
            };
            List<char> CharPool = new List<char>();
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

            var Actual = _searcher.FindMatchingSequences(CharPool, new Pattern<char>(SearchSpace),
                                                                   6).Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
            SearchSpace[2] = new HashSet<char>() { default(char) };
            Actual = _searcher.FindMatchingSequences(CharPool, new Pattern<char>(SearchSpace),
                                                        6).Select(x => string.Concat(x.MatchingSequence)).ToList();
            ExpectedMatches = new List<string>()
            {
                "izzard",
                "izzards",
            };
            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        }

//        [TestMethod]
//        public void AltNullValueTest()
//        {
//            HashSet<char>[] SearchSpace = new HashSet<char>[]
//{
//                null, null, null,
//                new HashSet<char>(){'i'},
//                new HashSet<char>(){'z'},
//                new HashSet<char>(){'z'},
//                new HashSet<char>(){'a'},
//                new HashSet<char>(){'r'},
//                null, null, null
//};
//            List<char> CharPool = new List<char>();
//            var ExpectedMatches = new List<string>()
//            {
//                "blizzard",
//                "blizzardly",
//                "blizzards",
//                "blizzardy",
//                "gizzard",
//                "gizzards",
//                "izzard",
//                "izzards",
//            };

//            var Actual = _alt.FindMatchingSequences(CharPool, new Pattern<char>(SearchSpace),
//                                                                   6).Select(x => string.Concat(x.MatchingSequence)).ToList();

//            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
//            SearchSpace[2] = new HashSet<char>() { default(char) };
//            Actual = _alt.FindMatchingSequences(CharPool, new Pattern<char>(SearchSpace),
//                                                        6).Select(x => string.Concat(x.MatchingSequence)).ToList();
//            ExpectedMatches = new List<string>()
//            {
//                "izzard",
//                "izzards",
//            };
//            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
//        }

        [TestMethod]
        public void OptionallyEmptyTest()
        {
            HashSet<char>[] SearchSpace = new HashSet<char>[]
{
                null, null, null,
                new HashSet<char>(){'i'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'a'},
                new HashSet<char>(){'r', '\0' },
                null, null, null
 };
            List<char> CharPool = new List<char>() { };
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
                "pizza"
            };

            var Actual = _searcher.FindMatchingSequences(CharPool, new Pattern<char>(SearchSpace),
                                                                   6).Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        }

        [TestMethod]
        public void FailedCase()
        {
            HashSet<Char>[] SearchSpace = new HashSet<char>[]
            {
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                new HashSet<char>(){'\0', 'y', 'x', 'w', 't', 's', 'r', 'n', 'm', 'l', 'i', 'h', 'g', 'e', 'd', 'b', 'a'},
                new HashSet<char>(){'\0', 't', 's', 'n', 'f', 'd'},
                new HashSet<char>(){'s'},
                null,
                null,
                null,
                null,
                null
            };
            List<char> letters = new List<char>() { 'e', 'h', 'y', 'k', 'd', 'l' };

            var results = _searcher.FindMatchingSequencesContainingIndex(letters, new Pattern<char>(SearchSpace), 0, 9);
        }

        [TestMethod]
        public void ForcedEmptyTest()
        {
            HashSet<char>[] SearchSpace = new HashSet<char>[]
            {
                null, null, null,
                new HashSet<char>(){'i'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'z'},
                new HashSet<char>(){'a'},
                new HashSet<char>(){'\0'},
                null, null, null
             };
            List<char> CharPool = new List<char>();
            var ExpectedMatches = new List<string>()
            {
                //"blizzard",
                //"blizzardly",
                //"blizzards",
                //"blizzardy",
                //"gizzard",
                //"gizzards",
                //"izzard",
                //"izzards",
                "pizza",
                //"pizzaz",
                //"pizzazz",
                //"pizzazzy",
                //"pizzazzes",
                //"pizzazes",
                //"pizzas",
                //"pizzalike"
            };

            var Actual = _searcher.FindMatchingSequences(CharPool, new Pattern<char>(SearchSpace),
                                                                   6).Select(x => string.Concat(x.MatchingSequence)).ToList();

            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
            SearchSpace[2] = new HashSet<char>() { default(char) };
            Actual = _searcher.FindMatchingSequences(CharPool, new Pattern<char>(SearchSpace),
                                                        6).Select(x => string.Concat(x.MatchingSequence)).ToList();
            ExpectedMatches = new List<string>()
            {
                //"izzard",
                //"izzards",
            };
            CollectionAssert.AreEquivalent(ExpectedMatches, Actual);
        }

        [TestMethod]
        public void StartingAtNonConcreteIndexTest()
        {
            var hash = new HashSet<char>[]
            {
                null,
                null,
                null,
                null,
                null,
                new HashSet<char>(){'\0', 's'},
                null
            };
            var letters = new List<char>() { 't', 'h', 'e', 'm', 'e', 's' };
            var Pattern = new Pattern<char>(hash);

            var matches =  _searcher.FindMatchingSequences(letters, Pattern, 0).Select(x => string.Concat(x.MatchingSequence)).ToList();
            var match = _searcher.FindMatchingSequences(letters, Pattern, 0).Where(x => string.Concat(x.MatchingSequence) == "themes");


            Assert.IsTrue(matches.Contains("themes"));

        }
    }
}
