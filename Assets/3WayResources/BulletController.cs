using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
  public float speed = 10.0f;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    GetComponent<Rigidbody>().velocity = transform.forward * speed;
  }

  private void OnTriggerEnter(Collider collision)
  {
    if(!collision.CompareTag("Player") && !collision.CompareTag("Coin") && !collision.CompareTag("Bullet"))
      Destroy(gameObject);
  }
}
