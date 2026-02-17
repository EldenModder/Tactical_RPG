using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointText;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Unit unit;

    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        Unit.OnAnyActionPointChanged += Unit_OnAnyActionPointChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        UpdateAPText();
        UpdateHealthBar();
    }

    private void LateUpdate()
    {
        Vector3 dirToCam = (cameraTransform.position - transform.position).normalized;
        transform.LookAt(transform.position + dirToCam * -1);
    }

    private void UpdateAPText()
    {
        actionPointText.text = unit.GetActionPoint().ToString();
    }

    private void Unit_OnAnyActionPointChanged(object sender, EventArgs e)
    {
        UpdateAPText();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetNormalizedHealth();
    }
}
