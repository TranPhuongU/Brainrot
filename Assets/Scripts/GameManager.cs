using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] itemPrefabs;
    public Transform spawnPoint;
    private AudioSource audioSource;
    public GameObject setting;

    public int currentScore = 0;
 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    void Start()
    {
        SpawnNextItem();
    }

    public void SpawnNextItem()
    {
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject newItem = Instantiate(itemPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        newItem.GetComponent<ItemController>().SetupItemObject(0, newItem.GetComponent<Rigidbody2D>(), true);

        if (newItem.TryGetComponent(out ItemController controller))
        {
            controller.MarkAsPlayerSpawned();
        }
    }
}
