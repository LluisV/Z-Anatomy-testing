using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelScene : MonoBehaviour
{
    public GameObject prefabContainer;
    public Text selectedText;
    public GameObject modelObject;

    // Start is called before the first frame update
    void Start()
    {
        // Get the selected button text from PlayerPrefs
        string selectedButton = PlayerPrefs.GetString("SelectedButton");

        // Set the text to the selected button text
        selectedText.text = selectedButton;

        // Load the corresponding prefab
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + selectedButton);

        // Instantiate the prefab in the container
        if (prefab != null)
        {
            // Instantiate the prefab
            GameObject model = Instantiate(prefab, prefabContainer.transform);

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
            Debug.LogError("Could not load prefab: " + selectedButton);
        }
    }

    // Helper method to get the scale factor for a given container and object
    private float GetScaleFactor(GameObject container, GameObject obj)
    {
        // Get the bounds of the object
        Bounds objBounds = GetBounds(obj);

        // Get the bounds of the container
        Bounds containerBounds = GetBounds(container);

        // Calculate the scale factor
        float scaleFactor = containerBounds.size.magnitude / objBounds.size.magnitude;

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
