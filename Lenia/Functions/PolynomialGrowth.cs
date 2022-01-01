using System;

public class PolynomialGrowth : ScalarFunction
{
    public double mu = 0.2;
    public double sigma = 0.2;

    PolynomialGrowth() {
    }

// warning, only works with positive x values
    public override double at(double x) {
        // return Math.Pow(1 - Math.Pow(x-mu, 2) / (9 * Math.Pow(sigma, 2)), 4.0) * 2 - 1;
        return Math.Pow(Math.Max(0, 1 - (x-mu)*(x-mu) / (9 * sigma*sigma) ), 4) * 2 - 1;
    }
}
