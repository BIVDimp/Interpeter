using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal sealed class Slice : Expression
    {
        public readonly Expression Collection;
        public readonly Expression Indexer;

        public Slice(Expression collection, Expression indexer, Position position)
            : base(position)
        {
            if (collection == null || indexer == null)
            {
                throw new ArgumentNullException();
            }
            Collection = collection;
            Indexer = indexer;
        }

        internal override Value Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue, params Value[] values)
        {
            if (values.Length != 2)
            {
                throw new ArgumentException();
            }

            Value collectionValue = values[0];
            Value indexerValue = values[1];

            try
            {
                return collectionValue[indexerValue];
            }
            catch (ValueException exception)
            {
                throw InterpretException.BuildException(exception, Position);
            }
        }
    }
}
