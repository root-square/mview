using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace MView.Behaviors
{
    public enum ScrollDirection
    {
        Top,
        Center,
        Bottom
    }

    public enum ScrollTarget
    {
        Vertical,
        Horizontal,
        Both
    }

    public static class AutoScrollBehavior
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(AutoScrollBehavior), new PropertyMetadata(false, OnAutoScrollPropertyChanged));

        public static readonly DependencyProperty ScrollTargetProperty =
            DependencyProperty.RegisterAttached("ScrollTarget", typeof(ScrollTarget), typeof(AutoScrollBehavior), new PropertyMetadata(ScrollTarget.Vertical, OnScrollTargetPropertyChanged));

        public static readonly DependencyProperty ScrollDirectionProperty =
            DependencyProperty.RegisterAttached("ScrollDirection", typeof(ScrollDirection), typeof(AutoScrollBehavior), new PropertyMetadata(ScrollDirection.Bottom, OnScrollDirectionPropertyChanged));

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }

        public static ScrollTarget GetScrollTarget(DependencyObject obj)
        {
            return (ScrollTarget)obj.GetValue(ScrollTargetProperty);
        }

        public static void SetScrollTarget(DependencyObject obj, ScrollTarget value)
        {
            obj.SetValue(ScrollTargetProperty, value);
        }

        public static ScrollDirection GetScrollDirection(DependencyObject obj)
        {
            return (ScrollDirection)obj.GetValue(ScrollDirectionProperty);
        }

        public static void SetScrollDirection(DependencyObject obj, ScrollDirection value)
        {
            obj.SetValue(ScrollDirectionProperty, value);
        }

        private static void ScrollVerticalBar(ScrollViewer? scrollViewer)
        {
            var scrollTarget = GetScrollTarget(scrollViewer!);
            var scrollDirection = GetScrollDirection(scrollViewer!);

            if (scrollTarget == ScrollTarget.Vertical || scrollTarget == ScrollTarget.Both)
            {
                if (scrollDirection == ScrollDirection.Top)
                {
                    scrollViewer?.ScrollToVerticalOffset(0);
                }
                else if (scrollDirection == ScrollDirection.Center)
                {
                    scrollViewer?.ScrollToVerticalOffset((scrollViewer.ExtentHeight - scrollViewer.ViewportHeight) / 2);
                }
                else
                {
                    scrollViewer?.ScrollToVerticalOffset(scrollViewer.ExtentHeight - scrollViewer.ViewportHeight);
                }
            }
        }

        private static void ScrollHorizontalBar(ScrollViewer? scrollViewer)
        {
            var scrollTarget = GetScrollTarget(scrollViewer!);
            var scrollDirection = GetScrollDirection(scrollViewer!);

            if (scrollTarget == ScrollTarget.Horizontal || scrollTarget == ScrollTarget.Both)
            {
                if (scrollDirection == ScrollDirection.Top)
                {
                    scrollViewer?.ScrollToHorizontalOffset(0);
                }
                else if (scrollDirection == ScrollDirection.Center)
                {
                    scrollViewer?.ScrollToHorizontalOffset((scrollViewer.ExtentWidth - scrollViewer.ViewportWidth) / 2);
                }
                else
                {
                    scrollViewer?.ScrollToHorizontalOffset(scrollViewer.ExtentWidth - scrollViewer.ViewportWidth);
                }
            }
        }

        private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            if (e.ExtentHeightChange != 0)
            {
                ScrollVerticalBar(scrollViewer);
            }

            if (e.ExtentWidthChange != 0)
            {
                ScrollHorizontalBar(scrollViewer);
            }
        }

        public static void OnAutoScrollPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var scrollViewer = obj as ScrollViewer;

            if (scrollViewer != null)
            {
                if ((bool)args.NewValue)
                {
                    ScrollVerticalBar(scrollViewer);
                    ScrollHorizontalBar(scrollViewer);

                    scrollViewer.ScrollChanged += OnScrollChanged;
                }
                else
                {
                    scrollViewer.ScrollChanged -= OnScrollChanged;
                }
            }
        }

        public static void OnScrollTargetPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var scrollViewer = obj as ScrollViewer;

            if (scrollViewer != null)
            {
                ScrollVerticalBar(scrollViewer);
                ScrollHorizontalBar(scrollViewer);
            }
        }

        public static void OnScrollDirectionPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var scrollViewer = obj as ScrollViewer;

            if (scrollViewer != null)
            {
                ScrollVerticalBar(scrollViewer);
                ScrollHorizontalBar(scrollViewer);
            }
        }
    }
}
