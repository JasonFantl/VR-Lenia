using UnityEngine;

public class LineGraphDisplay : MonoBehaviour {

    public ScalarFunction f;
    public int resolution = 100;

    public Pair xPos = new Pair(-3, 3);
    public Pair zPos = new Pair(-2, 2);
    public float yOffset = 1;

    public Pair domain = new Pair(-1, 1);
    public Pair range = new Pair(0, 1);

    public Color lineColor = Color.red;
    public float lineThickness = 0.5f;

    private LineRenderer l;

    void Start() {
        l = gameObject.AddComponent<LineRenderer>();
        l.useWorldSpace = false;
        l.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update() {
        if (f == null) return;
        // line stuff
        l.positionCount = resolution;
        l.startWidth = lineThickness;
        l.endWidth = lineThickness;
        l.startColor = lineColor; l.endColor = lineColor;

        for (int i = 0; i < resolution; i++) {
            float x = Util.map(i, 0, resolution - 1, domain.left, domain.right);
            float z = (float)f.at(x);
            float displayX = Util.map(x, domain.left, domain.right, xPos.left, xPos.right);
            float displayZ = Util.map(z, range.left, range.right, zPos.left, zPos.right);

            // diplay is weird, so have to flip x and z
            l.SetPosition(i, new Vector3(displayZ, yOffset, displayX));
        }
    }
}

