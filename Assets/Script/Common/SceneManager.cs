using UnityEngine;
using sm = UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instant;
    [SerializeField] private SceneField MenuScene;
    [SerializeField] private SceneField GameplayScene;
    private void Awake()
    {
        if (Instant == null)
        {
            Instant = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(SceneType type)
    {
        switch (type)
        {
            case SceneType.Menu:
                sm.SceneManager.LoadScene(MenuScene);
                break;
            case SceneType.GamePlay:
                sm.SceneManager.LoadScene(GameplayScene);
                break;
        }
    }
}

public enum SceneType
{
    Menu,
    GamePlay
}