using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishReadingApp.Entity
{
    public class WrapLayout : Layout<View>
    {
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            double currentX = 0;
            double currentY = 0;
            double maxHeightInRow = 0;

            foreach (var child in Children)
            {
                if (!child.IsVisible) continue;

                var measure = child.Measure(width, height);
                var childWidth = measure.Width;
                var childHeight = measure.Height;

                if (currentX + childWidth > width)
                {
                    currentX = 0;
                    currentY += maxHeightInRow;
                    maxHeightInRow = 0;
                }

                child.Layout(new Rect(x + currentX, y + currentY, childWidth, childHeight));

                currentX += childWidth;
                maxHeightInRow = Math.Max(maxHeightInRow, childHeight);
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            double width = 0;
            double height = 0;
            double currentWidth = 0;
            double currentHeight = 0;

            foreach (var child in Children)
            {
                if (!child.IsVisible) continue;

                var measure = child.Measure(widthConstraint, heightConstraint);
                var childWidth = measure.Width;
                var childHeight = measure.Height;

                if (currentWidth + childWidth > widthConstraint)
                {
                    width = Math.Max(width, currentWidth);
                    currentWidth = childWidth;
                    height += currentHeight;
                    currentHeight = childHeight;
                }
                else
                {
                    currentWidth += childWidth;
                    currentHeight = Math.Max(currentHeight, childHeight);
                }
            }

            width = Math.Max(width, currentWidth);
            height += currentHeight;

            return new SizeRequest(new Size(width, height));
        }
    }
}
