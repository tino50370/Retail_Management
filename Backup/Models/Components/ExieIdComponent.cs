using System;

namespace RetailKing.Models.Components
{
    [Serializable]
    public class ExieIdComponent : PenteIdComponent, IEquatable<ExieIdComponent>
    {
        public const string Key6Property = "Key6";
        protected object _Key6;

        public ExieIdComponent()
        {
        }

        public object Key6
        {
            get { return _Key6; }
            set { _Key6 = value; }
        }

        public bool Equals(ExieIdComponent idComponent)
        {
            if (idComponent == null) return false;
            if (!Equals(_Key1, idComponent._Key1)) return false;
            if (!Equals(_Key2, idComponent._Key2)) return false;
            if (!Equals(_Key3, idComponent._Key3)) return false;
            if (!Equals(_Key4, idComponent._Key4)) return false;
            if (!Equals(_Key5, idComponent._Key5)) return false;
            if (!Equals(_Key6, idComponent._Key6)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            return Equals(obj as ExieIdComponent);
        }

        public override int GetHashCode()
        {
            int result = _Key1.GetHashCode();
            result = 29 * result + _Key2.GetHashCode();
            result = 29 * result + _Key3.GetHashCode();
            result = 29 * result + _Key4.GetHashCode();
            result = 29 * result + _Key5.GetHashCode();
            result = 29 * result + _Key6.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            return _Key1 + "," + _Key2 + "," + _Key3 + "," + _Key4 + "," + _Key5 + "," + _Key6;
        }
    }
}
