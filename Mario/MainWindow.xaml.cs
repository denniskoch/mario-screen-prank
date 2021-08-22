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

        // Mario Sprite
        private const int MarioVelocity = 3; // Defines Mario's X movement per frame
        private AnimatedSprite _marioWalkingAnimation;
        private AnimatedSprite _marioStandingAnimation;
        private bool _marioIsWalking = true;

        // Luigi Sprite
        private const int LuigiVelocity = 4; // Defines Luigi's X movement per frame
        private AnimatedSprite _luigiWalkingAnimation;
        private AnimatedSprite _luigiStandingAnimation;
        private bool _luigiIsWalking = true;

        // Yoshi Sprite
        private const int YoshiVelocity = 5; // Defines Yoshi's X movement per frame
        private AnimatedSprite _yoshiWalkingAnimation;
        private AnimatedSprite _yoshiStandingAnimation;
        private bool _yoshiIsWalking = true;

        // Shell Sprite
        private const int ShellVelocity = 8;
        private AnimatedSprite _shellSpinningAnimation;
        private bool _shellIsActive = false;


        private int _screenWidth = 1024;
        private int _screenHeight = 768;

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
            _screenWidth =  (int)SystemParameters.WorkArea.Width;
            _screenHeight = (int)SystemParameters.WorkArea.Height;
            this.Height = 43;
            this.Width = _screenWidth;

            // Window Position
            this.Left = 0;
            this.Top = _screenHeight - this.Height;

            this.millisecondsBetweenFrames = 1000 / FPS;

            // Load Mario Frames
            _marioWalkingAnimation = new AnimatedSprite("mario", "walk", 6);
            _marioStandingAnimation = new AnimatedSprite("mario", "victory", 1);

            // Load Luigi Frames
            _luigiWalkingAnimation = new AnimatedSprite("luigi", "walk", 6);
            _luigiStandingAnimation = new AnimatedSprite("luigi", "victory", 1);

            // Load Yoshi Frames
            _yoshiWalkingAnimation = new AnimatedSprite("yoshi", "walk", 6);
            _yoshiStandingAnimation = new AnimatedSprite("yoshi", "stand", 1);

            // Load Shell Frames
            _shellSpinningAnimation = new AnimatedSprite("shell", "spinning", 8);

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
            contextMenu.MenuItems.Add("Launch Shell", contextMenuShell_Click);
            contextMenu.MenuItems.Add("Exit", contextMenuExit_Click);
            
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Visible = true;
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
                    MarioSprite.ImageSource = _marioWalkingAnimation.Frame();
                    double MarioPositionX = Canvas.GetLeft(Mario);

                    if (MarioPositionX < (_screenWidth + Mario.Width))
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
                    MarioSprite.ImageSource = _marioStandingAnimation.Frame();
                }

                // Update Luigi
                if (_luigiIsWalking)
                {
                    LuigiSprite.ImageSource = _luigiWalkingAnimation.Frame();
                    double LuigiPositionX = Canvas.GetLeft(Luigi);

                    if (LuigiPositionX < (_screenWidth + Luigi.Width))
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
                    LuigiSprite.ImageSource = _luigiStandingAnimation.Frame();
                }

                // Update Yoshi
                if (_yoshiIsWalking)
                {
                    YoshiSprite.ImageSource = _yoshiWalkingAnimation.Frame();
                    double YoshiPositionX = Canvas.GetLeft(Yoshi);

                    if (YoshiPositionX < (_screenWidth + Yoshi.Width))
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
                    YoshiSprite.ImageSource = _yoshiStandingAnimation.Frame();
                }

                if (_shellIsActive)
                {
                    ShellSprite.ImageSource = _shellSpinningAnimation.Frame();

                    double ShellPositionX = Canvas.GetLeft(Shell);

                    if (ShellPositionX < (_screenWidth + Shell.Width))
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
