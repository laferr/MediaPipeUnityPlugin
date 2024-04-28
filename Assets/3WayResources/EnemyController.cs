using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
  public GameObject[] meshes;
  private void Start()
  {
    int r = Random.Range(0, 3);
    meshes[r].SetActive(true);
  }
  private void OnTriggerEnter(Collider collision)
  {
    if (collision.GetComponent<Collider>().tag == "Bullet" || collision.GetComponent<Collider>().tag == "Player")
    {
      Destroy(gameObject);
    }
  }
}
