using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows")]
    public partial class RatingBar : Control
    {
        public event EventHandler<MouseEventArgs>? RatingButtonMouseClick;

        private int maximumValue = 5;
        private int ratingValue = 0;
        private readonly List<RatingButton> ratingButtonList = [];

        public int MaximumValue
        {
            get
            {
                return this.maximumValue;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 1, nameof(value));

                if (value < this.ratingValue)
                {
                    this.ratingValue = value;
                }

                this.maximumValue = value;

                this.CreateRatingButtons();
                this.SetRatingButtonsLocation();
                this.SetValue(this.ratingValue);
            }
        }

        public int Value
        {
            get
            {
                return this.ratingValue;
            }
        }

        public RatingBar()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();
        }

        public void SetValue(int value)
        {
            if (this.ratingValue < 0 || this.maximumValue < this.ratingValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            for (int index = 0; index < this.maximumValue; index++)
            {
                var btn = this.ratingButtonList[index];
                btn.IsActive = index < value;
            }

            this.ratingValue = value;
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.SetRatingButtonsLocation();
            base.OnInvalidated(e);
        }

        protected virtual void OnRatingButtonMouseClick(MouseEventArgs e)
        {
            this.RatingButtonMouseClick?.Invoke(this, e);
        }

        private void CreateRatingButtons()
        {
            foreach (var btn in this.ratingButtonList)
            {
                this.Controls.Remove(btn);
            }

            this.ratingButtonList.Clear();

            var list = new List<RatingButton>();
            for (var i = 0; i < this.maximumValue; i++)
            {
                var btn = new RatingButton();
                btn.MouseClick += new(this.RatingButton_MouseClick);
                btn.BackColor = this.BackColor;
                list.Add(btn);
            }

            var ary = list.ToArray();

            this.ratingButtonList.AddRange(ary);

            this.Controls.AddRange(ary);
        }

        private void SetRatingButtonsLocation()
        {
            var firstBtn = this.ratingButtonList.FirstOrDefault();
            if (firstBtn == null)
            {
                return;
            }

            var allWidth = firstBtn.Width * this.ratingButtonList.Count;

            var x = (int)((this.Width - allWidth) / 2f);
            var y = (int)((this.Height - firstBtn.Height) / 2f);

            foreach (var btn in this.ratingButtonList)
            {
                btn.Location = new Point(x, y);
                x += btn.Width;
            }
        }

        private void RatingButton_MouseClick(object? sender, MouseEventArgs e)
        {
            if (sender is not RatingButton)
            {
                throw new ArgumentException("評価値ボタンコントロールと互換性がありません。", nameof(sender));
            }

            var index = this.ratingButtonList.IndexOf((RatingButton)sender);
            if (index < 0)
            {
                throw new ArgumentException("createRatingButtonsメソッドで作成されたRatingButtonではありません。", nameof(sender));
            }

            var btn = this.ratingButtonList[index];
            if (btn.IsActive && index == this.ratingValue - 1)
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
