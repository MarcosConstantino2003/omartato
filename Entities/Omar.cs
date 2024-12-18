using System.Data;
using System.Drawing.Drawing2D;

public class Omar
{
    public float X { get; set; }
    public float Y { get; set; }
    public float size { get; set; }
    public float maxSpeed { get; set; }
    public float velocityX { get; set; }
    public float velocityY { get; set; }
    public float maxHP { get; set; }
    public float hp { get; set; }
    public int hpRegen { get; set; }
    public int lifeSteal { get; set; }
    public int damage { get; set; }
    public int meleeDamage { get; set; }
    public int rangedDamage { get; set; }
    public int elementalDamage { get; set; }
    public int shotSpeed { get; set; }
    public float speed { get; set; }
    public float armor { get; private set; }
    public int engineering { get; set; }
    public int critChance { get; set; }
    public int harvesting { get; set; }
    public int luck { get; set; }
    public List<Weapon> weapons { get; set; }
    private bool isTakingDamage;
    private DateTime damageStartTime;
    private bool isRed;
    private bool isInvulnerable;
    private System.Windows.Forms.Timer invulnerabilityTimer;
    public float range { get; set; }
    private DateTime lastRegenTime;
    private float regenInterval;
    private int playAreaLeft = 270;
    private int playAreaRight = 1270;
    private int playAreaTop = 25;
    private int playAreaBottom = 825;

    public Omar(float x, float y, float size)
    {
        X = x;
        Y = y;
        this.size = size;
        velocityX = 0;
        velocityY = 0;
        //stats iniciales
        speed = 4;
        maxSpeed = 9;
        maxHP = 15;
        hp = maxHP;
        damage = 0;
        rangedDamage = 0;
        meleeDamage = 0;
        shotSpeed = 1;
        range = 180;
        armor = 3;
        //hp regen
        hpRegen = 0;
        lastRegenTime = DateTime.Now;
        regenInterval = 3.3f - 0.3f * hpRegen;
        //iframes
        invulnerabilityTimer = new System.Windows.Forms.Timer();
        invulnerabilityTimer.Interval = 1000;
        invulnerabilityTimer.Tick += (sender, e) =>
        {
            isInvulnerable = false;
            invulnerabilityTimer.Stop();
        };
        //weapons
        weapons = new List<Weapon> { new Pistol(this) };   
    }

    public void MoveSmooth(float deltaX, float deltaY)
    {
        float magnitude = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        // Si no estamos moviéndonos, no normalizamos
        if (magnitude != 0)
        {
            deltaX = deltaX / magnitude;
            deltaY = deltaY / magnitude;
        }

        velocityX = deltaX * speed;
        velocityY = deltaY * speed;
    }

    public void UpdatePosition()
    {
        X += velocityX;
        Y += velocityY;

        X = Math.Clamp(X, playAreaLeft + size / 2, playAreaRight - size / 2);
        Y = Math.Clamp(Y, playAreaTop + size / 2, playAreaBottom - size / 2);

        //Regeneración de HP
        if (hpRegen > 0)
        {
            regenInterval = Math.Max(1f, 8.8f - 0.8f * hpRegen);
            if ((DateTime.Now - lastRegenTime).TotalSeconds >= regenInterval)
            {
                IncreaseHP(1);
                lastRegenTime = DateTime.Now;
            }
        }
        //iframes
        if (isTakingDamage)
        {
            double elapsedSeconds = (DateTime.Now - damageStartTime).TotalSeconds;
            if ((int)(elapsedSeconds * 10) % 2 == 0)
            {
                isRed = true;
            }
            else
            {
                isRed = false;
            }

            if (elapsedSeconds > 0.5)
            {
                isTakingDamage = false;
                isRed = false;
            }
        }
    }

