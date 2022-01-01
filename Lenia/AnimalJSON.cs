using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class AnimalParams {
    //these variables are case sensitive and must match the strings "firstName" and "lastName" in the JSON.
    public int R;
    public int T;
    public double[] b;
    public double m;
    public double s;
    public int kn;
    public int gn;
}

public class Animal {
    //these variables are case sensitive and must match the strings "firstName" and "lastName" in the JSON.
    public string name;
    public AnimalParams parameters;
    public float[][][] data;

    public Animal() {
        parameters = new AnimalParams();
    }
}

public class AnimalJSON : MonoBehaviour {
    public TextAsset jsonFile;

    public Animal animal;

    // Start is called before the first frame update
    public void loadJSON() {
        // UNity parser cant handle nested arrays, need to find alternative
        // animal = JsonUtility.FromJson<Animal>(jsonFile.text);

        animal = new Animal();

        JSONNode root = JSONNode.Parse(jsonFile.text);

        // parse shell values
        string[] stringFrac = root["parameters"]["b"].Value.Split(',');
        animal.parameters.b = new double[stringFrac.Length];
        for (int i = 0; i < stringFrac.Length; i++) {
            animal.parameters.b[i] = FractionToDouble(stringFrac[i]);
            Debug.LogFormat("Shell {0} is {1}", i, animal.parameters.b[i]);
        }

        // parse basic parameters
        animal.parameters.R = root["parameters"]["R"];
        Debug.LogFormat("R: {0}", animal.parameters.R);

        animal.parameters.T = root["parameters"]["T"];
        Debug.LogFormat("T: {0}", animal.parameters.T);

        animal.parameters.m = root["parameters"]["m"];
        Debug.LogFormat("Mu: {0}", animal.parameters.m);

        animal.parameters.s = root["parameters"]["s"];
        Debug.LogFormat("Sigma: {0}", animal.parameters.s);

        animal.parameters.kn = root["parameters"]["kn"];
        Debug.LogFormat("Kernel index: {0}", animal.parameters.kn);

        animal.parameters.gn = root["parameters"]["gn"];
        Debug.LogFormat("Growth index: {0}", animal.parameters.gn);


        // parse data
        JSONNode values = root["data"];
        animal.data = new float[values.Count][][];
        for (int i = 0; i < values.Count; i++) {
            animal.data[i] = new float[values[i].Count][];
            for (int j = 0; j < values[i].Count; j++) {
                animal.data[i][j] = new float[values[i][j].Count];
                for (int k = 0; k < values[i][j].Count; k++) {
                    animal.data[i][j][k] = values[i][j][k].AsFloat;
                }
            }
        }
    }


    public ScalarFunction generateKernel() {
        ScalarFunction core = gameObject.AddComponent<IdentityFunction>() as IdentityFunction;
        if (animal.parameters.kn == 1) {
            PolynomialKernel f = gameObject.AddComponent<PolynomialKernel>() as PolynomialKernel;
            f.alpha = 4.0;
            core = f;
        } else if (animal.parameters.kn == 2) {
            ExponentialKernel f = gameObject.AddComponent<ExponentialKernel>() as ExponentialKernel;
            f.alpha = 4.0;
            core = f;
        }

        ShelledFunction shellFunc = gameObject.AddComponent<ShelledFunction>() as ShelledFunction;
        shellFunc.f = core;
        shellFunc.shell = animal.parameters.b; // maybe need to reverse these?

        return shellFunc;
    }

    public ScalarFunction generateGrowth() {

        if (animal.parameters.gn == 1) {
            PolynomialGrowth f = gameObject.AddComponent<PolynomialGrowth>() as PolynomialGrowth;
            f.mu = animal.parameters.m;
            f.sigma = animal.parameters.s;
            return f;
        } else if (animal.parameters.gn == 2) {
            ExponentialGrowth f = gameObject.AddComponent<ExponentialGrowth>() as ExponentialGrowth;
            f.mu = animal.parameters.m;
            f.sigma = animal.parameters.s;
            return f;
        }

        return gameObject.AddComponent<IdentityFunction>() as IdentityFunction;
    }


    double FractionToDouble(string fraction) {
        double result;

        if (double.TryParse(fraction, out result)) {
            return result;
        }

        string[] split = fraction.Split(new char[] { ' ', '/' });

        if (split.Length == 2 || split.Length == 3) {
            int a, b;

            if (int.TryParse(split[0], out a) && int.TryParse(split[1], out b)) {
                if (split.Length == 2) {
                    return (double)a / b;
                }

                int c;

                if (int.TryParse(split[2], out c)) {
                    return a + (double)b / c;
                }
            }
        }

        throw new System.Exception("Not a valid fraction.");
    }
}

