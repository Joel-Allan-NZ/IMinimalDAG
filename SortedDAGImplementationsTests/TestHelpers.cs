using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations.Tests
{
    internal static class TestHelpers
    {
        internal static void NestedSequenceCompare<T>(List<List<T>> expected, List<List<T>> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                CollectionAssert.AreEquivalent(expected[i], actual[i]);
            }
        }
        internal static IEnumerable<string> GetSortedLines(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadLines(filePath).OrderBy(x => x);
            }
            else return null;
        }
    }
}
