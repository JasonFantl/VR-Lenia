using System;

public class ExponentialGrowth : ScalarFunction
{
    public double mu = 0.2;
    public double sigma = 0.2;

    public ExponentialGrowth() {
    }

    public override double at(double x) {
        return 2 * Math.Exp(-(Math.Pow(x - mu, 2) / (2 * sigma * sigma))) - 1;
    }
}
