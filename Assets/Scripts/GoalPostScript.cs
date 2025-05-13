using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalPostScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
            if (col.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene("MainMenu");
            Cursor.lockState = CursorLockMode.None;
            }
    }
}
