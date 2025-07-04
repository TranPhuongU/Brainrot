using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverZone : MonoBehaviour
{
    private float timer;

    private void Start()
    {
        timer = 3;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<ItemController>() != null)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SceneManager.LoadScene("GameOverScene");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<ItemController>() != null)
        {
            timer = 3;
        }
    }
}
