using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinimalDAGImplementations.Tests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class MinimalDAGConstructionTests
    {
        static MinimalDAG<char> _dawg;
        static MinimalDAG<int?> _intDAG;
        static MinimalDAG<Coord> _structDAG;
        static MinimalDAG<DateTime> _dateTimeDAG;

        static IEnumerable<string> _dawgData;
        static IEnumerable<IEnumerable<int?>> _intData;
        static IEnumerable<IEnumerable<Coord>> _structData;
        static IEnumerable<IEnumerable<DateTime>> _dateTimeData;

        [ClassInitialize]
        public static void ClassInitalize(TestContext context)
        {
            _dawgData = TestData.GetCharSequences().OrderBy(x => x);
            //_intData = TestData.GetIntSequences().SequenceOrder();
            _structData = TestData.GetCoordSequences().SequenceOrder();
            _dateTimeData = TestData.GetDateTimeSequences().SequenceOrder();

            _dawg = new MinimalDAG<char>(_dawgData, new MinimalDAGNodeFactory<char>());
            //_intDAG = new MinimalDAG<int?>(_intData, new MinimalDAGNodeFactory<int?>());
            _structDAG = new MinimalDAG<Coord>(_structData, new MinimalDAGNodeFactory<Coord>());
            _dateTimeDAG = new MinimalDAG<DateTime>(_dateTimeData, new MinimalDAGNodeFactory<DateTime>());
        }

        [TestMethod]
        public void CharBuildTest()
        {
            var Strings = _dawg.GetAllSequences().Select(x => string.Concat(x)).ToList();
            CollectionAssert.AreEquivalent(TestData.GetCharSequences(), Strings);
        }

        //[TestMethod]
        //public void IntBuildTest()
        //{
        //    var Ints = _intDAG.GetAllSequences().ToList();
        //    var ExpectedInts = _intData.ToList();
        //    Assert.AreEqual(ExpectedInts.Count, Ints.Count);
        //    for (int i = 0; i < ExpectedInts.Count; i++)
        //        CollectionAssert.AreEquivalent(ExpectedInts[i].ToList(), Ints[i]);
        //}

        [TestMethod]
        public void StructBuildTest()
        {
            var Structs = _structDAG.GetAllSequences().ToList();
            var ExpectedStructs =_structData.ToList();
            Assert.AreEqual(ExpectedStructs.Count, Structs.Count);
            for (int i = 0; i < ExpectedStructs.Count; i++)
                CollectionAssert.AreEquivalent(ExpectedStructs[i].ToList(), Structs[i]);
        }

        [TestMethod]
        public void DateTimeBuildTest()
        {
            var DateTimes = _dateTimeDAG.GetAllSequences().ToList();
            var Expected = _dateTimeData.ToList();
            Assert.AreEqual(Expected.Count, DateTimes.Count);
            for (int i = 0; i < Expected.Count; i++)
                CollectionAssert.AreEquivalent(Expected[i].ToList(), DateTimes[i]);
        }

    }  
}
