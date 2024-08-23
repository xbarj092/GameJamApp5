using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringStorage", menuName = "Game/StringStorage")]
public class StringStorage : ScriptableObject
{
    [field: SerializeField, Space(5)] public List<string> Strings { get; private set; }

    public string GetString(int index)
    {
        if (index < 0 || index >= Strings.Count)
        {
            Debug.LogError($"Trying to get string from string storage {name}, but something went wrong!. Index: {index}");
            return null;
        }

        return Strings[index];
    }
}
