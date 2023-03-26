using SteelSeries.GameSense;
using SteelSeries.GameSense.DeviceZone;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GSTest : MonoBehaviour
{
    GSClient client;

    struct Point
    {
        public Vector2Int currentPos;
        public Vector2Int previousPos;
    }

    Point[] points = new Point[5];

    enum MoveDirection
    {
        left,
        right,
        up,
        down
    }

    MoveDirection lastMoveDirection;

    byte[][] gameArea = new byte[][]
    {
        new byte[] { 20, 26, 08, 21, 23, 28, 24, 12, 18, 19, 47, 48 }, //Q Line
        new byte[] { 04, 22, 07, 09, 10, 11, 13, 14, 15, 51, 52 },     //A Line
        new byte[] { 29, 27, 06, 25, 05, 17, 16, 54, 55, 56 }          //Z Line  
    };

    public RGBPerkeyZoneCustom gameZone;
    ColorGradient colorGradient;
    ColorHandler handler;

    void Awake()
    {
        client = GSClient.Instance;
        gameZone = ScriptableObject.CreateInstance<RGBPerkeyZoneCustom>();
        colorGradient = ColorGradient.Create(new RGB(255, 0, 0), new RGB(255, 0, 0));
        handler = ColorHandler.Create(gameZone, IlluminationMode.Color, colorGradient);

        points[0].currentPos = new Vector2Int(7, 1);
        points[0].previousPos = new Vector2Int(7, 1);

        points[1].currentPos = new Vector2Int(6, 1);
        points[1].previousPos = new Vector2Int(6, 1);

        points[2].currentPos = new Vector2Int(5, 1);
        points[2].previousPos = new Vector2Int(5, 1);

        points[3].currentPos = new Vector2Int(4, 1);
        points[3].previousPos = new Vector2Int(4, 1);

        points[4].currentPos = new Vector2Int(3, 1);
        points[4].previousPos = new Vector2Int(3, 1);

        client.Events[0].handlers[0] = handler;

        RefreshSnake();
    }

    float movePerSeconds = 1;

    void Update()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movePerSeconds = 1;
            GoLeft();
            lastMoveDirection = MoveDirection.left;
        }

        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            movePerSeconds = 1;
            GoRight();
            lastMoveDirection = MoveDirection.right;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            movePerSeconds = 1;
            GoUp();
            lastMoveDirection = MoveDirection.up;
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            movePerSeconds = 1;
            GoDown();
            lastMoveDirection = MoveDirection.down;
        }
    }

    private void GoLeft()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].previousPos = points[i].currentPos;

            if (i == 0)
            {
                points[i].currentPos.x = points[i].currentPos.x == 0 ? gameArea[points[i].currentPos.y].Length - 1 : points[i].currentPos.x - 1;
            }

            else
            {
                points[i].currentPos = points[i - 1].previousPos;
            }
        }

        RefreshSnake();
    }

    private void GoRight()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].previousPos = points[i].currentPos;

            if (i == 0)
            {
                points[i].currentPos.x = points[i].currentPos.x == gameArea[points[i].currentPos.y].Length - 1 ? 0 : points[i].currentPos.x + 1;
            }

            else
            {
                points[i].currentPos = points[i - 1].previousPos;
            }
        }

        RefreshSnake();
    }

    private void GoUp()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].previousPos = points[i].currentPos;

            if (i == 0)
            {
                points[i].currentPos.y = points[i].currentPos.y == 0 ? 2 : points[i].currentPos.y - 1;
                points[i].currentPos.x = points[i].currentPos.x > gameArea[points[i].currentPos.y].Length - 1 ? gameArea[points[i].currentPos.y].Length - 1 : points[i].currentPos.x;
            }

            else
            {
                points[i].currentPos = points[i - 1].previousPos;
            }
        }

        RefreshSnake();
    }

    private void GoDown()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].previousPos = points[i].currentPos;

            if (i == 0)
            {
                points[i].currentPos.y = points[i].currentPos.y == 2 ? 0 : points[i].currentPos.y + 1;
                points[i].currentPos.x = points[i].currentPos.x > gameArea[points[i].currentPos.y].Length - 1 ? gameArea[points[i].currentPos.y].Length - 1 : points[i].currentPos.x;
            }

            else
            {
                points[i].currentPos = points[i - 1].previousPos;
            }
        }

        RefreshSnake();
    }

    private void RefreshSnake()
    {
        List<byte> pointPositions = new List<byte>();

        foreach (var item in points)
        {
            pointPositions.Add(gameArea[item.currentPos.y][item.currentPos.x]);
        }

        gameZone.zone = pointPositions.ToArray();

        handler.deviceZone = gameZone;

        client.BindEvent("SNAKE_POSITION", 0, 100, EventIconId.Default, new AbstractHandler[] { handler });
        client.SendEvent("SNAKE_POSITION", 100);
    }
}
