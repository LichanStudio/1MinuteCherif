using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private int _damageLabelPoolSize = 20;
    [SerializeField] private GameObject _damagePrefab;
    [SerializeField] private int _percentDamageVariation = 20;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnEnable()
    {
        for (int i = 0; i < _damageLabelPoolSize; i++)
        {
            GameObject damageLabel = Instantiate(_damagePrefab, transform);
            damageLabel.SetActive(false);
        }
    }

    public void OnDamagePlayer(PlayerScript playerScript, int damage)
    {
        if (playerScript == null) return;
        float variation = 1.0f + (Random.Range(-_percentDamageVariation, _percentDamageVariation) / 100f);
        int finalDamage = (int)(damage * variation);
        playerScript.TakeDamage(finalDamage);
    }

    public void OnDamageEnemy(EnemyScript enemyScript, int damage)
    {
        if (enemyScript == null) return;
        float variation = 1.0f + (Random.Range(-_percentDamageVariation, _percentDamageVariation) / 100f);
        int finalDamage = (int)(damage * variation);
        enemyScript.TakeDamage(finalDamage, transform.GetChild(0).gameObject);
    }

    public void GetBackLabelInPool(GameObject damageLabel)
    {
        damageLabel.transform.SetParent(transform);
        damageLabel.SetActive(false);
    }
}
