﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Kabaj.TestPhotonMultiplayerFPSGame
{
    /// <summary>
    /// Gunzzz!
    /// <para>This definitely needs a better summary</para>
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class Gun : MonoBehaviour
    {
        #region Public Fields

        public float damage = 10f; // how much damage a gun's bullet can impart (on a target)
        public float range = 100f; // how far a bullet can go
        public float fireRate = 1f; // how fast (per second) a bullet can be fired
        public float impactForce = 30f; // force imparted on a bullet hit

        [Tooltip("Audio Clip (wav file) played when gun is fired")]
        public AudioClip gunshotSound;
        [Tooltip("Muzzle flash displayed at the end of the gun when it is fired")]
        public ParticleSystem muzzleFlash;
        [Tooltip("Visual effect displayed when a bullet hits something")]
        public GameObject impactEffect;

        // Setting these references in unity allows this class to know 
        //  - what player owns this gun -> so we don't try to fire the wrong players' guns (or every players' guns) 
        //  - what the bullet trajectory is -> so it originates from the correct player's camera and travels in the direction the player is looking/aiming
        // There could be a better way of figuring this out... this works for now though
        [Tooltip("The player who is holding the gun. **This implementation might need revision**")]
        public MonoBehaviourPun playerWhoOwnsThisGun;
        [Tooltip("Camera of the player holding the gun. " +
            "This camera is used for raytracing (determining trajectory of bullet) **This implementation might need revision**")]
        public Camera fpsCam;
        [Tooltip("Whether the program uses the implementation where the gun is responsible for shooting itself (as opposed to player shooting gun) when user input = \"Fire1\"")]
        public bool gunShootsItselfImplementation = false;

        #endregion Public Fields

        #region Private Fields

        private AudioSource audioSource;
        private float nextTimeToFire = 0f; // used to make sure we don't fire faster than fireRate allows

        #endregion Private Fields

        #region Public Properties

        public bool IsReadyToShoot
        {
            get { return Time.time >= nextTimeToFire; }
        }

        #endregion Public Properties

        #region MonoBehaviour Callbacks 

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource)
            {
                Debug.LogError("Gun is Missing Audio Source Component", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // If we're not using the implementation where the gun code is responsible for shooting itself..
            if (!gunShootsItselfImplementation)
            {
                // Do nothing
                return;
            }

            // Make sure we don't fire other players' guns  
            // an alternative might be: if(!PhotonView.Get(this).IsMine) ... or something like it
            if (!playerWhoOwnsThisGun.photonView.IsMine)
            {
                return;
            }

            // If the Fire1 (on pc, left mouse click) button was pressed AND
            // If the current time is after the next time we can fire...
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                // Calculate the next time we can fire based on current time and 
                nextTimeToFire = Time.time + 1f / fireRate;
                // Shoot the gun (Duh!)
                Shoot();
            }
        }

        #endregion MonoBehaviour Callbacks 

        #region Public Methods

        /// <summary>
        /// Called by PlayerManager every time the gun needs to be shot.
        /// Protects against shooting faster than firerate allows
        /// 
        /// </summary>
        public void Shoot()
        {
            // Make sure we can't shoot until it's time
            if (!IsReadyToShoot) { return; }

            // Calculate the next time we can fire based on current time and the firerate
            nextTimeToFire = Time.time + 1f / fireRate;

            //Play gunshot sound
            PlayGunShotSound();

            // Play the muzzle flash particle system
            muzzleFlash.Play();

            RaycastHit hit;
            // Create a raycast from fps camera position in the direction it is facing (limit raycast to 'range' distance away)
            // Get back the 'hit' value (what got hit)
            // If cast hit something...
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                // Log what we hit
                //Debug.LogFormat("Gun: Shoot() hit object: {0}", hit.transform.name);

                // We expect game objects we are concerned about possibly hitting to be marked as targets (by our design)
                // Get the target that was hit (if any)
                ITarget target = hit.transform.GetComponent<ITarget>();
                
                // If we hit a target...
                if (target != null)
                {
                    // Make target take damage 
                    target.TakeDamage(damage);
                }

                // Add force to rigid body of target 
                // If target has rigidboy...
                if (hit.rigidbody != null)
                {
                    // We can add a force either one of two possible directions:
                    // 1) the direction we are looking
                    // 2) the normal of the surface we hit
                    // We'll use the normal (make sure it's negative so it's going backwards)
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }

                // Create a new impact effect (GameObject) 
                // (Vector3) hit.point: where in space we our bullet landed
                // Quaternion.LookRotation(hit.normal): a rotation representing the normal of the surface that we hit
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                // Destroy the impact game object after 2 seconds so it doesn't clutter our game heirarchy during gameplay 
                Destroy(impactGO, 2f);
            }
        }

        #endregion Public methods

        #region Private methods

        private void PlayGunShotSound()
        {
            // I read somewhere online that this allows the sounds to overlap
            audioSource.PlayOneShot(gunshotSound);
        }
        
        #endregion Private methods

    }
}