using GK;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using UnityEngine.Profiling;
using System;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using Mono.Cecil.Cil;
using UnityEditor;

public class LabelCollider : MonoBehaviour
{

    [HideInInspector]
    public Label label;
    [HideInInspector]
    public Transform parent;
    [HideInInspector]
    public TangibleBodyPart tangibleBodyPart;



    [Header("CLICK TO UPDATE MESH")]
    public bool update;
    public bool debug = false;

    [Header("-------------OPTIONS-------------")]

    [Header("The mesh size")]
    [Range(0.01f, 2f)]
    public float diameter = 0.25f;
    [Header("The position offset")]
    [Range(-0.1f, 0.1f)]
    public float offset = 0;

    [Header("--------ADVANCED OPTIONS--------")]

    [Header("Delete heavily rotated vertices")]
    [InspectorName("Enable")]
    public bool angleThresholdEnabled = false;
    [InspectorName("Invert")]
    public bool invertAngle = false;
    [Range(0f, 4)]
    public float angleThresholdMultiplier = 1;

    [Header("Limiting plane")]
    [InspectorName("Enable")]
    public bool enablePlane = false;
    [InspectorName("Invert")]
    public bool invertPlane = false;
    [Range(-0.25f, 0.25f)]
    public float planeOffset = -0.025f;

    [Header("Infalte the mesh (It doesn't work well with the angle threshold)")]
    [Range(-0.1f, 0.1f)]
    public float inflate = 0.01f;

    private Matrix4x4 localToWorld;
    private UnityEngine.Color randomColor = Color.black;
    private List<Vector3> intersectingVertices;
    private MeshFilter mf;
    private MeshCollider mc;
    private MeshRenderer mr;
    private Vector3 planeNormal = Vector3.zero;
    [HideInInspector]
    public string savePath;

    public void Build()
    {
        mf = gameObject.AddComponent<MeshFilter>();
        mr = gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<NameAndDescription>();
        gameObject.AddComponent<TangibleBodyPart>();
        gameObject.layer = LayerMask.NameToLayer("Body");
        label.line.GetPoints();
        transform.position = label.line.maxPoint.position;
        tangibleBodyPart = label.parent;
        transform.SetParent(parent);
        planeNormal = label.line.outDir;
        savePath = "Assets/Prefabs/Label Prefabs/Meshes/" + name + ".asset";
        BuildMesh();
    }

    public void Clicked()
    {
        LabelLevelManager.Instance.Clicked(this);
    }

