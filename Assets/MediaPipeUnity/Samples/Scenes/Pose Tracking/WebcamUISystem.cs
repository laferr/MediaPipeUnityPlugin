using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity.CoordinateSystem;
using UnityEngine.EventSystems;
using System.Linq;

namespace Mediapipe.Unity.Sample.PoseTracking
{
  public class WebcamUISystem : MonoBehaviour
  {
    private NormalizedLandmarkList _currentTarget;
    private bool isTracking = false;
    private Dictionary<HumanBodyBones, CalibrationData> parentCalibrationData =
  new Dictionary<HumanBodyBones, CalibrationData>();
    private Vector3 leftWristLm;
    private Vector3 rightWristLm;
    private bool isCalibrated = false;

    public float buttonHoldTime = 2.0f;

    public GameObject annotationLayer;
    public GameObject debugObj;

    public void DrawNow(NormalizedLandmarkList target)
    {
      _currentTarget = target;
      isTracking = true;
    }

    public UnityEngine.Rect GetScreenRect()
    {
      return annotationLayer.GetComponent<RectTransform>().rect;
    }

    public void DrawLater(NormalizedLandmarkList target) => DrawNow(target);
    private Vector3 LandmarkSetter(int index) =>
  new(_currentTarget.Landmark[index].X * 100.0f,
    _currentTarget.Landmark[index].Y * 100.0f, -150.0f);
    public void Calibrate()
    {
      parentCalibrationData.Clear();
      Transform[] annotations = new Transform[33];
      for (int i = 0; i < annotations.Length; i++)
      {
        annotations[i] = GameObject.Find("Point List Annotation").GetComponentsInChildren<PointAnnotation>()[i].transform;
      }

      leftWristLm = annotationLayer.GetComponentsInChildren<Transform>()[21].position;//LandmarkSetter(20);
      rightWristLm = annotationLayer.GetComponentsInChildren<Transform>()[20].position;

      //debugObj.transform.position = leftWristLm;


      if(!RisInteracting) PerformRaycastL(leftWristLm);
      if(!LisInteracting) PerformRaycastR(rightWristLm);

      //Debug.DrawRay(Camera.main.transform.position, leftWristLm, UnityEngine.Color.red);
    }
    // Start is called before the first frame update
    void Start()
    {
      //isCalibrated = true;
      StartCoroutine(CaliDelayer());
    }
    IEnumerator CaliDelayer()
    {
      yield return new WaitForSeconds(3.0f);
      isCalibrated = true;
    }
    // Update is called once per frame
    void Update()
    {
      if (isCalibrated && isTracking)
      {
        Calibrate();
      }
    }
    public RaycastHit _Hit;
    public LayerMask _RaycastCollidableLayers; //Set this in inspector, makes you able to say which layers should be collided with and which not.
    public float _CheckDistance = 300f;
    GameObject lastEventObj;
    bool LisInteracting = false;
    bool RisInteracting = false;
    //Method
    private void PerformRaycastL(Vector2 pos)
    {
      var eventData = new PointerEventData(EventSystem.current)
      {
        position = pos
      };
      var results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventData, results);

      for (int i = 0; i < results.Count; ++i)
      {
        if (results[0].gameObject.tag == "HoldButton")
        {
          LisInteracting = true;
          results[0].gameObject.GetComponent<WebcamButton>().OnPointerEnter();
          lastEventObj = results[0].gameObject;
        }
        else
        {
          LisInteracting = false;
          if (lastEventObj != null)
            lastEventObj.gameObject.GetComponent<WebcamButton>().OnPointerExit();
          lastEventObj = null;
        }
      }
    }
    private void PerformRaycastR(Vector2 pos)
    {
      var eventData = new PointerEventData(EventSystem.current)
      {
        position = pos
      };
      var results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventData, results);

      for (int i = 0; i < results.Count; ++i)
      {
        if (results[0].gameObject.tag == "HoldButton")
        {
          RisInteracting = true;
          results[0].gameObject.GetComponent<WebcamButton>().OnPointerEnter();
          lastEventObj = results[0].gameObject;
        }
        else
        {
          RisInteracting = false;
          if (lastEventObj != null)
            lastEventObj.gameObject.GetComponent<WebcamButton>().OnPointerExit();
          lastEventObj = null;
        }
      }
    }

  }


}
