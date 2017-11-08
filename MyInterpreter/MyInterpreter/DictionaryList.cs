using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public sealed class DictionaryList<TKey, TValue> : IVariableToValue<TKey, TValue>
    {
        public readonly List<Dictionary<TKey, TValue>> Dictionaries = new List<Dictionary<TKey, TValue>>()
        {
        };

        public DictionaryList()
        {
        }

        public DictionaryList(Dictionary<TKey, TValue> dictionary)
        {
            this.Dictionaries.Add(dictionary);
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (TryGetValue(key, out value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                for (int i = Dictionaries.Count - 1; i >= 0; i--)
                {
                    if (Dictionaries[i].ContainsKey(key))
                    {
                        Dictionaries[i][key] = value;
                        return;
                    }
                }
                throw new KeyNotFoundException();
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            for (int i = Dictionaries.Count - 1; i >= 0; i--)
            {
                if (Dictionaries[i].TryGetValue(key, out value))
                {
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        public bool ContainsKeyInLastDictionary(TKey key)
        {
            return Dictionaries[Dictionaries.Count - 1].ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            if (Dictionaries.Count < 1)
            {
                throw new InvalidOperationException("Список словарей пуст");
            }
            Dictionaries[Dictionaries.Count - 1].Add(key, value);
        }

        public void AddDictionary()
        {
            Dictionaries.Add(new Dictionary<TKey, TValue>());
        }

        public void RemoveDictionary()
        {
            if (Dictionaries.Count < 1)
            {
                throw new InvalidOperationException("Список словарей пуст");
            }
            Dictionaries.RemoveAt(Dictionaries.Count - 1);
        }
    }
}
