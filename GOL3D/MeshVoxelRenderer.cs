using UnityEngine;

// not at all fast, but simple and easy

public class MeshVoxelRenderer : CADisplay {

    public float cellSize = 0.5f;
    public GameObject dislpayCell;

    GameObject[] displayCells = new GameObject[0];

    public override void display(DsicreteCA ca) {
        // remove old boxs and add new boxs
        for (int i = 0; i < displayCells.Length; i++) {
            Object.Destroy(displayCells[i]);
        }

        displayCells = new GameObject[ca.onVoxels.Count];
        for (int i = 0; i < ca.onVoxels.Count; i++) {
            displayCells[i] = Instantiate(dislpayCell);
            displayCells[i].transform.position = ca.onVoxels[i] * cellSize;
            displayCells[i].transform.localScale = Vector3.one * cellSize;
        }
    }
}
