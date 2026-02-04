using System;

namespace AbS
{
    [Serializable]
    public class PopupFlowData
    {
        public string Title;
        public string Description;

        public string ConfirmButtonText;
        public string CancelButtonText;

        public Action OnConfirm;
        public Action OnCancel;
        public Action OnShow;
        public Action OnHide;

        public bool HideOnConfirm = true;
        public bool HideOnCancel = true;

        public PopupFlowData Clone()
        {
            return new PopupFlowData
            {
                Title = this.Title,
                Description = this.Description,
                ConfirmButtonText = this.ConfirmButtonText,
                CancelButtonText = this.CancelButtonText,
                OnConfirm = this.OnConfirm,
                OnCancel = this.OnCancel,
                OnShow = this.OnShow,
                OnHide = this.OnHide,
                HideOnConfirm = this.HideOnConfirm,
                HideOnCancel = this.HideOnCancel
            };
        }
    }
}