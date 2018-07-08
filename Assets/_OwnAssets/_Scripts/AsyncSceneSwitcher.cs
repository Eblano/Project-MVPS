using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace SealTeam4
{
    public class AsyncSceneSwitcher : MonoBehaviour
    {
        [SerializeField] private Image loadingIndicator;

        private void Start()
        {
            loadingIndicator.fillAmount = 0;
            StartCoroutine(LoadNextScene());
        }

        private IEnumerator LoadNextScene()
        {
            string sceneToLoad;

            if (PlayerPrefs.HasKey("SceneToLoad"))
            {
                sceneToLoad = PlayerPrefs.GetString("SceneToLoad");
                PlayerPrefs.DeleteKey("SceneToLoad");
            }
            else
                sceneToLoad = "_MainScene";

            Scene newScene = SceneManager.GetSceneByName(sceneToLoad);
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);

            while (!asyncOperation.isDone)
            {
                loadingIndicator.fillAmount = Mathf.Lerp(loadingIndicator.fillAmount, asyncOperation.progress * 1.11111f, 5 * Time.deltaTime);
                yield return null;
            }
        }
    }
}
