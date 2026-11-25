using System;
using System.Drawing;

namespace YMH219_FlappyBird
{
    // Engeli (üst ve alt boru çifti) temsil eden sınıf
    public class Pipe
    {
        public const int Width = 70;             // Boru genişliği
        private const int GapHeight = 150;       // Boşluk yüksekliği
        private const int MinTop = 60;           // Boşluğun tavana en yakın olabileceği yer

        private Random random;
        private int groundY;                     // Zeminin başladığı y değeri

        public float X { get; private set; }
        public int GapY { get; private set; }    // Boşluğun üst kenarı

        public Pipe(float x, int groundY, Random random)
        {
            this.groundY = groundY;
            this.random = random;
            Reset(x);
        }

        // Engeli verilen x konumuna alır ve boşluğu rastgele belirler
        public void Reset(float x)
        {
            X = x;
            GapY = random.Next(MinTop, groundY - GapHeight - MinTop);
        }

        // Engeli sola doğru hareket ettirir
        public void Update(float speed)
        {
            X -= speed;
        }

        // Engel tamamen ekrandan çıktı mı?
        public bool IsOffScreen()
        {
            return X + Width < 0;
        }

        // Kuş üst veya alt boruya değiyor mu?
        public bool Collides(RectangleF birdBounds)
        {
            RectangleF top = new RectangleF(X, 0, Width, GapY);
            RectangleF bottom = new RectangleF(X, GapY + GapHeight, Width, groundY - GapY - GapHeight);
            return top.IntersectsWith(birdBounds) || bottom.IntersectsWith(birdBounds);
        }

        public void Draw(Graphics g)
        {
            // Üst boru
            g.FillRectangle(Brushes.ForestGreen, X, 0, Width, GapY);
            g.DrawRectangle(Pens.DarkGreen, X, 0, Width, GapY);
            // Alt boru
            int bottomY = GapY + GapHeight;
            g.FillRectangle(Brushes.ForestGreen, X, bottomY, Width, groundY - bottomY);
            g.DrawRectangle(Pens.DarkGreen, X, bottomY, Width, groundY - bottomY);
        }
    }
}
