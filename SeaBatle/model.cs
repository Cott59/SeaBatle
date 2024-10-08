﻿using SeaBatle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace SeaBatle
{
    public enum ShotStatus //Статус выстрела
    { 
        Miss, //Промазал
        Wounded, //Ранил
        Kill, // Убил
       // EndBattle // Конец боя
    } 
    public enum CoordStatus //Статус координат
    {
        None, //Пусто
        Ship, //Корабль
        Shot, // Выстрел
        Got //Попал
    }
    public enum ShipType //Типы кораблей
    {x4, x3, x2, x1 }
    public enum Direction // Направление
    { Horizontal, Vertical}

    public class model
    {
        //Массив координат своих кораблей (кораблей игрока)
        public CoordStatus[,] PlayerShips = new CoordStatus[10, 10];
        //Массив координат кораблей противника
        public CoordStatus[,] EnemyShips = new CoordStatus[10, 10];
        //Количество клеток кораблей противника
        public int UndiscoverCells = 20;
        //Поле статус ранения
        public bool WoundedStatus;

        //Поле статуса последнего выстрела
        public ShotStatus LastShot;
              
        //Поле статус первого попадания
        public bool FirstGot;

        //Поле координат первого попадания
        public string? FirstGotCoord;
        // Поле координат последнего выстрела
        public string? LastShotCoord;

        //Конструктор. Инициализация полей модели
        public model()
        {
            LastShot = ShotStatus.Miss;
            WoundedStatus = false;
            for (int i = 0; i < 10 ; i++)
                for (int j = 0; j < 10 ; j++)
                {
                    PlayerShips[i, j] = CoordStatus.None;
                    EnemyShips[i, j] = CoordStatus.None;
                }
            FirstGotCoord = "";
            LastShotCoord = "";
            FirstGot = false;
        }

        //проверка наличия целой части корабля около данной точки
        public bool CheckShip(int x,int y)
        {
            if ((x != 9 && PlayerShips[x + 1, y] == CoordStatus.Ship ) ||
                (y != 9 && PlayerShips[x, y + 1] == CoordStatus.Ship) ||
                (x != 0 && PlayerShips[x - 1, y] == CoordStatus.Ship) ||
                (y != 0 && PlayerShips[x, y - 1] == CoordStatus.Ship))
                     return true;
            if ((x != 9 && PlayerShips[x + 1, y] == CoordStatus.Got) ||
                (y != 9 && PlayerShips[x, y + 1] == CoordStatus.Got) ||
                (x != 0 && PlayerShips[x - 1, y] == CoordStatus.Got) ||
                (y != 0 && PlayerShips[x, y - 1] == CoordStatus.Got))
                return true;
            return false;
        }
        public bool CheckShip(string CheckString)
        {
            int x = int.Parse(CheckString.Substring(0, 1));
            int y = int.Parse(CheckString.Substring(1));
            if ((x != 9 && PlayerShips[x + 1, y] == CoordStatus.Ship) ||
                (y != 9 && PlayerShips[x, y + 1] == CoordStatus.Ship) ||
                (x != 0 && PlayerShips[x - 1, y] == CoordStatus.Ship) ||
                (y != 0 && PlayerShips[x, y - 1] == CoordStatus.Ship))
                return true;
            return false;
        }

        //Генерация координаты второй точки
        public string NewLastShotCoord(string LastShotCoord)
        {
            Random rand = new Random();
            int x=0;
            int y=0;
            bool CheckCoord=true;
            while (CheckCoord==true)
            {
                x = int.Parse(LastShotCoord.Substring(0, 1));
                y = int.Parse(LastShotCoord.Substring(1));
                int q = rand.Next(1, 4);
                switch (q)
                {
                    case 1: x++; break;
                    case 2: x--; break;
                    case 3: y++; break;
                    case 4: y--; break;
                }
                if(PlayerShips[x, y] != CoordStatus.Shot)
                    CheckCoord=false;
            }
            LastShotCoord = x.ToString() + y.ToString();        
            return LastShotCoord;
        }

        public string NewRandomCoordPoint(string RandomCoordPoint)
        {
            bool CheckCoord = true;
            Random rand = new Random();
            int x=0;
            int y=0;
            while (CheckCoord == true)
            {
                x = rand.Next(0, 9);
                y = rand.Next(0, 9);
                if ((PlayerShips[x, y] != CoordStatus.Shot)&& (PlayerShips[x, y] != CoordStatus.Got))
                    CheckCoord = false;
            }
            return  RandomCoordPoint= x.ToString() + y.ToString();
        }


        //Выстрел. Входящий параметр - координаты выстрела в виде строки из 2х цифр
        //проверка статуса выстрела
        public ShotStatus Shot(string ShotCoord)
        {
            ShotStatus result = ShotStatus.Miss;
            int x, y; //координаты выстрела в числовом виде
            x = int.Parse(ShotCoord.Substring(0, 1));
            y = int.Parse(ShotCoord.Substring(1));
            if (PlayerShips[x, y] == CoordStatus.None) // Промах
            { 
                result = ShotStatus.Miss;
                PlayerShips[x, y]= CoordStatus.Shot;               
            }
            else 
            {
                if (CheckShip(x, y) == false)
                {
                    result = ShotStatus.Kill;// для однопалубного
                    UndiscoverCells--;
                    PlayerShips[x, y] = CoordStatus.Got;
                }
                else
                {
                    result = ShotStatus.Wounded;
                    PlayerShips[x, y] = CoordStatus.Got;
                    UndiscoverCells--;
                    if ((FirstGotCoord == LastShotCoord) && (FirstGot == false))// первое пападание
                    {
                        FirstGotCoord = x.ToString() + y.ToString();
                        LastShotCoord = x.ToString() + y.ToString();
                        FirstGot = true;
                        WoundedStatus = true;
                    }
                    else
                    {
                        LastShotCoord = x.ToString() + y.ToString();
                        WoundedStatus = true;
                    }                  

                    if ((CheckShip(FirstGotCoord) == false) && (CheckShip(LastShotCoord) == false))
                    {
                        result = ShotStatus.Kill;
                        FirstGot = false;
                        WoundedStatus = false;
                        FirstGotCoord = "";
                        LastShotCoord = "";
                    }
                }               
            }
            return result;
        }

        //Генерация выстрела, генерация нужных координат
        public string ShotGen()
        {
            string result="";                    
            
            if (FirstGot ==false)
            {
                result = NewRandomCoordPoint(result);
                //int x = rand.Next(0, 9);
                //int y = rand.Next(0, 9);
                //result = x.ToString() + y.ToString();
            }
            else
            {
                if ((FirstGotCoord == LastShotCoord) && (FirstGot == true))// первое пападание
                {
                    LastShotCoord = NewLastShotCoord(LastShotCoord);
                }
                else
                {
                    if ((FirstGotCoord != LastShotCoord) && (FirstGot == true) )
                    {
                        int x = int.Parse(LastShotCoord.Substring(0, 1));
                        int y = int.Parse(LastShotCoord.Substring(1));


                        if (PlayerShips[x, y] == CoordStatus.Shot)
                        {
                            LastShotCoord = FirstGotCoord;
                            LastShotCoord = NewLastShotCoord(LastShotCoord);
                        }
                        if((CheckShip(LastShotCoord) == true)&& (PlayerShips[x, y] == CoordStatus.Got))
                        {
                            int x1 = int.Parse(FirstGotCoord.Substring(0, 1));
                            int y1 = int.Parse(FirstGotCoord.Substring(1));
                            int x2 = int.Parse(LastShotCoord.Substring(0, 1));
                            int y2 = int.Parse(LastShotCoord.Substring(1));
                            if (x1 > x2) { x--; }
                            if (x1 < x2) { x++; }
                            if (y1 > y2) { y--; }
                            if (y1 < y2) { y++; }
                            LastShotCoord=x.ToString()+y.ToString();
                        }
                        if ((CheckShip(LastShotCoord) == false) && (PlayerShips[x, y] == CoordStatus.Got))
                        {
                            if (CheckShip(FirstGotCoord) == true)
                            {
                                int x1 = int.Parse(FirstGotCoord.Substring(0, 1));
                                int y1 = int.Parse(FirstGotCoord.Substring(1));
                                int x2 = int.Parse(LastShotCoord.Substring(0, 1));
                                int y2 = int.Parse(LastShotCoord.Substring(1));
                                if (x1 > x2) { x = x1 + 1; }
                                if (x1 < x2) { x = x1 - 1; }
                                if (y1 > y2) { y = y1 + 1; }
                                if (y1 < y2) { y = y1 - 1; }
                                LastShotCoord = x.ToString() + y.ToString();
                            }
                        }
                    }
                }
                result = LastShotCoord;
            }
            return result;
        }

        public bool CheckCoord(string xy, ShipType Type, Direction direction = Direction.Vertical)
        {
            bool result = true;
            return result;
        }
        //Добавляет или удаляет корабль 
        // ху - координаты корабля, Type- тип корабля, direction - направление размещения корабля, deleting - удалять или добавлять
        //В случае успешной операции возвращает true
        public bool AddDelShip(string xy, ShipType Type, Direction direction = Direction.Vertical, bool deleting = false)
        {
            bool result = true;
            if (deleting || CheckCoord(xy, Type, direction))
            {
                int x = int.Parse(xy.Substring(0, 1));
                int y = int.Parse(xy.Substring(1));
                CoordStatus status = new CoordStatus();
                if (deleting) status = CoordStatus.None; else status = CoordStatus.Ship;
                PlayerShips[x, y] = status;
                if (direction == Direction.Vertical)
                {
                    switch (Type)
                    {
                        case ShipType.x2:
                            PlayerShips[x, y + 1] = status;
                            break;
                        case ShipType.x3:
                            PlayerShips[x, y + 1] = status;
                            PlayerShips[x, y + 2] = status;
                            break;
                        case ShipType.x4:
                            PlayerShips[x, y + 1] = status;
                            PlayerShips[x, y + 2] = status;
                            PlayerShips[x, y + 3] = status;
                            break;
                    }
                }
                else
                {
                    switch (Type)
                    {
                        case ShipType.x2:
                            PlayerShips[x + 1, y] = status;
                            break;
                        case ShipType.x3:
                            PlayerShips[x + 1, y] = status;
                            PlayerShips[x + 2, y] = status;
                            break;
                        case ShipType.x4:
                            PlayerShips[x + 1, y] = status;
                            PlayerShips[x + 2, y] = status;
                            PlayerShips[x + 3, y] = status;
                            break;
                    }
                }
            }
            else result = false;
            return result;
        }
        public void DelShips()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    PlayerShips[i, j] = CoordStatus.None;
        }
    }
}




