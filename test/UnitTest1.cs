using System;
using System.Diagnostics;
using sudoku_sat_solver;
using Xunit;

namespace test {
    public class UnitTest1 {
        [Fact]
        public void TestLoader () {
            var loader = new SudokuLoader ();
            var board = loader.Load ("/home/xyz600/program/sudoku-sat-solver/test/test.txt");
            Assert.True (board.Count == 9);

            Int32[] result = {
                0,
                0,
                3,
                0,
                2,
                0,
                6,
                0,
                0,
                9,
                0,
                0,
                3,
                0,
                5,
                0,
                0,
                1,
                0,
                0,
                1,
                8,
                0,
                6,
                4,
                0,
                0,
                0,
                0,
                8,
                1,
                0,
                2,
                9,
                0,
                0,
                7,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                8,
                0,
                0,
                6,
                7,
                0,
                8,
                2,
                0,
                0,
                0,
                0,
                2,
                6,
                0,
                9,
                5,
                0,
                0,
                8,
                0,
                0,
                2,
                0,
                3,
                0,
                0,
                9,
                0,
                0,
                5,
                0,
                1,
                0,
                3,
                0,
                0,
            };

            int index = 0;
            foreach (var row in board) {
                Assert.True (row.Count == 9);
                foreach (var item in row) {
                    Assert.True (item == result[index]);
                    index++;
                }
            }
            Assert.True (index == 81);
        }
    }
}