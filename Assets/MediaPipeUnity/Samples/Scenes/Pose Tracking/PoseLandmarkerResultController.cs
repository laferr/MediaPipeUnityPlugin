using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Reflection;
using UnityEngine;
using Mediapipe.Tasks.Vision.PoseLandmarker;

namespace Mediapipe.Unity.Sample.PoseTracking
{
  public class PoseLandmarkerResultController : MonoBehaviour
  {
    public GameObject character;
    private Animator animator;
    private Transform virtualHipLm;
    private Transform virtualNeckLm;
    private Transform leftHipLm;
    private Transform rightHipLm;
    private Transform leftShoulderLm;
    private Transform rightShoulderLm;
    private Transform noseLm;
    private Transform leftElbowLm;
    private Transform rightElbowLm;
    private Transform leftWristLm;
    private Transform rightWristLm;
    private Transform leftKneeLm;
    private Transform rightKneeLm;
    private Transform leftAnkleLm;
    private Transform rightAnkleLm;
    private Transform leftFootLm;
    private Transform rightFootLm;

    public Camera cam;

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

    private CalibrationData spineUpDown, hipsTwist, chest, head;

    private bool isTracking = false;
    public bool footTracking = false;
    private bool isCalibrated = false;

    private void Start()
    {
      animator = character.GetComponent<Animator>();
      initialRotation = transform.rotation;
      initialPosition = transform.position;
      
      virtualNeckLm = new GameObject("VirtualNeck").transform;
      virtualHipLm = new GameObject("VirtualHip").transform;
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
        annotations[i] = GameObject.Find("PoseWorldLandmarks Annotation").GetComponentsInChildren<PointAnnotation>()[i].transform;
      }
      Debug.Log(annotations[0]);
      leftHipLm = annotations[24];//LandmarkSetter(24);
      rightHipLm = annotations[23];//LandmarkSetter(23);
      virtualHipLm.position = (leftHipLm.position + rightHipLm.position) / 2.0f;
      
      leftShoulderLm =  annotations[12];//LandmarkSetter(12);
      rightShoulderLm =  annotations[11];//LandmarkSetter(11);
      virtualNeckLm.position = (leftShoulderLm.position + rightShoulderLm.position) / 2.0f;

      noseLm =  annotations[0];//LandmarkSetter(0);
      
      leftElbowLm =  annotations[14];//LandmarkSetter(14);
      rightElbowLm =  annotations[13];//LandmarkSetter(13);
      leftWristLm =  annotations[16];//LandmarkSetter(16);
      rightWristLm =  annotations[15];//LandmarkSetter(15);
      leftKneeLm =  annotations[26];//LandmarkSetter(26);
      rightKneeLm =  annotations[25];//LandmarkSetter(25);
      leftAnkleLm =  annotations[28];//LandmarkSetter(28);
      rightAnkleLm =  annotations[27];//LandmarkSetter(27);
      leftFootLm =  annotations[32];//LandmarkSetter(32);
      rightFootLm =  annotations[31];//LandmarkSetter(31);
      
      spineUpDown = new CalibrationData(animator.GetBoneTransform(HumanBodyBones.Spine),
        animator.GetBoneTransform(HumanBodyBones.Neck),
        virtualHipLm, virtualNeckLm);
      hipsTwist = new CalibrationData(animator.GetBoneTransform(HumanBodyBones.Hips),
        animator.GetBoneTransform(HumanBodyBones.Hips),
        rightHipLm, leftHipLm);
      chest = new CalibrationData(animator.GetBoneTransform(HumanBodyBones.Chest),
        animator.GetBoneTransform(HumanBodyBones.Chest),
        rightShoulderLm, leftShoulderLm);
      head = new CalibrationData(animator.GetBoneTransform(HumanBodyBones.Neck),
        animator.GetBoneTransform(HumanBodyBones.Head),
        virtualNeckLm, noseLm);
      
      AddCalibration(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm,
        rightShoulderLm, rightElbowLm);
      AddCalibration(HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand,
        rightElbowLm, rightWristLm);

      AddCalibration(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg,
        rightHipLm, rightKneeLm);
      AddCalibration(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot,
        rightKneeLm, rightAnkleLm);

      AddCalibration(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm,
        leftShoulderLm, leftElbowLm);
      AddCalibration(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand,
        leftElbowLm, leftWristLm);

      AddCalibration(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg,
        leftHipLm, leftKneeLm);
      AddCalibration(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot,
        leftKneeLm, leftAnkleLm);

      if (footTracking)
      {
        AddCalibration(HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes,
          leftAnkleLm, leftFootLm);
        AddCalibration(HumanBodyBones.RightFoot, HumanBodyBones.RightToes,
          rightAnkleLm, rightFootLm);
      }

      //animator.enabled = false; // disable animator to stop interference.
    }


