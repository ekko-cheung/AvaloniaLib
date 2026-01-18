using System.Collections;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace AvaloniaLib.Controls
{
    public partial class AutoScrollLogList : UserControl
    {
        public static readonly StyledProperty<IEnumerable?> ItemsProperty =
            AvaloniaProperty.Register<AutoScrollLogList, IEnumerable?>(nameof(Items));
        
        public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
            AvaloniaProperty.Register<AutoScrollLogList, IDataTemplate?>(nameof(ItemTemplate));
        
        public static readonly StyledProperty<bool> AutoScrollProperty =
            AvaloniaProperty.Register<AutoScrollLogList, bool>(nameof(AutoScroll), true);

        private INotifyCollectionChanged? _currentCollection;
        private readonly ListBox? _listBox;

        public IEnumerable? Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public IDataTemplate? ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public bool AutoScroll
        {
            get => GetValue(AutoScrollProperty);
            set => SetValue(AutoScrollProperty, value);
        }

        public AutoScrollLogList()
        {
            InitializeComponent();

            // Find the ListBox (PART)
            _listBox = this.FindControl<ListBox>("PART_ListBox");

            // 监听 Items 属性变化，处理集合订阅与初始滚动
            this.GetObservable(ItemsProperty).Subscribe(OnItemsPropertyChanged);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnItemsPropertyChanged(IEnumerable? newItems)
        {
            if (_currentCollection != null)
            {
                _currentCollection.CollectionChanged -= CollectionChanged;
                _currentCollection = null;
            }
            
            if (newItems is INotifyCollectionChanged incc)
            {
                _currentCollection = incc;
                _currentCollection.CollectionChanged += CollectionChanged;
            }
            
            if (AutoScroll)
            {
                ScrollToEndDeferred();
            }
        }

        private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (!AutoScroll)
                return;
            
            if (e is { Action: NotifyCollectionChangedAction.Add, NewItems.Count: > 0 })
            {
                var lastNew = e.NewItems[^1];
                ScrollIntoViewDeferred(lastNew);
            }
            else
            {
                ScrollToEndDeferred();
            }
        }

        private void ScrollToEndDeferred()
        {
            var last = GetLastItemFromItems();
            if (last != null)
                ScrollIntoViewDeferred(last);
        }

        private object? GetLastItemFromItems()
        {
            switch (Items)
            {
                case IList { Count: > 0 } list:
                    return list[^1];
                case null:
                    return null;
            }

            object? last = null;
            foreach (var it in Items)
                last = it;
            return last;

        }

        private void ScrollIntoViewDeferred(object? item)
        {
            if (_listBox == null || item == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    _listBox.ScrollIntoView(item);
                }
                catch
                {
                    // ignore
                }
            }, DispatcherPriority.Background);
        }
    }
}