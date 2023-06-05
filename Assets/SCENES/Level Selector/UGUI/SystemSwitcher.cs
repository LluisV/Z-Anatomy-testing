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
        LoadSelectedPrefab();

    }

    private void LoadSelectedPrefab()
    {
        string selectedButton = systemTextField.text;

        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + selectedButton);

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
        }
        else
        {
            Debug.LogError("Could not load prefab: " + selectedButton);
        }
    }



    private void DestroyCurrentModel()
    {
        int childCount = prefabContainer.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(prefabContainer.transform.GetChild(i).gameObject);
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