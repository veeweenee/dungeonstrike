/* Summary: Deal with the different enemy combat styles toward the 
 *          player and ensures that damage is inflicted upon the
 *          player.
 */

using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public enum AtkType
    {
        Contact,
        Regular,
        Delay
    }

    [SerializeField] private AtkType atkType = AtkType.Regular;
    [SerializeField] private float attackDealt = 10f;
    [SerializeField] private float attackFrequency = 1f;
    [SerializeField] private float rangeofAttack = 0.7f;
    public float RangeOfAtk => rangeofAttack;

    private EnemyController enemyCtrl;
    private float atkImpactDelay = 0.4f;
    private float timeBetweenAttacks = 0f;

    void Awake()
    {
        enemyCtrl = GetComponent<EnemyController>();
    }

    public void Attack(PlayerHealth playerHealth)
    {
        if (playerHealth == null)
        {
            return;
        }

        timeBetweenAttacks -= Time.deltaTime;

        if(timeBetweenAttacks > 0f)
        {
            return;
        }

        switch (atkType)
        {
            case AtkType.Contact:
                ContactAtk(playerHealth);
                break;
            case AtkType.Regular:
                RegAtk(playerHealth);
                break;
            case AtkType.Delay:
                DelayAtk(playerHealth);
                break;
        }
        timeBetweenAttacks = attackFrequency;
    }

    private void ContactAtk(PlayerHealth playerHealth)
    {
        Vector2 direction = (playerHealth.transform.position - transform.position).normalized;
        enemyCtrl.SetDirection(direction);
        //enemyCtrl.SetMovingAnim(true);
        enemyCtrl.StopAnim();

        playerHealth.GetsInjury(attackDealt);
    }

    private void RegAtk(PlayerHealth playerHealth)
    {
        enemyCtrl.StopAnim();
        enemyCtrl.PlayAtkAnim();
        playerHealth.GetsInjury(attackDealt);
    }

    // Lets the attack line up by introducing a delay
    private void DelayAtk(PlayerHealth playerHealth)
    {
        enemyCtrl.StopAnim();
        enemyCtrl.PlayAtkAnim();
        StartCoroutine(DelayDmg(playerHealth));

    }
    private System.Collections.IEnumerator DelayDmg(PlayerHealth playerHealth)
    {
        yield return new WaitForSeconds(atkImpactDelay);

        if (playerHealth != null)
            playerHealth.GetsInjury(attackDealt);
    }

    public void WormAttack()
    {
        if (enemyCtrl.player == null)
            return;

        float distance = Vector2.Distance(transform.position, enemyCtrl.player.position);

        if (distance < rangeofAttack)
        {
            return;
        }
            PlayerHealth playerHealth = enemyCtrl.player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.GetsInjury(attackDealt); 
        }
    }

}
