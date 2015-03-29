using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Untangled
{
    public class ObservableDict <Tkey, TValue> : IDictionary<Tkey,TValue>
    {

        private Dictionary<Tkey, TValue> internalDict = new Dictionary<Tkey, TValue>();
        public event EventHandler isDictEmpty;

        public IEnumerator<KeyValuePair<Tkey, TValue>> GetEnumerator()
        {
            return internalDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<Tkey, TValue> item)
        {
            internalDict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            internalDict.Clear();
            isDictEmpty(this, new EventArgs());
        }

        public bool Contains(KeyValuePair<Tkey, TValue> item)
        {
            return (internalDict.ContainsKey(item.Key) && internalDict.ContainsValue(item.Value));
        }

        public void CopyTo(KeyValuePair<Tkey, TValue>[] array, int arrayIndex)
        {
            throw new Exception("Functionality not available!");
        }

        public bool Remove(KeyValuePair<Tkey, TValue> item)
        {
            bool removed = internalDict.Remove(item.Key);

            // raise an event if the dictionary is empty
            if (internalDict.Count == 0)
            {
                isDictEmpty(this, new EventArgs());
            }

            return removed;

        }

        public int Count { get { return internalDict.Count; } }
        public bool IsReadOnly { get { return false; } }
        public bool ContainsKey(Tkey key)
        {
            return internalDict.ContainsKey(key);
        }

        public void Add(Tkey key, TValue value)
        {
            internalDict.Add(key, value);
        }

        public bool Remove(Tkey key)
        {
            bool removed = internalDict.Remove(key);
            // raise an event if the dictionary is empty
            if (internalDict.Count == 0)
            {
                isDictEmpty(this, new EventArgs());
            }

            return removed;
        }

        public bool TryGetValue(Tkey key, out TValue value)
        {
            return internalDict.TryGetValue(key, out value);
        }

        public TValue this[Tkey key]
        {
            get
            {
                return internalDict[key];
            }
            set
            {
                internalDict[key] = value;
            }
        }

        public ICollection<Tkey> Keys { get; private set; }
        public ICollection<TValue> Values { get; private set; }
    }
}
