using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string titleSceneName;
    public SpaceshipController spaceship;

    // singleton pattern, sue me
    private static GameManager _instance;
    public static GameManager instance => _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync(titleSceneName);
    }
}
