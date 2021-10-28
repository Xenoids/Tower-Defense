using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // Method Singleton
    // akan ada satu selama game dimainkan dan dapat diakses
    // melalui vairable Instancenya.
    // fungsi -> memunculkan setiap tower UI ke panel selection

    private static LevelManager _instance = null;
    public static LevelManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
            }
            return _instance;
        }
    }

    [SerializeField] private int _maxLives = 3;
    [SerializeField] private int _totalEnemy = 15;

    [SerializeField] private GameObject _panel;
    [SerializeField] private Text _statusInfo;

    [SerializeField] private Text _livesInfo;

    [SerializeField] private Text _totalEnemyInfo;

    [SerializeField] private Transform _towerUIParent;
    [SerializeField] private GameObject _towerUIPrefab;

    [SerializeField] private Tower[] _towerPrefabs;

    [SerializeField] private Enemy[] _enemyPrefabs;

    [SerializeField] private Transform[] _enemyPaths;
    [SerializeField] private float _spawnDelay = 5f;

    private List<Tower> _spawnedTowers = new List<Tower>();

    private List<Enemy> _spawnedEnemies = new List<Enemy>();

    private List<Bullet> _spawnedBullets = new List<Bullet>();

    private int _currLives;

    private int _enemyCounter;

    private float _runSpawnDelay;

    public bool IsOver {get; private set;}

    // Start is called before the first frame update
    private void Start()
    {
        SetCurrLives(_maxLives);
        SetTotalEnemy(_totalEnemy);
        showAllTowerUI();
    }

    // menampilkan semua tower yang tersedia pada ui selection

    private void showAllTowerUI()
    {
        foreach(Tower tower in _towerPrefabs)
        {
            GameObject newTowerUIObj = Instantiate(_towerUIPrefab.gameObject, _towerUIParent);

            TowerUI newTowerUI = newTowerUIObj.GetComponent<TowerUI>();

            newTowerUI.SetTowerPrefab(tower);
            newTowerUI.transform.name = tower.name;
        }
    }

    private void Update()
    {
        // Counter utk spawn enemy dalam jeda waktu yang udah ditentukan

        // Time.unscaledDeltaTime -> delta time independent,
        // tidak terpengaruh sama apappun kecuali GO nya sendiri,
        // jadi dapat digunakan sbg penghitung waktu

        _runSpawnDelay -= Time.unscaledDeltaTime;

        if(_runSpawnDelay <= 0f)
        {
             SpawnEnemy();
            _runSpawnDelay = _spawnDelay;
        }

        foreach(Tower tower in _spawnedTowers)
        {
            tower.CheckNearestEnemy(_spawnedEnemies);
            tower.SeekTarget();
            tower.ShootTarget();
        }

        foreach(Enemy enemy in _spawnedEnemies)
        {
            if(!enemy.gameObject.activeSelf)
            {
                continue;
            }

             // nilainya 0.1 karena utk mentoleransi perbedaan posisi
        // akan terlalu sulit kalo beda posisinya sama persis / 0

        if(Vector2.Distance (enemy.transform.position,enemy.TargetPos) < 0.1f)
        {
            enemy.SetCurrPathIndex(enemy.CurrPathIndex+1);
            if(enemy.CurrPathIndex < _enemyPaths.Length)
            {
                enemy.SetTargetPos(_enemyPaths[enemy.CurrPathIndex].position);
            }
            else{
                ReducesLives(1);
                enemy.gameObject.SetActive(false);
            }
        }
        else{
            enemy.MoveToTarget();
        }

        } 

        // Jika menekan tombol R, method restart akan dipanggil

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }   

        if(IsOver)
        {
            return;
        }

    }

    // Mendaftarkan Tower yang dispawn agar bisa dikontrol oleh
    // LevelManager
    public void RegisterSpawnedTower(Tower tower)
    {
        _spawnedTowers.Add(tower);
    }

    private void SpawnEnemy()
    {

        SetTotalEnemy(--_enemyCounter);
        if(_enemyCounter <0)
        {
            bool isAllEnemyDestoryed =
            _spawnedEnemies.Find(e=>e.gameObject.activeSelf) == null;

            if(isAllEnemyDestoryed)
            {
                SetGameOver(true);
            }

            return;
        }

     
        int randIndex = Random.Range(0,_enemyPrefabs.Length);

        string enemyIndexString = (randIndex + 1).ToString();

        GameObject newEnemyObj = _spawnedEnemies.Find(e=> !e.gameObject.activeSelf && e.name.Contains(enemyIndexString))?.gameObject;
        
        if(newEnemyObj == null)
        {
            newEnemyObj = Instantiate(_enemyPrefabs[randIndex].gameObject);
        }

        // mengespawn enemy
        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();
        if(!_spawnedEnemies.Contains(newEnemy))
        {
            _spawnedEnemies.Add(newEnemy);
        }

        newEnemy.transform.position = _enemyPaths[0].position;

        newEnemy.SetTargetPos(_enemyPaths[1].position);

        newEnemy.SetCurrPathIndex(1);

        newEnemy.gameObject.SetActive(true);
    }

        public Bullet GetBulletFromPool(Bullet prefab)
        {
            GameObject newBulletObj = _spawnedBullets.Find(b=>!b.gameObject.activeSelf && b.name.Contains(prefab.name))?.gameObject;

            if(newBulletObj == null)
            {
                newBulletObj = Instantiate(prefab.gameObject);
            }

            Bullet newBullet = newBulletObj.GetComponent<Bullet>();
            if(!_spawnedBullets.Contains(newBullet))
            {
                _spawnedBullets.Add(newBullet);
            }

            return newBullet;
        }

        public void ExplodeAt(Vector2 point, float rad, int dmg)
        {
            foreach(Enemy enemy in _spawnedEnemies)
            {
                if(enemy.gameObject.activeSelf)
                {
                    if(Vector2.Distance(enemy.transform.position,point) <= rad)
                    {
                        enemy.ReduceEnemyHP(dmg);
                    }
                }
            }
        }

        public void ReducesLives(int value)
        {
            SetCurrLives(_currLives - value);
            if(_currLives <= 0)
            {
                SetGameOver(false);
            }
        }

        public void SetCurrLives(int currLives)
        {
            // Mathf.max berfungsi untuk ambil angka terbsr
            // sehingga _currLives disini tdk akan lebih kecil dri 0
            _currLives = Mathf.Max(currLives,0);
            _livesInfo.text =$"Lives : {_currLives}";
        }

        public void SetTotalEnemy(int total)
        {
            _enemyCounter = total;
            _totalEnemyInfo.text =$"Total Enemy : {Mathf.Max(_enemyCounter,0)}";
        }

        public void SetGameOver(bool isWin)
        {
            IsOver = true;

            _statusInfo.text = isWin? "You Win" : "You Lose!";
            _panel.gameObject.SetActive(true);
        }

    
        // Utk menampilkan garis penghubung dalam window Scene

        // tanpa harus diplay terlebih dahulu
        private void OnDrawGizmos()
        {
            for(int i=0;i<_enemyPaths.Length -1; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(_enemyPaths[i].position,_enemyPaths[i+1].position);
            }
        }

}
