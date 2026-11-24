using System;
using System.Drawing;
using System.Windows.Forms;

namespace YMH219_FlappyBird
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;
        private Bird bird;
        private const int GroundHeight = 80;   // Zemin yüksekliği

        public Form1()
        {
            InitializeComponent();

            bird = new Bird(100, ClientSize.Height / 2, 34);

            // Oyun döngüsü: yaklaşık 60 FPS
            gameTimer = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            KeyDown += Form1_KeyDown;
            Paint += Form1_Paint;
        }

        // Her tick'te kuş güncellenir ve ekran yeniden çizilir
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            bird.Update();
            bird.KeepInside(0, ClientSize.Height - GroundHeight);
            Invalidate();
        }

        // Space tuşu ile zıplama
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                bird.Jump();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Zemin
            int groundY = ClientSize.Height - GroundHeight;
            e.Graphics.FillRectangle(Brushes.SandyBrown, 0, groundY, ClientSize.Width, GroundHeight);
            e.Graphics.DrawLine(Pens.DarkGreen, 0, groundY, ClientSize.Width, groundY);

            bird.Draw(e.Graphics);
        }
    }
}
