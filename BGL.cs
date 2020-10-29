using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */
        Planet p1, p2;
        Raketa r;

        Random s1;
        int odgovor;
        int brojH = 0;   //brojači za labele
        int brojT = 0;
        int brojF = 0;
        double brojA;

        Label pitanje, o, o2;

        Label labelaHIT, labelaTRUE, labelaFALSE, labelaAVG;

        /* Initialization */
        
        private void IspisStatistike()
        {
            labelaHIT.Invoke((MethodInvoker)(() => labelaHIT.Text = brojH.ToString()));
            labelaTRUE.Invoke((MethodInvoker)(() => labelaTRUE.Text = brojT.ToString()));
            labelaFALSE.Invoke((MethodInvoker)(() => labelaFALSE.Text = brojF.ToString()));
            labelaAVG.Invoke((MethodInvoker)(() => labelaAVG.Text = (brojA*100).ToString("0.00")));
            
        }

        private void SetupGame()
        {
            
            //1. setup stage
            SetStageTitle("PMF");
            //setBackgroundColor(Color.WhiteSmoke);            
            setBackgroundPicture("backgrounds\\Pozadina.png");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            //2. add sprites
            //raketa je na 300,250
            r = new Raketa("sprites\\Rocket.png", 300, 250, 0);
            r.RotationStyle = "AllAround";
            r.SetSize(20);
            Game.AddSprite(r);

            p1 = new Planet("sprites\\Planet1.png", 300, 80, 1, 0);
            p2 = new Planet("sprites\\Planet2.png", 300, 460, 1, 0);
            p1.SetSize(15);
            p2.SetSize(15);
            Game.AddSprite(p1);
            Game.AddSprite(p2);
            

            //3. scripts that start
            Game.StartScript(KretanjeRakete);
            Game.StartScript(KretanjeP1);
            Game.StartScript(KretanjeP2);
            

            //DASHBOARD labele

            Label p = new Label();    
            p.Location = new Point(40, 100);
            p.AutoSize = true;
            p.Text = "Pitanje:";
            p.BackColor = System.Drawing.Color.Orange;
            this.Controls.Add(p);

            pitanje = new Label();  //labela za pitanje
            pitanje.Location = new Point(40, 150);
            pitanje.AutoSize = true;
            pitanje.Text = "pitanje";
            this.Controls.Add(pitanje);

            labelaHIT = new Label();  //labela HIT
            labelaHIT.Location = new Point(120, 370);
            labelaHIT.AutoSize = true;
            labelaHIT.Text = "0";
            this.Controls.Add(labelaHIT);

            labelaTRUE = new Label();  //labela TRUE
            labelaTRUE.Location = new Point(120, 425);
            labelaTRUE.AutoSize = true;
            labelaTRUE.Text = "0";
            this.Controls.Add(labelaTRUE);

            labelaFALSE = new Label();  //labela FALSE
            labelaFALSE.Location = new Point(120, 475);
            labelaFALSE.AutoSize = true;
            labelaFALSE.Text = "0";
            this.Controls.Add(labelaFALSE);

            labelaAVG = new Label();  //labela AVG
            labelaAVG.Location = new Point(120, 525);
            labelaAVG.AutoSize = true;
            labelaAVG.Text = "0";
            this.Controls.Add(labelaAVG);

            o = new Label();
            o.Location = new Point(p1.X-20, p1.Y+30);
            o.AutoSize = true;
            o.Text = "odgovor1";
            o.BackColor = System.Drawing.Color.Orange;
            this.Controls.Add(o);

            o2 = new Label();
            o2.Location = new Point(p2.X-20, p2.Y+40);
            o2.AutoSize = true;
            o2.Text = "odgovor2";
            o2.BackColor = System.Drawing.Color.Orange;
            this.Controls.Add(o2);



            //Generiranje pitanja i spremanje u svojstvo
            

            s1 = new Random();
            int br1 = s1.Next(1, 20);


            int br2 = s1.Next(1, 20);

            pitanje.Text = br1 + " + " + br2;

            odgovor = br1 + br2;
            Random dodani = new Random();
            int d = dodani.Next(1, 3);

            Random where = new Random();
            int b = where.Next(1, 3);
            if (b==1)
            {
                o.Text = odgovor.ToString();
                p1.Odgovor = odgovor;
                int t = d + odgovor;
                o2.Text = t.ToString();
                p2.Odgovor = t;
            }
            if (b==2)
            {
                o2.Text = odgovor.ToString();
                p2.Odgovor = odgovor;
                int t = d + odgovor;
                o.Text = t.ToString();
                p1.Odgovor = t;
            }



        }

        /* Scripts */


        private int KretanjeRakete()
        {
            while (START) //ili neki drugi uvjet
            {
                r.PointToMouse(sensing.Mouse);
                
                if (sensing.MouseDown)
                {
                    r.MoveSteps(5);
                }

                if (r.TouchingSprite(p1))
                {
                    if (p1.Odgovor==odgovor)
                    {
                        START = false;
                        
                        brojH++;
                        brojT++;
                        brojA = (double)brojT / (double)brojH;
                        

                        MessageBox.Show("Točno.");
                        IspisStatistike();
                        
                        s1 = new Random();
                        int br1 = s1.Next(1, 20);


                        int br2 = s1.Next(1, 20);
                        pitanje.Invoke((MethodInvoker)(() => pitanje.Text = br1 + " + " + br2));
                        //pitanje.Text = br1 + " + " + br2;

                        odgovor = br1 + br2;
                        Random dodani = new Random();
                        int d = dodani.Next(1, 3);

                        Random where = new Random();
                        int b = where.Next(1, 3);
                        if (b == 1)
                        {
                            o.Invoke((MethodInvoker)(() => o.Text = odgovor.ToString()));
                            //o.Text = odgovor.ToString();
                            p1.Odgovor = odgovor;
                            int t = d + odgovor;
                            o2.Invoke((MethodInvoker)(() => o2.Text = t.ToString()));
                            //o2.Text = t.ToString();
                            p2.Odgovor = t;
                        }
                        if (b == 2)
                        {
                            o2.Invoke((MethodInvoker)(() => o2.Text = odgovor.ToString()));
                            //o2.Text = odgovor.ToString();
                            p2.Odgovor = odgovor;
                            int t = d + odgovor;
                            o.Invoke((MethodInvoker)(() => o.Text = t.ToString()));
                            //o.Text = t.ToString();
                            p1.Odgovor = t;
                        }
                        p1.X = 300;
                        p1.Y = 80;
                        p2.X = 300;
                        p2.Y = 460;
                        r.X = 300;
                        r.Y = 250;
                        p1.Brzina = brojT;
                        p2.Brzina = brojT;
                        START = true;
                        Game.StartScript(KretanjeP1);
                        Game.StartScript(KretanjeP2);
                    }
                    else
                    {
                        //pogrešan odgovor
                        START = false;
                        
                        brojF++;
                        brojH++;
                        brojA = (double)brojT / (double)brojH;
                        MessageBox.Show("Netočno.");
                        IspisStatistike();
                        s1 = new Random();
                        int br1 = s1.Next(1, 20);


                        int br2 = s1.Next(1, 20);
                        pitanje.Invoke((MethodInvoker)(() => pitanje.Text = br1 + " + " + br2));
                        //pitanje.Text = br1 + " + " + br2;

                        odgovor = br1 + br2;
                        Random dodani = new Random();
                        int d = dodani.Next(1, 3);

                        Random where = new Random();
                        int b = where.Next(1, 3);
                        if (b == 1)
                        {
                            o.Invoke((MethodInvoker)(() => o.Text = odgovor.ToString()));
                            //o.Text = odgovor.ToString();
                            p1.Odgovor = odgovor;
                            int t = d + odgovor;
                            o2.Invoke((MethodInvoker)(() => o2.Text = t.ToString()));
                            //o2.Text = t.ToString();
                            p2.Odgovor = t;
                        }
                        if (b == 2)
                        {
                            o2.Invoke((MethodInvoker)(() => o2.Text = odgovor.ToString()));
                            //o2.Text = odgovor.ToString();
                            p2.Odgovor = odgovor;
                            int t = d + odgovor;
                            o.Invoke((MethodInvoker)(() => o.Text = t.ToString()));
                            //o.Text = t.ToString();
                            p1.Odgovor = t;
                        }
                        p1.X = 300;
                        p1.Y = 80;
                        p2.X = 300;
                        p2.Y = 460;
                        r.X = 300;
                        r.Y = 250;
                        p1.Brzina = brojT;
                        p2.Brzina = brojT;
                        
                        START = true;

                        Game.StartScript(KretanjeP1);
                        Game.StartScript(KretanjeP2);
                    }
                }
                if (r.TouchingSprite(p2))
                {
                    if (p2.Odgovor==odgovor)
                    {
                        START = false;
                        
                        brojH++;
                        brojT++;
                        brojA = (double)brojT / (double)brojH;
                        MessageBox.Show("Točno.");
                        IspisStatistike();
                        s1 = new Random();
                        int br1 = s1.Next(1, 20);


                        int br2 = s1.Next(1, 20);
                        pitanje.Invoke((MethodInvoker)(() => pitanje.Text = br1 + " + " + br2));
                        //pitanje.Text = br1 + " + " + br2;

                        odgovor = br1 + br2;
                        Random dodani = new Random();
                        int d = dodani.Next(1, 3);

                        Random where = new Random();
                        int b = where.Next(1, 3);
                        if (b == 1)
                        {
                            o.Invoke((MethodInvoker)(() => o.Text = odgovor.ToString()));
                            //o.Text = odgovor.ToString();
                            p1.Odgovor = odgovor;
                            int t = d + odgovor;
                            o2.Invoke((MethodInvoker)(() => o2.Text = t.ToString()));
                            //o2.Text = t.ToString();
                            p2.Odgovor = t;
                        }
                        if (b == 2)
                        {
                            o2.Invoke((MethodInvoker)(() => o2.Text = odgovor.ToString()));
                            //o2.Text = odgovor.ToString();
                            p2.Odgovor = odgovor;
                            int t = d + odgovor;
                            o.Invoke((MethodInvoker)(() => o.Text = t.ToString()));
                            //o.Text = t.ToString();
                            p1.Odgovor = t;
                        }
                        p1.X = 300;
                        p1.Y = 80;
                        p2.X = 300;
                        p2.Y = 460;
                        r.X = 300;
                        r.Y = 250;
                        p1.Brzina = brojT;
                        p2.Brzina = brojT;
                        START = true;
                        Game.StartScript(KretanjeP1);
                        Game.StartScript(KretanjeP2);
                    }
                    else
                    {
                        //pogrešan odgovor
                        START = false;

                        
                        
                        
                        brojF++;
                        brojH++;
                        brojA = (double)brojT / (double)brojH;
                        MessageBox.Show("Netočno.");
                        IspisStatistike();
                        s1 = new Random();
                        int br1 = s1.Next(1, 20);


                        int br2 = s1.Next(1, 20);
                        pitanje.Invoke((MethodInvoker)(() => pitanje.Text = br1 + " + " + br2));
                        //pitanje.Text = br1 + " + " + br2;

                        odgovor = br1 + br2;
                        Random dodani = new Random();
                        int d = dodani.Next(1, 3);

                        Random where = new Random();
                        int b = where.Next(1, 3);
                        if (b == 1)
                        {
                            o.Invoke((MethodInvoker)(() => o.Text = odgovor.ToString()));
                            //o.Text = odgovor.ToString();
                            p1.Odgovor = odgovor;
                            int t = d + odgovor;
                            o2.Invoke((MethodInvoker)(() => o2.Text = t.ToString()));
                            //o2.Text = t.ToString();
                            p2.Odgovor = t;
                        }
                        if (b == 2)
                        {
                            o2.Invoke((MethodInvoker)(() => o2.Text = odgovor.ToString()));
                            //o2.Text = odgovor.ToString();
                            p2.Odgovor = odgovor;
                            int t = d + odgovor;
                            o.Invoke((MethodInvoker)(() => o.Text = t.ToString()));
                            //o.Text = t.ToString();
                            p1.Odgovor = t;
                        }
                        p1.X = 300;
                        p1.Y = 80;
                        p2.X = 300;
                        p2.Y = 460;
                        r.X = 300;
                        r.Y = 250;
                        p1.Brzina = brojT;
                        p2.Brzina = brojT;
                        
                       
                        START = true;
                        Game.StartScript(KretanjeP1);
                        Game.StartScript(KretanjeP2);
                    }
                }
                if (brojH==15)
                {
                    START = false;

                    string over = "Game over.\n Broj točnih odgovora je " + brojT.ToString();
                    MessageBox.Show(over);
                    Application.Exit();
                }
                Wait(0.01);
            }
            return 0;
        }

        private int KretanjeP1()
        {
            while (START)
            {
                p1.X += p1.Brzina*2;

                if (p1.X>=1150)
                {
                    START = false;

                    brojH++;
                    brojF++;
                    brojA = (double)brojT / (double)brojH;
                    IspisStatistike();
                    MessageBox.Show("You're too slow.");
                    p1.X = 300;
                    p1.Y = 80;
                    p2.X = 300;
                    p2.Y = 460;
                    r.X = 300;
                    r.Y = 250;
                    START = true;
                    Game.StartScript(KretanjeP1);
                    Game.StartScript(KretanjeP2);
                    Game.StartScript(KretanjeRakete);
                }
                
                
                Wait(0.1);
            }
            return 0;
        }

        private int KretanjeP2()
        {
            while (START)
            {
                p2.X += p2.Brzina*2;

                if (p2.X>=1150)
                {
                    START = false;
                    brojH++;
                    brojF++;
                    brojA = (double)brojT / (double)brojH;
                    IspisStatistike();
                    MessageBox.Show("You're too slow.");
                    p1.X = 300;
                    p1.Y = 80;
                    p2.X = 300;
                    p2.Y = 460;
                    r.X = 300;
                    r.Y = 250;
                    START = true;
                    Game.StartScript(KretanjeP1);
                    Game.StartScript(KretanjeP2);
                    Game.StartScript(KretanjeRakete);
                }
                
                Wait(0.1);
            }
            return 0;
        }




        /* ------------ GAME CODE END ------------ */


    }
}
