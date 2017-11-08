using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    public abstract class Value
    {
        public enum Operator
        {
            Set,
            IndexerGet,
            IndexerSet,
            UnaryPlus,
            UnaryMinus,
            BinaryPlus,
            BinaryMinus,
            Multiply,
            Divide,
            Degree,
            Equal,
            NotEqual,
            Less,
            LessOrEqual,
            Greater,
            GreaterOrEqual,
            LogicalAnd,
            LogicalOr
        }

        private static readonly List<OperationProperties> OperationPropertiesList = new List<OperationProperties>()
        {
            //Bool methods
            new OperationProperties(ValueType.Bool, Operator.Set, ValueType.Bool, ValueType.Bool),
            new OperationProperties(ValueType.Bool, Operator.Equal, ValueType.Bool, ValueType.Bool),
            new OperationProperties(ValueType.Bool, Operator.NotEqual, ValueType.Bool, ValueType.Bool),
            new OperationProperties(ValueType.Bool, Operator.LogicalAnd, ValueType.Bool, ValueType.Bool),
            new OperationProperties(ValueType.Bool, Operator.LogicalOr, ValueType.Bool, ValueType.Bool),
            //Char methods
            new OperationProperties(ValueType.Char, Operator.Set, ValueType.Char, ValueType.Char),
            new OperationProperties(ValueType.Bool, Operator.Equal, ValueType.Char, ValueType.Char),
            new OperationProperties(ValueType.Bool, Operator.NotEqual, ValueType.Char, ValueType.Char),
            //Double methods
            new OperationProperties(ValueType.Double, Operator.Set, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Double, Operator.Set, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Double, Operator.Set, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Double, Operator.UnaryPlus, ValueType.Double),
            new OperationProperties(ValueType.Double, Operator.UnaryMinus, ValueType.Double),
            new OperationProperties(ValueType.Double, Operator.BinaryPlus, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Double, Operator.BinaryPlus, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Double, Operator.BinaryMinus, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Double, Operator.BinaryMinus, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Double, Operator.Multiply, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Double, Operator.Multiply, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Double, Operator.Divide, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Double, Operator.Divide, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Double, Operator.Degree, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Double, Operator.Degree, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.Equal, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.NotEqual, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.Less, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.LessOrEqual, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.Greater, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.GreaterOrEqual, ValueType.Double, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.Equal, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.NotEqual, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.Less, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.LessOrEqual, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.Greater, ValueType.Double, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.GreaterOrEqual, ValueType.Double, ValueType.Int),
            //Int methods
            new OperationProperties(ValueType.Int, Operator.Set, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Int, Operator.UnaryPlus, ValueType.Int),
            new OperationProperties(ValueType.Int, Operator.UnaryMinus, ValueType.Int),
            new OperationProperties(ValueType.Int, Operator.BinaryPlus, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Double, Operator.BinaryPlus, ValueType.Int, ValueType.Double),
            new OperationProperties(ValueType.Int, Operator.BinaryMinus, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Double, Operator.BinaryMinus, ValueType.Int, ValueType.Double),
            new OperationProperties(ValueType.Int, Operator.Multiply, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Double, Operator.Multiply, ValueType.Int, ValueType.Double),
            new OperationProperties(ValueType.Int, Operator.Divide, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Double, Operator.Divide, ValueType.Int, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.Equal, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.NotEqual, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.Less, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.LessOrEqual, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.Greater, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.GreaterOrEqual, ValueType.Int, ValueType.Int),
            new OperationProperties(ValueType.Bool, Operator.Equal, ValueType.Int, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.NotEqual, ValueType.Int, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.Less, ValueType.Int, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.LessOrEqual, ValueType.Int, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.Greater, ValueType.Int, ValueType.Double),
            new OperationProperties(ValueType.Bool, Operator.GreaterOrEqual, ValueType.Int, ValueType.Double),
            //String methods
            new OperationProperties(ValueType.String, Operator.Set, ValueType.String, ValueType.String),
            new OperationProperties(ValueType.Char, Operator.IndexerGet, ValueType.String, ValueType.Int),
            new OperationProperties(ValueType.String, Operator.BinaryPlus, ValueType.String, ValueType.String),
            new OperationProperties(ValueType.String, Operator.BinaryPlus, ValueType.String, ValueType.Char),
            new OperationProperties(ValueType.Bool, Operator.Equal, ValueType.String, ValueType.String),
            new OperationProperties(ValueType.Bool, Operator.NotEqual, ValueType.String, ValueType.String)
        };

        public readonly ValueType Type;

        protected Value(ValueType type)
        {
            Type = type;
        }

        public abstract Value Set(Value value);

        //All methods are virtual, becouse must not be implemented in child class

        public virtual Value this[Value index]
        {
            get
            {
                throw new ValueException(ValueException.ErrorType.InvalidIndexer, Type.ToString());
            }
            set
            {
                throw new ValueException(ValueException.ErrorType.InvalidIndexer, Type.ToString());
            }
        }

        public virtual Value Add()
        {
            throw BuildInvalidUnaryException(this, Operator.UnaryPlus);
        }

        public virtual Value Subtract()
        {
            throw BuildInvalidUnaryException(this, Operator.UnaryMinus);
        }

        public virtual Value Add(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.BinaryPlus);
        }

        public virtual Value Subtract(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.BinaryMinus);
        }

        public virtual Value Multiply(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.Multiply);
        }

        public virtual Value Divide(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.Divide);
        }

        public virtual Value Power(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.Degree);
        }

        public virtual Value IsLess(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.Less);
        }

        public virtual Value IsLessOrEqual(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.LessOrEqual);
        }

        public virtual Value IsGreater(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.Greater);
        }

        public virtual Value IsGreaterOrEqual(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.GreaterOrEqual);
        }

        public virtual Value IsEqual(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.Equal);
        }

        public virtual Value IsNotEqual(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.NotEqual);
        }

        public virtual Bool LogicalAnd(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.LogicalAnd);
        }

        public virtual Bool LogicalOr(Value value)
        {
            CheckForNull(value);
            throw BuildInvalidBinaryException(this, value, Operator.LogicalOr);
        }

        public static ValueType GetResultValueType(Operator valueOperator, params ValueType[] parameters)
        {
            if (parameters.Length <= 0)
            {
                throw new ArgumentException();
            }

            if (parameters[0].SimpleType == ValueType.Type.Array)
            {
                switch (valueOperator)
                {
                    case Operator.Set:
                        if (parameters.Length == 2 && ValueType.Equals(parameters[0], parameters[1]))
                        {
                            return parameters[0];
                        }
                        break;
                    case Operator.IndexerGet:
                        if (parameters.Length == 2 && ValueType.Equals(parameters[1], ValueType.Int))
                        {
                            return parameters[0].InternalType;
                        }
                        break;
                    case Operator.IndexerSet:
                        if (parameters.Length == 3 && ValueType.Equals(parameters[1], ValueType.Int) &&
                            ValueType.Equals(parameters[0].InternalType, parameters[2]))
                        {
                            return parameters[0].InternalType;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                foreach (OperationProperties property in OperationPropertiesList)
                {
                    if (property.Operator != valueOperator || property.Parameters.Length != parameters.Length)
                    {
                        continue;
                    }

                    bool isEquals = true;

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        isEquals = isEquals && ValueType.Equals(property.Parameters[i], parameters[i]);
                    }

                    if (isEquals)
                    {
                        return property.ResultType;
                    }
                }
            }

            return null;
        }

        public virtual bool Equals(Value value)
        {
            throw new NotImplementedException();
        }

        protected static ValueException BuildInvalidBinaryException
            (Value first, Value second, Operator valueOperator)
        {
            return new ValueException(ValueException.ErrorType.InvalidBinaryOperator,
                valueOperator, first.Type.ToString(), second.Type.ToString());
        }

        protected static ValueException BuildInvalidUnaryException
            (Value first, Operator valueOperator)
        {
            return new ValueException(ValueException.ErrorType.InvalidUnaryOperator,
                valueOperator, first.Type.ToString());
        }

        protected void CheckForNull(Value value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
        }
    }
}
