using UnityEngine;
using System.Threading;

// [ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class LeniaDriver : MonoBehaviour {

    public LeniaCA ca;
    public ComputeShader shader;

    public int renderDistance = 100;
    [Range(0.0f, 1.0f)]
    public float thresh = 0.4f;

    public bool modSpace = false;
    public bool transparent = true;
    public bool useColor = false;

    RenderTexture target;
    Camera cam;

    ComputeBuffer positionsBuffer;

    void Start() {
        Application.targetFrameRate = 60;
        positionsBuffer = new ComputeBuffer(ca.worldSize * ca.worldSize * ca.worldSize, 4); // 3D array of floats

        ca.init();
        setVoxels();
    }

    void Init() {
        cam = Camera.current;
    }


    // timing stuff
    float elapsed = 0f;
    public float delay = 1.0f;
    volatile bool computationDone = true; // accessed by multipile threads
    bool resetting = false;

    void Update() {
        elapsed += Time.deltaTime;
        if (elapsed >= delay && computationDone) {
            // reset timing values
            Debug.Log(elapsed);
            elapsed = 0.0f;

            computationDone = false;
            if (resetting) {
                resetting = false;
                ca.init();
            }

            // handle data
            setVoxels();

            // run the heavy computation again
            Thread thread = new System.Threading.Thread(() => {
                ca.step();
                computationDone = true;
            });
            thread.Start();
        }

        if (Input.GetKeyDown("space")) {
            resetting = true;
        }
    }

    void setVoxels() {
        int s = ca.worldSize;

        // have to convert doubles to floats since the compute shader can't handle doubles
        float[,,] tv = new float[s, s, s];
        for (int i = 0; i < s; i++) {
            for (int j = 0; j < s; j++) {
                for (int k = 0; k < s; k++) {
                    tv[i, j, k] = (float)ca.voxels[i, j, k];
                }
            }
        }
        positionsBuffer.SetData(tv);
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
        shader.SetMatrix("_CameraToWorld", cam.cameraToWorldMatrix);
        shader.SetMatrix("_CameraInverseProjection", cam.projectionMatrix.inverse);

        if (positionsBuffer != null) {
            shader.SetBuffer(0, "_Grid", positionsBuffer);
            shader.SetInt("_Size", ca.worldSize);
            shader.SetInt("_MAX_RAY_STEPS", renderDistance);
            shader.SetFloat("_thresh", thresh);

            shader.SetBool("_modSpace", modSpace);
            shader.SetBool("_enableTrans", transparent);
            shader.SetBool("_useColor", useColor);

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