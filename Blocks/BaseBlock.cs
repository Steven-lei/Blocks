using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocks
{ 
    public class Setting
    {
        public const int pixwidth = 20;
        public const int BoardWidth = 15;
        public const int BoardHeight = 20;
    }

    internal partial class GameBoard
    {
        private bool[,] map = new bool[Setting.BoardHeight, Setting.BoardWidth];

        private static GameBoard board = new GameBoard();
        static public GameBoard Board{get{return board;} }
        public bool[,] Map{get{return map;} }

        private int blockcount;
        public int BlockCount{ get { return blockcount; } }
        private int elimateLines;
        public int ElimateLines { get { return elimateLines; } }
        public void Clear()
        {
            for (int l = 0; l < map.GetLength(0); l++)
                for (int c = 0; c < map.GetLength(1); c++)
                {
                    map[l, c] = false;
                }

        }
        public void ReStart()
        {
            Clear();
            elimateLines = 0;
            blockcount = 0;
        }
        public virtual void Render(Graphics g)
        {
            for (int l = 0; l < map.GetLength(0); l++)
            {
                for (int c = 0; c < map.GetLength(1); c++)
                {
                    if (map[l, c])
                    {
                        g.FillRectangle(SystemBrushes.ControlDark, c * Setting.pixwidth,
                        l * Setting.pixwidth, Setting.pixwidth, Setting.pixwidth);
                    }
                    g.DrawRectangle(SystemPens.ButtonFace, c * Setting.pixwidth,
l * Setting.pixwidth, Setting.pixwidth, Setting.pixwidth);
                }
            }
        }
        public void Combine(BaseBlock block)
        {
            for (int l = 0; l < block.Map.GetLength(1); l++)
            {
                for (int c = 0; c < block.Map.GetLength(0); c++)
                {
                    if (block.Map[l, c])
                    {
                        System.Diagnostics.Debug.Assert(map[block.Position.Y + l, block.Position.X + c] == false);
                        map[block.Position.Y + l, block.Position.X + c] = true;
                    }
                }
            }
            CheckElimination();
        }
        public bool CheckInBoundary(BaseBlock block)
        {
            for (int l = block.Map.GetLength(1) - 1; l >= 0; l--)
            {
                for (int c = 0; c < block.Map.GetLength(0); c++)
                {
                    if (block.Map[l, c])
                    {
                        if (block.Position.X + c < 0 ||
                            block.Position.X + c >= Setting.BoardWidth)
                            return false;
                        if (block.Position.Y + l < 0 ||
                            block.Position.Y + l >= Setting.BoardHeight)
                            return false;
                    }
                }
            }
            return true;
        }
        public bool CheckConflict(BaseBlock block)
        {
            for (int l = block.Map.GetLength(1) - 1; l >= 0; l--)
            {
                for (int c = 0; c < block.Map.GetLength(0); c++)
                {
                    if (block.Map[l, c])
                    {
                        if(block.Position.Y + 1 >=0 && block.Position.Y + 1 < Setting.BoardHeight
                            && block.Position.X + c >=0 && block.Position.X + c < Setting.BoardWidth)
                        {
                            if (Map[block.Position.Y + l, block.Position.X + c])
                            {
                                return true;
                            }
                        }


                    }
                }
            }
            return false;
        }
        public void CheckElimination()
        {
            for (int l = map.GetLength(0)-1; l >=0 ; l--)
            {
                bool bAllFull = true;
                for (int c = 0; c < map.GetLength(1); c++)
                {
                    if (map[l, c] == false)
                    {
                        bAllFull = false;
                        break;
                    }
                }
                if (bAllFull)
                {
                    elimateLines++;
                    for (int t = l; t > 0; t--)
                    {
                        for (int c = 0; c < map.GetLength(1); c++)
                            map[t,c] = map[t - 1,c];
                    }
                    l++;
                }
            }
        }
    }
    internal class BaseBlock
    {
        protected int direction = 0;
        protected bool[,] map = { };
        public bool[,] Map{ get { return map; } }
        protected Point position = new Point(0, 0);
        public Point Position{ get { return position; } }

        public virtual void Transform()
        {
            int temp_direction = direction;
            bool [,] temp_map = map;
            DoTransform();
            if (GameBoard.Board.CheckConflict(this))
            {
                direction = temp_direction;
                map = temp_map;
            }
        }
        public virtual bool DoTransform()
        {
            direction++;
            return true;
        }
        public virtual void MoveLeft()
        {
            position.X--;
            if (!GameBoard.Board.CheckInBoundary(this)
                || GameBoard.Board.CheckConflict(this))    //cannot move
                position.X++;
        }
        public virtual void MoveRight()
        {
            position.X++;
            if (!GameBoard.Board.CheckInBoundary(this)
                || GameBoard.Board.CheckConflict(this))    //cannot move
                position.X--;
        }
        public virtual bool MoveDown()
        {
            position.Y++;
            if (!GameBoard.Board.CheckInBoundary(this)
                || GameBoard.Board.CheckConflict(this))    //cannot move
            {
                position.Y--;
                GameBoard.Board.Combine(this);
                return false;
            }
            return true;
        }
        public void Normalize()
        {
            if (position.X + GetSpaceLeft() < 0)
                position.X = -GetSpaceLeft();
            if (position.X + GetMaxWidth() >= Setting.BoardWidth)
                position.X = Setting.BoardWidth - GetMaxWidth();
        }
        public virtual int GetSpaceLeft()
        {
            int minCol = -1;
            for (int c = 0; c < map.GetLength(0); c++)
            {
                for (int l = 0; l < map.GetLength(1); l++)
                {
                    if (map[l, c])
                    {
                        minCol = c;
                        break;
                    }
                }
                if (minCol >= 0)
                    break;
            }
            return minCol;
        }
        //can override this to get perfomance
        public virtual int GetMaxWidth()
        {
            int maxcol = 0;
            for (int c = map.GetLength(0) - 1; c >= 0; c--)
            {
                for (int l = 0; l < map.GetLength(1); l++)
                {
                    if (map[l, c])
                    {
                        maxcol = c + 1;
                        break;
                    }
                }
                if (maxcol > 0)
                    break;
            }
            return maxcol;
        }
        public virtual int GetMaxHeight()
        {
            int maxline = 0;
            for (int l = map.GetLength(1) - 1; l >= 0; l--)
            {
                for (int c = 0; c < map.GetLength(0); c++)
                {
                    if (map[l, c])
                    {
                        maxline = l + 1;
                        break;
                    }
                }
                if (maxline > 0)
                    break;
            }
            return maxline;
        }

        
        public virtual void Render(Graphics g)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (map[i, j])
                    {
                        g.FillRectangle(SystemBrushes.ControlDark, (position.X + j) * Setting.pixwidth,
                        (position.Y + i) * Setting.pixwidth, Setting.pixwidth, Setting.pixwidth);
                    }
                    g.DrawRectangle(SystemPens.ActiveCaption, (position.X + j) * Setting.pixwidth,
(position.Y + i) * Setting.pixwidth, Setting.pixwidth, Setting.pixwidth);
                    //}
                }
            }
        }
    }

    internal partial class GameBoard
    {
        //
        //  *****
        internal class Block1 : BaseBlock
        {
            public Block1()
            {
                this.map = new bool[,]{
                  { false,false,false,false,false},
                  { false,false,false,false,false},
                  { true,true,true,true,true },
                  { false,false,false,false,false},
                  { false,false,false,false,false},
                };
            }
            public override bool DoTransform()
            {
                direction++;
                direction = direction % 2;
                switch(direction)
                {
                    case 0:
                        {
                            //changing from vertical to horizon
                            this.map = new bool[,]
                            {

                                { false,false,false,false,false},
                                { false,false,false,false,false},
                                { true,true,true,true,true },
                                { false,false,false,false,false},
                                { false,false,false,false,false},
                            };
                        }
                        break;
                    case 1://change from horizon to vertical
                        {
                            this.map = new bool[,]
                            {
                                { false,false,true,false,false},
                                { false,false,true,false,false},
                                { false,false,true,false,false },
                                { false,false,true,false,false},
                                { false,false,true,false,false},
                            };
                        }
                        break;
                }
                Normalize();
                return true;
            }


        }
        //  **
        //  **

        internal class Block2 : BaseBlock
        {
            public Block2()
            {
                this.map = new bool[,]{
                { true,true,false,false,false},
                { true,true,false,false,false},
                { false,false,false,false,false},
                { false,false,false,false,false},
                { false,false,false,false,false},
            };
            }
        }
        //    **
        //     **
        internal class Block3 : BaseBlock
        {
            public Block3()
            {
                this.direction = 0;
                this.map = new bool[,]
                {
                { true,true,false,false,false},
                { false,true,true,false,false},
                { false,false,false,false,false },
                { false,false,false,false,false},
                { false,false,false,false,false},
                };
            }
            public override bool DoTransform()
            {
                direction++;
                direction = direction % 2;
                switch (direction)
                {
                    case 0:
                        {
                            this.map = new bool[,]
                            {
                            { true,true,false,false,false},
                            { false,true,true,false,false},
                            { false,false,false,false,false },
                            { false,false,false,false,false},
                            { false,false,false,false,false},
                            };
                            break;
                        }
                    case 1:
                        {

                            this.map = new bool[,]
                            {
                            { false,true,false,false,false},
                            { true,true,false,false,false},
                            { true,false,false,false,false },
                            { false,false,false,false,false},
                            { false,false,false,false,false},
                            };
                            break;
                        }
                }
                Normalize();
                return true;
            }
        }
        //   **
        //  **
        internal class Block4 : BaseBlock
        {
            public Block4()
            {
                this.direction = 0;
                this.map = new bool[,]
                {
                { false,true,true,false,false},
                { true,true,false,false,false},
                { false,false,false,false,false },
                { false,false,false,false,false},
                { false,false,false,false,false},
                };
            }
            public override bool DoTransform()
            {
                direction++;
                direction = direction % 2;
                switch (direction)
                {
                    case 0:
                        {
                            this.map = new bool[,]
                            {
                            { false,true,true,false,false},
                            { true,true,false,false,false},
                            { false,false,false,false,false },
                            { false,false,false,false,false},
                            { false,false,false,false,false},
                            };
                            break;
                        }
                    case 1:
                        {
                            this.map = new bool[,]
                            {
                            { true,false,false,false,false},
                            { true,true,false,false,false},
                            { false,true,false,false,false },
                            { false,false,false,false,false},
                            { false,false,false,false,false},
                            };
                            break;
                        }
                }
                Normalize();
                return true;
            }
        }
        //   *
        //  ***
        internal class Block5 : BaseBlock
        {
            public Block5()
            {
                this.direction = 0;
                this.map = new bool[,]
                {
                { false,true,false,false,false},
                { true,true,true,false,false},
                { false,false,false,false,false },
                { false,false,false,false,false},
                { false,false,false,false,false},
                };
            }
            public override bool DoTransform()
            {
                direction++;
                direction = direction % 4;
                switch (direction)
                {
                    case 0:
                        {
                            this.map = new bool[,]
                            {
                            { false,true,false,false,false},
                            { true,true,true,false,false},
                            { false,false,false,false,false },
                            { false,false,false,false,false},
                            { false,false,false,false,false},
                            };
                            break;
                        }
                    case 1:
                        {
                            this.map = new bool[,]
                            {
                            { false,true,false,false,false},
                            { false,true,true,false,false},
                            { false,true,false,false,false },
                            { false,false,false,false,false},
                            { false,false,false,false,false},
                            };
                            break;
                        }
                    case 2:
                        {
                            this.map = new bool[,]
                            {
                            { false,false,false,false,false},
                            { true,true,true,false,false},
                            { false,true,false,false,false },
                            { false,false,false,false,false},
                            { false,false,false,false,false},
                            };
                            break;
                        }
                    case 3:
                        {
                            this.map = new bool[,]
                            {
                            { false,true,false,false,false},
                            { true,true,false,false,false},
                            { false,true,false,false,false },
                            { false,false,false,false,false},
                            { false,false,false,false,false},
                            };
                            break;
                        }
                }
                Normalize();
                return true;
            }
        }
        public BaseBlock CreateBlock()
        {
            blockcount++;
            BaseBlock? block = null;
            Random random = new Random();
            int num = random.Next(1, 6);
            switch (num)
            {
                case 1:
                    block = new Block1();
                    break;
                case 2:
                    block = new Block2();
                    break;
                case 3:
                    block = new Block3();
                    break;
                case 4:
                    block = new Block4();
                    break;
                case 5:
                    block = new Block5();
                    break;
            }
            return block;
        }
    }
}