    //private Vector3 LandmarkSetter(int index) =>
    //  new(_currentTarget.poseLandmarks[0].landmarks[index].x*100.0f,
    //    _currentTarget.poseLandmarks[0].landmarks[index].y*100.0f, _currentTarget.poseLandmarks[0].landmarks[index].z*100.0f);

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.C))
      {
        isCalibrated = true;
      }
      if (isTracking)
      {
        if (isCalibrated)
        {
          Calibrate();
          isCalibrated = false;
        }
        //Debug.Log(_currentTarget);
        // Adjust the vertical position of the avatar to keep it approximately grounded.
        if (parentCalibrationData.Count > 0)
        {
          float displacement = 0;
          RaycastHit h1;
          if (Physics.Raycast(animator.GetBoneTransform(HumanBodyBones.LeftFoot).position, Vector3.down, out h1, 100f,
                ground, QueryTriggerInteraction.Ignore))
          {
            displacement = (h1.point - animator.GetBoneTransform(HumanBodyBones.LeftFoot).position).y;
          }

          if (Physics.Raycast(animator.GetBoneTransform(HumanBodyBones.RightFoot).position, Vector3.down, out h1, 100f,
                ground, QueryTriggerInteraction.Ignore))
          {
            float displacement2 = (h1.point - animator.GetBoneTransform(HumanBodyBones.RightFoot).position).y;
            if (Mathf.Abs(displacement2) < Mathf.Abs(displacement))
            {
              displacement = displacement2;
            }
          }

          transform.position = Vector3.Lerp(transform.position,
            initialPosition + Vector3.up * displacement + Vector3.up * footGroundOffset,
            Time.deltaTime * 5f);
        }

        // Compute the new rotations for each limbs of the avatar using the calibration datas we created before.
        foreach (var i in parentCalibrationData)
        {
          Quaternion deltaRotTracked = Quaternion.FromToRotation(i.Value.initialDir, i.Value.CurrentDirection);
          i.Value.parent.rotation = deltaRotTracked * i.Value.initialRotation;
        }

        // Deal with spine chain as a special case.
        if (parentCalibrationData.Count > 0)
        {
          Vector3 hd = head.CurrentDirection;
          // Some are partial rotations which we can stack together to specify how much we should rotate.
          Quaternion headr = Quaternion.FromToRotation(head.initialDir, hd);
          Quaternion twist = Quaternion.FromToRotation(hipsTwist.initialDir,
            Vector3.Slerp(hipsTwist.initialDir, hipsTwist.CurrentDirection, .25f));
          Quaternion updown = Quaternion.FromToRotation(spineUpDown.initialDir,
            Vector3.Slerp(spineUpDown.initialDir, spineUpDown.CurrentDirection, .25f));
          // Compute the final rotations.
          Quaternion h = updown * updown * updown * twist * twist;
          Quaternion s = h * twist * updown;
          Quaternion c = s * twist * twist;
          float speed = 10f;
          hipsTwist.Tick(h * hipsTwist.initialRotation, speed);
          spineUpDown.Tick(s * spineUpDown.initialRotation, speed);
          chest.Tick(c * chest.initialRotation, speed);
          head.Tick(updown * twist * headr * head.initialRotation, speed);

          // For additional responsiveness, we rotate the entire transform slightly based on the hips.
          Vector3 d = Vector3.Slerp(hipsTwist.initialDir, hipsTwist.CurrentDirection, .25f);
          d.y *= 0.5f;
          Quaternion deltaRotTracked = Quaternion.FromToRotation(hipsTwist.initialDir, d);
          targetRot = deltaRotTracked * initialRotation;
          transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * speed);

          // The tracking of the camera.
          if (cam)
          {
            //Quaternion q = Quaternion.LookRotation(
            //  (animator.GetBoneTransform(HumanBodyBones.Chest).transform.position - cam.transform.position).normalized,
            //  Vector3.up);
            //cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, q, Time.deltaTime * 3f);
          }
        }
      }
    }

    private void AddCalibration(HumanBodyBones parent, HumanBodyBones child, Transform trackParent,
      Transform trackChild) =>
      parentCalibrationData.Add(parent,
        new CalibrationData(animator.GetBoneTransform(parent), animator.GetBoneTransform(child),
          trackParent, trackChild));

    public void DrawLater(NormalizedLandmarkList target) => DrawNow(target);
  }

  class CalibrationData
  {
    public Transform parent, child;
    public Transform tparent, tchild;
    public Vector3 initialDir;
    public Quaternion initialRotation;

    public Quaternion targetRotation;

    public void Tick(Quaternion newTarget, float speed)
    {
      parent.rotation = newTarget;
      parent.rotation = Quaternion.Lerp(parent.rotation, targetRotation, Time.deltaTime * speed);
    }

    public Vector3 CurrentDirection => (tchild.position - tparent.position).normalized;

    public CalibrationData(Transform fparent, Transform fchild, Transform tparent, Transform tchild)
    {
      initialDir = (tchild.position - tparent.position).normalized;
      initialRotation = fparent.rotation;
      parent = fparent;
      child = fchild;
      this.tparent = tparent;
      this.tchild = tchild;
    }
  }
}
