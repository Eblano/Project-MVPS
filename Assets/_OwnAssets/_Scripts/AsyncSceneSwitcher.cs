using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SealTeam4
{
    public class AsyncSceneSwitcher : MonoBehaviour
    {
        [SerializeField] private Image loadingIndicator;

        private void Start()
        {
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
                //loadingIndicator.fillAmount = asyncOperation.progress;
                if (loadingIndicator.fillAmount >= 1)
                    loadingIndicator.fillAmount = 0;
                loadingIndicator.fillAmount += 1 * Time.deltaTime;
                yield return null;
            }
        }
    }
}
