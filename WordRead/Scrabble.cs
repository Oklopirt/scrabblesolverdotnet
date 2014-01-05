/*
 * Current progress: prefix trie implemented. 
 * no  refactoring!!! f that noise
 * 
 *
 * Eventual ideas: preempt generation while the user types their letters in!
 * Obviously port to c++
 * OpenCL?
 * 
 * Do cool visualization?
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Text;

namespace WordRead
{
    

    public struct vec2
    {
        public int x, y;
        public vec2(int x, int y) { this.x = x; this.y = y; }

        public static vec2 operator +(vec2 a, vec2 b)
        {
            return new vec2(a.x + b.x, a.y + b.y);
        }

        public static vec2 operator +(vec2 v, bool dir)
        {
            if (dir == Scrabble.DOWN)
                return new vec2(v.x, v.y + 1);
            else
                return new vec2(v.x + 1, v.y);  byte[,] tripletz = {{1,5},{1,9},{5,1},{5,5},{5,9},{5,13},
            {9,1},{9,9},{9,5},{9,13},{13,5},{13,9}};
            fillArray(tripletz, desc, descriptor.tripleL);

            //now get the double words
            desc[7, 7] = descriptor.doubleW;
            descriptor dW = descriptor.doubleW;
            for (int i = 1; i <= 4; i++)
            {
                desc[i, i] = dW;
                desc[i, 14 - i] = dW;
                desc[14 - i, i] = dW;
                desc[14 - i, 14 - i] = dW;
            }

        }


        private static void fillArray(byte[,] coords, descriptor[,] fillin, descriptor D)
        {
            for (int i = 0; i < coords.GetLength(0); i++)
                fillin[coords[i, 0], coords[i, 1]] = D;
        }
        private int letter_value(char c)
        {
            //if c is in the blank alphabet
            if (c < LOWERCASE_OFFSET || c == '*')
                return 0;

            return lettervalues[(int)c - LOWERCASE_OFFSET];
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

            getRowCol(possibles, curletz, "", prefixTrie, row, col, row, col, dir, false, !firstword, false, 0, 0, 1);
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
                    if (dir == RIGHT)
                        wdList.RemoveAll(x => x.Dir == Scrabble.RIGHT && !(x.Col <= 7 && (x.Col + x.Length - 1) >= 7));
                    else if (dir == DOWN)
                        wdList.RemoveAll(x => x.Dir == Scrabble.DOWN && !(x.Row <= 7 && (x.Row + x.Length - 1) >= 7));
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

                    if (first_letter(i, j, RIGHT) || (board[i, j] == EMPTY && (j == 0 || board[i, j - 1] == EMPTY)))
                        if (firstword || (!firstword && i == 7))
                        {
                            List<wdescrip> horizontals = new List<wdescrip>();
                            allWordLists.Add(horizontals);
                            argsList.Add(new object[] { horizontals, i, j, RIGHT, ind });
                            ind++;
                        }

                }
            }

            //all vertical words
            for (int i = 1; i < board.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (first_letter(i, j, DOWN) || (board[i, j] == EMPTY && (i == 0 || board[i - 1, j] == EMPTY)))
                        if (firstword || (!firstword && j == 7))
                        {
                            List<wdescrip> verticals = new List<wdescrip>();
                            allWordLists.Add(verticals);
                            argsList.Add(new object[] { verticals, i, j, DOWN, ind });
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
        //start row, start col, direction, and availLetters all remain constant throughout the recursion, can be rapped in a struct
        
        //make the code for right and down the same (ignore other optimizations for now)
        //vec2 startPos
        //vec2 curPos
        public void getRowCol(List<wdescrip> wordList, List<char> availLetters, string wordSoFar, Trie prefixTrie,
                              int startRow, int startCol, int curRow, int curCol, bool dir, bool bridge, bool linkedin, bool sevenLet, int baseValue, int extraVal, int multiplier)
        {
            if (prefixTrie == null)
                return;

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
            if (board[curRow, curCol] != Scrabble.EMPTY)
            {
                if (!seven && (curletz.Count - availLetters.Count) == 7)
                {
                    addedVal += 50;
                    seven = true;
                }

                link = true;
                char onBoard = board[curRow, curCol];
                //return if it is futile to go on
                //if (!(W.anyStart(wordSoFar + onBoard)))
                if (prefixTrie[onBoard] == null)
                    return;

                var nextNode = prefixTrie[onBoard];

                if (dir == Scrabble.RIGHT)
                {

                    //if we reached the end or the next letter is empty
                    if (bridge && ((curCol == board.GetLength(1) - 1) || (board[curRow, curCol + 1] == Scrabble.EMPTY)))
                    { //|| ((board[curRow,curCol+1] == Scrabble.EMPTY) && bridge)){

                        //if we're legit up and down and the word is a word, add it. Don't need legit up and down since placement implies this
                        //if (W.isWord(wordSoFar + onBoard))

                        if (nextNode != null && nextNode.isWord)
                            wordList.Add(new wdescrip(wordSoFar + onBoard, startCol, startRow, Scrabble.RIGHT, addedVal + (baseValue + letter_value(onBoard)) * multiplier));
                    }
                    //else{

                    getRowCol(wordList, availLetters, wordSoFar + onBoard, prefixTrie[onBoard], startRow, startCol, curRow, curCol + 1, Scrabble.RIGHT, brid, link, seven, baseValue +
                              letter_value(onBoard), addedVal, multiplier);
                    //}
                }

                else if (dir == Scrabble.DOWN)
                {
                    if (bridge && ((curRow == board.GetLength(0) - 1) || (board[curRow + 1, curCol] == Scrabble.EMPTY)))
                    { // || ((board[curRow+1,curCol] == Scrabble.EMPTY) && bridge) ){
                        //if we're legit up and down and the word is a word, add it
                        //if (W.isWord(wordSoFar + onBoard))
                        if (nextNode != null && nextNode.isWord)
                            wordList.Add(new wdescrip(wordSoFar + onBoard, startCol, startRow, Scrabble.DOWN, addedVal + (baseValue + letter_value(onBoard)) * multiplier));
                    }
                    //else{
                    getRowCol(wordList, availLetters, wordSoFar + onBoard, prefixTrie[onBoard], startRow, startCol, curRow + 1, curCol, Scrabble.DOWN, brid, link, seven, baseValue +
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

                switch (desc[curRow, curCol])
                {
                    case descriptor.doubleL:
                        lettermult = 2;
                        break;
                    case descriptor.tripleL:
                        lettermult = 3;
                        break;
                    case descriptor.doubleW:
                        wordmult = 2;
                        break;
                    case descriptor.tripleW:
                        wordmult = 3;
                        break;
                }




                //iterate each of the letters at our disposal
                foreach (char useLetter in availLetters)
                {


                    bool isBlank = (useLetter == '*');
                    char[] iterLetters;

                    if (useLetter == '*')
                        iterLetters = UPPERCASE_ALPH;
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
                        //if (!W.anyStart(wordSoFar + useLet))
                        if (prefixTrie[useLet] == null)
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
                        bool surround = (dir == RIGHT && curCol == 14) || (dir == DOWN && curRow == 14) || (dir == RIGHT && board[curRow, curCol + 1] == EMPTY) || (dir == DOWN && board[curRow + 1, curCol] == EMPTY);
                        bool isWord = prefixTrie[useLet] != null && prefixTrie[useLet].isWord;
                        if (surround && (isWord || (wordSoFar.Equals("") && firstword)) && link)
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


                        if (dir == Scrabble.DOWN)
                            getRowCol(wordList, nextset, wordSoFar + useLet, prefixTrie[useLet], startRow,
                                      startCol, curRow + 1, curCol, dir, brid, link, seven, baseValue + baseAdd, addedVal, multiplier * wordmult);
                        else if (dir == Scrabble.RIGHT)
                            getRowCol(wordList, nextset, wordSoFar + useLet, prefixTrie[useLet], startRow,
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
            if (board[row, col] == EMPTY)
                return false;
            if (dir == RIGHT)
            {
                if (col == 0)
                    return true;
                if (board[row, col - 1] != EMPTY)
                    return false;
                else
                    return true;

            }

            if (dir == DOWN)
            {
                if (row == 0)
                    return true;
                if (board[row - 1, col] != EMPTY)
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
            if ((aroundword.Length == 1) || W.isWord(aroundword))
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
            if (dir == DOWN)
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

            else if (dir == RIGHT)
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
            return board[row, col] == EMPTY;
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
            if (dir == Scrabble.RIGHT)
            {

                if (row == (board.GetLength(0) - 1))
                    if (board[row - 1, col] == EMPTY)
                        return "" + letter;
                else if (row == 0)
                     if (board[row + 1, col] == EMPTY)
                         return "" + letter;
            }

            else if (dir == Scrabble.DOWN)
            {
                if (col == (board.GetLength(1) - 1))
                    if (board[row, col - 1] == EMPTY)
                        return "" + letter;
                    else if (col == 0)
                        if (board[row, col + 1] == EMPTY)
                            return "" + letter;
            }

            //temporarily fill in the letter to ease calculation
            //NOTE: this could and probably does cause threading problems!!!

            //board[row,col] = letter;

            StringBuilder s = new StringBuilder(15);

            if (dir == Scrabble.RIGHT)
            {
                int firstrow;
                for (firstrow = row; firstrow > 0 && board[firstrow - 1, col] != EMPTY; firstrow--) { }

                //string s = "";
                //board.GetLength(0)
                for (int i = firstrow; i < row && board[i, col] != EMPTY; i++)
                    s.Append(board[i, col]);

                s.Append(board[row, col]);

                for (int i = row + 1; i < board.GetLength(0) && board[i, col] != EMPTY; i++)
                    s.Append(board[i, col]);

                return s.ToString();
            }

                //do up to letter, then add letter, then do up to the end

            else if (dir == Scrabble.DOWN)
            {
                int firstcol;
                for (firstcol = col; firstcol > 0 && board[row, firstcol - 1] != EMPTY; firstcol--) { }

                for (int i = firstcol; i < col && board[row, i] != EMPTY; i++)
                    s.Append(board[row, i]);

                s.Append(board[row, col]);

                for (int i = col + 1; i < board.GetLength(1) && board[row, i] != EMPTY; i++)
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
