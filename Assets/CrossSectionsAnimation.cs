using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSectionsAnimation : MonoBehaviour
{

    public static CrossSectionsAnimation Instance;

    public Shader clippableShader;
    public Shader clippableShaderNoCulling;

    public GameObject plane;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMaterial(string tag, bool enabled)
    {
        int i = 0;
        Dictionary<int, Material[]> rendererMaterials = new Dictionary<int, Material[]>();

        foreach (var renderer in GlobalVariables.Instance.allBodyPartRenderers)
            rendererMaterials[i++] = renderer.sharedMaterials;

        for (i = 0; i < rendererMaterials.Count; i++)
            if (GlobalVariables.Instance.allBodyPartRenderers[i].CompareTag(tag))
                foreach (Material material in GlobalVariables.Instance.allBodyPartRenderers[i].materials)
                    material.SetFloat("_PlaneEnabled", enabled ? 1f : 0f);
    }

    public void SetOrientation(bool inverted)
    {
        if(!inverted)
            plane.transform.rotation = Quaternion.Euler(90, 0, 90);
        else
            plane.transform.rotation = Quaternion.Euler(90, 90, 0);

        plane.SetActive(true);
    }

    public void DisablePlane()
    {
        plane.SetActive(false);
    }
}
