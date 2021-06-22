using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public string gameSceneName;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            SceneManager.LoadSceneAsync(gameSceneName);
        }
    }
}
