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
        //таймер, щоб для кожного "тіку" змінювалися позиції об'єкту
        DispatcherTimer gameTimer  = new DispatcherTimer();

        //швидкість зміни для гравця, виносимо в зміну тому що в момент коли він наприклад торкається землі треба швидкість зменшити до 0 щоб він не рухався
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

        //
        ImageBrush backgroundSprite = new ImageBrush();
        ImageBrush playerSprite = new ImageBrush();
        ImageBrush obstacleSprite = new ImageBrush();

        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Focus(); // жля чого це

            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(10);

            backgroundSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/background.png"));
            Bacground.Fill = backgroundSprite;  
            Bacground2.Fill = backgroundSprite;

            obstacleSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/thorns.png"));
            obstacle.Fill = obstacleSprite;

            StartGame();
        }

        private void GameEngine(object sender, EventArgs e)
        {
            //тут встановлюємо з якою швидкість буде рухатися фон (на 3 пікселі в ліво кожного "тіку")
            Canvas.SetLeft(Bacground, Canvas.GetLeft(Bacground) - 3);
            Canvas.SetLeft(Bacground2, Canvas.GetLeft(Bacground2) - 3);

            //тут, якщо картинка вже вийшла за межі, переносимо її в кінець 2 картинки і таким чином створюємо ефект безкінечного фону
            if(Canvas.GetLeft(Bacground) < -1262)
            {
                Canvas.SetLeft(Bacground, Canvas.GetLeft(Bacground2) + Bacground2.Width);
            }

            if (Canvas.GetLeft(Bacground2) < -1262)
            {
                Canvas.SetLeft(Bacground2, Canvas.GetLeft(Bacground) + Bacground.Width);
            }

            //с"рухаємо" гравця і "рухаємо" перешкоди
            Canvas.SetTop(player, Canvas.GetTop(player) + speed);
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - 12);

            //встановлюємо значення для наших хітбоксів, щоб потім визнатичи точку дотику різних об'єктів між собою, в гравця забираємо -20 від ширини, щоб створювався ефект того, що він прям врізався в перешкоду
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 20, player.Height);
            obstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width, obstacle.Height);
            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);

            //тут перевіряємо чи гравецт торкнувся землі, якщо так то вниз його далі не опускаємо, встановлюємо йому картинку
            if (playerHitBox.IntersectsWith(groundHitBox))
            {
                speed = 0;
                playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
                player.Fill = playerSprite;
            }

            //тут перевіряємо чи гравець скаче, якщо так то "підіймаємо" його вгору віднімаючи значення від Canvas.Top, змешуємо відстань до максимальної можливої точки стрибку та встановлюємо картинку для стрибку
            //якщо він не підіймається, а вже опускається на землю то додаємо значення до Canvas.Top
            if (jumping)
            {
                speed = -9;
                force -= 1;
                SetPlayerImage(1);
            }
            else if (!jumping && !playerHitBox.IntersectsWith(groundHitBox))
            {
                speed = 12;
            }

            //якщо максимально можливої точки досягнуто, стрибок закінчився і далі потрібно буде спускатися, встоновлюємо заново значення макс. точки
            if (force < 0)
            {
                jumping = false;
                force = 25;
            }

            //робимо так, щоб перешкоди з'являлися в грі постійно
            if (Canvas.GetLeft(obstacle) < -50)
            {
                Canvas.SetLeft(obstacle, 950);

                //різна вистона для перешкод
                Canvas.SetTop(obstacle, obstacles[random.Next(0, obstacles.Length)]);

                //різна ширина для перешкод
                obstacle.Width = obstacleWidths[random.Next(0, obstacles.Length)];

                //обновлюємо рахунок
                scoreNum += 1;
                score.Content = "Score " + scoreNum;

            }

            //якщо гравець наштовхнувся на перешкоду, гру завершено, змінємо зображення та зупиняємо гру
            if (playerHitBox.IntersectsWith(obstacleHitBox))
            {
                SetPlayerImage(2);
                gameOver = true;
                gameTimer.Stop();
            }

        }

        //встановлюємо початкове зображення гравця, розмішення всіх об'єктів та початкові значення для змінних
        private void StartGame()
        {
            SetPlayerImage(0);

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

        //для того, щоб почати спочатку гру натискаючу на enter
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && gameOver) //delete to see why I need it
            {
                StartGame();
            }
        }

        //для стрибків натискаючи на space
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !jumping)
            {
                jumping = true;
                force = 20;
            }
        }

        //для встановлення відповідного зображення в різних ситуаціях
        private void SetPlayerImage(int i)
        {
            switch(i)
            { 
                case 0:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
                    player.Fill = playerSprite;
                    break;
                case 1:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player_up.png"));
                    player.Fill = playerSprite;
                    break;
                case 2:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player_die.png"));
                    player.Fill = playerSprite;
                    break;
            }   
        }
    }
}
