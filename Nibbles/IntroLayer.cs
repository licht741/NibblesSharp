using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace Nibbles
{
    public class IntroLayer : CCLayerColor
    {

        float ResTruck(CCSprite sprite)
        {
            float scale = 0.0f;
            float resWid = CCScene.DefaultDesignResolutionSize.Width;
            float resHei = CCScene.DefaultDesignResolutionSize.Height;

            return 2;
        }

        private Field fields;

        public IntroLayer() : base(CCColor4B.Blue)
        {
            fields = new Field(17, 10);
            fields.Initialize();

            cntH = 17;
            cntW = 10;
            Schedule(RunGameLogic, 1);

            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesMoved = OnTouchesMoved;
            AddEventListener(touchListener, this);


        }
        void OnTouchesMoved (List<CCTouch> touches, CCEvent touchEvent)
        {
            var diffByX = touches[0].Location.X - touches[0].PreviousLocation.X;
            var diffByY = touches[0].Location.Y - touches[0].PreviousLocation.Y;

            MovingDirections movingDirection;
            if (Math.Abs(diffByX) > Math.Abs(diffByY))
                movingDirection = (diffByX > 0) ? MovingDirections.Right : MovingDirections.Left;
            else
                movingDirection = (diffByY > 0) ? MovingDirections.Up : MovingDirections.Down;

            fields.Direction = movingDirection;
        }
        
        public void RunGameLogic(float frameTimeInSeconds)
        {
            update();
        }

        int cntH;
        int cntW;

        public void update()
        {
            RemoveAllChildren();

            int maxH = 720;
            int maxW = 1280;

            int sizeX = (int)(maxW / (double)cntH);
            int sizeY = (int)(maxH / (double)cntW);

            float scaleX = (float)(sizeX / 32.0);
            float scaleY = (float)(sizeY / 32.0);

            fields.Step();

            var state = fields.GenerateState();
            for (int i = 0; i < cntH; ++i)
                for (int j = 0; j < cntW; ++j)
                {
                    var picture = "grass";

                    if (state[i][j].CellType == CellTypes.SnakeHead)
                        picture = "snake_head";
                    if (state[i][j].CellType == CellTypes.SnakeMiddle)
                        picture = "snake_middle";
                    if (state[i][j].CellType == CellTypes.SnakeTail)
                        picture = "snake_tail";
                    if (state[i][j].CellType == CellTypes.Food)
                        picture = "food";

                    var paddlesprite = new CCSprite(picture);
                    paddlesprite.ScaleX = scaleX;
                    paddlesprite.ScaleY = scaleY;
                    paddlesprite.PositionX = maxW - sizeX / 2 - sizeX * i;
                    paddlesprite.PositionY = maxH - sizeY / 2 - sizeY * j;
                    AddChild(paddlesprite);
                }

        }



        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;


            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                // Perform touch handling here
            }
        }
    }
}

