using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeniaCA : MonoBehaviour {

    public int worldSize = 64;
    public double[,,] voxels;

    public ScalarFunction kernel;
    public int kernelSize = 16;
    double[,,] kernelMatrix;

    public ScalarFunction growth;

    public double dt = 0.1;
    public bool simpleCalculation = true;

    public bool randomWorld = false;
    public AnimalJSON presetCells;

    // fft stuff
    Exocortex.DSP.ComplexF[] worldFFT;
    Exocortex.DSP.ComplexF[] kernelFFT;

    public void init() {
        // init world
        voxels = new double[worldSize, worldSize, worldSize];
        if (randomWorld) {
            for (int x = 0; x < worldSize; x++) {
                for (int y = 0; y < worldSize; y++) {
                    for (int z = 0; z < worldSize; z++) {
                        voxels[x, y, z] = UnityEngine.Random.Range(0.0f, 1.0f);
                    }
                }
            }
        } else { // load preset
            presetCells.loadJSON();

            // set parameters and kernel and growth
            dt = 1.0 / presetCells.animal.parameters.T;
            kernel = presetCells.generateKernel();
            kernelSize = presetCells.animal.parameters.R*2-1;
            growth = presetCells.generateGrowth();

            // load cells
            float[][][] c = presetCells.animal.data;
            for (int x = 0; x < c.Length; x++) {
                for (int y = 0; y < c[x].Length; y++) {
                    for (int z = 0; z < c[x][y].Length; z++) {
                        voxels[x, y, z] = c[x][y][z];
                    }
                }
            }
        }

        kernelMatrix = KernelUtil.generateMatrix(kernel, kernelSize, true);
        worldFFT = new Exocortex.DSP.ComplexF[worldSize * worldSize * worldSize];
        kernelFFT = new Exocortex.DSP.ComplexF[worldSize * worldSize * worldSize];

        // calc kernel FFT
        for (int x = 0; x < kernelSize; x++) {
            for (int y = 0; y < kernelSize; y++) {
                for (int z = 0; z < kernelSize; z++) {
                    int xp = x + worldSize / 2 - kernelSize / 2;
                    int yp = y + worldSize / 2 - kernelSize / 2;
                    int zp = z + worldSize / 2 - kernelSize / 2;

                    int i = xp * worldSize * worldSize + yp * worldSize + zp;


                    kernelFFT[i].Re = (float)kernelMatrix[x, y, z];
                }
            }
        }

        Exocortex.DSP.Fourier.FFT3(kernelFFT, worldSize, worldSize, worldSize, Exocortex.DSP.FourierDirection.Forward);
    }

    public void step() {
        double[,,] newVoxels = new double[worldSize, worldSize, worldSize];

        if (simpleCalculation) {
            // simple way
            for (int x = 0; x < worldSize; x++) {
                for (int y = 0; y < worldSize; y++) {
                    for (int z = 0; z < worldSize; z++) {
                        if (x < voxels.GetLength(0) && y < voxels.GetLength(1)) {
                            newVoxels[x, y, z] = Util.cap(
                                voxels[x, y, z] + dt * growth.at(KernelUtil.convolve(voxels, kernelMatrix, x, y, z)),
                                0, 1);

                            // for shader testing
                            // newVoxels[x, y, z] = (x % 2 == 0 && y % 2 ==0 && z % 2 == 0)?(x+y+z)/(3.0*worldSize):0.0;
                        }
                    }
                }
            }
        } else {
            // using FFT

            // would be nice if we stored this as the actual world rather then transforming back and forth
            for (int x = 0; x < worldSize; x++) {
                for (int y = 0; y < worldSize; y++) {
                    for (int z = 0; z < worldSize; z++) {
                        int i = x * worldSize * worldSize + y * worldSize + z;
                        worldFFT[i].Re = (float)voxels[x, y, z];
                        worldFFT[i].Im = 0;
                    }
                }
            }

            // apply FFT to world
            Exocortex.DSP.Fourier.FFT3(worldFFT, worldSize, worldSize, worldSize, Exocortex.DSP.FourierDirection.Forward);

            // elementwise multiply
            Exocortex.DSP.ComplexF[] potnetialFFT = new Exocortex.DSP.ComplexF[worldSize * worldSize * worldSize];
            for (int i = 0; i < worldSize * worldSize * worldSize; i++) {
                potnetialFFT[i] = worldFFT[i] * kernelFFT[i];
            }

            // inverse potential
            Exocortex.DSP.Fourier.FFT3(potnetialFFT, worldSize, worldSize, worldSize, Exocortex.DSP.FourierDirection.Backward);

            for (int x = 0; x < worldSize; x++) {
                for (int y = 0; y < worldSize; y++) {
                    for (int z = 0; z < worldSize; z++) {
                        int i = x * worldSize * worldSize + y * worldSize + z;
                        double v = (double)potnetialFFT[i].Re / (worldSize * worldSize * worldSize); // FFT library doesn't do scaling
                        newVoxels[x, y, z] = Util.cap(voxels[x, y, z] + dt * growth.at(v), 0, 1);
                    }
                }
            }
        }

        voxels = newVoxels;
    }
}
