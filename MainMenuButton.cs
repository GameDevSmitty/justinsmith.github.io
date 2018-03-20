using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class MainMenuButton : MonoBehaviour, IInputClickHandler
{
    [SerializeField] GameObject pauseMenuGameObject;
    [SerializeField] private int sceneToLoadBuildIndex;
    [SerializeField] private RaycastForGaze raycastForGaze;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip selectedClip;
    private TextMesh textMesh;
    private GameObject loadingScreen;
    private bool isTargeted;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        textMesh = GetComponent<TextMesh>();
    }

    private void Update()
    {

        if (raycastForGaze.Hit.transform == transform)
        {
            if (!isTargeted)
            {

                audioSource.clip = hoverClip;
                audioSource.Play();
                isTargeted = true;
                textMesh.color = Color.green;
            }
        }
        else
        {
            if (isTargeted)
            {
                isTargeted = false;
                textMesh.color = Color.white;
            }
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        StartCoroutine(WaitForSelectSoundToPlay());
    }

    IEnumerator WaitForSelectSoundToPlay()
    {
        audioSource.clip = selectedClip;
        audioSource.Play();

        yield return new WaitForSeconds(selectedClip.length);

        pauseMenuGameObject.SetActive(false);

        loadingScreen = LoadingScreen.instance.LoadingScreenPlane;
        loadingScreen.SetActive(true);
        LoadingScreen.instance.CallCoroutine(sceneToLoadBuildIndex);
    }
}
