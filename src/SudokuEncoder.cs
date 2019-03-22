using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace sudoku_sat_solver {

    public class SudokuEncoder {

        public SudokuEncoder (Int32 size) {
            generator = new VariableGenerator ();
            this.size = size;
            vars = new List<List<VariableInteger>> ();
            for (int i = 0; i < size; i++) {
                vars.Add (new List<VariableInteger> ());
                for (int j = 0; j < size; j++) {
                    vars[i].Add (generator.GenerateInt (1, size));
                }
            }
        }

        private IExpression DifferentAll (List<VariableInteger> variables) {
            var expr = new ExpressionAnd ();
            foreach (var v1 in variables) {
                foreach (var v2 in variables) {
                    if (v1.id != v2.id) {
                        expr.children.Add (v1.NotEqualCondition (v2));
                    }
                }
            }
            return expr;
        }

        public ExpressionAnd encode (List<List<Int32>> board) {
            Debug.Assert (board.Count == size);
            foreach (var line in board) {
                Debug.Assert (line.Count == size);
            }

            var ret = new ExpressionAnd ();

            // 初期配置
            for (int col = 0; col < size; col++) {
                for (int row = 0; row < size; row++) {
                    if (board[col][row] > 0) {
                        ret.children.Add (vars[col][row].EqualCondition (board[col][row]));
                    }
                }
            }

            // どれかの値は割り当てられる
            for (int col = 0; col < size; col++) {
                for (int row = 0; row < size; row++) {
                    ret.children.Add (vars[col][row].AssignAny ());
                }
            }

            // 縦が全て異なる値
            for (int col = 0; col < size; col++) {
                var lst = new List<VariableInteger> ();
                for (int row = 0; row < size; row++) {
                    lst.Add (vars[col][row]);
                }
                ret.children.Add (DifferentAll (lst));
            }

            // 横が全て異なる値
            for (int row = 0; row < size; row++) {
                var lst = new List<VariableInteger> ();
                for (int col = 0; col < size; col++) {
                    lst.Add (vars[col][row]);
                }
                ret.children.Add (DifferentAll (lst));
            }

            Int32 subsize = (Int32) Math.Round (Math.Sqrt (size));
            // 各 grid 内が全て異なる値
            for (int gcol = 0; gcol < subsize; gcol++) {
                for (int grow = 0; grow < subsize; grow++) {
                    var lst = new List<VariableInteger> ();
                    for (int lcol = 0; lcol < subsize; lcol++) {
                        for (int lrow = 0; lrow < subsize; lrow++) {
                            var col = gcol * subsize + lcol;
                            var row = grow * subsize + lrow;
                            lst.Add (vars[col][row]);
                        }
                    }
                    ret.children.Add (DifferentAll (lst));
                }
            }

            var ret2 = new ExpressionAnd ();
            // 一番浅い部分は省略する必要がないので、いらない
            foreach (var child in ret.Flatten ().children) {
                if (child.GetType () == typeof (ExpressionInteger)) {
                    ret2.children.Add (child);
                } else {
                    var simpleChild = TseitinTranslate (child, ret2);
                    ret2.children.Add (simpleChild);
                }
            }
            return ret2;
        }

        // expr を簡約した結果を返す
        // 新規変数を追加する時に生じた制約は、result に入れる
        public IExpression TseitinTranslate (IExpression expr, ExpressionAnd newExpr) {
            var ret = expr.Empty ();
            foreach (var child in expr.children) {
                if (child.GetType () == typeof (ExpressionInteger)) {
                    ret.children.Add (child);
                } else {
                    var childExpr = TseitinTranslate (child, newExpr);

                    // childExpr を newId の変数で置き換え
                    var newId = generator.GenerateId ();
                    ret.children.Add (new ExpressionInteger (newId, true));

                    // childExpr == newId の制約
                    var newChildExpr = new ExpressionInteger (newId, false);
                    if (childExpr.GetType () == typeof (ExpressionAnd)) {
                        foreach (var subexpr in childExpr.children) {
                            var newSubExpr = new ExpressionOr ();
                            newSubExpr.children.Add (newChildExpr.Clone ());
                            newSubExpr.children.Add (subexpr);
                            newExpr.children.Add (newSubExpr);
                        }
                    } else { // childExpr.GetType() == typeof (ExpressionOr)
                        childExpr.children.Add (newChildExpr);
                        newExpr.children.Add (childExpr);
                    }
                }
            }
            return ret;
        }

        public Int32 VariableCount {
            get {
                return generator.VariableCount;
            }
        }

        private VariableGenerator generator;
        private List<List<VariableInteger>> vars;
        private Int32 size;
    }

    public class SudokuDecoder {

    }
}