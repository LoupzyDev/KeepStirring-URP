using System;
using UnityEngine;

public class FollowWorld : MonoBehaviour
{
    [Header("Tweaks")]
    [SerializeField] float minScaleDistance = 250f;
    [SerializeField] float maxScaleDistance = 350f;
    [SerializeField] public GameObject target;
    [SerializeField] public Vector3 offset;
    [SerializeField] public float scaleMultiplier = 5f;
    //[SerializeField] public float scaleSpeed = 10f;

    private bool isShown = false;


    private void OnEnable()
    {
        RayTriggerManager.OnTooltipEnter += ShowTooltip;
        RayTriggerManager.OnTooltipExit += HideTooltip;
    }

    private void OnDisable()
    {
        RayTriggerManager.OnTooltipEnter -= ShowTooltip;
        RayTriggerManager.OnTooltipExit -= HideTooltip;
        HideTooltip();
    }

    private void HideTooltip()
    {
        isShown = false;
        transform.position = new Vector3(-1000, -1000, -1000);
    }

    private void ShowTooltip(GameObject tooltipObject)
    {
        if (tooltipObject == target)
        {
            isShown = true;
        }
        else
        {
            HideTooltip();
        }
    }

    private void Update()
    {
        if (!isShown)
        {
            return;
        }
        // Convert the camera's world position to a screen position
        Camera cam = RayTriggerManager.cam;
        Vector3 screenPos = cam.WorldToScreenPoint(target.transform.position + offset);

        if (transform.position != screenPos)
        {
            transform.position = screenPos;
            // Scale the UI object based on the distance between the camera and the lookAt object
            // If the camera is closer to the object, the scale of the UI object will be larger
            // If the camera is further from the object, the scale of the UI object will be smaller
            float distance = Vector3.Distance(cam.transform.position, target.transform.position);
            // Clamp the distance between the min and max scale distance to get a value between 0 and 100
            distance = maxScaleDistance - Mathf.Clamp(distance, minScaleDistance, maxScaleDistance);
            distance = Mathf.Clamp(distance, (minScaleDistance / maxScaleDistance) * 100, 100);
            float scale = distance / 100 * scaleMultiplier;

            // Smoothly scale the UI object to the new scale with a speed of 10
            transform.localScale = new Vector3(scale, scale, scale);
            //transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(scale, scale, scale), scaleSpeed * Time.deltaTime);
        }
    }

}
