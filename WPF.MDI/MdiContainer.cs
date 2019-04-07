using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

///
namespace WPF.MDI 
{
    [ContentProperty("Children")]
    public class MdiContainer : UserControl
    {
        /// <summary>
        /// Type of theme to use.
        /// </summary>
        public enum ThemeType 
        { 
            /// <summary>
            /// Generic Visual Studio designer theme.
            /// </summary>
            Generic,
            /// <summary>
            /// Windows XP blue theme.
            /// </summary>
            Luna,
            /// <summary>
            /// Windows Vista and 7 theme.
            /// </summary>
            Aero  
        } 

        #region Static Members

        private static ResourceDictionary _currentResourceDictionary;

        #endregion
        
        #region Dependency Properties

        /// <summary>
        /// Identifies the WPF.MDI.MdiContainer.Theme dependency property.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiContainer.Theme property.</returns>
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(ThemeType), typeof(MdiContainer),
            new UIPropertyMetadata(ThemeType.Aero, ThemeValueChanged));

        #endregion

        #region Property Accessors

        /// <summary>
        /// Gets or sets the container theme.
        /// The default is determined by the operating system.
        /// This is a dependency property.
        /// </summary>
        /// <value>The container theme.</value>
        public ThemeType Theme
        {
            get { return (ThemeType)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the child elements.
        /// </summary>
        /// <value>The child elements.</value>
        public ObservableCollection<MdiChild> Children { get; set; }

        private readonly Canvas _windowCanvas;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MdiContainer"/> class.
        /// </summary>
        public MdiContainer()
        {
            Background = Brushes.DarkGray;

            Children = new ObservableCollection<MdiChild>();
            Children.CollectionChanged += Children_CollectionChanged;

            Content = new ScrollViewer
            {
                Content = _windowCanvas = new Canvas(),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            ThemeValueChanged(this, Environment.OSVersion.Version.Major == 5 
                ? new DependencyPropertyChangedEventArgs(ThemeProperty, Theme, ThemeType.Luna) 
                : new DependencyPropertyChangedEventArgs(ThemeProperty, Theme, ThemeType.Aero));

            Loaded += MdiContainer_Loaded;
            SizeChanged += MdiContainer_SizeChanged;
        }

        #region Container Events

        /// <summary>
        /// Handles the Loaded event of the MdiContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MdiContainer_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
            {
// ReSharper disable PossibleNullReferenceException
                Window.GetWindow(this).Activated += MdiContainer_Activated;
                Window.GetWindow(this).Deactivated += MdiContainer_Deactivated;
// ReSharper restore PossibleNullReferenceException
            }

            _windowCanvas.Width = _windowCanvas.ActualWidth;
            _windowCanvas.Height = _windowCanvas.ActualHeight;

            _windowCanvas.VerticalAlignment = VerticalAlignment.Top;
            _windowCanvas.HorizontalAlignment = HorizontalAlignment.Left;

            InvalidateSize();
        }

        /// <summary>
        /// Handles the Activated event of the MdiContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MdiContainer_Activated(object sender, EventArgs e)
        {
            if (_windowCanvas.Children.Count == 0)
                return;

            ((MdiChild)_windowCanvas.Children[_windowCanvas.Children.Count - 1]).Focused = true;
        }

        /// <summary>
        /// Handles the Deactivated event of the MdiContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MdiContainer_Deactivated(object sender, EventArgs e)
        {
            if (_windowCanvas.Children.Count == 0)
                return;

            ((MdiChild)_windowCanvas.Children[_windowCanvas.Children.Count - 1]).Focused = false;
        }

        /// <summary>
        /// Handles the SizeChanged event of the MdiContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void MdiContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_windowCanvas.Children.Count == 0 || ((MdiChild)_windowCanvas.Children[_windowCanvas.Children.Count - 1]).WindowState != WindowState.Maximized)
                return;

            ((MdiChild)_windowCanvas.Children[_windowCanvas.Children.Count - 1]).Width = ActualWidth;
            ((MdiChild)_windowCanvas.Children[_windowCanvas.Children.Count - 1]).Height = ActualHeight;
        }

        #endregion

        #region ObservableCollection Events

        /// <summary>
        /// Handles the CollectionChanged event of the Children control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var mdiChild = Children[e.NewStartingIndex];

                        Canvas.SetLeft(mdiChild, (_windowCanvas.Children.Count * 25) + 2);
                        Canvas.SetTop(mdiChild, (_windowCanvas.Children.Count * 25) + 2);

                        if (_windowCanvas.Children.Count == 0 || ((MdiChild)_windowCanvas.Children[_windowCanvas.Children.Count - 1]).WindowState != WindowState.Maximized)
                        {
                            _windowCanvas.Children.Add(mdiChild);

                            mdiChild.Loaded += (s, a) => Focus(mdiChild);
                        }
                        else
                            _windowCanvas.Children.Insert(_windowCanvas.Children.Count - 1, mdiChild);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        _windowCanvas.Children.Remove((MdiChild)e.OldItems[0]);

                        if (_windowCanvas.Children.Count > 0)
                            Focus((MdiChild)_windowCanvas.Children[_windowCanvas.Children.Count - 1]);
                    }
                    break;
            }
        }

        #endregion

        #region IAddChild Members

        /// <summary>
        /// Adds a child object.
        /// </summary>
        /// <param name="value">The child object to add.</param>
        public new void AddChild(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.GetType() != typeof(MdiChild))
                throw new ArgumentException("Value must be an MdiChild control");

            Children.Add((MdiChild)value);
        }

        /// <summary>
        /// Adds the text content of a node to the object.
        /// </summary>
        /// <param name="text">The text to add to the object.</param>
        public new void AddText(string text)
        {

        }

        #endregion

        /// <summary>
        /// Focuses a child and brings it into view.
        /// FUTURE: Look into how ZIndex works.
        /// </summary>
        /// <param name="mdiChild">The MDI child.</param>
        internal void Focus(MdiChild mdiChild)
        {
            for (var i = 0; i < _windowCanvas.Children.Count; i++)
            {
                ((MdiChild) _windowCanvas.Children[i]).Focused =
                    (_windowCanvas.Children[i] == mdiChild && _windowCanvas.Children[i].GetType() == typeof (MdiChild));
            }

            _windowCanvas.Children.Remove(mdiChild);
            _windowCanvas.Children.Add(mdiChild);
        }

        /// <summary>
        /// Invalidates the size checking to see if the furthest
        /// child point exceeds the current height and width.
        /// </summary>
        internal void InvalidateSize()
        {
            var largestPoint = new Point(0, 0);

            for (int i = 0; i < _windowCanvas.Children.Count; i++)
            {
                var mdiChild = (MdiChild) _windowCanvas.Children[i];

                var farPosition = new Point(Canvas.GetLeft(mdiChild) + mdiChild.ActualWidth, Canvas.GetTop(mdiChild) + mdiChild.ActualHeight);

                if (farPosition.X > largestPoint.X)
                    largestPoint.X = farPosition.X;

                if (farPosition.Y > largestPoint.Y)
                    largestPoint.Y = farPosition.Y;
            }

            _windowCanvas.Width = largestPoint.X;
            _windowCanvas.Height = largestPoint.Y;
        }

        #region Dependency Property Events

        /// <summary>
        /// Dependency property event once the theme value has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ThemeValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var themeType = (ThemeType) e.NewValue;

            if (_currentResourceDictionary != null)
                Application.Current.Resources.MergedDictionaries.Remove(_currentResourceDictionary);

            switch (themeType)
            {
            case ThemeType.Luna:
                // ReSharper disable PossibleNullReferenceException
                Application.Current.Resources.MergedDictionaries.Add(_currentResourceDictionary = (ResourceDictionary) XamlReader.Load(
                    Application.GetResourceStream(new Uri(@"WPF.MDI;component/Themes/Luna.xaml", UriKind.Relative)).Stream));
                // ReSharper restore PossibleNullReferenceException
                break;

            case ThemeType.Aero:
                // ReSharper disable PossibleNullReferenceException
                Application.Current.Resources.MergedDictionaries.Add(_currentResourceDictionary = (ResourceDictionary) XamlReader.Load(
                    Application.GetResourceStream(new Uri(@"WPF.MDI;component/Themes/Aero.xaml", UriKind.Relative)).Stream));
                // ReSharper restore PossibleNullReferenceException
                break;
            }
        }

        #endregion
    }
}