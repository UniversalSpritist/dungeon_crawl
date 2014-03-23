﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeon_Crawl
{
    class Program
    {
        public static int selectedSpecies = 0;

        public static Species currSpecies;
        public static Class currClass;
        public static Player player;

        public static int renderX = 999;
        public static int renderY = 0;

        public static int currTurn = 0;
        public static int selectedSlot = 0;

        public static List<string> msgLog = new List<string>();

        static void Main(string[] args)
        {
            Console.WindowWidth = Console.LargestWindowWidth;
            Console.WindowHeight = Console.LargestWindowHeight;
            Species.init();
            Class.init();
            Tile.init();
            Item.init();
            showMainMenu();
        }
        public static void showMainMenu()
        {
            Boolean hasSelectedSpecies = false;
            Boolean hasSelectedClass = false;

            //Draw the species list
            while (!hasSelectedSpecies)
            {
                ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 0, 0, 30, Species.speciesList.Length + 1, false);
                Util.writeLn("Select a species", 1, 0);
                Species.drawAllSpecies();
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedSpecies++;
                    if (selectedSpecies > Species.speciesList.Length - 1)
                    {
                        selectedSpecies = 0;
                    }
                }
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedSpecies--;
                    if (selectedSpecies < 0)
                    {
                        selectedSpecies = Species.speciesList.Length - 1;
                    }
                }
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    currSpecies = Species.speciesList[selectedSpecies];
                    hasSelectedSpecies = true;
                }
            }
            Console.Clear();
            selectedSpecies = 0;
            while (!hasSelectedClass)
            {
                ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 0, 0, 30, Class.classList.Length + 1, false);
                Util.writeLn("Select a class--Species: " + currSpecies.abbrv, 1, 0);
                Class.drawAllClasses();
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedSpecies++;
                    if (selectedSpecies > Species.speciesList.Length - 1)
                    {
                        selectedSpecies = 0;
                    }
                }
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedSpecies--;
                    if (selectedSpecies < 0)
                    {
                        selectedSpecies = Species.speciesList.Length - 1;
                    }
                }
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    currClass = Class.classList[selectedSpecies];
                    hasSelectedClass = true;
                }
            }
            Console.Clear();
            Console.WriteLine("What is your name?");
            player = new Player(Console.ReadLine(), currSpecies, currClass);
            //Console.WriteLine("You are a " + player.identifier);
            Console.WriteLine();
            player.WriteStats();
            Console.ReadLine();
            World.genMap();
            startGame();
        }
        public static void renderGame()
        {
            Console.Clear();
            ConsoleEx.DrawRectangle(BorderStyle.Text, 0, 0, 26, 26, false);
            for (int y = -14; y < 13; y++)
            {
                for (int x = -14; x < 13; x++)
                {
                    while (renderX + x < 0)
                    {
                        x++;
                    }
                    while (renderY + y < 0)
                    {
                        y++;
                    }
                    if (Math.Sqrt(Math.Pow((renderX + x) - renderX, 2) + Math.Pow((renderY + y) - renderY, 2)) < 12.6)
                    {
                        Console.SetCursorPosition(Math.Min(26, Math.Max(1, x + 13)), Math.Min(26, Math.Max(1, y + 13)));
                        try
                        {
                            if (renderX + x == renderX && renderY + y == renderY)
                            {
                                ConsoleEx.TextColor(ConsoleForeground.LightGray, ConsoleBackground.Black);
                                Console.Write("P");
                            }
                            else
                            {
                                World.draw(renderX + x, renderY + y);
                            }

                        }
                        catch
                        {
                            //ConsoleEx.TextColor(ConsoleForeground.Red, ConsoleBackground.Black);
                            //Console.Write('X');
                        }
                        ConsoleEx.TextColor(ConsoleForeground.LightGray, ConsoleBackground.Black);
                    }
                    else
                    {
                        Console.SetCursorPosition(Math.Min(26, Math.Max(1, x + 13)), Math.Min(26, Math.Max(1, y + 13)));
                        ConsoleEx.TextColor(ConsoleForeground.DarkGray, ConsoleBackground.Black);
                        Console.Write(':');
                    }
                }
            }
            ConsoleEx.TextColor(ConsoleForeground.LightGray, ConsoleBackground.Black);
            ConsoleEx.DrawRectangle(BorderStyle.Text, 27, 0, 26, 40, false);
            ConsoleEx.DrawRectangle(BorderStyle.Text, 0, 27, 26, 13, false);
            Console.SetCursorPosition(2, 27);
            Console.WriteLine("Turn " + currTurn);
            Console.SetCursorPosition(2, 0);
            Console.Write("Pos: " + renderX + "/" + renderY);
            player.WriteRPGStats(28, 1);
            if (player.status.statusEffects.Count > 0)
            {
                ConsoleEx.DrawRectangle(BorderStyle.Text, 54, 0, 35, player.status.statusEffects.Count + 1, false);
                player.status.drawStatus(55, 1);
            }
            for (int x = 0; x < player.inventory.Length; x++)
            {
                if (player.inventoryStacks[x] > 0)
                {
                    string s = player.inventoryStacks[x] + " " + player.inventory[x].name;
                    if (Program.selectedSlot == x)
                    {
                        s = "> " + s;
                    }
                    if (player.inventoryEquip[x])
                    {
                        s = s + " (Equipped)";
                    }
                    if (player.inventory[x].bound && player.inventory[x].discoveredBound)
                    {
                        s = s + " (Bound)";
                    }
                    if (player.status.statusEffects.Count > 0)
                    {
                        Util.writeLn(s, 90, 1 + x);
                    }
                    else
                    {
                        Util.writeLn(s, 54, 1 + x);
                    }
                }
                else
                {
                    string s = "Empty Slot";
                    if (Program.selectedSlot == x)
                    {
                        s = "> " + s;
                    }
                    if (player.status.statusEffects.Count > 0)
                    {
                        Util.writeLn(s, 90, 1 + x);
                    }
                    else
                    {
                        Util.writeLn(s, 54, 1 + x);
                    }
                    player.inventory[x] = null;
                    player.inventoryStacks[x] = 0;
                    player.inventoryEquip[x] = false;
                }
            }
            ConsoleEx.DrawRectangle(BorderStyle.Text, 0, 41, 88, 6, false);
            for (int x = 0; x < 5; x++)
            {
                try
                {
                    Util.writeLn(msgLog.ToArray()[msgLog.ToArray().Length - (1 + x)], 1, 42 + x);
                }
                catch
                {
                    break;
                }
            }
            player.status.update();
        }
        public static void startGame()
        {
            Boolean turn = false;
            while (true)
            {
                if (!turn)
                {
                    renderGame();
                    turn = true;
                }
                int iteration = 0;
                while (turn)
                {
                    if (iteration > 0)
                    {
                        renderGame();
                    }
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.RightArrow && canMoveRight())
                    {
                        renderX = Math.Min(renderX + 1, 999);
                    }
                    if (keyInfo.Key == ConsoleKey.LeftArrow && canMoveLeft())
                    {
                        renderX = Math.Max(renderX - 1, 0);
                    }
                    if (keyInfo.Key == ConsoleKey.DownArrow && canMoveDown())
                    {
                        renderY = Math.Min(renderY + 1, 999);
                    }
                    if (keyInfo.Key == ConsoleKey.UpArrow && canMoveUp())
                    {
                        renderY = Math.Max(renderY - 1, 0);
                    }
                    if (World.gold[renderX, renderY] > 0)
                    {
                        player.addGold(World.gold[renderX, renderY]);
                        World.gold[renderX, renderY] = 0;
                    }
                    turn = false;
                    if (keyInfo.Key == ConsoleKey.W && !Console.CapsLock)
                    {
                        turn = true;
                        Program.selectedSlot--;
                        if (Program.selectedSlot < 0)
                        {
                            Program.selectedSlot = player.inventory.Length - 1;
                        }
                    }
                    if (keyInfo.Key == ConsoleKey.S && !Console.CapsLock)
                    {
                        turn = true;
                        Program.selectedSlot++;
                        if (Program.selectedSlot > player.inventory.Length - 1)
                        {
                            Program.selectedSlot = 0;
                        }
                    }
                    if (keyInfo.Key == ConsoleKey.E && !Console.CapsLock)
                    {
                        if (player.species != Species._faerie)
                        {
                            if (player.inventoryStacks[Program.selectedSlot] > 0 && player.inventory[Program.selectedSlot].equippable)
                            {
                                try
                                {
                                    if (player.inventoryStacks[Program.selectedSlot] > 0 && !(player.inventory[Program.selectedSlot].bound && player.inventory[Program.selectedSlot].discoveredBound))
                                    {
                                        player.inventoryEquip[Program.selectedSlot] = !player.inventoryEquip[Program.selectedSlot];
                                        turn = false;
                                    }
                                    else if (player.inventory[Program.selectedSlot].bound && player.inventory[Program.selectedSlot].discoveredBound && player.inventoryEquip[Program.selectedSlot])
                                    {
                                        msgLog.Add("You can't unequip a bound item!");
                                    }
                                }
                                catch
                                {
                                    player.inventoryEquip[Program.selectedSlot] = true;
                                    turn = false;
                                }
                                try
                                {
                                    if (player.inventoryEquip[Program.selectedSlot] && player.inventory[Program.selectedSlot].bound && !player.inventory[Program.selectedSlot].discoveredBound)
                                    {
                                        player.inventory[Program.selectedSlot].discoveredBound = true;
                                    }
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                turn = true;
                            }
                        }
                        else
                        {
                            turn = true;
                            msgLog.Add("You are intangible and cannot wield any items!");
                        }
                    }
                    iteration++;
                }
                currTurn++;
            }
        }
        public static Boolean canMoveRight()
        {
            return !World.map[renderX + 1, renderY].solid;
        }
        public static Boolean canMoveLeft()
        {
            return !World.map[renderX - 1, renderY].solid;
        }
        public static Boolean canMoveUp()
        {
            return !World.map[renderX, renderY - 1].solid;
        }
        public static Boolean canMoveDown()
        {
            return !World.map[renderX, renderY + 1].solid;
        }
    }
}
