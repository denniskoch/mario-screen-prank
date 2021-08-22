using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario
{
    class AnimatedSprite
    {
        private string _name;
        private string _action;
        private int _currentFrame;
        private int _totalNumberFrames;
        private BitmapImage[] _frames;
        private BitmapImage _frame;

        public AnimatedSprite(string name, string action, int totalNumberFrames)
        {
            _name = name;
            _action = action;
            _currentFrame = 0;
            _totalNumberFrames = totalNumberFrames;
            LoadFrames();
        }

        private void LoadFrames()
        {
            _frames = new BitmapImage[_totalNumberFrames];
            string resourcePath;

            for (int i = 0; i < _totalNumberFrames; i++)
            {
                resourcePath = String.Format("pack://application:,,,/Mario;component/Resources/sprites/{0}/{1}/{2}.png", _name, _action, i);
                _frames[i] = new BitmapImage(new Uri(resourcePath));
            }
        }
        public BitmapImage Frame()
        {
            _frame = _frames[_currentFrame];
            this._currentFrame = (this._currentFrame + 1 + _totalNumberFrames) % _totalNumberFrames;

            return _frame;
        }

    }

}
