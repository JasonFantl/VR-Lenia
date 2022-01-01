using System.Collections.Generic;
using UnityEngine;

public class GOL : DsicreteCA {
    public int[] rule = { 10, 21, 10, 21 };

    public override void initGrid() {
        voxels = new int[size, size, size];
        onVoxels = new List<Vector3>();

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    voxels[x, y, z] = UnityEngine.Random.Range(0, 2);
                }
            }
        }
    }

    public override void step() {
        int[,,] newGrid = new int[size, size, size];
        onVoxels.Clear();

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    newGrid[x, y, z] = runRules(x, y, z);

                    // meta stuff
                    if (newGrid[x, y, z] == 1) {
                        onVoxels.Add(new Vector3(x, y, z));
                    }
                }
            }
        }

        voxels = newGrid;
    }

    int runRules(int x, int y, int z) {
        int sum = 0;
        for (int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                for (int k = -1; k < 2; k++) {
                    // if ((i != 0 || j != 0 || k != 0) &&
                    //     (x + i >= voxels.GetLength(0) || x + i < 0 ||
                    //      y + j >= voxels.GetLength(1) || y + j < 0 ||
                    //      z + k >= voxels.GetLength(2) || z + k < 0)) {
                    //     // make sides = 0, so don't add to sum
                    //     continue;
                    // }

                    int xp = Util.mod(x + i, voxels.GetLength(0));
                    int yp = Util.mod(y + j, voxels.GetLength(1));
                    int zp = Util.mod(z + k, voxels.GetLength(2));

                    // make sure this position exist in old grid
                    if (xp < voxels.GetLength(0) && yp < voxels.GetLength(1) && zp < voxels.GetLength(2)) {
                        sum += voxels[xp, yp, zp];
                    }
                }
            }
        }

        bool isAlive = (x < voxels.GetLength(0) && y < voxels.GetLength(1) && z < voxels.GetLength(2)) &&
                       (voxels[x, y, z] == 1);
        // rule 1
        if (isAlive && (sum >= rule[0] && sum <= rule[1])) {
            return 1;
        }
        // rule 2
        if (!isAlive && (sum >= rule[2] && sum <= rule[3])) {
            return 1;
        }
        // rule 3
        return 0;
    }
}
