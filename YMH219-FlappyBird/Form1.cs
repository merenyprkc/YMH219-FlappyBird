using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace YMH219_FlappyBird
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;
        private Bird bird;
        private List<Pipe> pipes = new List<Pipe>();
        private Random random = new Random();

        private const int GroundHeight = 80;   // Zemin yüksekliği
        private const int PipeCount = 3;       // Aynı anda dolaşan engel sayısı
        private const int PipeSpacing = 220;   // Engeller arası yatay mesafe
        private const float PipeSpeed = 3f;    // Engellerin sola hız değeri

        public Form1()
        {
            InitializeComponent();

            bird = new Bird(100, ClientSize.Height / 2, 34);

            // Engelleri sağ kenardan başlayarak aralıklı yerleştir
            int groundY = ClientSize.Height - GroundHeight;
            for (int i = 0; i < PipeCount; i++)
            {
                pipes.Add(new Pipe(ClientSize.Width + i * PipeSpacing, groundY, random));
            }

            // Oyun döngüsü: yaklaşık 60 FPS
            gameTimer = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            KeyDown += Form1_KeyDown;
            Paint += Form1_Paint;
        }

        // Her tick'te kuş ve engeller güncellenir, ekran yeniden çizilir
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            bird.Update();
            bird.KeepInside(0, ClientSize.Height - GroundHeight);
            MovePipes();
            Invalidate();
        }

        // Engelleri hareket ettirir, ekran dışına çıkanı en sağa taşır
        private void MovePipes()
        {
            foreach (Pipe pipe in pipes)
            {
                pipe.Update(PipeSpeed);
            }

            foreach (Pipe pipe in pipes)
            {
                if (pipe.IsOffScreen())
                {
                    float rightMost = 0;
                    foreach (Pipe p in pipes)
                    {
                        if (p.X > rightMost) rightMost = p.X;
                    }
                    pipe.Reset(rightMost + PipeSpacing);
                }
            }
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
            foreach (Pipe pipe in pipes)
            {
                pipe.Draw(e.Graphics);
            }

            // Zemin
            int groundY = ClientSize.Height - GroundHeight;
            e.Graphics.FillRectangle(Brushes.SandyBrown, 0, groundY, ClientSize.Width, GroundHeight);
            e.Graphics.DrawLine(Pens.DarkGreen, 0, groundY, ClientSize.Width, groundY);

            bird.Draw(e.Graphics);
        }
    }
}
