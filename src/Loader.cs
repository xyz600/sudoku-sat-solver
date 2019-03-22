using System;
using System.Collections.Generic;
using System.IO;

namespace sudoku_sat_solver {

    public class SudokuLoader {

        public List<List<Int32>> Load (string Filepath) {
            var list = new List<List<Int32>> ();
            using (var reader = new StreamReader (Filepath)) {
                string line = null;
                while ((line = reader.ReadLine ()) != null) {
                    if (line.Length > 0) {
                        var row = new List<Int32> ();
                        foreach (var token in line.Split (" ")) {
                            row.Add (Int32.Parse (token));
                        }
                        list.Add (row);
                    }
                }
            }
            return list;
        }
    }
}