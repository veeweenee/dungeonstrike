/* Summary: Manages enemies' directional movement and their speed
 */

using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
 
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool usesDirectionalAnim = false;

    // Worm Retreat Behaviour
    [SerializeField] public bool usesRetreatBhvr = false;
    [SerializeField] private float minRetreatDist = 3f;
    [SerializeField] private float maxRetreatDist = 6f;
    [SerializeField] private float ambushChance = 0.4f;
    [SerializeField] private float retreatDelay;

    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    private float currRetreatDist;
    private bool canRetreat = true;

    public bool CanRetreat => canRetreat;
    public bool IsWormIdle => anim.GetCurrentAnimatorStateInfo(0).IsName("Worm_Idle_Tree");
    public float CurrRetreatDist => currRetreatDist;


    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Start()
    {
        if (usesRetreatBhvr)
        {
            currRetreatDist = Random.Range(minRetreatDist, maxRetreatDist);
            anim.Play("Worm_Appear_Tree");          
        }
    }

    public void Move(Vector2 direction)
    {
        rigidBody.linearVelocity = direction * moveSpeed;
        SetDirection(direction);
        anim.SetBool("isMoving", true);
    }

    public void SetDirection(Vector2 direction)
    {
        // Flip the sprite to face the player's horizontal direction.
        if (direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        // Updates the animation values for the rat specifically.
        if (usesDirectionalAnim)
        {
            anim.SetFloat("moveX", direction.x);
            anim.SetFloat("moveY", direction.y);
        }
    }
    
    public void PlayAtkAnim()
    {
        anim.SetTrigger("attackTrigger");
    }

    public void SetMovingAnim(bool moving)
    {
        anim.SetBool("isMoving", moving);
    }

    public void StopAnim()
    {
        rigidBody.linearVelocity = Vector2.zero;
        anim.SetBool("isMoving", false);
    }

    public void BeginRetreat()
    {
        canRetreat = false;
        anim.SetTrigger("retreatTrigger");
        StartCoroutine(RetreatAndAppear());
    }
  
    private System.Collections.IEnumerator RetreatAndAppear()
    {
        yield return new WaitForSeconds(retreatDelay);

        DungeonGenerator dungeon = 
            FindFirstObjectByType<DungeonGenerator>();

        RectInt playerRoom = new RectInt();

        Vector2Int playerPos = Vector2Int.FloorToInt(player.position);

        foreach (RectInt room in dungeon.BSPAlg.generatedRooms)
        {

            if (room.Contains(playerPos))
            {
                playerRoom = room;
                break;
            }
        }

        Vector2Int newPos = Vector2Int.zero;
        bool validPos = false;

        // Worm can ambush the player given a chance
        if (Random.value < ambushChance && playerRoom.Contains(playerPos))
        {
                newPos = playerPos;
                validPos = true;
        }

        else
        {
            // Find a valid position within the room
            for (int attempts = 0; attempts < 50; attempts++)
            {
                Vector2 direction = Random.insideUnitCircle.normalized;
                float distFromPlayer = Random.Range(minRetreatDist, maxRetreatDist);
                Vector2 position = (Vector2)player.position + direction * distFromPlayer;
                Vector2Int testPos = Vector2Int.RoundToInt(position);

                // Let the worm stay within the room's bounds
                if (testPos.x > playerRoom.xMin + 1 &&
                    testPos.x < playerRoom.xMax - 1 &&
                    testPos.y > playerRoom.yMin + 1 &&
                    testPos.y < playerRoom.yMax - 1)
                {
                    newPos = testPos;
                    validPos = true;
                    break;
                }
            }
        }

        if (!validPos)
        {
            canRetreat = true;
            yield break;
        }

        transform.position = new Vector3(newPos.x, newPos.y, 0f);
        anim.Play("Worm_Appear_Tree");

        yield return new WaitForSeconds(0.5f);
        canRetreat = true;
    }
}
