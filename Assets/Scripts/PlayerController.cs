using UnityEngine;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour
{
    public AudioClip deathClip; // 사망시 재생할 오디오 클립
    public float jumpForce = 700f; // 점프 힘

    private int jumpCount = 0; // 누적 점프 횟수
    private bool isGrounded = false; // 바닥에 닿았는지 나타냄
    private bool isDead = false; // 사망 상태

    private Rigidbody2D playerRigidbody; // 사용할 리지드바디 컴포넌트
    private Animator animator; // 사용할 애니메이터 컴포넌트
    private AudioSource playerAudio; // 사용할 오디오 소스 컴포넌트

    private void Start()
    {
        // 초기화
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {

        // 사망 시 Return
        if (isDead)
            return;

        // 사용자 입력을 감지하고 점프하는 처리
        if (Input.GetMouseButtonDown(0) && jumpCount < 2)
        {
            // 점프 횟수 증가
            jumpCount++;

            // 점프 직전 속도 (0, 0)으로 변경
            playerRigidbody.velocity = Vector2.zero;

            // RigidBody에 +y로 힘 주기
            playerRigidbody.AddForce(new Vector2(0, jumpForce));

            // Audio Source 재생
            playerAudio.Play();
        }
        else if (Input.GetMouseButtonUp(0) && playerRigidbody.velocity.y > 0)
        {
            // 마우스 왼쪽 버튼에서 손을 떼고 +y로 상승 중이라면 현재 속도를 절반으로
            playerRigidbody.velocity = playerRigidbody.velocity * 0.5f;
        }

        // 애니메이터의 Grounded 파라미터를 isGround 값으로 갱신
        animator.SetBool("Grounded", isGrounded);
    }

    // 사망 처리
    private void Die()
    {
        //Animator의 Die 트리거 활성화
        animator.SetTrigger("Die");

        //오디오 클립을 deathClip으로 설정
        playerAudio.clip = deathClip;
        //효과음 재생
        playerAudio.Play();

        //속도를 (0, 0)으로 변경
        playerRigidbody.velocity = Vector2.zero;
        //사망 상태를 true로 변경
        isDead = true;

    }

    // 트리거 콜라이더를 가진 장애물과의 충돌을 감지
    private void OnTriggerEnter2D(Collider2D other)
    {
        //상대방 태그가 Dead, 내가 죽지 않았을 때
        if (other.tag == "Dead" && !isDead)
        {
            Die();
        }
    }

    // 바닥에 닿았음을 감지하는 처리
    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
         * Collider와 닿았다 && 충돌 표면이 위쪽이다
         * 충돌 표면이 위쪽이다: normal의 y값이 0.7f (약 45도 위) 이상이다
         */
        if (collision.contacts[0].normal.y > 0.7f)
        {
            //Ground 처리
            isGrounded = true;
            jumpCount = 0;
        }
    }

    // 바닥에서 벗어났음을 감지하는 처리
    private void OnCollisionExit2D(Collision2D collision)
    {
        //Collider에서 떼어진 경우 isGrounded = false
        isGrounded = false;
    }
}