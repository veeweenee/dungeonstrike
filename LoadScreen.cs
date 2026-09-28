/* Summary: Loads the main scene containing the game
 */

using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadScreen : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene("Main");
    }
}
