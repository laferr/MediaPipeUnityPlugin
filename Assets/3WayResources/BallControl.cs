using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BallControl : MonoBehaviour
{
  public bool isLeft = false;
  public float speed = 10.0f;

  GameObject canvas;
  public GameObject shadowPrefab;
  GameObject shadow;
  // Start is called before the first frame update
  void Start()
  {
    canvas = GameObject.Find("Canvas");
    if (isLeft)
      gameObject.transform.position = new Vector3(Random.Range(0, 4.0f), Random.Range(-2.3f, 3.0f), 100);
    else
      gameObject.transform.position = new Vector3(Random.Range(0, -4.0f), Random.Range(-2.3f, 3.0f), 100);

    GetComponent<Rigidbody>().velocity = -transform.forward * speed;
    shadow = Instantiate(shadowPrefab);
    shadow.transform.parent = canvas.transform;
    shadow.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    shadow.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -3.5f);
    shadow.GetComponent<Image>().DOFade(0.25f, 2.0f).SetDelay(3.5f);
  }

  private void OnCollisionEnter(Collision collision)
  {
    Destroy(gameObject);
    Destroy(shadow);
  }
}
