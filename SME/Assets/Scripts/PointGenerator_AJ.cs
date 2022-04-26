using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGenerator_AJ : MonoBehaviour
{
    public static PointGenerator_AJ Instance;
    [SerializeField] private float m_step;
    [SerializeField] private float m_start;
    [SerializeField] private float m_stop;
    private List<Vector3> m_points;
    // Start is called before the first frame update
    void Awake()
    {
        this.m_points = new List<Vector3>();
        PointGenerator_AJ.Instance = this;
    }

    public Vector3[] GetPoints()
    {
        this.m_points = new List<Vector3>();
        float t = this.m_start;
        while (t < this.m_stop)
        {
            var x = t;
            var y = this.GetYFromEq(x);

            var hold = new Vector3(x, y, 0f);

            this.m_points.Add(hold);

            t += this.m_step;
        }
        return this.m_points.ToArray();
    }
    public bool LoadPoints()
    {
        this.m_points = new List<Vector3>();
        float t = this.m_start;
        while (t < this.m_stop)
        {
            var x = t;
            var y = this.GetYFromEq(x);

            var hold = new Vector3(x, y, 0f);

            this.m_points.Add(hold);

            t += this.m_step;
        }
        return true;
    }

    public Vector3[] GetPointList()
    {
        return this.m_points.ToArray();

    }

    public float GetYFromEq(float x)
    {
        var x_2 = x * x;
        var y = Mathf.Sin(x_2)+2;
        return y;
    }

    public Vector2 GetSplineError(float splineVal, float t)
    {
        float x2 = t * t;
        float realVal = Mathf.Sin(x2)+2;

        float ret = Mathf.Abs(splineVal - realVal);
        return new Vector2(ret, ret/Mathf.Abs(realVal)); // relative error |ret|/|realVal|
    }

    public Vector3 GetStepVal()
    {
        var ret = new Vector3(this.m_step, this.m_start, this.m_stop);
        return ret;
    }
}
