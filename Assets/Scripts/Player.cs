﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement; // ok all the time this is the only game object that loads scenes

public class Player : MonoBehaviour {

    [Tooltip("In ms^-1")][SerializeField] float speed = 20f;
    [Tooltip("In m")] [SerializeField] float xRange = 5f;
    [Tooltip("In m")] [SerializeField] float yRange = 3f;

    [SerializeField] float positionPitchFactor = -5f;
    [SerializeField] float controlPitchFactor = -20f;
    [SerializeField] float positionYawFactor = 5f;
    [SerializeField] float controlRollFactor = -20f;

    [SerializeField] GameObject deathEffect;
    [SerializeField] List<GameObject> guns;
   
    float xThrow, yThrow;
    bool isDying = false;

    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(DeathSequence());
    }
	
    private IEnumerator DeathSequence()
    {
        if (isDying) { yield return null; }
        isDying = true;
        StartCoroutine(SubDeathSequence());
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }

    private IEnumerator SubDeathSequence()
    {
        deathEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject gun in guns)
        {
            gun.SetActive(false);
        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDying)
        {
            ProcessTranslation();
            ProcessRotation();
        }
    }

    private void ProcessRotation()
    {
        float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
        float pitchDueToControlThrow = yThrow * controlPitchFactor;
        float pitch = pitchDueToPosition + pitchDueToControlThrow;

        float yaw = transform.localPosition.x * positionYawFactor;

        float roll = xThrow * controlRollFactor;

        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
    }

    private void ProcessTranslation()
    {
        xThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        yThrow = CrossPlatformInputManager.GetAxis("Vertical");

        float xOffset = xThrow * speed * Time.deltaTime;
        float yOffset = yThrow * speed * Time.deltaTime;

        float rawXPos = transform.localPosition.x + xOffset;
        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

        float rawYPos = transform.localPosition.y + yOffset;
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange);

        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }
}
