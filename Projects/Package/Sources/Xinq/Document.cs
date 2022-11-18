using System;
using System.IO;

namespace Xinq
{
    internal abstract class Document
    {
        private bool _loadedAsReadOnly;
        private bool _isReadOnly;

        public string Filename
        {
            get;
            private set;
        }

        public bool IsDirty
        {
            get;
            set;
        }

        public bool IsReadOnly
        {
            get
            {
                if (_loadedAsReadOnly)
                    return true;

                return _isReadOnly;
            }
            set
            {
                _isReadOnly = value;
            }
        }

        public void DetermineReadOnlyMode()
        {
            _isReadOnly = ((File.GetAttributes(Filename) & FileAttributes.ReadOnly) != 0);
        }

        public void Load(string filename, bool asReadOnly)
        {
            LoadDocument(filename);

            Filename = filename;
            _loadedAsReadOnly = asReadOnly;
            IsDirty = false;

            DetermineReadOnlyMode();
        }

        public void Load(string filename)
        {
            Load(filename, false);
        }

        public void Save(string filename, bool asCopy)
        {
            SaveDocument(filename);

            if (!asCopy)
            {
                Filename = filename;
                IsDirty = false;

                DetermineReadOnlyMode();
            }
        }

        public void Save(string filename)
        {
            Save(filename, false);
        }

        public void Save()
        {
            Save(Filename);
        }

        public void Rename(string filename)
        {
            Filename = filename;

            DetermineReadOnlyMode();
        }

        protected abstract void LoadDocument(string filename);

        protected abstract void SaveDocument(string filename);

        public void Reload()
        {
            Load(Filename);
        }
    }
}
