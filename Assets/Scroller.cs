using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mediapipe.Unity.Sample.PoseTracking
{
  public class Scroller : MonoBehaviour
  {
    public ThreeWayRunGame tw;
  // Start is called before the first frame update
  void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      for (int i = 1; i < GetComponentsInChildren<Transform>().Length; i++)
      {
        GetComponentsInChildren<Transform>()[i].position = new Vector3(GetComponentsInChildren<Transform>()[i].position.x, GetComponentsInChildren<Transform>()[i].position.y, GetComponentsInChildren<Transform>()[i].position.z - tw.speedWeight * Time.deltaTime*3.0f);
        if(GetComponentsInChildren<Transform>()[i].position.z <= -21.81f)
        {
          GetComponentsInChildren<Transform>()[i].position = new Vector3(GetComponentsInChildren<Transform>()[i].position.x, GetComponentsInChildren<Transform>()[i].position.y, 74.8f);
        }
      }
    }
  }
}
