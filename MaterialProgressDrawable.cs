using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views.Animations;
using Android.Content.Res;
using Android.Util;

namespace xam.LoadToast
{

    public class MaterialProgressDrawable :Drawable, IAnimatable
    {
    
    // Maps to ProgressBar.Large style
    public static int LARGE = 0;
    // Maps to ProgressBar default style
    public static int DEFAULT = 1;
    private static LinearInterpolator LINEAR_INTERPOLATOR = new LinearInterpolator();
    private static EndCurveInterpolator END_CURVE_INTERPOLATOR = new EndCurveInterpolator();
    private static StartCurveInterpolator START_CURVE_INTERPOLATOR = new StartCurveInterpolator();
    private static AccelerateDecelerateInterpolator EASE_INTERPOLATOR = new AccelerateDecelerateInterpolator();

    // Maps to ProgressBar default style
    private static int CIRCLE_DIAMETER = 40;
    private static float CENTER_RADIUS = 8.75f; //should add up to 10 when + stroke_width
    private static float STROKE_WIDTH = 2.5f;
    // Maps to ProgressBar.Large style
    private static int CIRCLE_DIAMETER_LARGE = 56;
    private static float CENTER_RADIUS_LARGE = 12.5f;
    static float STROKE_WIDTH_LARGE = 3f;
    /**
     * The duration of a single progress spin in milliseconds.
     */
    private static int ANIMATION_DURATION = 1000 * 80 / 60;
    /**
     * The number of points in the progress "star".
     */
    private static float NUM_POINTS = 5f;
    /**
     * Layout info for the arrowhead in dp
     */
    private static int ARROW_WIDTH = 10;
    private static int ARROW_HEIGHT = 5;
    private static float ARROW_OFFSET_ANGLE = 0;
    /**
     * Layout info for the arrowhead for the large spinner in dp
     */
    static int ARROW_WIDTH_LARGE = 12;
    static int ARROW_HEIGHT_LARGE = 6;
    private static float MAX_PROGRESS_ARC = .8f;
        private Color[] COLORS = new Color[]{
            Color.Black
    };
    /**
     * The list of animators operating on this drawable.
     */
    private List<Animation> mAnimators = new List<Animation>();
    /**
     * The indicator ring, used to manage animation state.
     */
    private Ring mRing;

