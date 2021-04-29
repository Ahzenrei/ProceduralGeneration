using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundToMesh))]
[RequireComponent(typeof(BezierCurve))]
public class PointGenerator : MonoBehaviour
{

    public float secondsBeforeUpdate = 0.5f;

    BezierCurve bezier;
    SoundToMesh soundToMesh;
    // Start is called before the first frame update
    void Start()
    {
        bezier = GetComponent<BezierCurve>();
        soundToMesh = GetComponent<SoundToMesh>();
        StartCoroutine(UpdatePoint());
    }

    IEnumerator UpdatePoint()
    {
        while (true)
        {
            CreatePoint();
            yield return new WaitForSeconds(secondsBeforeUpdate);
        }
    }

    public void CreatePoint()
    {
        if (bezier == null)
        {
            bezier = GetComponent<BezierCurve>();
        }


        float vertical = Random.Range(-16, 16);
        float horizontal = Random.Range(-16, 16);

        Vector3 point = new Vector3(0, vertical, horizontal);
        bezier.AddPoint(point);
        bezier.DeleteOldPoint();
    }
    public void Reset()
    {
        if (bezier == null)
        {
            bezier = GetComponent<BezierCurve>();
        }

        bezier.Reset();
    }
}
