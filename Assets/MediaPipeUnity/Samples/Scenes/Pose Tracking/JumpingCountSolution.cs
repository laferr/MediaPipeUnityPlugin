using System;
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
  public class JumpingCountSolution : MonoBehaviour
  {
    private Animator animator;
    private Transform virtualHipLm;
    private Transform virtualNeckLm;
    private Vector3 leftHipLm;
    private Vector3 rightHipLm;
    private Transform leftShoulderLm;
    private Transform rightShoulderLm;

    [HideInInspector]
    public Vector3 noseLm;

    private Transform leftElbowLm;
    private Transform rightElbowLm;
    private Vector3 leftWristLm;
    private Vector3 rightWristLm;
    private Transform leftKneeLm;
    private Transform rightKneeLm;
    private Transform leftAnkleLm;
    private Transform rightAnkleLm;
    private Transform leftFootLm;
    private Transform rightFootLm;

    public GameObject character;
    public GameObject char_Larm;
    public GameObject char_Rarm;

    public LayerMask ground;
    public float footGroundOffset = .1f;

    /*public GameObject leftHip;
    public GameObject rightHip;
    public GameObject leftKnee;
    public GameObject rightKnee;
    public GameObject leftAnkle;
    public GameObject rightAnkle;*/

    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private Quaternion targetRot;

    private Dictionary<HumanBodyBones, CalibrationData> parentCalibrationData =
      new Dictionary<HumanBodyBones, CalibrationData>();

    private NormalizedLandmarkList _currentTarget;

    private bool isTracking = false;
    public bool footTracking = false;
    public bool isCalibrated = false;
    public bool is3WayGame = false;

    int jumpCount = 0;
    bool isCounted = false;

    public float jumpThreshold = 50.0f;
    public float LRThreshold = 15.0f;

    public GameObject thresholdSettingPanel;

    public Text jumpCountText;
    public int CLR = 0;

    bool lArmThrow = false;
    bool rArmThrow = false;
    public GameObject bulletPrefab;
    public GameObject bulletTriplePrefab;
    public GameObject bulletSpiralPrefab;

    public Transform firePoint;
    Vector3 lastPos = Vector3.zero;

    public BulletType bulletType = BulletType.normal;

    public enum BulletType
    {
      normal,
      triple,
      big,
      doubleTap,
      spiral
    }

    private void Start()
    {
      if (PlayerPrefs.HasKey("JumpThreshold") && !is3WayGame)
      {
        jumpThreshold = PlayerPrefs.GetFloat("JumpThreshold");
        thresholdOptionText.text = jumpThreshold.ToString();
      }
      if (PlayerPrefs.HasKey("LRThreshold") && is3WayGame)
      {
        LRThreshold = PlayerPrefs.GetFloat("LRThreshold");
        thresholdOptionText.text = LRThreshold.ToString();
      }
      initialRotation = transform.rotation;
      initialPosition = transform.position;

      virtualNeckLm = new GameObject("VirtualNeck").transform;
      virtualHipLm = new GameObject("VirtualHip").transform;

      StartCoroutine(CaliDelayer());
    }
    IEnumerator CaliDelayer()
    {
      yield return new WaitForSeconds(3.0f);
      isCalibrated = true;
    }

    public void DrawNow(NormalizedLandmarkList target)
    {
      _currentTarget = target;

      isTracking = true;
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
      leftHipLm = LandmarkSetter(24);
      rightHipLm = LandmarkSetter(23);

      leftWristLm = LandmarkSetter(16);
      rightWristLm = LandmarkSetter(15);
      virtualHipLm.position = (leftHipLm + rightHipLm) / 2.0f;

      noseLm = LandmarkSetter(0);
      //Debug.Log(noseLm);
      //animator.enabled = false; // disable animator to stop interference.
    }


    private Vector3 LandmarkSetter(int index) =>
      new(_currentTarget.Landmark[index].X * 100.0f,
        _currentTarget.Landmark[index].Y * 100.0f, _currentTarget.Landmark[index].Z * 100.0f);

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.C))
      {
        isCalibrated = true;
      }
      if (Input.GetKeyDown(KeyCode.Tab))
      {
        thresholdSettingPanel.SetActive(true);
      }
      if (Input.GetKeyDown(KeyCode.Alpha1))
      {
        //SceneManager.LoadScene(0);
        bulletType = BulletType.normal;
      }
      if (Input.GetKeyDown(KeyCode.Alpha2))
      {
        //SceneManager.LoadScene(1);
        bulletType = BulletType.triple;
      }
      if (Input.GetKeyDown(KeyCode.Alpha3))
      {
        bulletType = BulletType.spiral;
      }
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        Application.Quit();
      }
      if (isTracking)
      {
        if (isCalibrated)
        {
          Calibrate();
          //isCalibrated = false;
          if (is3WayGame)
          {
            //Debug.Log(noseLm);
            //if(noseLm.x > 50 + LRThreshold)
            //{
            //  //left
            //  CLR = 1;
            //}
            //else if (noseLm.x < 50 - LRThreshold)
            //{
            //  //right
            //  CLR = 2;
            //}
            //else
            //{
            //  //center
            //  CLR = 0;
            //}

            character.transform.position = Vector3.Lerp(lastPos, new Vector3(-(noseLm.x - 50) * 0.1f, 1, -6.32f), 0.75f);
            lastPos = character.transform.position;
            //test
            if (leftWristLm.y <= noseLm.y && !lArmThrow)
            {
              if (bulletType == BulletType.normal)
              {
                _ = char_Larm.transform.DOLocalRotate(new Vector3(360, 0, 0), 0.25f, RotateMode.FastBeyond360);
                //throw bullet
                var gg = Instantiate(bulletPrefab);
                gg.transform.position = char_Larm.transform.position;
                lArmThrow = true;
              }
              else if (bulletType == BulletType.triple)
              {
                _ = char_Larm.transform.DOLocalRotate(new Vector3(360, 0, 0), 0.25f, RotateMode.FastBeyond360);
                //throw bullet
                var gg = Instantiate(bulletTriplePrefab);
                var gg1 = Instantiate(bulletTriplePrefab);
                var gg2 = Instantiate(bulletTriplePrefab);
                gg.transform.position = char_Larm.transform.position;
                gg1.transform.position = char_Larm.transform.position;
                gg2.transform.position = char_Larm.transform.position;
                gg1.transform.forward = new Vector3(0.15f, 0, 1);
                gg2.transform.forward = new Vector3(-0.15f, 0, 1);
                lArmThrow = true;
              }
              else if (bulletType == BulletType.doubleTap)
              {

              }
              else if (bulletType == BulletType.spiral)
              {
                _ = char_Larm.transform.DOLocalRotate(new Vector3(360, 0, 0), 0.25f, RotateMode.FastBeyond360);
                //throw bullet
                var gg = Instantiate(bulletSpiralPrefab);
                gg.transform.position = char_Larm.transform.position + new Vector3(-1f, 0, 0);
                gg.transform.DOMoveX(1f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetRelative(true);
                lArmThrow = true;
              }
              else if (bulletType == BulletType.big)
              {

              }

            }
            else if (leftWristLm.y > noseLm.y)
            {
              lArmThrow = false;
            }
            if (rightWristLm.y <= noseLm.y && !rArmThrow)
            {
              if (bulletType == BulletType.normal)
              {
                _ = char_Rarm.transform.DOLocalRotate(new Vector3(360, 0, 0), 0.25f, RotateMode.FastBeyond360);
                //throw bullet
                var gg = Instantiate(bulletPrefab);
                gg.transform.position = char_Rarm.transform.position;
                rArmThrow = true;
              }
              else if (bulletType == BulletType.triple)
              {
                _ = char_Rarm.transform.DOLocalRotate(new Vector3(360, 0, 0), 0.25f, RotateMode.FastBeyond360);
                //throw bullet
                var gg = Instantiate(bulletTriplePrefab);
                var gg1 = Instantiate(bulletTriplePrefab);
                var gg2 = Instantiate(bulletTriplePrefab);
                gg.transform.position = char_Rarm.transform.position;
                gg1.transform.position = char_Rarm.transform.position;
                gg2.transform.position = char_Rarm.transform.position;
                gg1.transform.forward = new Vector3(0.15f, 0, 1);
                gg2.transform.forward = new Vector3(-0.15f, 0, 1);
                rArmThrow = true;
              }
              else if (bulletType == BulletType.doubleTap)
              {

              }
              else if (bulletType == BulletType.spiral)
              {
                _ = char_Rarm.transform.DOLocalRotate(new Vector3(360, 0, 0), 0.25f, RotateMode.FastBeyond360);
                //throw bullet
                var gg = Instantiate(bulletSpiralPrefab);
                gg.transform.position = char_Rarm.transform.position + new Vector3(1f, 0, 0);
                gg.transform.DOMoveX(-1f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetRelative(true);
                rArmThrow = true;
              }
              else if (bulletType == BulletType.big)
              {

              }
            }
            else if (rightWristLm.y > noseLm.y)
            {
              rArmThrow = false;
            }
          }
          else
          {
            if (virtualHipLm.position.y < jumpThreshold && !isCounted)
            {
              jumpCount++;
              //Debug.Log(jumpCount);
              jumpCountText.text = "Jump : " + jumpCount.ToString("D2");
              isCounted = true;
              character.GetComponent<Rigidbody>().AddForce(new Vector3(0, 250.0f));
            }
            else if (virtualHipLm.position.y >= jumpThreshold && isCounted)
            {
              isCounted = false;
              character.GetComponent<Rigidbody>().AddForce(new Vector3(0, -200.0f));
            }
          }

        }
        //Debug.Log(_currentTarget);
        // Adjust the vertical position of the avatar to keep it approximately grounded.
        if (parentCalibrationData.Count > 0)
        {

        }

      }
    }
    public void DrawLater(NormalizedLandmarkList target) => DrawNow(target);

    public Text thresholdOptionText;
    public Slider thresholdSlider;

    public GameObject jumpThresholdVisualizer;
    public void OnThresholdSliderChange()
    {
      if (is3WayGame)
      {
        thresholdOptionText.text = thresholdSlider.value.ToString();
        LRThreshold = thresholdSlider.value;
      }
      else
      {
        thresholdOptionText.text = thresholdSlider.value.ToString();
        jumpThreshold = thresholdSlider.value;
        jumpThresholdVisualizer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(jumpThreshold - 50) / 50 * 562.5f);
      }
    }

    public void OnThresholdChangeEnd()
    {
      if (is3WayGame)
      {
        PlayerPrefs.SetFloat("LRThreshold", LRThreshold);
        PlayerPrefs.Save();
      }
      else
      {
        PlayerPrefs.SetFloat("JumpThreshold", jumpThreshold);
        PlayerPrefs.Save();
      }
    }
  }
}

