using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher Instance;
    public GameObject leavePrefab;
    public GameObject enterPrefab;
    public Type loadType;
    public string sceneName;
    public bool waitToEnter;

    public float progress { get; private set; }

    [System.Serializable]
    public enum Type
    {
        LoadScene,
        LoadSceneAsync
    }

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        var leaveAnimation = leavePrefab.GetComponent<Animation>();
        Instantiate(leavePrefab, transform);

        Invoke("LoadScene", leaveAnimation.clip.length);
    }

    /// <summary>
    /// Ready to active next scene.
    /// </summary>
    public void ReadyToEnter()
    {
        waitToEnter = false;
    }

    private void LoadScene()
    {
        if (!waitToEnter)
        {
            AddEnterPrefab();
        }

        if (loadType == Type.LoadSceneAsync)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void AddEnterPrefab()
    {
        var enterAnimation = enterPrefab.GetComponent<Animation>();
        var enterClone = Instantiate(enterPrefab);
        DontDestroyOnLoad(enterClone);
        Destroy(enterClone, enterAnimation.clip.length);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            progress = asyncOperation.progress;

            if (progress >= 0.9f)
            {
                if (!waitToEnter && asyncOperation.allowSceneActivation == false)
                {
                    AddEnterPrefab();
                    asyncOperation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
