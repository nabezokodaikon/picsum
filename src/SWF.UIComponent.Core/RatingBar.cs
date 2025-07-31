using SWF.Core.Base;
using System.ComponentModel;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class RatingBar
        : BaseControl
    {
        public event EventHandler<MouseEventArgs>? RatingButtonMouseClick;

        private int _maximumValue = 5;
        private int _ratingValue = 0;
        private readonly List<RatingButton> _ratingButtonList = [];

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaximumValue
        {
            get
            {
                return this._maximumValue;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 1, nameof(value));

                if (value < this._ratingValue)
                {
                    this._ratingValue = value;
                }

                this._maximumValue = value;

                this.CreateRatingButtons();
                this.SetControlsBounds(WindowUtil.GetCurrentWindowScale(this));
                this.SetValue(this._ratingValue);
            }
        }

        public int Value
        {
            get
            {
                return this._ratingValue;
            }
        }

        public RatingBar()
        {

        }

        public void SetControlsBounds(float scale)
        {
            var firstBtn = this._ratingButtonList.FirstOrDefault();
            if (firstBtn == null)
            {
                return;
            }

            firstBtn.SetControlsBounds(scale);
            var allWidth = firstBtn.Width * this._ratingButtonList.Count;
            var x = (int)((this.Width - allWidth) / 2f);
            var y = (int)((this.Height - firstBtn.Height) / 2f);
            firstBtn.Location = new Point(x, y);
        }

        public void SetValue(int value)
        {
            if (this._ratingValue < 0 || this._maximumValue < this._ratingValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            for (var index = 0; index < this._maximumValue; index++)
            {
                var btn = this._ratingButtonList[index];
                btn.IsActive = index < value;
            }

            this._ratingValue = value;
        }

        protected virtual void OnRatingButtonMouseClick(MouseEventArgs e)
        {
            this.RatingButtonMouseClick?.Invoke(this, e);
        }

        private void CreateRatingButtons()
        {
            foreach (var btn in this._ratingButtonList)
            {
                this.Controls.Remove(btn);
            }

            this._ratingButtonList.Clear();

            var list = new List<RatingButton>();
            for (var i = 0; i < this._maximumValue; i++)
            {
                var btn = new RatingButton();
                btn.MouseClick += new(this.RatingButton_MouseClick);
                btn.BackColor = this.BackColor;
                list.Add(btn);
            }

            var ary = list.ToArray();

            this._ratingButtonList.AddRange(ary);

            this.Controls.AddRange(ary);
        }

        private void RatingButton_MouseClick(object? sender, MouseEventArgs e)
        {
            if (sender is not RatingButton)
            {
                throw new ArgumentException("評価値ボタンコントロールと互換性がありません。", nameof(sender));
            }

            var index = this._ratingButtonList.IndexOf((RatingButton)sender);
            if (index < 0)
            {
                throw new ArgumentException("createRatingButtonsメソッドで作成されたRatingButtonではありません。", nameof(sender));
            }

            var btn = this._ratingButtonList[index];
            if (btn.IsActive && index == this._ratingValue - 1)
            {
                this.SetValue(index);
            }
            else
            {
                this.SetValue(index + 1);
            }

            this.OnRatingButtonMouseClick(e);
        }

    }
}
