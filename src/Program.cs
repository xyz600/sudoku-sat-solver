using System;

namespace sudoku_sat_solver {
    class Program {

        static void Dump (Int32 varCount, ExpressionAnd CNF) {
            var cnfSize = CNF.children.Count;
            Console.Error.WriteLine ($"p cnf {varCount} {cnfSize}");
            foreach (var child in CNF.children) {
                if (typeof (ExpressionInteger) == child.GetType ()) {
                    var itemInt = (ExpressionInteger) child;
                    var val = itemInt.negative ? itemInt.id : -itemInt.id;
                    Console.Error.WriteLine ($"{val} 0");
                } else if (typeof (ExpressionOr) == child.GetType ()) {
                    foreach (var item in child.children) {
                        var itemInt = (ExpressionInteger) item;
                        var val = itemInt.negative ? itemInt.id : -itemInt.id;
                        Console.Error.Write ($"{val} ");
                    }
                    Console.Error.WriteLine (0);
                } else {
                    throw new Exception ("unexpected type");
                }
            }
        }

        static void Main (string[] args) {
            var loader = new SudokuLoader ();
            var filepath = args[0];
            var board = loader.Load (filepath);
            var size = board.Count;
            var encoder = new SudokuEncoder (size);
            var result = encoder.encode (board);
            Dump (encoder.VariableCount, result);
        }
    }
}