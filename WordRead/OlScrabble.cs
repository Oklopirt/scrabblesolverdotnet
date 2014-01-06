using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Text;

namespace WordRead
{
    public partial class Scrabble
    {

        const bool Down = true;
        const bool Right = false;
        const int ScrabHw = 15;
        //definition of the thins on the words
        enum Descriptor { Empty, DoubleL, TripleL, DoubleW, TripleW };
        readonly static Descriptor[,] Desc;
        readonly static byte[] Lettervalues = { 1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3, 10, 1, 1, 1, 1, 4, 4, 8, 4, 10 };
        const int LowercaseOffset = 97;
        const string dictionary = "EnglishWords.txt";
        const char Empty = (char)0;
        readonly static char[] UppercaseAlph = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        //the current board. Holds 0 if empty, the letter otherwise
        char[,] board;

        //the current set of letters
        List<char> curletz;

        //has the first word been placed? false is no
        bool firstword = false;

        //the word database
        Trie W;

        List<wdescrip> curPossWords = null;

        public Scrabble()
        {

            board = new char[ScrabHw, ScrabHw];
            for (int i = 0; i < ScrabHw; i++)
                for (int j = 0; j < ScrabHw; j++)
                    board[i, j] = Empty;

            curletz = new List<char>();
            W = Trie.FromFile(dictionary);

        }

        public void placeLetters(int row, int col, string word, bool dir)
        {
            if (word.Length != 0)
                firstword = true;

            if (row >= board.GetLength(0) || col >= board.GetLength(1))
            {
                error("row or column beyond scope of board");
                return;
            }


            int strlen = word.Length;
            char[] wordletz = word.ToCharArray();


            if (dir == Scrabble.Down)
            {
                if (row + strlen > board.GetLength(0))
                {
                    error("word goes lower than board");
                    return;
                }

                for (int i = row; i < row + strlen; i++)
                    board[i, col] = wordletz[i - row];
            }

            else if (dir == Scrabble.Right)
            {
                if (col + strlen > board.GetLength(1))
                {
                    error("word goes further right than board");
                    return;
                }
                for (int i = col; i < col + strlen; i++)
                    board[row, i] = wordletz[i - col];
            }
        }

        public void reset()
        {
            firstword = false;
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(0); j++)
                    board[i, j] = Empty;
        }

        public void refrLetters(string letz)
        {
            curletz = new List<char>(letz.ToCharArray());


        }

        public void listLetters()
        {
            foreach (char c in curletz)
                Console.WriteLine(c + " ");
        }

        public bool DorR(string inp)
        {
            if (inp.Equals("D") || inp.Equals("d"))
                return Scrabble.Down;
            else if (inp.Equals("R") || inp.Equals("r"))
            {

                return Scrabble.Right;
            }
            else { error("bad input for direction"); return false; }
        }

        public void gameLoop()
        {
            String s;
            Console.WriteLine("Welcome to the jungle. Start doing stuff");
            //IO loop
            while (!(s = Console.ReadLine()).Equals("exit"))
            {
                String[] cmds = s.Split(new Char[] { ' ' });
                String first = cmds[0];
                if (first.Equals("letters"))
                    refrLetters(cmds[1]);
                else if (first.Equals("showletters"))
                    listLetters();
                // place row col D|R word
                else if (first.Equals("place"))
                {
                    int row = int.Parse(cmds[1]);
                    int col = int.Parse(cmds[2]);
                    bool dir = DorR(cmds[3]);
                    placeLetters(row, col, cmds[4], dir);
                }

                //verify row col D|R letter
                else if (first.Equals("verify"))
                {
                    int row = int.Parse(cmds[1]);
                    int col = int.Parse(cmds[2]);
                    bool dir = DorR(cmds[3]);
                    char let = char.Parse(cmds[4]);
                    bool ans = verify_around(let, row, col, dir);

                    if (ans)
                        Console.WriteLine("Yeah, it works");
                    else
                        Console.WriteLine("Nah, it doesn't work");
                }
                //value row col D|R letter
                else if (first.Equals("value"))
                {
                    int row = int.Parse(cmds[1]);
                    int col = int.Parse(cmds[2]);
                    bool dir = DorR(cmds[3]);
                    char let = char.Parse(cmds[4]);
                    Console.WriteLine("The value around " + let + " is " + value_around(let, row, col, dir));
                }
                //rowcol row col D|R
                else if (first.Equals("rowcol"))
                {
                    int row = int.Parse(cmds[1]);
                    int col = int.Parse(cmds[2]);
                    bool dir = DorR(cmds[3]);
                    listAtRowCol(row, col, dir);
                }

                //around row col D|R letter
                else if (first.Equals("around"))
                {
                    int row = int.Parse(cmds[1]);
                    int col = int.Parse(cmds[2]);
                    bool dir = DorR(cmds[3]);
                    char let = char.Parse(cmds[4]);
                    Console.WriteLine("Around " + let + " is " + aroundletters(let, row, col, dir));

                }

                else if (first.Equals("win"))
                {
                    listAll();
                }
                else if (first.Equals("show"))
                    displayBoard();

                else if (first.Equals("reset"))
                    reset();

                else if (first.Equals("startwith"))
                {
                    string response = (W.IsPrefix(cmds[1])) ? " is " : " is not ";
                    Console.WriteLine("There " + response + "a word that starts with " + cmds[1]);

                }

                else if (first.Equals("help"))
                    Console.WriteLine("exit\nletters string\nplace row col D|R word\nshow\nshowletters\nverify row col D|R letter\nvalue row col D|R letter\nrowcol row col D|R\naround row col D|R letter\nwin\nreset\nstartwith prefix");




                else { error("WTF"); }

            }
        }



