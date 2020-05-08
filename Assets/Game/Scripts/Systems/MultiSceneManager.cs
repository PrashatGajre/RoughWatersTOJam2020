using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiSceneManager : Singleton<MultiSceneManager>
{
    [HideInInspector] public readonly SceneLoadedEvent mOnSceneLoad = new SceneLoadedEvent();
    [HideInInspector] public readonly SceneLoadedEvent mOnSceneUnload = new SceneLoadedEvent();
    public float mDelayTime = 1.0f;
    readonly List<string> mCurrentLoadedScenes = new List<string>();

    public void LoadScene(string pSceneName)
    {
        StartCoroutine(LoadSceneWithDelay(pSceneName));
    }

    public void LoadScenes(List<string> pSceneNames)
    {
        StartCoroutine(LoadMultipleScenesWithDelay(pSceneNames));
    }

    public void UnloadScene(string pSceneName)
    {
        List<string> aTempList = new List<string>();
        aTempList.Add(pSceneName);
        StartCoroutine(UnloadMultipleScenes(aTempList));
    }

    public void UnloadScenes(List<string> pSceneNames)
    {
        StartCoroutine(UnloadMultipleScenes(pSceneNames));
    }

    IEnumerator UnloadMultipleScenes(List<string> pSceneNames)
    {
        if (MenuManager.IsValidSingleton())
        {
            MenuManager.Instance.ShowLoad();
        }
        List<string> aRemovedScenes = new List<string>();
        foreach (string aScene in pSceneNames)
        {
            if (!mCurrentLoadedScenes.Contains(aScene))
            {
                continue;
            }
            yield return StartCoroutine(AsyncSceneUnload(aScene));
            mCurrentLoadedScenes.Remove(aScene);
            aRemovedScenes.Add(aScene);
        }
        mOnSceneUnload.Invoke(aRemovedScenes);
        if (MenuManager.IsValidSingleton())
        {
            MenuManager.Instance.HideLoad();
        }
    }

    IEnumerator AsyncSceneUnload(string pSceneName)
    {
        if (!mCurrentLoadedScenes.Contains(pSceneName))
        {
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(mDelayTime);
            Application.backgroundLoadingPriority = ThreadPriority.Low;

            AsyncOperation aSync = SceneManager.UnloadSceneAsync(pSceneName);
            yield return new WaitUntil(() => aSync.isDone);

            aSync = Resources.UnloadUnusedAssets();
            yield return new WaitUntil(() => aSync.isDone);
            Application.backgroundLoadingPriority = ThreadPriority.Normal;
            yield return new WaitForSeconds(mDelayTime);
        }

    }

    IEnumerator LoadMultipleScenesWithDelay(List<string> pSceneNames)
    {
        MenuManager.Instance.ShowLoad();
        foreach (string aSceneName in pSceneNames)
        {
            yield return StartCoroutine(LoadSceneWithDelay(aSceneName, false, false));
        }
        mOnSceneLoad.Invoke(mCurrentLoadedScenes);
        MenuManager.Instance.HideLoad();
    }

    IEnumerator LoadSceneWithDelay(string pSceneName, bool pShowLoadingScreen = true, bool pRaiseEvents = true)
    {
        if(mCurrentLoadedScenes.Contains(pSceneName))
        {
            yield break;
        }
        else
        {
            if(pShowLoadingScreen)
            {
                MenuManager.Instance.ShowLoad();
            }
            yield return new WaitForSeconds(mDelayTime);

            Application.backgroundLoadingPriority = ThreadPriority.Low;

            AsyncOperation aSync = SceneManager.LoadSceneAsync(pSceneName, LoadSceneMode.Additive);
            yield return new WaitUntil(() => aSync.isDone);

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            yield return new WaitForSeconds(mDelayTime);

            mCurrentLoadedScenes.Add(pSceneName);
            if (pRaiseEvents)
            {
                mOnSceneLoad.Invoke(mCurrentLoadedScenes);
            }

            if (pShowLoadingScreen)
            {
                MenuManager.Instance.HideLoad();
            }
        }
    }
}
