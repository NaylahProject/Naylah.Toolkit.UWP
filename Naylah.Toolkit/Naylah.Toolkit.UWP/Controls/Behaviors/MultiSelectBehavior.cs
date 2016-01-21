using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Naylah.Toolkit.UWP.Behaviors
{
    public class MultiSelectBehavior : DependencyObject, IBehavior
    {

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
    "SelectedItems",
    typeof(ObservableCollection<object>),
    typeof(MultiSelectBehavior),
    new PropertyMetadata(new ObservableCollection<object>(), PropertyChangedCallback));

        private bool _selectionChangedInProgress;

        public DependencyObject AssociatedObject
        {
            get; set;
        }

        public ListViewBase AssociatedObjectList
        {
            get { return (ListViewBase)AssociatedObject; }
        }


        public ObservableCollection<object> SelectedItems
        {
            get { return (ObservableCollection<object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            AssociatedObjectList.SelectionChanged += OnSelectionChanged;
        }

        public void Detach()
        {
            AssociatedObjectList.SelectionChanged -= OnSelectionChanged;
        }


        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectionChangedInProgress) return;
            _selectionChangedInProgress = true;
            foreach (var item in e.RemovedItems)
            {
                if (SelectedItems.Contains(item))
                {
                    SelectedItems.Remove(item);
                }
            }

            foreach (var item in e.AddedItems)
            {
                if (!SelectedItems.Contains(item))
                {
                    SelectedItems.Add(item);
                }
            }
            _selectionChangedInProgress = false;
        }


        private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler handler = (s, e) => SelectedItemsChanged(sender, e);
            if (args.OldValue is ObservableCollection<object>)
            {
                (args.OldValue as ObservableCollection<object>).CollectionChanged -= handler;
            }

            if (args.NewValue is ObservableCollection<object>)
            {
                (args.NewValue as ObservableCollection<object>).CollectionChanged += handler;
            }
        }

        private static void SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is MultiSelectBehavior)
            {
                var listViewBase = (sender as MultiSelectBehavior).AssociatedObjectList;

                var listSelectedItems = listViewBase.SelectedItems;
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (listSelectedItems.Contains(item))
                        {
                            listSelectedItems.Remove(item);
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (!listSelectedItems.Contains(item))
                        {
                            listSelectedItems.Add(item);
                        }
                    }
                }
            }
        }
    }
}
