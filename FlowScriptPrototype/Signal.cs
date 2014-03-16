using System;

namespace FlowScriptPrototype
{
    public abstract class Signal
    {
        public abstract Signal Add(Signal other);

        public abstract Signal Subtract(Signal other);

        public abstract Signal Multiply(Signal other);

        public abstract Signal Divide(Signal other);

        public abstract Signal Modulo(Signal other);

        public abstract bool EqualTo(Signal other);

        public abstract bool GreaterThan(Signal other);

        public abstract bool LessThan(Signal other);
    }

    public class NaNSignal : Signal
    {
        public override Signal Add(Signal other)
        {
            return new NaNSignal();
        }

        public override Signal Subtract(Signal other)
        {
            return new NaNSignal();
        }

        public override Signal Multiply(Signal other)
        {
            return new NaNSignal();
        }

        public override Signal Divide(Signal other)
        {
            return new NaNSignal();
        }

        public override Signal Modulo(Signal other)
        {
            return new NaNSignal();
        }

        public override bool EqualTo(Signal other)
        {
            return other is NaNSignal;
        }

        public override bool GreaterThan(Signal other)
        {
            return false;
        }

        public override bool LessThan(Signal other)
        {
            return false;
        }

        public override string ToString()
        {
 	         return float.NaN.ToString();
        }
    }

    public class StringSignal : NaNSignal
    {
        public String Value { get; set; }

        public StringSignal(String val)
        {
            Value = val;
        }

        public override bool EqualTo(Signal other)
        {
            return other is StringSignal && Value.Equals(((StringSignal) other).Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class IntSignal : Signal
    {
        public long Value { get; set; }

        public IntSignal(long val)
        {
            Value = val;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override Signal Add(Signal other)
        {
            if (other is IntSignal) {
                return new IntSignal(Value + ((IntSignal) other).Value);
            } else {
                return new NaNSignal();
            }
        }

        public override Signal Subtract(Signal other)
        {
            if (other is IntSignal) {
                return new IntSignal(Value - ((IntSignal) other).Value);
            } else {
                return new NaNSignal();
            }
        }

        public override Signal Multiply(Signal other)
        {
            if (other is IntSignal) {
                return new IntSignal(Value * ((IntSignal) other).Value);
            } else {
                return new NaNSignal();
            }
        }

        public override Signal Divide(Signal other)
        {
            if (other is IntSignal) {
                return new IntSignal(Value / ((IntSignal) other).Value);
            } else {
                return new NaNSignal();
            }
        }

        public override Signal Modulo(Signal other)
        {
            if (other is IntSignal) {
                return new IntSignal(Value % ((IntSignal) other).Value);
            } else {
                return new NaNSignal();
            }
        }

        public override bool EqualTo(Signal other)
        {
            return other is IntSignal && Value.Equals(((IntSignal) other).Value);
        }

        public override bool GreaterThan(Signal other)
        {
            if (other is IntSignal) {
                return Value > ((IntSignal) other).Value;
            } else {
                return false;
            }
        }

        public override bool LessThan(Signal other)
        {
            if (other is IntSignal) {
                return Value < ((IntSignal) other).Value;
            } else {
                return false;
            }
        }
    }
}
