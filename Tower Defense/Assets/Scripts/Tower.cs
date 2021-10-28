using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    
    // Komponen Tower
    [SerializeField] private SpriteRenderer _towerPlace;
    [SerializeField] private SpriteRenderer _towerHead;

    // Properties Tower
    [SerializeField] private int _shootpow = 1;
    [SerializeField] private float _shootdis = 1f;

    [SerializeField] private float _shootdelay = 5f;

    [SerializeField] private float _bulletspd = 1f;

    [SerializeField] private float _bulletSplashRad = 0f;

    [SerializeField] private Bullet _bulletPrefab;

    private float _runShootDelay;

    private Enemy _targetEnemy;

    private Quaternion _targetRotation;

    // Mengecek musuh terdekat

    public void CheckNearestEnemy(List<Enemy> enemies)
    {
        if(_targetEnemy != null)
        {
            if(!_targetEnemy.gameObject.activeSelf || Vector3.Distance(transform.position,_targetEnemy.transform.position) > _shootdis)
            {
                _targetEnemy = null;
            }
            else{
                return;
            }
        }

        float nearestDis = Mathf.Infinity;

        Enemy nearestEnemy = null;

        foreach(Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position,enemy.transform.position);
            if(distance > _shootdis){
                continue;
            }

            if(distance < nearestDis)
            {
                nearestDis = distance;
                nearestEnemy = enemy;
            }
        }

        _targetEnemy = nearestEnemy;
    }

    // Menembak musuh yg telah disimpan sbg target
    public void ShootTarget()
    {
        if(_targetEnemy == null) return;

        _runShootDelay -=Time.unscaledDeltaTime;
        if(_runShootDelay <= 0f)
        {
            bool headHasAimed = Mathf.Abs(_towerHead.transform.rotation.eulerAngles.z - _targetRotation.eulerAngles.z) <10f;
            if(!headHasAimed)
            {
                return;
            }

            Bullet bullet =
            LevelManager.Instance.GetBulletFromPool(_bulletPrefab);

            bullet.transform.position = transform.position;
            bullet.SetProperties(_shootpow,_bulletspd,_bulletSplashRad);

            bullet.setTargetEnemy(_targetEnemy);

            bullet.gameObject.SetActive(true);

            _runShootDelay = _shootdelay;
            
        }
    }

    public void SeekTarget()
    {
        if(_targetEnemy == null)
        {
            return;
        }

        Vector3 direction =
        _targetEnemy.transform.position - transform.position;

        float targetAngle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;

        _targetRotation = Quaternion.Euler(new Vector3(0f,0f,targetAngle - 90f));

        _towerHead.transform.rotation = Quaternion.RotateTowards(_towerHead.transform.rotation,_targetRotation,Time.deltaTime * 180f);
    }


    // Digunakan untuk menyimpan posisi yang akan ditempati
    // selama tower di drag
    public Vector2? PlacePos{get; private set;}

    public void SetPlacePos(Vector2? newPos)
    {
        PlacePos = newPos;
    }

    public void LockPlacement()
    {
        transform.position = (Vector2) PlacePos;
    }

    // Mengubah order in layer pada tower yang sedang didrag

    public void ToggleOrderInLayer(bool toFront)
    {
        int orderInLayer = toFront ? 2 : 0;
        _towerPlace.sortingOrder = orderInLayer;
        _towerHead.sortingOrder = orderInLayer;
    }

    // method untuk mengambil sprite pd tower _towerHead
    public Sprite GetTowerHeadIcon()
    {
        return _towerHead.sprite;
    }

}
