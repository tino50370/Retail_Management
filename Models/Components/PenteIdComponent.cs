using System;

namespace RetailKing.Models.Components
{
    [Serializable]
    public class PenteIdComponent : QuadIdComponent, IEquatable<PenteIdComponent>
    {
        public const string Key5Property = "Key5";
        protected object _Key5;

        public PenteIdComponent()
        {
        }

        public object Key5
        {
            get { return _Key5; }
            set { _Key5 = value; }
        }

        public bool Equals(PenteIdComponent idComponent)
        {
            if (idComponent == null) return false;
            if (!Equals(_Key1, idComponent._Key1)) return false;
            if (!Equals(_Key2, idComponent._Key2)) return false;
            if (!Equals(_Key3, idComponent._Key3)) return false;
            if (!Equals(_Key4, idComponent._Key4)) return false;
            if (!Equals(_Key5, idComponent._Key5)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            return Equals(obj as PenteIdComponent);
        }

        public override int GetHashCode()
        {
            int result = _Key1.GetHashCode();
            result = 29 * result + _Key2.GetHashCode();
            result = 29 * result + _Key3.GetHashCode();
            result = 29 * result + _Key4.GetHashCode();
            result = 29 * result + _Key5.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            return _Key1 + "," + _Key2 + "," + _Key3 + "," + _Key4 + "," + _Key5;
        }
    }
}
