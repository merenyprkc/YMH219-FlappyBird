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
        private Label scoreLabel;
        private Label levelLabel;
        private Label highScoreLabel;
        private int highScore = 0;
        private int score = 0;
        private bool gameOver = false;

        private const int GroundHeight = 80;   // Zemin yüksekliği
        private const int PipeCount = 3;       // Aynı anda dolaşan engel sayısı
        private const int PipeSpacing = 220;   // Engeller arası yatay mesafe
        private const float StartSpeed = 3f;   // Başlangıç engel hızı
        private const float MaxSpeed = 7f;     // En yüksek engel hızı
        private const int LevelScore = 5;      // Kaç skorda bir seviye atlanır
        private float pipeSpeed = StartSpeed;  // Şu anki engel hızı

        public Form1()
        {
            InitializeComponent();

            bird = new Bird(100, ClientSize.Height / 2, 34);

            // Engelleri oluştur, konumları StartGame içinde ayarlanır
            int groundY = ClientSize.Height - GroundHeight;
            for (int i = 0; i < PipeCount; i++)
            {
                pipes.Add(new Pipe(ClientSize.Width + i * PipeSpacing, groundY, random));
            }

            // Skor ve seviye etiketleri
            scoreLabel = CreateLabel(10, 10, 22);
            levelLabel = CreateLabel(10, 50, 14);
            highScoreLabel = CreateLabel(10, 75, 14);

            highScore = HighScore.Load();

            // Oyun döngüsü: yaklaşık 60 FPS
            gameTimer = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameTimer_Tick;

            KeyDown += Form1_KeyDown;
            Paint += Form1_Paint;

            StartGame();
        }

        // Ekranda kullanılacak şeffaf etiketi oluşturur
        private Label CreateLabel(int x, int y, int fontSize)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.BackColor = Color.Transparent;
            label.Font = new Font("Arial", fontSize, FontStyle.Bold);
            label.ForeColor = Color.White;
            label.Location = new Point(x, y);
            Controls.Add(label);
            return label;
        }

        // Skor ve seviye etiketlerini günceller
        private void UpdateLabels()
        {
            scoreLabel.Text = "Skor: " + score;
            levelLabel.Text = "Seviye: " + (score / LevelScore + 1);
            highScoreLabel.Text = "En yüksek: " + highScore;
        }

        // Oyunu başa sarar: kuş, engeller, skor ve durum sıfırlanır
        private void StartGame()
        {
            bird.Reset(100, ClientSize.Height / 2);

            for (int i = 0; i < pipes.Count; i++)
            {
                pipes[i].Reset(ClientSize.Width + i * PipeSpacing);
            }

            score = 0;
            pipeSpeed = StartSpeed;
            UpdateLabels();
            gameOver = false;
            gameTimer.Start();
            Invalidate();
        }

        // Her tick'te kuş ve engeller güncellenir, ekran yeniden çizilir
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            bird.Update();
            bird.KeepInside(0, ClientSize.Height - GroundHeight);
            MovePipes();
            CheckCollision();
            if (gameOver) return;
            UpdateScore();
            Invalidate();
        }

        // Engelleri hareket ettirir, ekran dışına çıkanı en sağa taşır
        private void MovePipes()
        {
            foreach (Pipe pipe in pipes)
            {
                pipe.Update(pipeSpeed);
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

        // Kuş bir engelin önünden geçtiyse skor 1 artar
        private void UpdateScore()
        {
            foreach (Pipe pipe in pipes)
            {
                if (!pipe.Passed && pipe.X + Pipe.Width < bird.X)
                {
                    pipe.Passed = true;
                    score++;
                    UpdateLabels();

                    // Her seviyede engeller biraz daha hızlanır
                    pipeSpeed = Math.Min(MaxSpeed, StartSpeed + (score / LevelScore) * 0.5f);
                }
            }
        }

        // Kuş boruya veya zemine çarptıysa oyunu durdurur
        private void CheckCollision()
        {
            int groundY = ClientSize.Height - GroundHeight;
            bool hitGround = bird.Y + bird.Size >= groundY;

            bool hitPipe = false;
            foreach (Pipe pipe in pipes)
            {
                if (pipe.Collides(bird.Bounds)) hitPipe = true;
            }

            if (hitGround || hitPipe)
            {
                gameOver = true;

                // Yeni rekor varsa kaydet
                if (score > highScore)
                {
                    highScore = score;
                    HighScore.Save(highScore);
                    UpdateLabels();
                }

                gameTimer.Stop();
                Invalidate();
            }
        }

        // Space tuşu ile zıplama
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && !gameOver)
            {
                bird.Jump();
            }
            else if (e.KeyCode == Keys.R && gameOver)
            {
                StartGame();
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

            // Oyun bittiyse ekrana yazı yaz
            if (gameOver)
            {
                using (Font font = new Font("Arial", 22, FontStyle.Bold))
                {
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    RectangleF area = new RectangleF(0, 0, ClientSize.Width, ClientSize.Height);
                    e.Graphics.DrawString("OYUN BİTTİ\nYeniden başlamak için R", font, Brushes.Red, area, format);
                }
            }
        }
    }
}
