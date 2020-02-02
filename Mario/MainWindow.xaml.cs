using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace Mario
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int FPS = 20;

        // Mario
        private const int MARIO_VELOCITY = 3;
        private const int MARIO_WALK_FRAMES = 6;
        BitmapImage[] MarioWalkFrames = new BitmapImage[MARIO_WALK_FRAMES];
        private int MarioWalkCurrentFrame = 0;

        private int ScreenWidth = 1024;
        private int ScreenHeight = 768;

        long last = 0;

        long millisecondsBetweenFrames;

        public MainWindow()
        {
            InitializeComponent();

            ScreenWidth =  (int)SystemParameters.WorkArea.Width;
            ScreenHeight = (int)SystemParameters.WorkArea.Height;
            
            // Window Dimensions
            this.Height = 43;
            this.Width = ScreenWidth;

            // Window Position
            this.Left = 0;
            this.Top = ScreenHeight - this.Height;

            this.millisecondsBetweenFrames = 1000 / FPS;

            // Load Mario Walk Frames
            MarioWalkFrames[0] = new BitmapImage(new Uri(@"pack://application:,,,/Mario;component/Resources/sprites/mario/walk/2.png"));
            MarioWalkFrames[1] = new BitmapImage(new Uri(@"pack://application:,,,/Mario;component/Resources/sprites/mario/walk/2.png"));
            MarioWalkFrames[2] = new BitmapImage(new Uri(@"pack://application:,,,/Mario;component/Resources/sprites/mario/walk/0.png"));
            MarioWalkFrames[3] = new BitmapImage(new Uri(@"pack://application:,,,/Mario;component/Resources/sprites/mario/walk/0.png"));
            MarioWalkFrames[4] = new BitmapImage(new Uri(@"pack://application:,,,/Mario;component/Resources/sprites/mario/walk/1.png"));
            MarioWalkFrames[5] = new BitmapImage(new Uri(@"pack://application:,,,/Mario;component/Resources/sprites/mario/walk/1.png"));
            
            // Mario Initially Off Screen
            Canvas.SetLeft(this.Mario, (-1 * Mario.Width));

            // Create Timer
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Start();
            timer.Tick += tick;

        }

        private void tick(object sender, EventArgs e)
        {

            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long elapsedTime = now - this.last;

            if (elapsedTime >= millisecondsBetweenFrames)
            {
                this.MarioWalkCurrentFrame = (this.MarioWalkCurrentFrame + 1 + MARIO_WALK_FRAMES) % MARIO_WALK_FRAMES;

                MarioSprite.ImageSource = this.MarioWalkFrames[this.MarioWalkCurrentFrame];

                double MarioPositionX = Canvas.GetLeft(Mario);

                if (MarioPositionX < (ScreenWidth + Mario.Width))
                {
                    MarioPositionX += MARIO_VELOCITY;
                }
                else
                {
                    MarioPositionX = (-1 * Mario.Width);
                }

                Canvas.SetLeft(this.Mario, MarioPositionX);

                this.last = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }

        }
    }
}
