using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace sudoku_sat_solver {
    public abstract class IExpression {
        public abstract List<IExpression> children {
            get;
        }

        // 否定を内部に伝播させる
        public abstract IExpression Negate ();

        public abstract IExpression Flatten ();

        public abstract IExpression Empty ();
    }

    public class ExpressionInteger : IExpression {

        public ExpressionInteger (Int32 id, bool positive) {
            this.id = id;
            this.positive = positive;
        }

        public ExpressionInteger (ExpressionInteger src) {
            this.id = src.id;
            this.positive = src.positive;
        }

        public ExpressionInteger Clone () {
            return new ExpressionInteger (this);
        }

        public override List<IExpression> children {
            get {
                return null;
            }
        }

        public override IExpression Negate () {
            var ret = Clone ();
            ret.positive = !ret.positive;
            return ret;
        }

        public override IExpression Flatten () {
            return Clone ();
        }

        public override IExpression Empty () {
            throw new Exception ("not supported for ExpressionInteger empty");
        }

        public Int32 id;
        public bool positive;
    }

    public class ExpressionOr : IExpression {

        public ExpressionOr () {
            vars = new List<IExpression> ();
        }

        public override List<IExpression> children {
            get {
                return vars;
            }
        }

        public override IExpression Empty () { return new ExpressionOr (); }
        public override IExpression Negate () {
            var expr = new ExpressionAnd ();
            foreach (var child in children) {
                expr.children.Add (child.Negate ());
            }
            return expr;
        }

        public override IExpression Flatten () {
            var ret = new ExpressionOr ();
            foreach (var child in children) {
                var flatten = child.Flatten ();
                if (GetType () == child.GetType ()) {
                    foreach (var child2 in flatten.children) {
                        ret.children.Add (child2);
                    }
                } else {
                    ret.children.Add (flatten);
                }
            }
            return ret;
        }

        private List<IExpression> vars;
    }

    public class ExpressionAnd : IExpression {

        public ExpressionAnd () {
            vars = new List<IExpression> ();
        }

        public override List<IExpression> children {
            get {
                return vars;
            }
        }

        public override IExpression Empty () { return new ExpressionAnd (); }

        public override IExpression Negate () {
            var expr = new ExpressionOr ();
            foreach (var child in children) {
                expr.children.Add (child.Negate ());
            }
            return expr;
        }

            public override IExpression Flatten () {
            var ret = new ExpressionAnd ();
            foreach (var child in children) {
                var flatten = child.Flatten ();
                if (GetType () == child.GetType ()) {
                    foreach (var child2 in flatten.children) {
                        ret.children.Add (child2);
                    }
                } else {
                    ret.children.Add (flatten);
                }
            }
            return ret;
        }

        private List<IExpression> vars;
    }
}