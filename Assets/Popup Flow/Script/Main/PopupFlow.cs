using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AbS
{
    /// <summary>
    /// Centralized Popup Manager
    /// - Singleton based
    /// - Queue supported (one popup at a time)
    /// - Handles default values & overloads
    /// </summary>
    public class PopupFlow : MonoBehaviour
    {
        #region Singleton

        public static PopupFlow Instance { get; private set; }

        #endregion Singleton

        #region Inspector References

        [Header("Popup UI")]
        [SerializeField] private PopupFlowUi popupUI;

        [Header("Default Popup Data")]
        public PopupFlowData DefaultData;

        #endregion Inspector References

        #region Private State

        // Queue to manage multiple popup requests
        private readonly Queue<PopupFlowData> popupQueue = new Queue<PopupFlowData>();

        // Tracks whether a popup is currently visible
        private bool isPopupActive;

        #endregion Private State

        #region Unity Lifecycle

        private void Awake()
        {
            // Enforce singleton
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Ensure EventSystem exists for UI interaction
            EnsureEventSystemExists();

            // Ensure default popup values are valid
            ValidateDefaults();

            // Subscribe to popup hide callback to handle queue
            popupUI.OnPopupHidden += HandlePopupHidden;
        }

        #endregion Unity Lifecycle

        #region Validation & Setup

        /// <summary>
        /// Ensures default popup data is always available
        /// </summary>
        private void ValidateDefaults()
        {
            if (DefaultData == null)
            {
                DefaultData = new PopupFlowData
                {
                    Title = "Alert",
                    Description = "Something happened",
                    ConfirmButtonText = "OK",
                    CancelButtonText = "Cancel"
                };
            }
        }

        #endregion Validation & Setup

        #region Core Show Logic (Queue Aware)

        /// <summary>
        /// Main popup show method (queue enabled)
        /// </summary>
        public void Show(
            string title = null,
            string description = null,
            string confirmButton = null,
            string cancelButton = null,
            Action onConfirm = null,
            Action onCancel = null,
            Action onShow = null,
            Action onHide = null
        )
        {
            PopupFlowData data = BuildPopupData(
                title,
                description,
                confirmButton,
                cancelButton,
                onConfirm,
                onCancel,
                onShow,
                onHide
            );

            EnqueueOrShow(data);
        }

        /// <summary>
        /// Either enqueue popup or show immediately
        /// </summary>
        private void EnqueueOrShow(PopupFlowData data)
        {
            if (isPopupActive)
            {
                popupQueue.Enqueue(data);
                return;
            }

            ShowInternal(data);
        }

        /// <summary>
        /// Shows popup immediately
        /// </summary>
        private void ShowInternal(PopupFlowData data)
        {
            isPopupActive = true;
            popupUI.Show(data);
        }

        /// <summary>
        /// Called when popup is hidden
        /// Displays next popup from queue if available
        /// </summary>
        private void HandlePopupHidden()
        {
            isPopupActive = false;

            if (popupQueue.Count > 0)
            {
                PopupFlowData nextPopup = popupQueue.Dequeue();
                ShowInternal(nextPopup);
            }
        }

        #endregion Core Show Logic (Queue Aware)

        #region Show Method Overloads

        public void Show(string title)
        {
            Show(title, null, null, null, null, null, null, null);
        }

        public void Show(string title, string description)
        {
            Show(title, description, null, null, null, null, null, null);
        }

        public void Show(string title, string description, Action onConfirm)
        {
            Show(title, description, null, null, onConfirm, null, null, null);
        }

        public void Show(string title, string description, string confirmButton)
        {
            Show(title, description, confirmButton, null, null, null, null, null);
        }

        public void Show(string title, string description, string confirmButton, Action onConfirm)
        {
            Show(title, description, confirmButton, null, onConfirm, null, null, null);
        }

        public void Show(string title, string description, string confirmButton, string cancelButton)
        {
            Show(title, description, confirmButton, cancelButton, null, null, null, null);
        }

        public void Show(string title, string description, Action onConfirm, Action onCancel)
        {
            Show(title, description, null, null, onConfirm, onCancel, null, null);
        }

        public void Show(
            string title,
            string description,
            string confirmButton,
            string cancelButton,
            Action onConfirm,
            Action onCancel
        )
        {
            Show(title, description, confirmButton, cancelButton, onConfirm, onCancel, null, null);
        }

        public void Show(
            string title,
            string description,
            Action onConfirm,
            Action onCancel,
            Action onShow,
            Action onHide
        )
        {
            Show(title, description, null, null, onConfirm, onCancel, onShow, onHide);
        }

        #endregion Show Method Overloads

        #region Popup Data Builder

        /// <summary>
        /// Builds popup data using defaults and overrides
        /// </summary>
        private PopupFlowData BuildPopupData(
            string title,
            string description,
            string confirmButton,
            string cancelButton,
            Action onConfirm,
            Action onCancel,
            Action onShow,
            Action onHide
        )
        {
            // Clone default data to avoid mutation
            PopupFlowData data = DefaultData.Clone();

            if (!string.IsNullOrEmpty(title)) data.Title = title;
            if (!string.IsNullOrEmpty(description)) data.Description = description;
            if (!string.IsNullOrEmpty(confirmButton)) data.ConfirmButtonText = confirmButton;

            // Cancel button visibility logic
            if (!string.IsNullOrEmpty(cancelButton) || onCancel != null)
            {
                if (!string.IsNullOrEmpty(cancelButton))
                    data.CancelButtonText = cancelButton;
            }
            else
            {
                // No cancel button if no text and no callback
                data.CancelButtonText = null;
            }

            data.OnConfirm = onConfirm;
            data.OnCancel = onCancel;
            data.OnShow = onShow;
            data.OnHide = onHide;

            return data;
        }

        #endregion Popup Data Builder

        #region Force Controls

        /// <summary>
        /// Force hide current popup
        /// </summary>
        public void Hide()
        {
            popupUI.Hide();
        }

        #endregion Force Controls

        #region Event System Helper

        /// <summary>
        /// Ensures EventSystem exists (important for UI interaction)
        /// </summary>
        public static void EnsureEventSystemExists()
        {
            if (EventSystem.current != null)
                return;

            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();

            Debug.Log("[PopupManager] EventSystem was missing and created automatically.");
        }

        #endregion Event System Helper
    }
}