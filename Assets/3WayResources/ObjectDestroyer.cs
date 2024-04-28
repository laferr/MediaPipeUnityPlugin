using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mediapipe.Unity.Sample.PoseTracking
{
  public class ObjectDestroyer : MonoBehaviour
  {
    GameObject controller;
    public int speed = 12;
    // Start is called before the first frame update
    void Start()
    {
      controller = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
      GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -speed * controller.GetComponent<ThreeWayRunGame>().speedWeight);
      if (transform.position.z < -15.0f)
      {
        Destroy(gameObject);
      }
    }
  }
}
