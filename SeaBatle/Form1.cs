namespace SeaBatle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Model = new model();
            //Model.PlayerShips[0, 0] = CoordStatus.Ship;
            //Model.PlayerShips[5, 1] = CoordStatus.Ship;
            //Model.PlayerShips[5, 2] = CoordStatus.Ship;
            //Model.PlayerShips[5, 3] = CoordStatus.Ship;
            //Model.PlayerShips[5, 4] = CoordStatus.Ship;
            //Model.PlayerShips[7, 3] = CoordStatus.Ship;
        }
        model Model;
        private void button21_Click(object sender, EventArgs e)
        {

            Model.LastShot = Model.Shot(textBox1.Text);

            MessageBox.Show(Model.LastShot.ToString());
            if (Model.UndiscoverCells == 0) { MessageBox.Show("EndBattle"); }

        }

        private void button22_Click(object sender, EventArgs e)
        {
            string s = Model.ShotGen();
            textBox1.Text = s;

        }

        private void ShowCoordShip(object sender, EventArgs e)
        {
            textBox1.Text = (sender as Button).Tag as string;


        }

        //перерисовать
        private void button2_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                {
                    string name = "b" + x.ToString() + y.ToString();
                    var b = this.Controls.Find(name, true);

                    if (b.Count() > 0)
                    {
                        var btn = b[0];
                        switch (Model.PlayerShips[x, y])
                        {
                            case CoordStatus.Ship:
                                btn.Text = "x";
                                break;
                            case CoordStatus.None: btn.Text = ""; break;
                        }

                    }
                }
        }

        private void button1_Click(object sender, EventArgs e)//поставить
        {
            Direction direction;
            ShipType shipType = ShipType.x1;
            if (checkBox1.Checked) direction = Direction.Vertical; else direction = Direction.Horizontal;
            if (radioButton1.Checked) shipType = ShipType.x1;
            if (radioButton2.Checked) shipType = ShipType.x2;
            if (radioButton3.Checked) shipType = ShipType.x3;
            if (radioButton4.Checked) shipType = ShipType.x4;
            Model.AddDelShip(textBox1.Text, shipType, direction, checkBox2.Checked);
            button2_Click(sender, e);
        }
    }
}