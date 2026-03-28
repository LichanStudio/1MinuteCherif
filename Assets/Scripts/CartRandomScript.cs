using System.Collections;
using UnityEngine;

public class CartRandomScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager _gameManager;

    [Header("Settings")]
    [SerializeField] private CartSpawner _spawner;
    [SerializeField] private Collider2D _doorToLock;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_doorToLock.enabled && collider != null && collider.gameObject.CompareTag("Player"))
        {
            if (_doorToLock != null) _doorToLock.enabled = true;
            ActionsManager.OnSpawnCart?.Invoke(false);
            ActionsManager.OnDamagePlayer?.Invoke(-9999);
            StartCoroutine(WaitAndLockDoor());
        }
    }

    private IEnumerator WaitAndLockDoor()
    {
        yield return new WaitForSeconds(2f);
        if (_doorToLock != null) _doorToLock.enabled = false;
    }
}
