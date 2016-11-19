using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Xamarin.NineOldAndroids.Animations;
using Android.Views.Animations;
using Android.Util;
using Android.Content.Res;

namespace xam.LoadToast
{
    /**
* Created by Wannes2 on 23/04/2015.
*/
    public class LoadToastView :ImageView
    {

    private string mText = "";

    private Paint textPaint = new Paint();
    private Paint backPaint = new Paint();
    private Paint iconBackPaint = new Paint();
    private Paint loaderPaint = new Paint();
    private Paint successPaint = new Paint();
    private Paint errorPaint = new Paint();

    private Rect iconBounds;
    private Rect mTextBounds = new Rect();
    private RectF spinnerRect = new RectF();

    private int MAX_TEXT_WIDTH = 100; // in DP
    private int BASE_TEXT_SIZE = 20;
    private int IMAGE_WIDTH = 40;
    private int TOAST_HEIGHT = 48;
    private int LINE_WIDTH = 3;
    private float WIDTH_SCALE = 0f;
    private int MARQUE_STEP = 1;

    private long prevUpdate = 0;

    private Drawable completeicon;
    private Drawable failedicon;

    private ValueAnimator va;
    private ValueAnimator cmp;

    private bool issuccess = true;
    private bool outOfBounds = false;

    private Path toastPath = new Path();
    private AccelerateDecelerateInterpolator easeinterpol = new AccelerateDecelerateInterpolator();
    private MaterialProgressDrawable spinnerDrawable;

        public LoadToastView(Context context) : base(context)
        {
            textPaint.TextSize = 15;
            textPaint.Color = Color.Black;
            textPaint.AntiAlias = true;

            backPaint.Color = Color.White;
            backPaint.AntiAlias = true;

            iconBackPaint.Color = Color.Blue;
            iconBackPaint.AntiAlias = true;

            loaderPaint.StrokeWidth = dpToPx(4);
            loaderPaint.AntiAlias = true;
            loaderPaint.Color = fetchPrimaryColor();
            loaderPaint.SetStyle(Paint.Style.Stroke);

            successPaint.Color = Resources.GetColor(Resource.Color.color_success);
            errorPaint.Color = Resources.GetColor(Resource.Color.color_error);
            successPaint.AntiAlias = true;
            errorPaint.AntiAlias = true;

            MAX_TEXT_WIDTH = dpToPx(MAX_TEXT_WIDTH);
            BASE_TEXT_SIZE = dpToPx(BASE_TEXT_SIZE);
            IMAGE_WIDTH = dpToPx(IMAGE_WIDTH);
            TOAST_HEIGHT = dpToPx(TOAST_HEIGHT);
            LINE_WIDTH = dpToPx(LINE_WIDTH);
            MARQUE_STEP = dpToPx(MARQUE_STEP);

            int padding = (TOAST_HEIGHT - IMAGE_WIDTH) / 2;
            iconBounds = new Rect(TOAST_HEIGHT + MAX_TEXT_WIDTH - padding, padding, TOAST_HEIGHT + MAX_TEXT_WIDTH - padding + IMAGE_WIDTH, IMAGE_WIDTH + padding);
            //loadicon = Resources.getDrawable(R.mipmap.ic_launcher);
            //loadicon.SetBounds(iconBounds);

            completeicon = Resources.GetDrawable(Resource.Drawable.ic_navigation_check);
            completeicon.Bounds = iconBounds;
            failedicon = Resources.GetDrawable(Resource.Drawable.ic_error);
            failedicon.Bounds = iconBounds;

            va = ValueAnimator.OfFloat(0, 1);
            va.SetDuration(6000);
            va.AddUpdateListener(new AnimatorUpdateListener()
            {
                AnimationUpdate = (a) =>
           {
               //WIDTH_SCALE = valueAnimator.getAnimatedFraction();
               PostInvalidate();
           }
            });

            va.RepeatMode = ValueAnimator.Infinite;
            va.RepeatCount = 9999999;
            va.SetInterpolator(new LinearInterpolator());
            va.Start();
            initSpinner();
            calculateBounds();
        }

