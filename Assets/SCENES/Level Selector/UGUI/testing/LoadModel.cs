using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadModel : MonoBehaviour
{
    public GameObject model;
    private bool hasLoaded = false;

    public void Load()
    {
        if (!hasLoaded)
        {
            StartCoroutine(LoadModelAsync());
        }
    }

    public IEnumerator LoadModelAsync()
    {
        // Wait for the current frame to end
        yield return new WaitForEndOfFrame();

        // Instantiate the model in the scene
        var instance = Instantiate(model);

        // Move the model to the center of the screen
        instance.transform.position = Vector3.zero;

        hasLoaded = true;
    }
}