    private void OnDrawGizmos()
    {
        if(debug)
        {
            //Draw the sphere
            UnityEngine.Color c = randomColor;
            c.a = .1f;
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position, diameter / 2);

            //Draw the plane
            float size = .1f;
            Vector3 planeTangent = Vector3.Cross(Vector3.up, planeNormal).normalized;
            Vector3 planeBitangent = Vector3.Cross(planeNormal, planeTangent).normalized;
            Vector3 planePosition = transform.position + planeNormal * planeOffset;

            Vector3 corner1 = planePosition - planeTangent * size + planeBitangent * size;
            Vector3 corner2 = planePosition + planeTangent * size + planeBitangent * size;
            Vector3 corner3 = planePosition + planeTangent * size - planeBitangent * size;
            Vector3 corner4 = planePosition - planeTangent * size - planeBitangent * size;

            Gizmos.DrawLine(corner1, corner2);
            Gizmos.DrawLine(corner2, corner3);
            Gizmos.DrawLine(corner3, corner4);
            Gizmos.DrawLine(corner4, corner1);
            Gizmos.DrawLine(planePosition, planePosition + planeNormal * size);
        }
    }

    private void OnValidate()
    {
        if (parent != null && update)
        {
            update = false;
            BuildMesh();
            transform.parent.position = Vector3.zero;
            transform.parent.rotation = Quaternion.identity;
        }
        
    }

    private void BuildMesh()
    {
        MeshFilter mf = tangibleBodyPart.GetComponentInParent<MeshFilter>();
        Vector3[] vertices = mf.sharedMesh.vertices;
        int[] triangles = mf.sharedMesh.triangles;
        localToWorld = tangibleBodyPart.transform.localToWorldMatrix;

        Vector3 planeNormal = label.line.outDir;
        if (invertPlane)
            planeNormal = -planeNormal;
        Vector3 planePosition = transform.position + planeNormal * planeOffset;

        GetIntersectingVertices(vertices, triangles, transform.position, diameter / 2, planePosition, planeNormal);
        CreateMeshWithIntersectingVertices(intersectingVertices);
    }

    private void GetIntersectingVertices(Vector3[] vertices, int[] triangles, Vector3 spherePosition, float sphereRadius, Vector3 planePosition, Vector3 planeNormal)
    {
        intersectingVertices = new List<Vector3>();
        Plane plane = new Plane(planeNormal, planePosition);

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v1 = localToWorld.MultiplyPoint3x4(vertices[triangles[i]]);
            Vector3 v2 = localToWorld.MultiplyPoint3x4(vertices[triangles[i + 1]]);
            Vector3 v3 = localToWorld.MultiplyPoint3x4(vertices[triangles[i + 2]]);
            Vector3 faceNormal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

            float adjustedRadius = sphereRadius;

            if (angleThresholdEnabled)
            {
                // Determine the angle between the face normal and the vector connecting the sphere and the centroid
                float angle = Mathf.Acos(Vector3.Dot(faceNormal, planeNormal));
                float proportionalAngle = Mathf.Pow(angle / Mathf.PI, angleThresholdMultiplier);
                if(invertAngle)
                    adjustedRadius = sphereRadius * (1 - proportionalAngle);
                else
                    adjustedRadius = sphereRadius * proportionalAngle;
            }

            // Check if any of the vertices are inside the  sphere
            bool v1InsideSphere = Vector3.Distance(v1, spherePosition) <= adjustedRadius;
            bool v2InsideSphere = Vector3.Distance(v2, spherePosition) <= adjustedRadius;
            bool v3InsideSphere = Vector3.Distance(v3, spherePosition) <= adjustedRadius;

            bool v1OnCorrectSide = true;
            bool v2OnCorrectSide = true;
            bool v3OnCorrectSide = true;

            if(enablePlane)
            {
                v1OnCorrectSide = plane.GetSide(v1);
                v2OnCorrectSide = plane.GetSide(v2);
                v3OnCorrectSide = plane.GetSide(v3);
            }

            if (v1InsideSphere && v2InsideSphere && v3InsideSphere && (!enablePlane || v1OnCorrectSide && v2OnCorrectSide && v3OnCorrectSide))
            {
                intersectingVertices.Add(v1);
                intersectingVertices.Add(v2);
                intersectingVertices.Add(v3);
            }
            else
            {
                if (v1InsideSphere && (v1OnCorrectSide || !enablePlane))
                    intersectingVertices.Add(v1);
                if (v2InsideSphere && (v2OnCorrectSide || !enablePlane))
                    intersectingVertices.Add(v2);
                if (v3InsideSphere && (v3OnCorrectSide || !enablePlane))
                    intersectingVertices.Add(v3);
                if ((v1InsideSphere && !v2InsideSphere && !v3InsideSphere) || (v2InsideSphere && !v1InsideSphere && !v3InsideSphere) || (v3InsideSphere && !v1InsideSphere && !v2InsideSphere))
                {
                    Vector3 intersectionPoint;
                    if (SphereEdgeIntersection(v1, v2, spherePosition, adjustedRadius, out intersectionPoint) && (!enablePlane ||plane.GetSide(intersectionPoint)))
                        intersectingVertices.Add(intersectionPoint);

                    if (SphereEdgeIntersection(v2, v3, spherePosition, adjustedRadius, out intersectionPoint) && (!enablePlane || plane.GetSide(intersectionPoint)))
                        intersectingVertices.Add(intersectionPoint);

                    if (SphereEdgeIntersection(v3, v1, spherePosition, adjustedRadius, out intersectionPoint) && (!enablePlane || plane.GetSide(intersectionPoint)))
                        intersectingVertices.Add(intersectionPoint);
                }
            }
        }
    }


    private bool SphereEdgeIntersection(Vector3 v1, Vector3 v2, Vector3 spherePosition, float sphereRadius, out Vector3 intersectionPoint)
    {
        Vector3 edge = v2 - v1;
        Vector3 sphereToEdge = v1 - spherePosition;

        float a = Vector3.Dot(edge, edge);
        float b = 2f * Vector3.Dot(sphereToEdge, edge);
        float c = Vector3.Dot(sphereToEdge, sphereToEdge) - sphereRadius * sphereRadius;

        float discriminant = b * b - 4f * a * c;

        if (discriminant < 0f)
        {
            intersectionPoint = Vector3.zero;
            return false;
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float t1 = (-b - sqrtDiscriminant) / (2f * a);
        float t2 = (-b + sqrtDiscriminant) / (2f * a);

        if (t1 >= 0f && t1 <= 1f)
        {
            intersectionPoint = v1 + t1 * edge;
            return true;
        }

        if (t2 >= 0f && t2 <= 1f)
        {
            intersectionPoint = v1 + t2 * edge;
            return true;
        }

        intersectionPoint = Vector3.zero;
        return false;

    }

    private void InflateMesh(Mesh mesh)
    {
        // Copia la información de vértices del mesh original
        Vector3[] originalVertices = mesh.vertices;
        Vector3[] inflatedVertices = new Vector3[originalVertices.Length];

        // Calcula la posición inflada de cada vértice
        for (int i = 0; i < originalVertices.Length; i++)
        {
            inflatedVertices[i] = originalVertices[i] + (originalVertices[i].normalized * inflate);
        }

        // Actualiza los vértices del mesh con los vértices inflados
        mesh.vertices = inflatedVertices;

        // Recalcula las normales y la información de tangentes para que el mesh inflado se renderice correctamente
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    private void CreateMeshWithIntersectingVertices(List<Vector3> intersectingVertices)
    {
        ConvexHullCalculator calculator = new ConvexHullCalculator();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector3> verts = new List<Vector3>();
        if (intersectingVertices.Count < 4)
        {
            Debug.LogError(name + ": <color=#FF0000>Mesh not generated.</color> Try to modify its parameters.");
            return;
        }
        try
        {
            calculator.GenerateHull(intersectingVertices.ConvertAll(it => it - transform.localPosition), true, ref verts, ref triangles, ref normals, 5000, name);
        }
        catch
        {
            Debug.Log(name +  ": Mesh not generated. Trying to generate again with different parameters...");
            //Make some changes
            if (angleThresholdEnabled)
            {
                angleThresholdEnabled = false;
                BuildMesh();
            }
            else if (diameter == 0.25f)
            {
                diameter += 0.1f;
                BuildMesh();
            }
            else
                Debug.LogError(name + ": <color=#FF0000>Mesh not generated</color>. Try to modify its parameters.");
            return;
        }

        //Set offset
        transform.position += label.line.outDir * offset;

        if (verts.Count < 2000)
            Debug.Log(name + " mesh generated: <color=#7FFF00>" + verts.Count + "</color> verts");
        else
            Debug.Log(name + " mesh generated: <color=#FFFF00>" + verts.Count + "</color> verts");
        Mesh intersectingMesh = new Mesh();
        intersectingMesh.vertices = verts.ToArray();
        intersectingMesh.triangles = triangles.ToArray();
        // Re-calculate the normals and bounds of the mesh
        intersectingMesh.RecalculateNormals();
        intersectingMesh.RecalculateBounds();

        if (inflate != 0)
            InflateMesh(intersectingMesh);

        intersectingMesh.Optimize();

        // Set a random color for the mesh
        // Assign the new mesh to a MeshFilter component on a new GameObject


        AssetDatabase.CreateAsset(intersectingMesh, savePath);

        mf.sharedMesh = intersectingMesh;
        mf.sharedMesh.name = name;
        UpdateMaterial();
        if (mc == null)
            mc = gameObject.AddComponent<MeshCollider>();
        else
            mc.sharedMesh = intersectingMesh;
    }

    public void UpdateMaterial()
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetFloat("_Surface", 1);
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        if (randomColor == Color.black)
            randomColor = new Color(Random.value, Random.value, Random.value);
        randomColor.a = debug ? 1 : 0;
        mat.color = randomColor;
        if(mr == null)
            mr = GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        mr.shadowCastingMode = ShadowCastingMode.Off;
    }
}
