using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        private SoundPlayer soundpl;
        private SoundPlayer soundend;
        private int lvl = 0;

        public Form1()
        {
            InitializeComponent();
            soundpl = new SoundPlayer("eat.wav");
            soundend = new SoundPlayer("gameOver.wav");
            new Settings();
            
            gameTimer.Interval = 100 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();
            StartGame();
        }

        private void StartGame()
        {
            lblGameOver.Visible = false;
            new Settings();
            lbllvl.Text = (Settings.Speed-1).ToString();
            gameTimer.Interval = 100 / Settings.Speed;
            System.Diagnostics.Debug.WriteLine(Settings.Speed);
            Snake.Clear();
            Circle head = new Circle();
            head.X = 10;
            head.Y = 10;
            Snake.Add(head);


            lblScore.Text = Settings.Score.ToString();
            GenerateFood();
        }

        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle();
            food.X = random.Next(0, maxXPos);
            food.Y = random.Next(0, maxYPos);

        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            if (Settings.GameOver == true)
            {
                if(Input.keyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.keyPressed(Keys.Right) && Settings.direction != Direction.Left )
                {
                    Settings.direction = Direction.Right;
                }
                else if (Input.keyPressed(Keys.Left) && Settings.direction != Direction.Right)
                {
                    Settings.direction = Direction.Left;
                }
                else if (Input.keyPressed(Keys.Up) && Settings.direction != Direction.Down)
                {
                    Settings.direction = Direction.Up;
                }
                else if (Input.keyPressed(Keys.Down) && Settings.direction != Direction.Up)
                {
                    Settings.direction = Direction.Down;
                }
                MovePlayer();
            }
            pbCanvas.Invalidate();
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            if (Settings.GameOver == false)
            {
                Brush snakeColour;
                for (int i = 0; i < Snake.Count; i++)
                {
                    if(i == 0)
                    {
                        snakeColour = Brushes.Black;
                    }
                    else
                    {
                        snakeColour = Brushes.Green;
                    }
                    canvas.FillEllipse(snakeColour,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));


                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(food.X * Settings.Width,
                                      food.Y * Settings.Height,
                                      Settings.Width, Settings.Height));
                }
            }
            else
            {
                string gameOver = "Game over \nYour final score is: " + Settings.Score + "\nPress Enter to try again";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
            }
        }

        private void MovePlayer()
        {
            for(int i = Snake.Count - 1; i >= 0; i--)
            {
                if(i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;

                    }
                    int maxXPos = pbCanvas.Size.Width / Settings.Width;
                    int maxYPos = pbCanvas.Size.Height / Settings.Height;

                    if(Snake[i].X < 0 || Snake[i].Y < 0 || Snake[i].X >= maxXPos || Snake[i].Y >= maxYPos)
                    {
                        Die();
                    }

                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if(Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            Die();
                        }
                    }

                    if(Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }
                }

          
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }
        private void Eat()
        {
            Circle food = new Circle();
            soundpl.Play();
            food.X = Snake[Snake.Count - 1].X;
            food.Y = Snake[Snake.Count - 1].Y;

            Snake.Add(food);

            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();
            GenerateFood();
            Settings.lvl += 100;
            System.Diagnostics.Debug.WriteLine(Settings.lvl);
            if (Settings.lvl >= 2000)
            {
                Settings.Speed += 1;
                Settings.lvl = 0;
                gameTimer.Interval = 100 / Settings.Speed;
                lbllvl.Text = (Settings.Speed-1).ToString();
            }
        }
        private void Die()
        {
            Settings.GameOver = true;
            soundend.Play();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }
    }
}
