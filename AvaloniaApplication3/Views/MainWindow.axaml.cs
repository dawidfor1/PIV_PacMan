using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Controls.ApplicationLifetimes;
using System.Diagnostics;
using System.Net.Mime;


namespace AvaloniaApplication3.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        GameSetUp();
    }
    
    DispatcherTimer gameTimer = new DispatcherTimer(); // create a new instance of the dispatcher timer called game timer
        bool goLeft, goRight, goDown, goUp; // 4 boolean created to move player in 4 direction
        bool noLeft, noRight, noDown, noUp; // 4 more boolean created to stop player moving in that direction
        int speed = 8; // player speed
        Rect pacmanHitBox; // player hit box, this will be used to check for collision between player to walls, ghost and coints
        int ghostSpeed = 10; // ghost image speed
        int ghostMoveStep = 160; // ghost step limits
        int currentGhostStep; // current movement limit for the ghosts
        int score = 0; // score keeping integer
  
        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            // this is the key down event
            if (e.Key == Key.Left && noLeft == false)
            {
                // if the left key is down and the boolean noLeft is set to false
                goRight = goUp = goDown = false; // set rest of the direction booleans to false
                noRight = noUp = noDown = false; // set rest of the restriction boolean to false
                goLeft = true; // set go left true
                pacman.RenderTransformOrigin = new RelativePoint(0.5,0.5,RelativeUnit.Relative); // rotate the pac man image to face left
                pacman.RenderTransform = new RotateTransform(-180); // rotate the pac man image to face left
            }
            if (e.Key == Key.Right && noRight == false)
            {
                // if the right key pressed and no right boolean is false
                noLeft = noUp = noDown = false; // set rest of the direction boolean to false
                goLeft = goUp = goDown = false; // set rest of the restriction boolean to false
                goRight = true; // set go right to true
                pacman.RenderTransformOrigin = new RelativePoint(0.5,0.5,RelativeUnit.Relative);
                pacman.RenderTransform = new RotateTransform(0); // rotate the pac man image to face right
            }
            if (e.Key == Key.Up && noUp == false)
            {
                // if the up key is pressed and no up is set to false
                noRight = noDown = noLeft = false; // set rest of the direction boolean to false
                goRight = goDown = goLeft = false; // set rest of the restriction boolean to false
                goUp = true; // set go up to true
                pacman.RenderTransformOrigin = new RelativePoint(0.5,0.5,RelativeUnit.Relative);
                pacman.RenderTransform = new RotateTransform(-90); // rotate the pac man character to face up
            }
            if (e.Key == Key.Down && noDown == false)
            {
                // if the down key is press and the no down boolean is false
                noUp = noLeft = noRight = false; // set rest of the direction boolean to false
                goUp = goLeft = goRight = false; // set rest of the restriction boolean to false
                goDown = true; // set go down to true
                pacman.RenderTransformOrigin = new RelativePoint(0.5,0.5,RelativeUnit.Relative);
                pacman.RenderTransform = new RotateTransform(90); // rotate the pac man character to face down
            }
        }
        
        private void GameSetUp()
        {
            // this function will run when the program loads
            MyCanvas.Focus(); // set my canvas as the main focus for the program
            gameTimer.Tick += GameLoop; // link the game loop event to the time tick
            gameTimer.Interval = TimeSpan.FromMilliseconds(20); // set time to tick every 20 milliseconds
            gameTimer.Start(); // start the time
            currentGhostStep = ghostMoveStep; // set current ghost step to the ghost move step
            // below pac man and the ghosts images are being imported from the images folder and then we are assigning the image brush to the rectangles
            ImageBrush pacmanImage = new ImageBrush();
            pacmanImage.Source = new Bitmap("./images/pacman.jpg");
            pacman.Fill = pacmanImage;
            ImageBrush redGhost = new ImageBrush();
            redGhost.Source = new Bitmap("./images/red.jpg");
            redGuy.Fill = redGhost;
            ImageBrush orangeGhost = new ImageBrush();
            orangeGhost.Source = new Bitmap("./images/orange.jpg");
            orangeGuy.Fill = orangeGhost;
            ImageBrush pinkGhost = new ImageBrush();
            pinkGhost.Source = new Bitmap("./images/pink.jpg");
            pinkGuy.Fill = pinkGhost;
        }
         private void GameLoop(object sender, EventArgs e)
        {
            // this is the game loop event, this event will control all of the movements, outcome, collision and score for the game
            txtScore.Content = "Score: " + score; // show the scoreo to the txtscore label. 
            // start moving the character in the movement directions
            if (goRight)
            {
                // if go right boolean is true then move pac man to the right direction by adding the speed to the left 
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + speed);
            }
            if (goLeft)
            {
                // if go left boolean is then move pac man to the left direction by deducting the speed from the left
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - speed);
            }
            if (goUp)
            {
                // if go up boolean is true then deduct the speed integer from the top position of the pac man
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) - speed);
            }
            if (goDown)
            {
                // if go down boolean is true then add speed integer value to the pac man top position
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) + speed);
            }
            //end the movement 
           // restrict the movement
            if (goDown && Canvas.GetTop(pacman) + 80 > 600)
            {
                // if pac man is moving down the position of pac man is grater than the main window height then stop down movement
                noDown = true;
                goDown = false;
            }
            if (goUp && Canvas.GetTop(pacman) < 1)
            {
                // is pac man is moving and position of pac man is less than 1 then stop up movement
                noUp = true;
                goUp = false;
            }
            if (goLeft && Canvas.GetLeft(pacman) - 10 < 1)
            {
                // if pac man is moving left and pac man position is less than 1 then stop moving left
                noLeft = true;
                goLeft = false;
            }
            if (goRight && Canvas.GetLeft(pacman) + 70 > 800)
            {
                // if pac man is moving right and pac man position is greater than the main window then stop moving right
                noRight = true;
                goRight = false;
            }
            pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height); // asssign the pac man hit box to the pac man rectangle
            // below is the main game loop that will scan through all of the rectangles available inside of the game
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                // loop through all of the rectangles inside of the game and identify them using the x variable
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height); // create a new rect called hit box for all of the available rectangles inside of the game
                // find the walls, if any of the rectangles inside of the game has the tag wall inside of it
                if ((string)x.Tag == "wall")
                {
                    // check if we are colliding with the wall while moving left if true then stop the pac man movement
                    if (goLeft == true && pacmanHitBox.Intersects(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + 10);
                        noLeft = true;
                        goLeft = false;
                    }
                    // check if we are colliding with the wall while moving right if true then stop the pac man movement
                    if (goRight == true && pacmanHitBox.Intersects(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 10);
                        noRight = true;
                        goRight = false;
                    }
                    // check if we are colliding with the wall while moving down if true then stop the pac man movement
                    if (goDown == true && pacmanHitBox.Intersects(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) - 10);
                        noDown = true;
                        goDown = false;
                    }
                    // check if we are colliding with the wall while moving up if true then stop the pac man movement
                    if (goUp == true && pacmanHitBox.Intersects(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) + 10);
                        noUp = true;
                        goUp = false;
                    }
                }
               
                // if any rectangle has the tag ghost inside of it
                if ((string) x.Tag == "ghost")
                {
                    // check if pac man collides with the ghost 
                    if (pacmanHitBox.Intersects(hitBox))
                    {
                        // if collision has happened, then end the game by calling the game over function and passing in the message
                        GameOver("Przegrałeś","Niestety zostałeś zjedzony przez ducha, spróbuj ponownie!");
                    }
                    // if there is a rectangle called orange guy in the game
                    if (x.Name.ToString() == "orangeGuy")
                    {
                        // move that rectangle to towards the left of the screen
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - ghostSpeed);
                    }
                    else
                    {
                        // other ones can move towards the right of the screen
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);
                    }
                    // reduce one from the current ghost step integer
                    currentGhostStep--;
                    // if the current ghost step integer goes below 1 
                    if (currentGhostStep < 1)
                    {
                        // reset the current ghost step to the ghost move step value
                        currentGhostStep = ghostMoveStep;
                        // reverse the ghost speed integer
                        ghostSpeed = -ghostSpeed;
                    }
                }
            }

            foreach (var x in MyCanvas.Children.OfType<Ellipse>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                // check if the any of the rectangles has a coin tag inside of them
                if ((string)x.Tag == "coin")
                {
                    // if pac man collides with any of the coin and coin is still visible to the screen
                    if (pacmanHitBox.Intersects(hitBox) && x.IsVisible)
                    {
                        // set the coin visiblity to hidden
                        x.IsVisible = false;
                        // add 1 to the score
                        score++;
                    }
                }
            }

            // if the player collected 85 coins in the game
            if (score == 85)
            {
                // show game over function with the you win message
               GameOver("Wygrałeś","Gratulację, wygrałeś udało Ci się zebrać wszystkie monety");
            }
        }
        private void GameOver(string title,string message)
        {
            // inside the game over function we passing in a string to show the final message to the game
            gameTimer.Stop(); // stop the game timer
            var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow(title, message);
            messageBoxStandardWindow.Show();
            Close();
        }
}