using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSetter : MonoBehaviour
{
  public GameObject bg;
  public Texture tt;
  // Start is called before the first frame update
  void Start()
  {
    StartCoroutine(BGTrans());
  }
  IEnumerator BGTrans()
  {
    yield return new WaitForSeconds(3.0f);
    Debug.Log("Try Change");
    bg.GetComponent<RawImage>().material.SetTexture("_MaskTex", tt);
    bg.GetComponent<RawImage>().material.SetFloat("_Threshold", 0.4f);
  }
  // Update is called once per frame
  void Update()
  {

  }
}
