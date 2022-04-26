using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;
public class BS_AJ_PointLoader : MonoBehaviour
{
    private BezierSpline _spline;
    [SerializeField] private float _calcStep;

    [SerializeField] private Vector2 _errorMean;
    [SerializeField] private Vector2 _variance;
    [SerializeField] private Vector2 _stdDev;
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
        var totalError = new Vector2(0f, 0f);
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

        var totalVar = new Vector2(0f, 0f);
        t = start;
        while (t < stop)
        {
            var normT = (t - start) / tDiff;

            var splineVal = this._spline.GetPoint(normT).y;

            var errorVal = PointGenerator_AJ.Instance.GetSplineError(splineVal, t);

            var diffX = (errorVal.x - this._errorMean.x);
            var diffY = (errorVal.y - this._errorMean.y);

            totalVar.x += diffX * diffX;
            totalVar.y += diffY * diffY;

            t += this._calcStep;
        }

        this._variance = totalVar / n;
        this._stdDev.x = Mathf.Sqrt(this._variance.x);
        this._stdDev.y = Mathf.Sqrt(this._variance.y);
    }
}
