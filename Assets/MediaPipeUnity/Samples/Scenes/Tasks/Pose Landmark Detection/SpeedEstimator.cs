using Mediapipe.Tasks.Vision.PoseLandmarker;
using UnityEngine;
using UnityEngine.UI;

public class SpeedEstimator : MonoBehaviour
{
  public float athleteHeight = 170f;
  public int cameraFps = 10;

  private float speed = 0f;
  private float newSpeed = 0f;
  private float prevAng = 0f;
  private int strideFrames = 0;
  private int direction = 1;
  private int times = 0;

  // Define keypoints indices
  private const int Hip = 8;
  private const int LeftKnee = 13;
  private const int RightKnee = 10;
  private const int LeftAnkle = 14;
  private const int RightAnkle = 11;
    private Vector3[] keypoints;

    private float distance = 0.0f;
    private int pixels = 0;
    void Update()
    {
        //Vector3[] keypoints = GetKeyPoints(); // Mediapipe에서 랜드마크 데이터 가져오기
        CalculateSpeed(keypoints);
        // 다른 로직 (예: UI 업데이트, 시각화 등)
    }

    void GetPixelPerMeter(Vector3[] keypoints)
    {
      Vector3 coordEyes = (keypoints[5] + keypoints[2])/2;
      Vector3 coordNeck = (keypoints[11] + keypoints[12])/2;
      Vector3 coordHip = (keypoints[23] + keypoints[24])/2;
      Vector3 coordLeftKnee = keypoints[26];
      Vector3 coordLeftAnkle = keypoints[28];

      distance += Vector3.Distance(coordEyes, coordNeck);
      distance += Vector3.Distance(coordNeck, coordHip);
      distance += Vector3.Distance(coordHip, coordLeftKnee);
      distance += Vector3.Distance(coordLeftKnee, coordLeftAnkle);

      pixels = (int)(distance / athleteHeight * 100);
    }

    private void CalculateSpeed(Vector3[] keypoints)
    {
      Vector3 hipCoord = (keypoints[23] + keypoints[24])/2;
      Vector3 leftKneeCoord = keypoints[26];
      Vector3 rightKneeCoord = keypoints[25];

      float ang = direction * GetAngle(leftKneeCoord, hipCoord, rightKneeCoord);

      if (ang > prevAng)
      {
        strideFrames++;
        prevAng = ang;
      } else if (times == 0 || times == 1)
      {
        strideFrames++;
        times++;
      }
      else
      {
        direction *= -1;
        prevAng = -ang;
        if (strideFrames > 0)
        {
          float strideMeters = GetStrideDistance(athleteHeight, true);
          float strideTime = strideFrames / (float)cameraFps;
          speed = strideMeters / strideTime * 3.6f; // Convert to km/h
          newSpeed = speed - ((speed - newSpeed) / 4);
          strideFrames = 0;
          times = 0;
        }
      }
      Debug.Log(newSpeed);
    }

    private float GetAngle(Vector3 a, Vector3 b, Vector3 c)
    {
      // Calculate the angle between three points
      Vector3 ab = b - a;
      Vector3 cb = b - c;
      return Mathf.Atan2(cb.y, cb.x) - Mathf.Atan2(ab.y, ab.x);
    }

    private float GetStrideDistance(float height, bool isRunning)
    {
      return isRunning ? height * 0.011f : height * 0.00413f;
    }
    public void GetKeyPoints(PoseLandmarkerResult result)
    {
        // 여기에 Mediapipe 랜드마크 데이터를 가져오는 로직을 구현합니다.
        // 예시를 위해 임시 데이터를 반환합니다.
        Vector3[] mpVector = new Vector3[33];
        for (int i = 0; i < 33; i++)
        {
          mpVector[i] = new Vector3(result.poseLandmarks[0].landmarks[i].x, result.poseLandmarks[0].landmarks[i].y,
            0);//result.poseLandmarks[0].landmarks[i].z);
        }

        

        keypoints = mpVector;
        //return mpVector; // 실제로는 Mediapipe 데이터를 사용해야 합니다.
    }

    // 추가적인 메소드들...
}
