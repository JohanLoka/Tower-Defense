using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavManager : MonoBehaviour
{
    public static NavManager Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }

    //  Is called from UI or Scripts demanding a scene-change
    public void GoToScene(string sceneName)
    {
        StartCoroutine(_goToScene(sceneName));
    }

    //  Is called from UI or Scripts demanding a scene-change
    public void EnterMap(string mapName)
    {
        //  If we supply a empty string, we use the saved one in MapManager
        string mapToLoad = mapName.Length > 0 ? mapName : MapManager.Instance.SelectedMap;

        //  Make sure scene is correct
        bool validates = MapManager.Instance.IsMapCorrect(mapToLoad);

        if (validates)
        {
            MapManager.Instance.SetMap(mapToLoad);
            StartCoroutine(_goToScene("SampleScene"));
        }
    }

    //  We use a small delay here to make sure we clear old scene first.
    private IEnumerator _goToScene(string sceneName, float delay = 1)
    {

        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(sceneName);
    }
}
