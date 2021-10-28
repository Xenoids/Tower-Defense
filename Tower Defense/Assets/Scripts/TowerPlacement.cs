using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{

    private Tower _TowerTerpasang;

    // Method yang terpanggil 1x apabila obj Rb
    // yang menyentuh area collider tadi

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_TowerTerpasang != null) return;
        Tower tower = collision.GetComponent<Tower>();

        if(tower != null)
        {
            tower.SetPlacePos(transform.position);
            _TowerTerpasang = tower;
        }
    }

    // Kebalikan dari OnTriggerEnter2D, method ini terpanggil
    // apabila udah meninggalkan area collider

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(_TowerTerpasang == null) return;

        _TowerTerpasang.SetPlacePos(null);
        _TowerTerpasang = null;
    }
}