        private callBack mCallback = new callBack()
        {
            _InvalidateDrawable = (d) => { if (d != null) { d.InvalidateSelf(); } },
            _ScheduleDrawable = (d, what, when) => { if (d != null) { d.ScheduleSelf(what, when); } },
            _UnscheduleDrawable = (d, what) => {
                if (d != null)
                { d.UnscheduleSelf(what); }
            }
        };

bool mFinishing;
/**
 * Canvas rotation in degrees.
 */
private float mRotation;
private Resources mResources;
private View mAnimExcutor;
private Animation mAnimation;
private float mRotationCount;
private double mWidth;
private double mHeight;
private bool mShowArrowOnFirstStart = false;

public MaterialProgressDrawable(Context context, View animExcutor)
{
    mAnimExcutor = animExcutor;
    mResources = context.Resources;

    mRing = new Ring(mCallback);
    mRing.setColors(COLORS);

    updateSizes(DEFAULT);
    setupAnimators();
}

public void setSizeParameters(double progressCircleWidth, double progressCircleHeight,
                               double centerRadius, double strokeWidth, float arrowWidth, float arrowHeight)
{
    Ring ring = mRing;
    mWidth = progressCircleWidth;
    mHeight = progressCircleHeight;
    ring.setStrokeWidth((float)strokeWidth);
    ring.setCenterRadius(centerRadius);
    ring.setColorIndex(0);
    ring.setArrowDimensions(arrowWidth, arrowHeight);
    ring.setInsets((int)mWidth, (int)mHeight);
}

/**
 * Set the overall size for the progress spinner. This updates the radius
 * and stroke width of the ring.
 *

 */
public void updateSizes(int size)
{
    DisplayMetrics metrics = mResources.DisplayMetrics;
    float screenDensity = metrics.Density;
    if (size == LARGE)
    {
        setSizeParameters(CIRCLE_DIAMETER_LARGE * screenDensity, CIRCLE_DIAMETER_LARGE * screenDensity, CENTER_RADIUS_LARGE * screenDensity,
                STROKE_WIDTH_LARGE * screenDensity, ARROW_WIDTH_LARGE * screenDensity, ARROW_HEIGHT_LARGE * screenDensity);
    }
    else
    {
        setSizeParameters(CIRCLE_DIAMETER * screenDensity, CIRCLE_DIAMETER * screenDensity, CENTER_RADIUS * screenDensity, STROKE_WIDTH * screenDensity,
                ARROW_WIDTH * screenDensity, ARROW_HEIGHT * screenDensity);
    }
}

/**
 * @param show Set to true to display the arrowhead on the progress spinner.
 */
public void showArrow(bool show)
{
    mRing.setShowArrow(show);
}

/**
 * @param scale Set the scale of the arrowhead for the spinner.
 */
public void setArrowScale(float scale)
{
    mRing.setArrowScale(scale);
}

/**
 * Set the start and end trim for the progress spinner arc.
 *
 * @param startAngle start angle
 * @param endAngle   end angle
 */
public void setStartEndTrim(float startAngle, float endAngle)
{
    mRing.setStartTrim(startAngle);
    mRing.setEndTrim(endAngle);
}

/**
 * Set the amount of rotation to apply to the progress spinner.
 *
 * @param rotation Rotation is from [0..1]
 */
public void setProgressRotation(float rotation)
{
    mRing.setRotation(rotation);
}

/**
 * Update the background color of the circle image view.
 */
public void setBackgroundColor(Color color)
{
    mRing.setBackgroundColor(color);
}

/**
 * Set the colors used in the progress animation from color resources.
 * The first color will also be the color of the bar that grows in response
 * to a user swipe gesture.
 *
 * @param colors
 */
public void setColorSchemeColors(Color[] colors)
{
    mRing.setColors(colors);
    mRing.setColorIndex(0);
}
        public override int IntrinsicHeight
        {
            get
            {
                return (int)mHeight;
            }
        }
        public override int IntrinsicWidth
        {
            get
            {
                return (int)mWidth;
            }
        }

 public override void Draw(Canvas c)
{
    Rect bounds =this.Bounds;
    int saveCount = c.Save();
    c.Rotate(mRotation, bounds.ExactCenterX(), bounds.ExactCenterY());
    mRing.draw(c, bounds);
    c.RestoreToCount(saveCount);
}


        public override int Alpha
        {
            get
            {
                return mRing.getAlpha();
            }

            set
            {
                base.Alpha = value;
            }
        }

        public override void SetAlpha(int alpha)
        {
            mRing.setAlpha(alpha);
        }
      
    public override void SetColorFilter(ColorFilter colorFilter)
{
    mRing.setColorFilter(colorFilter);
}
    private float getRotation()
{
    return mRotation;
}

    void setRotation(float rotation)
{
    mRotation = rotation;
    InvalidateSelf();
}

        public override int Opacity
        {
            get
            {
                return 0;
            }
        }

        bool IAnimatable.IsRunning
        {
            get
            {
                throw new NotImplementedException();
            }
        }

   public  bool IsRunning()
{
 return !this.mAnimation.HasEnded;
}

        //@Override
    public void Start()
{
    mAnimation.Reset();
    mRing.storeOriginals();
    mRing.setShowArrow(mShowArrowOnFirstStart);

    // Already showing some part of the ring
    if (mRing.getEndTrim() != mRing.getStartTrim())
    {
        mFinishing = true;
        mAnimation.Duration=ANIMATION_DURATION / 2;
        mAnimExcutor.StartAnimation(mAnimation);
    }
    else
    {
        mRing.setColorIndex(0);
        mRing.resetOriginals();
        mAnimation.Duration=ANIMATION_DURATION;
        mAnimExcutor.StartAnimation(mAnimation);
    }
}

private void applyFinishTranslation(float interpolatedTime, Ring ring)
{
    // shrink back down and complete a full rotation before
    // starting other circles
    // Rotation goes between [0..1].
    float targetRotation = (float)(Math.Floor(ring.getStartingRotation() / MAX_PROGRESS_ARC)
            + 1f);
    float startTrim = ring.getStartingStartTrim()
            + (ring.getStartingEndTrim() - ring.getStartingStartTrim()) * interpolatedTime;
    ring.setStartTrim(startTrim);
    float rotation = ring.getStartingRotation()
            + ((targetRotation - ring.getStartingRotation()) * interpolatedTime);
    ring.setRotation(rotation);
}

