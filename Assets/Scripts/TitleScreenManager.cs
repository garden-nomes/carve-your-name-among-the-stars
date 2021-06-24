using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public string gameSceneName;
    public DitheredFadeEffect fadeEffect;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            StartCoroutine(StartGameCoroutine());
        }
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return fadeEffect.FadeOut();
        SceneManager.LoadSceneAsync(gameSceneName);
    }
}
