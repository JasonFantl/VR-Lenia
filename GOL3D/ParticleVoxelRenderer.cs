using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]

public class ParticleVoxelRenderer : CADisplay {

    public ParticleSystem ps;
    public float voxelScale = 0.1f;

    ParticleSystem.Particle[] voxels;

    public override void display(DsicreteCA ca) {
        
        voxels = new ParticleSystem.Particle[ca.onVoxels.Count];

        for (int i = 0; i < ca.onVoxels.Count; i++) {
            voxels[i].position = ca.onVoxels[i] * voxelScale;
            voxels[i].startColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            voxels[i].startSize = voxelScale;
        }

        ps.SetParticles(voxels, voxels.Length);
    }
}
