using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BeamCalculator.Helpers;


public class ObservableCollectionWithItemNotify<T> : ObservableCollection<T> where T : INotifyPropertyChanged
{
    public ObservableCollectionWithItemNotify()
    {
        this.CollectionChanged += items_CollectionChenged;
    }

    public ObservableCollectionWithItemNotify(IEnumerable<T> collection) : base(collection)
    {
        this.CollectionChanged += items_CollectionChenged;
        foreach (INotifyPropertyChanged item in collection)
        {
            item.PropertyChanged += item_PropertyChanged;
        }
    }

    private void items_CollectionChenged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e != null)
        {
            if (e.OldItems != null)
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;

            if (e.NewItems != null)
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;
        }
    }

    private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var replace = new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Replace,
            sender,
            sender,
            this.Items.IndexOf((T)sender));

        this.OnCollectionChanged(replace);
    }
}
