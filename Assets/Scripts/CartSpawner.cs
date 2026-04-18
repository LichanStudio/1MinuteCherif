using UnityEngine;

public class CartSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject CartPrefab;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private SaloonScript _saloon;

    public void OnEnable()
    {
        ActionsManager.OnEndSession += HandleEndSession;
        ActionsManager.OnSpawnCart += SpawnCart;
    }

    public void OnDisable()
    {
        ActionsManager.OnEndSession -= HandleEndSession;
        ActionsManager.OnSpawnCart -= SpawnCart;
    }

    public void HandleEndSession()
    {
        ActionsManager.OnSpawnCart?.Invoke(true);
    }

    public void SpawnCart(bool toSaloon = true)
    {
        Vector3 spawnPos = PlayerManager.Instance.PlayerObject.transform.position + new Vector3(spawnRadius, 0f, 0f);
        GameObject cart = Instantiate(CartPrefab, spawnPos, Quaternion.identity);
        if (toSaloon && cart.TryGetComponent(out CartScript cartScript))
        {
            cartScript.SetSaloon(_saloon);
        }
    }
}
