using System;

namespace RetailKing.Models.Components
{
    [Serializable]
    public class EftaIdComponent : ExieIdComponent, IEquatable<EftaIdComponent>
    {
        public const string Key7Property = "Key7";
        protected object _Key7;

        public EftaIdComponent()
        {
        }

        public object Key7
        {
            get { return _Key7; }
            set { _Key7 = value; }
        }

        public bool Equals(EftaIdComponent idComponent)
        {
            if (idComponent == null) return false;
            if (!Equals(_Key1, idComponent._Key1)) return false;
            if (!Equals(_Key2, idComponent._Key2)) return false;
            if (!Equals(_Key3, idComponent._Key3)) return false;
            if (!Equals(_Key4, idComponent._Key4)) return false;
            if (!Equals(_Key5, idComponent._Key5)) return false;
            if (!Equals(_Key6, idComponent._Key6)) return false;
            if (!Equals(_Key7, idComponent._Key7)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            return Equals(obj as EftaIdComponent);
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
            return result;
        }

        public override string ToString()
        {
            return _Key1 + "," + _Key2 + "," + _Key3 + "," + _Key4 + "," + _Key5 + "," + _Key6 + "," + _Key7;
        }
    }
}