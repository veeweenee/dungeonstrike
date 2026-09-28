/* Summary: Handles the enemy AI behaviour when going towards the
 *          the player, trigggering attacks and the retreat movement
 *          of the worm boss.
 */

using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float rangeofDetection = 5f;

    private EnemyController enemyCtrl;
    private EnemyCombat enemyCombat;
    private EnemyHealth enemyHealth;


    private void Awake()
    {
        enemyCtrl = GetComponent<EnemyController>();
        enemyCombat = GetComponent<EnemyCombat>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (enemyHealth.isExhausted)
        {
            enemyCtrl.StopAnim();
            return;
        }

        if (enemyCtrl.player == null)
        {
            return;
        }

        PlayerHealth playerHealth = enemyCtrl.player.GetComponent<PlayerHealth>();

        if (playerHealth == null || playerHealth.currentHealth <= 0f)
        {
            enemyCtrl.StopAnim();
            return;
        }
        
        if (enemyCtrl.usesRetreatBhvr)
        {
            WormBhvr();
            return;

        }


        float playerDist = Vector2.Distance(transform.position, enemyCtrl.player.position);

        if (playerDist <= enemyCombat.RangeOfAtk)
        {
            enemyCtrl.StopAnim();
            enemyCombat.Attack(playerHealth);

        }
        else if(playerDist <= rangeofDetection)
        {
            Vector2 direction = (enemyCtrl.player.position - transform.position).normalized;
            enemyCtrl.Move(direction);
        }
        else
        {
            enemyCtrl.StopAnim();
        }
    }

   private void WormBhvr() 
   {
        float playerDist = Vector2.Distance(transform.position,
            enemyCtrl.player.position);
        
        if(enemyCtrl.CanRetreat && enemyCtrl.IsWormIdle &&
            playerDist <= enemyCtrl.CurrRetreatDist)
        {
            enemyCtrl.BeginRetreat();
        }

        if(playerDist <= enemyCombat.RangeOfAtk)
        {
            enemyCombat.Attack(
                enemyCtrl.player.GetComponent<PlayerHealth>());
        }

        enemyCtrl.StopAnim();

    }

    //void FollowPlayer()
    //{
    //    Vector2 direction = (enemyCtrl.player.position - transform.position).normalized;
    //    enemyCtrl.Move(direction);
    //}
}
