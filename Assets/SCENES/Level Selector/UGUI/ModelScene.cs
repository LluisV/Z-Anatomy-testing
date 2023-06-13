using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UIElements;

public class ModelScene : MonoBehaviour
{
    public static ModelScene Instance;

    public GameObject prefabContainer;
    public TextMeshProUGUI selectedText;
    public Dictionary<string, GameObject> targets = new Dictionary<string, GameObject>();
    [Header("Debug options")]
    public bool DEBUG = false;
    public string levelName = string.Empty;
    public string bodypartNames;
    public bool bothSides = true;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        List<string> receivedList = bodypartNames.Split(',').ToList();
        
        if (!DEBUG)
        {
            // Get all the name of all the bodyparts of this level
            string jsonString = PlayerPrefs.GetString("BodypartList");
            receivedList = JsonUtility.FromJson<SerializableList<string>>(jsonString).list;
            // Get the level name
            levelName = PlayerPrefs.GetString("LevelName");
        }

        // Convert the selectedText to a TextMeshProUGUI component
        TextMeshProUGUI selectedTextTMP = selectedText.GetComponent<TextMeshProUGUI>();
        selectedTextTMP.text = levelName;

        // Load the "skeleton" prefab
        GameObject globalParent = Resources.Load<GameObject>("Prefabs/Skeleton");

        if (globalParent != null)
        {
            foreach (string bodypart in receivedList)
            {
                bool left = false;
                Transform targetTransform;
                targetTransform = globalParent.transform.RecursiveFindChildRaw(bodypart);

                // It is null, let's try adding .l
                if(targetTransform == null)
                {
                    targetTransform = globalParent.transform.RecursiveFindChildRaw(bodypart + ".l");
                    left = targetTransform != null;
                }
                // It is null let's try adding .g
                if (targetTransform == null)
                    targetTransform = globalParent.transform.RecursiveFindChildRaw(bodypart + ".g");
                // It is null
                if (targetTransform == null)
                    throw new Exception("Something went wrong loading " + bodypart + ". Check the names");

                // Only body parts (not labels, lines, etc)
                if (targetTransform.gameObject.IsLabel())
                    continue;
                if (targetTransform.gameObject.IsLine())
                    continue;

                // Instantiate the corresponding GameObject in the container
                GameObject model = Instantiate(targetTransform.gameObject, prefabContainer.transform, true);
                model.name = targetTransform.gameObject.name;
                model.GetComponent<BodyPartVisibility>()?.HideLabels();

                //Destroy all its childrens (labels, lines, etc)
                foreach (Transform child in model.transform)
                {
                    child.GetComponent<BodyPartVisibility>()?.HideLabels();
                    if(child.gameObject.IsLabel() || child.gameObject.IsLine())
                        Destroy(child.gameObject);
                }

                //Add it to the target list
                targets.Add(model.name, model);

                // If we want both sides, we do the same for the right one
                if (left && bothSides)
                {
                    targetTransform = globalParent.transform.RecursiveFindChildRaw(bodypart + ".r");
                    // Instantiate the corresponding GameObject in the container
                    model = Instantiate(targetTransform.gameObject, prefabContainer.transform, true);
                    model.name = targetTransform.gameObject.name;
                    //Destroy all its childrens (labels, lines, etc)
                    foreach (Transform child in model.transform)
                        Destroy(child.gameObject);
                    //Add it to the target list
                    targets.Add(model.name, model);
                }
            }
        }
        else
        {
            Debug.LogError("Could not load global prefab");
        }
    }
}
