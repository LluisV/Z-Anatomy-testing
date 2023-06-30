using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemSwitcher : MonoBehaviour
{
    public GameObject prefabContainer;
    public TMP_Text systemTextField;
    public GameObject modelObject;

    private string[] systems = { "Muscular", "Skeleton", "Nervous" };
    private int currentIndex;

    private void Start()
    {
        currentIndex = 0;
        if(systemTextField.text == "")
        {
            systemTextField.text = "Skeleton";
            LoadSelectedPrefab();
        }
    }

    private void LoadSelectedPrefab()
    {
        string selectedButton = systemTextField.text;
        LoadPrefab(selectedButton);
    }

    private void LoadPrefab(string modelName)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + modelName);

        if (prefab != null)
        {
            DestroyCurrentModel(); // Destroy the previous model before loading the new one
            
            GameObject model = Instantiate(prefab, prefabContainer.transform);
            modelObject = model;
            model.transform.SetParent(prefabContainer.transform);

            // Set the model's scale to 1 without affecting its local position or rotation
            model.transform.localScale = Vector3.one;

            // Adjust the model's position and scale relative to the container
            Vector3 containerSize = prefabContainer.transform.lossyScale;
            Vector3 modelSize = GetBounds(model).size;
            float scaleFactor = Mathf.Min(containerSize.x / modelSize.x, containerSize.y / modelSize.y, containerSize.z / modelSize.z);
            model.transform.localScale *= scaleFactor;
            GlobalVariables.Instance.GetScripts(model);
            CrossSectionsAnimation.Instance.SetMaterial(model.tag, true);
        }
        else
        {
            Debug.LogError("Could not load prefab: " + modelName);
        }

    }



    private void DestroyCurrentModel()
    {
        foreach (Transform child in prefabContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void DestroyHumanBodyModel()
    {
        GameObject humanBodyModel = GameObject.Find("@Human Body");
        if (humanBodyModel != null)
        {
            Destroy(humanBodyModel);
        }
    }

    private float GetScaleFactor(GameObject container, GameObject obj)
    {
        Bounds objBounds = GetBounds(obj);
        Bounds containerBounds = GetBounds(container);
        float scaleFactor = containerBounds.size.magnitude / objBounds.size.magnitude;
        return scaleFactor;
    }

    private Bounds GetBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds;
    }

    public void OnRightArrowButtonClick()
    {
        DestroyHumanBodyModel();
        currentIndex = (currentIndex + 1) % systems.Length;
        UpdateSystemTextField();
        LoadSelectedPrefab();
    }

    public void OnLeftArrowButtonClick()
    {
        currentIndex = (currentIndex - 1 + systems.Length) % systems.Length;
        UpdateSystemTextField();
        LoadSelectedPrefab();
    }

    private void UpdateSystemTextField()
    {
        string systemName = systems[currentIndex];
        systemTextField.text = systemName;
    }
}