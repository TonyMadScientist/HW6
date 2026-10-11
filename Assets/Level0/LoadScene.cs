using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene:MonoBehaviour
{
    public void LadScenebyName(string name)
    {
        SceneManager.LoadScene("Scene1-FarmUI&Cam");
    }
}
