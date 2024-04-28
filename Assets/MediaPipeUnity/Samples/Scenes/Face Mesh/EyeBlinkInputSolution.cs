using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe.Unity.Sample.FaceMesh
{
  public class EyeBlinkInputSolution : MonoBehaviour
  {
    public GameObject leftText;
    public GameObject rightText;

    private Vector2 leftEyeUpperLid;
    private Vector2 leftEyeLowerLid;
    private Vector2 leftEyeInner;
    private Vector2 leftEyeOuter;

    private Vector2 rightEyeUpperLid;
    private Vector2 rightEyeLowerLid;
    private Vector2 rightEyeInner;
    private Vector2 rightEyeOuter;

    private bool isLeftOpened = false;
    private bool isRightOpened = false;
    public void CalcNow(List<NormalizedLandmarkList> landmarks)
    {
      leftEyeUpperLid = new Vector2(landmarks[0].Landmark[386].X, landmarks[0].Landmark[386].Y);
      leftEyeLowerLid = new Vector2(landmarks[0].Landmark[374].X, landmarks[0].Landmark[374].Y);
      leftEyeInner = new Vector2(landmarks[0].Landmark[362].X, landmarks[0].Landmark[362].Y);
      leftEyeOuter = new Vector2(landmarks[0].Landmark[263].X, landmarks[0].Landmark[263].Y);
                
      rightEyeUpperLid = new Vector2(landmarks[0].Landmark[159].X, landmarks[0].Landmark[159].Y);
      rightEyeLowerLid = new Vector2(landmarks[0].Landmark[145].X, landmarks[0].Landmark[145].Y);
      rightEyeInner = new Vector2(landmarks[0].Landmark[133].X, landmarks[0].Landmark[133].Y);
      rightEyeOuter = new Vector2(landmarks[0].Landmark[33].X, landmarks[0].Landmark[33].Y);
      //Debug.Log(Vector2.Distance(leftEyeUpperLid, leftEyeLowerLid) / Vector2.Distance(leftEyeInner, leftEyeOuter));

    }

    private void Update()
    {
      if (!isLeftOpened && Vector2.Distance(leftEyeUpperLid, leftEyeLowerLid) / Vector2.Distance(leftEyeInner, leftEyeOuter) < 0.2f)
      {
        //Debug.Log("Blink Left");
        leftText.SetActive(true);
        isLeftOpened = true;
      }
      else if (isLeftOpened && Vector2.Distance(leftEyeUpperLid, leftEyeLowerLid) / Vector2.Distance(leftEyeInner, leftEyeOuter) >= 0.2f)
      {
        leftText.SetActive(false);
        isLeftOpened = false;
      }
      
      if (!isRightOpened && Vector2.Distance(rightEyeUpperLid, rightEyeLowerLid) / Vector2.Distance(rightEyeInner, rightEyeOuter) < 0.2f)
      {
        //Debug.Log("Blink Left");
        rightText.SetActive(true);
        isRightOpened = true;
      }
      else if (isRightOpened && Vector2.Distance(rightEyeUpperLid, rightEyeLowerLid) / Vector2.Distance(rightEyeInner, rightEyeOuter) >= 0.2f)
      {
        rightText.SetActive(false);
        isRightOpened = false;
      }
    }
  }
}
