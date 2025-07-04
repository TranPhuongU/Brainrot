using System.Collections;
using UnityEngine;

public enum ItemType
{
    TungSahur,
    Tralalero,
    Bananini,
    Capuchino,
    BrBrPatapim,
    BriBri,
    Lulilo
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class ItemController : MonoBehaviour
{
    public int amountScore;

    [SerializeField] GameObject smokeFx;
    [SerializeField] AudioClip dropSfx;

    [SerializeField] private AudioClip mergeSound;
    private AudioSource audioSource;

    [SerializeField] float timeSpawn;
    private bool canGrow;
    public float maxSize;
    public float growSpeed;

    public float moveSpeed = 5f;
    public bool isDragging = true; // Cho phép gán từ bên ngoài
    private bool isPlayerSpawned = false;
    private Rigidbody2D rb;
    private float screenWidth;

    private float defaulGravity = 1;

    [SerializeField] public ItemType itemType;
    [SerializeField] private GameObject itemObject; // Prefab nâng cấp tiếp theo

    private bool canMoveRight = true;
    private bool canMoveLeft = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        screenWidth = Screen.width;
        audioSource = GetComponent<AudioSource>();
    }

    public void SetupItemObject(float _gravity, Rigidbody2D _rb, bool _canGrow)
    {
        rb = _rb;
        rb.gravityScale = _gravity;
        canGrow = _canGrow;
        if (!canGrow)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(.1f, .1f, .1f);
        }
    }

    void Update()
    {
        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (!isDragging) return;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float newX = worldPos.x;

            // Giới hạn theo vùng deadzone
            if (!canMoveRight && newX > transform.position.x)
                newX = transform.position.x;
            if (!canMoveLeft && newX < transform.position.x)
                newX = transform.position.x;

            transform.position = new Vector3(newX, transform.position.y, 0);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Drop();
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
            float newX = worldPos.x;

            if (!canMoveRight && newX > transform.position.x)
                newX = transform.position.x;
            if (!canMoveLeft && newX < transform.position.x)
                newX = transform.position.x;

            transform.position = new Vector3(newX, transform.position.y, 0);

            if (touch.phase == TouchPhase.Ended)
            {
                Drop();
            }
        }
#endif
    }

    void Drop()
    {
        audioSource.PlayOneShot(dropSfx);
        isDragging = false;
        rb.gravityScale = defaulGravity;

        if (isPlayerSpawned)
        {
            StartCoroutine(DelayedSpawn(timeSpawn));
        }
    }

    public void MarkAsPlayerSpawned()
    {
        isPlayerSpawned = true;
    }

    public void DisableDragging()
    {
        isDragging = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDragging && collision.gameObject.TryGetComponent(out ItemController other))
        {
            if (other.itemType == this.itemType && GetInstanceID() < other.GetInstanceID())
            {

                if (itemObject == null || itemType == ItemType.Lulilo)
                {
                    Vector3 mergePoss = (transform.position + other.transform.position) / 2;
                    Instantiate(smokeFx, mergePoss, Quaternion.identity);
                    GameManager.Instance.PlaySound(mergeSound);
                    GameManager.Instance.currentScore += amountScore;
                    Destroy(other.gameObject);
                    Destroy(this.gameObject);
                    return;
                }

               
                Vector3 mergePos = (transform.position + other.transform.position) / 2;
                Instantiate(smokeFx, mergePos, Quaternion.identity);
                GameObject newItem = Instantiate(itemObject, mergePos, Quaternion.identity);

                GameManager.Instance.currentScore += amountScore;

                newItem.GetComponent<ItemController>().SetupItemObject(1, GetComponent<Rigidbody2D>(), false);
                GameManager.Instance.PlaySound(mergeSound);

                if (newItem.TryGetComponent(out ItemController newController))
                {
                    newController.DisableDragging();
                }

                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
    }

    public void PlaySpawnEffect(float duration = 0.25f, float targetScale = 1f)
    {
        StartCoroutine(ScaleUpCoroutine(duration, targetScale));
    }

    IEnumerator ScaleUpCoroutine(float duration, float targetScale)
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        float time = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * targetScale;

        transform.localScale = startScale;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        transform.localScale = endScale;

        if (col != null) col.enabled = true;
    }

    IEnumerator DelayedSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.SpawnNextItem();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("DeadZoneRight"))
            canMoveRight = false;
        else if (col.CompareTag("DeadZoneLeft"))
            canMoveLeft = false;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("DeadZoneRight"))
            canMoveRight = true;
        else if (col.CompareTag("DeadZoneLeft"))
            canMoveLeft = true;
    }
}
