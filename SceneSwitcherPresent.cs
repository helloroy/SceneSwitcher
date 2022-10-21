using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneSwitcherPresent", menuName = "ScriptableObjects/SceneSwitcherPresent", order = 1)]
public class SceneSwitcherPresent : ScriptableObject
{
    [Tooltip("Prefab to add to scene when call from LoadScene or LoadSceneAsync by SceneSwitcherPresent.")]
    public GameObject leavePrefab;
    [Tooltip("Prefab to add to scene when next scene is ready.")]
    public GameObject enterPrefab;

    [Tooltip("Set ture if you want to active next scene by SceneSwitcher.Instance.ReadyToEnter().")]
    public bool waitToEnter;

    public void LoadScene(string sceneName)
    {
        CreateSceneSwitcher(sceneName, SceneSwitcher.Type.LoadScene);
    }

    public void LoadSceneAsync(string sceneName)
    {
        CreateSceneSwitcher(sceneName, SceneSwitcher.Type.LoadSceneAsync);
    }

    void CreateSceneSwitcher(string sceneName, SceneSwitcher.Type loadType)
    {
        var sceneSwitcher = new GameObject(typeof(SceneSwitcher).Name).AddComponent<SceneSwitcher>();
        sceneSwitcher.leavePrefab = leavePrefab;
        sceneSwitcher.enterPrefab = enterPrefab;
        sceneSwitcher.waitToEnter = waitToEnter;
        sceneSwitcher.loadType = loadType;
        sceneSwitcher.sceneName = sceneName;

#if UNITY_EDITOR 
        if (loadType == SceneSwitcher.Type.LoadScene && waitToEnter == true)
        {
            Debug.Log($"waitToEnter is work with LoadSceneAsync only.", this);
        }
#endif

    }

    /// <summary>
    /// Ready to active next scene.
    /// </summary>
    public void ReadyToEnter()
    {
        SceneSwitcher.Instance.ReadyToEnter();
    }

    private void OnValidate()
    {
        CheckAnimation(leavePrefab);
        CheckAnimation(enterPrefab);
    }

    private void CheckAnimation(GameObject checkObject)
    {
        if (!checkObject.TryGetComponent<Animation>(out var anim))
        {
            Debug.LogWarning($"No Animation component found on {checkObject.name}", checkObject);
            return;
        }

        if (!anim.clip)
        {
            Debug.LogWarning($"No Animation Clip found on {checkObject.name}", checkObject);
            return;
        }

        if (!anim.clip.legacy)
        {
            anim.clip.legacy = true;
            Debug.Log($"Animation Clip {anim.clip.name} changed to legacy", anim.clip);
            return;
        }
    }

}
