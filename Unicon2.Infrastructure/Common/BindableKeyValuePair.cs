using System;
using System.Collections;
using System.ComponentModel;
using Prism.Mvvm;

namespace Unicon2.Infrastructure.Common
{
    public class BindableKeyValuePair<K, V> : BindableBase,INotifyDataErrorInfo
    {
        private K _key;
        private V _value;
        private bool _isInEditMode;
        public K Key
        {
            get { return _key; }
            set
            {
                _key = value;
                RaisePropertyChanged();
            }
        }

        public V Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged();
            }
        }

        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set
            {
                _isInEditMode = value;
                RaisePropertyChanged();
            }
        }

        public BindableKeyValuePair()
        {
            
        }
        public BindableKeyValuePair(K key, V value)
        {
            Key = key;
            Value = value;
        }

        public IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }

        public bool HasErrors { get; }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}