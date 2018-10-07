using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Arkanoid
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        enum BallCorners
        {
            TopLeft,
            TopCenter,
            TopRight,
            LeftCenter,
            RightCenter,
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        double _degree = 90;
        bool _racketIsMoving;
        Point _racketPosTopLeft;
        Point _ballPosTopLeft;
        double _racketLimitLeft, _racketLimitRight;
        Point _ballLimitTopLeft, _ballLimitBottomRight, _ballLimitTopRight, _ballLimitBottomLeft;
        Vector _racketShift;
        double _baseRacketShift = 4;
        double _baseBallShift = 6;
        Vector _ballShift;
        Vector _ballNormalizedShift;
        bool _inPlay;
        DispatcherTimer _timer = new DispatcherTimer();
        bool _processing;
        double _gameOver;
        Random _rnd = new Random();
        List<Rectangle> _bricks = new List<Rectangle>();


        Dictionary<BallCorners, Point> _ballCorners;

        public GameWindow()
        {
            InitializeComponent();
            InitGame();
        }

        void InitGame()
        {
            Fill();
            _racketShift.X = 0;
            _racketShift.Y = 0;
            _ballShift.X = 0;
            _ballShift.Y = _baseBallShift;
            _racketLimitLeft = 0;
            _racketLimitRight = canvasGame.Width - racket.Width;
            _racketPosTopLeft.X = canvasGame.Width / 2 - racket.Width / 2;
            _racketPosTopLeft.Y = canvasGame.Height - racket.Height;
            Canvas.SetLeft(racket, _racketPosTopLeft.X);
            Canvas.SetTop(racket, _racketPosTopLeft.Y);
            _ballPosTopLeft.X = _racketPosTopLeft.X + racket.Width / 2 - ball.Width / 2;
            _ballPosTopLeft.Y = _racketPosTopLeft.Y - ball.Height;
            Canvas.SetLeft(ball, _ballPosTopLeft.X);
            Canvas.SetTop(ball, _ballPosTopLeft.Y);
            _ballLimitTopLeft.X = borderLeft.Width;
            _ballLimitTopLeft.Y = borderTop.Height;
            _ballLimitBottomRight.X = canvasGame.Width - borderRight.Width - ball.Width;
            _ballLimitBottomRight.Y = Canvas.GetTop(racket) - ball.Height;
            _ballLimitTopRight.X = canvasGame.Width - borderRight.Width - ball.Width;
            _ballLimitTopRight.Y = borderTop.Height;
            _ballLimitBottomLeft.X = borderLeft.Width;
            _ballLimitBottomLeft.Y = Canvas.GetTop(racket) - ball.Height;
            _gameOver = canvasGame.Height;
            
                _ballCorners = new Dictionary<BallCorners, Point>
                          {
                              {
                                  BallCorners.TopLeft, new Point
                                      (ball.Width / 2 * (1 - Math.Cos(45 * Math.PI / 180)),
                                       ball.Height / 2 * (1 - Math.Sin(45 * Math.PI / 180)))
                              },
                              {
                                  BallCorners.TopRight, new Point
                                      (ball.Width / 2 + ball.Width / 2 * Math.Cos(45 * Math.PI / 180),
                                       ball.Height / 2 * (1 - Math.Sin(45 * Math.PI / 180)))
                              },
                              {
                                  BallCorners.BottomLeft, new Point
                                      (ball.Width / 2 * (1 - Math.Cos(45 * Math.PI / 180)),
                                       ball.Height / 2 + ball.Height / 2 * Math.Sin(45 * Math.PI / 180))
                              },
                              {
                                  BallCorners.BottomRight, new Point
                                      (ball.Width / 2 + ball.Width / 2 * Math.Cos(45 * Math.PI / 180),
                                       ball.Height / 2 + ball.Height / 2 * Math.Sin(45 * Math.PI / 180))
                              },
                              {
                                  BallCorners.TopCenter, new Point(ball.Width/2 , 0)
                              },
                              {
                                  BallCorners.LeftCenter, new Point(0 , ball.Height / 2)
                              },
                              {
                                  BallCorners.RightCenter, new Point(ball.Width, ball.Height / 2)
                              },
                              {
                                  BallCorners.BottomCenter, new Point(ball.Width / 2, ball.Height)
                              }
                          };

            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Start();
        }

        void Fill()
        {
            var redBrick1 = (Rectangle)FindResource("BrickRed");
            Canvas.SetLeft(redBrick1, 80); Canvas.SetTop(redBrick1, 200);
            var cyanBrick1 = (Rectangle)FindResource("BrickCyan");
            Canvas.SetLeft(cyanBrick1, 80); Canvas.SetTop(cyanBrick1, 260);
            var blueBrick1 = (Rectangle)FindResource("BrickBlue");
            Canvas.SetLeft(blueBrick1, 80); Canvas.SetTop(blueBrick1, 320);

            var yellowBrick2 = (Rectangle)FindResource("BrickYellow");
            Canvas.SetLeft(yellowBrick2, 290); Canvas.SetTop(yellowBrick2, 200);
            var greenBrick2 = (Rectangle)FindResource("BrickGreen");
            Canvas.SetLeft(greenBrick2, 290); Canvas.SetTop(greenBrick2, 260);
            var magentaBrick2 = (Rectangle)FindResource("BrickMagenta");
            Canvas.SetLeft(magentaBrick2, 290); Canvas.SetTop(magentaBrick2, 320);

            var redBrick3 = (Rectangle)FindResource("BrickRed");
            Canvas.SetLeft(redBrick3, 500); Canvas.SetTop(redBrick3, 200);
            var cyanBrick3 = (Rectangle)FindResource("BrickCyan");
            Canvas.SetLeft(cyanBrick3, 500); Canvas.SetTop(cyanBrick3, 260);
            var blueBrick3 = (Rectangle)FindResource("BrickBlue");
            Canvas.SetLeft(blueBrick3, 500); Canvas.SetTop(blueBrick3, 320);

            var yellowBrick4 = (Rectangle)FindResource("BrickYellow");
            Canvas.SetLeft(yellowBrick4, 710); Canvas.SetTop(yellowBrick4, 200);
            var greenBrick4 = (Rectangle)FindResource("BrickGreen");
            Canvas.SetLeft(greenBrick4, 710); Canvas.SetTop(greenBrick4, 260);
            var magentaBrick4 = (Rectangle)FindResource("BrickMagenta");
            Canvas.SetLeft(magentaBrick4, 710); Canvas.SetTop(magentaBrick4, 320);

            var redBrick5 = (Rectangle)FindResource("BrickRed");
            Canvas.SetLeft(redBrick5, 920); Canvas.SetTop(redBrick5, 200);
            var cyanBrick5 = (Rectangle)FindResource("BrickCyan");
            Canvas.SetLeft(cyanBrick5, 920); Canvas.SetTop(cyanBrick5, 260);
            var blueBrick5 = (Rectangle)FindResource("BrickBlue");
            Canvas.SetLeft(blueBrick5, 920); Canvas.SetTop(blueBrick5, 320);

            var yellowBrick6 = (Rectangle)FindResource("BrickYellow");
            Canvas.SetLeft(yellowBrick6, 1130); Canvas.SetTop(yellowBrick6, 200);
            var greenBrick6 = (Rectangle)FindResource("BrickGreen");
            Canvas.SetLeft(greenBrick6, 1130); Canvas.SetTop(greenBrick6, 260);
            var magentaBrick6 = (Rectangle)FindResource("BrickMagenta");
            Canvas.SetLeft(magentaBrick6, 1130); Canvas.SetTop(magentaBrick6, 320);

            _bricks.Add(redBrick1);
            _bricks.Add(redBrick3);
            _bricks.Add(redBrick5);
            _bricks.Add(cyanBrick1);
            _bricks.Add(cyanBrick3);
            _bricks.Add(cyanBrick5);
            _bricks.Add(blueBrick1);
            _bricks.Add(blueBrick3);
            _bricks.Add(blueBrick5);
            _bricks.Add(yellowBrick2);
            _bricks.Add(yellowBrick4);
            _bricks.Add(yellowBrick6);
            _bricks.Add(greenBrick2);
            _bricks.Add(greenBrick4);
            _bricks.Add(greenBrick6);
            _bricks.Add(magentaBrick2);
            _bricks.Add(magentaBrick4);
            _bricks.Add(magentaBrick6);

            foreach (var brick in _bricks)
            {
                canvasGame.Children.Add(brick);
            }
        }

        void MoveRacket(object sender, EventArgs e)
        {
            MovingRacket();
            if (!_inPlay) MovingGluedBall();
        }
        
        void MovingRacket()
        {
            _racketPosTopLeft.X += _racketShift.X;
            if (_racketPosTopLeft.X < _racketLimitLeft) _racketPosTopLeft.X = _racketLimitLeft;
            else if (_racketPosTopLeft.X > _racketLimitRight) _racketPosTopLeft.X = _racketLimitRight;
            Canvas.SetLeft(racket, _racketPosTopLeft.X);
        }

        void MovingGluedBall()
        {
            _ballPosTopLeft.X = (_racketPosTopLeft.X + racket.Width / 2 - ball.Width / 2);
            Canvas.SetLeft(ball, _ballPosTopLeft.X);
        }

        void MoveBall(object sender, EventArgs e)
        {
            if (_processing) return;
            _processing = true;
            MovingBall();
        }

        void MovingBall()
        {
            var tempBallShift = new Vector
                                   {
                                       X = _baseBallShift * _ballNormalizedShift.X,
                                       Y = _baseBallShift * _ballNormalizedShift.Y
                                   };

            var tempBallPos = Vector.Add(tempBallShift, _ballPosTopLeft);

            if (tempBallPos.X < _ballLimitTopLeft.X)
            {
                tempBallPos = CalculateShift(tempBallPos, _ballLimitTopLeft, _ballLimitBottomLeft);
            }

            else if (tempBallPos.X > _ballLimitTopRight.X)
            {
                tempBallPos = CalculateShift(tempBallPos, _ballLimitTopRight, _ballLimitBottomRight);
            }

            else if (tempBallPos.Y < _ballLimitTopLeft.Y)
            {
                tempBallPos = CalculateShift(tempBallPos, _ballLimitTopLeft, _ballLimitTopRight);
            }

            else if (tempBallPos.Y > _ballLimitBottomLeft.Y)
            {
                if (_ballPosTopLeft.Y > _gameOver)
                {
                    _timer.Stop();
                    var result = MessageBox.Show("Вы проиграли!", "Сочувствуем!", MessageBoxButton.OK);
                    if (result == MessageBoxResult.OK) Close();
                }
                if (CollisionCheck(_ballCorners, _ballPosTopLeft, tempBallShift, racket, out var lim1, out var lim2))
                    tempBallPos = CalculateShift(tempBallPos, lim1, lim2);
            }

            else
            {
                foreach (var brick in _bricks)
                {
                    if (!CollisionCheck
                            (_ballCorners, _ballPosTopLeft, tempBallShift, brick, out var lim1, out var lim2)) continue;
                    
                    tempBallPos = CalculateShift(tempBallPos, lim1, lim2);
                    break;
                }
            }

            bool CollisionCheck(Dictionary<BallCorners, Point> ballCorners, Point bPosTl, Vector tBShift, Rectangle obstruction, out Point lim1, out Point lim2)
            {
                var isHit = false;
                var bCorners = new Dictionary<BallCorners, Point>();
                foreach (var kVp in ballCorners)
                {
                    var bCorner = new Point { X = kVp.Value.X + bPosTl.X, Y = kVp.Value.Y + bPosTl.Y };
                    
                    bCorners.Add(kVp.Key, bCorner);
                }
                
                var comp = new Point();
                lim1 = new Point();
                lim2 = new Point();
                
                switch (tBShift)
                {
                    case Vector shift when (shift.Y < 0 && shift.X < 0):

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopCenter],
                                 Point.Add(bCorners[BallCorners.TopCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                comp = ballCorners[BallCorners.TopCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.LeftCenter],
                                 Point.Add(bCorners[BallCorners.LeftCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                comp = ballCorners[BallCorners.LeftCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopRight],
                                 Point.Add(bCorners[BallCorners.TopRight], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree += 15;
                                comp = ballCorners[BallCorners.TopRight];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomLeft],
                                 Point.Add(bCorners[BallCorners.BottomLeft], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree -= 15;
                                comp = ballCorners[BallCorners.BottomLeft];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopLeft],
                                 Point.Add(bCorners[BallCorners.TopLeft], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree += 135;
                                comp = ballCorners[BallCorners.TopLeft];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopLeft],
                                 Point.Add(bCorners[BallCorners.TopLeft], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree -= 135;
                                comp = ballCorners[BallCorners.TopLeft];
                            }

                        break;

                    case Vector shift when (shift.Y < 0 && shift.X > 0):

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopCenter],
                                 Point.Add(bCorners[BallCorners.TopCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                comp = ballCorners[BallCorners.TopCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.RightCenter],
                                 Point.Add(bCorners[BallCorners.RightCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                comp = ballCorners[BallCorners.RightCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopLeft],
                                 Point.Add(bCorners[BallCorners.TopLeft], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree -= 15;
                                comp = ballCorners[BallCorners.TopLeft];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomRight],
                                 Point.Add(bCorners[BallCorners.BottomRight], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree += 15;
                                comp = ballCorners[BallCorners.BottomRight];
                                break;
                            }

                            isHit = CheckLinesCross
                            (bCorners[BallCorners.TopRight],
                             Point.Add(bCorners[BallCorners.TopRight], tBShift),
                             lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                             lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree -= 135;
                                comp = ballCorners[BallCorners.TopRight];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopRight],
                                 Point.Add(bCorners[BallCorners.TopRight], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree += 135;
                                comp = ballCorners[BallCorners.TopRight];
                            }

                        break;

                    case Vector shift when (shift.Y > 0 && shift.X < 0):

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomCenter],
                                 Point.Add(bCorners[BallCorners.BottomCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)));
                            if (isHit)
                            {
                                comp = ballCorners[BallCorners.BottomCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.LeftCenter],
                                 Point.Add(bCorners[BallCorners.LeftCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                comp = ballCorners[BallCorners.LeftCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomRight],
                                 Point.Add(bCorners[BallCorners.BottomRight], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)));
                            if (isHit)
                            {
                                _degree -= 15;
                                comp = ballCorners[BallCorners.BottomRight];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopLeft],
                                 Point.Add(bCorners[BallCorners.TopLeft], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree += 15;
                                comp = ballCorners[BallCorners.TopLeft];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomLeft],
                                 Point.Add(bCorners[BallCorners.BottomLeft], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)));
                            if (isHit)
                            {
                                _degree += 135;
                                comp = ballCorners[BallCorners.BottomLeft];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomLeft],
                                 Point.Add(bCorners[BallCorners.BottomLeft], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree -= 135;
                                comp = ballCorners[BallCorners.BottomLeft];
                            }

                        break;

                    case Vector shift when (shift.Y > 0 && shift.X > 0):

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomCenter],
                                 Point.Add(bCorners[BallCorners.BottomCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)));
                            if (isHit)
                            {
                                comp = ballCorners[BallCorners.BottomCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.RightCenter],
                                 Point.Add(bCorners[BallCorners.RightCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                comp = ballCorners[BallCorners.RightCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomLeft],
                                 Point.Add(bCorners[BallCorners.BottomLeft], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)));
                            if (isHit)
                            {
                                _degree += 15;
                                comp = ballCorners[BallCorners.BottomLeft];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopRight],
                                 Point.Add(bCorners[BallCorners.TopRight], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree -= 15;
                                comp = ballCorners[BallCorners.TopRight];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomRight],
                                 Point.Add(bCorners[BallCorners.BottomRight], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)));
                            if (isHit)
                            {
                                _degree -= 135;
                                comp = ballCorners[BallCorners.BottomRight];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomRight],
                                 Point.Add(bCorners[BallCorners.BottomRight], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree += 135;
                                comp = ballCorners[BallCorners.BottomRight];
                            }

                        break;

                    case Vector shift when ((shift.Y < 0.0 || shift.Y > 0.0 ) && shift.X == 0.0):

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopCenter],
                                 Point.Add(bCorners[BallCorners.TopCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                comp = ballCorners[BallCorners.TopCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomCenter],
                                 Point.Add(bCorners[BallCorners.BottomCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)));
                            if (isHit)
                            {
                                comp = ballCorners[BallCorners.BottomCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopRight],
                                 Point.Add(bCorners[BallCorners.TopRight], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree -= 15;
                                comp = ballCorners[BallCorners.TopRight];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.TopLeft],
                                 Point.Add(bCorners[BallCorners.TopLeft], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree += 15;
                                comp = ballCorners[BallCorners.TopLeft];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomRight],
                                 Point.Add(bCorners[BallCorners.BottomRight], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)));
                            if (isHit)
                            {
                                _degree += 15;
                                comp = ballCorners[BallCorners.BottomRight];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.BottomLeft],
                                 Point.Add(bCorners[BallCorners.BottomLeft], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction)),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction)));
                            if (isHit)
                            {
                                _degree -= 15;
                                comp = ballCorners[BallCorners.BottomLeft];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.LeftCenter],
                                 Point.Add(bCorners[BallCorners.LeftCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree -= 45;
                                comp = ballCorners[BallCorners.LeftCenter];
                                break;
                            }

                            isHit = CheckLinesCross
                                (bCorners[BallCorners.RightCenter],
                                 Point.Add(bCorners[BallCorners.RightCenter], tBShift),
                                 lim1 = new Point(Canvas.GetLeft(obstruction), Canvas.GetTop(obstruction) + obstruction.Height),
                                 lim2 = new Point(Canvas.GetLeft(obstruction) + obstruction.Width, Canvas.GetTop(obstruction) + obstruction.Height));
                            if (isHit)
                            {
                                _degree += 45;
                                comp = ballCorners[BallCorners.RightCenter];
                            }

                        break;
                }

                if (isHit && !Equals(obstruction, racket))
                {
                    obstruction.Visibility = Visibility.Collapsed;
                    _bricks.Remove(obstruction);
                    if (_bricks.Count == 0)
                    {
                        _timer.Stop();
                        var result = MessageBox.Show("Вы победили!", "Поздравляем!", MessageBoxButton.OK);
                        if (result == MessageBoxResult.OK) Close();
                    }
                }

                lim1.X -= comp.X;
                lim1.Y -= comp.Y;
                
                lim2.X -= comp.X;
                lim2.Y -= comp.Y;
                return isHit;
            }

            bool CheckLinesCross(Point start, Point end, Point lim1, Point lim2)
            {
                var a = end - start;
                var b = lim1 - lim2;
                var c = start - lim1;

                var alphaNumerator = b.Y * c.X - b.X * c.Y;
                var alphaDenominator = a.Y * b.X - a.X * b.Y;
                var betaNumerator = a.X * c.Y - a.Y * c.X;
                var betaDenominator = a.Y * b.X - a.X * b.Y;

                var doIntersect = true;

                if (alphaDenominator == 0.0 || betaDenominator == 0.0)
                {
                    doIntersect = false;
                }
                else
                {
                    if (alphaDenominator > 0)
                    {
                        if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
                        {
                            doIntersect = false;

                        }
                    }
                    else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
                    {
                        doIntersect = false;
                    }

                    if (doIntersect && betaDenominator > 0)
                    {
                        if (betaNumerator < 0 || betaNumerator > betaDenominator)
                        {
                            doIntersect = false;
                        }
                    }
                    else if (betaNumerator > 0 || betaNumerator < betaDenominator)
                    {
                        doIntersect = false;
                    }
                }

                return doIntersect;
            }

            Point CalculateShift(Point tBallPos, Point lim1, Point lim2)
            {
                _degree = _degree + _rnd.Next(-20, 21);
                if (_degree >= 360) _degree -= 360;
                else if (_degree <= -360) _degree += 360;

                if (_degree < 10 && _degree > 350) _degree = (_rnd.Next(2) == 0)? 20: 340;
                else if (_degree > -10 && _degree < -350) _degree = (_rnd.Next(2) == 0) ? -20 : -340;
                else if (_degree > 170 && _degree < 190) _degree = (_rnd.Next(2) == 0) ? 160 : 200;
                else if (_degree < -170 && _degree > -190) _degree = (_rnd.Next(2) == 0) ? -160 : -200;

                var oldBnShift = new Vector
                                    {
                                        X = _ballNormalizedShift.X,
                                        Y = _ballNormalizedShift.Y
                                    };

                _ballNormalizedShift.X = Math.Cos(_degree * Math.PI / 180);
                _ballNormalizedShift.Y = -Math.Sin(_degree * Math.PI / 180);

                if (lim1.X == lim2.X)
                {
                    tBallPos.X = lim1.X - _ballNormalizedShift.X / oldBnShift.X * (tBallPos.X - lim1.X);
                    _ballNormalizedShift.X = -_ballNormalizedShift.X;
                    _degree = 180 - _degree;
                }

                else if (lim1.Y == lim2.Y)
                {
                    tBallPos.Y = lim1.Y - _ballNormalizedShift.Y / oldBnShift.Y * (tBallPos.Y - lim1.Y);
                    _ballNormalizedShift.Y = -_ballNormalizedShift.Y;
                    _degree = 360 - _degree;
                }

                return tBallPos;
            }

            Canvas.SetLeft(ball, tempBallPos.X);
            Canvas.SetTop(ball, tempBallPos.Y);

            _ballPosTopLeft.X = tempBallPos.X;
            _ballPosTopLeft.Y = tempBallPos.Y;
            _processing = false;
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key;

            switch (key)
            {
                case Key.Left:
                    _racketShift.X = -_baseRacketShift;
                    if (!_racketIsMoving)
                    {
                        _racketIsMoving = true;
                        _timer.Tick += MoveRacket;
                    }
                    break;
                case Key.Right:
                    _racketShift.X = _baseRacketShift;
                    if (!_racketIsMoving)
                    {
                        _racketIsMoving = true;
                        _timer.Tick += MoveRacket;
                    }
                    break;
                case Key.Space:
                    if (_inPlay) break;
                    _inPlay = true;
                    _ballPosTopLeft.X = Canvas.GetLeft(ball);
                    _degree = (_racketShift.X == 0.0) ? 90 : (_racketShift.X > 0.0)? 45: 135;
                    _ballNormalizedShift.X = Math.Cos(_degree * Math.PI / 180);
                    _ballNormalizedShift.Y = -Math.Sin(_degree * Math.PI / 180);
                    _timer.Tick += MoveBall;
                    break;
            }
        }

        void Window_KeyUp(object sender, KeyEventArgs e)
        {
            var key = e.Key;
            if (key != Key.Left && key != Key.Right) return;
            _racketShift.X = 0;
            _timer.Tick -= MoveRacket;
            _racketIsMoving = false;
        }
    }
}