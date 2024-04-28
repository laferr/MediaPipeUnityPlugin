using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Tasks.Vision.FaceLandmarker;

namespace Mediapipe.Unity.Sample.FaceLandmarkDetection
{
  public class FaceBlendshapeResultController : MonoBehaviour
  {
    [SerializeField] private SkinnedMeshRenderer _face;
    [SerializeField] private GameObject _faceRoot;
    [SerializeField] private GameObject _leftEye;
    [SerializeField] private GameObject _rightEye;
    [SerializeField] private int[] blendshapeIndex = new int[52];
    private FaceLandmarkerResult _currentTarget;

    public RectTransform annotationLayer;

    private bool isTracking = false;

    public bool mocap = false;

    private void Awake()
    {
      
    }

    public void DrawNow(FaceLandmarkerResult target)
    {
      _currentTarget = target;
      
      //Debug.Log(_currentTarget.faceBlendshapes[0].categories[0].score);

      isTracking = true;
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.S))
      {
        mocap = true;
      }
      
      if (isTracking && mocap)
      {
        //faceBlendshapeMocap
        for (int i = 1; i < blendshapeIndex.Length; i++)
        {
          _face.SetBlendShapeWeight(i - 1, _currentTarget.faceBlendshapes[0].categories[blendshapeIndex[i]].score * 100);
        }
        
        //Debug.Log(_currentTarget.faceLandmarks[0].landmarks[152].z);
        //169 389 - 152
        Vector3 leftFace = new Vector3(_currentTarget.faceLandmarks[0].landmarks[389].x,
          _currentTarget.faceLandmarks[0].landmarks[389].y, _currentTarget.faceLandmarks[0].landmarks[389].z);
        Vector3 rightFace = new Vector3(_currentTarget.faceLandmarks[0].landmarks[169].x,
          _currentTarget.faceLandmarks[0].landmarks[169].y, _currentTarget.faceLandmarks[0].landmarks[169].z);
        Vector3 chin = new Vector3(_currentTarget.faceLandmarks[0].landmarks[152].x,
          _currentTarget.faceLandmarks[0].landmarks[152].y, _currentTarget.faceLandmarks[0].landmarks[152].z);

        var leftVec = leftFace - chin;
        var rightVec = rightFace - chin;

        var normalVec = Vector3.Cross(leftVec, rightVec).normalized;

        _faceRoot.transform.forward = normalVec;
        
        Vector3 leftMiddleFace = new Vector3(_currentTarget.faceLandmarks[0].landmarks[366].x,
          _currentTarget.faceLandmarks[0].landmarks[366].y, _currentTarget.faceLandmarks[0].landmarks[366].z);
        Vector3 rightMiddleFace = new Vector3(_currentTarget.faceLandmarks[0].landmarks[137].x,
          _currentTarget.faceLandmarks[0].landmarks[137].y, _currentTarget.faceLandmarks[0].landmarks[137].z);
        Vector3 nose = new Vector3(_currentTarget.faceLandmarks[0].landmarks[1].x,
          _currentTarget.faceLandmarks[0].landmarks[1].y, _currentTarget.faceLandmarks[0].landmarks[1].z);
        
        var leftMiddleVec = leftMiddleFace - nose;
        var rightMiddleVec = rightMiddleFace - nose;

        var normalMiddleVec = Vector3.Cross(leftMiddleVec, rightMiddleVec).normalized;

        _faceRoot.transform.rotation = Quaternion.Euler(new Vector3(-_faceRoot.transform.rotation.eulerAngles.x + 13.0f, 
          _faceRoot.transform.rotation.eulerAngles.y, GetAngle(Vector3.up, normalMiddleVec) + 90.0f));

        Camera mainCamera = Camera.main;
        _leftEye.transform.LookAt(mainCamera.transform);
        _rightEye.transform.LookAt(mainCamera.transform);
        
        _faceRoot.transform.position = new Vector3(GameObject.Find("FaceLandmarkList Annotation").GetComponentsInChildren<PointAnnotation>()[1].transform.position.x,
          GameObject.Find("FaceLandmarkList Annotation").GetComponentsInChildren<PointAnnotation>()[1].transform.position.y,
          GameObject.Find("FaceLandmarkList Annotation").GetComponentsInChildren<PointAnnotation>()[1].transform.position.z);
        
        Vector3 leftSideFace = new Vector3(_currentTarget.faceLandmarks[0].landmarks[234].x,
          _currentTarget.faceLandmarks[0].landmarks[234].y, _currentTarget.faceLandmarks[0].landmarks[234].z);
        Vector3 rightSideFace = new Vector3(_currentTarget.faceLandmarks[0].landmarks[454].x,
          _currentTarget.faceLandmarks[0].landmarks[454].y, _currentTarget.faceLandmarks[0].landmarks[454].z);

        _faceRoot.transform.localScale = new Vector3(1, 1, 1) * Vector3.Distance(leftSideFace, rightSideFace) * 15.0f;
        //new Vector3(_currentTarget.faceLandmarks[0].landmarks[1].x,
        //_currentTarget.faceLandmarks[0].landmarks[1].y,35);

      }
      else
      {
        //faceAnnotaionInput
      }
    }
    public static float GetAngle (Vector3 vStart, Vector3 vEnd)
    {
      Vector3 v = vEnd - vStart;
 
      return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }
    public void DrawLater(FaceLandmarkerResult target) => DrawNow(target);
    
    protected void UpdateCurrentTarget<TValue>(TValue newTarget, ref TValue currentTarget)
    {

    }
  }
}
