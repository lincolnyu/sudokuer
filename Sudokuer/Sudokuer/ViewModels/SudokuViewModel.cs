using System;
using System.Collections.Generic;
using System.ComponentModel;
using SudokuSolver;

namespace Sudokuer.ViewModels
{
    public class SudokuViewModel : BaseViewModel
    {
        #region Enumerations

        public enum ValueTypes
        {
            Puzzle,
            KeyJustUpdated,
            Key,
        }

        public class CellViewModel : BaseViewModel
        {
            #region Delegates

            public delegate void CellChangedEventHandler(int row, int col);

            #endregion

            #region Fields

            private ValueTypes _valueType;
            private string _value;

            #endregion

            #region Constructors

            public CellViewModel(int row, int col)
            {
                Row = row;
                Col = col;
            }

            #endregion

            #region Properties

            public ValueTypes ValueType
            {
                get { return _valueType; }
                set
                {
                    if (_valueType != value)
                    {
                        _valueType = value;
                        OnPropertyChanged();
                        RaiseCellChangedEvent();
                    }
                }
            }

            public string Value
            {
                get { return _value; }
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        OnPropertyChanged();
                        RaiseCellChangedEvent();
                    }
                }
            }

            public int Row { get; private set; }

            public int Col { get; private set; }

            #endregion

            #region Events

            public event CellChangedEventHandler CellChanged;

            #endregion

            #region Methods

            private void RaiseCellChangedEvent()
            {
                if (CellChanged != null)
                {
                    CellChanged(Row, Col);
                }
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly Solver _solver = new Solver();

        private bool _skuChangingCells;

        private bool _needReSolve;

        #endregion

        #region Constructors

        public SudokuViewModel()
        {
            SubregionSize = 3;
        }

        #endregion

        private int _subregionSize;

        /// <summary>
        ///  If EliminatesKey() is being called
        /// </summary>
        private bool _inEliminatesKey;

        #region Properties

        public IList<int> SubregionSizes
        {
            get { return new[] {2,3,4,5}; }
        }

        public int SubregionSize
        {
            get { return _subregionSize; }
            set
            {
                if (_subregionSize != value)
                {
                    _subregionSize = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Size");

                    ResizeMatrix();
                }
            }
        }

        public int Size
        {
            get { return SubregionSize*SubregionSize; }
        }

        public CellViewModel[][] Cells { get; private set; }

        #endregion

        #region Methods

        private void ResizeMatrix()
        {
            if (Cells != null)
            {
                for (var i = 0; i < Cells.Length; i++)
                {
                    for (var j = 0; j < Cells[i].Length; j++)
                    {
                        Cells[i][j].PropertyChanged -= OnCellPropertyChanged;
                        Cells[i][j].CellChanged -= CellChanged;
                    }
                }
            }

            Cells = new CellViewModel[Size][];
            for (var i = 0; i < Size; i++)
            {
                Cells[i] = new CellViewModel[Size];
                for (var j = 0; j < Size; j++)
                {
                    Cells[i][j] = new CellViewModel(i, j);
                    Cells[i][j].PropertyChanged += OnCellPropertyChanged;
                    Cells[i][j].CellChanged += CellChanged;
                }
            }

            _solver.SubregionSize = SubregionSize;
            _solver.Setup();
            _needReSolve = true;
        }


        public bool Solve()
        {
            return _needReSolve ? ReSolve() : SolveNext();
        }


        public void Reset()
        {
            _skuChangingCells = true;
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    var cell = Cells[i][j];
                    if (cell.ValueType != ValueTypes.Puzzle)
                    {
                        cell.Value = "";
                    }
                }
            }
            _skuChangingCells = false;
            _needReSolve = true;
           // _solver.Restart(); // this is not needed
        }

        public void Clear()
        {
            _skuChangingCells = true;
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    var cell = Cells[i][j];
                    cell.Value = "";
                }
            }
            _skuChangingCells = false;
            _needReSolve = true;
        }

        private void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // seems nothing needs to be done since CellChanged() takes it over
        }

        private void CellChanged(int row, int col)
        {
            if (!_skuChangingCells)
            {
                _needReSolve = true;
                EliminatesKey(row, col);
            }
        }

        private void EliminatesKey(int row, int col)
        {
            if (_inEliminatesKey)
            {
                return;
            }
            _inEliminatesKey = true;
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    var cell = Cells[i][j];
                    if (i == row && j == col && !string.IsNullOrWhiteSpace(cell.Value))
                    {
                        cell.ValueType = ValueTypes.Puzzle;
                        continue;
                    }
                    if (cell.ValueType != ValueTypes.Puzzle)
                    {
                        cell.Value = "";
                    }
                }
            }
            _inEliminatesKey = false;
        }

        private bool SolveNext()
        {
            _solver.Next();
            return RunSolve();
        }

        private bool ReSolve()
        {
            // collects user input
            var tuples = new List<Solver.Tuple>();
            const int min = 1;
            var max = Size;
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    var cell = Cells[i][j];
                    var val = cell.Value;
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        continue;
                    }
                    int ival;
                    if (!int.TryParse(val, out ival) || ival < min || ival > max)
                    {
                        // TODO report error
                        continue;
                    }
                    // NOTE ival is 1 based
                    tuples.Add(new Solver.Tuple { Row = i, Column = j, Value = ival - 1 });
                    cell.ValueType = ValueTypes.Puzzle;
                }
            }
            _solver.UnPlace();
            var validPlace = _solver.Place(tuples);
            if (!validPlace)
            {
                return false;
            }
            return RunSolve();
        }

        private bool RunSolve()
        {
            var sol = _solver.Solve();
            _needReSolve = false;
            if (sol == null)
            {
                return false;
            }

            _skuChangingCells = true;
            foreach (var s in sol)
            {
                var cell = Cells[s.Row][s.Column];
                var oldVal = cell.Value;
                var newVal = Convert.ToString(s.Value + 1);
                if (oldVal != newVal && !string.IsNullOrWhiteSpace(oldVal))
                {
                    cell.ValueType = ValueTypes.KeyJustUpdated;
                }
                else
                {
                    cell.ValueType = ValueTypes.Key;
                }
                cell.Value = newVal;
            }
            _skuChangingCells = false;

            return true;
        }

        #endregion
    }
}
