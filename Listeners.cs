using System;
using Android.Graphics.Drawables;
using Java.Lang;
using Xamarin.NineOldAndroids.Animations;
using static Android.Graphics.Drawables.Drawable;
using static Android.Views.ViewTreeObserver;
using Android.Views.Animations;
using static Android.Views.Animations.Animation;

namespace xam.LoadToast
{
    public class OnGlobalLayoutListener : Java.Lang.Object, IOnGlobalLayoutListener
    {
        public Action GlobalLayout { get; set; }
        public void OnGlobalLayout()
        {
            GlobalLayout?.Invoke();
        }
    }

    public class AnimatorUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
    {
        public Action<ValueAnimator> AnimationUpdate { get; set; }
        public void OnAnimationUpdate(ValueAnimator p0)
        {
            AnimationUpdate?.Invoke(p0);
        }
    }

    public class callBack : Java.Lang.Object, ICallback
    {
        public Action<Drawable> _InvalidateDrawable { get; set; }
        public Action<Drawable, IRunnable, long> _ScheduleDrawable { get; set; }
        public Action<Drawable, IRunnable> _UnscheduleDrawable { get; set; }

        public void InvalidateDrawable(Drawable who)
        {
            _InvalidateDrawable?.Invoke(who);
        }

        public void ScheduleDrawable(Drawable who, IRunnable what, long when)
        {
            _ScheduleDrawable?.Invoke(who, what, when);
        }

        public void UnscheduleDrawable(Drawable who, IRunnable what)
        {
            _UnscheduleDrawable?.Invoke(who, what);
        }
    }

    public class XAnimation : Animation {
        public Action<float,Transformation> Apply{ get; set; }
        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            Apply?.Invoke(interpolatedTime, t);
        }

    }

    public class XAnimationListener : Java.Lang.Object, IAnimationListener
    {
        public Action<Animation> OnEnd { get; set; }
        public Action<Animation> OnRepeat { get; set; }
        public Action<Animation> OnStart { get; set; }

        public void OnAnimationEnd(Animation animation)
        {
            OnEnd?.Invoke(animation);
        }

        public void OnAnimationRepeat(Animation animation)
        {
            OnRepeat?.Invoke(animation);
        }

        public void OnAnimationStart(Animation animation)
        {
            OnStart?.Invoke(animation);
        }
    }



}