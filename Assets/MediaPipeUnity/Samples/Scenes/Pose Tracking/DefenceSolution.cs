using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using DG.Tweening;
namespace Mediapipe.Unity.Sample.PoseTracking
{
  public class DefenceSolution : MonoBehaviour
  {
    [HideInInspector]
    public Vector3 noseLm;

    private Vector3 leftWristLm;
    private Vector3 rightWristLm;

    private Dictionary<HumanBodyBones, CalibrationData> parentCalibrationData =
      new Dictionary<HumanBodyBones, CalibrationData>();

    public GameObject lRing;
    public GameObject rRing;
    Vector3 lLastPos = Vector3.zero;
    Vector3 rLastPos = Vector3.zero;

    private NormalizedLandmarkList _currentTarget;

    private bool isTracking = false;
    public bool isCalibrated = false;
    // Start is called before the first frame update
    void Start()
    {
      StartCoroutine(CaliDelayer());
    }
    IEnumerator CaliDelayer()
    {
      yield return new WaitForSeconds(3.0f);
      isCalibrated = true;
    }
    public void Calibrate()
    {
      parentCalibrationData.Clear();
      Transform[] annotations = new Transform[33];
      for (int i = 0; i < annotations.Length; i++)
      {
        annotations[i] = GameObject.Find("Point List Annotation").GetComponentsInChildren<PointAnnotation>()[i].transform;
      }

      //Debug.Log(annotations[0]);
      leftWristLm = LandmarkSetter(16);
      rightWristLm = LandmarkSetter(15);
      //Debug.Log(noseLm);
      //animator.enabled = false; // disable animator to stop interference.
    }
    public void DrawNow(NormalizedLandmarkList target)
    {
      _currentTarget = target;

      isTracking = true;
    }
    private Vector3 LandmarkSetter(int index) =>
      new(_currentTarget.Landmark[index].X * 100.0f,
        _currentTarget.Landmark[index].Y * 100.0f, _currentTarget.Landmark[index].Z * 100.0f);

    bool isLeftGen = false;
    float timer = 0.0f;

    public GameObject leftBall;
    public GameObject rightBall;
    // Update is called once per frame
    void Update()
    {
      if (isTracking)
      {
        if (isCalibrated)
        {
          Calibrate();
          timer += Time.deltaTime;
          lRing.transform.position = Vector3.Lerp(lLastPos, new Vector3(-(leftWristLm.x - 50) * 0.11f, -(leftWristLm.y - 50) * 0.06f, lRing.transform.position.z), 0.7f);//leftWristLm;
          rRing.transform.position = Vector3.Lerp(rLastPos, new Vector3(-(rightWristLm.x - 50) * 0.11f, -(rightWristLm.y - 50) * 0.06f, rRing.transform.position.z), 0.7f);//rightWristLm;
          lLastPos = lRing.transform.position;
          rLastPos = rRing.transform.position;
          if(timer > 1.5f)
          {
            if (isLeftGen)
            {
              Instantiate(leftBall);
            }
            else
            {
              Instantiate(rightBall);
            }
            timer = 0.0f;
            isLeftGen = !isLeftGen;
          }

        }
      }
    }
    public void DrawLater(NormalizedLandmarkList target) => DrawNow(target);
  }
}
