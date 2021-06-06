using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTank : MonoBehaviour
{
    //UIManager _uiManager;

    [SerializeField] Image _healthBar;

    [SerializeField] GameObject _canvasBar;

    [SerializeField] ArmyManager _armyManager;
    [SerializeField] float _maxHealth = 100f;
    [SerializeField] float _health;

    [SerializeField] Camera _camera;

    bool _isDead = false;

    public bool isDead { get { return _isDead; } }

    void UpdateHealthbar()
    {
        float fill = _health / _maxHealth;
        if (_healthBar != null)
        {

            _healthBar.fillAmount = fill;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;

        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _armyManager = GameObject.Find("Army Manager").GetComponent<ArmyManager>();

    }

    public void TakeDamage(float damage)
    {
        _health = Mathf.Max(_health - damage, 0);
    }

    public void Die()
    {
        if (_isDead) return;

        _isDead = true;

        this.gameObject.layer = 2;

        gameObject.tag = "Untagged";
        
        Destroy(gameObject, 2f);


    }

    // Update is called once per frame
    void Update()
    {

        _canvasBar.gameObject.transform.LookAt(_camera.transform);
        UpdateHealthbar();

        if (_health <= 0 && !_isDead)
        {
            Die();
        }

    }
}
