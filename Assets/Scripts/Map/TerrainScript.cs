using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    public int SpeedModifier = 0;

    public void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterManager.Instance.SelectedCharacter.AddAditionnalStats(new Stats { Speed = SpeedModifier });
        }
    }

    public void OnTriggerExit2D(UnityEngine.Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterManager.Instance.SelectedCharacter.AddAditionnalStats(new Stats { Speed = -SpeedModifier });
        }
    }
}
