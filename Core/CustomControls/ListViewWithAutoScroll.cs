﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace TestTask_Somnium.Core.CustomControls
{

    /// <summary>
    /// Implementation of Autoscrolldown behavior on new message added in the linked collection
    /// The clearest way to do it with mvvm pattern, witch I was able to found
    /// thanks to the https://stackoverflow.com/questions/3317194/mvvm-how-to-make-a-list-view-auto-scroll-to-a-new-item-in-a-list-view
    /// </summary>
    class ListViewWithAutoScroll : ListView
    {
        ///<summary>
        ///Define the AutoScroll property. If enabled, causes the ListBox to scroll to 
        ///the last item whenever a new item is added.
        ///</summary>
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.Register(
                "AutoScroll",
                typeof(Boolean),
                typeof(ListViewWithAutoScroll),
                new FrameworkPropertyMetadata(
                    true, //Default value.
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    AutoScroll_PropertyChanged));

        /// <summary>
        /// Gets or sets whether or not the list should scroll to the last item 
        /// when a new item is added.
        /// </summary>
        [Category("Common")] //Indicate where the property is located in VS designer.
        public bool AutoScroll
        {
            get { return (bool)GetValue(AutoScrollProperty); }
            set { SetValue(AutoScrollProperty, value); }
        }

        /// <summary>
        /// Event handler for when the AutoScroll property is changed.
        /// This delegates the call to SubscribeToAutoScroll_ItemsCollectionChanged().
        /// </summary>
        /// <param name="d">The DependencyObject whose property was changed.</param>
        /// <param name="e">Change event args.</param>
        private static void AutoScroll_PropertyChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SubscribeToAutoScroll_ItemsCollectionChanged(
                (ListViewWithAutoScroll)d,
                (bool)e.NewValue);
        }

        /// <summary>
        /// Subscribes to the list items' collection changed event if AutoScroll is enabled.
        /// Otherwise, it unsubscribes from that event.
        /// For this to work, the underlying list must implement INotifyCollectionChanged.
        ///
        /// (This function was only creative for brevity)
        /// </summary>
        /// <param name="listView">The list box containing the items collection.</param>
        /// <param name="subscribe">Subscribe to the collection changed event?</param>
        private static void SubscribeToAutoScroll_ItemsCollectionChanged(
            ListViewWithAutoScroll listView, bool subscribe)
        {
            INotifyCollectionChanged notifyCollection =
                listView.Items.SourceCollection as INotifyCollectionChanged;
            if (notifyCollection != null)
            {
                if (subscribe)
                {
                    //AutoScroll is turned on, subscribe to collection changed events.
                    notifyCollection.CollectionChanged +=
                        listView.AutoScroll_ItemsCollectionChanged;
                }
                else
                {
                    //AutoScroll is turned off, unsubscribe from collection changed events.
                    notifyCollection.CollectionChanged -=
                        listView.AutoScroll_ItemsCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Event handler called only when the ItemCollection changes
        /// and if AutoScroll is enabled.
        /// </summary>
        /// <param name="sender">The ItemCollection.</param>
        /// <param name="e">Change event args.</param>
        private void AutoScroll_ItemsCollectionChanged(
            object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int count = Items.Count;
                ScrollIntoView(Items[count - 1]);
            }
        }

        /// <summary>
        /// Constructor a new listView.
        /// </summary>
        public ListViewWithAutoScroll()
        {
            //Subscribe to the AutoScroll property's items collection 
            //changed handler by default if AutoScroll is enabled by default.
            SubscribeToAutoScroll_ItemsCollectionChanged(
                this, (bool)AutoScrollProperty.DefaultMetadata.DefaultValue);
        }
    }
}
