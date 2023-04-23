using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public struct giantPos
{
    public int x;
    public int y;
}

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

class Player
{


    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int TX = int.Parse(inputs[0]);
        int TY = int.Parse(inputs[1]);

        // game loop
        while (true)
        {
            string command = "";
            List<giantPos> gPos = new List<giantPos>();
            inputs = Console.ReadLine().Split(' ');
            int H = int.Parse(inputs[0]); // the remaining number of hammer strikes.
            int N = int.Parse(inputs[1]); // the number of giants which are still present on the map.
            for (int i = 0; i < N; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int X = int.Parse(inputs[0]);
                int Y = int.Parse(inputs[1]);
                giantPos g;
                g.x = X;
                g.y = Y;
                gPos.Add(g);
            }

            List<bool> runAwayMoves = new List<bool>();
            runAwayMoves = RunAway(TX, TY, gPos);
            List<int> bestMove = new List<int>();
            bestMove = Scan(TX, TY, gPos);
            int power = int.MinValue;
            int index = -1;
            int moveIndex = -1;
            List<int> goodMoves = new List<int>();
            // Finding the single best move or several if multiple moves are equally good.
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    index++;
                    if (runAwayMoves[index] == true && bestMove[index] > power)
                    {
                        moveIndex = index;
                        power = bestMove[index];
                        goodMoves.Clear();
                        goodMoves.Add(index);
                    }
                    else if (runAwayMoves[index] == true && bestMove[index] == power)
                    {
                        goodMoves.Add(index);
                    }
                }
            }

            // If we end up with several equally good moves, then take the one that puts Thor
            // closest to the center.
            if (TY > 9 && TX > 20 && goodMoves.Contains(0))
            {
                command = "NW";
                TY--;
                TX--;
            }
            else if (TY > 9 && goodMoves.Contains(1)) 
            {
                command = "N";
                TY--;
            }
            else if (TY > 9 && TX < 20 && goodMoves.Contains(2))
            {
                command = "NE";
                TY--;
                TX++;
            }
            else if (TX > 20 && goodMoves.Contains(3)) 
            {
                command = "W";
                TX--;
            }
            else if (TX < 20 && goodMoves.Contains(5))
            {
                command = "E";
                TX++;
            }
            else if (TX > 20 && TY < 9 && goodMoves.Contains(6))
            {
                command = "SW";
                TY++;
                TX--;
            }
            else if (TY < 9 && goodMoves.Contains(7))
            {
                command = "S";
                TY++;
            }
            else if (TY < 9 && TX < 20 && goodMoves.Contains(8))
            {
                command = "SE";
                TY++;
                TX++;
            }
            // If we haven't found a move already then just pick the first valid move that had the highest power.
            // For instance if Thor was already at center.
            else
            {
                switch (moveIndex)
                {
                    case 0:
                        command = "NW";
                        TX--;
                        TY--;
                        break;
                    case 1:
                        command = "N";
                        TY--;
                        break;
                    case 2:
                        command = "NE";
                        TX++;
                        TY--;
                        break;
                    case 3:
                        command = "W";
                        TX--;
                        break;
                    case 5:
                        command = "E";
                        TX++;
                        break;
                    case 6:
                        command = "SW";
                        TX--;
                        TY++;
                        break;
                    case 7:
                        command = "S";
                        TY++;
                        break;
                    case 8:
                        command = "SE";
                        TX++;
                        TY++;
                        break;
                    default: // No good runawaymoves so strike.
                        command = "STRIKE";
                        break;
                }
            }
               
            // The movement or action to be carried out: WAIT STRIKE N NE E SE S SW W or N
            Console.WriteLine(command);
        }
    }

    // Check "power" of moves ie how many giants get closer or further away depending on move direction.
    // 0 = NW, 1 = N, 2 = NE, 3 = W, 4 = STAY, 5 = E, 6 = SW, 7 = S, 8 = SE
    public static List<int> Scan(int tX, int tY, List<giantPos> gPos)
    {
        List<int> power = new List<int>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int closer = 0;
                foreach (giantPos giantPosition in gPos)
                {
                    if (Distance(tX, tY, giantPosition) < Distance(tX-j, tY-i, giantPosition))
                    {
                        closer++;
                    }
                    else if (Distance(tX, tY, giantPosition) > Distance(tX - j, tY - i, giantPosition))
                    {
                        closer--;
                    }
                }
                power.Add(closer); 
            }
        }
        return power;
    }

    // Return Bool List of moves. True if valid move for running away, else false.
    // 0 = NW, 1 = N, 2 = NE, 3 = W, 4 = STAY, 5 = E, 6 = SW, 7 = S, 8 = SE
    public static List<bool> RunAway(int tX, int tY, List<giantPos> gPos)
    {
        List<bool> moves = new List<bool>();
        bool validMove = true;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                validMove = true;
                foreach (giantPos gpos in gPos)
                {
                    if (Distance(tX + j, tY + i, gpos) <= 1 || tX + j > 39 || tX + j < 0 || tY + i > 17 || tY + i < 0)
                    {
                        validMove = false;
                        break;
                    }

                }
                moves.Add(validMove); 
            }
        }
        return moves;
    }

    // Determine distance between Thor and giant
    public static int Distance(int tx, int ty, giantPos gPos)
    {
        // They move diagonally, so only the biggest of the two x or y distances is the actual distance.
        int distance;
        distance = Math.Abs(tx - gPos.x);
        if (distance < Math.Abs(ty - gPos.y)) return Math.Abs(ty - gPos.y);
        return distance;
    }
}

