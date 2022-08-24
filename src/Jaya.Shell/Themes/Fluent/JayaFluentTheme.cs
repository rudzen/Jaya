using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

#nullable enable

namespace Jaya.Shell.Themes
{
    /// <summary>
    /// Includes the fluent theme in an application.
    /// </summary>
    public class JayaFluentTheme : AvaloniaObject, IStyle, IResourceProvider
    {
        private readonly Uri _baseUri;
        private Styles _fluentDark = new();
        private Styles _fluentLight = new();
        private Styles _sharedStyles = new();
        private Styles _densityStyles = new();
        private bool _isLoading;
        private IStyle? _loaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="JayaFluentTheme"/> class.
        /// </summary>
        /// <param name="baseUri">The base URL for the XAML context.</param>
        public JayaFluentTheme(Uri baseUri)
        {
            _baseUri = baseUri;
            InitStyles(baseUri);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JayaFluentTheme"/> class.
        /// </summary>
        /// <param name="serviceProvider">The XAML service provider.</param>
        public JayaFluentTheme(IServiceProvider serviceProvider)
        {
            _baseUri = ((IUriContext)serviceProvider.GetService(typeof(IUriContext))).BaseUri;
            InitStyles(_baseUri);
        }

        public static readonly StyledProperty<FluentThemeMode> ModeProperty =
            AvaloniaProperty.Register<JayaFluentTheme, FluentThemeMode>(nameof(Mode));

        public static readonly StyledProperty<DensityStyle> DensityStyleProperty =
            AvaloniaProperty.Register<JayaFluentTheme, DensityStyle>(nameof(DensityStyle));

        /// <summary>
        /// Gets or sets the mode of the fluent theme (light, dark).
        /// </summary>
        public FluentThemeMode Mode
        {
            get => GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        /// <summary>
        /// Gets or sets the density style of the fluent theme (normal, compact).
        /// </summary>
        public DensityStyle DensityStyle
        {
            get => GetValue(DensityStyleProperty);
            set => SetValue(DensityStyleProperty, value);
        }
        
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);
            
            if (_loaded is null)
            {
                // If style wasn't yet loaded, no need to change children styles,
                // it will be applied later in Loaded getter.
                return;
            }
            
            if (change.Property == ModeProperty)
            {
                Styles modeStyles = (Mode == FluentThemeMode.Dark)
                    ? _fluentDark
                    : _fluentLight
                ;

                for (int i = 0; i < modeStyles.Count; i++)
                {
                    //if ((Loaded as Styles).Count)
                    (Loaded as Styles)![i + 1] = modeStyles[i];
                }
                /*if (Mode == FluentThemeMode.Dark)
                {
                    
                    (Loaded as Styles)![1] = _fluentDark[0];
                    (Loaded as Styles)![2] = _fluentDark[1];
                }
                else
                {
                    (Loaded as Styles)![1] = _fluentLight[0];
                    (Loaded as Styles)![2] = _fluentLight[1];
                }*/
            }

            if (change.Property == DensityStyleProperty)
            {
                Action<int> action = (DensityStyle == DensityStyle.Compact)
                    ? index => (Loaded as Styles)!.Add(_densityStyles[index])
                    : index => (Loaded as Styles)!.Remove(_densityStyles[index])
                ;

                for (int i = 0; i < _densityStyles.Count; i++)
                {
                    action(i);
                }
                
                /*if (DensityStyle == DensityStyle.Compact)
                {
                    (Loaded as Styles)!.Add(_densityStyles[0]);
                }
                else if (DensityStyle == DensityStyle.Normal)
                {
                    (Loaded as Styles)!.Remove(_densityStyles[0]);
                }*/
            }
        }

        public IResourceHost? Owner => (Loaded as IResourceProvider)?.Owner;

