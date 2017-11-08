using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyInterpreter.AST;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    partial class Parser
    {
        #region Initialize Dictionaries
        private static readonly Dictionary<BinaryOperationType, Value.Operator> binaryOperationToValueOperator =
            new Dictionary<BinaryOperationType, Value.Operator>() 
            {
                { BinaryOperationType.Plus, Value.Operator.BinaryPlus },
                { BinaryOperationType.Minus, Value.Operator.BinaryMinus },
                { BinaryOperationType.Multiply, Value.Operator.Multiply },
                { BinaryOperationType.Divide, Value.Operator.Divide },
                { BinaryOperationType.Degree, Value.Operator.Degree },
                { BinaryOperationType.LogicalAnd, Value.Operator.LogicalAnd },
                { BinaryOperationType.LogicalOr, Value.Operator.LogicalOr }
            };

        private static readonly Dictionary<CompareOperator, Value.Operator> compareOperatorToValueOperator =
            new Dictionary<CompareOperator, Value.Operator>() 
            {
                { CompareOperator.IsEqual, Value.Operator.Equal },
                { CompareOperator.IsNotEqual, Value.Operator.NotEqual },
                { CompareOperator.IsLess, Value.Operator.Less },
                { CompareOperator.IsLessOrEqual, Value.Operator.LessOrEqual },
                { CompareOperator.IsGreater, Value.Operator.Greater },
                { CompareOperator.IsGreaterOrEqual, Value.Operator.GreaterOrEqual }
            };

        private static readonly Dictionary<UnaryOperationType, Value.Operator> unaryOperationToValueOperator =
            new Dictionary<UnaryOperationType, Value.Operator>() 
            {
                { UnaryOperationType.Plus, Value.Operator.UnaryPlus },
                { UnaryOperationType.Minus, Value.Operator.UnaryMinus }
            };
        #endregion

        //Node nextNode = program.NodeList.Find(node => node is Declaration) ??
        //    program.NodeList.Find(node => node is Function && (node as Function).Signature.Equals(mainFunctionSignature));
        private readonly DictionaryList<string, ValueType> variableToValueType = new DictionaryList<string, ValueType>();
        private readonly Dictionary<Expression, ValueType> expressionToValueType = new Dictionary<Expression, ValueType>();

        private void HandleJumpStatements()
        {
            if (program == null)
            {
                return;
            }

            Dictionary<string, Statement> labelNameToStatement = new Dictionary<string, Statement>();

            foreach (Node node in program.NodeList)
            {
                if (node is Function)
                {
                    HandleJumpsInBlock((node as Function).Block, node as Function, null, labelNameToStatement);
                }
            }
        }

        private void HandleJumpsInBlock(Block curBlock, Function curFunction, IterationStatement curIteration,
            Dictionary<string, Statement> labelNameToStatement)
        {
            List<string> newLabelList = new List<string>();
            CollectLabels(curBlock, labelNameToStatement, newLabelList);

            for (int i = 0; i < curBlock.StatementList.Count; i++)
            {
                HandleJumpsInStatement(curBlock.StatementList[i], curFunction,
                            curIteration, labelNameToStatement);
            }

            foreach (string name in newLabelList)
            {
                labelNameToStatement.Remove(name);
            }
        }

        private void HandleJumpsInStatement(Statement statement, Function curFunction, IterationStatement curIteration,
            Dictionary<string, Statement> labelNameToStatement)
        {
            if (statement is Block)
            {
                HandleJumpsInBlock(statement as Block, curFunction,
                    curIteration, labelNameToStatement);
            }
            else
            {
                HandleJumpsInSimpleStatement(statement, curFunction,
                    curIteration, labelNameToStatement);
            }
        }

        private void HandleJumpsInSimpleStatement(Statement statement, Function curFunction, IterationStatement curIteration,
            Dictionary<string, Statement> labelNameToStatement)
        {
            if (statement is Goto)
            {
                Goto go = statement as Goto;
                if (!labelNameToStatement.ContainsKey(go.GotoLabel.Name))
                {
                    errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.LabelDoesNotExist, go.GotoLabel.Position)));
                }
                else
                {
                    go.NextStatement = labelNameToStatement[go.GotoLabel.Name];
                }
            }
            else if (statement is Break)
            {
                Break breakStatement = statement as Break;

                if (curIteration == null)
                {
                    errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.NoEnclosingLoop, breakStatement.Position)));
                }
                else
                {
                    breakStatement.IterationStatement = curIteration;
                }
            }
            else if (statement is Return)
            {
                Return returnStatement = statement as Return;
                returnStatement.Function = curFunction;
            }
            else if (statement is If)
            {
                If ifStatement = statement as If;

                HandleJumpsInStatement(ifStatement.TrueStatement, curFunction,
                        curIteration, labelNameToStatement);

                if (ifStatement.FalseStatement != null)
                {
                    HandleJumpsInStatement(ifStatement.FalseStatement, curFunction,
                            curIteration, labelNameToStatement);
                }
            }
            else if (statement is IterationStatement)
            {
                IterationStatement newIteration = statement as IterationStatement;
                HandleJumpsInStatement(newIteration.Body, curFunction,
                        (statement as IterationStatement), labelNameToStatement);
            }
        }

        private void CollectLabels(Block curBlock, Dictionary<string, Statement> labelNameToStatement, List<string> newLabelList)
        {
            foreach (Statement statement in curBlock.StatementList)
            {
                foreach (Label label in statement.Labels)
                {
                    if (labelNameToStatement.ContainsKey(label.Name))
                    {
                        errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.LabelDuplicate, label.Position)));
                        continue;
                    }
                    else
                    {
                        labelNameToStatement.Add(label.Name, statement);
                        newLabelList.Add(label.Name);
                    }
                }
            }
        }

        private void HandleFunctionCallAndCheckType()
        {
            if (program == null)
            {
                return;
            }

            variableToValueType.AddDictionary();

            foreach (Node node in program.NodeList)
            {
                if (node is Declaration)
                {
                    HandleDeclaration(node as Declaration);
                }
            }

            foreach (Node node in program.NodeList)
            {
                if (node is Function)
                {
                    variableToValueType.AddDictionary();
                    HandleFunction(node as Function);
                    variableToValueType.RemoveDictionary();
                }
            }

            variableToValueType.RemoveDictionary();
        }
        //+
        private void HandleDeclaration(Declaration declaration)
        {
            ValueType exprType;

            if (variableToValueType.ContainsKeyInLastDictionary(declaration.DeclVariable.Name))
            {
                errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.VariableAlreadyExists,
                    declaration.DeclVariable.Name, declaration.DeclVariable.Position)));
                return;
            }

            if (declaration.Expr == null)
            {
                variableToValueType.Add(declaration.DeclVariable.Name, declaration.ValueType);
                return;
            }

            HandleExpression(declaration.Expr);

            if (!expressionToValueType.TryGetValue(declaration.Expr, out exprType))
            {
                throw new NotImplementedException();
            }
            if (exprType != null && Value.GetResultValueType(Value.Operator.Set, declaration.ValueType, exprType) == null)
            {
                errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.ImplicitConversion,
                    new string[] { exprType.ToString(), declaration.ValueType.ToString() }, declaration.Position)));
            }
            else
            {
                variableToValueType.Add(declaration.DeclVariable.Name, declaration.ValueType);
            }
        }
        //+
        private void HandleBlock(Block block)
        {
            foreach (Statement statement in block.StatementList)
            {
                HandleStatement(statement);
            }
        }
        //+
        private void HandleAssignment(Assignment assignment)
        {
            if (assignment.UnaryExpr is Slice)
            {
                Slice slice = assignment.UnaryExpr as Slice;
                ValueType collectionType, indexerType, exprType, resultType = null;
                HandleExpression(slice.Collection);
                HandleExpression(slice.Indexer);
                HandleExpression(assignment.Expr);

                if (!expressionToValueType.TryGetValue(slice.Collection, out collectionType)
                    || !expressionToValueType.TryGetValue(slice.Indexer, out indexerType)
                    || !expressionToValueType.TryGetValue(assignment.Expr, out exprType))
                {
                    throw new NotImplementedException();
                }

                if (collectionType != null && indexerType != null && exprType != null)
                {
                    resultType =
                        Value.GetResultValueType(Value.Operator.IndexerSet, collectionType, indexerType, exprType);
                    if (resultType == null)
                    {
                        errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.InvalidSetIndexer,
                            new string[] { collectionType.ToString(), indexerType.ToString(), exprType.ToString() },
                            assignment.Position)));
                    }
                }
            }
            else
            {
                ValueType unaryExprType, exprType;
                HandleExpression(assignment.UnaryExpr);
                HandleExpression(assignment.Expr);

                if (!expressionToValueType.TryGetValue(assignment.UnaryExpr, out unaryExprType)
                    || !expressionToValueType.TryGetValue(assignment.Expr, out exprType))
                {
                    throw new NotImplementedException();
                }

                if (unaryExprType != null && exprType != null
                    && Value.GetResultValueType(Value.Operator.Set, unaryExprType, exprType) == null)
                {
                    errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.ImplicitConversion,
                        new string[] { exprType.ToString(), unaryExprType.ToString() }, assignment.Position)));
                }
            }
        }
        //+
        private void HandleDoWhile(DoWhile doWhile)
        {
            HandleStatement(doWhile.Body);
            HandleStatement(doWhile.LoopCondition);
        }
        //+
        private void HandleFor(For forStatement)
        {
            variableToValueType.AddDictionary();
            HandleStatement(forStatement.Initializer);
            HandleStatement(forStatement.LoopCondition);
            HandleStatement(forStatement.Body);
            HandleStatement(forStatement.Iterator);
            variableToValueType.RemoveDictionary();
        }
        //+
        private void HandleStatement(Statement statement)
        {
            if (statement is Assignment)
            {
                HandleAssignment(statement as Assignment);
            }
            else if (statement is Declaration)
            {
                HandleDeclaration(statement as Declaration);
            }
            else if (statement is Block)
            {
                variableToValueType.AddDictionary();
                HandleBlock(statement as Block);
                variableToValueType.RemoveDictionary();
            }
            else if (statement is DoWhile)
            {
                variableToValueType.AddDictionary();
                HandleDoWhile(statement as DoWhile);
                variableToValueType.RemoveDictionary();
            }
            else if (statement is For)
            {
                variableToValueType.AddDictionary();
                HandleFor(statement as For);
                variableToValueType.RemoveDictionary();
            }
            else if (statement is FunctionCall)
            {
                variableToValueType.AddDictionary();
                HandleFunctionCall(statement as FunctionCall);
                variableToValueType.RemoveDictionary();
            }
            else if (statement is Print)
            {
                HandlePrint(statement as Print);
            }
            else if (statement is If)
            {
                variableToValueType.AddDictionary();
                HandleIf(statement as If);
                variableToValueType.RemoveDictionary();
            }
            else if (statement is While)
            {
                variableToValueType.AddDictionary();
                HandleWhile(statement as While);
                variableToValueType.RemoveDictionary();
            }
            else if (statement is Fork)
            {
                HandleFork(statement as Fork);
            }
            else if (statement is Return)
            {
                HandleReturn(statement as Return);
            }
            else if (statement is Break)
            {
            }
            else if (statement is Goto)
            {
            }
            else if (statement.GetType() == typeof(Statement))
            {
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        //+
        private void HandlePrint(Print print)
        {
            if (print.Parameters.Count <= 0)
            {
                return;
            }
            ValueType exprType;

            foreach (Expression expr in print.Parameters)
            {
                HandleExpression(expr);
            }

            for (int i = 0; i < print.Parameters.Count; i++)
            {
                if (!expressionToValueType.TryGetValue(print.Parameters[i], out exprType))
                {
                    throw new NotImplementedException();
                }
                if (exprType != null && ValueType.Equals(exprType, ValueType.Void))
                {
                    errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.ImplicitConversion,
                        new string[] { ValueType.Void.ToString(), ValueType.String.ToString() }, print.Parameters[i].Position)));
                }
            }

            expressionToValueType.TryGetValue(print.Parameters[0], out exprType);
            if (exprType != null && print.Parameters.Count > 1 && !ValueType.Equals(exprType, ValueType.String))
            {
                errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.ImplicitConversion,
                    new string[] { exprType.ToString(), ValueType.String.ToString() }, print.Parameters[0].Position)));
            }
        }
        //+
        private void HandleReturn(Return returnStatement)
        {
            HandleExpression(returnStatement.Expression);
            ValueType exprType;

            if (!expressionToValueType.TryGetValue(returnStatement.Expression, out exprType))
            {
                throw new NotImplementedException();
            }

            if (exprType != null && !ValueType.Equals(returnStatement.Function.Signature.ReturnType, exprType))
            {
                errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.ImplicitConversion,
                    new string[] { exprType.ToString(), returnStatement.Function.Signature.ReturnType.ToString() }, returnStatement.Position)));
            }
        }
        //+
        private void HandleFunctionCall(FunctionCall functionCall)
        {
            foreach (Expression expr in functionCall.Parameters)
            {
                HandleExpression(expr);
            }

            List<ValueType> signature = new List<ValueType>(functionCall.Parameters.Count);
            bool stopSearching = false;

            for (int i = 0; i < functionCall.Parameters.Count; i++)
            {
                signature.Add(expressionToValueType[functionCall.Parameters[i]]);
                if (signature[i] == null)
                {
                    stopSearching = true;
                }
            }
            if (stopSearching)
            {
                return;
            }

            foreach (Function function in functionList)
            {
                if (IsSignatureEquals(function.Signature, signature, functionCall.Name))
                {
                    functionCall.Function = function;
                    return;
                }
            }

            errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.FunctionNotExist,
                functionCall.Name, functionCall.Position)));
        }

        private bool IsSignatureEquals(FunctionSignature functionSignature, List<ValueType> signature, string name)
        {
            bool isEquals = string.Equals(functionSignature.Name, name, StringComparison.InvariantCulture);
            isEquals = isEquals && signature.Count == functionSignature.Parameters.Count;

            if (!isEquals)
            {
                return false;
            }

            for (int i = 0; i < signature.Count; i++)
            {
                if (!ValueType.Equals(signature[i], functionSignature.Parameters[i].ValueType))
                {
                    return false;
                }
            }
            return true;
        }
        //+
        private void HandleWhile(While whileStatement)
        {
            HandleStatement(whileStatement.LoopCondition);
            HandleStatement(whileStatement.Body);
        }
        //+
        private void HandleIf(If ifStatement)
        {
            HandleStatement(ifStatement.IfCondition);
            HandleStatement(ifStatement.TrueStatement);
            if (ifStatement.FalseStatement != null)
            {
                HandleStatement(ifStatement.FalseStatement);
            }
        }
        //+
        private void HandleFork(Fork fork)
        {
            ValueType condType;
            HandleExpression(fork.Condition);

            if (!expressionToValueType.TryGetValue(fork.Condition, out condType))
            {
                throw new NotImplementedException();
            }
            if (condType != null && !ValueType.Equals(condType, ValueType.Bool))
            {
                errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.ImplicitConversion,
                    new string[] { condType.ToString(), ValueType.Bool.ToString() }, fork.Position)));
            }
        }
        //+
        private void HandleFunction(Function function)
        {
            foreach (Declaration declaration in function.Signature.Parameters)
            {
                variableToValueType.Add(declaration.DeclVariable.Name, declaration.ValueType);
            }
            HandleStatement(function.Block);
        }
        //+
        private void HandleExpression(Expression expression)
        {
            if (expression is ArrayCreator)
            {
                HandleArrayCreator(expression as ArrayCreator);
            }
            else if (expression is BinaryOperation)
            {
                HandleBinaryOperation(expression as BinaryOperation);
            }
            else if (expression is Condition)
            {
                HandleCondition(expression as Condition);
            }
            else if (expression is Constant)
            {
                HandleConstant(expression as Constant);
            }
            else if (expression is Slice)
            {
                HandleSlice(expression as Slice);
            }
            else if (expression is UnaryOperation)
            {
                HandleUnaryOperation(expression as UnaryOperation);
            }
            else if (expression is Variable)
            {
                HandleVariable(expression as Variable);
            }
            else if (expression is Method)
            {
                HandleMethod(expression as Method);
            }
            else if (expression is FunctionCallExpr)
            {
                HandleFunctionCallExpr(expression as FunctionCallExpr);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private void HandleMethod(Method method)
        {
            ValueType exprType;
            HandleExpression(method.Expr);

            if (!expressionToValueType.TryGetValue(method.Expr, out exprType))
            {
                throw new NotImplementedException();
            }

            if (exprType != null && !ValueType.Equals(exprType, ValueType.Double) && !ValueType.Equals(exprType, ValueType.Int))
            {
                errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.ImplicitConversion,
                    new string[] { exprType.ToString(), ValueType.Double.ToString() }, method.Expr.Position)));
            }

            expressionToValueType.Add(method, ValueType.Double);
        }
        //+
        private void HandleVariable(Variable variable)
        {
            ValueType varType;

            if (!variableToValueType.TryGetValue(variable.Name, out varType))
            {
                errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.NotDeclaredVariable,
                    variable.Name, variable.Position)));
            }
            expressionToValueType.Add(variable, varType);
        }
        //+
        private void HandleFunctionCallExpr(FunctionCallExpr functionCallExpr)
        {
            HandleFunctionCall(functionCallExpr.FunctionCall);

            expressionToValueType.Add(functionCallExpr, functionCallExpr.FunctionCall.Function != null ?
                functionCallExpr.FunctionCall.Function.Signature.ReturnType : null);
        }
        //+
        private void HandleUnaryOperation(UnaryOperation unaryOperation)
        {
            ValueType exprType, resultType = null;
            HandleExpression(unaryOperation.Expr);

            if (!expressionToValueType.TryGetValue(unaryOperation.Expr, out exprType))
            {
                throw new NotImplementedException();
            }

            if (exprType != null)
            {
                resultType =
                    Value.GetResultValueType(unaryOperationToValueOperator[unaryOperation.Operation], exprType);
                if (resultType == null)
                {
                    errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.InvalidBinaryOperator,
                        new string[] { ToString(unaryOperationToValueOperator[unaryOperation.Operation]),
                        exprType.ToString() }, unaryOperation.Position)));
                }
            }

            expressionToValueType.Add(unaryOperation, resultType);
        }
        //+
        private void HandleSlice(Slice slice)
        {
            ValueType collectionType, indexerType, resultType = null;
            HandleExpression(slice.Collection);
            HandleExpression(slice.Indexer);

            if (!expressionToValueType.TryGetValue(slice.Collection, out collectionType)
                || !expressionToValueType.TryGetValue(slice.Indexer, out indexerType))
            {
                throw new NotImplementedException();
            }

            if (collectionType != null && indexerType != null)
            {
                resultType =
                    Value.GetResultValueType(Value.Operator.IndexerGet, collectionType, indexerType);
                if (resultType == null)
                {
                    errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.InvalidIndexer,
                        new string[] { collectionType.ToString(), indexerType.ToString() },
                        slice.Position)));
                }
            }

            expressionToValueType.Add(slice, resultType);
        }
        //+
        private void HandleCondition(Condition condition)
        {
            ValueType leftType, rightType, resultType = null;

            if (condition.Operator == CompareOperator.None)
            {
                expressionToValueType.Add(condition, ValueType.Bool);
                return;
            }

            HandleExpression(condition.LeftOperand);
            HandleExpression(condition.RightOperand);

            if (!expressionToValueType.TryGetValue(condition.LeftOperand, out leftType)
                || !expressionToValueType.TryGetValue(condition.RightOperand, out rightType))
            {
                throw new NotImplementedException();
            }

            if (leftType != null && rightType != null)
            {
                resultType =
                    Value.GetResultValueType(compareOperatorToValueOperator[condition.Operator], leftType, rightType);
                if (resultType == null)
                {
                    errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.InvalidCompareOperator,
                        new string[] { ToString(compareOperatorToValueOperator[condition.Operator]),
                        leftType.ToString(), rightType.ToString() }, condition.Position)));
                }
            }

            expressionToValueType.Add(condition, resultType);
        }
        //+
        private void HandleArrayCreator(ArrayCreator arrayCreator)
        {
            ValueType exprType;

            foreach (Expression expr in arrayCreator.Sizes)
            {
                HandleExpression(expr);

                if (!expressionToValueType.TryGetValue(expr, out exprType))
                {
                    throw new NotImplementedException();
                }
                if (exprType != null && !ValueType.Equals(exprType, ValueType.Int))
                {
                    errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.ImplicitConversion,
                        new string[] { exprType.ToString(), ValueType.Int.ToString() }, expr.Position)));
                }
            }

            ValueType arrayType = arrayCreator.ArrayElementsType;
            for (int i = 0; i < arrayCreator.Sizes.Count; i++)
            {
                arrayType = ValueType.Array(arrayType);
            }

            expressionToValueType.Add(arrayCreator, arrayType);
        }
        //+
        private void HandleBinaryOperation(BinaryOperation binaryOperation)
        {
            ValueType leftType, rightType, resultType = null;
            HandleExpression(binaryOperation.LeftOperand);
            HandleExpression(binaryOperation.RightOperand);

            if (!expressionToValueType.TryGetValue(binaryOperation.LeftOperand, out leftType)
                || !expressionToValueType.TryGetValue(binaryOperation.RightOperand, out rightType))
            {
                throw new NotImplementedException();
            }

            if (leftType != null && rightType != null)
            {
                resultType =
                    Value.GetResultValueType(binaryOperationToValueOperator[binaryOperation.Operation], leftType, rightType);
                if (resultType == null)
                {
                    errors.Add(Error.CreateError(new ParserException(ParserException.ErrorType.InvalidBinaryOperator,
                        new string[] { ToString(binaryOperationToValueOperator[binaryOperation.Operation]),
                        leftType.ToString(), rightType.ToString() }, binaryOperation.Position)));
                }
            }

            expressionToValueType.Add(binaryOperation, resultType);
        }
        //+
        private void HandleConstant(Constant constant)
        {
            expressionToValueType.Add(constant, constant.ConstantValue.Type);
        }

        private string ToString(Value.Operator valueOperator)
        {
            return ValueException.OperatorToString[valueOperator];
        }
    }
}
