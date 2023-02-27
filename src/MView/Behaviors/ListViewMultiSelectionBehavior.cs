using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MView.Behaviors
{
    public class ListViewMultiSelectionBehavior : Behavior<ListView>
    {
        private bool _isUpdatingTarget;
        private bool _isUpdatingSource;

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(ListViewMultiSelectionBehavior), new UIPropertyMetadata(null, OnSelectedItemsChanged));

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (SelectedItems != null)
            {
                AssociatedObject.SelectedItems.Clear();

                foreach (var item in SelectedItems)
                {
                    AssociatedObject.SelectedItems.Add(item);
                }
            }
        }
        private static void OnSelectedItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var behavior = o as ListViewMultiSelectionBehavior;

            if (behavior == null)
                return;

            var oldValue = e.OldValue as INotifyCollectionChanged;
            var newValue = e.NewValue as INotifyCollectionChanged;

            if (oldValue != null)
            {
                oldValue.CollectionChanged -= behavior.OnSourceCollectionChanged;
                behavior.AssociatedObject.SelectionChanged -= behavior.OnListViewSelectionChanged;
            }

            if (newValue != null)
            {
                behavior.AssociatedObject.SelectedItems.Clear();

                foreach (var item in (IEnumerable)newValue)
                {
                    behavior.AssociatedObject.SelectedItems.Add(item);
                }

                behavior.AssociatedObject.SelectionChanged += behavior.OnListViewSelectionChanged;
                newValue.CollectionChanged += behavior.OnSourceCollectionChanged;
            }
        }

        void OnSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isUpdatingSource)
                return;

            try
            {
                _isUpdatingTarget = true;

                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        AssociatedObject.SelectedItems.Remove(item);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        AssociatedObject.SelectedItems.Add(item);
                    }
                }

                if (e.Action == NotifyCollectionChangedAction.Reset)
                    AssociatedObject.SelectedItems.Clear();
            }
            finally
            {
                _isUpdatingTarget = false;
            }
        }

        private void OnListViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingTarget)
                return;

            var selectedItems = this.SelectedItems;

            if (selectedItems == null)

                return;

            try
            {
                _isUpdatingSource = true;

                if (AssociatedObject.SelectedItems.Count == 0)
                    selectedItems.Clear();
                else
                {
                    foreach (var item in e.RemovedItems)
                    {
                        selectedItems.Remove(item);
                    }
                }

                foreach (var item in e.AddedItems)
                {
                    selectedItems.Add(item);
                }
            }
            finally
            {
                _isUpdatingSource = false;
            }
        }
    }
}
