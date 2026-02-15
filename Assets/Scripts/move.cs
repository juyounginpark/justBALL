using System.Collections; // Coroutine 사용을 위해 추가
using UnityEngine;
using UnityEngine.InputSystem;

public class move : MonoBehaviour
{
    public float rotationSpeed = 200f;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    // 점프 관련 변수
    public int maxJumpCount = 2;
    private int currentJumpCount = 0;

    // 데미지 관련 변수
    private bool isDamaged = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        // 키보드가 연결되어 있지 않으면 실행하지 않음
        if (Keyboard.current == null) return;

        // 데미지 입은 상태면 조작 불가 (선택 사항, 여기서는 조작 가능하게 둠)
        // if (isDamaged) return; 

        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        // A키를 누르면 왼쪽으로 이동하며 회전
        if (Keyboard.current.aKey.isPressed)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
        }

        // D키를 누르면 오른쪽으로 이동하며 회전
        if (Keyboard.current.dKey.isPressed)
        {
            transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void HandleJump()
    {
        // 스페이스바를 누르면 점프 (최대 점프 횟수 제한)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && rb != null)
        {
            if (currentJumpCount < maxJumpCount)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // 기존 Y 속도 초기화 (이중 점프 시 자연스럽게)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                currentJumpCount++;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥에 닿으면 점프 횟수 초기화
        if (collision.gameObject.CompareTag("Ground"))
        {
            currentJumpCount = 0;
        }

        // 가시에 닿으면 데미지 처리
        if (collision.gameObject.CompareTag("Spike"))
        {
            if (!isDamaged)
            {
                StartCoroutine(TakeDamage());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 가시가 Trigger인 경우 데미지 처리
        if (collision.gameObject.CompareTag("Spike"))
        {
            if (!isDamaged)
            {
                StartCoroutine(TakeDamage());
            }
        }
    }

    IEnumerator TakeDamage()
    {
        isDamaged = true; // 데미지 중복 방지

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        // 떨림 효과 (Camera Shake 대신 캐릭터 자체를 흔듦)
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 originalPos = transform.localPosition;
        float magnitude = 0.1f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos; // 위치 복구

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor; // 색상 복구
        }

        isDamaged = false;
    }
}
