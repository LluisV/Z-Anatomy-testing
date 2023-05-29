using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModelScene : MonoBehaviour
{
    public GameObject prefabContainer;
    public TextMeshProUGUI selectedText;

    public GameObject modelObject;

    // Start is called before the first frame update
    void Start()
    {
        // Get the selected button text from PlayerPrefs
        string selectedButton = PlayerPrefs.GetString("SelectedButton");

        // Convert the selectedText to a TextMeshProUGUI component
        TextMeshProUGUI selectedTextTMP = selectedText.GetComponent<TextMeshProUGUI>();

        // Set the text to the selected button text
        selectedTextTMP.text = selectedButton;

        // Load the "skeleton" prefab
        GameObject skeletonPrefab = Resources.Load<GameObject>("Prefabs/Skeleton");

        if (skeletonPrefab != null)
        {
            // Search for the corresponding GameObject with the selectedButton text inside the "skeleton" prefab
            Transform targetTransform = FindGameObjectInChildren(skeletonPrefab.transform, selectedButton);

            if (targetTransform != null)
            {
                // Instantiate the corresponding GameObject in the container
                GameObject model = Instantiate(targetTransform.gameObject, prefabContainer.transform);

                // Set the reference to the model GameObject
                modelObject = model;

                // Set the parent of the model to the ModelScene GameObject
                model.transform.SetParent(prefabContainer.transform);

                // Scale the model to fit the prefabContainer
                float scaleFactor = GetScaleFactor(prefabContainer, model);
                model.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }
            else
            {
                Debug.LogError("Could not find GameObject with the text: " + selectedButton + " inside the skeleton prefab");
            }
        }
        else
        {
            Debug.LogError("Could not load skeleton prefab");
        }
    }


    // Helper method to recursively search for a GameObject with a specific text in the hierarchy
    private Transform FindGameObjectInChildren(Transform parent, string searchText)
    {
        if (parent.name == searchText)
        {
            return parent;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Transform result = FindGameObjectInChildren(child, searchText);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }




    // Helper method to get the scale factor for a given container and object
    // Helper method to get the scale factor for a given container and object
    private float GetScaleFactor(GameObject container, GameObject obj)
    {
        // Get the bounds of the object
        Bounds objBounds = GetBounds(obj);

        // Get the bounds of the container
        Bounds containerBounds = GetBounds(container);

        // Calculate the scale factor
        float scaleFactor = containerBounds.size.magnitude / Mathf.Max(objBounds.size.x, objBounds.size.y, objBounds.size.z);

        return scaleFactor;
    }

    // Helper method to get the bounds of a GameObject
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
}
