using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Game.Scripts.Util
{
    public delegate void ValueChangedHandler<T>(T oldValue);

    /// <summary>
    /// Observable allows being informed of a value changing, and getting the current value.
    /// </summary>
    public class Observable<T>
    {
        protected T value;

        public Observable(T initialValue)
        {
            value = initialValue;
        }

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                T oldValue = this.value;
                this.value = value;
                if (OnValueChanged != null)
                    OnValueChanged(oldValue);
            }
        }

        public static implicit operator bool(Observable<T> obj)
        {
            return obj != null && obj.value != null;
        }

        public event ValueChangedHandler<T> OnValueChanged;

    }

}
