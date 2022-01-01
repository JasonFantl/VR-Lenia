using UnityEngine;
using System.Threading;

// [ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class DAAMaster : MonoBehaviour {

    public DsicreteCA ca;
    public ComputeShader shader;

    public int maxStepSize = 100;

    RenderTexture target;
    Camera cam;

    ComputeBuffer positionsBuffer;

    void Start() {
        Application.targetFrameRate = 60;
        positionsBuffer = new ComputeBuffer(ca.size * ca.size * ca.size, 4); // 3D array of ints (do doubles later)

        ca.initGrid();
        positionsBuffer.SetData(ca.voxels);
    }

    void Init() {
        cam = Camera.current;
    }


    // timing stuff
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

            // handle data
            positionsBuffer.SetData(ca.voxels);


            // run the heavy computation again
            Thread thread = new System.Threading.Thread(() => {
                ca.step();
                computationDone = true;
            });
            thread.Start();
        }

        if (Input.GetKeyDown("space")) {
            ca.initGrid();
            positionsBuffer.SetData(ca.voxels);
            Debug.Log("changed stuff");
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (ca == null) return;

        Init();
        InitRenderTexture();
        SetParameters();

        int threadGroupsX = Mathf.CeilToInt(cam.pixelWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(cam.pixelHeight / 8.0f);
        shader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(target, destination);
    }

    void SetParameters() {
        shader.SetTexture(0, "Destination", target);
        // shader.SetFloat("power", Mathf.Max(fractalPower, 1.01f));
        // shader.SetFloat("darkness", darkness);
        // shader.SetFloat("blackAndWhite", blackAndWhite);
        // shader.SetVector("colourAMix", new Vector3(redA, greenA, blueA));
        // shader.SetVector("colourBMix", new Vector3(redB, greenB, blueB));

        shader.SetMatrix("_CameraToWorld", cam.cameraToWorldMatrix);
        shader.SetMatrix("_CameraInverseProjection", cam.projectionMatrix.inverse);
        // shader.SetVector("_LightDirection", directionalLight.transform.forward);

        if (positionsBuffer != null) {
            shader.SetBuffer(0, "_Grid", positionsBuffer);
            shader.SetInt("_Size", ca.size);
            shader.SetInt("_MAX_RAY_STEPS", maxStepSize);
        }
    }

    void InitRenderTexture() {
        if (target == null || target.width != cam.pixelWidth || target.height != cam.pixelHeight) {
            if (target != null) {
                target.Release();
            }
            target = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create();
        }
    }
}