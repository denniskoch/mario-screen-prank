using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Mario
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int FPS = 20;

        // Mario Sprite Constants
        private const int MarioVelocity = 3; // Defines Mario's X movement per frame
        private const int MarioNumWalkFrames = 6; // Number of frame in Mario's walk animation
        BitmapImage[] _marioWalkFrames = new BitmapImage[MarioNumWalkFrames];
        BitmapImage _marioStandingFrame;

        private int _marioWalkCurrentFrame = 0;
        private bool _marioIsWalking = true;

        private const int LuigiVelocity = 4; // Defines Mario's X movement per frame
        private const int LuigiNumWalkFrames = 6; // Number of frame in Mario's walk animation
        BitmapImage[] _luigiWalkFrames = new BitmapImage[LuigiNumWalkFrames];
        BitmapImage _luigiStandingFrame;

        // Luigi's animation will start on 2nd frame just so he's slightly out of step with mario... to keep things interesting
        private int _luigiWalkCurrentFrame = 1;
        private bool _luigiIsWalking = true;

        private const int YoshiVelocity = 5; // Defines Mario's X movement per frame
        private const int YoshiNumWalkFrames = 6; // Number of frame in Mario's walk animation
        BitmapImage[] _yoshiWalkFrames = new BitmapImage[YoshiNumWalkFrames];
        BitmapImage _yoshiStandingFrame;

        private int _yoshiWalkCurrentFrame = 0;
        private bool _yoshiIsWalking = true;

        private const int ShellVelocity = 8;
        private const int ShellNumSpinningFrames = 8;
        BitmapImage[] _shellSpinningFrames = new BitmapImage[ShellNumSpinningFrames];

        private int _shellSpinningCurrentFrame = 0;
        private bool _shellIsActive = false;

        private int screenWidth = 1024;
        private int screenHeight = 768;

        private long last = 0;

        private long millisecondsBetweenFrames;

        private NotifyIcon notifyIcon;

        private System.Windows.Forms.ContextMenu contextMenu;

        private Random random;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {

            InitializeComponent();

            random = new Random(Guid.NewGuid().GetHashCode());

            // Window Dimensions
            screenWidth =  (int)SystemParameters.WorkArea.Width;
            screenHeight = (int)SystemParameters.WorkArea.Height;
            this.Height = 43;
            this.Width = screenWidth;

            // Window Position
            this.Left = 0;
            this.Top = screenHeight - this.Height;

            this.millisecondsBetweenFrames = 1000 / FPS;

            // Load Mario Frames
            _marioWalkFrames = loadFrames("mario", "walk", MarioNumWalkFrames);
            _marioStandingFrame = new BitmapImage(new Uri(@"pack://application:,,,/Mario;component/Resources/sprites/mario/victory/0.png"));

            // Load Luigi Frames
            _luigiWalkFrames = loadFrames("luigi", "walk", LuigiNumWalkFrames);
            _luigiStandingFrame = new BitmapImage(new Uri(@"pack://application:,,,/Mario;component/Resources/sprites/luigi/victory/0.png"));

            // Load Yoshi Frames
            _yoshiWalkFrames = loadFrames("yoshi", "walk", YoshiNumWalkFrames);
            _yoshiStandingFrame = new BitmapImage(new Uri(@"pack://application:,,,/Mario;component/Resources/sprites/yoshi/stand/0.png"));

            // Load Shell Frames
            _shellSpinningFrames = loadFrames("shell", "spinning", ShellNumSpinningFrames);

            // Set the initial start positions of our friends, spaced out so that they're not on top of each other
            Canvas.SetLeft(this.Mario, (-1 * Mario.Width));
            Canvas.SetLeft(this.Luigi, (-2 * Luigi.Width));
            Canvas.SetLeft(this.Yoshi, (-3 * Yoshi.Width));
            Canvas.SetLeft(this.Shell, (-3 * Shell.Width));

            // Create timer to refresh/animate screen
            var frameTimer = new DispatcherTimer();
            frameTimer.Interval = TimeSpan.FromMilliseconds(10);
            frameTimer.Start();
            frameTimer.Tick += frameTimerTick;

            // Create NotifyIcon in SysTray
            notifyIcon = new NotifyIcon();
            System.IO.Stream iconStream = System.Windows.Application.GetResourceStream(new Uri(@"pack://application:,,,/Mario;component/Resources/mushroom.ico")).Stream;
            notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            iconStream.Dispose();
            
            contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add("Shell", contextMenuShell_Click);
            contextMenu.MenuItems.Add("Exit", contextMenuExit_Click);
            
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Visible = true;
        }

        private BitmapImage[] loadFrames(string name, string action, int numberOfFrames)
        {
            BitmapImage[] frames = new BitmapImage[numberOfFrames];
            String resourcePath;

            for (int i = 0; i < numberOfFrames; i++)
            {
                resourcePath = String.Format("pack://application:,,,/Mario;component/Resources/sprites/{0}/{1}/{2}.png", name, action, i);
                frames[i] = new BitmapImage(new Uri(resourcePath));
            }

            return frames;
        }

        /// <summary>
        /// Handle even when someone clicks on Exit from the systray context menu
        /// </summary>
        private void contextMenuExit_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void contextMenuShell_Click(object sender, EventArgs e)
        {
            if (! _shellIsActive)
            {
                _shellIsActive = true;  
            }
        }

        /// <summary>
        /// Timer tick event which will update the current image in the animation sequence for each of the sprites, as well as their position
        /// </summary>
        private void frameTimerTick(object sender, EventArgs e)
        {

            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long elapsedTime = now - this.last;

            if (elapsedTime >= millisecondsBetweenFrames)
            {
                // Update Mario
                if (_marioIsWalking)
                {
                    this._marioWalkCurrentFrame = (this._marioWalkCurrentFrame + 1 + MarioNumWalkFrames) % MarioNumWalkFrames;
                    MarioSprite.ImageSource = this._marioWalkFrames[this._marioWalkCurrentFrame];
                    double MarioPositionX = Canvas.GetLeft(Mario);

                    if (MarioPositionX < (screenWidth + Mario.Width))
                    {
                        MarioPositionX += MarioVelocity;
                    }
                    else
                    {
                        MarioPositionX = (-1 * Mario.Width);
                    }

                    Canvas.SetLeft(this.Mario, MarioPositionX);
                }
                else
                {
                    MarioSprite.ImageSource = _marioStandingFrame;
                }

                // Update Luigi
                if (_luigiIsWalking)
                {
                    this._luigiWalkCurrentFrame = (this._luigiWalkCurrentFrame + 1 + LuigiNumWalkFrames) % LuigiNumWalkFrames;
                    LuigiSprite.ImageSource = this._luigiWalkFrames[this._luigiWalkCurrentFrame];
                    double LuigiPositionX = Canvas.GetLeft(Luigi);

                    if (LuigiPositionX < (screenWidth + Luigi.Width))
                    {
                        LuigiPositionX += LuigiVelocity;
                    }
                    else
                    {
                        LuigiPositionX = (-1 * Luigi.Width);
                    }

                    Canvas.SetLeft(this.Luigi, LuigiPositionX);
                }
                else
                {
                    LuigiSprite.ImageSource = _luigiStandingFrame;
                }

                // Update Yoshi
                if (_yoshiIsWalking)
                {
                    this._yoshiWalkCurrentFrame = (this._yoshiWalkCurrentFrame + 1 + YoshiNumWalkFrames) % YoshiNumWalkFrames;
                    YoshiSprite.ImageSource = this._yoshiWalkFrames[this._yoshiWalkCurrentFrame];
                    double YoshiPositionX = Canvas.GetLeft(Yoshi);

                    if (YoshiPositionX < (screenWidth + Yoshi.Width))
                    {
                        YoshiPositionX += YoshiVelocity;
                    }
                    else
                    {
                        YoshiPositionX = (-1 * Yoshi.Width);
                    }

                    Canvas.SetLeft(this.Yoshi, YoshiPositionX);
                }
                else
                {
                    YoshiSprite.ImageSource = _yoshiStandingFrame;
                }

                if (_shellIsActive)
                {
                    this._shellSpinningCurrentFrame = (this._shellSpinningCurrentFrame + 1 + ShellNumSpinningFrames) % ShellNumSpinningFrames;
                    ShellSprite.ImageSource = this._shellSpinningFrames[this._shellSpinningCurrentFrame];

                    double ShellPositionX = Canvas.GetLeft(Shell);

                    if (ShellPositionX < (screenWidth + Shell.Width))
                    {
                        ShellPositionX += ShellVelocity;
                    }
                    else
                    {
                        ShellPositionX = (-1 * Shell.Width);
                        _shellIsActive = false;
                    }

                    Canvas.SetLeft(this.Shell, ShellPositionX);
                }
                
                // Time of Last Update
                this.last = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }

        }

        /// <summary>
        /// Handle event when someone clicks on Mario
        /// </summary>
        private void mario_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_marioIsWalking)
            {
                _marioIsWalking = false;
            }
            else
            {
                _marioIsWalking = true;
            }
        }

        /// <summary>
        /// Handle event when someone clicks on Luigi
        /// </summary>
        private void luigi_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_luigiIsWalking)
            {
                _luigiIsWalking = false;
            }
            else
            {
                _luigiIsWalking = true;
            }
        }

        /// <summary>
        /// Hanlde event when someone clicks on Luigi
        /// </summary>
        private void yoshi_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_yoshiIsWalking)
            {
                _yoshiIsWalking = false;
            }
            else
            {
                _yoshiIsWalking = true;
            }
        }

        /// <summary>
        /// Hanlde event when someone clicks on Luigi
        /// </summary>
        private void shell_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _shellIsActive = false;
            double ShellPositionX = (-1 * Shell.Width);
            Canvas.SetLeft(this.Shell, ShellPositionX);
        }
    }
}
