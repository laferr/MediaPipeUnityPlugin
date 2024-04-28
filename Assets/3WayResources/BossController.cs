using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
  int counter = 3;
  public Text counterText;
  public GameObject powerUpPrefab;
  private void Start()
  {
    counter = Random.Range(3, 8);
    counterText.text = counter.ToString();
  }
  private void OnTriggerEnter(Collider collision)
  {
    if(collision.tag == "Bullet" | collision.tag == "Player")
    {
      counter--;
      counterText.text = counter.ToString();
      if (counter <= 0)
      {
        GameObject go = Instantiate(powerUpPrefab);
        go.transform.position = gameObject.transform.position;
        go.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -10);
        Destroy(gameObject);
      }
    }
  }
}
