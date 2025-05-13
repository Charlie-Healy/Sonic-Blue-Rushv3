using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined; 
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayGame1()
    {
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Quit Game!");
    }
}
