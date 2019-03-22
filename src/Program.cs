using System;

namespace sudoku_sat_solver {
    class Program {
        static void Main (string[] args) {
            var loader = new SudokuLoader ();
            var filepath = args[0];
            var board = loader.Load (filepath);
            var size = board.Count;
            var encoder = new SudokuEncoder (size);
            var result = encoder.encode (board);
        }
    }
}