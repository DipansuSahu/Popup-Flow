using AbS;
using System.Collections;
using UnityEngine;

public class PopupFlowExample : MonoBehaviour
{
    private void Start()
    {
        // Start demo coroutine
        StartCoroutine(DemoPopupQueue());
    }

    private IEnumerator DemoPopupQueue()
    {
        // -------------------------------
        // 1️⃣ First popup: default title, custom description
        // -------------------------------
        PopupFlow.Instance.Show(
            PopupFlow.Instance.DefaultData.Title,  // Using default title
            "No Internet Connection Detected!",
            onConfirm: () => Debug.Log("First popup confirmed"),
            onCancel: () => Debug.Log("First popup canceled"),
            onShow: () => Debug.Log("First popup shown"),
            onHide: () => Debug.Log("First popup hidden")
        );

        yield return new WaitForSeconds(2f); // Wait 2 seconds

        Debug.Log("Showing second popup...");

        // -------------------------------
        // 2️⃣ Second popup: custom title + description
        // -------------------------------
        PopupFlow.Instance.Show(
            "Server Error",
            "Failed to fetch data from server. Retry?",
            confirmButton: "Retry",
            cancelButton: "Cancel",
            onConfirm: RetryServer,
            onCancel: CancelServer
        );

        yield return new WaitForSeconds(1f); // Wait 1 second

        // -------------------------------
        // 3️⃣ Third popup: only title, using default description
        // -------------------------------
        PopupFlow.Instance.Show("Maintenance Notice");
    }

    // -------------------------------
    // Example confirm callback
    // -------------------------------
    private void RetryServer()
    {
        Debug.Log("Retrying server...");
        // Simulate retry logic here
    }

    // -------------------------------
    // Example cancel callback
    // -------------------------------
    private void CancelServer()
    {
        Debug.Log("Server request canceled by user.");
    }
}
