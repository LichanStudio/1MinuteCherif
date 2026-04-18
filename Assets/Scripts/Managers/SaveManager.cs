using System;
using System.IO;
using System.Text;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SAVE_FILE = "save.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE);

    public SaveData CurrentSave { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        Load();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(CurrentSave, prettyPrint: true);
        string encrypted = Encrypt(json);
        File.WriteAllText(SavePath, encrypted);
        CurrentSave.SetLastSaveDate();
        CurrentSave.SavePlayerPrefs();
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            CurrentSave = new SaveData();
            CurrentSave.LoadPlayerPrefs();
            return;
        }

        string encrypted = File.ReadAllText(SavePath);
        string json = Decrypt(encrypted);
        CurrentSave = JsonUtility.FromJson<SaveData>(json);
        CurrentSave.LoadPlayerPrefs();
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);

        CurrentSave = new SaveData();
    }

    private string Encrypt(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        return Convert.ToBase64String(bytes);
        // Simple obfuscation
        // Pour du vrai chiffrement => AES avec System.Security.Cryptography
    }

    private string Decrypt(string data)
    {
        byte[] bytes = Convert.FromBase64String(data);
        return Encoding.UTF8.GetString(bytes);
    }
}