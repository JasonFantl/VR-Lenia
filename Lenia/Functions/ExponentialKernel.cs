using System;

public class ExponentialKernel : ScalarFunction
{
    public double alpha = 0.2;

    ExponentialKernel() {
    }

    public override double at(double x) {
        return Math.Exp(alpha - (alpha / (4 * x * (1 - x))));;
    }
}