    public bool IsCollidingWithDiamond(Diamond diamond)
    {
        // Desplazamientos para ajustar la hitbox del diamante
        float offsetX = diamond.Size * 0.5f;
        float offsetY = diamond.Size * 0.5f;

        // Coordenadas ajustadas del diamante
        float adjustedDiamondX = diamond.Position.X + offsetX;
        float adjustedDiamondY = diamond.Position.Y + offsetY;

        // Calcular la distancia entre Omar y el centro ajustado del diamante
        float dx = X - adjustedDiamondX;
        float dy = Y - adjustedDiamondY;
        float distance = (float)Math.Sqrt(dx * dx + dy * dy);

        // Verificar colisión usando las hitboxes ajustadas
        return distance < (size / 2 + diamond.Size / 2);
    }

    public bool IsCollidingWithEnemy(Enemy enemy)
    {
        float offsetX = -size / 2;

        return (X + offsetX < enemy.Position.X + enemy.Size &&
                X + size + offsetX > enemy.Position.X &&
                Y - size / 2 < enemy.Position.Y + enemy.Size &&
                Y + size / 2 > enemy.Position.Y);
    }

    public bool IsCollidingWithHeart(Heart heart)
    {
        float offsetX = heart.Size;
        float offsetY = heart.Size;

        float adjustedHeartX = heart.Position.X + offsetX;
        float adjustedHeartY = heart.Position.Y + offsetY;

        return (X < adjustedHeartX + heart.Size &&
                X + size > adjustedHeartX &&
                Y < adjustedHeartY + heart.Size &&
                Y + size > adjustedHeartY);
    }

    public int GetShootDelay()
    {
        int calculatedDelay = 500 - (int)(shotSpeed * 30);
        return Math.Max(calculatedDelay, 100); // Limitar el delay a 100ms como mínimo
    }

    public void IncreaseHP(int amount)
    {
        hp += amount;
        if (hp > maxHP)
        {
            hp = maxHP;
        }
    }

    public void heal()
    {
        hp = maxHP;
    }

    public int takeDamage(float amount)
    {
        if (!isInvulnerable)
        {
            //aplicar armor
            float reductionFactor = 1 - (armor * 0.06f);
            if (reductionFactor < 0) reductionFactor = 0;
            float finalDamage = amount * reductionFactor;
            int roundedDamage = (int)Math.Round(finalDamage);
            //reducir hp
            hp -= roundedDamage;
            if (hp < 0) hp = 0;
            isTakingDamage = true;
            damageStartTime = DateTime.Now;
            isInvulnerable = true;
            invulnerabilityTimer.Start();
            return roundedDamage;
        }
        return 0;
    }


    public void changeSpeed(float amount)
    {
        speed += amount;
        if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }
    }

    public void changeShotSpeed(int amount)
    {
        shotSpeed += amount;
    }

    public void changeHPRegen(int amount)
    {
        hpRegen += amount;
    }
    public void changeDamage(int amount)
    {
        damage += amount;
    }

    public void changeMaxHP(int amount)
    {
        maxHP += amount;
    }

    public void changeArmor(int amount)
    {
        armor += amount;
        if (armor < 0) armor = 0;
    }

    public bool getisInvulnerable()
    {
        return isInvulnerable;
    }

    public void ResetPosition()
    {
        X = 770;
        Y = 400;
    }


    public void Shoot(List<Bullet> bullets, Enemy closestEnemy)
    {
        // Usar el método de cada arma en la lista
        foreach (var weapon in weapons)
        {
              weapon.Shoot(bullets, closestEnemy);
        }
    }


    public void Draw(Graphics g)
    {
        // Cargar la imagen de Omar
        Image omarImage = Image.FromFile("img\\omar.webp");

        // Verificar si Omar está tomando daño y decidir el color
        bool drawRedOverlay = isTakingDamage && isRed;

        // Dibujar la imagen centrada en las coordenadas de Omar
        float imageX = X - size / 2;
        float imageY = Y - size / 2;
        g.DrawImage(omarImage, imageX, imageY, size, size);

        if (drawRedOverlay)
            if (drawRedOverlay)
            {
                using (Brush redBrush = new SolidBrush(Color.FromArgb(120, Color.Red))) // Semitransparente
                {
                    g.FillEllipse(redBrush, imageX, imageY, size - 2, size - 2);
                }
            }
        // Dibujar todas las armas
        foreach (var weapon in weapons)
        {
              weapon.Draw(g); // Llamar a la lógica de dibujo de cada arma
        }
    }


}
