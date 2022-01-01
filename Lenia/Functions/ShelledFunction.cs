using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelledFunction : ScalarFunction {
    public ScalarFunction f;
    public double[] shell;

    public ShelledFunction() {
        shell = new double[0];
    }

    // assumes x is positive
    public override double at(double x) {
        double br = shell.Length * x;
        int i = (int)(Util.cap(br, 0, shell.Length - 1)); // gives us the shell its in
        return shell[i] * f.at(br - i);
    }
}
