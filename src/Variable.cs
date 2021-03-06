using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace sudoku_sat_solver {

    public class VariableInteger {

        public VariableInteger (Int64 lowerbound, List<Int32> vars, Int32 id) {
            this.variables = vars;
            this.lowerbound = lowerbound;
            this.id = id;
        }

        // one-hot encode された整数を decode する 
        public Int64 Decode (List<bool> values) {
            for (int i = 0; i < variables.Count; i++) {
                // i番目の bool 変数が true なら、その変数は (i+1) である
                if (values[variables[i]]) {
                    return lowerbound + i;
                }
            }
            throw new Exception ("no value to decode");
        }

        // 値域の中のどれか一つは取れるという制約を出力
        public IExpression AssignAny () {
            var ret = new ExpressionOr ();
            for (int i = 0; i < variables.Count; i++) {
                var item = new ExpressionAnd ();
                // 下から i 番目の値が採用されるような割当て
                for (int j = 0; j < variables.Count; j++) {
                    if (i == j) {
                        item.children.Add (new ExpressionInteger (variables[j], true));
                    } else {
                        item.children.Add (new ExpressionInteger (variables[j], false));
                    }
                }
                ret.children.Add (item);
            }
            return ret;
        }

        // this == var が成立するような expression を返す
        // (this == 1 && var == 1) || (this == 2 && var == 2) || ...
        public IExpression EqualCondition (VariableInteger var) {
            Debug.Assert (var.lowerbound == lowerbound);
            Debug.Assert (var.variables.Count == variables.Count);

            var ret = new ExpressionOr ();
            for (int i = 0; i < variables.Count; i++) {
                var item = new ExpressionAnd ();
                for (int j = 0; j < variables.Count; j++) {
                    if (i == j) {
                        item.children.Add (new ExpressionInteger (variables[j], true));
                        item.children.Add (new ExpressionInteger (var.variables[j], true));
                    } else {
                        item.children.Add (new ExpressionInteger (variables[j], false));
                        item.children.Add (new ExpressionInteger (var.variables[j], false));
                    }
                }
                ret.children.Add (item);
            }
            return ret;
        }

        public IExpression NotEqualCondition (VariableInteger var) {
            return EqualCondition (var).Negate ();
        }

        // 変数が var という値を取る制約
        public IExpression EqualCondition (Int32 var) {
            Debug.Assert (lowerbound <= var && var < lowerbound + variables.Count);

            var expr = new ExpressionAnd ();
            for (int i = 0; i < variables.Count; i++) {
                if (lowerbound + i == var) {
                    expr.children.Add (new ExpressionInteger (variables[i], true));
                } else {
                    expr.children.Add (new ExpressionInteger (variables[i], false));
                }
            }
            return expr;
        }

        public IExpression NotEqualCondition (Int32 var) {
            return EqualCondition (var).Negate ();
        }

        public List<Int32> variables;
        public Int64 lowerbound;

        public Int32 id;
    }

    // 整数を表現する bool 変数集合
    // 制約式を生成できる
    public class VariableGenerator {

        public VariableGenerator () {
            baseId = 1;
        }

        public VariableInteger GenerateInt (Int32 lowerbound, Int32 upperbound) {
            var ids = new List<Int32> ();
            for (int id = 0; id <= upperbound - lowerbound; id++) {
                ids.Add (baseId);
                baseId++;
            }
            var ret = new VariableInteger (lowerbound, ids, baseId);
            return ret;
        }

        public Int32 VariableCount {
            get {
                return baseId;
            }
        }

        public Int32 GenerateId () {
            var result = baseId;
            baseId++;
            return result;
        }

        private Int32 baseId;
    }
}