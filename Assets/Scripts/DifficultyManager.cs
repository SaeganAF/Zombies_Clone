using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("Difficulty Timing")]
    public float difficultyInterval = 90f; // 1 minute 30 seconds

    [Header("Scaling Amounts")]
    public float speedIncrease = 0.5f;
    public int healthIncrease = 3;

    [Header("Current Difficulty Level")]
    public int difficultyLevel = 0;

    private float timer = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= difficultyInterval)
        {
            timer = 0f;
            difficultyLevel++;
            Debug.Log("Difficulty increased! Level: " + difficultyLevel);
        }
    }

    public float GetSpeedMultiplier()
    {
        return 1f + (difficultyLevel * speedIncrease);
    }

    public int GetExtraHealth()
    {
        return difficultyLevel * healthIncrease;
    }
}
