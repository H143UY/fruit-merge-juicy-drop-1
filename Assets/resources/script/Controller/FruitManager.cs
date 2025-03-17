using Core.Pool;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
public class FruitManager : MonoBehaviour
{
    public static FruitManager instance;
    public GameObject[] fruits;
    public Dictionary<GameObject, int> fruitLevels = new Dictionary<GameObject, int>();
    public Transform PosSpawnFruit;
    private List<FruitController> fallenFruits = new List<FruitController>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        SpawnFruit(0, PosSpawnFruit.position, true);
        LoadFallenFruitsData();
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(InstantiateFruit());
        }
    }
    private IEnumerator InstantiateFruit()
    {
        yield return new WaitForSeconds(0.5f);
        int randomIndex = Random.Range(0, 4);
        SpawnFruit(randomIndex, PosSpawnFruit.position, true);
    }
    private void SpawnFruit(int index, Vector3 position, bool isNew)
    {
        var fruit = SmartPool.Instance.Spawn(fruits[index], position, transform.rotation);
        FruitController fruitController = fruit.GetComponent<FruitController>();

        if (fruitController != null)
        {
            fruitController.SetLevel(index, isNew);
        }
    }
    public void UpLevelFruit(int level, Vector3 upPosition)
    {
        if (level >= fruits.Length) return;

        var newFruit = SmartPool.Instance.Spawn(fruits[level], upPosition, transform.rotation);
        newFruit.transform.localScale = Vector3.zero;
        newFruit.transform.DOScale(Vector3.one, 1f);
        FruitController fruitController = newFruit.GetComponent<FruitController>();
        fruitController.SetLevel(level, false, true);
        AddFruitsToList(fruitController);
    }
    public void AddFruitsToList(FruitController fruit)
    {
        if (!fallenFruits.Contains(fruit))
        {
            fallenFruits.Add(fruit);
        }
    }
    public void RemoveFruits(FruitController fruit)
    {
        if (fallenFruits.Contains(fruit))
        {
            fallenFruits.Remove(fruit);
            Debug.Log($"remove {fruit.name}");
        }
    }
    public void SaveFallenFruitsData()
    {
        PlayerPrefs.SetInt("FallenFruitCount", fallenFruits.Count);

        for (int i = 0; i < fallenFruits.Count; i++)
        {
            Vector3 pos = fallenFruits[i].transform.position;
            int fruitID = fallenFruits[i].level;
            FruitController fruit = fallenFruits[i].GetComponent<FruitController>();
            int uniqueID = fruit.uniqueID;
            PlayerPrefs.SetFloat($"FallenFruit_{i}_x", pos.x);
            PlayerPrefs.SetFloat($"FallenFruit_{i}_y", pos.y);
            PlayerPrefs.SetInt($"FallenFruit_{i}_ID", fruitID);
            PlayerPrefs.SetInt($"FallenFruit_{i}_UniqueID", uniqueID);
        }
        PlayerPrefs.Save();
    }
    public void LoadFallenFruitsData()
    {
        int count = PlayerPrefs.GetInt("FallenFruitCount", 0);
        fallenFruits.Clear();

        for (int i = 0; i < count; i++)
        {
            float x = PlayerPrefs.GetFloat($"FallenFruit_{i}_x");
            float y = PlayerPrefs.GetFloat($"FallenFruit_{i}_y");
            int fruitID = PlayerPrefs.GetInt($"FallenFruit_{i}_ID");
            int uniqueID = PlayerPrefs.GetInt($"FallenFruit_{i}_UniqueID");
            var fruit = SmartPool.Instance.Spawn(fruits[fruitID], new Vector3(x, y), transform.rotation);
            FruitController fruitController = fruit.GetComponent<FruitController>();

            if (fruitController != null)
            {
                fruitController.SetLevel(fruitID, false);
                fruitController.SetBounce();
                fruitController.uniqueID = uniqueID;
                fallenFruits.Add(fruitController);
                Debug.Log($"Fruit {i} - Unique ID: {uniqueID}");
            }
        }
    }
}
