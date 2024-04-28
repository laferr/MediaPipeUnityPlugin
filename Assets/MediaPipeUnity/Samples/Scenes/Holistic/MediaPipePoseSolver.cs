using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity.CoordinateSystem;
using Mediapipe.Unity.Sample.Holistic;
using Google.Protobuf;
using Mediapipe;
using Mediapipe.Unity;

public class MediaPipePoseSolver : MonoBehaviour
{
  public GameObject avatar;
  public struct Pose
  {
    public Arm LeftArm;
    public Arm RightArm;
    public Leg LeftLeg;
    public Leg RightLeg;
    public Vector3 Spine;
    public Hips Hips;
  }

  public struct Arm
  {
    public Vector3 Upper;
    public Vector3 Lower;
    public Vector3 Hand;
  }

  public struct Leg
  {
    public Vector3 Upper;
    public Vector3 Lower;
  }

  public struct Hips
  {
    public Vector3 WorldPosition;
    public Vector3 Position;
    public Vector3 Rotation;
  }

  public NormalizedLandmarkList normalizedLandmarkList;

  public GameObject leftHand;
  public GameObject rightHand;

  private void Update()
  {
    if (normalizedLandmarkList != null)
    {
      //Debug.Log(Solve(normalizedLandmarkList));
      Pose pose = Solve(normalizedLandmarkList);
      SkinnedMeshRenderer avatarMesh = avatar.GetComponentInChildren<SkinnedMeshRenderer>();
      //avatar.transform.position = new Vector3(pose.Spine.x, pose.Spine.y, avatar.transform.position.z);
      //for(int i = 0; i < avatarMesh.bones.Length; i++)
      //{
      //  Debug.Log(avatarMesh.bones[i].name);
      //}
      //Debug.Log(pose.LeftArm.Upper);
      
      //avatarMesh.bones[0].position = pose.Spine;
      avatarMesh.bones[1].position = pose.Hips.Position;
      //avatarMesh.bones[1].localRotation = Quaternion.Euler(pose.Hips.Rotation);
      //avatarMesh.bones[6].localRotation = Quaternion.Euler(pose.LeftLeg.Lower);
      //avatarMesh.bones[7].localRotation = Quaternion.Euler(pose.LeftLeg.Upper);
      //avatarMesh.bones[32].localRotation = Quaternion.Euler(pose.RightLeg.Upper);
      //avatarMesh.bones[7].localRotation = Quaternion.Euler(pose.RightLeg.Upper);
      leftHand.transform.position = ToVector(normalizedLandmarkList.Landmark[15]);
      //avatarMesh.bones[10].localRotation = Quaternion.Euler(pose.LeftArm.Lower);
      //avatarMesh.bones[9].localRotation = Quaternion.Euler(pose.LeftArm.Upper);
      rightHand.transform.position = ToVector(normalizedLandmarkList.Landmark[16]);
      //avatarMesh.bones[34].localRotation = Quaternion.Euler(pose.RightArm.Lower);
      //avatarMesh.bones[33].localRotation = Quaternion.Euler(pose.RightArm.Upper);
    }
  }

  public Pose Solve(NormalizedLandmarkList normalizedLandmarkList)
  {
    (Arm leftArm, Arm rightArm) = CalculateArms(normalizedLandmarkList);
    (Hips hips, Vector3 spine) = CalculateHips(normalizedLandmarkList);

    var pose = new Pose
    {
      LeftArm = leftArm,
      RightArm = rightArm,
      Hips = hips,
      Spine = spine
    };

    return pose;
  }

  public Vector3 ToVector(NormalizedLandmark landmark) => new(landmark.X, landmark.Y, landmark.Z);
  public Vector2 ToVector2(Vector3 vector) => new(vector.x, vector.y);

