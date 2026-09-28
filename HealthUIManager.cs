/* Summary: Handles the player's health status visually, with 
 *          heart containers in relation to the total health level
 *          of the player.
 */

using UnityEngine;
using UnityEngine.UI;

public class HealthUIManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image[] heartContainers;

    public Sprite emptyHeart;
    public Sprite halfHeart;
    public Sprite fullHeart;

    private const float unitPerFullHeart = 20f;

    
    void Update()
    {
        UpdateHeartContainers();
    }

    private void UpdateHeartContainers()
    {
        float currentHealth = playerHealth.currentHealth;
    
        for (int i = 0; i < heartContainers.Length; i++)
        {
            float heartHealth = currentHealth - (i * unitPerFullHeart);
           
            if (heartHealth >= unitPerFullHeart)
            {
                heartContainers[i].sprite = fullHeart;
            }
            else if (heartHealth >= unitPerFullHeart / 2f)
            {
                heartContainers[i].sprite = halfHeart;
            }
            else
            {
                heartContainers[i].sprite = emptyHeart;

            }
        }

    }
}
