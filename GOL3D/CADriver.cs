using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class CADriver : MonoBehaviour {

    public DsicreteCA ca;
    public CADisplay displayer;

    float elapsed = 0f;
    public float delay = 1.0f;

    volatile bool computationDone = true; // accessed by multipile threads

    void Update() {
        elapsed += Time.deltaTime;
        if (elapsed >= delay && computationDone) {
            // reset timing values
            if (delay > 0) {
                elapsed = Util.mod(elapsed, delay);
            }
            computationDone = false;

            // display last computation
            displayer.display(ca);

            // run the heavy computation again
            Thread thread = new System.Threading.Thread(() => {
                ca.step();
                computationDone = true;
            });
            thread.Start();
        }

        if (Input.GetKeyDown("space")) {
            ca.initGrid();
        }
    }
}

public abstract class DsicreteCA : MonoBehaviour {
    public int size = 20;

    [HideInInspector]
    public int[,,] voxels;
    [HideInInspector]
    public List<Vector3> onVoxels;

    public abstract void step();
    public abstract void initGrid();

    void Start() {
        voxels = new int[size, size, size];
        onVoxels = new List<Vector3>();
        initGrid();
    }
}


public abstract class CADisplay : MonoBehaviour {
    public abstract void display(DsicreteCA ca);
}
