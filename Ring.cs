using System;
using Android.Graphics;

namespace xam.LoadToast
{
    public class Ring
    {
        private RectF mTempBounds = new RectF();
        private Paint mPaint = new Paint();
        private Paint mArrowPaint = new Paint();

        private callBack mCallback;
        private Paint mCirclePaint = new Paint();
        private float mStartTrim = 0.0f;
        private float mEndTrim = 0.0f;
        private float mRotation = 0.0f;
        private float mStrokeWidth = 5.0f;
        private float mStrokeInset = 2.5f;
        private Color[] mColors;
        // mColorIndex represents the offset into the available mColors that the
        // progress circle should currently display. As the progress circle is
        // animating, the mColorIndex moves by one to the next available color.
        private int mColorIndex;
        private float mStartingStartTrim;
        private float mStartingEndTrim;
        private float mStartingRotation;
        private bool mShowArrow;
        private Path mArrow;
        private float mArrowScale;
        private double mRingCenterRadius;
        private int mArrowWidth;
        private int mArrowHeight;
        private int mAlpha;
        private Color mBackgroundColor;
        private float ARROW_OFFSET_ANGLE = 0;

        public Ring(callBack callback)
        {
            mCallback = callback;

            mPaint.StrokeCap = Paint.Cap.Square;
            mPaint.AntiAlias = true;
            mPaint.SetStyle(Paint.Style.Stroke);

            mArrowPaint.SetStyle(Paint.Style.Fill);
            mArrowPaint.AntiAlias = true;
        }

        public void setBackgroundColor(Color color)
        {
            mBackgroundColor = color;
        }

        /**
         * Set the dimensions of the arrowhead.
         *
         * @param width  Width of the hypotenuse of the arrow head
         * @param height Height of the arrow point
         */
        public void setArrowDimensions(float width, float height)
        {
            mArrowWidth = (int)width;
            mArrowHeight = (int)height;
        }

        /**
         * Draw the progress spinner
         */
        public void draw(Canvas c, Rect bounds)
        {
            RectF arcBounds = mTempBounds;
            arcBounds.Set(bounds);
            arcBounds.Inset(mStrokeInset, mStrokeInset);

            float startAngle = (mStartTrim + mRotation) * 360;
            float endAngle = (mEndTrim + mRotation) * 360;
            float sweepAngle = endAngle - startAngle;
            mPaint.Color = mColors[mColorIndex];
            c.DrawArc(arcBounds, startAngle, sweepAngle, false, mPaint);

            drawTriangle(c, startAngle, sweepAngle, bounds);

            if (mAlpha < 255)
            {
                mCirclePaint.Color = mBackgroundColor;
                mCirclePaint.Alpha = 255 - mAlpha;
                c.DrawCircle(bounds.ExactCenterX(), bounds.ExactCenterY(), bounds.Width() / 2,
                        mCirclePaint);
            }
        }

        private void drawTriangle(Canvas c, float startAngle, float sweepAngle, Rect bounds)
        {
            if (mShowArrow)
            {
                if (mArrow == null)
                {
                    mArrow = new Path();
                    mArrow.SetFillType(Path.FillType.EvenOdd);
                }
                else
                {
                    mArrow.Reset();
                }

                // Adjust the position of the triangle so that it is inset as
                // much as the arc, but also centered on the arc.
                float x = (float)(mRingCenterRadius * Math.Cos(0) + bounds.ExactCenterX());
                float y = (float)(mRingCenterRadius * Math.Sin(0) + bounds.ExactCenterY());

                // Update the path each time. This works around an issue in SKIA
                // where concatenating a rotation matrix to a scale matrix
                // ignored a starting negative rotation. This appears to have
                // been fixed as of API 21.
                mArrow.MoveTo(0, 0);
                mArrow.LineTo((mArrowWidth) * mArrowScale, 0);
                mArrow.LineTo(((mArrowWidth) * mArrowScale / 2), (mArrowHeight
                        * mArrowScale));
                mArrow.Offset(x - ((mArrowWidth) * mArrowScale / 2), y);
                mArrow.Close();
                // draw a triangle
                mArrowPaint.Color = mColors[mColorIndex];
                //when sweepAngle < 0 adjust the position of the arrow
                c.Rotate(startAngle + (sweepAngle < 0 ? 0 : sweepAngle) - ARROW_OFFSET_ANGLE, bounds.ExactCenterX(),
                        bounds.ExactCenterY());
                c.DrawPath(mArrow, mArrowPaint);
            }
        }

