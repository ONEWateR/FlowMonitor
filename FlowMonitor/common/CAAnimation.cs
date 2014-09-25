using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace onewater.flowmonitor.common
{
    class CAAnimation
    {

        // 默认缓冲
        public static BounceEase be = new BounceEase()
        {
            Bounces = 0,
            Bounciness = 1,
            EasingMode = EasingMode.EaseIn
        };

        /// <summary>
        /// 动态显示内容
        /// </summary>
        /// <param name="target"></param>
        /// <param name="duration"></param>
        public static void Show(ContentControl target, double duration = .8)
        {

            ThicknessAnimation animtion = new ThicknessAnimation()
            {
                From = new Thickness(target.Margin.Left + 150, target.Margin.Top, target.Margin.Right, target.Margin.Bottom),
                To = new Thickness(160, 0, 0, 0),
                Duration = TimeSpan.FromSeconds(duration),
                FillBehavior = FillBehavior.HoldEnd,
                AccelerationRatio = .5,
                EasingFunction = be
            };

            DoubleAnimation animtion2 = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(duration),
                FillBehavior = FillBehavior.HoldEnd,
                AccelerationRatio = .5,
                EasingFunction = be
            };

            target.BeginAnimation(ContentControl.MarginProperty, animtion);
            target.BeginAnimation(ContentControl.OpacityProperty, animtion2);
        }

        /// <summary>
        /// 播放动画（DoubleAnimation）
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="dp"></param>
        /// <param name="target"></param>
        /// <param name="duration"></param>
        public static void PlayAnimation(double from, double to, DependencyProperty dp, Control target, double duration = 1)
        {
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(duration),
                FillBehavior = FillBehavior.HoldEnd,
                AccelerationRatio = .5,
                EasingFunction = CAAnimation.be
            };
            target.BeginAnimation(dp, animation);
        }
    }
}
