using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletNew : MonoBehaviour, ISubscriber
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip bulletFiredClip;
    [SerializeField] private Transform tendrils;

    private bool hasHitFood;
    public bool HasHitFood { get { return hasHitFood; } set { hasHitFood = value; } }
    private bool hasHitBoundary;
    private AudioSource audioSource;
    private RaycastHit hit;
    private int boundaryLayerMask = 13;

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    void Start()
    {
        boundaryLayerMask = 1 << boundaryLayerMask;
        hasHitFood = false;
        audioSource = transform.GetComponent<AudioSource>();
        audioSource.clip = bulletFiredClip;
        audioSource.Play();

        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;

        StartCoroutine(MoveToEndThenDestroy());
    }

    IEnumerator MoveToEndThenDestroy()
    {
        Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, boundaryLayerMask);

        Debug.Log(hit.point);

        while (transform.position != hit.point)
        {
            transform.Rotate(new Vector3(0f, 0f, 15f) * moveSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, hit.point, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    public void HitSomething(GameObject bulletToDestroy, bool b)
    {
        if (bulletToDestroy == this.gameObject)
        {
            Destroy(gameObject);
        }
    }

    public void Subscribe()
    {
        EventManager.OnBulletHit += HitSomething;
    }

    public void Unsubscribe()
    {
        EventManager.OnBulletHit -= HitSomething;
    }
}
