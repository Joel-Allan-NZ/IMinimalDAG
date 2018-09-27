using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations.Tests
{
    internal static class TestData
    {


        internal static List<string> GetCharSequences()
        {
            return new List<string>()
            {
                "pies",
                "fhqwhgads",
                "ferret",
                "farce",
                "force",
                "piece",
                "pieces",
                "piecemeal",
                "either",
                "zither"
            };
        }

        internal static List<List<int?>> GetIntSequences()
        {
            return new List<List<int?>>()
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
                },
                new List<int?>()
                {
                    0,
                    5,
                    2,
                    1
                },
                new List<int?>()
                {
                    int.MaxValue,
                    int.MinValue,
                    0,
                    null
                }
            };
        }

        internal static List<List<Coord>> GetCoordSequences()
        {
            return new List<List<Coord>>()
            {
                new List<Coord>()
                {
                    new Coord(9, 5),
                    new Coord(10, 6),
                    new Coord(11, 8),
                    new Coord(12, 8)
                },
                new List<Coord>()
                {
                    new Coord(8, 5),
                    new Coord(9,5),
                    new Coord(11, 9),
                    new Coord(12, 10)
                },
                new List<Coord>()
                {
                    new Coord(17, 3),
                    new Coord(51, 23),
                    new Coord(12, 31)
                }
            };
        }

        internal static List<List<DateTime>> GetDateTimeSequences()
        {
            return new List<List<DateTime>>()
            {
                new List<DateTime>()
                {
                    new DateTime(2018, 9, 20, 9, 59, 30),
                    new DateTime(1990, 4, 26),
                    new DateTime(1990, 4, 25),
                    new DateTime(2001, 9, 11)
                },
                new List<DateTime>()
                {
                    new DateTime(1990, 4, 26),
                    new DateTime(2001, 9, 11),
                    new DateTime(1919, 4, 25)
                },
                new List<DateTime>()
                {
                    new DateTime(1988, 3, 11),
                    new DateTime(2018, 9, 15),
                    new DateTime(1971, 6, 23),
                    new DateTime(1996, 11, 11)
                }
            };
        }
    }
}
