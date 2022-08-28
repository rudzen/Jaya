using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace Jaya.Shell.Views
{
    public partial class AddressBarView : UserControl
    {
        //AutoCompleteBox? _addressEditField = null;
        List<IAvaloniaObject> _excludeFromFocusGrab = new List<IAvaloniaObject>();
        IInputElement? _lastFocused = null;

        public AddressBarView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            //_addressEditField = this.Find<AutoCompleteBox>("AddressEditField");
            var addressBar = this.Find<ToggleButton>("AddressBar");
            _excludeFromFocusGrab.Add(addressBar);

            var addressEditField = this.Find<AutoCompleteBox>("AddressEditField");
            _excludeFromFocusGrab.Add(addressEditField);
            
            var searchBar = this.Find<ToggleButton>("SearchBar");
            _excludeFromFocusGrab.Add(searchBar);

            var searchField = this.Find<AutoCompleteBox>("SearchField");
            _excludeFromFocusGrab.Add(searchField);
            
            SetFieldEvents(addressEditField, addressBar);
            SetFieldEvents(searchField, searchBar);
        }

        private void SetFieldEvents(AutoCompleteBox field, ToggleButton bar)
        {
            //IInputElement? lastFocused = null;
            
            field.PropertyChanged += (s, e) =>
            {
                //Console.WriteLine($"PROPERTY: '{e.Property.Name}'");
                if (e.Property == AutoCompleteBox.IsEnabledProperty)
                {
                    if ((e.NewValue is bool newVal) && newVal)
                    {
                        IInputElement? newLastFocused = FocusManager.Instance?.Current;
                        if
                        (
                            (newLastFocused == null) || 
                            (
                                (!_excludeFromFocusGrab.Contains((IAvaloniaObject)newLastFocused)) ||
                                (
                                    (newLastFocused is IControl ctrl) &&
                                    (ctrl.TemplatedParent != null) &&
                                    (!_excludeFromFocusGrab.Contains(ctrl.TemplatedParent))
                                )
                            )
                        )
                        {
                            _lastFocused = newLastFocused;
                        }
                        
                        Dispatcher.UIThread.Post(() =>
                        {
                            field.Focus();
                        }, DispatcherPriority.Layout);
                    }
                    else
                        _lastFocused?.Focus();
                }
            };

            field.KeyDown += (s, e) =>
            {
                bool isSubmitKey = (e.Key == Key.Enter) || (e.Key == Key.Return);
                bool isCancelKey = (e.Key == Key.Escape);
                if (isSubmitKey || isCancelKey)
                {
                    e.Handled = true;
                    bar.IsChecked = false;
                }
            };
        }
    }
}
