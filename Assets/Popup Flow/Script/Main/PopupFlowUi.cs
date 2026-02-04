using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AbS
{
    /// <summary>
    /// Popup UI Controller
    /// - Handles visual presentation
    /// - Button bindings
    /// - Show / Hide animations
    /// - Notifies PopupManager when closed
    /// </summary>
    public class PopupFlowUi : MonoBehaviour
    {
        #region Inspector References

        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform popupRoot;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TMP_Text confirmButtonText;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text cancelButtonText;

        #endregion Inspector References

        #region Events

        /// <summary>
        /// Invoked after popup is fully hidden
        /// Used by PopupManager to process queue
        /// </summary>
        public event Action OnPopupHidden;

        #endregion Events

        #region Private State

        private PopupFlowData currentData;
        private const float ANIM_DURATION = 0.3f;

        #endregion Private State

        #region Public API

        /// <summary>
        /// Displays popup with provided data
        /// </summary>
        public void Show(PopupFlowData data)
        {
            currentData = data;

            ApplyData(data);
            RegisterButtons();

            gameObject.SetActive(true);
            AnimateShow();

            data.OnShow?.Invoke();
        }

        /// <summary>
        /// Hides popup with animation
        /// </summary>
        public void Hide()
        {
            AnimateHide(() =>
            {
                currentData?.OnHide?.Invoke();
                gameObject.SetActive(false);

                // Notify PopupManager that popup is closed
                OnPopupHidden?.Invoke();
            });
        }

        #endregion Public API

        #region Data Binding

        /// <summary>
        /// Applies popup data to UI elements
        /// </summary>
        private void ApplyData(PopupFlowData data)
        {
            titleText.text = data.Title;
            descriptionText.text = data.Description;

            confirmButtonText.text = data.ConfirmButtonText;
            cancelButtonText.text = data.CancelButtonText;

            // Cancel button visibility handled by text presence
            cancelButton.gameObject.SetActive(!string.IsNullOrEmpty(data.CancelButtonText));
        }

        #endregion Data Binding

        #region Button Handling

        /// <summary>
        /// Registers button click callbacks
        /// </summary>
        private void RegisterButtons()
        {
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(() =>
            {
                currentData?.OnConfirm?.Invoke();

                // Only hide if allowed
                if (currentData == null || currentData.HideOnConfirm)
                    Hide();
            });

            cancelButton.onClick.AddListener(() =>
            {
                currentData?.OnCancel?.Invoke();

                // Only hide if allowed
                if (currentData == null || currentData.HideOnCancel)
                    Hide();
            });
        }

        #endregion Button Handling

        #region Animations

        /// <summary>
        /// Plays popup show animation
        /// </summary>
        private void AnimateShow()
        {
            canvasGroup.alpha = 0f;
            popupRoot.localScale = Vector3.one * 0.8f;

            canvasGroup.DOFade(1f, ANIM_DURATION);
            popupRoot
                .DOScale(1f, ANIM_DURATION)
                .SetEase(Ease.OutBack);
        }

        /// <summary>
        /// Plays popup hide animation
        /// </summary>
        private void AnimateHide(Action onComplete)
        {
            canvasGroup.DOFade(0f, ANIM_DURATION);
            popupRoot
                .DOScale(0.8f, ANIM_DURATION)
                .SetEase(Ease.InBack)
                .OnComplete(() => onComplete?.Invoke());
        }

        #endregion Animations
    }
}