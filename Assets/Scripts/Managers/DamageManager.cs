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
        ActionsManager.OnDamageEntity += OnDamageEntity;
    }

    public void OnDisable()
    {
        ActionsManager.OnDamageEntity -= OnDamageEntity;
    }

    public void OnDamageEntity(EntityScript entityScript, int damage)
    {
        entityScript.TakeDamage(CalculateDamage(damage), transform.GetChild(0).gameObject);
    }

    public int CalculateDamage(int baseDmg)
    {
        float variation = 1.0f + (Random.Range(-_percentDamageVariation, _percentDamageVariation) / 100f);
        int finalDamage = (int)(baseDmg * variation);
        return finalDamage;
    }

    public void GetBackLabelInPool(GameObject damageLabel)
    {
        damageLabel.transform.SetParent(transform);
        damageLabel.SetActive(false);
    }
}
