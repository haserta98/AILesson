using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchAlgorithm
{
    struct Position
    {
        public int X;
        public int Y;
        public Position(int x, int y) //constructer metodu
        {
            X = x;
            Y = y;
        }

        public static Position Left()
        {
            return new Position(-1, 0);
        }
        public static Position Right()
        {
            return new Position(1, 0);
        }

        public static Position Up()
        {
            return new Position(0, -1);
        }
        public static Position Down()
        {
            return new Position(0, 1);
        }

        public static Position operator +(Position x, Position y)
        {
            Position pos = new Position();
            pos.X = x.X + y.X;
            pos.Y = x.Y + y.Y;
            return pos;
        }
        public static Position operator *(Position position, int x)
        {
            Position pos = new Position();
            pos.X = x * position.X;
            pos.Y = x * position.Y;
            return pos;
        }
        public static Position operator -(Position x, Position y)
        {
            Position pos = new Position();
            pos.X = x.X - y.X;
            pos.Y = x.Y - y.Y;
            return pos;
        }
        public static bool operator ==(Position x, Position y)
        {
            return x.X == y.X && x.Y == y.Y;
        }
        public static bool operator !=(Position x, Position y)
        {
            return x.X != y.X || x.Y != y.Y;
        }
    }

    public partial class Form1 : Form
    {
        int[,] pixels;
        int[,] PixelMatris;
        List<Position> visited = new List<Position>();
        List<Position> neighbour = new List<Position>();
        Bitmap bitmap;
        int blockPixelSize = 20; //her blok 10*10'luk bir pixel'e sahip
        Position startpos = new Position(0, 4);
        Position endpos = new Position(9, 3);
        Position current;
        double temp;
        double tempCost;
        Position minNeighbour;
        Position tempNeighbour;
        double neighBourCost;
        double tempNeighbourCost;
        Position lastGreaterThanOneNeighbour;
        public Form1()
        {
            this.Visible = true;
            InitializeComponent();
            ImageToArrayMatrix();
            current = startpos;
            temp = CalculateDist(current);
        }

        private void AStarAlgorithm()
        {
            while (true)
            {
                LookAround(); //komşuları tara
                AStarHeuristicFunction(); //fonksiyonu çalıştır
                Draw(current, Color.Aqua); //ilerlediğin bölgeleri boya
                if (CalculateDist(current) == 0) //eğer hedefe vardıysa  
                    break; //döngüyü kır        
            }
        }

        private void BestFirstSearch()
        {
            while (true)
            {
                LookAround();
                BestFirstSearchHeuristicFunction();
                Draw(current, Color.Aqua);
                if (CalculateDist(current) == 0)
                {
                    break;
                }
            }
        }

        private void LookAround()
        {
            if (IsValid(Position.Right())) //sağ taraf ziyaret edilebilir mi
            {
                if (!visited.Contains(Position.Right() + current)) //daha önce ziyaret edilmiş mi
                {
                    neighbour.Add(Position.Right() + current); //edilmemişse burayı komşu olarak ata
                }
            }
            if (IsValid(Position.Left()))
            {
                if (!visited.Contains(Position.Left() + current))
                {
                    neighbour.Add(Position.Left() + current);
                }
            }
            if (IsValid(Position.Up()))
            {
                if (!visited.Contains(Position.Up() + current))
                {
                    neighbour.Add(Position.Up() + current);
                }
            }
            if (IsValid(Position.Down()))
            {
                if (!visited.Contains(Position.Down() + current))
                {
                    neighbour.Add(Position.Down() + current);
                }
            }
        }

        private void AStarHeuristicFunction()
        {

            if (neighbour.Count == 0) //eğer komşu sayısı sıfır ise demek ki problem var
            {
                current = lastGreaterThanOneNeighbour; //önceki 1'den fazla komşusu olan pozisyona git
                AStarAlgorithm(); //tekrar çalıştır algoritmayı
            }
            
            else
            {
               
                tempCost = CalculateDist(neighbour[0]) + CalculateFromStart(); //karşılaştırarak en küçüğünü bulacağımız için ilk komşunun maliyetini değişkene aktarıp ona göre işlem yapıcaz
                minNeighbour = neighbour[0];

                for (int i = 0; i < neighbour.Count; i++)
                {
                    neighBourCost = CalculateDist(neighbour[i]) + CalculateFromStart(); //komşunun bitişe olan maliyeti ve şuanki noktaya geliş maliyeti hesaplanıyor
                    if (tempCost >= neighBourCost) //tek tek bütün komşulara bakıp en küçüğü bulunuyor
                    {
                        minNeighbour = neighbour[i];
                        tempCost = neighBourCost;
                    }
                }
                if (neighbour.Count > 1) lastGreaterThanOneNeighbour = current; //komşusu 1'den fazla ise o anki konumu ata
                current = minNeighbour; //o anki pozisyonuen küçük komşuya atanıyor
                visited.Add(current); //ziyaret edildi olarak işaretle
            }            
            neighbour.Clear();
        }

        private void BestFirstSearchHeuristicFunction()
        {

           if (neighbour.Count == 0) //eğer komşu sayısı sıfır ise demek ki problem var
            {
                 //bir önceki yeri ziyaret etmemiş olarak ayarla
                current = lastGreaterThanOneNeighbour;
                BestFirstSearch(); //tekrar komşu ara
            }
            
            else
            {
               
                tempCost = CalculateDist(neighbour[0]); //karşılaştırarak en küçüğünü bulacağımız için ilk komşunun maliyetini değişkene aktarıp ona göre işlem yapıcaz
                minNeighbour = neighbour[0];
                for (int i = 0; i < neighbour.Count; i++)
                {
                    neighBourCost = CalculateDist(neighbour[i]); //komşunun bitişe olan maliyeti ve şuanki noktaya geliş maliyeti hesaplanıyor
                    if (tempCost >= neighBourCost) //tek tek bütün komşulara bakıp en küçüğü bulunuyor
                    {
                        minNeighbour = neighbour[i];
                        tempCost = neighBourCost;
                    }
                }
                if (neighbour.Count > 1) lastGreaterThanOneNeighbour = current;
                current = minNeighbour; //o anki pozisyonuen küçük komşuya atanıyor
                visited.Add(current); //ziyaret edildi olarak işaretle
                MessageBox.Show(visited.Count + " visit");
            }            
            neighbour.Clear();
        }
        
        private void ChangeBlock(int x, int y)
        {
            //verilen koordinatlara göre blokların işlevlerinin değiştirilmesi 
            int type = 0; //checkbox'a göre type, type= engel yada normal yol
            if (checkBox1.Checked)
                type = 1;
            if (x < 10 && y < 5)
            {
                switch (type)
                {
                    case 0:

                        PixelMatris[x, y] = 1;
                        Draw(new Position(x, y), Color.White);
                        break;
                    case 1:
                        PixelMatris[x, y] = 0;
                        Draw(new Position(x, y), Color.Black);
                        break;
                }
            }
        }

        private void ImageToArrayMatrix()
        {
            //görüntü işleme kısmı
            bitmap = new Bitmap(pictureBox1.Image);
            pixels = new int[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width - 1; x++)
            {
                for (int y = 0; y < bitmap.Height - 1; y++)
                {
                    switch (bitmap.GetPixel(x, y).Name)
                    {
                        case "ffffffff":
                            pixels[x, y] = 1; //beyaz
                            break;
                        case "ff000000":
                            pixels[x, y] = 0; //siyah
                            break;
                        case "ff00c000":
                            pixels[x, y] = -1; //yeşil
                            break;
                        case "ffc00000":
                            pixels[x, y] = -2; //kırmızı
                            break;
                    }
                }
            }

            PixelMatris = new int[bitmap.Width / blockPixelSize, bitmap.Height / blockPixelSize]; //Her bloğun boyutu 20*20 pixel olduğu için bloklara bölüyorum
            for (int x = 1; x <= bitmap.Width / blockPixelSize; x++)
            {
                for (int y = 1; y <= bitmap.Height / blockPixelSize; y++)
                {
                    PixelMatris[x - 1, y - 1] = pixels[x * (blockPixelSize) - 3, y * (blockPixelSize) - 3];
                }
            }
        }

        private double CalculateDist(Position pos)
        {
            //hedefe olan uzaklık
            return Math.Sqrt(Math.Pow((endpos.X - pos.X), 2) + Math.Sqrt(Math.Pow((endpos.Y - pos.Y), 2)));
            //return Math.Abs(endpos.X - pos.X) + Math.Abs(endpos.Y - pos.Y);
        }

        private double CalculateFromStart()
        {
            //başlangıçtan şuanki konuma uzaklığı
            return Math.Sqrt(Math.Pow((current.X - startpos.X), 2) + Math.Sqrt(Math.Pow((current.Y - startpos.Y), 2)));
        }

        private bool IsValid(Position checkPos)

        {
            //uygun hareket mi değil mi diye kontrol edilen bölge
            return current.X + checkPos.X > -1 &&
                    current.Y + checkPos.Y > -1 &&
                    current.X + checkPos.X < bitmap.Width / blockPixelSize &&
                   current.Y + checkPos.Y < bitmap.Height / blockPixelSize &&
                    (PixelMatris[current.X + checkPos.X, current.Y + checkPos.Y] == 1 || PixelMatris[current.X + checkPos.X, current.Y + checkPos.Y] == -2);

        }

        private void Draw(Position pos, Color color)
        {
            //istenilen pozisyona istenilen rengin çizilmesi
            for (int x = (pos.X * blockPixelSize); x < blockPixelSize * (pos.X + 1); x++)
            {
                for (int y = (pos.Y * blockPixelSize); y < blockPixelSize * (pos.Y + 1); y++)
                {
                    bitmap.SetPixel(x, y, color);

                }
            }
            pictureBox1.Image = bitmap;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AStarAlgorithm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BestFirstSearch();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChangeBlock(Convert.ToInt16(textBox1.Text), Convert.ToInt16(textBox2.Text));
        }


    }
}

