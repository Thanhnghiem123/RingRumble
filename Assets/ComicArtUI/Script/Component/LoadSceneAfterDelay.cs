using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace ComicUI
{
    public class LoadSceneAfterDelay : MonoBehaviour
    {
        public string sceneName;
        public Image loadingImage;
        

        public static LoadSceneAfterDelay Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            StartCoroutine(LoadSceneDelay());
        }

        public IEnumerator LoadSceneDelay()
        {
            float duration = 2f;
            float count = 0;

            while (duration > count)
            {
                count += Time.deltaTime;
                loadingImage.fillAmount = count;
                yield return null;
            }

            SceneManager.LoadScene(sceneName);
        }

        public void SetScene(string sceneName)
        {
            this.sceneName = sceneName;
        }

    }
}