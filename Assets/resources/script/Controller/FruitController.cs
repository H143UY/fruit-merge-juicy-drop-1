using Core.Pool;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    private Rigidbody2D rg;
    public bool FruitNew;
    private CircleCollider2D circle;
    public int level { get; private set; }
    public bool isIdle;
    public bool isUpgraded;
    private Animator animator;
    private bool CheckGameOver;
    private bool canbounce;
    public bool isMerging;
    public int uniqueID;
    private void Awake()
    {
        isUpgraded = false;
        canbounce = !isUpgraded;
        if (!FruitNew)
        {
            FruitNew = true;
        }
        uniqueID = GenerateUniqueID();
    }
    private void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circle = GetComponent<CircleCollider2D>();
        circle.enabled = false;
        isIdle = false;
        CheckGameOver = false;
        isMerging = false;
    }
    private void Update()
    {
        if (FruitNew && !isUpgraded)
        {
            this.transform.position = FruitManager.instance.PosSpawnFruit.position;
        }
        if (!FruitNew && rg.velocity.magnitude < 0.1f)
        {
            CheckGameOver = true;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            this.PostEvent(EventID.GameOver);
        }
        AnimatorFruit();

        if (Input.GetMouseButtonUp(0) && FruitNew)
        {
            Release();
        }

        if (!isIdle)
        {
            float TimeIdle = 0;
            TimeIdle += Time.deltaTime;
            if (TimeIdle >= 10)
            {
                isIdle = true;
            }
        }
        if (isUpgraded || !FruitNew)
        {
            circle.enabled = true;
        }
    }
    public void Release()
    {
        FruitNew = false;
        if (circle != null)
        {
            circle.enabled = true;
        }
        FruitManager.instance.AddFruitsToList(this);
    }

    public void SetLevel(int newLevel, bool isNew, bool upgraded = false)
    {
        level = newLevel;
        FruitNew = isNew;
        isUpgraded = upgraded;
        isMerging = false;
        if (isNew || upgraded)
        {
            uniqueID = GenerateUniqueID();
        }
    }
    private int GenerateUniqueID()
    {
        return Guid.NewGuid().GetHashCode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("san") || collision.gameObject.CompareTag("Fruit"))
        {
            if (canbounce)
            {
                transform.DOMoveY(transform.position.y + 0.5f, 0.2f).SetEase(Ease.OutQuad);
                canbounce = false;
            }
        }

        if (collision.gameObject.CompareTag("Fruit"))
        {
            FruitController otherFruit = collision.gameObject.GetComponent<FruitController>();

            if (otherFruit != null && otherFruit.level == this.level && !isMerging && !otherFruit.isMerging)
            {
                isMerging = true;
                otherFruit.isMerging = true;
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    int upLevel = level + 1;
                    if (upLevel < FruitManager.instance.fruits.Length)
                    {
                        Vector3 spawnPos = (transform.position + otherFruit.transform.position) / 2;
                        this.transform.DOMove(spawnPos, 0.5f);
                        otherFruit.transform.DOMove(spawnPos, 0.5f).OnComplete(() =>
                        {
                            FruitManager.instance.RemoveFruits(otherFruit);
                            FruitManager.instance.RemoveFruits(this);
                            SmartPool.Instance.Despawn(otherFruit.gameObject);
                            SmartPool.Instance.Despawn(this.gameObject);
                            FruitManager.instance.UpLevelFruit(upLevel, spawnPos);
                        });
                    }
                    else
                    {
                        SmartPool.Instance.Despawn(otherFruit.gameObject);
                        SmartPool.Instance.Despawn(this.gameObject);
                    }
                });
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CheckGameOver && collision.gameObject.tag == "Dead")
        {
            this.PostEvent(EventID.GameOver);
        }
    }
    private void AnimatorFruit()
    {
        animator.SetBool("isIdle", isIdle);
    }
    private void SetIdle()
    {
        isIdle = false;
    }
    public void SetBounce()
    {
        canbounce = false;
    }
}
