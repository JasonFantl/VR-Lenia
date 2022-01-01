using System;

public class RectangularFunction : ScalarFunction
{
    public Interval[] intervals;

    public RectangularFunction() {
        intervals = new Interval[0];
    }

    public override double at(double x) {
        foreach (Interval interval in intervals) {
            if (x >= interval.left && x <= interval.right) return interval.amplitude;
        }
        return 0;
    }
}


[Serializable]
public struct Interval {
    public double left, right, amplitude;
    public Interval(double l, double r, double a) {
        left = l; right = r; amplitude = a;
    }
}

