using System;

public static class KernelUtil {

    public static double[,,] generateMatrix(ScalarFunction kernel, int size, bool normalize) {
        double[,,] kernelMatrix = new double[size, size, size];

        double norm = 0; // for normalizing

        // fill matrix with values from kernel
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                for (int k = 0; k < size; k++) {
                    double ni = 2 * (i / (size - 1.0)) - 1;
                    double nj = 2 * (j / (size - 1.0)) - 1;
                    double nk = 2 * (k / (size - 1.0)) - 1;

                    double r = Math.Sqrt(ni * ni + nj * nj + nk * nk);

                    if (r <= 1.0) {
                        kernelMatrix[i, j, k] = kernel.at(r);
                    } else {
                        kernelMatrix[i, j, k] = 0;
                    }

                    norm += kernelMatrix[i, j, k];
                }
            }
        }

        // normalize
        if (normalize) {
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    for (int k = 0; k < size; k++) {
                        kernelMatrix[i, j, k] /= norm;
                    }
                }
            }
        }

        return kernelMatrix;
    }

    public static double convolve(double[,,] grid, double[,,] kernel, int xc, int yc, int zc) {
        double sum = 0;
        int kw = kernel.GetLength(0);
        int kl = kernel.GetLength(1);
        int kh = kernel.GetLength(2);

        int gw = grid.GetLength(0);
        int gl = grid.GetLength(1);
        int gh = grid.GetLength(1);

        for (int i = 0; i < kw; i++) {
            for (int j = 0; j < kl; j++) {
                for (int k = 0; k < kh; k++) {
                    int x = Util.mod(xc + i - kw / 2, gw);
                    int y = Util.mod(yc + j - kl / 2, gl);
                    int z = Util.mod(zc + k - kh / 2, gh);
                    sum += grid[x, y, z] * kernel[i, j, k];
                }
            }
        }
        return sum;
    }
}
