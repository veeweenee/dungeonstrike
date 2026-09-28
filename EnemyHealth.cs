/* Summary: Deals with the health levels of the enemy 
 */


using UnityEngine;
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float enemyHealth = 30f;
    [SerializeField] private bool hasDeathAnim = false;
    [SerializeField] private bool isBoss = false;

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Rigidbody2D rigidBody;
    private Color originalColor;

    private bool isEnemyDead = false;
    private bool isFlashing = false;

    public bool isExhausted=> isEnemyDead;
    
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void GetsInjury(float damage)
    {
        if (isEnemyDead)
            return;

        enemyHealth -= damage;

        StartCoroutine(HurtFlash());

        if (enemyHealth <= 0f)
        {
            enemyHealth = 0f;
            enemyDies();
        }
    }

    public void enemyDies()
    {
        if (isEnemyDead)
            return;

        isEnemyDead = true;

        if (isBoss)
        {
            Debug.Log("Boss is defeated");
            
            if (GameManager.instance != null)
            {
                Debug.Log("Move to the next level");
                GameManager.instance.NextLevel();
            }
            else
            {
                Debug.Log("GameManager is null..");

            }
            
        }

        rigidBody.linearVelocity = Vector2.zero;
        anim.SetBool("isMoving", false);

        if (hasDeathAnim)
            anim.SetTrigger("deathTrigger");
        else
            StartCoroutine(DeathFade());

        Debug.Log("Enemy has died");
    }

    private System.Collections.IEnumerator HurtFlash()
    {
        if (isFlashing)
            yield break;

        isFlashing = true;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = originalColor;
        isFlashing = false;
    }
    private System.Collections.IEnumerator DeathFade()
    {
        float fadeDuration = 0.6f;
        float timeElapsed = 0f;
        Color defColor = spriteRenderer.color;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);

            spriteRenderer.color = new Color(defColor.r,
                defColor.g,
                defColor.b,
                alpha);

            yield return null;

        }
        spriteRenderer.color = new Color(defColor.r,
               defColor.g,
               defColor.b,
               0f);
    }

}
