using System;

namespace RetailKing.Models.Components
{
    [Serializable]
    public class OchtoIdComponent : EftaIdComponent, IEquatable<OchtoIdComponent>
    {
        public const string Key8Property = "Key8";
        protected object _Key8;

        public OchtoIdComponent()
        {
        }

        public object Key8
        {
            get { return _Key8; }
            set { _Key8 = value; }
        }

        public bool Equals(OchtoIdComponent idComponent)
        {
            if (idComponent == null) return false;
            if (!Equals(_Key1, idComponent._Key1)) return false;
            if (!Equals(_Key2, idComponent._Key2)) return false;
            if (!Equals(_Key3, idComponent._Key3)) return false;
            if (!Equals(_Key4, idComponent._Key4)) return false;
            if (!Equals(_Key5, idComponent._Key5)) return false;
            if (!Equals(_Key6, idComponent._Key6)) return false;
            if (!Equals(_Key7, idComponent._Key7)) return false;
            if (!Equals(_Key8, idComponent._Key8)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            return Equals(obj as OchtoIdComponent);
        }

        public override int GetHashCode()
        {
            int result = _Key1.GetHashCode();
            result = 29 * result + _Key2.GetHashCode();
            result = 29 * result + _Key3.GetHashCode();
            result = 29 * result + _Key4.GetHashCode();
            result = 29 * result + _Key5.GetHashCode();
            result = 29 * result + _Key6.GetHashCode();
            result = 29 * result + _Key7.GetHashCode();
            result = 29 * result + _Key8.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            return _Key1 + "," + _Key2 + "," + _Key3 + "," + _Key4 + "," + _Key5 + "," + _Key6 + "," + _Key7 + "," + _Key8;
        }
    }
}
