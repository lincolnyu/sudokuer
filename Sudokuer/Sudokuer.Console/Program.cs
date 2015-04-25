using System;
using System.Collections.Generic;
using System.Text;
using SudokuSolver;

namespace Sudokuer.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            int size;
        _reinputSize:
            System.Console.WriteLine("Input block size (for instance for 9x9 Sudoku it's 3):");
            var inputStr = System.Console.ReadLine();
            if (!int.TryParse(inputStr, out size))
            {
                System.Console.WriteLine("Wrong input format");
                goto _reinputSize;
            }

            var ssol = new Solver {SubregionSize = size};

            ssol.Setup();

        _reinputPuzzle:

            ssol.UnPlace();

            System.Console.WriteLine("Input the puzzle in one line in such form: row1,col1,number1;row2,col2,number2;...");
            // sample data:
            // 0,0,5;0,1,3;0,4,7;1,0,6;1,3,1;1,4,9;1,5,5;2,1,9;2,2,8;2,7,6;3,0,8;3,4,6;3,8,3;4,0,4;4,3,8;4,5,3;4,8,1;5,0,7;5,4,2;5,8,6;6,1,6;6,6,2;6,7,8;7,3,4;7,4,1;7,5,9;7,8,5;8,4,8;8,7,7;8,8,9
            inputStr = System.Console.ReadLine();
            var puzzle = ParsePuzzleString(inputStr);
            ssol.Place(puzzle);

            System.Console.WriteLine("The puzzle:");
            var puzzleStr = PuzzleToString(size, puzzle);
            System.Console.WriteLine(puzzleStr);
            System.Console.WriteLine();

            ssol.Restart();

            ConsoleKeyInfo k;
            while (true)
            {
                var sol = ssol.Solve();
                if (sol == null)
                {
                    System.Console.WriteLine("No more solution");
                    break;
                }
                var solStr = SolutionToString(size, puzzle, sol);
                System.Console.WriteLine(solStr);
                System.Console.WriteLine();
                System.Console.Write("Want to find the next solution? (Y/N) ");
                k = System.Console.ReadKey();
                System.Console.WriteLine();
                if (k.Key != ConsoleKey.Y)
                {
                    break;
                }
                ssol.Next();
            }

            System.Console.Write("Input another puzzle with same size? (Y/N) ");
            k = System.Console.ReadKey();
            System.Console.WriteLine();
            if (k.Key == ConsoleKey.Y)
            {
                goto _reinputPuzzle;
            }

            System.Console.Write("Input another puzzle with a different size? (Y/N) ");
            k = System.Console.ReadKey();
            System.Console.WriteLine();
            if (k.Key == ConsoleKey.Y)
            {
                goto _reinputSize;
            }
        }

        private static string PuzzleToString(int size, IEnumerable<Solver.Tuple> puzzle)
        {
            return SolutionToString(size, puzzle, new Solver.Tuple[]{});
        }

        private static string SolutionToString(int size, IEnumerable<Solver.Tuple> puzzle, IEnumerable<Solver.Tuple> sol)
        {
            var lsize = size*size;
            var map = new string[lsize, lsize];
            var maxLen = 0;
            foreach (var p in puzzle)
            {
                var s= string.Format("[{0}]", p.Value+1);
                map[p.Row, p.Column] = s;
                if (s.Length > maxLen)
                {
                    maxLen = s.Length;
                }
            }

            foreach (var o in sol)
            {
                var s = string.Format("{0}", o.Value+1);
                map[o.Row, o.Column] = s;
                if (s.Length > maxLen)
                {
                    maxLen = s.Length;
                }
            }

            var formatStr = string.Format("{{0,{0}}}", maxLen + 1);
            var sb = new StringBuilder();
            for (var i = 0; i < lsize; i++)
            {
                for (var j = 0; j < lsize; j++)
                {
                    var s = map[i, j];
                    sb.AppendFormat(formatStr, s);
                }
                if (i < lsize - 1)
                {
                    sb.AppendLine();    
                }
            }
            return sb.ToString();
        }

        private static IList<Solver.Tuple> ParsePuzzleString(string puzzleString)
        {
            var list = new List<Solver.Tuple>();
            var error = false;
            var tupleStrs = puzzleString.Split(';');
            foreach (var tupleStr in tupleStrs)
            {
                if (string.IsNullOrWhiteSpace(tupleStr))
                {
                    error = true;
                    continue;
                }
                var components = tupleStr.Split(',');
                if (components.Length != 3)
                {
                    error = true;
                    continue;
                }
                int row, col, val;
                if (!int.TryParse(components[0], out row))
                {
                    error = true;
                    continue;
                }
                if (!int.TryParse(components[1], out col))
                {
                    error = true;
                    continue;
                }
                if (!int.TryParse(components[2], out val))
                {
                    error = true;
                    continue;
                }
                list.Add(new Solver.Tuple {Row = row, Column = col, Value = val-1});
            }
            if (error)
            {
                System.Console.WriteLine("There were errors in the input, but it's still been accepted.");
            }
            return list;
        }
    }
}
