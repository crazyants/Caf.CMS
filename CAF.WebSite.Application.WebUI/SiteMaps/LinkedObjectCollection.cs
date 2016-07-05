using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.WebUI
{
    public class LinkedObjectCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : LinkedObjectBase<T>
    {
        private readonly IList<T> innerList = new List<T>();
        public T Parent
        {
            get;
            protected set;
        }
        public int Count
        {
            get
            {
                return this.innerList.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return this.innerList.IsReadOnly;
            }
        }
        public T this[int index]
        {
            get
            {
                return this.innerList[index];
            }
            set
            {
                if (index < 0 || index >= this.innerList.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
               
                T previousSibling = default(T);
                T nextSibling = default(T);
                if (index > 0)
                {
                    previousSibling = this.innerList[index - 1];
                    previousSibling.NextSibling = value;
                }
                if (index + 1 < this.innerList.Count)
                {
                    nextSibling = this.innerList[index + 1];
                    nextSibling.PreviousSibling = value;
                }
                value.Parent = this.Parent;
                value.PreviousSibling = previousSibling;
                value.NextSibling = nextSibling;
                this.Cleanup(index);
                this.innerList[index] = value;
            }
        }
        public LinkedObjectCollection(T parent)
        {
            this.Parent = parent;
        }
        public void Add(T item)
        {
            
            item.Parent = this.Parent;
            if (this.innerList.Count > 0)
            {
                T previousSibling = this.innerList[this.innerList.Count - 1];
                previousSibling.NextSibling = item;
                item.PreviousSibling = previousSibling;
            }
            this.innerList.Add(item);
        }
        public void Clear()
        {
            foreach (T current in this.innerList)
            {
                LinkedObjectCollection<T>.Cleanup(current);
            }
            this.innerList.Clear();
        }
        public bool Contains(T item)
        {
            return this.innerList.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.innerList.CopyTo(array, arrayIndex);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return this.innerList.GetEnumerator();
        }
        public int IndexOf(T item)
        {
            return this.innerList.IndexOf(item);
        }
        public void Insert(int index, T item)
        {
            if (index < 0 || index > this.innerList.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            
            if (index == this.innerList.Count)
            {
                this.Add(item);
                return;
            }
            item.Parent = this.Parent;
            T previousSibling = default(T);
            if (index > 0)
            {
                previousSibling = this.innerList[index - 1];
                previousSibling.NextSibling = item;
            }
            T nextSibling = this.innerList[index];
            nextSibling.PreviousSibling = item;
            item.PreviousSibling = previousSibling;
            item.NextSibling = nextSibling;
            this.innerList.Insert(index, item);
        }
        public bool Remove(T item)
        {
            int num = this.IndexOf(item);
            if (num > -1)
            {
                this.RemoveAt(num);
                return true;
            }
            return false;
        }
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.innerList.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            T t = default(T);
            T t2 = default(T);
            if (index > 0)
            {
                t = this.innerList[index - 1];
            }
            if (index + 1 < this.innerList.Count)
            {
                t2 = this.innerList[index + 1];
            }
            if (t != null)
            {
                t.NextSibling = t2;
            }
            if (t2 != null)
            {
                t2.PreviousSibling = t;
            }
            this.Cleanup(index);
            this.innerList.RemoveAt(index);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        private static void Cleanup(T item)
        {
            item.PreviousSibling = default(T);
            item.NextSibling = default(T);
            item.Parent = default(T);
        }
        private void Cleanup(int index)
        {
            if (this.innerList.Count > 0 && index > -1 && index < this.innerList.Count)
            {
                LinkedObjectCollection<T>.Cleanup(this.innerList[index]);
            }
        }
    }
}
