using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ModelScene : MonoBehaviour
{
    public GameObject prefabContainer;
    public TextMeshProUGUI selectedText;

    public GameObject modelObject;

    [Header("DEBUG OPTIONS")]
    public bool debug = false;
    public string structureName;

    // Start is called before the first frame update
    void Start()
    {
        // Get the selected button text from PlayerPrefs
        string selectedButton = PlayerPrefs.GetString("SelectedButton");

        // Convert the selectedText to a TextMeshProUGUI component
        TextMeshProUGUI selectedTextTMP = selectedText.GetComponent<TextMeshProUGUI>();

        // Load the "skeleton" prefab
        GameObject skeletonPrefab = Resources.Load<GameObject>("Prefabs/Human Body");

        if (skeletonPrefab != null)
        {
            Transform targetTransform;
            if (debug)
            {
                selectedTextTMP.text = structureName;
                targetTransform = FindGameObjectInChildren(skeletonPrefab.transform, structureName);
            }
            else
            {
                selectedTextTMP.text = selectedButton;
                targetTransform = FindGameObjectInChildren(skeletonPrefab.transform, selectedButton);
            }

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

                // Center the camera
                StartCoroutine(waitToCenter());
                IEnumerator waitToCenter()
                {
                    // Get all the info of the loaded model
                    GlobalVariables.Instance.globalParent = prefabContainer.transform.GetChild(0).gameObject;
                    GlobalVariables.Instance.GetScripts();

                    // Wait until all models are initializated
                    TangibleBodyPart[] bodyParts = model.GetComponents<TangibleBodyPart>();
                    yield return new WaitUntil(() => bodyParts.All(it => it.initialized));

                    // Center the camera
                    CameraController camScript = Camera.main.GetComponent<CameraController>();
                    camScript.SetTarget(model);
                    camScript.CenterImmediate();
                }

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
    // Helper method to recursively search for a GameObject with a specific text in the hierarchy
    private Transform FindGameObjectInChildren(Transform parent, string searchText)
    {
        // Try to find an exact match
        Transform result = FindChildByName(parent, searchText);
        if (result != null)
        {
            return result;
        }

        // Try adding ".g" and search again
        result = FindChildByName(parent, searchText + ".g");
        if (result != null)
        {
            return result;
        }

        // Try adding ".l" and search again
        result = FindChildByName(parent, searchText + ".l");
        if (result != null)
        {
            return result;
        }

        // Perform a breadth-first search
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();

            // Check if the current transform name contains the search text
            if (current.name.Contains(searchText))
            {
                return current;
            }

            for (int i = 0; i < current.childCount; i++)
            {
                Transform child = current.GetChild(i);
                queue.Enqueue(child);
            }
        }

        return null;
    }


    // Helper method to find a child GameObject by name
    private Transform FindChildByName(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
            {
                return child;
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