        /**
         * Set the colors the progress spinner alternates between.
         *
         * @param colors Array of integers describing the colors. Must be non-<code>null</code>.
         */
        public void setColors(Color[] colors)
        {
            mColors = colors;
            // if colors are reset, make sure to reset the color index as well
            setColorIndex(0);
        }

        /**
         * @param index Index into the color array of the color to display in
         *              the progress spinner.
         */
        public void setColorIndex(int index)
        {
            mColorIndex = index;
        }

        /**
         * Proceed to the next available ring color. This will automatically
         * wrap back to the beginning of colors.
         */
        public void goToNextColor()
        {
            mColorIndex = (mColorIndex + 1) % (mColors.Length);
        }

        public void setColorFilter(ColorFilter filter)
        {
            mPaint.SetColorFilter(filter);
            invalidateSelf();
        }

        /**
         * @return Current alpha of the progress spinner and arrowhead.
         */
        public int getAlpha()
        {
            return mAlpha;
        }

        /**
         * @param alpha Set the alpha of the progress spinner and associated arrowhead.
         */
        public void setAlpha(int alpha)
        {
            mAlpha = alpha;
        }

        //@SuppressWarnings("unused")
        public float getStrokeWidth()
        {
            return mStrokeWidth;
        }

        /**
         * @param strokeWidth Set the stroke width of the progress spinner in pixels.
         */
        public void setStrokeWidth(float strokeWidth)
        {
            mStrokeWidth = strokeWidth;
            mPaint.StrokeWidth = strokeWidth;
            invalidateSelf();
        }

        //@SuppressWarnings("unused")
        public float getStartTrim()
        {
            return mStartTrim;
        }

        //@SuppressWarnings("unused")
        public void setStartTrim(float startTrim)
        {
            mStartTrim = startTrim;
            invalidateSelf();
        }

        public float getStartingStartTrim()
        {
            return mStartingStartTrim;
        }

        public float getStartingEndTrim()
        {
            return mStartingEndTrim;
        }

        //@SuppressWarnings("unused")
        public float getEndTrim()
        {
            return mEndTrim;
        }

        //@SuppressWarnings("unused")
        public void setEndTrim(float endTrim)
        {
            mEndTrim = endTrim;
            invalidateSelf();
        }

        //@SuppressWarnings("unused")
        public float getRotation()
        {
            return mRotation;
        }

        ////@SuppressWarnings("unused")
        public void setRotation(float rotation)
        {
            mRotation = rotation;
            invalidateSelf();
        }

        public void setInsets(int width, int height)
        {
            float minEdge = (float)Math.Min(width, height);
            float insets;
            if (mRingCenterRadius <= 0 || minEdge < 0)
            {
                insets = (float)Math.Ceiling(mStrokeWidth / 2.0f);
            }
            else
            {
                insets = (float)(minEdge / 2.0f - mRingCenterRadius);
            }
            mStrokeInset = insets;
        }

        ////@SuppressWarnings("unused")
        public float getInsets()
        {
            return mStrokeInset;
        }

        public double getCenterRadius()
        {
            return mRingCenterRadius;
        }

        /**
         * @param centerRadius Inner radius in px of the circle the progress
         *                     spinner arc traces.
         */
        public void setCenterRadius(double centerRadius)
        {
            mRingCenterRadius = centerRadius;
        }

        /**
         * @param show Set to true to show the arrow head on the progress spinner.
         */
        public void setShowArrow(bool show)
        {
            if (mShowArrow != show)
            {
                mShowArrow = show;
                invalidateSelf();
            }
        }

        /**
         * @param scale Set the scale of the arrowhead for the spinner.
         */
        public void setArrowScale(float scale)
        {
            if (scale != mArrowScale)
            {
                mArrowScale = scale;
                invalidateSelf();
            }
        }

        /**
         * @return The amount the progress spinner is currently rotated, between [0..1].
         */
        public float getStartingRotation()
        {
            return mStartingRotation;
        }

        /**
         * If the start / end trim are offset to begin with, store them so that
         * animation starts from that offset.
         */
        public void storeOriginals()
        {
            mStartingStartTrim = mStartTrim;
            mStartingEndTrim = mEndTrim;
            mStartingRotation = mRotation;
        }

        /**
         * Reset the progress spinner to default rotation, start and end angles.
         */
        public void resetOriginals()
        {
            mStartingStartTrim = 0;
            mStartingEndTrim = 0;
            mStartingRotation = 0;
            setStartTrim(0);
            setEndTrim(0);
            setRotation(0);
        }

        private void invalidateSelf()
        {
            mCallback.InvalidateDrawable(null);
        }
    }
}