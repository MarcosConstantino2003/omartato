public class FastEnemy : Enemy
{
    public FastEnemy(PointF position) : base(position, 30)
    {
        HP = 4;
        Speed = 1.3f;
        Defense = 0;
    }

    public override void Draw(Graphics g)
    {
        Brush brush = new SolidBrush(Color.Purple); 
        Pen blackPen = new Pen(Color.Black, 2);
        g.FillRectangle(brush, Position.X, Position.Y, Size, Size);
        g.DrawRectangle(blackPen, Position.X, Position.Y, Size, Size);

        string hpText = $"HP: {HP}";
        using (Font font = new Font("Arial", 10, FontStyle.Bold))
        {
            SizeF textSize = g.MeasureString(hpText, font);
            float textX = Position.X + (Size - textSize.Width) / 2;
            float textY = Position.Y - textSize.Height - 5;
            g.DrawString(hpText, font, Brushes.White, textX, textY);
        }
    }
}