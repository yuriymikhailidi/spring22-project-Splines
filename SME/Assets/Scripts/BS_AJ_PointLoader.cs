using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;
public class BS_AJ_PointLoader : MonoBehaviour
{
    private BezierSpline _spline;
    // Start is called before the first frame update
    void Start()
    {
        this._spline = new GameObject().AddComponent<BezierSpline>();
        StartCoroutine(this.InitSpline());
    }


    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator InitSpline()
    {
        yield return new WaitUntil(PointGenerator_AJ.Instance.LoadPoints);

        var pntList = PointGenerator_AJ.Instance.GetPointList();

        int len = pntList.Length;
        this._spline.Initialize(len);
        this._spline.loop = false;
        for (int i = 0; i < len; i++)
        {
            this._spline[i].position = pntList[i];
        }
        this._spline.AutoConstructSpline2();
    }
}
