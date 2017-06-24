﻿using System;
using System.Collections.Generic;
using System.Text;
using NDProperty;

namespace NSCI.UI.Controls
{
    public partial class ScrollView : ContentControl
    {
        private int maxScrollPosition;

        public override bool SupportSelection => true;

        [NDP]
        private void OnScrollPositionChanged(OnChangedArg<int> arg)
        {

        }

        protected override void OnHasFocusChanged(OnChangedArg<bool> arg)
        {
            base.OnHasFocusChanged(arg);
            InvalidateRender();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = base.EnsureMinMaxWidthHeight(availableSize);
            var scroolbarVisible = true;
            Content.Measure(availableSize.Inflat(scroolbarVisible ? -1 : 0, 0));
            return EnsureMinMaxWidthHeight(Content.DesiredSize.Inflat(scroolbarVisible ? 1 : 0, 0));
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            var scroolbarVisible = true;
            finalSize = finalSize.Inflat(scroolbarVisible && finalSize.Width > 0 ? -1 : 0, 0);

            maxScrollPosition = (int)(Content.DesiredSize.Height - finalSize.Height);

            if (ScrollPosition > maxScrollPosition)
                ScrollPosition = maxScrollPosition;
            if (ScrollPosition < 0)
                ScrollPosition = 0;

            Content.Arrange(new Rect(0, -ScrollPosition, finalSize.Width - 1, Content.DesiredSize.Height));

        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            var scroolbarVisible = true;
            //new Rect(new Point(0, -ScrollPosition), finalSize)
            Content.Render(frame.GetGraphicsBuffer(new Rect(0, -ScrollPosition, scroolbarVisible ? frame.Width - 1 : frame.Width, Content.ArrangedPosition.Height), new Rect(0, 0, scroolbarVisible ? frame.Width - 1 : frame.Width, frame.Height)));
            var foregroundColor = HasFocus ? ConsoleColor.Red : Background;

            if (frame.Height == 1)
            {
                if (ScrollPosition == 0)
                    frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.DOWNWARDS_ARROW, foregroundColor, Foreground);
                else if (ScrollPosition == maxScrollPosition)
                    frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.UPWARDS_ARROW, foregroundColor, Foreground);
                else
                    frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.UP_DOWN_ARROW, foregroundColor, Foreground);
            }
            else if (frame.Height == 2)
            {
                if (ScrollPosition == 0)
                    frame[frame.Width - 1, 0] = new ColoredKey(Characters.Block.FULL_BLOCK, Foreground, Background);
                else
                    frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.BLACK_UP_POINTING_TRIANGLE, foregroundColor, Foreground);

                if (ScrollPosition == maxScrollPosition)
                    frame[frame.Width - 1, 1] = new ColoredKey(Characters.Block.FULL_BLOCK, Foreground, Background);
                else
                    frame[frame.Width - 1, 1] = new ColoredKey(Characters.Arrows.BLACK_DOWN_POINTING_TRIANGLE, foregroundColor, Foreground);

            }
            else
            {


                frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.BLACK_UP_POINTING_TRIANGLE, foregroundColor, Foreground);
                frame[frame.Width - 1, frame.Height - 1] = new ColoredKey(Characters.Arrows.BLACK_DOWN_POINTING_TRIANGLE, foregroundColor, Foreground);
                frame.FillRect(frame.Width - 1, 1, 1, frame.Height - 2, Foreground, Background, Characters.Block.FULL_BLOCK);


                var numberOfPossibleScrollbarLocation = (frame.Height - 2) * 2 - 1;
                int currentScrollBarPosition;
                if (ScrollPosition == 0)
                    currentScrollBarPosition = 0;
                else if (ScrollPosition == maxScrollPosition)
                    currentScrollBarPosition = numberOfPossibleScrollbarLocation - 1;
                else
                    currentScrollBarPosition = (int)((ScrollPosition - 1) / (double)(maxScrollPosition - 2) * (numberOfPossibleScrollbarLocation - 2)) + 1;

                if (currentScrollBarPosition % 2 == 0) // Print FullBlock
                {
                    frame[frame.Width - 1, currentScrollBarPosition / 2 + 1] = new ColoredKey(Characters.Block.FULL_BLOCK, foregroundColor, Foreground);
                }
                else // Print two Half Blocks
                {
                    frame[frame.Width - 1, currentScrollBarPosition / 2 + 1] = new ColoredKey(Characters.Block.LOWER_HALF_BLOCK, foregroundColor, Foreground);
                    frame[frame.Width - 1, currentScrollBarPosition / 2 + 2] = new ColoredKey(Characters.Block.UPPER_HALF_BLOCK, foregroundColor, Foreground);
                }
            }


        }

        public override bool PreviewHandleInput(Control originalTarget, ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.UpArrow && keyInfo.Modifiers == ConsoleModifiers.Control || keyInfo.Key == ConsoleKey.DownArrow && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                if (keyInfo.Key == ConsoleKey.UpArrow)
                    ScrollPosition--;
                else
                    ScrollPosition++;
                InvalidateArrange();
                return true;
            }
            return base.PreviewHandleInput(originalTarget, keyInfo);
        }

    }
}
