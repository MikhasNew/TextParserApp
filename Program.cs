﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace TextParserApp
{
    class Program
    {

        static Dictionary<string, int> WordsCountDictyonary = new Dictionary<string, int>();
        static Dictionary<char, int> LetterCountDyctionary = new Dictionary<char, int>();
        static StringBuilder PunctList = new StringBuilder();

        static int LongsteOffersOfSimbalsIndex;
        static int LongsteOffersOfSimbalLength;
        static int ShortestOffersOfWordsIndex;
        static int ShortestOffersOfWordsCount;

        static int OffersNumber = 0;

        static async Task Main( )
        {
            Console.WriteLine("Working...........");

            string assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string resPath = assemblyPath + "\\Resource\\Sample.txt";

            Stream streamOffers = new FileStream(assemblyPath + "\\Resource\\SampleOffers.txt", FileMode.Create);
            Stream streamWords = new FileStream(assemblyPath + "\\Resource\\SampleWords.txt", FileMode.Create);
            Stream streamWordsStatistic = new FileStream(assemblyPath + "\\Resource\\SampleWordsStatistic.txt", FileMode.Create);
            Stream streamPunctuationSymbols = new FileStream(assemblyPath + "\\Resource\\streamPunctuationSymbols.txt", FileMode.Create);
            Stream streamOtherInformation = new FileStream(assemblyPath + "\\Resource\\streamOtherInformation.txt", FileMode.Create);

            StreamWriter streamWriterOffers = new StreamWriter(streamOffers, System.Text.Encoding.Unicode);
            StreamWriter streamWriterWords = new StreamWriter(streamWords, System.Text.Encoding.Unicode);
            StreamWriter streamWriterWordsStatistic = new StreamWriter(streamWordsStatistic, System.Text.Encoding.Unicode);
            StreamWriter streamWriterPunctuationSymbols = new StreamWriter(streamPunctuationSymbols, System.Text.Encoding.Unicode);
            StreamWriter streamWriterOtherInformation = new StreamWriter(streamOtherInformation, System.Text.Encoding.Unicode);

            string offersSplitPattern = @"(?<=(?<!Dr|Mr|Ms|St|aet|Fig|pp|(?:[A-Za-z]\.[A-Za-z])| [A-Z]|[A-Z]{2,})(?:[\.?!]{1,}[""*)}\]]{0,})(?=(?:[\s]{1,}[""]{0,1}[A-Z0-9*#_({[=])|$))";
            Regex offersSplitRegex = new Regex(offersSplitPattern);

            string wordsPattern = @"[\w]{1,}";
            Regex wordsRegex = new Regex(wordsPattern);

            string punctPattern = @"(?:["",.?!:;']{1,1})";
            Regex punctRegex = new Regex(punctPattern);

            string tempString = null;

            try
            {
                using (var sr = new StreamReader(resPath))
                {
                    string stringLine;// = sr.ReadLine();
                    do
                    {
                        stringLine = sr.ReadLine();
                        var offersItems = offersSplitRegex.Split(string.Concat(tempString, stringLine + " "));
                        int offersCountInLine = offersItems.Length;
                        if (sr.EndOfStream) //in case last Liene add fake Offer
                        {
                            offersCountInLine++;
                        }
                        for (int i = 0; i < offersCountInLine - 1; i++)
                        {
                            CheckLongOffersOfSimbals(OffersNumber, offersItems[i].Length);

                            var words = wordsRegex.Matches(offersItems[i]);
                            CheckShortOffersOfWords(OffersNumber, words.Count);

                            foreach (Match word in words)
                            {
                                await streamWriterWords.WriteLineAsync(word.Value);

                                var charsWord = word.Value.ToLower();

                                AddToWordsDictionary(charsWord);
                                AddToletterCountDyctionary(charsWord.ToCharArray());
                            }

                            var punctSimbls = punctRegex.Matches(offersItems[i]);
                            foreach (Match simbl in punctSimbls)
                            {
                                PunctList.Append(simbl.Value);
                            }

                            await streamWriterOffers.WriteLineAsync(offersItems[i]);

                            OffersNumber++;
                        }

                        tempString = offersItems.Last();

                    }

                    while (!sr.EndOfStream); 

                }
                await streamWriterPunctuationSymbols.WriteLineAsync(PunctList);

                foreach (var word in WordsCountDictyonary.OrderBy(word => word.Key))
                {
                    await streamWriterWordsStatistic.WriteLineAsync($"{word.Key} - {word.Value}");
                }

                await streamWriterOffers.FlushAsync();
                await streamOffers.DisposeAsync();

                using (StreamReader sr = new StreamReader(assemblyPath + "\\Resource\\SampleOffers.txt"))
                {
                    bool writteLong = false;
                    bool writeCurz = false;

                    int lineNumber = 0;

                    var dict = LetterCountDyctionary.OrderBy(e => e.Value).Last();
                    await streamWriterOtherInformation.WriteLineAsync($"The most common letter:\n {dict.Key} - {dict.Value}");

                    while (sr != null)
                    {
                        var ci = await sr.ReadLineAsync();

                        if (lineNumber == LongsteOffersOfSimbalsIndex)
                        {
                            //var longStr = await sr.ReadLineAsync();
                            await streamWriterOtherInformation.WriteLineAsync($"This is a great offer for the number of characters:\n {ci}");
                            writteLong = true;
                        }
                        if (lineNumber == ShortestOffersOfWordsIndex)
                        {
                            //var Str = await sr.ReadLineAsync();
                            await streamWriterOtherInformation.WriteLineAsync($"This is the smallest sentence in terms of the number of words:\n {ci}");
                            writeCurz = true;
                        }
                        if (writteLong && writeCurz)
                            break;
                        lineNumber++;

                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("\n -!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!");
                Console.WriteLine($"\n {e}");
                Console.WriteLine("\n -!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!-!");

                await streamWriterOffers.FlushAsync();
            }
            finally
            {
                await streamWriterPunctuationSymbols.FlushAsync();
                await streamWriterWordsStatistic.FlushAsync();
                await streamWriterWords.FlushAsync();
                await streamWriterOtherInformation.FlushAsync();

                await streamOffers.DisposeAsync();
                await streamWords.DisposeAsync();
                await streamOtherInformation.DisposeAsync();
                await streamWordsStatistic.DisposeAsync();
                await streamPunctuationSymbols.DisposeAsync();
                Process.Start("explorer.exe", assemblyPath + "\\Resource\\");
            }


        }

        public static void CheckLongOffersOfSimbals(int index, int length)
        {
            if (OffersNumber == 0)
            {
                LongsteOffersOfSimbalsIndex = 0;
                LongsteOffersOfSimbalLength = length;
            }
            else if (LongsteOffersOfSimbalLength < length)
            {
                LongsteOffersOfSimbalLength = length;
                LongsteOffersOfSimbalsIndex = index;
            }

        }
        public static void CheckShortOffersOfWords(int index, int count)
        {
            if (OffersNumber == 0)
            {
                ShortestOffersOfWordsIndex = 0;
                ShortestOffersOfWordsCount = count;
            }
            else if (ShortestOffersOfWordsCount > count)
            {
                ShortestOffersOfWordsCount = count;
                ShortestOffersOfWordsIndex = index;
            }

        }

        public static void AddToWordsDictionary(string word)
        {
            if (!int.TryParse(word, out _))
            {
                if (WordsCountDictyonary.ContainsKey(word))
                {
                    WordsCountDictyonary[word]++;
                }
                else
                {
                    WordsCountDictyonary.Add(word, 1);
                }
            }
        }

        public static void AddToletterCountDyctionary(char[] leters)
        {
            foreach (var leter in leters)
            {
                if (LetterCountDyctionary.ContainsKey(leter))
                {
                    LetterCountDyctionary[leter]++;
                }
                else
                {
                    LetterCountDyctionary.Add(leter, 0);
                }
            }
        }
    }
}




