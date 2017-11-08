using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Interfaces;
using System.Runtime.Serialization;

namespace MiniStudio
{
    internal sealed class XmlCaretaker
    {
        private readonly string filePath;
        private readonly IStorableSettings[] originators;

        public XmlCaretaker(string filePath, params IStorableSettings[] originators)
        {
            if (filePath == null || originators == null)
            {
                throw new ArgumentNullException();
            }
            foreach (IStorableSettings originator in originators)
            {
                if (originator == null)
                {
                    throw new ArgumentNullException();
                }
            }
            this.filePath = filePath;
            this.originators = originators;
        }

        public void SaveState()
        {
            IMemento[] mementos = GatherMementos();
            Type[] mementoTypes = GatherMementoTypes();
            DataContractSerializer serializer = new DataContractSerializer(typeof(IMemento[]), mementoTypes);

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                serializer.WriteObject(writer.BaseStream, mementos);
            }
        }

        public bool LoadState()
        {
            Type[] mementoTypes = GatherMementoTypes();
            DataContractSerializer serializer = new DataContractSerializer(typeof(IMemento[]), mementoTypes);
            IMemento[] mementos;

            if (!TryDeserialize(serializer, out mementos))
            {
                return false;
            }

            return DistributeMementos(mementos);
        }

        private bool TryDeserialize(DataContractSerializer serializer, out IMemento[] mementos)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    mementos = (IMemento[])(serializer.ReadObject(fileStream));
                }
            }
            catch
            {
                mementos = null;
                return false;
            }

            return true;
        }

        private IMemento[] GatherMementos()
        {
            IMemento[] mementos = new IMemento[originators.Length];

            for (int i = 0; i < originators.Length; i++)
            {
                mementos[i] = originators[i].GetMemento();
            }

            return mementos;
        }

        private Type[] GatherMementoTypes()
        {
            Type[] mementoTypes = new Type[originators.Length];

            for (int i = 0; i < originators.Length; i++)
            {
                mementoTypes[i] = originators[i].GetMementoType();
            }

            return mementoTypes;
        }

        private bool DistributeMementos(IMemento[] mementos)
        {
            bool isMementoSet;

            foreach (IStorableSettings originator in originators)
            {
                isMementoSet = false;
                foreach (IMemento memento in mementos)
                {
                    if (originator.SetMemento(memento))
                    {
                        isMementoSet = true;
                        break;
                    }
                }
                if (!isMementoSet)
                {
                    ClearStores();
                    return false;
                }
            }
            return true;
        }

        private void ClearStores()
        {
            foreach (IStorableSettings originator in originators)
            {
                originator.Clear();
            }
        }
    }
}
