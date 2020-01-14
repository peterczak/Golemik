using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Golem
{
    
    public partial class Form1 : Form
    {
        private Boolean wasClicked = false; // czy zostal wybrany pionek
        private string lastMovement;
        private int lastCordX;
        private int lastCordY;
        Boolean anotherJump;
        private List<KeyValuePair<int, int>> myList = new List<KeyValuePair<int, int>>();
        private List<KeyValuePair<int, int>> allMovements = new List<KeyValuePair<int, int>>();

        public Form1()
        {
            InitializeComponent();
            
        }
        public void clearBoard()  // kolorujemy szachownice po tych zmienionych kolorach
        {
            int counter = 0;
            for(int i=0; i<8; i++)
            {
                for (int j = 0; j<8; j++)
                {
                    Control[] cs = this.Controls.Find("pic" + i + j, true); // lapie konkretne pole (kwadratu np pic05)
                    PictureBox pb = cs[0] as PictureBox;
                    if ((i % 2 == 0 && j % 2 == 0)  || (i%2 == 1 && j % 2 == 1))
                        pb.BackColor = Color.FromName("BurlyWood");
                    else pb.BackColor = Color.FromName("Sienna");
                    counter++;
                }

            }
        }
        private void OnClick(object sender, EventArgs e)
        {
            clearBoard();
            disableImages();
            if (lastMovement == "white") enableImage("black");
            else enableImage("white");
            PictureBox p = sender as PictureBox;
            int cordX = int.Parse(p.Name.Substring(3, 1)); //pic05
            int cordY = int.Parse(p.Name.Substring(4, 1)); 
            Control[] test = this.Controls.Find("pic" + cordX + cordY, true);
            PictureBox thesame = test[0] as PictureBox;
            if (!wasClicked || thesame.Image != null)// albo nacisniety po raz kolejny pionek
            {
                wasClicked = true; 
                for (int i = -1; i < 2; i++) // petla ktore podaje determinuje zakres ruchu pionka
                {
                    for (int j =-1; j <2; j++)
                    {
                        int x = cordX + i;
                        int y = cordY + j;
                        if ((i + cordX) >= 0 && (i + cordX) < 8 && (j + cordY) >= 0 && j + cordY < 8)
                        {
                            Control[] cs = this.Controls.Find("pic" + x + y, true);
                            PictureBox pb = cs[0] as PictureBox;
                            if (pb.Image == null && !anotherJump)
                            {
                                pb.Enabled = true;
                                pb.BackColor = Color.FromName("green");
                            }
                            else if(pb.Image != null)
                            {
                                checkJumping(x, i, y, j);
                            }
                        }
                    }
                }
               
                lastCordX = cordX;
                lastCordY = cordY;
                allMovements.Insert(0, new KeyValuePair<int, int>(cordX, cordY));
            }
            else
            {      
                if (cordX != lastCordX || cordY != lastCordY)
                {
                    anotherJump = false;
                    Control[] cs = this.Controls.Find("pic" + lastCordX + lastCordY, true);
                    PictureBox pb = cs[0] as PictureBox;
                    if (pb.Image.Tag == "black")
                    {
                        p.Image = Image.FromFile(@"pawnBlack.png");
                        p.Image.Tag = "black";
                    }
                    else
                    {
                        p.Image = Image.FromFile(@"pawnWhite.png");
                        p.Image.Tag = "white";
                    }
                        foreach (KeyValuePair<int, int> pair in myList)
                        {
                            if (pair.Key == cordX && pair.Value == cordY )
                            {
                                anotherJump = true;
                            }
                        }
                    if (anotherJump)
                    {
                        myList = new List<KeyValuePair<int, int>>();
                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                int x = cordX + i;
                                int y = cordY + j;
                                if ((i + cordX) >= 0 && (i + cordX) < 8 && (j + cordY) >= 0 && j + cordY < 8)
                                {
                                    Control[] cs2 = this.Controls.Find("pic" + x + y, true);
                                    PictureBox pb2 = cs2[0] as PictureBox;
                                    if (pb2.Image != null)
                                    {
                                        checkJumping(x, i, y, j);
                                    }
                                }
                            }
                        }
                        if(myList.Count == 1)
                        {
                            if (myList[0].Key == lastCordX && myList[0].Value == lastCordY) anotherJump = false;
                        }
                        if (myList.Count == 0) anotherJump = false;
                    }
                    clearBoard();
                    pb.Image = null;
                    pb.Refresh();
                    p.Refresh();
                    wasClicked = false;
                    Control[] kolej = this.Controls.Find("kolej", true);
                    PictureBox imageKolej = kolej[0] as PictureBox;
                    disableImages();
                   
                    if (anotherJump)
                    {
                        p.Enabled = true;
                    }
                    else if (lastMovement == "black")

                    {
                    enableImage("black");

                        imageKolej.Image = Image.FromFile(@"pawnBlack.png");
                        lastMovement = "white";
                    }
                    else
                    {
                        enableImage("white");
    
                        imageKolej.Image = Image.FromFile(@"pawnWhite.png");
                        lastMovement = "black";
                    }
                    myList = new List<KeyValuePair<int, int>>();

                    
                    allMovements.Insert(0, new KeyValuePair<int, int>(cordX, cordY));
                    isWinnner();
                }
            }           
        }

        private void button1_Click(object sender, EventArgs e) // start gry
        {
            clearBoard();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Control[] cs = this.Controls.Find("pic" + i + j, true);
                    PictureBox pb = cs[0] as PictureBox;
                    pb.Enabled = false;
                    pb.Image = null;
                }
            }
            endTurn.Visible = true;
            lastMovement = "white";
            Control[] Kolej = this.Controls.Find("LabelKolej", true);
            Label labelKolej = Kolej[0] as Label;
            labelKolej.Visible = true;
            Control[] con = this.Controls.Find("kolej", true);
            PictureBox kolej = con[0] as PictureBox;
            kolej.Image = Image.FromFile(@"pawnBlack.png");
          
            for(int i=0; i<2; i++)
            {
                for(int j=0; j<8; j++)
                {
                    Control[] cs = this.Controls.Find("pic"+i+j, true);
                    PictureBox pb = cs[0] as PictureBox;
                    pb.Enabled = false;
                    pb.Image = Image.FromFile(@"pawnWhite.png");
                    pb.Image.Tag = "white";
                    pb.Refresh();
                }
            }
            for (int i = 6; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Control[] cs = this.Controls.Find("pic" + i + j, true);
                    PictureBox pb = cs[0] as PictureBox;
                    pb.Enabled = true;
                    pb.Image = Image.FromFile(@"pawnBlack.png");
                    pb.Image.Tag = "black";
                    pb.Refresh();
                }
            }
        }
        private void disableImages()
        {
            for(int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++)
                {
                    Control[] cs = this.Controls.Find("pic" + i + j, true);
                    PictureBox pb = cs[0] as PictureBox;
                    pb.Enabled = false;
                }
            }
        }
        private void enableImage(string name)
        {
            Image image1 = Image.FromFile(@"pawnWhite.png");
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Control[] cs = this.Controls.Find("pic" + i + j, true);
                    PictureBox pb = cs[0] as PictureBox;
                    if (pb.Image != null)
                    {
                        if((string)pb.Image.Tag == name)
                        {
                            pb.Enabled = true;
                        }
                      
                    }
                }
            }
        }
        private Boolean isWinnner() // do wygrania
        {
            bool isWinner = true;
            for(int i=0; i<2; i++)
            {
                for (int j=0; j<8; j++)
                {
                    Control[] cs = this.Controls.Find("pic" + i + j, true);
                    PictureBox pb = cs[0] as PictureBox;
                    if (pb.Image == null) isWinner = false;
                    else if (pb.Image.Tag == "white") isWinner = false;
                }
            }
            if (isWinner)
            {
                MessageBox.Show("Mamy zwycięzce Czarnego!");
                disableImages();
                return true;
            }
            
            isWinner = true;
            for (int i = 6; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Control[] cs = this.Controls.Find("pic" + i + j, true);
                    PictureBox pb = cs[0] as PictureBox;
                    if (pb.Image == null) isWinner = false;
                    else if (pb.Image.Tag == "black") isWinner = false;
                }
            }
            if(isWinner)
            {
                MessageBox.Show("Mamy zwycięzce Białego");
                disableImages();
                return true;
            }
            return false;

        }
        private Boolean checkJumping(int cordX, int directionX, int cordY, int directionY) // sprawdzal czy pole istnieje w planszy, dodaje
            //tablicy wszystkie mozliwe skoki, koloruje na czarno 
        {

        Boolean isPossible = false;
            
                    if ((directionX + cordX) >= 0 && (directionX + cordX) < 8 && (directionY + cordY) >= 0 && directionY + cordY < 8)
                    {
                         int x = directionX + cordX;
                         int y = directionY + cordY;
                        Control[] cs = this.Controls.Find("pic" + x + y, true);
                        PictureBox pb = cs[0] as PictureBox;
                        if (pb.Image == null)
                        {
                            pb.Enabled = true;
                             myList.Add(new KeyValuePair<int, int>(x, y));
                            pb.BackColor = Color.FromName("black");
                            isPossible = true;
                        }
                    }
                 
            
            return isPossible;
        }

        private void button2_Click(object sender, EventArgs e) // nastepna tura
        {
            clearBoard();
            disableImages();
            if (lastMovement == "black")
            {
                enableImage("black");
                lastMovement = "white";
                Control[] con = this.Controls.Find("kolej", true);
                PictureBox kolej = con[0] as PictureBox;
                kolej.Image = Image.FromFile(@"pawnBlack.png");
            }
            else
            {
                enableImage("white");
                lastMovement = "black";
                Control[] con = this.Controls.Find("kolej", true);
                PictureBox kolej = con[0] as PictureBox;
                kolej.Image = Image.FromFile(@"pawnWhite.png");
            }
            wasClicked = false;
            anotherJump = false;
      
        }
    }
}
