using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    public GameObject LoadingScreenPlane { get { return loadingScreen; } }
    [SerializeField] float loadingScreenDelay;
    [SerializeField] GameObject gardenerSnakePrefab;
    [SerializeField] Transform snakeSpawnTransform;
    GardenerSnake gardenerSnake;

    public static LoadingScreen instance = null;
    WaitForSeconds delay;
    AsyncOperation sceneLoading;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        delay = new WaitForSeconds(loadingScreenDelay);
    }

    public void CallCoroutine(int sceneToLoadBuildIndex)
    {
        StartCoroutine(WaitForSceneToLoad(sceneToLoadBuildIndex));
    }

    IEnumerator WaitForSceneToLoad(int sceneToLoadBuildIndex)
    {
        Camera holoLensCamera = GameObject.Find("HoloLensCamera").GetComponent<Camera>();
        Camera loadingScreenCamera = GameObject.Find("SecondCamera").GetComponent<Camera>();
        GameObject mainMenuGameObject;

        if(GameObject.Find("NewMainMenu") != null)
        {
            mainMenuGameObject = GameObject.Find("NewMainMenu");
            mainMenuGameObject.SetActive(false);
        }
        
        loadingScreen.SetActive(true);
        loadingScreenCamera.enabled = true;
        holoLensCamera.enabled = false;

        gardenerSnake = Instantiate(gardenerSnakePrefab, snakeSpawnTransform.position, Quaternion.identity).GetComponent<GardenerSnake>();
        
        yield return delay;

        sceneLoading = SceneManager.LoadSceneAsync(sceneToLoadBuildIndex);

        while(!sceneLoading.isDone)
        {
            yield return null;
        }

        gardenerSnake.DestroySnake();

        holoLensCamera = GameObject.Find("HoloLensCamera").GetComponent<Camera>();

        loadingScreenCamera.enabled = false;
        holoLensCamera.enabled = true;
        loadingScreen.SetActive(false);
    }
}