    private void initSpinner()
{
    spinnerDrawable = new MaterialProgressDrawable(Context, this);

    spinnerDrawable.setStartEndTrim(0, .5f);
    spinnerDrawable.setProgressRotation(.5f);

    int mDiameter = TOAST_HEIGHT;
    int mProgressStokeWidth = LINE_WIDTH;
    spinnerDrawable.setSizeParameters(mDiameter, mDiameter,
            (mDiameter - mProgressStokeWidth * 2) / 4,
            mProgressStokeWidth,
            mProgressStokeWidth * 4,
            mProgressStokeWidth * 2);

    spinnerDrawable.setBackgroundColor(Color.Transparent);
            spinnerDrawable.setColorSchemeColors(new Color[] { loaderPaint.Color });
    spinnerDrawable.SetVisible(true, false);
    spinnerDrawable.SetAlpha(255);

    SetImageDrawable(null);
    SetImageDrawable(spinnerDrawable);

    spinnerDrawable.Start();
}

public void setTextColor(Color color)
{
    textPaint.Color=color;
}

public void setBackgroundColor(Color color)
{
    backPaint.Color=color;
    iconBackPaint.Color=color;
}

public void setProgressColor(Color color)
{
    loaderPaint.Color=color;
            spinnerDrawable.setColorSchemeColors(new Color[] { color });
}

public void show()
{
    WIDTH_SCALE = 0f;
    if (cmp != null) cmp.RemoveAllUpdateListeners();
}

public void success()
{
            issuccess = true;
    done();
}

public void error()
{
            issuccess = false;
    done();
}

private void done()
{
    cmp = ValueAnimator.OfFloat(0, 1);
    cmp.SetDuration(600);
            cmp.AddUpdateListener(new AnimatorUpdateListener()
            {

                AnimationUpdate = (valueAnimator) =>
          {
              WIDTH_SCALE = 2f * (valueAnimator.AnimatedFraction);
    //Log.d("lt", "ws " + WIDTH_SCALE);
    PostInvalidate();
          }
            });
        cmp.SetInterpolator(new DecelerateInterpolator());
        cmp.Start();
    }

        private Color fetchPrimaryColor()
        {
            Color color = Color.Rgb(155, 155, 155);
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
            {
                TypedValue typedValue = new TypedValue();
                TypedArray a = Context.ObtainStyledAttributes(typedValue.Data, new int[] { Android.Resource.Attribute.ColorAccent });
                color = a.GetColor(0, color);
                a.Recycle();
            }
            return color;
        }

        private int dpToPx(int dp)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Resources.DisplayMetrics);
        }

public void setText(string text)
{
    mText = text;
    calculateBounds();
}

