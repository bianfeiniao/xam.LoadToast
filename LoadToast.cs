using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using Java.Lang;
using XViewPropertyAnimator = Xamarin.NineOldAndroids.Views.ViewPropertyAnimator;
using Xamarin.NineOldAndroids.Views;

namespace xam.LoadToast
{
    /**
  * Created by Wannes2 on 23/04/2015.
  */
    public class LoadToast
    {

        private string mText = "";
        private LoadToastView mView;
        private ViewGroup mParentView;
        private int mTranslationY = 0;
        private bool mShowCalled = false;
        private bool mToastCanceled = false;
        private bool mInflated = false;
        private bool mVisible = false;


        public LoadToast(Context context)
        {
            mView = new LoadToastView(context);
            mParentView = (ViewGroup)((Activity)context).Window.DecorView.FindViewById(Android.Resource.Id.Content);
            mParentView.AddView(mView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
            ViewHelper.SetAlpha(mView, 0);

            mParentView.PostDelayed(new Runnable(() =>
            {
                ViewHelper.SetTranslationX(mView, (mParentView.Width - mView.Width) / 2);
                ViewHelper.SetTranslationY(mView, -mView.Height + mTranslationY);
                mInflated = true;
                if (!mToastCanceled && mShowCalled) { show(); }

            }), 1);

            mParentView.ViewTreeObserver.AddOnGlobalLayoutListener(new OnGlobalLayoutListener()
            {
                GlobalLayout = () =>
           {
               checkZPosition();
           }
            });
        }

        public LoadToast setTranslationY(int pixels)
        {
            mTranslationY = pixels;
            return this;
        }

        public LoadToast setText(string message)
        {
            mText = message;

            mView.setText(mText);
            return this;
        }

        public LoadToast setTextColor(Color color)
        {
            mView.setTextColor(color);
            return this;
        }

        public LoadToast setBackgroundColor(Color color)
        {
            mView.SetBackgroundColor(color);
            return this;
        }

        public LoadToast setProgressColor(Color color)
        {
            mView.setProgressColor(color);
            return this;
        }

        public LoadToast show()
        {
            if (!mInflated)
            {
                mShowCalled = true;
                return this;
            }
            mView.show();
            ViewHelper.SetTranslationX(mView, (mParentView.Width - mView.Width) / 2);
            ViewHelper.SetAlpha(mView, 0f);
            ViewHelper.SetTranslationY(mView, -mView.Height + mTranslationY);
            //mView.SetVisibility(View.VISIBLE);
            XViewPropertyAnimator.Animate(mView).Alpha(1f).TranslationY(25 + mTranslationY)
                    .SetInterpolator(new DecelerateInterpolator())
                    .SetDuration(300).SetStartDelay(0).Start();

            mVisible = true;
            checkZPosition();

            return this;
        }

        public void success()
        {
            if (!mInflated)
            {
                mToastCanceled = true;
                return;
            }
            mView.success();
            slideUp();
        }

        public void error()
        {
            if (!mInflated)
            {
                mToastCanceled = true;
                return;
            }
            mView.error();
            slideUp();
        }

        private void checkZPosition()
        {
            // If the toast isn't visible, no point in updating all the views
            if (!mVisible) return;
            int pos = mParentView.IndexOfChild(mView);
            int count = mParentView.ChildCount;
            if (pos != count - 1)
            {
                ((ViewGroup)mView.Parent).RemoveView(mView);
                mParentView.RequestLayout();
                mParentView.AddView(mView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
            }
        }

        private void slideUp()
        {
            XViewPropertyAnimator.Animate(mView).SetStartDelay(1000).Alpha(0f)
            .TranslationY(-mView.Height + mTranslationY)
            .SetInterpolator(new AccelerateInterpolator())
            .SetDuration(300)
            .Start();

            mVisible = false;
        }
    }
}
