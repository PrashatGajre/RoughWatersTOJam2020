using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
	public bool loadScene = true;
	public SceneReference startupScene;

	private void Start()
	{
		// Set system parameters

		Scene scene = SceneManager.GetSceneByPath(startupScene);
		if (scene.isLoaded == false && loadScene == true)
		{
			StartCoroutine(bootSequence());
		}
		else if (scene.buildIndex == -1)
		{
			Debug.LogError("Scene not found in build list: " + startupScene);
		}
		else
		{
			StartCoroutine(ignoreBootSequence());
		}
	}

	IEnumerator ignoreBootSequence()
	{
		yield return new WaitForSeconds(1);
		SceneLoadedCallback(null);
	}

	IEnumerator bootSequence()
	{
		yield return new WaitForSeconds(1);
		MultiSceneManager.Instance.mOnSceneLoad.AddListener(SceneLoadedCallback);
		MultiSceneManager.Instance.LoadScene(startupScene);
	}

	void SceneLoadedCallback(List<string> scenesLoaded)
	{
		MultiSceneManager.Instance.mOnSceneLoad.RemoveListener(SceneLoadedCallback);
		MenuManager.Instance.HideLoad();
		MenuManager.Instance.ShowMenu(NetworkManager.Instance.mStartMenu);
		AudioManager.Instance.PlayMusic("Music", true, 0.2f);
	}
}
