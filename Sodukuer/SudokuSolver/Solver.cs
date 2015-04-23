using System.Collections.Generic;
using System.Linq;
using QSharp.Scheme.ExactCover;

namespace SudokuSolver
{
    public class Solver
    {
        #region Nested types

        public struct Tuple
        {
            #region Properties

            public int Row { get; set; }
            public int Column { get; set; }
            public int Value { get; set; }

            #endregion
        }

        private class Set
        {
            #region Properties

            public int Row { get; set; }
            public int Col { get; set; }
            public int Val { get; set; }

            public int SubregionObjId { get; set; }
            public int RowObjId { get; set; }
            public int ColObjId { get; set; }

            #endregion
        }

        #endregion

        #region Fields

        private readonly DancingLinks<Set, int> _dl = new DancingLinks<Set, int>();

        private readonly IDictionary<Set, object> _rowDict = new Dictionary<Set, object>();
        private readonly DancingLinks<Set, int>.SavedBeforeFix _fixSaved = new DancingLinks<Set, int>.SavedBeforeFix();

        private int[,] _subes;
        private int[,] _rowes;
        private int[,] _coles;
        private Set[,,] _sets;

        #endregion

        #region Properties

        public int Size { get; set; }

        public int SubregionSize { get; set; }

        #endregion

        #region Methods

        public void Setup()
        {
            var lenInSubregion = Size/SubregionSize;
            var totalSubRegions = lenInSubregion*lenInSubregion;

            _subes = new int[totalSubRegions,Size];
            _rowes = new int[Size, Size];
            _coles = new int[Size, Size];
            _sets = new Set[Size, Size, Size];

            var objId = 1;
            for (var n = 0; n < Size; n++)
            {
                for (var y = 0; y < Size; y++)
                {
                    _rowes[y, n] = objId++;
                }
                for (var x = 0; x < Size; x++)
                {
                    _coles[x, n] = objId++;
                }
                for (var sub = 0; sub < totalSubRegions; sub++)
                {
                    _subes[sub, n] = objId++;
                }
            }

            for (var i = 0; i < lenInSubregion; i++)
            {
                var ibase = i*lenInSubregion;
                for (var j = 0; j < lenInSubregion; j++)
                {
                    var sub = ibase + j;
                    for (var y = i*SubregionSize; y < (i + 1)*SubregionSize; y++)
                    {
                        for (var x = j*SubregionSize; x < (j + 1)*SubregionSize; x++)
                        {
                            for (var n = 0; n < Size; n++)
                            {
                                //var set = new Set
                                _sets[y, x, n] = new Set
                                {
                                    Row = y,
                                    Col = x,
                                    Val = n,
                                    SubregionObjId = _subes[sub, n],
                                    RowObjId = _rowes[y, n],
                                    ColObjId = _coles[x, n]
                                };
                            }
                        }
                    }
                }
            }

            var dlsets = new List<DancingLinks<Set, int>.Set>();
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    for (var n = 0; n < Size; n++)
                    {
                        var set = _sets[y, x, n];
                        var dlset = new DancingLinks<Set, int>.Set
                        {
                            Contents =
                            {
                                set.RowObjId,
                                set.ColObjId,
                                set.SubregionObjId
                            },
                            Row = set
                        };
                        dlsets.Add(dlset);
                    }
                }
            }

            var allcols = _subes.Cast<int>().ToList();
            allcols.AddRange(_rowes.Cast<int>());
            allcols.AddRange(_coles.Cast<int>());

            _dl.Populate(dlsets, allcols, _rowDict);
        }

        public void Place(int row, int col, int val)
        {
            var set = _sets[row, col, val];
            _dl.Fix(_rowDict, new[] {set}, _fixSaved);
        }

        public IEnumerable<Tuple> Solve()
        {
            while (_dl.State != DancingLinks<Set, int>.States.Terminated
                   && _dl.State != DancingLinks<Set, int>.States.FoundSolution)
            {
                _dl.Step();
            }
            if (_dl.State == DancingLinks<Set, int>.States.FoundSolution)
            {
                return _dl.Solution.Select(x =>
                    new Tuple {Row = x.Row, Column = x.Col, Value = x.Val});
            }
            return null;
        }

        /// <summary>
        ///  If want to get the next solution
        /// </summary>
        public void Next()
        {
            _dl.Step();
        }

        public void Reset()
        {
            _dl.Reset();
        }

        public void UnPlace()
        {
            _dl.Reset();
            _dl.UnFix(_fixSaved);
        }

        #endregion
    }
}
