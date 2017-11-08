using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class Assignment : Statement
    {
        public readonly Expression UnaryExpr;
        public readonly Expression Expr;

        public Assignment(Expression unaryExpr, Expression expr, Position position)
            : base(position)
        {
            if (unaryExpr == null || expr == null)
            {
                throw new ArgumentNullException();
            }
            if (!(unaryExpr is Variable || unaryExpr is Slice))
            {
                throw new ArgumentException("The left-hand side of an assignment must be a variable or indexer");
            }
            UnaryExpr = unaryExpr;
            Expr = expr;
        }
    }
}
