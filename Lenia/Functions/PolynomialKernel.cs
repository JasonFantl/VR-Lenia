using System;

public class PolynomialKernel : ScalarFunction
{
    public double alpha = 4;

    PolynomialKernel() {
    }

// warning, only works with positive x values
    public override double at(double x) {
        return Math.Pow(4 * x * (1 - x), alpha);
    }
}
