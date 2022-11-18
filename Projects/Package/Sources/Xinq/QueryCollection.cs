using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Xinq
{
    internal class QueryCollection : ICollection<Query>
    {
        private XinqDocument _document;
        private List<Query> _items = new List<Query>();

        public QueryCollection(XinqDocument document)
        {
            _document = document;
        }

        public void Add(Query item)
        {
            _items.Add(item);
            _document.IsDirty = true;
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
            _document.IsDirty = true;
        }

        public Query this[string name]
        {
            get
            {
                foreach (var query in _items)
                    if (string.Compare(query.Name, name, true, CultureInfo.InvariantCulture) == 0)
                        return query;

                return null;
            }
        }

        #region ICollection<Query> Members

        public void Clear()
        {
            _items.Clear();
            _document.IsDirty = true;
        }

        public bool Contains(Query item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(Query[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(Query item)
        {
            var removed = _items.Remove(item);

            if (removed)
                _document.IsDirty = true;

            return removed;
        }

        #endregion

        #region IEnumerable<Query> Members

        public IEnumerator<Query> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion
    }
}
