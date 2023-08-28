using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace LearnXaml
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //тімер щоб для кожного "тіку" змінювалися позиції об'єкту
        DispatcherTimer gameTimer  = new DispatcherTimer();

        //швидкістть зміни для гравця, виносимо в зміну тому що в момент коли він наприклад торкається землі треба швидкість зменшити до 0 щоб він не рухався
        int speed = 5;

        //точка до якої може підстрибнути фігурка
        int force = 25;

        //щоб розуміти чи граець стрибає чи ні
        bool jumping;

        //щоб розуміти чи гра закінчилась
        bool gameOver;

        //хітбокси, щоб встановити межі наших об'єктів і визначити коли вони торкаються одне одного
        Rect playerHitBox;
        Rect obstacleHitBox;
        Rect groundHitBox;

        //щоб зробити перешкоди різного розміру\висоти
        int[] obstacles = { 300, 310, 320, 315, 350 };
        int[] obstacleWidths = { 70, 100, 50, 40, 80 };

        Random random = new Random();

        //щоб рахувати рахунок
        int scoreNum = 0;
        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Focus(); // жля чого це
            //gameTimer.Start();

            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(10);

            StartGame();
        }

        private double x = 900;
        private void GameEngine(object sender, EventArgs e)
        {

            //Debug.WriteLine("GameEngine tick");
            Canvas.SetLeft(Bacground, Canvas.GetLeft(Bacground) - 3);
            Canvas.SetLeft(Bacground2, Canvas.GetLeft(Bacground2) - 3);

            if(Canvas.GetLeft(Bacground) < -1262)
            {
                Canvas.SetLeft(Bacground, Canvas.GetLeft(Bacground2) + Bacground2.Width);
            }

            if (Canvas.GetLeft(Bacground2) < -1262)
            {
                Canvas.SetLeft(Bacground2, Canvas.GetLeft(Bacground) + Bacground.Width);
            }


            Canvas.SetTop(player, Canvas.GetTop(player) + speed);
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - 12);

            /*if (Canvas.GetLeft(obstacle) < 0)
            {
                Canvas.SetLeft(obstacle, x - 12);
                x = Canvas.GetLeft(obstacle);
            }*/


            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            obstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width, obstacle.Height);
            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);


            if (playerHitBox.IntersectsWith(groundHitBox))
            {
                speed = 0;

                //jumping = false;
                //Debug.WriteLine("Jumping is set to false");
            }



            if (jumping == true)
            {
                speed = -9;
                force -= 1;
            }
            else if (!jumping && !playerHitBox.IntersectsWith(groundHitBox))
            {
                speed = 12;
            }

            if (force < 0)
            {
                jumping = false;
                force = 25;
            }



            if (Canvas.GetLeft(obstacle) < -50)
            {
                Canvas.SetLeft(obstacle, 950);

                Canvas.SetTop(obstacle, obstacles[random.Next(0, obstacles.Length)]);

                obstacle.Width = obstacleWidths[random.Next(0, obstacles.Length)];

                scoreNum += 1;
                score.Content = "Score " + scoreNum;

            }


            if (playerHitBox.IntersectsWith(obstacleHitBox))
            {
                gameOver = true;
                gameTimer.Stop();
            }

        }

        private void StartGame()
        {
            Canvas.SetLeft(Bacground, 0);
            Canvas.SetLeft(Bacground2, 1262);

            Canvas.SetLeft(player, 100);
            Canvas.SetTop(player, 140);

            Canvas.SetLeft(obstacle, 700);
            Canvas.SetTop(obstacle, 300);

            jumping = false;
            gameOver = false;
            scoreNum = 0;

            score.Content = "Score " + scoreNum;

            gameTimer.Start();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine("KeyIsDown called");
            if (e.Key == Key.Enter && gameOver == true) //delete to see why I need it
            {
                //Debug.WriteLine("S");
                StartGame();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine("KeyIsUp called");
            if (e.Key == Key.Space && jumping == false && Canvas.GetTop(player) > 260) // player Top > 260? why I need to write
            {
                //Debug.WriteLine("Jumping is set to true");
                jumping = true;
                force = 20;
                //speed = -12;
            }
        }
    }
}
