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
            public int RowColObjId { get; set; }

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
        private int[,] _rowcoles;
        private Set[,,] _sets;

        #endregion

        #region Properties

        public int SubregionSize { get; set; }

        public int Size
        {
            get { return SubregionSize*SubregionSize; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Sets up for the specified size
        /// </summary>
        public void Setup()
        {
            var lenInSubregion = SubregionSize;
            var totalSubRegions = lenInSubregion*lenInSubregion;

            _subes = new int[totalSubRegions,Size];
            _rowes = new int[Size, Size];
            _coles = new int[Size, Size];
            _rowcoles = new int[Size, Size];
            _sets = new Set[Size, Size, Size];

            var objId = 0; // 0 based is surely ok. it's index like id
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    _rowes[j, i] = objId++;
                    _rowcoles[j, i] = objId++;
                    _coles[j, i] = objId++;
                }
                for (var sub = 0; sub < totalSubRegions; sub++)
                {
                    _subes[sub, i] = objId++;
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
                                    ColObjId = _coles[x, n],
                                    RowColObjId = _rowcoles[y, x]
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
                            Contents = new[]
                            {
                                set.RowObjId,
                                set.ColObjId,
                                set.SubregionObjId,
                                set.RowColObjId
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
            allcols.AddRange(_rowcoles.Cast<int>());

            _dl.Populate(dlsets, allcols, _rowDict);
        }

        /// <summary>
        ///  Places a pre-existing number
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="col">The column</param>
        /// <param name="val">The number</param>
        public void Place(int row, int col, int val)
        {
            var set = _sets[row, col, val];
            _dl.Fix(_rowDict, new[] {set}, _fixSaved);
        }

        /// <summary>
        ///  Place pre-existing numbers
        /// </summary>
        /// <param name="tuples">The pre-existing numbers</param>
        public void Place(IEnumerable<Tuple> tuples)
        {
            var pesets = tuples.Select(tuple => _sets[tuple.Row, tuple.Column, tuple.Value]).ToList();
            _dl.Fix(_rowDict, pesets, _fixSaved);
        }

        /// <summary>
        ///  Removes all placed pre-existing numbers
        /// </summary>
        public void UnPlace()
        {
            if (_fixSaved.FirstColumn != null)
            {
                _dl.UnFix(_fixSaved);
            }
        }

        /// <summary>
        ///  Solves and gets the solution
        /// </summary>
        /// <returns>The solution if any or null</returns>
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

        #endregion
    }
}
