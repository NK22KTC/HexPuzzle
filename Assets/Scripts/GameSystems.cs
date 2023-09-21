using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSystems : MonoBehaviour
{
    float fadeTime = 2f;

    public static int loadSceneNum = 0;

    public void FadeIn(GameObject fadeImageObject, Image fadeImage)
    {
        fadeImageObject.SetActive(true);

        Color color = fadeImage.color;
        color.a -= Time.fixedDeltaTime * fadeTime;
        fadeImage.color = color;
    }

    public void FadeOut(GameObject fadeImageObject, Image fadeImage)
    {
        fadeImageObject.SetActive(true);

        Color color = fadeImage.color;
        color.a += Time.fixedDeltaTime * fadeTime;
        fadeImage.color = color;
    }
    
    public IEnumerator LoadingScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneNum);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}