using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private int _bulletpow;
    private float _bulletspd;

    private float _bulletsplashrad;

    private Enemy _targetEnemy;

    // fixed update -> lebih konsisten jeda pemanggilannya
    // cocok digunakan apabila karakter punya physic 
    // (RigidBody, dll)

    private void FixedUpdate()
    {

        if(LevelManager.Instance.IsOver)
        {
            return;
        }
        
        if(_targetEnemy != null)
        {
            if(!_targetEnemy.gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                _targetEnemy = null;
                return;
            }
        

        transform.position = Vector3.MoveTowards(transform.position, _targetEnemy.transform.position,_bulletspd * Time.fixedDeltaTime);

        Vector3 direction =
        _targetEnemy.transform.position - transform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0f,0f,targetAngle - 90f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_targetEnemy == null)
        {
            return;
        }
        if(collision.gameObject.Equals(_targetEnemy.gameObject))
        {
            gameObject.SetActive(false);

            // Bullet yang memiliki efek splash area
            if(_bulletsplashrad > 0f)
            {
                LevelManager.Instance.ExplodeAt(transform.position,_bulletsplashrad, _bulletpow);
            }

            // bullet yg single target
            else
            {
                _targetEnemy.ReduceEnemyHP(_bulletpow);
            }

            _targetEnemy = null;
        }
    }

    public void SetProperties(int bulletpow, float bulletspd, float bulletsplashrad)
    {
        _bulletpow = bulletpow;
        _bulletspd = bulletspd;
        _bulletsplashrad = bulletsplashrad;
    }

    public void setTargetEnemy(Enemy enemy)
    {
        _targetEnemy = enemy;
    }
   
}
