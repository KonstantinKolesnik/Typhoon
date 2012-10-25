using System;
using System.Collections;
using System.Collections.Generic;

namespace Typhoon.NMRA
{
    public class NMRACommandCollection : ICollection<NMRACommand>
    {
        #region Fields
        private readonly List<NMRACommand> list = new List<NMRACommand>();
        #endregion

        #region Properties
        public int Count
        {
            get { return list.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        public virtual NMRACommand this[int index]
        {
            get
            {
                if (index < 0 || index > list.Count - 1)
                    throw new ArgumentOutOfRangeException("index");
                return list[index];
            }
            set
            {
                if (index < 0 || index > list.Count - 1)
                    throw new ArgumentOutOfRangeException("index");
                list[index] = value;
            }
        }
        #endregion

        #region Public methods
        public IEnumerator<NMRACommand> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public virtual void Add(NMRACommand item)
        {
            list.Add(item);
        }
        public virtual void Insert(int index, NMRACommand item)
        {
            list.Insert(index, item);
        }
        public bool Remove(NMRACommand item)
        {
            return list.Remove(item);
        }
        public void RemoveAt(int idx)
        {
            list.RemoveAt(idx);
        }
        public void Clear()
        {
            list.Clear();
        }

        public void CopyTo(NMRACommand[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }
        public bool Contains(NMRACommand item)
        {
            return list.Contains(item);
        }

        public void MoveUp(NMRACommand item)
        {
            if (list.Contains(item))
            {
                int idx = list.IndexOf(item);
                if (idx == 0)
                    idx = list.Count - 1;
                else
                    idx--;
                list.Remove(item);
                list.Insert(idx, item);
            }
        }
        public void MoveDown(NMRACommand item)
        {
            if (list.Contains(item))
            {
                int idx = list.IndexOf(item);
                if (idx == list.Count - 1)
                    idx = 0;
                else
                    idx++;
                list.Remove(item);
                list.Insert(idx, item);
            }
        }
        #endregion
    }
}
