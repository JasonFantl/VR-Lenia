using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayDsiplay : MonoBehaviour {
    public ScalarFunction kernel;
    public int kernelSize = 8;
    public bool normalize = false;

    public float displayWidth = 100;
    public bool useColor = false;

    public AnimalJSON presets;

    public GameObject dislpayCell;

    GameObject[,] displayCells;

    void Start() {
        displayCells = new GameObject[0, 0];
    }

    void Update() {
        if (presets != null) {
            presets.loadJSON();
            kernel = presets.generateKernel();
            kernelSize = presets.animal.parameters.R * 2 - 1;
        }

        double[,,] kernelMatrix = KernelUtil.generateMatrix(kernel, kernelSize, normalize);

        // destroy old objects
        for (int i = 0; i < displayCells.GetLength(0); i++) {
            for (int j = 0; j < displayCells.GetLength(1); j++) {
                Object.Destroy(displayCells[i, j]);
            }
        }

        displayCells = new GameObject[kernelSize, kernelSize];
        for (int i = 0; i < kernelSize; i++) {
            for (int j = 0; j < kernelSize; j++) {
                // for this special case
                kernelMatrix[i, j, kernelSize / 2] = kernelMatrix[i, j, kernelSize / 2]/2.0;
                
                float cellSize = displayWidth / kernelSize;
                displayCells[i, j] = Instantiate(dislpayCell);
                displayCells[i, j].transform.position = new Vector3(i, j, 0) * cellSize;
                displayCells[i, j].transform.localScale = Vector3.one * cellSize;
                float v = (float)(1.0 - kernelMatrix[i, j, kernelSize / 2]);
                Color col = Color.white * v;
                if (useColor) {
                    col = Util.grayToColor(1f - v);
                }
                displayCells[i, j].GetComponent<Renderer>().material.color = col;
            }
        }
    }
}
