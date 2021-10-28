using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerUI : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    // Mengubah tower data pada prefab TowerUI
    [SerializeField] private Image _towerIcon;

    private Tower _towerPrefab;
    private Tower _currSpawnedTower;

    public void SetTowerPrefab(Tower tower)
    {
        _towerPrefab = tower;
        _towerIcon.sprite = tower.GetTowerHeadIcon();
    }

    // Implementasi dari Interface IBeginDragHandler
    // Method ini terpanggil 1x ketika pertama drag UI
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject newTowerObj = Instantiate(_towerPrefab.gameObject);

        _currSpawnedTower = newTowerObj.GetComponent<Tower>();

        _currSpawnedTower.ToggleOrderInLayer(true);
    }

    // Implementasi dari interface IDragHandler
    // Method ini terpanggil selama mendrag-UI
    public void OnDrag(PointerEventData eventData)
    {
        Camera maincam = Camera.main;

        Vector3 mousePos = Input.mousePosition;

        mousePos.z = -maincam.transform.position.z;

        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);

        _currSpawnedTower.transform.position = targetPos;
    }

    // Implementasi dari interface IEndDragHandler
    // terpanggil 1x apabila mendrop UI nya

    public void OnEndDrag(PointerEventData eventData)
    {
        if(_currSpawnedTower.PlacePos == null)
        {
            Destroy(_currSpawnedTower.gameObject);
        }
        else
        {
            _currSpawnedTower.LockPlacement();
            _currSpawnedTower.ToggleOrderInLayer(false);

            LevelManager.Instance.RegisterSpawnedTower(_currSpawnedTower);
            _currSpawnedTower = null;
        }
    }
   

}
