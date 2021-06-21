using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject theEndUI;
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

    private void Start()
    {
        theEndUI.SetActive(false);
    }

    public void EndGame(bool canContinue)
    {
        StartCoroutine(EndGameCoroutine(canContinue));
    }

    private IEnumerator EndGameCoroutine(bool canContinue)
    {
        spaceship.disableThrottle = true;
        theEndUI.SetActive(true);
        yield return null;
        while (!Input.GetKeyUp(KeyCode.Z)) { yield return null; }

        if (canContinue)
        {
            spaceship.disableThrottle = false;
            theEndUI.SetActive(false);
        }
        else
        {
            Restart();
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
