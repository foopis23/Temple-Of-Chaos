﻿using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    public class WeaponController : MonoBehaviour
    {
        public Animator shootAnimator;
        public Animator reloadAnimator;
        public Animator equipAnimator;
        
        public Transform bulletSpawnPoint;
        public GameObject weaponObject;
        private IWeapon _weapon;

        public void Start()
        {
            _weapon = weaponObject.GetComponent<IWeapon>();
            reloadAnimator.gameObject.SetActive(false);
            shootAnimator.gameObject.SetActive(true);
            equipAnimator.gameObject.SetActive(false);
        }

        public void Update()
        {
            if (_weapon == null)
            {
                return;
            }
            
            if (Input.GetButtonDown("Fire1") && _weapon.Fire(bulletSpawnPoint))
            {
                reloadAnimator.gameObject.SetActive(false);
                shootAnimator.gameObject.SetActive(true);
                equipAnimator.gameObject.SetActive(false);
                shootAnimator.Play("shoot");
            }

            if (Input.GetButtonDown("Reload") && _weapon.Reload())
            {
                reloadAnimator.gameObject.SetActive(true);
                shootAnimator.gameObject.SetActive(false);
                equipAnimator.gameObject.SetActive(false);
                reloadAnimator.Play("reload");
            }
        }
    }
}