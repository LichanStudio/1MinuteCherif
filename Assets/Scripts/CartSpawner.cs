using UnityEngine;

public class CartSpawner : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager _gameManager;

    [Header("Settings")]
    public GameObject CartPrefab;
    public Transform Player;
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
        Vector3 spawnPos = Player.position + new Vector3(spawnRadius, 0f, 0f);
        GameObject cart = Instantiate(CartPrefab, spawnPos, Quaternion.identity);
        if (toSaloon && cart.TryGetComponent(out CartScript cartScript))
        {
            cartScript.SetSaloon(_saloon);
        }
    }
}
