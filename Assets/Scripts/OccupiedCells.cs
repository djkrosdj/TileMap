using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OccupiedCells : MonoBehaviour
{
    // Словарь для хранения данных
    private Dictionary<Vector3, bool> _occupiedCells = new();
    
    // Метод для добавления элемента в словарь
    public void AddToDictionary(Vector3 key, bool value)
    {
        // Проверка наличия ключа в словаре
        if (!_occupiedCells.ContainsKey(key))
        {
            // Добавление элемента в словарь
            _occupiedCells.Add(key, value);
        }
    }

    public bool? IsCellOccupied(Vector3 key)
    {
        if (_occupiedCells.ContainsKey(key))
        {
            var value = _occupiedCells[key];
            return value;
        }

        return null;
    }
    
}
