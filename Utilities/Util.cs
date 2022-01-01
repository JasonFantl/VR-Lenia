using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Util {
    public static float map(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static double map(double value, double from1, double to1, double from2, double to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static double cap(double v, double l, double h) {
        if (v < l) return l;
        if (v > h) return h;
        return v;
    }

    public static int mod(int x, int m) {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    public static float mod(float x, float m) {
        float r = x % m;
        return r < 0 ? r + m : r;
    }

    public static double mod(double x, double m) {
        double r = x % m;
        return r < 0 ? r + m : r;
    }

        public static Color grayToColor(float x) {
        float r = Mathf.Min(Mathf.Max(0f, 1.5f-Mathf.Abs(1f-4f*(x-0.5f))),1f);
		float g = Mathf.Min(Mathf.Max(0f, 1.5f-Mathf.Abs(1f-4f*(x-0.25f))),1f);
		float b = Mathf.Min(Mathf.Max(0f, 1.5f-Mathf.Abs(1f-4f*x)),1f);
        return new Color(r, g, b);
    }
}



[Serializable]
public struct Pair {
    public float left, right;
    public Pair(float l, float r) {
        left = l; right = r;
    }
}

