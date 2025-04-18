using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private ProductionMenuView productionMenuView;
    [SerializeField] private InformationPanelView informationPanelView;
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TextMeshProUGUI notificationText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateInformationPanel(Building building)
    {
        informationPanelView.ShowBuildingInfo(building);
    }

    public void UpdateInformationPanel(Soldier unit)
    {
        informationPanelView.ShowUnitInfo(unit);
    }

    public void ClearInformationPanel()
    {
        informationPanelView.ClearPanel();
    }

    public void ShowNotification(string message)
    {
        notificationText.text = message;
        notificationPanel.SetActive(true);
        StartCoroutine(HideNotificationAfterDelay());
    }

    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        notificationPanel.SetActive(false);
    }
}