using System;
using System.Collections.Generic;
using System.Diagnostics;
using sudoku_sat_solver;
using Xunit;

namespace test {

    public class UnitTestEncoder {
        void CheckType<T> (IExpression exp) {
            Assert.Equal (typeof (T), exp.GetType ());
        }

        [Fact]
        void TestEncodeCNF () {
            int size = 4;
            var encoder = new SudokuEncoder (size);
            var board = new List<List<Int32>> ();

            for (int i = 0; i < size; i++) {
                var row = new List<Int32> ();
                for (int j = 0; j < size; j++) {
                    row.Add (0);
                }
                board.Add (row);
            }
            var result = encoder.encode (board);
            foreach (var child in result.children) {
                if (typeof (ExpressionOr) == child.GetType ()) {
                    foreach (var child2 in child.children) {
                        CheckType<ExpressionInteger> (child2);
                    }
                } else if (typeof (ExpressionAnd) == child.GetType ()) {
                    Assert.True (false);
                }
            }
        }
    }

    public class UnitTestVariable {
        void CheckType<T> (IExpression exp) {
            Assert.Equal (typeof (T), exp.GetType ());
        }

        [Fact]
        public void TestInteger () {
            var gen = new VariableGenerator ();
            var i1 = gen.GenerateInt (1, 3);
            var i2 = gen.GenerateInt (1, 3);
            var i3 = gen.GenerateId ();
            Assert.Equal (6, i3);
        }

        [Fact]
        public void TestIntegerEqual () {
            var gen = new VariableGenerator ();
            var i1 = gen.GenerateInt (1, 9);
            var i2 = gen.GenerateInt (1, 9);
            var expr = i1.EqualCondition (i2);
            Assert.Equal (9, expr.children.Count);
            CheckType<ExpressionOr> (expr);

            foreach (var child in expr.children) {
                Assert.Equal (18, child.children.Count);
                CheckType<ExpressionAnd> (child);
                int count = 0;
                foreach (var child2 in child.children) {
                    CheckType<ExpressionInteger> (child2);
                    var ins = (ExpressionInteger) child2;
                    if (ins.negative) {
                        count++;
                    }
                }
                Assert.Equal (2, count);
            }
        }
    }

    public class UnitTestExpression {

        public ExpressionAnd GenerateTestAnd () {
            var expr = new ExpressionAnd ();

            var expr1 = new ExpressionAnd ();
            expr1.children.Add (new ExpressionInteger (0, true));
            expr1.children.Add (new ExpressionInteger (1, false));
            expr.children.Add (expr1);

            var expr2 = new ExpressionAnd ();
            expr2.children.Add (new ExpressionInteger (2, true));
            expr2.children.Add (new ExpressionInteger (3, false));
            expr.children.Add (expr2);

            return expr;
        }

        public ExpressionOr GenerateTestOr () {
            var expr = new ExpressionOr ();

            var expr1 = new ExpressionOr ();
            expr1.children.Add (new ExpressionInteger (0, true));
            expr1.children.Add (new ExpressionInteger (1, false));
            expr.children.Add (expr1);

            var expr2 = new ExpressionOr ();
            expr2.children.Add (new ExpressionInteger (2, true));
            expr2.children.Add (new ExpressionInteger (3, false));
            expr.children.Add (expr2);
            return expr;
        }

        public ExpressionAnd GenerateTestAndOr () {
            var expr = new ExpressionAnd ();

            var expr1 = new ExpressionOr ();
            expr1.children.Add (new ExpressionInteger (0, true));
            expr1.children.Add (new ExpressionInteger (1, false));
            expr.children.Add (expr1);

            var expr2 = new ExpressionOr ();
            expr2.children.Add (new ExpressionInteger (2, true));
            expr2.children.Add (new ExpressionInteger (3, false));
            expr.children.Add (expr2);
            return expr;
        }

        void CheckType<T> (IExpression exp) {
            Assert.Equal (typeof (T), exp.GetType ());
        }

        [Fact]
        public void TestExpressionInteger () {
            var expr = new ExpressionInteger (0, true);
            Assert.True (expr.id == 0);
            Assert.True (expr.negative);
        }

        [Fact]
        public void TextExpressionAndFlatten () {
            var expr = GenerateTestAnd ();
            var flatten = expr.Flatten ();
            Assert.Equal (4, flatten.children.Count);
            for (int i = 0; i < flatten.children.Count; i++) {
                CheckType<ExpressionInteger> (flatten.children[i]);
                ExpressionInteger subexpr = (ExpressionInteger) flatten.children[i];
                Assert.Equal (i, subexpr.id);
            }
        }

        [Fact]
        public void TextExpressionOrFlatten () {
            var expr = GenerateTestOr ();
            var flatten = expr.Flatten ();
            Assert.Equal (4, flatten.children.Count);
            for (int i = 0; i < flatten.children.Count; i++) {
                CheckType<ExpressionInteger> (flatten.children[i]);
                ExpressionInteger subexpr = (ExpressionInteger) flatten.children[i];
                Assert.Equal (i, subexpr.id);
            }
        }

        [Fact]
        public void TextExpressionAndOrFlatten () {
            var expr = GenerateTestAndOr ();
            var flatten = expr.Flatten ();
            Assert.Equal (2, flatten.children.Count);
            CheckType<ExpressionAnd> (expr);
        }

        [Fact]
        public void TestNegate () {
            var expr = GenerateTestAndOr ();
            var negExpr = expr.Negate ();
            int index = 0;
            bool[] expected = { false, true, false, true };

            CheckType<ExpressionOr> (negExpr);
            foreach (var child in negExpr.children) {
                CheckType<ExpressionAnd> (child);
                foreach (var child2 in child.children) {
                    CheckType<ExpressionInteger> (child2);
                    var subexpr = (ExpressionInteger) child2;
                    Assert.Equal (expected[index], subexpr.negative);
                    Assert.Equal (index, subexpr.id);
                    index++;
                }
            }
        }

        [Fact]
        public void TestEmpty () {
            var expOr = new ExpressionOr ();
            var emptyOr = expOr.Empty ();
            CheckType<ExpressionOr> (emptyOr);

            var expAnd = new ExpressionAnd ();
            var emptyAnd = expAnd.Empty ();
            CheckType<ExpressionAnd> (emptyAnd);
        }
    }

    public class UnitTestLoader {
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