        /// <summary>
        /// Gets the loaded style.
        /// </summary>
        public IStyle Loaded
        {
            get
            {
                if (_loaded == null)
                {
                    _isLoading = true;

                    Styles loadedStyles = new Styles() { _sharedStyles };

                    Styles modeStyles = (Mode == FluentThemeMode.Dark)
                        ? _fluentDark
                        : _fluentLight
                    ;
                    
                    for (int i = 0; i < modeStyles.Count; i++)
                    {
                        loadedStyles.Add(modeStyles[i]);
                    }
                    
                    /*if (Mode == FluentThemeMode.Light)
                    {
                        _loaded = new Styles() { _sharedStyles, _fluentLight[0], _fluentLight[1] };
                    }
                    else if (Mode == FluentThemeMode.Dark)
                    {
                        _loaded = new Styles() { _sharedStyles, _fluentDark[0], _fluentDark[1] };
                    }*/

                    if (DensityStyle == DensityStyle.Compact)
                    {
                        for (int i = 0; i < _densityStyles.Count; i++)
                        {
                            (loadedStyles as Styles)!.Add(_densityStyles[i]);
                        }
                        //(_loaded as Styles)!.Add(_densityStyles[0]);
                    }

                    _loaded = loadedStyles;
                    _isLoading = false;
                }

                return _loaded!;
            }
        }

        bool IResourceNode.HasResources => (Loaded as IResourceProvider)?.HasResources ?? false;

        IReadOnlyList<IStyle> IStyle.Children => _loaded?.Children ?? Array.Empty<IStyle>();

        public event EventHandler OwnerChanged
        {
            add
            {
                if (Loaded is IResourceProvider rp)
                {
                    rp.OwnerChanged += value;
                }
            }
            remove
            {
                if (Loaded is IResourceProvider rp)
                {
                    rp.OwnerChanged -= value;
                }
            }
        }

        public SelectorMatchResult TryAttach(IStyleable target, IStyleHost? host) => Loaded.TryAttach(target, host);

        public bool TryGetResource(object key, out object? value)
        {
            if (!_isLoading && Loaded is IResourceProvider p)
            {
                return p.TryGetResource(key, out value);
            }

            value = null;
            return false;
        }

        void IResourceProvider.AddOwner(IResourceHost owner) => (Loaded as IResourceProvider)?.AddOwner(owner);
        void IResourceProvider.RemoveOwner(IResourceHost owner) => (Loaded as IResourceProvider)?.RemoveOwner(owner);

        private void InitStyles(Uri baseUri)
        {
            _sharedStyles = new Styles
            {
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/AccentColors.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/Base.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/Controls/FluentControls.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Jaya.Shell/Themes/Fluent/JayaFluent.axaml")
                }
            };

            _fluentLight = GetColorSchemeStyles(baseUri, "Light");

            _fluentDark = GetColorSchemeStyles(baseUri, "Dark");
            
            _densityStyles = new Styles
            {
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/DensityStyles/Compact.xaml")
                }
            };
        }

        private Styles GetColorSchemeStyles(Uri baseUri, string schemeName)
            => new Styles
            {
                new StyleInclude(baseUri)
                {
                    Source = new Uri($"avares://Avalonia.Themes.Fluent/Accents/Base{schemeName}.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri($"avares://Avalonia.Themes.Fluent/Accents/FluentControlResources{schemeName}.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri($"avares://Jaya.Shell/Themes/Fluent/ColorSchemes/Fluent{schemeName}.axaml")
                }
            };
    }
}

/*using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;


namespace Jaya.Shell.Themes
{
    public class JayaFluentTheme : FluentTheme
    {
        public JayaFluentTheme(Uri baseUri) : base(baseUri)
        {
        }


        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged<T>(change);

            if (change.Property == ModeProperty)
            {
                if (Mode == FluentThemeMode.Dark)
                {
                    (Loaded as Styles)![1] = _fluentDark[0];
                    (Loaded as Styles)![2] = _fluentDark[1];
                }
                else
                {
                    (Loaded as Styles)![1] = _fluentLight[0];
                    (Loaded as Styles)![2] = _fluentLight[1];
                }
            }
        }
    }
}*/