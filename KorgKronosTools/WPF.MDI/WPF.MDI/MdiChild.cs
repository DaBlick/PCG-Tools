using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using WPF.MDI.Event;

namespace WPF.MDI
{
    [ContentProperty("Content")]
    public class MdiChild : Control
    {
        #region Dependency Properties

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.ContentProperty dependency property.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.ContentProperty property.</returns>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(MdiChild));

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.TitleProperty dependency property.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.TitleProperty property.</returns>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MdiChild));

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.IconProperty dependency property.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.IconProperty property.</returns>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(MdiChild));

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.ShowIconProperty dependency property.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.ShowIconProperty property.</returns>
        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.Register("ShowIcon", typeof(bool), typeof(MdiChild),
            new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.ResizableProperty dependency property.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.ResizableProperty property.</returns>
        public static readonly DependencyProperty ResizableProperty =
            DependencyProperty.Register("Resizable", typeof(bool), typeof(MdiChild),
            new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.FocusedProperty dependency property.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.FocusedProperty property.</returns>
        public static readonly DependencyProperty FocusedProperty =
            DependencyProperty.Register("Focused", typeof(bool), typeof(MdiChild),
            new UIPropertyMetadata(false, FocusedValueChanged));

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.MinimizeBoxProperty dependency property.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.MinimizeBoxProperty property.</returns>
        public static readonly DependencyProperty MinimizeBoxProperty =
            DependencyProperty.Register("MinimizeBox", typeof(bool), typeof(MdiChild),
            new UIPropertyMetadata(true, MinimizeBoxValueChanged));

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.MaximizeBoxProperty dependency property.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.MaximizeBoxProperty property.</returns>
        public static readonly DependencyProperty MaximizeBoxProperty =
            DependencyProperty.Register("MaximizeBox", typeof(bool), typeof(MdiChild),
            new UIPropertyMetadata(true, MaximizeBoxValueChanged));

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.WindowStateProperty dependency property.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.WindowStateProperty property.</returns>
        public static readonly DependencyProperty WindowStateProperty =
            DependencyProperty.Register("WindowState", typeof(WindowState), typeof(MdiChild),
            new UIPropertyMetadata(WindowState.Normal, WindowStateValueChanged));
        
        #endregion

        #region Depedency Events

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.ClosingEvent routed event.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.ClosingEvent routed event.</returns>
        public static readonly RoutedEvent ClosingEvent =
            EventManager.RegisterRoutedEvent("Closing", RoutingStrategy.Bubble, typeof(ClosingEventArgs), typeof(MdiChild));

        /// <summary>
        /// Identifies the WPF.MDI.MdiChild.ClosedEvent routed event.
        /// </summary>
        /// <returns>The identifier for the WPF.MDI.MdiChild.ClosedEvent routed event.</returns>
        public static readonly RoutedEvent ClosedEvent =
            EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventArgs), typeof(MdiChild));

        #endregion

        #region Property Accessors

        /// <summary>
        /// Gets or sets the content.
        /// This is a dependency property.
        /// </summary>
        /// <value>The content.</value>
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the window title.
        /// This is a dependency property.
        /// </summary>
        /// <value>The window title.</value>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the window icon.
        /// This is a dependency property.
        /// </summary>
        /// <value>The window icon.</value>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to [show the window icon].
        /// This is a dependency property.
        /// </summary>
        /// <value><c>true</c> if [show the window icon]; otherwise, <c>false</c>.</value>
        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the [window is resizable].
        /// This is a dependency property.
        /// </summary>
        /// <value><c>true</c> if [window is resizable]; otherwise, <c>false</c>.</value>
        public bool Resizable
        {
            get { return (bool)GetValue(ResizableProperty); }
            set { SetValue(ResizableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the [window is focused].
        /// This is a dependency property.
        /// </summary>
        /// <value><c>true</c> if [window is focused]; otherwise, <c>false</c>.</value>
        public bool Focused
        {
            get { return (bool)GetValue(FocusedProperty); }
            set { SetValue(FocusedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to [show the minimize box button].
        /// This is a dependency property.
        /// </summary>
        /// <value><c>true</c> if [show the minimize box button]; otherwise, <c>false</c>.</value>
        public bool MinimizeBox
        {
            get { return (bool)GetValue(MinimizeBoxProperty); }
            set { SetValue(MinimizeBoxProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to [show the maximize box button].
        /// This is a dependency property.
        /// </summary>
        /// <value><c>true</c> if [show the maximize box button]; otherwise, <c>false</c>.</value>
        public bool MaximizeBox
        {
            get { return (bool)GetValue(MaximizeBoxProperty); }
            set { SetValue(MaximizeBoxProperty, value); }
        }

        /// <summary>
        /// Gets or sets the state of the window.
        /// This is a dependency property.
        /// </summary>
        /// <value>The state of the window.</value>
        public WindowState WindowState
        {
            get { return (WindowState)GetValue(WindowStateProperty); }
            set { SetValue(WindowStateProperty, value); }
        }
        
        #endregion

        #region Event Accessors

        public event RoutedEventHandler Closing
        {
            add { AddHandler(ClosingEvent, value); }
            remove { RemoveHandler(ClosingEvent, value); }
        }

        public event RoutedEventHandler Closed
        {
            add { AddHandler(ClosedEvent, value); }
            remove { RemoveHandler(ClosedEvent, value); }
        }

        #endregion

        #region Member Declarations

        #region Top Buttons

        private Button _minimizeButton;

        private Button _maximizeButton;

        private Button _closeButton;

        #endregion

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        public MdiContainer Container { get; private set; }

        private Rect _originalDimension;

        #endregion

        /// <summary>
        /// Initializes the <see cref="MdiChild"/> class.
        /// </summary>
        static MdiChild()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MdiChild), new FrameworkPropertyMetadata(typeof(MdiChild)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MdiChild"/> class.
        /// </summary>
        public MdiChild()
        {
            Loaded += MdiChild_Loaded;
            GotFocus += MdiChild_GotFocus;
        }

        #region Control Events

        /// <summary>
        /// Handles the Loaded event of the MdiChild control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MdiChild_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement currentControl = this;

            while (currentControl != null && currentControl.GetType() != typeof(MdiContainer))
                currentControl = (FrameworkElement)currentControl.Parent;

            if (currentControl != null)
                Container = (MdiContainer)currentControl;
            else
                throw new ApplicationException("Unable to find MdiContainer parent.");
        }

        /// <summary>
        /// Handles the GotFocus event of the MdiChild control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MdiChild_GotFocus(object sender, RoutedEventArgs e)
        {
            Focus();
        }

        #endregion

        #region Control Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _minimizeButton = (Button)Template.FindName("MinimizeButton", this);
            _maximizeButton = (Button)Template.FindName("MaximizeButton", this);
            _closeButton = (Button)Template.FindName("CloseButton", this);

            if (_minimizeButton != null)
                _minimizeButton.Click += MinimizeButtonClick;

            if (_maximizeButton != null)
                _maximizeButton.Click += MaximizeButtonClick;

            if (_closeButton != null)
                _closeButton.Click += CloseButtonClick;

            var dragThumb = (Thumb)Template.FindName("DragThumb", this);

            if (dragThumb != null)
            {
                dragThumb.DragStarted += Thumb_DragStarted;
                dragThumb.DragDelta += DragThumbDragDelta;

                /* MK: Removed functionality
                dragThumb.MouseDoubleClick += (sender, e) =>
                {
                    if (WindowState == WindowState.Minimized)
                        MinimizeButtonClick(null, null);
                    else if (WindowState == WindowState.Normal)
                        MaximizeButtonClick(null, null);
                };
                 */
            }

            var resizeLeft = (Thumb)Template.FindName("ResizeLeft", this);
            var resizeTopLeft = (Thumb)Template.FindName("ResizeTopLeft", this);
            var resizeTop = (Thumb)Template.FindName("ResizeTop", this);
            var resizeTopRight = (Thumb)Template.FindName("ResizeTopRight", this);
            var resizeRight = (Thumb)Template.FindName("ResizeRight", this);
            var resizeBottomRight = (Thumb)Template.FindName("ResizeBottomRight", this);
            var resizeBottom = (Thumb)Template.FindName("ResizeBottom", this);
            var resizeBottomLeft = (Thumb)Template.FindName("ResizeBottomLeft", this);

            if (resizeLeft != null)
            {
                resizeLeft.DragStarted += Thumb_DragStarted;
                resizeLeft.DragDelta += ResizeLeft_DragDelta;
            }

            if (resizeTop != null)
            {
                resizeTop.DragStarted += Thumb_DragStarted;
                resizeTop.DragDelta += ResizeTop_DragDelta;
            }

            if (resizeRight != null)
            {
                resizeRight.DragStarted += Thumb_DragStarted;
                resizeRight.DragDelta += ResizeRight_DragDelta;
            }

            if (resizeBottom != null)
            {
                resizeBottom.DragStarted += Thumb_DragStarted;
                resizeBottom.DragDelta += ResizeBottom_DragDelta;
            }

            if (resizeTopLeft != null)
            {
                resizeTopLeft.DragStarted += Thumb_DragStarted;

                resizeTopLeft.DragDelta += (sender, e) =>
                {
                    ResizeTop_DragDelta(null, e);
                    ResizeLeft_DragDelta(null, e);

                    Container.InvalidateSize();
                };
            }

            if (resizeTopRight != null)
            {
                resizeTopRight.DragStarted += Thumb_DragStarted;

                resizeTopRight.DragDelta += (sender, e) =>
                {
                    ResizeTop_DragDelta(null, e);
                    ResizeRight_DragDelta(null, e);

                    Container.InvalidateSize();
                };
            }

            if (resizeBottomRight != null)
            {
                resizeBottomRight.DragStarted += Thumb_DragStarted;

                resizeBottomRight.DragDelta += (sender, e) =>
                {
                    ResizeBottom_DragDelta(null, e);
                    ResizeRight_DragDelta(null, e);

                    Container.InvalidateSize();
                };
            }

            if (resizeBottomLeft != null)
            {
                resizeBottomLeft.DragStarted += Thumb_DragStarted;

                resizeBottomLeft.DragDelta += (sender, e) =>
                {
                    ResizeBottom_DragDelta(null, e);
                    ResizeLeft_DragDelta(null, e);

                    Container.InvalidateSize();
                };
            }

            MinimizeBoxValueChanged(this, new DependencyPropertyChangedEventArgs(MinimizeBoxProperty, true, MinimizeBox));
            MaximizeBoxValueChanged(this, new DependencyPropertyChangedEventArgs(MaximizeBoxProperty, true, MaximizeBox));
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            Focused = true;
        }

        #endregion

        #region Top Button Events

        /// <summary>
        /// Handles the Click event of the minimizeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Minimized ? WindowState.Normal : WindowState.Minimized;
        }

        /// <summary>
        /// Handles the Click event of the maximizeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        /// <summary>
        /// Handles the Click event of the closeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            var eventArgs = new ClosingEventArgs(ClosingEvent);
            RaiseEvent(eventArgs);

            if (!eventArgs.Handled) //MK: Added for preventing closing
            {
                if (eventArgs.Cancel)
                    return;

                Close();

                RaiseEvent(new RoutedEventArgs(ClosedEvent));
            }
        }

        #endregion

        #region Thumb Events

        /// <summary>
        /// Handles the DragStarted event of the Thumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (!Focused)
                Focused = true;
        }

        /// <summary>
        /// Handles the DragDelta event of the ResizeLeft control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width - e.HorizontalChange < MinWidth)
                return;

            Width -= e.HorizontalChange;
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);

            if (sender != null)
                Container.InvalidateSize();
        }

        /// <summary>
        /// Handles the DragDelta event of the ResizeTop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeTop_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Height - e.VerticalChange < MinHeight)
                return;

            Height -= e.VerticalChange;
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

            if (sender != null)
                Container.InvalidateSize();
        }

        /// <summary>
        /// Handles the DragDelta event of the ResizeRight control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange < MinWidth)
                return;

            Width += e.HorizontalChange;

            if (sender != null)
                Container.InvalidateSize();
        }

        /// <summary>
        /// Handles the DragDelta event of the ResizeBottom control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeBottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Height + e.VerticalChange < MinHeight)
                return;

            Height += e.VerticalChange;

            if (sender != null)
                Container.InvalidateSize();
        }

        #endregion

        #region Control Drag Event

        /// <summary>
        /// Handles the DragDelta event of the dragThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void DragThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                return;

            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

            Container.InvalidateSize();
        }

        #endregion

        #region Dependency Property Events

        /// <summary>
        /// Dependency property event once the focused value has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void FocusedValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == (bool)e.OldValue)
                return;

            var mdiChild = (MdiChild)sender;
            var focused = (bool)e.NewValue;

            mdiChild.RaiseEvent(focused ? new RoutedEventArgs(GotFocusEvent, mdiChild) : new RoutedEventArgs(LostFocusEvent, mdiChild));
        }

        /// <summary>
        /// Dependency property event once the minimize box value has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MinimizeBoxValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var mdiChild = (MdiChild)sender;
            var visible = (bool)e.NewValue;

            if (visible)
            {
                var maximizeVisible = true;

                if (mdiChild._maximizeButton != null)
                    maximizeVisible = mdiChild._maximizeButton.Visibility == Visibility.Visible;

                if (mdiChild._minimizeButton != null)
                    mdiChild._minimizeButton.IsEnabled = true;

                if (!maximizeVisible)
                {
                    if (mdiChild._maximizeButton != null)
                        mdiChild._minimizeButton.Visibility = Visibility.Visible;

                    if (mdiChild._maximizeButton != null)
                        mdiChild._maximizeButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                bool maximizeEnabled = true;

                if (mdiChild._maximizeButton != null)
                    maximizeEnabled = mdiChild._maximizeButton.IsEnabled;

                if (mdiChild._minimizeButton != null)
                    mdiChild._minimizeButton.IsEnabled = false;

                if (!maximizeEnabled)
                {
                    if (mdiChild._minimizeButton != null)
                        mdiChild._minimizeButton.Visibility = Visibility.Hidden;

                    if (mdiChild._maximizeButton != null)
                        mdiChild._maximizeButton.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Dependency property event once the maximize box value has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MaximizeBoxValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var mdiChild = (MdiChild)sender;
            var visible = (bool)e.NewValue;

            if (visible)
            {
                bool minimizeVisible = true;

                if (mdiChild._minimizeButton != null)
                    minimizeVisible = mdiChild._minimizeButton.Visibility == Visibility.Visible;

                if (mdiChild._maximizeButton != null)
                    mdiChild._maximizeButton.IsEnabled = true;

                if (!minimizeVisible)
                {
                    if (mdiChild._maximizeButton != null)
                        mdiChild._minimizeButton.Visibility = Visibility.Visible;

                    if (mdiChild._maximizeButton != null)
                        mdiChild._maximizeButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                bool minimizeEnabled = true;

                if (mdiChild._minimizeButton != null)
                    minimizeEnabled = mdiChild._minimizeButton.IsEnabled;

                if (mdiChild._maximizeButton != null)
                    mdiChild._maximizeButton.IsEnabled = false;

                if (!minimizeEnabled)
                {
                    if (mdiChild._maximizeButton != null)
                        mdiChild._minimizeButton.Visibility = Visibility.Hidden;

                    if (mdiChild._maximizeButton != null)
                        mdiChild._maximizeButton.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Dependency property event once the windows state value has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void WindowStateValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var mdiChild = (MdiChild)sender;
            var previousWindowState = (WindowState)e.OldValue;
            var windowState = (WindowState)e.NewValue;

            if (mdiChild.Container == null)
                return;

            if (previousWindowState == WindowState.Maximized)
            {
                foreach (var pcgWindow in mdiChild.Container.Children)
                {
                    if (pcgWindow != mdiChild)
                        pcgWindow.IsEnabled = true;
                }

                ((ScrollViewer)mdiChild.Container.Content).HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                ((ScrollViewer)mdiChild.Container.Content).VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }

            switch (windowState)
            {
                case WindowState.Normal:
                    {
                        Canvas.SetLeft(mdiChild, mdiChild._originalDimension.X);
                        Canvas.SetTop(mdiChild, mdiChild._originalDimension.Y);

                        mdiChild.Width = mdiChild._originalDimension.Width;
                        mdiChild.Height = mdiChild._originalDimension.Height;
                    }
                    break;
                case WindowState.Minimized:
                    {
                        if (previousWindowState == WindowState.Normal)
                            mdiChild._originalDimension = new Rect(Canvas.GetLeft(mdiChild), Canvas.GetTop(mdiChild), mdiChild.ActualWidth, mdiChild.ActualHeight);

                        var minimizedWindows = mdiChild.Container.Children.Count(t => t != mdiChild && t.WindowState == WindowState.Minimized);

                        Canvas.SetLeft(mdiChild, minimizedWindows * 160);
                        Canvas.SetTop(mdiChild, mdiChild.Container.ActualHeight - 29);

                        mdiChild.Width = 160;
                        mdiChild.Height = 129; //MK
                    }
                    break;
                case WindowState.Maximized:
                    {
                        if (previousWindowState == WindowState.Normal)
                            mdiChild._originalDimension = new Rect(Canvas.GetLeft(mdiChild), Canvas.GetTop(mdiChild), mdiChild.ActualWidth, mdiChild.ActualHeight);

                        foreach (var pcgWindow in mdiChild.Container.Children)
                        {
                            if (pcgWindow != mdiChild)
                                pcgWindow.IsEnabled = false;
                        }

                        Canvas.SetLeft(mdiChild, 0);
                        Canvas.SetTop(mdiChild, 0);

                        mdiChild.Width = mdiChild.Container.ActualWidth;
                        mdiChild.Height = mdiChild.Container.ActualHeight;

                        ((ScrollViewer)mdiChild.Container.Content).HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        ((ScrollViewer)mdiChild.Container.Content).VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    }
                    break;
            }
        }
        
        #endregion

        /// <summary>
        /// Set focus to the child window and brings into view.
        /// </summary>
        public new void Focus()
        {
            if (Container != null)
            {
                Container.Focus(this);
            }
        }

        /// <summary>
        /// Manually closes the child window.
        /// </summary>
        public void Close()
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;

            Container.Children.Remove(this);
        }
    }
}