  private (Arm, Arm) CalculateArms(NormalizedLandmarkList normalizedLandmarkList)
  {
    var landmarks = normalizedLandmarkList.Landmark;

    var rightArm = new Arm();
    var leftArm = new Arm();

    rightArm.Upper = VectorExtensions.FindRotation(ToVector(landmarks[11]), ToVector(landmarks[13]), true);
    leftArm.Upper = VectorExtensions.FindRotation(ToVector(landmarks[12]), ToVector(landmarks[14]), true);
    rightArm.Upper.y = VectorExtensions.AngleBetween3DCoords(ToVector(landmarks[12]), ToVector(landmarks[11]), ToVector(landmarks[13]));
    leftArm.Upper.y = VectorExtensions.AngleBetween3DCoords(ToVector(landmarks[11]), ToVector(landmarks[12]), ToVector(landmarks[14]));

    rightArm.Lower = VectorExtensions.FindRotation(ToVector(landmarks[13]), ToVector(landmarks[15]), true);
    leftArm.Lower = VectorExtensions.FindRotation(ToVector(landmarks[14]), ToVector(landmarks[16]), true);
    rightArm.Lower.y = VectorExtensions.AngleBetween3DCoords(ToVector(landmarks[11]), ToVector(landmarks[13]), ToVector(landmarks[15]));
    leftArm.Lower.y = VectorExtensions.AngleBetween3DCoords(ToVector(landmarks[12]), ToVector(landmarks[14]), ToVector(landmarks[16]));
    rightArm.Lower.z = Math.Clamp(rightArm.Lower.z, -2.14f, 0f);
    leftArm.Lower.z = Math.Clamp(leftArm.Lower.z, -2.14f, 0f);

    rightArm.Hand = VectorExtensions.FindRotation(ToVector(landmarks[15]), Vector3.Lerp(ToVector(landmarks[17]), ToVector(landmarks[19]), .5f), true);
    leftArm.Hand = VectorExtensions.FindRotation(ToVector(landmarks[16]), Vector3.Lerp(ToVector(landmarks[18]), ToVector(landmarks[20]), .5f), true);

    // Modify rotations slightly for more natural movement
    RigArm(ref rightArm, "right");
    RigArm(ref leftArm, "left");

    return (leftArm, rightArm);
  }

  private void RigArm(ref Arm arm, string side)
  {
    float invert = side == "right" ? 1f : -1f;

    arm.Upper.z *= -2.3f * invert;

    arm.Upper.y *= MathF.PI * invert;
    arm.Upper.y -= Math.Max(arm.Lower.x, 0);
    arm.Upper.y -= -invert * Math.Max(arm.Lower.z, 0);
    arm.Upper.x -= 0.3f * invert;

    arm.Lower.z *= -2.14f * invert;
    arm.Lower.y *= 2.14f * invert;
    arm.Lower.x *= 2.14f * invert;

    // Clamp values to realistic humanoid limits
    arm.Upper.x = Math.Clamp(arm.Upper.x, -0.5f, MathF.PI);
    arm.Lower.x = Math.Clamp(arm.Lower.x, -0.3f, 0.3f);

    arm.Hand.y = Math.Clamp(arm.Hand.z * 2, -0.6f, 0.6f); // sides
    arm.Hand.z = arm.Hand.z * -2.3f * invert; // up and down
  }

  private (Hips, Vector3) CalculateHips(NormalizedLandmarkList list)
  {
    var landmarks = list.Landmark;

    // Find 2D normalized hip and shoulder joint positions / distances
    Vector2 hipLeft2d = ToVector2(ToVector(landmarks[23]));
    Vector2 hipRight2d = ToVector2(ToVector(landmarks[24]));
    Vector2 shoulderLeft2d = ToVector2(ToVector(landmarks[11]));
    Vector2 shoulderRight2d = ToVector2(ToVector(landmarks[12]));

    Vector2 hipCenter2d = Vector2.Lerp(hipLeft2d, hipRight2d, 1);
    Vector2 shoulderCenter2d = Vector2.Lerp(shoulderLeft2d, shoulderRight2d, 1);
    float spineLength = Vector2.Distance(hipCenter2d, shoulderCenter2d);

    var hips = new Hips
    {
      Position = new Vector3(Math.Clamp(-1 * (hipCenter2d.x - .65f), -1, 1), 0, Math.Clamp(spineLength - 1, -2, 0)),
      Rotation = VectorExtensions.RollPitchYaw(ToVector(landmarks[23]), ToVector(landmarks[24])),
    };

    if (hips.Rotation.y > .5f)
      hips.Rotation.y -= 2;

    hips.Rotation.y += .5f;

    //Stop jumping between left and right shoulder tilt
    if (hips.Rotation.z > 0)
      hips.Rotation.z = 1 - hips.Rotation.z;

    if (hips.Rotation.z < 0)
      hips.Rotation.z = -1 - hips.Rotation.z;

    float turnAroundAmountHips = Math.Abs(hips.Rotation.y).Remap(.2f, .4f);
    hips.Rotation.z *= 1 - turnAroundAmountHips;
    hips.Rotation.x = 0; // Temp fix for inaccurate X axis

    Vector3 spine = VectorExtensions.RollPitchYaw(ToVector(landmarks[11]), ToVector(landmarks[12]));

    if (spine.y > .5f)
      spine.y -= 2;

    spine.y += .5f;

    // Prevent jumping between left and right shoulder tilt
    if (spine.z > 0)
      spine.z = 1 - spine.z;

    if (spine.z < 0)
      spine.z = -1 - spine.z;

    // Fix weird large numbers when 2 shoulder points get too close
    float turnAroundAmount = Math.Abs(spine.y).Remap(.2f, .4f);
    spine.z *= 1 - turnAroundAmount;
    spine.x = 0; // Temp fix for inaccurate X axis

    RigHips(ref hips, ref spine);
    return (hips, spine);
  }

