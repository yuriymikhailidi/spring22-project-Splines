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

        [SerializeField] private float _errorMean;
        [SerializeField] private float _variance;
        [SerializeField] private float _stdDev;
        private int MaybeDont = 0;

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
            Debug.Log(this._Base_Spline.Length);
            var values = PointGenerator_AJ.Instance.GetStepVal();

            var step = values.x;
            var start = values.y;
            var stop = values.z;

            var tDiff = stop - start;
            var totalError = 0f;
            var t = start;
            var n = 0;
            var numNodes = Points.Length;
            Debug.Log("the while loop has started");
            Debug.Log(numNodes);
            List<float> splinevals = new List<float>();
            List<float> splineErrors = new List<float>();
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
                totalError += errorVal;

                n++;
                if(this._calcStep == 0){
                    this._calcStep = 0.01f;
                }
                t += this._calcStep;
                
            }
            Debug.Log("the while loop is done");
            this._errorMean = totalError / n;

            //calc variance
            
            var totalVar = 0f;
            t = start;
            int n2 = 0;
            while (t < stop)
            {
                var diff = (splineErrors[n2] - this._errorMean);

                totalVar += diff * diff;

                t += this._calcStep;
                n2++;
            }

            this._variance = totalVar / n;
            this._stdDev = Mathf.Sqrt(this._variance);
            
        }

        // Update is called once per frame
        void Update()
        {
            if(MaybeDont%600==599){// only call this ~ once every 10 seconds
                Vector3[] Points = PointGenerator_AJ.Instance.GetPointList();
                var values = PointGenerator_AJ.Instance.GetStepVal();

                var step = values.x;
                var start = values.y;
                var stop = values.z;

                var tDiff = stop - start;
                var totalError = 0f;
                var t = start;
                var n = 0;
                var numNodes = Points.Length;
                Debug.Log("the while loop has started");
                Debug.Log(numNodes);
                List<float> splinevals = new List<float>();
                List<float> splineErrors = new List<float>();
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
                    totalError += errorVal;

                    n++;
                    if(this._calcStep == 0){
                        this._calcStep = 0.01f;
                    }
                    t += this._calcStep;
                    
                }
                Debug.Log("the while loop is done");
                this._errorMean = totalError / n;

                //calc variance
                
                var totalVar = 0f;
                t = start;
                int n2 = 0;
                while (t < stop)
                {
                    var diff = (splineErrors[n2] - this._errorMean);

                    totalVar += diff * diff;

                    t += this._calcStep;
                    n2++;
                }

                this._variance = totalVar / n;
                this._stdDev = Mathf.Sqrt(this._variance);
            }
            MaybeDont++;
        }

        public float getY_prime(float x){
            return 2*x*Mathf.Cos(x*x);
        }
    }
}