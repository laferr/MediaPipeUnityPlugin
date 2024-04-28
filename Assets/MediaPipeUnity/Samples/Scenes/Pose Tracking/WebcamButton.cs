using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mediapipe.Unity.Sample.PoseTracking
{
  public class WebcamButton : MonoBehaviour
  {
    //public PointerEventData eventData;
    public float gaugeTime = 2.0f;
    public GameObject gauge;
    private bool isActivated = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if (isHold && !isActivated)
      {
        gauge.GetComponent<UnityEngine.UI.Image>().fillAmount += (1.0f / gaugeTime) * Time.deltaTime;
        if (gauge.GetComponent<UnityEngine.UI.Image>().fillAmount >= 1.0f)
        {
          OnHoldEnded();
        }
      }
      else
      {
        gauge.GetComponent<UnityEngine.UI.Image>().fillAmount = 0.0f;
      }
    }
    bool isHold = false;
    public void OnPointerEnter()
    {
      isHold = true;
      //Debug.Log("isHold " + isHold);
    }
    public void OnPointerExit()
    {
      isHold = false;
      isActivated = false;
      //Debug.Log("isHold " + isHold);
    }
    public void OnHoldEnded()
    {
      //Debug.Log("HoldEnd");
      isHold = false;
      isActivated = true;
      GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
    }
  }
}
