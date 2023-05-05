using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MeshManagement : MonoBehaviour
{
    public static MeshManagement Instance;
    private SelectedObjectsManagement selectedObjectsManagement;
    [HideInInspector]
    public Dictionary<int, Material[]> rendererMaterials = new Dictionary<int, Material[]>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        selectedObjectsManagement = GetComponent<SelectedObjectsManagement>();

        //Hide body sections
        foreach (Transform section in GlobalVariables.Instance.globalParent.transform)
        {
            if (section.CompareTag("Skeleton"))
                section.transform.SetActiveRecursively(true);
            else
                section.transform.SetActiveRecursively(false);
        }

        //Hide insertions
        foreach (var item in GlobalVariables.Instance.insertions)
        {
            item.gameObject.SetActive(false);
            item.GetComponent<BodyPartVisibility>().isVisible = false;
        }

        //Get all renderers and assign labels
        int i = 0;
        foreach (var renderer in GlobalVariables.Instance.allBodyPartRenderers)
        {
            rendererMaterials[i] = renderer.sharedMaterials;
            i++;
            BodyPartVisibility script = renderer.GetComponent<BodyPartVisibility>();
        }

        HideAllLabels();

        //Set default shader params
        for (i = 0; i < rendererMaterials.Count; i++)
        {
            foreach (Material material in rendererMaterials[i])
            {
                if (material == null)
                    continue;
                material.SetVector("_PlanePosition", new Vector3(0, 0, 0));
                material.SetVector("_PlaneNormal", transform.up);
                material.SetFloat("_PlaneEnabled", 1f);
            }
        }

        foreach (var item in GlobalVariables.Instance.globalParent.GetComponentsInChildren<BodyPartVisibility>(true))
            GetInsertionsInChild(item);

        SelectedObjectsManagement.Instance.GetActiveObjects();
    }

    private void GetInsertionsInChild(BodyPartVisibility visibilityScript)
    {
        if (!visibilityScript.CompareTag("Muscles"))
            return;

        foreach (var item in visibilityScript.GetComponentsInChildren<BodyPartVisibility>(true))
        {
            if (item != visibilityScript && item.HasInsertions())
            {
                visibilityScript.insertions.AddRange(item.insertions);
            }
        }

    }

    public void PartiallyDeleteNotSelected()
    {
        int i = 0;
        List<GameObject> toDelete = new List<GameObject>();
        foreach (var renderer in GlobalVariables.Instance.allBodyPartRenderers)
        {
            if (selectedObjectsManagement.selectedObjects.Count > 0 && !selectedObjectsManagement.selectedObjects.Contains(renderer.gameObject) && renderer.CompareTag(selectedObjectsManagement.selectedObjects[0].tag))
            {
                toDelete.Add(renderer.gameObject);
            }
            i++;
        }
        selectedObjectsManagement.DeleteList(toDelete);
    }

    public void IsolationClick(bool showLabels = true)
    {
        // - The selected objects are the same as isolated objects
        bool case1 = SelectedObjectsManagement.Instance.selectedObjects.SequenceEqual(SelectedObjectsManagement.Instance.lastIsolatedObjects);
        bool case2 = false;
        bool case3 = false;

        bool onlyOneIsolated = SelectedObjectsManagement.Instance.lastIsolatedObjects.Where(it => it.IsBodyPart()).Count() == 1;

        if (!case1 && onlyOneIsolated)
        {
            GameObject lastIsolated = SelectedObjectsManagement.Instance.lastIsolatedObjects.Find(it => it.IsBodyPart());

            // - Or there's only one bodypart selected and it's the same as the prev one
            case2 = onlyOneIsolated
                && SelectedObjectsManagement.Instance.selectedObjects.Where(it => it.IsBodyPart()).Count() == 1
                && lastIsolated == SelectedObjectsManagement.Instance.selectedObjects.Find(it => it.IsBodyPart());

            if (!case2)
            {
                // - Or there's no selected object and the active is the isolated one
                case3 = SelectedObjectsManagement.Instance.selectedObjects.Count == 0
                    && onlyOneIsolated
                    && SelectedObjectsManagement.Instance.activeObjects.Where(it => it.IsBodyPart()).Count() == 1
                    && lastIsolated == SelectedObjectsManagement.Instance.activeObjects.Find(it => it.IsBodyPart());
            }
        }


        //Can undo isolation?
        bool canUndoIsolation = case1 || case2 || case3;

        if (canUndoIsolation)
        {

            SelectedObjectsManagement.Instance.lastIsolatedObjects.Clear();

            return;
        }

        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 1)
        {
            //IF it is label, we select its parent too
            Label label = SelectedObjectsManagement.Instance.selectedObjects[0].GetComponent<Label>();
            if (label != null)
            {
                if (!SelectedObjectsManagement.Instance.selectedObjects.Contains(label.parent.gameObject))
                {
                    SelectedObjectsManagement.Instance.SelectObject(label.parent.gameObject);
                }
            }
        }

        SelectedObjectsManagement.Instance.lastIsolatedObjects = new List<GameObject>(SelectedObjectsManagement.Instance.selectedObjects);

        foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
            item.transform.SetActiveParentsRecursively(true);

        SelectedObjectsManagement.Instance.GetActiveObjects();
        SelectedObjectsManagement.Instance.DeleteOutlineToActiveObjects();

        ActionControl.Instance.UpdateButtons();
    }

    public void PartialIsolationClick()
    {

        //Can undo isolation
        if (SelectedObjectsManagement.Instance.selectedObjects.SequenceEqual(SelectedObjectsManagement.Instance.lastIsolatedObjects))
        {
            SelectedObjectsManagement.Instance.lastIsolatedObjects.Clear();
            return;
        }

        SelectedObjectsManagement.Instance.lastIsolatedObjects = new List<GameObject>(SelectedObjectsManagement.Instance.selectedObjects);

        PartiallyDeleteNotSelected();
        selectedObjectsManagement.DeleteOutlineToSelectedObjects();
        foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
        {
            item.transform.SetActiveParentsRecursively(true);
            item.gameObject.layer = 6;
        }

    }

    private List<GameObject> NotSelected()
    {
        List<GameObject> toDelete = new List<GameObject>();
        foreach (BodyPartVisibility script in GlobalVariables.Instance.allVisibilityScripts)
        {
            if (!selectedObjectsManagement.selectedObjects.Contains(script.gameObject) && script.gameObject.activeSelf)
                toDelete.Add(script.gameObject);
        }
        return toDelete;
    }

    public void EnableInsertions()
    {
        List<GameObject> shown = new List<GameObject>();
        foreach (var insertion in GlobalVariables.Instance.insertions)
        {
            if (!insertion.gameObject.activeInHierarchy)
                shown.Add(insertion.gameObject);
        }
    }

    public void DisableInsertions()
    {
        List<GameObject> hidden = new List<GameObject>();
        foreach (var insertion in GlobalVariables.Instance.insertions)
        {
            if (insertion.gameObject.activeInHierarchy)
                hidden.Add(insertion.gameObject);
        }
    }

    public void HideAllLabels()
    {
        foreach (var child in GlobalVariables.Instance.allBodyParts)
        {
            if (child != null)
                child.GetComponent<BodyPartVisibility>().HideLabels();
        }
    }
}
