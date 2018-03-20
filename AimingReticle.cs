using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingReticle : MonoBehaviour
{
    [SerializeField] private Material targetReticle_mat;
    [SerializeField] private float minimumScale;
    [SerializeField] private float maximumScale;
    [SerializeField] private Transform reticleBasePosition;
    private float lerpScale;
    private float distanceToTarget;
    private Color targetReticleColorGood = Color.green;
    private Color targetReticleColorBad = Color.red;
    private Color colorBeingAimedAt;
    private float lerpTime = .2f;
    private float currentLerpTime;
    private bool hasTargetHolder;
    private bool hasTarget;
    public bool HasTarget { get { return hasTarget; } set { hasTarget = value; } }
    private bool isLerping;
    
	// Use this for initialization
	void Start ()
    {
        isLerping = false;
        hasTarget = false;
        colorBeingAimedAt = targetReticleColorGood;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(hasTarget)
        {
            if (colorBeingAimedAt == targetReticleColorBad)
            {
                StopAllCoroutines();
                isLerping = false;
            }

            if (!isLerping)
            {
                StartCoroutine(LerpAimingReticleColor(targetReticleColorGood));
            }
        }
        else
        {
            transform.position = reticleBasePosition.position;
            transform.localScale = reticleBasePosition.localScale;

            if(colorBeingAimedAt == targetReticleColorGood)
            {
                StopAllCoroutines();
                isLerping = false;
            }

            if (!isLerping)
            {
                StartCoroutine(LerpAimingReticleColor(targetReticleColorBad));
            }
            
        }
	}

    public void ChangeReticlePositionAndScale(Vector3 hitPoint)
    {
        transform.position = hitPoint;
        distanceToTarget = (hitPoint - Camera.main.transform.position).magnitude;

        lerpScale = Mathf.Lerp(minimumScale, maximumScale, distanceToTarget / 100f);
        transform.localScale = new Vector3(lerpScale, lerpScale, lerpScale);
    }

    IEnumerator LerpAimingReticleColor(Color colorToLerpTo)
    {
        currentLerpTime = 0f;
        colorBeingAimedAt = colorToLerpTo;
        isLerping = true;

        while(targetReticle_mat.color != colorToLerpTo)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            float t = currentLerpTime / lerpTime;
            t = t * t * t * (t * (6f * t - 15f) + 10f);

            targetReticle_mat.color = Color.Lerp(targetReticle_mat.color, colorToLerpTo, t);
            yield return null;
        }

        isLerping = false;
    }
}