        private static void error(string msg)
        {
            Console.WriteLine("ERR: " + msg);
        }

        public void displayBoard()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    char inplay = board[i, j];
                    if (inplay == (char)0)
                        Console.Write(descrip_rep(Desc[i, j]) + " ");
                    else { Console.Write(board[i, j] + " "); }
                }
                Console.WriteLine("\n");
            }
        }

        private static char descrip_rep(Descriptor d)
        {
            switch (d)
            {
                case Descriptor.DoubleL:
                    return '2';

                case Descriptor.TripleL:
                    return '3';

                case Descriptor.DoubleW:
                    return 'D';

                case Descriptor.TripleW:
                    return 'T';

                case Descriptor.Empty:
                    return '0';

                default:
                    error(" descrip WTF");
                    return '!';

            }
        }

        //initialize the descriptors
        static Scrabble()
        {
            Desc = new Descriptor[ScrabHw, ScrabHw];
            //most things are empty
            for (int i = 0; i < ScrabHw; i++)
                for (int j = 0; j < ScrabHw; j++)
                    Desc[i, j] = Descriptor.Empty;

            int[,] tripdubz = { { 0, 0 }, { 0, 7 }, { 0, 14 }, { 7, 0 }, { 7, 14 }, { 14, 0 }, { 14, 7 }, { 14, 14 } };
            fillArray(tripdubz, Desc, Descriptor.TripleW);

            int[,] doubleletz = {{0,3},{0,11},{2,6},{2,8},{3,0},{3,7},{3,14},
            {6,2},{6,6},{6,8},{6,12},{7,3},{7,11},{8,2},{8,6},{8,8},{8,12},
            {11,0},{11,7},{11,14},{12,6},{12,8},{14,3},{14,11}};
            fillArray(doubleletz, Desc, Descriptor.DoubleL);

            int[,] tripletz = {{1,5},{1,9},{5,1},{5,5},{5,9},{5,13},
            {9,1},{9,9},{9,5},{9,13},{13,5},{13,9}};
            fillArray(tripletz, Desc, Descriptor.TripleL);

            //now get the double words
            Desc[7, 7] = Descriptor.DoubleW;
            Descriptor dW = Descriptor.DoubleW;
            for (int i = 1; i <= 4; i++)
            {
                Desc[i, i] = dW;
                Desc[i, 14 - i] = dW;
                Desc[14 - i, i] = dW;
                Desc[14 - i, 14 - i] = dW;
            }

        }


        private static void fillArray(int[,] coords, Descriptor[,] fillin, Descriptor D)
        {
            for (int i = 0; i < coords.GetLength(0); i++)
                fillin[coords[i, 0], coords[i, 1]] = D;
        }
        private int letter_value(char c)
        {
            //if c is in the blank alphabet
            if (c < LowercaseOffset || c == '*')
                return 0;

            return Lettervalues[(int)c - LowercaseOffset];
        }



        public void listAtRowCol(int row, int col, bool dir)
        {
            List<wdescrip> possibles = new List<wdescrip>();
            RowCol(possibles, row, col, dir);
            var possible = possibles.ToList();
            possible.Sort();
            foreach (wdescrip w in possibles)
                w.printOut();
        }

        public void listAll()
        {
            List<wdescrip> possibles = new List<wdescrip>();
            getAll(ref possibles);
            possibles.Sort();

            foreach (wdescrip w in possibles)
                w.printOut();
        }


        public void RowCol(List<wdescrip> possibles, int row, int col, bool dir)
        {

            getRowCol(possibles, curletz, "", row, col, row, col, dir, false, !firstword, false, 0, 0, 1);
        }



        public double getAll(ref List<wdescrip> wordListGet)
        {
            Stopwatch s = new Stopwatch();

            s.Start();

            int threadCount = 0;
            ManualResetEvent doneEvent = new ManualResetEvent(false);

            WaitCallback workerAction = obj =>
            {
                object[] args = (object[])obj;
                List<wdescrip> wdList = (List<wdescrip>)args[0];
                int i = (int)args[1];
                int j = (int)args[2];
                bool dir = (bool)args[3];
                int index = (int)args[4];
                RowCol(wdList, i, j, dir);
                if (!firstword)
                {
                    if (dir == Right)
                        wdList.RemoveAll(x => x.Dir == Scrabble.Right && !(x.Col <= 7 && (x.Col + x.Length - 1) >= 7));
                    else if (dir == Down)
                        wdList.RemoveAll(x => x.Dir == Scrabble.Down && !(x.Row <= 7 && (x.Row + x.Length - 1) >= 7));
                }
                if (Interlocked.Decrement(ref threadCount) == 0)
                {
                    doneEvent.Set();
                }
            };

            List<List<wdescrip>> allWordLists = new List<List<wdescrip>>();

            List<object> argsList = new List<object>();

            int ind = 0;

            //get all horizontal words
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1) - 1; j++)
                {
                    //if not within, continue

                    if (first_letter(i, j, Right) || (board[i, j] == Empty && (j == 0 || board[i, j - 1] == Empty)))
                        if (firstword || (!firstword && i == 7))
                        {
                            List<wdescrip> horizontals = new List<wdescrip>();
                            allWordLists.Add(horizontals);
                            argsList.Add(new object[] { horizontals, i, j, Right, ind });
                            ind++;
                        }

                }
            }

            //all vertical words
            for (int i = 1; i < board.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (first_letter(i, j, Down) || (board[i, j] == Empty && (i == 0 || board[i - 1, j] == Empty)))
                        if (firstword || (!firstword && j == 7))
                        {
                            List<wdescrip> verticals = new List<wdescrip>();
                            allWordLists.Add(verticals);
                            argsList.Add(new object[] { verticals, i, j, Down, ind });
                            ind++;
                        }
                }
            }



            threadCount = ind;
            foreach (object arg in argsList)
            {
                ThreadPool.QueueUserWorkItem(workerAction, arg);
            }

            doneEvent.WaitOne();
            doneEvent.Close();

            wordListGet = allWordLists.SelectMany(x => x).ToList();
            wordListGet.Sort();

            curPossWords = wordListGet;
            return s.Elapsed.TotalSeconds;
        }

        //bridge describes the proptery of having reached the final block of already placed letters
        public void getRowCol(List<wdescrip> wordList, List<char> availLetters, string wordSoFar,
                              int startRow, int startCol, int curRow, int curCol, bool dir, bool bridge, bool linkedin, bool sevenLet, int baseValue, int extraVal, int multiplier)
        {
            bool brid = bridge;
            bool link = linkedin;
            int addedVal = extraVal;
            bool seven = sevenLet;


            //we're done if beyond the scope of the board
            if (curRow >= board.GetLength(0) || curCol >= board.GetLength(1))
                return;

            //future -- instead of recursing on getrowcol in this case, go through all the letters until the end of the placed letters and THEN
            //recurse.
            //if we're on a placed letter, add word if it is final letter and addition of letter is a word
            //this happens when we've added letters that bridge already placed letters
            if (board[curRow, curCol] != Scrabble.Empty)
            {
                if (!seven && (curletz.Count - availLetters.Count) == 7)
                {
                    addedVal += 50;
                    seven = true;
                }

                link = true;
                char onBoard = board[curRow, curCol];
                //return if it is futile to go on
                if (!(W.IsPrefix(wordSoFar + onBoard)))
                    return;

                if (dir == Scrabble.Right)
                {

                    //if we reached the end or the next letter is empty
                    if (bridge && ((curCol == board.GetLength(1) - 1) || (board[curRow, curCol + 1] == Scrabble.Empty)))
                    { //|| ((board[curRow,curCol+1] == OldScrabble.EMPTY) && bridge)){

                        //if we're legit up and down and the word is a word, add it. Don't need legit up and down since placement implies this
                        if (W.IsWord(wordSoFar + onBoard))
                            wordList.Add(new wdescrip(wordSoFar + onBoard, startCol, startRow, Scrabble.Right, addedVal + (baseValue + letter_value(onBoard)) * multiplier));
                    }
                    //else{

                    getRowCol(wordList, availLetters, wordSoFar + onBoard, startRow, startCol, curRow, curCol + 1, Scrabble.Right, brid, link, seven, baseValue +
                              letter_value(onBoard), addedVal, multiplier);
                    //}
                }

                else if (dir == Scrabble.Down)
                {
                    if (bridge && ((curRow == board.GetLength(0) - 1) || (board[curRow + 1, curCol] == Scrabble.Empty)))
                    { // || ((board[curRow+1,curCol] == OldScrabble.EMPTY) && bridge) ){
                        //if we're legit up and down and the word is a word, add it
                        if (W.IsWord(wordSoFar + onBoard))
                            wordList.Add(new wdescrip(wordSoFar + onBoard, startCol, startRow, Scrabble.Down, addedVal + (baseValue + letter_value(onBoard)) * multiplier));
                    }
                    //else{
                    getRowCol(wordList, availLetters, wordSoFar + onBoard, startRow, startCol, curRow + 1, curCol, Scrabble.Down, brid, link, seven, baseValue +
                              letter_value(onBoard), addedVal, multiplier);
                    //}
                }

            }//end of if case of letter space being occupied

            //the letter space is not occupied
            else
            {
                brid = true;
                int lettermult = 1;
                int wordmult = 1;

                switch (Desc[curRow, curCol])
                {
                    case Descriptor.DoubleL:
                        lettermult = 2;
                        break;
                    case Descriptor.TripleL:
                        lettermult = 3;
                        break;
                    case Descriptor.DoubleW:
                        wordmult = 2;
                        break;
                    case Descriptor.TripleW:
                        wordmult = 3;
                        break;
                }




                //iterate each of the letters at our disposal
                foreach (char useLetter in availLetters)
                {


                    bool isBlank = (useLetter == '*');
                    char[] iterLetters;

                    if (useLetter == '*')
                        iterLetters = UppercaseAlph;
                    else
                        iterLetters = new char[] { useLetter };

                    for (int i = 0; i < iterLetters.Length; i++)
                    {
                        addedVal = extraVal;
                        char useLet = iterLetters[i];

                        //if introducing the letter creates a problem, dismiss it
                        if (!verify_around(useLet, curRow, curCol, dir))
                        {
                            //Console.WriteLine(useLet + " at " + curRow + " " + curCol + " no good");
                            continue;
                        }

                        //if there is no word starting with this, continue
                        if (!W.IsPrefix(wordSoFar + useLet))
                            continue;

                        int letter_worth = letter_value(useLet);
                        int baseAdd = letter_worth * lettermult;


                        if (has_neighbors(curRow, curCol, dir))
                        {
                            link = true;
                            int aroundVal = value_around(useLet, curRow, curCol, dir) * wordmult;
                            addedVal += aroundVal;
                            addedVal += letter_worth * lettermult * wordmult;


                        }

                        if (!sevenLet && (curletz.Count - availLetters.Count) == 6)
                        {
                            addedVal += 50;
                            seven = true;
                        }

                        //if we got a word, add to the list and recurse. Also add if
                        //its a single letter but the environment is good

                        //true if thre aren't letters to the right or below
                        bool surround = (dir == Right && curCol == 14) || (dir == Down && curRow == 14) || (dir == Right && board[curRow, curCol + 1] == Empty) || (dir == Down && board[curRow + 1, curCol] == Empty);

                        if (surround && (W.IsWord(wordSoFar + useLet) || (wordSoFar.Equals("") && firstword)) && link)
                        {
                            int value;
                            if (wordSoFar.Length == 0)
                                value = addedVal;
                            else
                                value = addedVal + (baseValue + baseAdd) * (multiplier * wordmult);


                            wordList.Add(new wdescrip(wordSoFar + useLet, startCol, startRow, dir, value));
                        }
                        //recurse on the problem
                        List<char> nextset = new List<char>(availLetters.ToArray());


                        nextset.Remove(useLetter);


                        if (dir == Scrabble.Down)
                            getRowCol(wordList, nextset, wordSoFar + useLet, startRow,
                                      startCol, curRow + 1, curCol, dir, brid, link, seven, baseValue + baseAdd, addedVal, multiplier * wordmult);
                        else if (dir == Scrabble.Right)
                            getRowCol(wordList, nextset, wordSoFar + useLet, startRow,
                                      startCol, curRow, curCol + 1, dir, brid, link, seven, baseValue + baseAdd, addedVal, multiplier * wordmult);
                        //end of iterLetters block
                    }
                    //end of foreach block
                }

            }


        }

        //tells whether a letter in a given direction is the first
        private bool first_letter(int row, int col, bool dir)
        {
            if (board[row, col] == Empty)
                return false;
            if (dir == Right)
            {
                if (col == 0)
                    return true;
                if (board[row, col - 1] != Empty)
                    return false;
                else
                    return true;

            }

            if (dir == Down)
            {
                if (row == 0)
                    return true;
                if (board[row - 1, col] != Empty)
                    return false;
                else
                    return true;
            }

            error("NO DIRECTION");
            return false;
        }

        private bool verify_around(char letter, int row, int col, bool dir)
        {
            string aroundword = aroundletters(letter, row, col, dir);
            if ((aroundword.Length == 1) || W.IsWord(aroundword))
                return true;
            else
            {
                //Console.WriteLine(aroundword + " is aroundword");
                return false;
            }
        }

        private bool has_neighbors(int row, int col, bool dir)
        {
            //check left and right
            if (dir == Down)
            {
                if (col == 0)
                {
                    if (board_empty(row, col + 1))
                        return false;
                    else return true;
                }
                else if (col == (board.GetLength(1) - 1))
                {
                    if (board_empty(row, col - 1))
                        return false;
                    else
                        return true;
                }

                else
                    return ((!board_empty(row, col - 1)) || (!board_empty(row, col + 1)));

            }

            else if (dir == Right)
            {
                if (row == 0)
                {
                    if (board_empty(row + 1, col))
                        return false;
                    else return true;
                }
                else if (row == (board.GetLength(0) - 1))
                {
                    if (board_empty(row - 1, col))
                        return false;
                    else
                        return true;
                }

                else
                    return ((!board_empty(row - 1, col)) || (!board_empty(row + 1, col)));

            }

            else
            {
                error("has_neighbors has no direction");
                return false;
            }

        }

        private bool board_empty(int row, int col)
        {
            return board[row, col] == Empty;
        }

        private int value_around(char letter, int row, int col, bool dir)
        {
            string aroundword = aroundletters(letter, row, col, dir);
            char[] wordletz = aroundword.ToCharArray();
            int total = 0;
            foreach (char c in wordletz)
                total += letter_value(c);
            //Console.WriteLine("Value of " + aroundword + " is " + total)
            return total - letter_value(letter);
        }

        //find the word around a location a letter is placed
        private string aroundletters(char letter, int row, int col, bool dir)
        {


            //check that we only have one letter
            if (dir == Scrabble.Right)
            {

                if (row == (board.GetLength(0) - 1))
                    if (board[row - 1, col] == Empty)
                        return "" + letter;
                    else if (row == 0)
                        if (board[row + 1, col] == Empty)
                            return "" + letter;
            }

            else if (dir == Scrabble.Down)
            {
                if (col == (board.GetLength(1) - 1))
                    if (board[row, col - 1] == Empty)
                        return "" + letter;
                    else if (col == 0)
                        if (board[row, col + 1] == Empty)
                            return "" + letter;
            }

            //temporarily fill in the letter to ease calculation
            //NOTE: this could and probably does cause threading problems!!!

            //board[row,col] = letter;

            StringBuilder s = new StringBuilder(15);

            if (dir == Scrabble.Right)
            {
                int firstrow;
                for (firstrow = row; firstrow > 0 && board[firstrow - 1, col] != Empty; firstrow--) { }

                //string s = "";
                //board.GetLength(0)
                for (int i = firstrow; i < row && board[i, col] != Empty; i++)
                    s.Append(board[i, col]);

                s.Append(board[row, col]);

                for (int i = row + 1; i < board.GetLength(0) && board[i, col] != Empty; i++)
                    s.Append(board[i, col]);

                return s.ToString();
            }

                //do up to letter, then add letter, then do up to the end

            else if (dir == Scrabble.Down)
            {
                int firstcol;
                for (firstcol = col; firstcol > 0 && board[row, firstcol - 1] != Empty; firstcol--) { }

                for (int i = firstcol; i < col && board[row, i] != Empty; i++)
                    s.Append(board[row, i]);

                s.Append(board[row, col]);

                for (int i = col + 1; i < board.GetLength(1) && board[row, i] != Empty; i++)
                {
                    s.Append(board[row, i]);
                }

                return s.ToString();
            }

            error("Verify failed");
            return "failure";

        }




    }
}
