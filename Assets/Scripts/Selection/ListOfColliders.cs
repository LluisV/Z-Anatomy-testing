using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListOfColliders : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> currentCollisions = new List<GameObject>();

    [HideInInspector]
    public bool triggerExitEnabled = true;

    private void OnTriggerEnter(Collider other)
    {
        // Add the GameObject collided with to the list.
        if(other.gameObject.layer == 6 || other.gameObject.layer == 13)
        {
            TangibleBodyPart script = other.gameObject.GetComponent<TangibleBodyPart>();
            if(script != null)
            {
                script.SetSelectionState(true);
                SelectedObjectsManagement.Instance.SelectObject(other.gameObject);
                currentCollisions.Add(other.gameObject);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(triggerExitEnabled && currentCollisions.Count > 0)
        {
            // Remove the GameObject collided with from the list.
            TangibleBodyPart script = other.gameObject.GetComponent<TangibleBodyPart>();
            if (script != null)
            {
                SelectedObjectsManagement.Instance.DeselectObject(other.gameObject);
                currentCollisions.Remove(other.gameObject);
            }
        }
    }
}
