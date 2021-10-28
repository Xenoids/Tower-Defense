using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _maxHP = 1;
    [SerializeField] private float _moveSPD = 1f;

    [SerializeField] private SpriteRenderer _HPBar;
    [SerializeField] private SpriteRenderer _HPFill;

    private int _currHP;

    public Vector3 TargetPos {get; private set;}

    public int CurrPathIndex{get; private set;}

    // Method terpanggil 1x stiap menghidupkan
    // GO yang memiliki script ini

    private void OnEnable()
    {
        _currHP = _maxHP ;
        _HPFill.size = _HPBar.size;
    }

    public void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position,TargetPos,_moveSPD * Time.deltaTime
        );
    }

    public void SetTargetPos(Vector3 targetPos)
    {
        TargetPos = targetPos;
        _HPBar.transform.parent = null;

        // Mengubah rotasi dari Enemy

        Vector3 distance = TargetPos - transform.position;

        if(Mathf.Abs(distance.y) > Mathf.Abs(distance.x))
        {
            // menghadap atas
            if(distance.y > 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0f,0f,90f));
            }
            // menghadap bawah
            else{
                transform.rotation = Quaternion.Euler(new Vector3(0f,0f,-90f));
            }
        }
        else
        {
            // Menghadap kanan(default)
            if(distance.x > 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0f,0f,0f));
            }

            // Menhadap kiri
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0f,0f,180f));
            }
        }

        _HPBar.transform.parent = transform;

    }

    public void ReduceEnemyHP(int dmg)
    {
        _currHP -=dmg;
        AudioPlayer.Instance.PlaySFX("hit-enemy");
        if(_currHP <= 0)
        {
            _currHP = 0;
            gameObject.SetActive(false);
            AudioPlayer.Instance.PlaySFX("enemy-die");
        }

        float HP =(float) _currHP / _maxHP;

        _HPFill.size = new Vector2(HP * _HPBar.size.x, _HPBar.size.y);
    }

    // Menandai index terakhir pada path

    public void SetCurrPathIndex(int currIndex)
    {
        CurrPathIndex = currIndex;
    }
  

}
