using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TankGame
{
    public enum LoadingSceneBehavior
    {
        None,
        AnyKey,
        SpecificKey,
        WaitSeconds
    }

    public class LoadingSceneOptions
    {
        public LoadingSceneBehavior LoadingSceneBehavior = LoadingSceneBehavior.None;
        public KeyCode KeyCode = KeyCode.Space;
        public float Seconds = 1f;
        public string LoadingSceneSceneName = "Loading";
        public string LoadingBarTag = "LoadBar";

        public LoadingSceneOptions() { }

        public LoadingSceneOptions(LoadingSceneBehavior loadingScreenBehavior)
        {
            LoadingSceneBehavior = loadingScreenBehavior;
        }
    }

    public class SceneLoadManager : Singleton<SceneLoadManager>
    {
        public bool SetActiveScene(string scene)
        {
            return SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
        }

        public AsyncOperation UnloadScene(string scene)
        {
            return SceneManager.UnloadSceneAsync(scene);
        }

        public AsyncOperation LoadScene(string scene, LoadSceneMode mode = LoadSceneMode.Single, Action preload = null)
        {
            if (preload == null) { preload = () => { }; }
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, mode);
            operation.allowSceneActivation = false;
            StartCoroutine(PreloadCoroutine(operation, preload));
            return operation;
        }

        public void LoadSceneWithLoadingScene(string scene, LoadingSceneOptions options = null, Action preload = null) {
            if (options == null) { options = new LoadingSceneOptions(); }
            if (preload == null) { preload = () => { }; }
            SceneManager.LoadSceneAsync(options.LoadingSceneSceneName).completed += (AsyncOperation asyncOperation) =>
            {
                StartCoroutine(LoadSceneWithLoadingSceneCoroutine(scene, options, preload));
            };
        }

        private IEnumerator PreloadCoroutine(AsyncOperation operation, Action preload)
        {
            while (operation.progress < 0.9f) {
                yield return null;
            }
            preload();
            operation.allowSceneActivation = true;
        }

        private IEnumerator LoadSceneWithLoadingSceneCoroutine(string scene, LoadingSceneOptions options, Action preload)
        {
            GameObject loadBarGameObject = GameObject.FindGameObjectsWithTag(options.LoadingBarTag)[0];
            Image loadBar = loadBarGameObject.GetComponent<Image>();
            loadBar.fillAmount = 0;
            yield return null;
            AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(scene);
            sceneLoadOperation.allowSceneActivation = false;
            while (sceneLoadOperation.progress < 0.9f)
            {
                loadBar.fillAmount = (sceneLoadOperation.progress + 0.1f) * 0.5f;
                yield return null;
            }
            preload();
            loadBar.fillAmount = 1f;
            yield return null;
            if (options.LoadingSceneBehavior.Equals(LoadingSceneBehavior.WaitSeconds))
            {
                yield return new WaitForSeconds(options.Seconds);
            }
            else if (options.LoadingSceneBehavior.Equals(LoadingSceneBehavior.SpecificKey))
            {
                while (!sceneLoadOperation.isDone)
                {
                    if (Input.GetKeyDown(options.KeyCode)) { break; }
                    yield return null;
                }
            } 
            else if (options.LoadingSceneBehavior.Equals(LoadingSceneBehavior.AnyKey))
            {
                while (!sceneLoadOperation.isDone)
                {
                    if (Input.anyKeyDown) { break; }
                    yield return null;
                }
            }
            sceneLoadOperation.allowSceneActivation = true;
        }
    } 
}
