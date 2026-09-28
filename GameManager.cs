/* Summary: Manages the different states of the game regarding
 *          the game's levels and most of the menu settings.
 */

using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currLevel = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void NextLevel()
    {
        if (currLevel < 3)
        {
            currLevel++;
            Invoke(nameof(LoadNextLevel), 1.5f);
        }
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene("LoadScreen");
    }

    // TODO: factor in the pause menu here

    public void RetryGame()
    {
        currLevel = 1;
        SceneManager.LoadScene("LoadScreen");
    }
    public void GoToMainMenu()
    {
        currLevel = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
