using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;
public class BS_AJ_PointLoader : MonoBehaviour
{
    private BezierSpline _spline;
    [SerializeField] private float _calcStep;

    [SerializeField] private float _errorMean;
    [SerializeField] private float _variance;
    [SerializeField] private float _stdDev;
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
        var values = PointGenerator_AJ.Instance.GetStepVal();

        var step = values.x;
        var start = values.y;
        var stop = values.z;

        int len = pntList.Length;
        this._spline.Initialize(len);
        this._spline.loop = false;
        for (int i = 0; i < len; i++)
        {
            this._spline[i].position = pntList[i];
        }
        //adjust control points
        this._spline.AutoConstructSpline2();

        var tDiff = stop - start;
        var totalError = 0f;
        var t = start;
        var n = 0;
        while (t < stop)
        {
            var normT = (t - start) / tDiff;

            var splineVal = this._spline.GetPoint(normT).y;

            var errorVal = PointGenerator_AJ.Instance.GetSplineError(splineVal, t);

            totalError += errorVal;

            n++;
            t += this._calcStep;
        }

        this._errorMean = totalError / n;

        //calc variance

        var totalVar = 0f;
        t = start;
        while (t < stop)
        {
            var normT = (t - start) / tDiff;

            var splineVal = this._spline.GetPoint(normT).y;

            var errorVal = PointGenerator_AJ.Instance.GetSplineError(splineVal, t);

            var diff = (errorVal - this._errorMean);

            totalVar += diff * diff;

            t += this._calcStep;
        }

        this._variance = totalVar / n;
        this._stdDev = Mathf.Sqrt(this._variance);
    }
}
