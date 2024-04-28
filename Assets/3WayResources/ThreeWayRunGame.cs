using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace Mediapipe.Unity.Sample.PoseTracking
{
  public class ThreeWayRunGame : MonoBehaviour
  {
    public GameObject mid_Prefab;
    public GameObject left_Prefab;
    public GameObject right_Prefab;

    public GameObject boss_Prefab;
    public GameObject enemy_Prefab;

    public GameObject[] healthSprite;

    public GameObject gameOverText;
    private float timer;
    public float speedWeight = 1.0f;

    public Renderer road;

    bool isMidTurn = true;
    bool gameOver = true;
    public float offset = 0.0f;

    int playerHealth = 3;
    int score = 0;

    public Text scoreText;

    public JumpingCountSolution js;

    float bossTimer = 0.0f;
    float coinTimer = 0.0f;

    public GameObject PowerUpSelectPanel;
    private void Start()
    {
      StartCoroutine(CaliDelayer());
    }
    IEnumerator CaliDelayer()
    {
      yield return new WaitForSeconds(3.0f);
      gameOver = false;
      speedWeight = 1.0f;
      timer = 0.0f;
      playerHealth = 3;
      score = 0;
      healthSprite[0].SetActive(true);
      healthSprite[1].SetActive(true);
      healthSprite[2].SetActive(true);
      scoreText.text = "00";
      gameOverText.SetActive(false);
    }
    // Update is called once per frame
    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.C))
      {
        gameOver = false;
        speedWeight = 1.0f;
        timer = 0.0f;
        playerHealth = 3;
        score = 0;
        healthSprite[0].SetActive(true);
        healthSprite[1].SetActive(true);
        healthSprite[2].SetActive(true);
        scoreText.text = "00";
        gameOverText.SetActive(false);
      }
      if (!gameOver)
      {
        offset += Time.deltaTime * speedWeight * 0.3f;
        road.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));

        timer += Time.deltaTime;
        bossTimer += Time.deltaTime;
        coinTimer += Time.deltaTime;
        if(coinTimer > 5.0f / speedWeight)
        {
          int r = Random.Range(0, 3);
          if (r == 0)
          {
            GameObject left = Instantiate(left_Prefab);
          }
          else if (r == 1)
          {
            GameObject right = Instantiate(right_Prefab);
          }
          else
          {
            GameObject mid = Instantiate(mid_Prefab);
          }
          coinTimer = 0.0f;
        }
        if (timer > 2.0f / speedWeight)
        {
          GameObject enemy = Instantiate(enemy_Prefab);
          enemy.transform.position += new Vector3(Random.Range(-4.0f, 4.0f), 0, 0);
          speedWeight *= 1.01f;
          timer = 0.0f;
        }
        if (bossTimer > 12.0f / speedWeight)
        {
          GameObject boss = Instantiate(boss_Prefab);
          boss.transform.position += new Vector3(Random.Range(-4.0f, 4.0f), 0, 0);
          bossTimer = 0.0f;
        }
        //Test Input
        //if (Input.GetKey(KeyCode.A) || js.CLR == 1)
        //{
        //  transform.position = new Vector3(-3, transform.position.y, transform.position.z);
        //}
        //else if (Input.GetKey(KeyCode.D) || js.CLR == 2)
        //{
        //  transform.position = new Vector3(3, transform.position.y, transform.position.z);
        //}
        //else
        //{
        //  transform.position = new Vector3(0, transform.position.y, transform.position.z);
        //}
      }
      else
      {
        speedWeight = 0.0f;
        timer = 0.0f;
      }
      if (Input.GetKeyDown(KeyCode.R))
      {
        SceneManager.LoadScene("3Way");
      }
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag("Obstacle") | other.CompareTag("Enemy"))
      {
        Destroy(other.gameObject);
        playerHealth--;
        switch (playerHealth)
        {
          case 2:
            healthSprite[0].SetActive(false);
            break;
          case 1:
            healthSprite[1].SetActive(false);
            break;
          case 0:
            healthSprite[2].SetActive(false);
            gameOver = true;
            gameOverText.SetActive(true);
            break;
          default:
            break;
        }
        _ = StartCoroutine(CollisionDelay());
      }
      else if (other.CompareTag("Coin"))
      {
        Destroy(other.gameObject);
        score++;
        scoreText.text = string.Format("{0}", score.ToString("D2"));
      }
      else if (other.CompareTag("PowerUp"))
      {
        Destroy(other.gameObject);
        Time.timeScale = 0;
        js.isCalibrated = false;
        PowerUpSelectPanel.SetActive(true);
      }
    }

    private IEnumerator CollisionDelay()
    {
      GetComponent<CapsuleCollider>().enabled = false;
      yield return new WaitForSeconds(1.0f);
      GetComponent<CapsuleCollider>().enabled = true;
    }

    public void PowerUpButton(int index)
    {
      switch (index)
      {
        case 0:
          js.bulletType = JumpingCountSolution.BulletType.normal;
          break;
        case 1:
          js.bulletType = JumpingCountSolution.BulletType.triple;
          break;
        case 2:
          js.bulletType = JumpingCountSolution.BulletType.spiral;
          break;
      }
      PowerUpSelectPanel.SetActive(false);
      js.isCalibrated = true;
      Time.timeScale = 1;
    }
  }
}
