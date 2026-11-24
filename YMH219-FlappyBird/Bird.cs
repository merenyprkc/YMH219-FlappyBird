using System.Drawing;

namespace YMH219_FlappyBird
{
    // Kuşu temsil eden sınıf: konum, hız ve çizim işlemleri burada
    public class Bird
    {
        private const float Gravity = 0.6f;      // Her tick'te hıza eklenen yer çekimi
        private const float JumpSpeed = -9f;     // Zıplayınca verilen yukarı hız (negatif = yukarı)

        public float X { get; private set; }
        public float Y { get; private set; }
        public int Size { get; private set; }
        private float velocity;                  // Dikey hız

        public Bird(float x, float y, int size)
        {
            X = x;
            Y = y;
            Size = size;
            velocity = 0;
        }

        // Her timer tick'inde çağrılır: yer çekimi hızı artırır, hız konumu değiştirir
        public void Update()
        {
            velocity += Gravity;
            Y += velocity;
        }

        // Space tuşuna basılınca kuş yukarı doğru hız kazanır
        public void Jump()
        {
            velocity = JumpSpeed;
        }

        // Kuşu sarı bir daire olarak çizer
        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Gold, X, Y, Size, Size);
            g.DrawEllipse(Pens.Black, X, Y, Size, Size);
            // Göz
            g.FillEllipse(Brushes.Black, X + Size * 0.65f, Y + Size * 0.25f, Size * 0.15f, Size * 0.15f);
        }
    }
}
