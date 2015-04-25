using System.Collections.Generic;
using SudokuSolver;

namespace Sudokuer.ViewModels
{
    public class SudokuViewModel : BaseViewModel
    {
        #region Fields

        private Solver _solver = new Solver();

        #endregion

        #region Constructors

        public SudokuViewModel()
        {
            SubregionSize = 3;
        }

        #endregion

        private int _subregionSize;

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

        public string[][] Values { get; private set; }

        #endregion

        #region Methods

        private void ResizeMatrix()
        {
            Values = new string[Size][];
            for (var i = 0; i < Size; i++)
            {
                Values[i] = new string[Size];
            }
            OnPropertyChanged("Values");

            _solver.SubregionSize = SubregionSize;
            _solver.Setup();
        }

        public void Solve()
        {
            // collects user input
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    
                }
            }
        }

        #endregion
    }
}
