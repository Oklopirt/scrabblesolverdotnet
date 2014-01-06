using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WordRead
{
    public partial class BoardDisplay : Form
    {
        private Scrabble m_Game;

        public BoardDisplay(Scrabble scrabgame)
        {
            m_Game = scrabgame;
            InitializeComponent();
          //  ClientSize = new Size(700, 700);
            
            pBoard.Paint += new PaintEventHandler(BoardDisplay_Paint);
            pBoard.Click += new EventHandler(Board_Click);
            pLetters.Paint += new PaintEventHandler(Letters_Paint);
            pLetters.Click += new EventHandler(Letters_Click);
            calTime.Text = "";
            
            this.KeyDown += new KeyEventHandler(Disable_Box);
            this.KeyPress += new KeyPressEventHandler(Disable_KeyPress);
            this.KeyUp += new KeyEventHandler(Board_Key);
            WordBox.SelectedIndexChanged += new EventHandler(WordBox_Change);
            DoubleBuffered = true;
           
            System.Reflection.PropertyInfo bufferProp =
         typeof(System.Windows.Forms.Control).GetProperty(
               "DoubleBuffered",
               System.Reflection.BindingFlags.NonPublic |
               System.Reflection.BindingFlags.Instance);

            bufferProp.SetValue(pBoard, true, null);
            bufferProp.SetValue(pLetters, true, null);
            

            
        }

        void Disable_Box(object sender, KeyEventArgs ke)
        {
            if (m_Game.selCol > -1 || m_Game.selSlot > -1)
                ke.Handled = true;
        }


        void Disable_KeyPress(object sender, KeyPressEventArgs ke)
        {
            if (m_Game.selCol > -1 || m_Game.selSlot > -1)
                ke.Handled = true;
        }

        void Board_Key(object sender, KeyEventArgs ke)
        {
            char sendKey = (char)ke.KeyValue;
            if (!ke.Shift && sendKey >= 65 && sendKey <= 90)
                sendKey += (char)32;
            else if (ke.Shift && sendKey == 56)
                sendKey = '*';
            else if (ke.KeyValue >= 37 && ke.KeyValue <= 40)
                sendKey = (char)(ke.KeyValue - 37);
            
            m_Game.ProcessKey(sendKey);
            if (m_Game.selCol > -1 || m_Game.selSlot > -1)
                ke.Handled = true;
            Refresh();
            
        }

        void Letters_Paint(object sender, PaintEventArgs pe)
        {
            m_Game.DrawLetters(pe.Graphics);
        }

        void BoardDisplay_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            var penBlack = new Pen(Color.Black);
            m_Game.Draw(graphics);
        }

        void Board_Click(object sender, EventArgs e)
        {
            
            MouseEventArgs me = (MouseEventArgs)e;
            int xCoord = me.X;
            int yCoord = me.Y;
            m_Game.BoardClick(xCoord, yCoord);
            Refresh();
            
        }

        void WordBox_Change(object sender, EventArgs e)
        {
            m_Game.selList = WordBox.SelectedIndex;
            m_Game.RefCur(WordBox.SelectedIndex);
            m_Game.selSlot = -1;
            m_Game.selRow = -1;
            m_Game.selCol = -1;
            Refresh();
        }

        void Letters_Click(object sender, EventArgs e)
        {
            ActiveControl = pLetters;
            
            MouseEventArgs me = (MouseEventArgs)e;
            m_Game.LettersClick(me.X, me.Y);
            Refresh();
        }

       
      

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            WordBox.Items.Clear();
            
            
            WordBox.Items.Add("Generating words...");
            Refresh();
            
            m_Game.ExtractSlots();
            List<wdescrip> words = new List<wdescrip>();
            double elapsed = m_Game.getAll(ref words);
            List<string> summaries = words.ConvertAll<string>(x => x.word + "-" + x.value + " points");
            
            WordBox.Enabled = true;
            WordBox.Items.Clear();
            WordBox.BeginUpdate();
            foreach (string s in summaries)
            {
                WordBox.Items.Add(s);
            }
            WordBox.EndUpdate();

            calTime.Text = "" + elapsed;

        }

        private void PlaceButton_Click(object sender, EventArgs e)
        {
            int selWord = WordBox.SelectedIndex;
            m_Game.Place(selWord);
            Refresh();
        }

       

    

        


    }

    public partial class Scrabble
    {
        const int TILE_HEIGHT = 30;
        const int TILE_WIDTH = 30;
        public int selRow = -1;
        public int selCol = -1;
        public int selList = -1;
        public int selSlot = -1;
        
        public wdescrip selWdescrip;
        private char[] slots = "       ".ToCharArray();
        private bool selDir = Scrabble.Right;
        const char LEFT_KEY = (char)0;
        const char UP_KEY = (char)1;
        const char RIGHT_KEY = (char)2;
        const char DOWN_KEY = (char)3;
        
        public static Font defaultFont = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);
        public static Font valueFont = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.SansSerif), 10);
        public static Font letterFont = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.SansSerif), 15);
        public static Pen defaultPen = new Pen(Brushes.Black, 3);
        public static Brush letterBrush = Brushes.MediumPurple;

        public void RefCur(int index)
        {
            if(curPossWords != null && index>=0 && index<curPossWords.Count)
                selWdescrip = curPossWords[index];
        }

        public void Place(int selIndex)
        {
            if(curPossWords !=null && selIndex >= 0 && selIndex < curPossWords.Count){
                wdescrip wd = curPossWords[selIndex];
                placeLetters(wd.Row, wd.Col, wd.word, wd.Dir);
                firstword = true;
            
            }
        }

        public void ProcessKey(char key)
        {
            if (selRow > -1 && selCol > -1)
            {
                if ((key >= 'a' && key <= 'z') || (key >= 'A' && key <= 'Z') )
                {
                    board[selRow, selCol] = key;
                    if (selDir == Scrabble.Right)
                    {
                        if (selCol < 14)
                            selCol++;
                    }

                    if (selDir == Scrabble.Down)
                    {
                        if (selRow < 14)
                            selRow++;
                    }
                }
                //delete key
                if (key == '\b')
                {
                    board[selRow, selCol] = (char)0;
                    if (selDir == Scrabble.Right && selCol > 0)
                        --selCol;
                    else if (selDir == Scrabble.Down && selRow > 0)
                        --selRow;


                }

                if (key == LEFT_KEY)
                {
                    if (selDir == Scrabble.Down)
                        selDir = Scrabble.Right;
                    else if(selCol>0)
                    {
                        --selCol;
                    }
                }

                else if (key == UP_KEY)
                {
                    if (selDir == Scrabble.Right)
                        selDir = Scrabble.Down;

                    else if (selRow > 0)
                        --selRow;
                }

                else if (key == RIGHT_KEY)
                {
                    if (selDir == Scrabble.Down)
                        selDir = Scrabble.Right;
                    else if (selCol < 14)
                        ++selCol;

                }

                else if (key == DOWN_KEY)
                {
                    if (selDir == Scrabble.Right)
                        selDir = Scrabble.Down;
                    else if (selRow < 14)
                        ++selRow;
                }

                firstword = !boardempty();
            }

            if (selSlot > -1)
            {
                if ((key >= 'a' && key <= 'z') || key == '*')
                {
                    slots[selSlot] = key;
                    if (selSlot < 6)
                        selSlot++;
                }

                else if (key == '\b')
                {
                    slots[selSlot] = (char)0;
                    if (selSlot > 0)
                        --selSlot;
                }

                else if (key == LEFT_KEY && selSlot > 0)
                    --selSlot;
                else if (key == RIGHT_KEY && selSlot < 6)
                    ++selSlot;
            }
        }

        public bool boardempty()
        {
            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                    if (board[i, j] != (char)0)
                        return false;
            return true;
        }

        public void BoardClick(int x, int y)
        {
            int row = y / TILE_HEIGHT;
            int col = x / TILE_WIDTH;
            selRow = row;
            selCol = col;
            selSlot = -1;
            selWdescrip = null;
        }

        public void LettersClick(int x, int y)
        {
            selSlot = x / TILE_WIDTH;
            selRow = -1;
            selCol = -1;
            selWdescrip = null;

        }

        private void DrawLetter(Graphics g, int x, int y, char letter, bool actualLetter)
        {
            Brush useBrush = actualLetter ? Brushes.Yellow : Brushes.LightGreen;
            g.FillRectangle(useBrush, x, y, TILE_WIDTH, TILE_HEIGHT);

            g.DrawRectangle(defaultPen, x, y, TILE_WIDTH, TILE_HEIGHT);
            g.DrawString(letter + "", letterFont
                , Brushes.Black, x, y);
            g.DrawString(letter_value(letter).ToString(), valueFont, Brushes.Black, char.IsUpper(letter)? x+15: x+12, y + 15);
        }

        public void ExtractSlots()
        {
            curletz = new List<char>();
            foreach (char c in slots)
            {
                if ((char.IsLower(c) || c== '*'))
                    curletz.Add(c);
            }
        }

        public void DrawLetters(Graphics g)
        {
            int x = 0;
            int y = 0;
            int i = 0;
            foreach (char c in slots)
            {
                
                
                
                char dispLetter = c;
                
                if (c == (char)0)
                    dispLetter = ' ';
                g.FillRectangle(dispLetter == ' '? letterBrush: Brushes.Yellow, x, y, TILE_WIDTH, TILE_HEIGHT);
                g.DrawString(dispLetter+"", letterFont
                    , Brushes.Black, x, y);
                if(! (dispLetter==' '))
                    g.DrawString(letter_value(dispLetter).ToString(), valueFont, Brushes.Black, x+12, y + 15);
                Pen usePen = defaultPen;
                if (i == selSlot)
                    usePen = new Pen(Brushes.YellowGreen, 5);
                g.DrawRectangle(usePen, x, y, TILE_WIDTH, TILE_HEIGHT);
                
                x += TILE_WIDTH;
                i++;

            }
        }

        public void Draw(Graphics g)
        {
            
 
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    DrawTile(i, j, g);
        }

        public char inWdescrip(wdescrip w, int row, int col)
        {
            if (w == null)
                return ' ';
            if (w.Dir == Scrabble.Right)
            {
                if (row == w.Row && col >= w.Col && col < w.Col + w.Length)
                {
                    return w.word[col - w.Col];
                    
                }
                return ' ';
                
            }

            else
            {
                if (col == w.Col && row >= w.Row && row < w.Row + w.Length)
                {
                    return w.word[row - w.Row];
                }
                return ' ';
            }
                
            
            
        }

        private void DrawTile(int row, int col, Graphics g)
        {
            int y = row * TILE_HEIGHT;
            int x = col * TILE_WIDTH;
            char c = inWdescrip(selWdescrip, row, col);
            //there is a letter
            if (board[row, col] != 0)
            {

                DrawLetter(g, x, y,board[row,col], true);

            }

            

            else if (c != ' '){
                DrawLetter(g,x,y,c,false);
            }

            

            else
            {
                Descriptor tileType = Desc[row, col];
                Brush backColor = DescBackColor(tileType);
                switch (tileType)
                {
                    case Descriptor.DoubleL:
                        g.DrawRectangle(defaultPen, x, y, TILE_WIDTH, TILE_HEIGHT);
                        g.FillRectangle(backColor, x, y, TILE_WIDTH, TILE_HEIGHT);
                        g.DrawString("Double", defaultFont, Brushes.Black, x - 2 , y + 2);
                        g.DrawString("Letter", defaultFont, Brushes.Black, x + 2, y + 10);
                        break;
                    case Descriptor.TripleL:
                        g.DrawRectangle(defaultPen, x, y, TILE_WIDTH, TILE_HEIGHT);
                        g.FillRectangle(backColor, x, y, TILE_WIDTH, TILE_HEIGHT);
                        g.DrawString("Triple", defaultFont, Brushes.Black, x + 2, y + 2);
                        g.DrawString("Letter", defaultFont, Brushes.Black, x + 2, y + 10);
                        
                        break;
                    case Descriptor.DoubleW:
                        g.DrawRectangle(defaultPen, x, y, TILE_WIDTH, TILE_HEIGHT);
                        g.FillRectangle(backColor, x, y, TILE_WIDTH, TILE_HEIGHT);
                        g.DrawString("Double", defaultFont, Brushes.Black, x - 2 , y + 2);
                        g.DrawString("Word", defaultFont, Brushes.Black, x + 2, y + 10);
                        break;
                    case Descriptor.TripleW:
                        g.DrawRectangle(defaultPen, x, y, TILE_WIDTH, TILE_HEIGHT);
                        g.FillRectangle(backColor, x, y, TILE_WIDTH, TILE_HEIGHT);
                        g.DrawString("Triple", defaultFont, Brushes.Black, x + 2, y + 2);
                        g.DrawString("Word", defaultFont, Brushes.Black, x + 2, y + 10);
                        break;
                    case Descriptor.Empty:
              
                        g.DrawRectangle(defaultPen, x, y, TILE_WIDTH, TILE_HEIGHT);
                        g.FillRectangle(backColor, x, y, TILE_WIDTH, TILE_HEIGHT);
                        
                        break;
                    
                        
                }

            }

            //highlight if in the zone, chief
            if ((row == selRow && selDir == Scrabble.Right) || (col == selCol && selDir == Scrabble.Down))
            {
                //Brush b = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal,Color.Yellow);
                Brush b = new SolidBrush(Color.FromArgb(90, Color.Yellow));
                g.FillRectangle(b, x, y, TILE_WIDTH, TILE_HEIGHT);
                

                if (row == selRow && col == selCol)
                {
                    Pen p = new Pen(Brushes.GreenYellow, 5);
                    g.DrawRectangle(p, x, y, TILE_WIDTH, TILE_HEIGHT);
                }

            }

        }




        private Brush DescBackColor(Descriptor d)
        {
            switch (d)
            {
                case Descriptor.DoubleL:
                    return Brushes.LightBlue;
                case Descriptor.TripleL:
                    return Brushes.Blue;
                case Descriptor.DoubleW:
                    //return Brushes.Salmon;
                    return Brushes.MediumVioletRed;
                case Descriptor.TripleW:
                    return Brushes.Red;
                case Descriptor.Empty:
                    return letterBrush;
                default:
                    return Brushes.Black;

            }
        }
    }

}
