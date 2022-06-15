namespace Blocks
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GameSetting();
            GameStart();
        }

        BaseBlock block = null;
        private void btnStart_Click(object sender, EventArgs e)
        {
        }


        private void GameSetting()
        {
            picBox.Width = Setting.BoardWidth * Setting.pixwidth;
            picBox.Height = Setting.BoardHeight * Setting.pixwidth;
        }
        private void OnUpdateCanvas(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            GameBoard.Board.Render(g);
            if(block != null)
                block.Render(g);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    {
                        if (block != null)
                            block.Transform();
                    }
                    break;
                case Keys.Left:
                    {
                        if (block != null)
                        {
                            block.MoveLeft();
                        }
                    }
                    break;
                case Keys.Right:
                    {
                        if (block != null)
                        {
                            block.MoveRight();
                        }
                    }
                    break;
                case Keys.Space:
                    if (block != null)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (block.MoveDown() == false)
                            {
                                block = GameBoard.Board.CreateBlock();
                                break;
                            }
                        }
                    }
                    break;
                case Keys.Down:
                    {
                        if (block != null)
                        {
                            if (block.MoveDown() == false)
                                block = GameBoard.Board.CreateBlock();
                        }
                    }
                    break;
            }
            this.Focus();
            this.picBox.Invalidate();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void picBox_MouseDown(object sender, MouseEventArgs e)
        {
            picBox.Focus();
        }

        private int lastDownTickCount = 0;
        private int lastSpeedupTickCount = 0;
        private int CurrentLevel = 0;
        private int [] SpeedLevels = { 500, 400, 300, 200, 100, 50, 40,30,20,10 };
        private void OnTimeEvent(object sender, EventArgs e)
        {
            int now = Environment.TickCount;
            if (now - lastDownTickCount > SpeedLevels[CurrentLevel])
            {
                lastDownTickCount = now;
                if (block != null)
                {
                    if (block.MoveDown() == false)
                        block = GameBoard.Board.CreateBlock();
                }
                this.picBox.Invalidate();
            }
            if (lastSpeedupTickCount == 0)
                lastSpeedupTickCount = now;
            if (now - lastSpeedupTickCount > 10000)
            {
                //level up
                CurrentLevel++;
                lastSpeedupTickCount = now;
                this.Text = "Current Level:" + (CurrentLevel+1);
            }

        }
        public void GameStart()
        {
            block = GameBoard.Board.CreateBlock();
            GameBoard.Board.Clear();
            this.picBox.Focus();
            this.picBox.Invalidate();
            this.timer.Start();
            CurrentLevel = 0;
            lastSpeedupTickCount = 0;
            lastDownTickCount = 0;
            this.Text = "Current Level:" + (CurrentLevel+1);
        }
    }
}