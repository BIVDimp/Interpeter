using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter.AST
{
    internal class Declaration : Statement
    {
        public readonly ValueType ValueType;
        public readonly Variable DeclVariable;
        public readonly Expression Expr;

        public Declaration(ValueType valueType, Variable declaredVariable, Expression expr, Position position)
            : base(position)
        {
            if (declaredVariable == null)
            {
                throw new ArgumentNullException();
            }
            ValueType = valueType;
            DeclVariable = declaredVariable;
            Expr = expr;
        }
    }
}