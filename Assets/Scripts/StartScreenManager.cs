using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    public GameObject startScreen;

    private bool gameStarted = false;

    void Start()
    {
        if (startScreen != null) startScreen.SetActive(true);

        // Unlock cursor so player can click Play
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Freeze all enemy and player movement by pausing time
        Time.timeScale = 0f;
    }

    void Update()
    {
        // Also allow pressing Space or Enter to start
        if (!gameStarted && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        if (startScreen != null) startScreen.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}