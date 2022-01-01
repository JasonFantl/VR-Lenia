using UnityEngine;

public abstract class ScalarFunction : MonoBehaviour {
    public abstract double at(double x);
}

public class IdentityFunction : ScalarFunction
{
    void Start() {
    }

    public override double at(double x) {
        return x;
    }
}
