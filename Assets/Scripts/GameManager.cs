using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject theEnd;

    // singleton pattern -- sue me
    private static GameManager _instance;
    public static GameManager instance => _instance;
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        theEnd.SetActive(false);
    }

    public void EndGame()
    {
        StartCoroutine(EndGameCoroutine());
    }

    private IEnumerator EndGameCoroutine()
    {
        theEnd.SetActive(true);
        yield return null;
        while (!Input.GetKeyUp(KeyCode.Z)) { yield return null; }
        Restart();
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
