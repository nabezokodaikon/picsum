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
                return _maximumValue;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                if (value < _value)
                {
                    _value = value;
                }

                _maximumValue = value;

                createRatingButtons();
                setRatingButtonsLocation();
                SetValue(_value);
            }
        }

        public int Value
        {
            get
            {
                return _value;
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
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void SetValue(int value)
        {
            if (_value < 0 || _maximumValue < _value)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            for (int index = 0; index < _maximumValue; index++)
            {
                RatingButton btn = _ratingButtonList[index];
                btn.IsActive = index < value;
            }

            _value = value;
        }

        #endregion

        #region 継承メソッド

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            setRatingButtonsLocation();
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
            foreach (RatingButton btn in _ratingButtonList)
            {
                this.Controls.Remove(btn);
            }

            _ratingButtonList.Clear();

            List<RatingButton> list = new List<RatingButton>();
            for (int i = 0; i < _maximumValue; i++)
            {
                RatingButton btn = new RatingButton();
                btn.MouseClick += new MouseEventHandler(ratingButton_MouseClick);
                btn.BackColor = this.BackColor;
                list.Add(btn);
            }

            RatingButton[] ary = list.ToArray();

            _ratingButtonList.AddRange(ary);

            this.Controls.AddRange(ary);
        }

        private void setRatingButtonsLocation()
        {
            RatingButton firstBtn = _ratingButtonList.FirstOrDefault();
            if (firstBtn == null)
            {
                return;
            }

            int allWidth = firstBtn.Width * _ratingButtonList.Count;

            int x = (int)((this.Width - allWidth) / 2f);
            int y = (int)((this.Height - firstBtn.Height) / 2f);

            foreach (RatingButton btn in _ratingButtonList)
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

            int index = _ratingButtonList.IndexOf((RatingButton)sender);
            if (index < 0)
            {
                throw new ArgumentException("createRatingButtonsメソッドで作成されたRatingButtonではありません。", "sender");
            }

            RatingButton btn = _ratingButtonList[index];
            if (btn.IsActive && index == _value - 1)
            {
                SetValue(index);
            }
            else
            {
                SetValue(index + 1);
            }

            OnRatingButtonMouseClick(e);
        }

        #endregion
    }
}
