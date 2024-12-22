using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private void OnEnable()
    {
        Cursor.SetCursor(GameManager.instance.UIMouseCursor, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 0)
        {
            Cursor.SetCursor(GameManager.instance.shootMouseCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.visible = false;
        }
        
    }
}