private void calculateBounds()
{
    outOfBounds = false;
    prevUpdate = 0;

    textPaint.TextSize=BASE_TEXT_SIZE;
    textPaint.GetTextBounds(mText, 0, mText.Length, mTextBounds);
    if (mTextBounds.Width() > MAX_TEXT_WIDTH)
    {
        int textSize = BASE_TEXT_SIZE;
        while (textSize > dpToPx(13) && mTextBounds.Width() > MAX_TEXT_WIDTH)
        {
            textSize--;
            //Log.d("bounds", "width " + mTextBounds.width() + " max " + MAX_TEXT_WIDTH);
            textPaint.TextSize=textSize;
            textPaint.GetTextBounds(mText, 0, mText.Length, mTextBounds);
        }
        if (mTextBounds.Width() > MAX_TEXT_WIDTH)
        {
            outOfBounds = true;
            /**
            float keep = (float)MAX_TEXT_WIDTH / (float)mTextBounds.width();
            int charcount = (int)(mText.length() * keep);
            //Log.d("calc", "keep " + charcount + " per " + keep + " len " + mText.length());
            mText = mText.substring(0, charcount);
            textPaint.getTextBounds(mText, 0, mText.length(), mTextBounds);
            **/
        }
    }
}

        protected override void OnDraw(Canvas c)
        {
            float ws = Math.Max(1f - WIDTH_SCALE, 0f);
            // If there is nothing to display, just draw a circle
            if (mText.Length == 0) ws = 0;

            float translateLoad = (1f - ws) * (IMAGE_WIDTH + MAX_TEXT_WIDTH);
            float leftMargin = translateLoad / 2;
            float textOpactity = Math.Max(0, ws * 10f - 9f);
            textPaint.Alpha = (int)(textOpactity * 255);
            spinnerRect.Set(iconBounds.Left + dpToPx(4) - translateLoad / 2, iconBounds.Top + dpToPx(4),
                            iconBounds.Right - dpToPx(4) - translateLoad / 2, iconBounds.Bottom - dpToPx(4));

            int circleOffset = (int)(TOAST_HEIGHT * 2 * (Math.Sqrt(2) - 1) / 3);
            int th = TOAST_HEIGHT;
            int pd = (TOAST_HEIGHT - IMAGE_WIDTH) / 2;
            int iconoffset = (int)(IMAGE_WIDTH * 2 * (Math.Sqrt(2) - 1) / 3);
            int iw = IMAGE_WIDTH;

            float totalWidth = leftMargin * 2 + th + ws * (IMAGE_WIDTH + MAX_TEXT_WIDTH) - translateLoad;

            toastPath.Reset();
            toastPath.MoveTo(leftMargin + th / 2, 0);
            toastPath.RLineTo(ws * (IMAGE_WIDTH + MAX_TEXT_WIDTH), 0);
            toastPath.RCubicTo(circleOffset, 0, th / 2, th / 2 - circleOffset, th / 2, th / 2);

            toastPath.RLineTo(-pd, 0);
            toastPath.RCubicTo(0, -iconoffset, -iw / 2 + iconoffset, -iw / 2, -iw / 2, -iw / 2);
            toastPath.RCubicTo(-iconoffset, 0, -iw / 2, iw / 2 - iconoffset, -iw / 2, iw / 2);
            toastPath.RCubicTo(0, iconoffset, iw / 2 - iconoffset, iw / 2, iw / 2, iw / 2);
            toastPath.RCubicTo(iconoffset, 0, iw / 2, -iw / 2 + iconoffset, iw / 2, -iw / 2);
            toastPath.RLineTo(pd, 0);

            toastPath.RCubicTo(0, circleOffset, circleOffset - th / 2, th / 2, -th / 2, th / 2);
            toastPath.RLineTo(ws * (-IMAGE_WIDTH - MAX_TEXT_WIDTH), 0);
            toastPath.RCubicTo(-circleOffset, 0, -th / 2, -th / 2 + circleOffset, -th / 2, -th / 2);
            toastPath.RCubicTo(0, -circleOffset, -circleOffset + th / 2, -th / 2, th / 2, -th / 2);
            c.DrawCircle(spinnerRect.CenterX(), spinnerRect.CenterY(), iconBounds.Height() / 1.9f, backPaint);

            c.DrawPath(toastPath, backPaint);
            toastPath.Reset();

            float prog = va.AnimatedFraction * 6.0f;
            float progrot = prog % 2.0f;
            float proglength = easeinterpol.GetInterpolation(prog % 3f / 3f) * 3f - .75f;
            if (proglength > .75f)
            {
                proglength = .75f - (prog % 3f - 1.5f);
                progrot += (prog % 3f - 1.5f) / 1.5f * 2f;
            }

            if (mText.Length == 0)
            {
                ws = Math.Max(1f - WIDTH_SCALE, 0f);
            }

            c.Save();

            c.Translate((totalWidth - TOAST_HEIGHT) / 2, 0);
            base.OnDraw(c);

            c.Restore();

            if (WIDTH_SCALE > 1f)
            {
                Drawable icon = (issuccess) ? completeicon : failedicon;
                float circleProg = WIDTH_SCALE - 1f;
                textPaint.Alpha = (int)(128 * circleProg + 127);
                int paddingicon = (int)((1f - (.25f + (.75f * circleProg))) * TOAST_HEIGHT / 2);
                int completeoff = (int)((1f - circleProg) * TOAST_HEIGHT / 8);
                icon.SetBounds((int)spinnerRect.Left + paddingicon, (int)spinnerRect.Top + paddingicon + completeoff, (int)spinnerRect.Right - paddingicon, (int)spinnerRect.Bottom - paddingicon + completeoff);
                c.DrawCircle(leftMargin + TOAST_HEIGHT / 2, (1f - circleProg) * TOAST_HEIGHT / 8 + TOAST_HEIGHT / 2,
                        (.25f + (.75f * circleProg)) * TOAST_HEIGHT / 2, (issuccess) ? successPaint : errorPaint);
                c.Save();
                c.Rotate(90 * (1f - circleProg), leftMargin + TOAST_HEIGHT / 2, TOAST_HEIGHT / 2);
                icon.Draw(c);
                c.Restore();

                prevUpdate = 0;
                return;
            }

            int yPos = (int)((th / 2) - ((textPaint.Descent() + textPaint.Ascent()) / 2));

            if (outOfBounds)
            {
                float shift = 0;
                if (prevUpdate == 0)
                {
                    prevUpdate = DateTime.Now.Ticks;
                }
                else
                {
                    shift = ((float)(DateTime.Now.Ticks - prevUpdate) / 16f) * MARQUE_STEP;
                    if (shift - MAX_TEXT_WIDTH > mTextBounds.Width())
                    {
                        prevUpdate = 0;
                    }
                }
                c.ClipRect(th / 2, 0, th / 2 + MAX_TEXT_WIDTH, TOAST_HEIGHT);
                c.DrawText(mText, th / 2 - shift + MAX_TEXT_WIDTH, yPos, textPaint);
            }
            else
            {
                c.DrawText(mText, 0, mText.Length, th / 2 + (MAX_TEXT_WIDTH - mTextBounds.Width()) / 2, yPos, textPaint);
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            SetMeasuredDimension(measureWidth(widthMeasureSpec),
                    measureHeight(heightMeasureSpec));
        }

        /**
         * Determines the width of this view
         * @param measureSpec A measureSpec packed into an int
         * @return The width of the view, honoring constraints from measureSpec
         */
        private int measureWidth(int measureSpec)
        {
            int result = 0;
            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);

            if (specMode == MeasureSpecMode.Exactly)
            {
                // We were told how big to be
                result = specSize;
            }
            else
            {
                // Measure the text

                result = IMAGE_WIDTH + MAX_TEXT_WIDTH + TOAST_HEIGHT;
                if (specMode == MeasureSpecMode.AtMost)
                {
                    // Respect AT_MOST value if that was what is called for by measureSpec
                    result = Math.Min(result, specSize);
                }
            }
            return result;
        }

        /**
         * Determines the height of this view
         * @param measureSpec A measureSpec packed into an int
         * @return The height of the view, honoring constraints from measureSpec
         */
        private int measureHeight(int measureSpec)
        {
            int result = 0;
            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);
            if (specMode == MeasureSpecMode.Exactly)
            {
                // We were told how big to be
                result = specSize;
            }
            else
            {
                // Measure the text (beware: ascent is a negative number)
                result = TOAST_HEIGHT;
                if (specMode == MeasureSpecMode.AtMost)
                {
                    // Respect AT_MOST value if that was what is called for by measureSpec
                    result = Math.Min(result, specSize);
                }
            }
            return result;
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();

            if (cmp != null) cmp.RemoveAllUpdateListeners();
            if (va != null) va.RemoveAllUpdateListeners();
        }
}
}