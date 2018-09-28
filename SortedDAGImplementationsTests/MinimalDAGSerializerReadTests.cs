using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalDAGImplementations.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinimalDAGImplementations.Tests
{
    [TestClass]
    public class MinimalDAGSerializerReadTests
    {
        static string _testDataDirectoryPath;

        static IEnumerable<IEnumerable<int?>> _intData;
        static IEnumerable<IEnumerable<Coord>> _structData;
        static IEnumerable<IEnumerable<DateTime>> _dateTimeData;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _testDataDirectoryPath = @"C:\Users\joelc\source\repos\ScrabbleSolver\TestData"; //Directory.GetCurrentDirectory();

            _intData = TestData.GetIntSequences().SequenceOrder();
            _structData = TestData.GetCoordSequences().SequenceOrder();
            _dateTimeData = TestData.GetDateTimeSequences().SequenceOrder();
        }

        [TestMethod]
        public void GZipJsonEnglishCharReadTest()
        {
            string filePath = Path.Combine(_testDataDirectoryPath, "ScrabbleJsonSerializedGZIP.gz");
            var val = MinimalDAGSerializer.ReadCompressed<char>(filePath);

            var ExpectedSequences = TestHelpers.GetSortedLines(Path.Combine(_testDataDirectoryPath, "EnglishScrabbleWords.txt")).ToList();
            var CharSequences = val.GetAllSequences();
            var ActualSequences = CharSequences.Select(x => string.Concat(x)).ToList();

            CollectionAssert.AreEquivalent(ExpectedSequences, ActualSequences);

        }

        [TestMethod]
        public void GZipJsonTurkishCharReadTest()
        {
            string filePath = Path.Combine(_testDataDirectoryPath, "TurkishJsonSerializedGZIP.gz");
            var val = MinimalDAGSerializer.ReadCompressed<char>(filePath);

            var ExpectedSequences = TestHelpers.GetSortedLines(Path.Combine(_testDataDirectoryPath, "TurkishWordList.txt")).ToList();
            var CharSequences = val.GetAllSequences();
            var ActualSequences = CharSequences.Select(x => string.Concat(x)).ToList();

            CollectionAssert.AreEquivalent(ExpectedSequences, ActualSequences);
        }

        [TestMethod]
        public void GZipJsonNullIntReadTest()
        {
            string filePath = Path.Combine(_testDataDirectoryPath, "NullIntJsonSerializedGZIP.gz");
            var val = MinimalDAGSerializer.ReadCompressed<int?>(filePath);

            var ExpectedSequences = _intData.Select(x => x.ToList()).ToList();
            var ActualSequences = val.GetAllSequences().ToList();

            TestHelpers.NestedSequenceCompare(ExpectedSequences, ActualSequences);
        }

        [TestMethod]
        public void GZipStructReadTest()
        {
            string filePath = Path.Combine(_testDataDirectoryPath, "StructJsonSerializedGZIP.gz");
            var val = MinimalDAGSerializer.ReadCompressed<Coord>(filePath, new CoordConverter());

            var ExpectedSequences = _structData.Select(x => x.ToList()).ToList();
            var ActualSequences = val.GetAllSequences().ToList();

            TestHelpers.NestedSequenceCompare(ExpectedSequences, ActualSequences);
        }

        [TestMethod]
        public void GZipDateReadTest()
        {
            string filePath = Path.Combine(_testDataDirectoryPath, "DateTimeJsonSerializedGZIP.gz");
            var val = MinimalDAGSerializer.ReadCompressed<DateTime>(filePath);

            var ExpectedSequences = _dateTimeData.Select(x => x.ToList()).ToList();
            var ActualSequences = val.GetAllSequences().ToList();

            TestHelpers.NestedSequenceCompare(ExpectedSequences, ActualSequences);
        }

    }
}
