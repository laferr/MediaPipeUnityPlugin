using System.Collections;
using System.Collections.Generic;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;
using UnityEngine;

public class CalibrationTimer : MonoBehaviour
{
    //public PipeServer server;
    public int timer = 5;
    public KeyCode calibrationKey = KeyCode.C;

    private bool calibrated;

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(calibrationKey))
        {
            if(!calibrated)
            {
                calibrated = true;
                StartCoroutine(Timer());
            }
            else
            {
                StartCoroutine(Notify());
            }
        }
    }
    private IEnumerator Timer()
    {
        int t = timer;
        while (t > 0)
        {
            yield return new WaitForSeconds(1f);
            --t;
        }
        //PoseLandmarkerResultController[] a = FindObjectsByType<PoseLandmarkerResultController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        //foreach(PoseLandmarkerResultController aa in a)
        //{
            //aa.Calibrate();
        //}
        //if (a.Length>0)
        //{
        //}
        //else
        //{
        //}
        yield return new WaitForSeconds(1.5f);
    }
    private IEnumerator Notify()
    {
        yield return new WaitForSeconds(3f);
    }
}
