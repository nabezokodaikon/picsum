using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SWF.UIComponent.Common
{
    public class RatingBar : Control
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        public event EventHandler<MouseEventArgs> RatingButtonMouseClick;

        #endregion

        #region インスタンス変数

        private int _maximumValue = 5;
        private int _value = 0;
        private readonly List<RatingButton> _ratingButtonList = new List<RatingButton>();

        #endregion

        #region パブリックプロパティ

        public int MaximumValue
        {
            get
            {
                return this._maximumValue;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                if (value < this._value)
                {
                    this._value = value;
                }

                this._maximumValue = value;

                this.createRatingButtons();
                this.setRatingButtonsLocation();
                this.SetValue(this._value);
            }
        }

        public int Value
        {
            get
            {
                return this._value;
            }
        }

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        #endregion

        #region コンストラクタ

        public RatingBar()
        {
            this.initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void SetValue(int value)
        {
            if (this._value < 0 || this._maximumValue < this._value)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            for (int index = 0; index < this._maximumValue; index++)
            {
                RatingButton btn = this._ratingButtonList[index];
                btn.IsActive = index < value;
            }

            this._value = value;
        }

        #endregion

        #region 継承メソッド

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.setRatingButtonsLocation();
            base.OnInvalidated(e);
        }

        protected virtual void OnRatingButtonMouseClick(MouseEventArgs e)
        {
            if (RatingButtonMouseClick != null)
            {
                RatingButtonMouseClick(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);
        }

        private void createRatingButtons()
        {
            foreach (RatingButton btn in this._ratingButtonList)
            {
                this.Controls.Remove(btn);
            }

            this._ratingButtonList.Clear();

            List<RatingButton> list = new List<RatingButton>();
            for (int i = 0; i < this._maximumValue; i++)
            {
                RatingButton btn = new RatingButton();
                btn.MouseClick += new MouseEventHandler(this.ratingButton_MouseClick);
                btn.BackColor = this.BackColor;
                list.Add(btn);
            }

            RatingButton[] ary = list.ToArray();

            this._ratingButtonList.AddRange(ary);

            this.Controls.AddRange(ary);
        }

        private void setRatingButtonsLocation()
        {
            RatingButton firstBtn = this._ratingButtonList.FirstOrDefault();
            if (firstBtn == null)
            {
                return;
            }

            int allWidth = firstBtn.Width * this._ratingButtonList.Count;

            int x = (int)((this.Width - allWidth) / 2f);
            int y = (int)((this.Height - firstBtn.Height) / 2f);

            foreach (RatingButton btn in this._ratingButtonList)
            {
                btn.Location = new Point(x, y);
                x += btn.Width;
            }
        }

        #endregion

        #region イベント

        private void ratingButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (!(sender is RatingButton))
            {
                throw new ArgumentException("評価値ボタンコントロールと互換性がありません。", "sender");
            }

            int index = this._ratingButtonList.IndexOf((RatingButton)sender);
            if (index < 0)
            {
                throw new ArgumentException("createRatingButtonsメソッドで作成されたRatingButtonではありません。", "sender");
            }

            RatingButton btn = this._ratingButtonList[index];
            if (btn.IsActive && index == this._value - 1)
            {
                this.SetValue(index);
            }
            else
            {
                this.SetValue(index + 1);
            }

            this.OnRatingButtonMouseClick(e);
        }

        #endregion
    }
}
