using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

namespace SplineMesh {
    //[ExecuteInEditMode]
    public class SM_HW_PointLoader : MonoBehaviour
    {
        // Start is called before the first frame update
        private GameObject generated;
        private Spline _Base_Spline = null;

        [SerializeField] private float _calcStep;

        [SerializeField] private Vector2 _ErrorMean;
        [SerializeField] private Vector2 _variance;
        [SerializeField] private Vector2 _stdDev;
        private int maybeDont = 0;
        void Start()
        {
            this._Base_Spline = GetComponent<Spline>(); 
            PointGenerator_AJ.Instance.LoadPoints();
            Vector3[] Points = PointGenerator_AJ.Instance.GetPointList();
            Debug.Log(Points);
            foreach(Vector3 pos in Points){
                Vector3 dir = new Vector3(pos.x,getY_prime(pos.x),0);
                Debug.Log("pos x "+pos.x+" dir "+dir);
                SplineNode newNode = new SplineNode(pos,pos);//dir.normalized);
                this._Base_Spline.AddNode(newNode);

            }  
            // removing the initial points in the spline  
            for(int i=0; i<2; i++){
                SplineNode unwanted_node = this._Base_Spline.nodes[0];
                this._Base_Spline.RemoveNode(unwanted_node);
            }
            var values = PointGenerator_AJ.Instance.GetStepVal();

            var step = values.x;
            var start = values.y;
            var stop = values.z;

            var tDiff = stop - start;
            var totalError = 0f;
            var totalRelError = 0f;
            var t = start;
            var n = 0;
            var numNodes = Points.Length;
            Debug.Log("the while loop has started");
            Debug.Log(numNodes);
            List<float> splinevals = new List<float>();
            List<Vector2> splineErrors = new List<Vector2>();
            while (t < stop)
            {
                var normT = (t - start) / tDiff;
                var Time = normT*numNodes;
                if(Time > numNodes-1){
                    Debug.Log("Time was set to"+Time);
                    Time = numNodes-1;
                }
                var other_version_of_Time =  Time - Mathf.FloorToInt(Time);

                var splineVal = this._Base_Spline.GetCurve(Time).GetLocation(other_version_of_Time).y;
                splinevals.Add(splineVal);
                
                var errorVal = PointGenerator_AJ.Instance.GetSplineError(splineVal, t);
                splineErrors.Add(errorVal);
                totalError += errorVal.x;
                totalRelError += errorVal.y;

                n++;
                if(this._calcStep == 0){
                    this._calcStep = 0.01f;
                }
                t += this._calcStep;
                
            }
            Debug.Log("the while loop is done");
            this._ErrorMean = new Vector2(totalError / n, totalRelError / n);

            //calc variance
            
            var totalVar = new Vector2(0f,0f);
            t = start;
            int n2 = 0;
            while (t < stop)
            {
                var diff1 = (splineErrors[n2].x - this._ErrorMean.x);
                var diff2 = (splineErrors[n2].y - this._ErrorMean.y);
                totalVar.x += diff1 * diff1;
                totalVar.y += diff2 * diff2;
                t += this._calcStep;
                n2++;
            }
            this._variance.x = totalVar.x / n;
            this._variance.y = totalVar.y / n;
            this._stdDev.x = Mathf.Sqrt(this._variance.x);
            this._stdDev.y = Mathf.Sqrt(this._variance.y);
            
        }

        // Update is called once per frame
        void Update()
        {
            if(maybeDont%600 == 599){
            Vector3[] Points = PointGenerator_AJ.Instance.GetPointList();
            
            var values = PointGenerator_AJ.Instance.GetStepVal();

            var step = values.x;
            var start = values.y;
            var stop = values.z;

            var tDiff = stop - start;
            var totalError = 0f;
            var totalRelError = 0f;
            var t = start;
            var n = 0;
            var numNodes = Points.Length;
            Debug.Log("the while loop has started");
            Debug.Log(numNodes);
            List<float> splinevals = new List<float>();
            List<Vector2> splineErrors = new List<Vector2>();
            while (t < stop)
            {
                var normT = (t - start) / tDiff;
                var Time = normT*numNodes;
                if(Time > numNodes-1){
                    Debug.Log("Time was set to"+Time);
                    Time = numNodes-1;
                }
                var other_version_of_Time =  Time - Mathf.FloorToInt(Time);

                var splineVal = this._Base_Spline.GetCurve(Time).GetLocation(other_version_of_Time).y;
                splinevals.Add(splineVal);
                
                var errorVal = PointGenerator_AJ.Instance.GetSplineError(splineVal, t);
                splineErrors.Add(errorVal);
                totalError += errorVal.x;
                totalRelError += errorVal.y;

                n++;
                if(this._calcStep == 0){
                    this._calcStep = 0.01f;
                }
                t += this._calcStep;
                
            }
            Debug.Log("the while loop is done");
            this._ErrorMean = new Vector2(totalError / n, totalRelError / n);

            //calc variance
            
            var totalVar = new Vector2(0f,0f);
            t = start;
            int n2 = 0;
            while (t < stop)
            {
                var diff1 = (splineErrors[n2].x - this._ErrorMean.x);
                var diff2 = (splineErrors[n2].y - this._ErrorMean.y);
                totalVar.x += diff1 * diff1;
                totalVar.y += diff2 * diff2;
                t += this._calcStep;
                n2++;
            }
            this._variance.x = totalVar.x / n;
            this._variance.y = totalVar.y / n;
            this._stdDev.x = Mathf.Sqrt(this._variance.x);
            this._stdDev.y = Mathf.Sqrt(this._variance.y);
            }
            maybeDont ++;
        }
        

        public float getY_prime(float x){
            return 2*x*Mathf.Cos(x*x);
        }
    }
}