using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene:MonoBehaviour
{
    public AudioClip clickSound;
    public void LadScenebyName(string name)
    {
         GameObject soundObj = new GameObject("ClickSound");
        DontDestroyOnLoad(soundObj);
        soundObj.AddComponent<AudioSource>().PlayOneShot(clickSound);
        Destroy(soundObj, clickSound.length);
        SceneManager.LoadScene("Scene1-FarmUI&Cam");
        
    }
}