 private void setupAnimators()
        {
            Ring ring = mRing;
            XAnimation animation = new XAnimation()
            {
                Apply = (interpolatedTime, t) =>
                  {
                      if (mFinishing)
                      {
                          applyFinishTranslation(interpolatedTime, ring);
                      }
                      else
                      {
                // The minProgressArc is calculated from 0 to create an
                // angle that
                // matches the stroke width.
						float minProgressArc = (float)Java.Lang.Math.ToRadians(ring.getStrokeWidth() / (2 * Math.PI * ring.getCenterRadius()));
                         
                          float startingEndTrim = ring.getStartingEndTrim();
                          float startingTrim = ring.getStartingStartTrim();
                          float startingRotation = ring.getStartingRotation();

                // Offset the minProgressArc to where the endTrim is
                // located.
                float minArc = MAX_PROGRESS_ARC - minProgressArc;
                          float endTrim = startingEndTrim + (minArc
                                  * START_CURVE_INTERPOLATOR.GetInterpolation(interpolatedTime));
                          float startTrim = startingTrim + (MAX_PROGRESS_ARC
                                  * END_CURVE_INTERPOLATOR.GetInterpolation(interpolatedTime));

                          float sweepTrim = endTrim - startTrim;
                //Avoid the ring to be a full circle
                if (Math.Abs(sweepTrim) >= 1)
                          {
                              endTrim = startTrim + 0.5f;
                          }

                          ring.setEndTrim(endTrim);

                          ring.setStartTrim(startTrim);

                          float rotation = startingRotation + (0.25f * interpolatedTime);
                          ring.setRotation(rotation);

                          float groupRotation = ((720.0f / NUM_POINTS) * interpolatedTime)
                                  + (720.0f * (mRotationCount / NUM_POINTS));
                          setRotation(groupRotation);
                      }
                  }
            };
            animation.RepeatCount = Animation.Infinite;
            animation.RepeatMode = RepeatMode.Restart;
            animation.Interpolator = LINEAR_INTERPOLATOR;
            animation.SetAnimationListener(new XAnimationListener()
            {
                OnStart = (a) =>
                  {
                      mRotationCount = 0;
                  },

                OnRepeat = (a) =>
                {
                    ring.storeOriginals();
                    ring.goToNextColor();
                    ring.setStartTrim(ring.getEndTrim());
                    if (mFinishing)
                    {
                        // finished closing the last ring from the swipe gesture; go
                        // into progress mode
                        mFinishing = false;
                        animation.Duration = ANIMATION_DURATION;
                        ring.setShowArrow(false);
                    }
                    else
                    {
                        mRotationCount = (mRotationCount + 1) % (NUM_POINTS);
                    }
                }
            });
            mAnimation = animation;
        }

 public void showArrowOnFirstStart(bool showArrowOnFirstStart)
{
    this.mShowArrowOnFirstStart = showArrowOnFirstStart;
}

        public void Stop()
        {
            mAnimExcutor.ClearAnimation();
            setRotation(0);
            mRing.setShowArrow(false);
            mRing.setColorIndex(0);
            mRing.resetOriginals();
        }

        public interface ProgressDrawableSize
{
}


/**
 * Squishes the interpolation curve into the second half of the animation.
 */

public class EndCurveInterpolator :AccelerateDecelerateInterpolator
{
    public override float GetInterpolation(float input)
    {
        return base.GetInterpolation(Math.Max(0, (input - 0.5f) * 2.0f));
    }
}

/**
 * Squishes the interpolation curve into the first half of the animation.
 */
public class StartCurveInterpolator :AccelerateDecelerateInterpolator
{
    public override float GetInterpolation(float input)
    {
        return base.GetInterpolation(Math.Min(1, input * 2.0f));
    }
}

}
}