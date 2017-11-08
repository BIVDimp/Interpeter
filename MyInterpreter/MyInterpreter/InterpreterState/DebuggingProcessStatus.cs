using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyInterpreter.AST;

namespace MyInterpreter
{
    public sealed class DebuggingProcessStatus : BaseInterpreterStatus
    {
        public readonly Statement CurrentStatement;
        public readonly List<KeyValuePair<string, string>> Variables;

        internal DebuggingProcessStatus(Statement statement, DictionaryList<string, Pair<Value, Variable.Type>> variables)
        {
            if (statement == null)
            {
                throw new ArgumentNullException();
            }

            this.CurrentStatement = statement;
            this.Variables = ToList(variables);
        }

        private List<KeyValuePair<string, string>> ToList(DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            List<KeyValuePair<string, string>> answer = new List<KeyValuePair<string, string>>();
            string suffix;

            for (int i = 0; i < variableToValue.Dictionaries.Count; i++)
            {
                suffix = new string('.', i);
                foreach (KeyValuePair<string, Pair<Value, Variable.Type>> pair in variableToValue.Dictionaries[i])
                {
                    answer.Add(new KeyValuePair<string, string>(suffix + pair.Key,
                        pair.Value.Second == Variable.Type.Assigned ? pair.Value.First.ToString() : string.Empty));
                }
            }

            return answer;
        }
    }
}