  private void RigHips(ref Hips hips, ref Vector3 spine)
  {
    // Convert normalized values to radians
    hips.Rotation *= MathF.PI;

    hips.WorldPosition = new Vector3
    (
        hips.Position.x * (.5f + 1.8f * -hips.Position.z),
        0,
        hips.Position.z * (.1f + hips.Position.z * -2)
    );

    spine *= MathF.PI;
  }

  public static Quaternion ToQ(Vector3 v)
  {
    return ToQ(v.y, v.x, v.z);
  }

  public static Quaternion ToQ(float yaw, float pitch, float roll)
  {
    yaw *= Mathf.Deg2Rad;
    pitch *= Mathf.Deg2Rad;
    roll *= Mathf.Deg2Rad;
    float rollOver2 = roll * 0.5f;
    float sinRollOver2 = (float)Math.Sin((double)rollOver2);
    float cosRollOver2 = (float)Math.Cos((double)rollOver2);
    float pitchOver2 = pitch * 0.5f;
    float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
    float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
    float yawOver2 = yaw * 0.5f;
    float sinYawOver2 = (float)Math.Sin((double)yawOver2);
    float cosYawOver2 = (float)Math.Cos((double)yawOver2);
    Quaternion result;
    result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
    result.x = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
    result.y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
    result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

    return result;
  }

  public static Vector3 FromQ2(Quaternion q1)
  {
    float sqw = q1.w * q1.w;
    float sqx = q1.x * q1.x;
    float sqy = q1.y * q1.y;
    float sqz = q1.z * q1.z;
    float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
    float test = q1.x * q1.w - q1.y * q1.z;
    Vector3 v;

    if (test > 0.4995f * unit)
    { // singularity at north pole
      v.y = 2f * Mathf.Atan2(q1.y, q1.x);
      v.x = Mathf.PI / 2;
      v.z = 0;
      return NormalizeAngles(v * Mathf.Rad2Deg);
    }
    if (test < -0.4995f * unit)
    { // singularity at south pole
      v.y = -2f * Mathf.Atan2(q1.y, q1.x);
      v.x = -Mathf.PI / 2;
      v.z = 0;
      return NormalizeAngles(v * Mathf.Rad2Deg);
    }
    Quaternion q = new Quaternion(q1.w, q1.z, q1.x, q1.y);
    v.y = (float)Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
    v.x = (float)Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
    v.z = (float)Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll
    return NormalizeAngles(v * Mathf.Rad2Deg);
  }

  static Vector3 NormalizeAngles(Vector3 angles)
  {
    angles.x = NormalizeAngle(angles.x);
    angles.y = NormalizeAngle(angles.y);
    angles.z = NormalizeAngle(angles.z);
    return angles;
  }

  static float NormalizeAngle(float angle)
  {
    while (angle > 360)
      angle -= 360;
    while (angle < 0)
      angle += 360;
    return angle;
  }


}
