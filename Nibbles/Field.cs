using System;
using System.Collections.Generic;
using System.Linq;

namespace Nibbles
{
    enum CellTypes { Empty, Food, SnakeHead, SnakeMiddle, SnakeTail }
    enum MovingDirections { Left, Up, Right, Down, None }

    class GameCell
    {
        public CellTypes CellType { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public MovingDirections MovingDirection { get; set; }

        public void Move()
        {
            Move(MovingDirection);
        }

        public void Move(MovingDirections movDirection)
        {
            switch (movDirection)
            {
                case MovingDirections.Up:
                    PositionY -= 1;
                    break;
                case MovingDirections.Right:
                    PositionX -= 1;
                    break;
                case MovingDirections.Down:
                    PositionY += 1;
                    break;
                case MovingDirections.Left:
                    PositionX += 1;
                    break;
            }
        }
    }


    class Snake
    {
        public GameCell Head { get; set; }
        public List<GameCell> Middle { get; set; }
        public GameCell Tail { get; set; }
        public MovingDirections MovingDirection { get; set; }


        public static Snake GenerateRandomSnake(int maxWidth, int maxHeight)
        {
            var random = new Random();

            var initialMovingDirection = MovingDirections.Right;
            var head = new GameCell
            {
                CellType = CellTypes.SnakeHead,
                PositionX = random.Next(2, maxWidth - 2),
                PositionY = random.Next(2, maxHeight - 2),
                MovingDirection = initialMovingDirection
            };

            var tail = new GameCell
            {
                CellType = CellTypes.SnakeTail,
                PositionX = head.PositionX + 1,
                PositionY = head.PositionY,
                MovingDirection = head.MovingDirection
            };

            return new Snake
            {
                Head = head,
                Middle = new List<GameCell>(),
                Tail = tail,
                MovingDirection = MovingDirections.Right
            };
        }
    }

    class Field
    {
        public List<List<GameCell>> Cells { get; set; }
        private int _width;
        private int _height;

        //private

        public MovingDirections Direction
        {
            get
            {
                return snake.Head.MovingDirection;
            }

            set
            {
                if (Direction == MovingDirections.Up && value == MovingDirections.Down ||
                    Direction == MovingDirections.Down && value == MovingDirections.Up ||
                    Direction == MovingDirections.Left && value == MovingDirections.Right ||
                    Direction == MovingDirections.Right && value == MovingDirections.Left)
                        return;

                snake.MovingDirection = value;
                snake.Head.MovingDirection = value;
            }

        }

        public Field(int width, int height)
        {
            Cells = new List<List<GameCell>>();
            for (int i = 0; i < width; ++i)
            {
                Cells.Add(new List<GameCell>());
                for (int j = 0; j < height; ++j)
                {
                    Cells[i].Add(new GameCell
                    {
                        CellType = CellTypes.Empty,
                        PositionX = i,
                        PositionY = j,
                        MovingDirection = MovingDirections.None
                    });
                }
                    
            }
            _width = width;
            _height = height;
        }

        public int Width  { get { return _width;  } }
        public int Height { get { return _height; } }

        private bool PositionCollision(GameCell gc1, GameCell gc2)
        {
            return gc1.PositionX == gc2.PositionX &&
                   gc1.PositionY == gc2.PositionY;
        }

        private bool CollisionWithFood() {
            if (food == null)
                return false;

            return PositionCollision(snake.Head, food);
        } 

        private bool CollisionWithBody()
        {
            var collisionPlace = snake.Middle.FirstOrDefault(c => PositionCollision(c, snake.Head));
            if (collisionPlace != default(GameCell))
                return true;

            return PositionCollision(snake.Head, snake.Tail);
        }

        public void Step()
        {

            if (snake == null)
                return;

            var head = snake.Head;
            Cells[head.PositionX][head.PositionY].MovingDirection = head.MovingDirection;

            var prevX = head.PositionX;
            var prevY = head.PositionY;

            // Detect collision
            if (CollisionWithFood())
            {
                head.Move(snake.MovingDirection);
                food = GenerateFood(Width, Height);
                snake.Middle.Add(new GameCell
                {
                    PositionX = prevX,
                    PositionY = prevY,
                    CellType = CellTypes.SnakeMiddle,
                    MovingDirection = head.MovingDirection
                });

                return;
            }

            if (CollisionWithBody())
            {
                snake = null;
                food = null;

                return;
            }

            // tail
            head.Move(snake.MovingDirection);

            foreach (var cell in snake.Middle)
            {
                var movingDirectionMiddle = Cells[cell.PositionX][cell.PositionY].MovingDirection;
                if (movingDirectionMiddle == MovingDirections.None)
                    movingDirectionMiddle = Direction;
                cell.Move(movingDirectionMiddle);
            }

            var tail = snake.Tail;
            var movingDirection = Cells[tail.PositionX][tail.PositionY].MovingDirection;
            if (movingDirection == MovingDirections.None)
                movingDirection = Direction;

            prevX = tail.PositionX;
            prevY = tail.PositionY;

            tail.Move(movingDirection);

            Cells[prevX][prevY].CellType = CellTypes.Empty;

        }

        private Snake snake;
        private GameCell food;


        public void Initialize()
        {
            snake = Snake.GenerateRandomSnake(Width, Height);
            food = GenerateFood(Width, Height);
            var tail = snake.Tail;
            Cells[tail.PositionX][tail.PositionY].MovingDirection = snake.Head.MovingDirection;
        }

        Random random = new Random();

        public GameCell GenerateFood(int maxWidth, int maxHeight)
        {
            return new GameCell
            {
                CellType = CellTypes.Food,
                PositionX = random.Next(1, maxWidth - 1),
                PositionY = random.Next(1, maxHeight - 1),
                MovingDirection = MovingDirections.None
            };
        }

        public void Move()
        {
            switch (snake.MovingDirection)
            {
                case MovingDirections.Up:
                    snake.Head.PositionY -= 1;
                    break;
                case MovingDirections.Left:
                    snake.Head.PositionX += 1;
                    break;
                case MovingDirections.Right:
                    snake.Head.PositionX += 1;
                    break;
                case MovingDirections.Down:
                    snake.Head.PositionX -= 1;
                    break;
            }
        }

        public List<List<GameCell>> GenerateState()
        {
            var temp = new List<List<GameCell>>(Cells);

            if (snake != null)
            {
                var head = snake.Head;
                temp[head.PositionX][head.PositionY].CellType = CellTypes.SnakeHead;
                snake.Middle.ForEach(cell => temp[cell.PositionX][cell.PositionY].CellType = CellTypes.SnakeMiddle);

                var tail = snake.Tail;
                temp[tail.PositionX][tail.PositionY].CellType = CellTypes.SnakeTail;
            }

            if (food != null)
                temp[food.PositionX][food.PositionY].CellType = CellTypes.Food;

            return temp;
        }

    }
}