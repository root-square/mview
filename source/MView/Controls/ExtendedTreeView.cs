using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MView.Controls
{
    public class ExtendedTreeView : TreeView
    {
        public ExtendedTreeView() : base()
        {
            this.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(___ICH);
        }

        void ___ICH(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectedItem != null)
            {
                SetValue(SelectedDirectoryItemProperty, SelectedItem);
            }
        }

        public object SelectedDirectoryItem
        {
            get
            {
                return (object)GetValue(SelectedDirectoryItemProperty);
            }
            set
            {
                SetValue(SelectedDirectoryItemProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedDirectoryItemProperty = DependencyProperty.Register("SelectedDirectoryItem", typeof(object), typeof(ExtendedTreeView), new UIPropertyMetadata(null));
    }
}
