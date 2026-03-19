using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Transform unitPrefab;
    private void Start()
    {
        UnitSpawnManager.instance.SetSelectedUnit(unitPrefab);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            
        }
    }
}
