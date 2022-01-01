using System;
using System.Collections.Generic;
using UnityEngine;

public static class FFT {

    // samples must be a power of 2
    // operates in place
    public static void FFT1D(Complex[] samples, bool reverse) {
        int log2length = 0;
        for (int i = 1; i < samples.Length; i <<= 1)
            log2length++;

        // bit reversal
        int mid = samples.Length >> 1; // mid = samples.Length / 2;
        int j = 0;
        for (int i = 0; i < samples.Length - 1; i++) {
            if (i < j) {
                Complex tmp = samples[i];
                samples[i] = samples[j];
                samples[j] = tmp;
            }
            int k = mid;
            while (k <= j) {
                j -= k;
                k >>= 1;
            }
            j += k;
        }

        // compute FFT
        Complex r = new Complex(-1, 0);
        int l2 = 1;
        for (int l = 0; l < log2length; l++) {
            int l1 = l2;
            l2 <<= 1;
            Complex r2 = new Complex(1, 0);
            for (int n = 0; n < l1; n++) {
                for (int i = n; i < samples.Length; i += l2) {
                    int i1 = i + l1;
                    Complex tmp = r2 * samples[i1];
                    samples[i1] = samples[i] - tmp;
                    samples[i] += tmp;
                }
                r2 = r2 * r;
            }
            r.img = Math.Sqrt((1d - r.real) / 2d);
            if (!reverse)
                r.img = -r.img;
            r.real = Math.Sqrt((1d + r.real) / 2d);
        }

        if (!reverse) // forward transform
        {
            double scale = 1d / samples.Length;
            for (int i = 0; i < samples.Length; i++)
                samples[i] *= scale;
        }
    }

    public static void FFT2D(Complex[][] samples, bool reverse) {
        // FFT the rows
        for (int i = 0; i < samples.Length; i++) {
            FFT1D(samples[i], reverse);
        }
        // transpose the matrix
        for (int i = 0; i < samples.Length; i++) {
            for (int j = 0; j < i; j++) {
                Complex tmp = samples[i][j];
                samples[i][j] = samples[j][i];
                samples[j][i] = tmp;
            }
        }
        // FFT the (transposed) cols 
        for (int i = 0; i < samples.Length; i++) {
            FFT1D(samples[i], reverse);
        }

        // we will immediatly undo this operation, so no need to transpose again
    }

    // A and B must have the same dimensions
    public static Complex[][] ElementWiseMult(Complex[][] A, Complex[][] B) {
        Complex[][] C = new Complex[A.Length][];
        for (int i = 0; i < A.Length; i++) {
            C[i] = new Complex[A[i].Length];
            for (int j = 0; j < A[i].Length; j++) {
                C[i][j] = A[i][j] * B[i][j];
            }
        }
        return C;
    }

    public static Complex[][] Double2Complex(double[][] input) {
        Complex[][] result = new Complex[input.Length][];
        for (int i = 0; i < input.Length; i++) {
            result[i] = new Complex[input[i].Length];
            for (int j = 0; j < input[i].Length; j++) {
                result[i][j] = new Complex(input[i][j], 0);
            }
        }
        return result;
    }

    public static double[][] Complex2Double(Complex[][] input) {
        double[][] result = new double[input.Length][];
        for (int i = 0; i < input.Length; i++) {
            result[i] = new double[input[i].Length];
            for (int j = 0; j < input[i].Length; j++) {
                result[i][j] = input[i][j].real;
            }
        }
        return result;
    }

    public static void Shift(double[][] input) {
        double min = -999999999;
        for (int i = 0; i < input.Length; i++) {
            for (int j = 0; j < input[i].Length; j++) {
                min = (min < input[i][j])?min:input[i][j];
            }
        }
        for (int i = 0; i < input.Length; i++) {
            for (int j = 0; j < input[i].Length; j++) {
                input[i][j] += -min;
            }
        }
    }
}


