using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    [SerializeField] private Material roadMat;
    [SerializeField] private Material sideRoadMat;

    //private int internCurveCount = 1;

    private Vector3[] curvePoints;
    private List<Vector3> points = null;

    [SerializeField] private float _pointDistance = 64;
    [SerializeField] private float _scrollingSpeed = 3f;
    public float PointDistance { get { return _pointDistance; } }
    //we have our 4 first point then we add 3 points per curve
    public int CurveCount { get { return (points.Count - 4) / 3 + 1; } }
    public int SamplePerCurve { get { return sampleCount / CurveCount; } }

    [Range(20, 500)] [SerializeField] private int sampleCount = 10;
    [Range(0, 1)] [SerializeField] private float radiusSphere = 0.1f;
    [Range(1, 10)] [SerializeField] private int maxCurveCount = 3;

    private Texture2D texture;
    private Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        curvePoints = new Vector3[SamplePerCurve * CurveCount];

        points = new List<Vector3>();

        points.Add(new Vector3(0, 0, 0));
        points.Add(new Vector3(PointDistance / 3, 0, 0));
        points.Add(new Vector3(2 * PointDistance / 3, 0, 0));
        points.Add(new Vector3(PointDistance, 0, 0));

        texture = new Texture2D(curvePoints.Length, 1, TextureFormat.RGBAFloat, false);
        texture.name = "Curve";
        CalculateBezierCurve();
        LoadTexture();
    }

    private void Update()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i] -= Vector3.right * _scrollingSpeed * Time.deltaTime;
        }

        curvePoints = new Vector3[SamplePerCurve * CurveCount];
        texture = new Texture2D(curvePoints.Length, 1, TextureFormat.RGBAFloat, false);
        texture.name = "Curve";

        CalculateBezierCurve();
        LoadTexture();
    }

    void CalculateBezierCurve()
    {
        int samplePerCurve = SamplePerCurve;
        for (int i = 0; i < CurveCount; i++)
        {
            for (int j = 0; j < samplePerCurve; j++)
            {
                float t = (float)j / samplePerCurve;
                curvePoints[i * samplePerCurve + j] = CubicCurve(points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[i * 3 + 3], t);
            }
        }
    }

    //Every point of the curve has two control point, unless it is the end of the curve
    public void AddPoint(Vector3 nextPoint)
    {
        //internCurveCount++;
        nextPoint.x += points[points.Count - 1].x + PointDistance;

        //The last point isn't anymore the last one so we add a control point to it
        Vector3 nextControlPoint = points[points.Count - 1] * 2 - points[points.Count - 2];

        points.Add(nextControlPoint);

        //We add the first control point of the new point
        points.Add((nextControlPoint + nextPoint) / 2);

        //We add the new point which is the new end of the curve
        points.Add(nextPoint);

        curvePoints = new Vector3[SamplePerCurve * CurveCount];
        texture = new Texture2D(curvePoints.Length, 1, TextureFormat.RGBAFloat, false);
        texture.name = "Curve";

        CalculateBezierCurve();
        LoadTexture();
    }

    public void DeleteOldPoint()
    {
        while (maxCurveCount < CurveCount)
        {
            points.RemoveRange(0, 3);
        }
    }

    //public void DeletePoints()
    //{
    //    points.RemoveRange(0, 3);
    //}

    private Vector3 LerpVector3(Vector3 v1, Vector3 v2, float t)
    {
        Vector3 result = Vector3.zero;

        result.x = Mathf.Lerp(v1.x, v2.x, t);
        result.y = Mathf.Lerp(v1.y, v2.y, t);
        result.z = Mathf.Lerp(v1.z, v2.z, t);

        return result;
    }

    private Vector3 QuadraticCurve(Vector3 v1, Vector3 v2, Vector3 v3, float t)
    {
        Vector3 p1 = LerpVector3(v1, v2, t);
        Vector3 p2 = LerpVector3(v2, v3, t);
        return LerpVector3(p1, p2, t);
    }

    private Vector3 CubicCurve(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float t)
    {
        Vector3 p1 = QuadraticCurve(v1, v2, v3, t);
        Vector3 p2 = QuadraticCurve(v2, v3, v4, t);
        return LerpVector3(p1, p2, t);
    }


    void LoadTexture()
    {
        int i = 0;
        foreach (Vector3 point in curvePoints)
        {
            texture.SetPixel(i, 0, new Color(point.x, point.y, point.z));
            i++;
        }

        texture.Apply();
        roadMat.SetTexture("_Curve", texture);
        sideRoadMat.SetTexture("_Curve", texture);
    }

    public void Reset()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;

        //internCurveCount = 1;

        points = new List<Vector3>();

        points.Add(new Vector3(0, 0, 0));
        points.Add(new Vector3(PointDistance / 3, 0, 0));
        points.Add(new Vector3(2 * PointDistance / 3, 0, 0));
        points.Add(new Vector3(PointDistance, 0, 0));

        curvePoints = new Vector3[SamplePerCurve * CurveCount];

        CalculateBezierCurve();

        texture = new Texture2D(curvePoints.Length, 1, TextureFormat.RGBAFloat, false);
        texture.name = "Curve";

        LoadTexture();
    }

    private void OnValidate()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;

        if (points == null)
        {
            points = new List<Vector3>();

            points.Add(new Vector3(0, 0, 0));
            points.Add(new Vector3(PointDistance / 3, 0, 0));
            points.Add(new Vector3(2 * PointDistance / 3, 0, 0));
            points.Add(new Vector3(PointDistance, 0, 0));
        }

        curvePoints = new Vector3[SamplePerCurve * CurveCount];

        CalculateBezierCurve();

        texture = new Texture2D(curvePoints.Length, 1, TextureFormat.RGBAFloat, false);
        texture.name = "Curve";

        LoadTexture();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (Vector3 point in points)
        {
            Gizmos.DrawSphere(point, radiusSphere);
        }

        Gizmos.color = Color.green;

        for (int i = 1; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i - 1], points[i]);
        }

    }
}
