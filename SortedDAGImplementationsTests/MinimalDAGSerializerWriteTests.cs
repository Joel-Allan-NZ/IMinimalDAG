using IMinimalDAGInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalDAGImplementations;
using MinimalDAGImplementations.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations.Tests
{
    [TestClass]
    public class MinimalDAGSerializerWriteTests
    {
        static IMinimalDAG<char> _dag;
        static string _scrabbleTestDataDirectoryPath;

        static IEnumerable<IEnumerable<int?>> _intData;
        static IEnumerable<IEnumerable<Coord>> _structData;
        static IEnumerable<IEnumerable<DateTime>> _dateTimeData;

        static MinimalDAG<int?> _intDAG;
        static MinimalDAG<Coord> _structDAG;
        static MinimalDAG<DateTime> _dateTimeDAG;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _dag = new MinimalDAG<char>(TestHelpers.GetSortedLines(Path.Combine(Directory.GetCurrentDirectory(), "EnglishScrabbleWords.txt")), new MinimalDAGNodeFactory<char>());
            _scrabbleTestDataDirectoryPath = @"C:\Users\joelc\source\repos\ScrabbleSolver\TestData"; //Directory.GetCurrentDirectory();

            _intData = TestData.GetIntSequences().SequenceOrder();
            _structData = TestData.GetCoordSequences().SequenceOrder();
            _dateTimeData = TestData.GetDateTimeSequences().SequenceOrder();

            _intDAG = new MinimalDAG<int?>(_intData, new MinimalDAGNodeFactory<int?>());
            _structDAG = new MinimalDAG<Coord>(_structData, new MinimalDAGNodeFactory<Coord>());
            _dateTimeDAG = new MinimalDAG<DateTime>(_dateTimeData, new MinimalDAGNodeFactory<DateTime>());
        }

        [TestMethod]
        public void GZipJsonEnglishCharWriteTest()
        {
            //var big = new MinimalDAG<char>(GetSortedLines(Path.Combine(_scrabbleTestDataDirectoryPath, "EnglishScrabbleWords.txt")), new MinimalDAGNodeFactory<char>());
            string filePath = Path.Combine(_scrabbleTestDataDirectoryPath, "ScrabbleJsonSerializedGZIP.gz");
            MinimalDAGSerializer.Compress<char>(_dag, filePath);
        }

        [TestMethod]
        public void GZipJsonTurkishWordWriteTest()
        {
            var _turkDag = new MinimalDAG<char>(TestHelpers.GetSortedLines(Path.Combine(_scrabbleTestDataDirectoryPath, "TurkishWordList.txt")), new MinimalDAGNodeFactory<char>());
            string filePath = Path.Combine(_scrabbleTestDataDirectoryPath, "TurkishJsonSerializedGZIP.gz");
            MinimalDAGSerializer.Compress<char>(_turkDag, filePath);
        }

        [TestMethod]
        public void GZipJsonNullIntWriteTest()
        {
            string filePath = Path.Combine(_scrabbleTestDataDirectoryPath, "NullIntJsonSerializedGZIP.gz");
            MinimalDAGSerializer.Compress<int?>(_intDAG, filePath);

        }

        [TestMethod]
        public void GZipStructWriteTest()
        {
            string filePath = Path.Combine(_scrabbleTestDataDirectoryPath, "StructJsonSerializedGZIP.gz");
            MinimalDAGSerializer.Compress(_structDAG, filePath, new CoordConverter());
        }

        [TestMethod]
        public void GZipDateWriteTest()
        {
            string filePath = Path.Combine(_scrabbleTestDataDirectoryPath, "DateTimeJsonSerializedGZIP.gz");
            MinimalDAGSerializer.Compress(_dateTimeDAG, filePath);
        }

    }
}