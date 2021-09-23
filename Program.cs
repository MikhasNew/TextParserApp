using System;
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

        static Dictionary<string, int> wordsCountDictyonary = new Dictionary<string, int>();
        static Dictionary<char, int> letterCountDyctionary = new Dictionary<char, int>();
        //static List<string> punctList = new List<string>();
        static StringBuilder punctList = new StringBuilder();

        static int longsteOffersOfSimbalsIndex;
        static int longsteOffersOfSimbalLength;
        static int shortestOffersOfWordsIndex;
        static int shortestOffersOfWordsCount;

        public static int offersNumber = 0;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Working...........");

            string assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string resPath = assemblyPath + "\\Resource\\Sample.txt";

            Stream streamOffers = new FileStream(assemblyPath + "\\Resource\\SampleOffers.txt", FileMode.Create);
            Stream streamWords = new FileStream(assemblyPath + "\\Resource\\SampleWords.txt", FileMode.Create);
            Stream streamWordsStatistic = new FileStream(assemblyPath + "\\Resource\\SampleWordsStatistic.txt", FileMode.Create);
            Stream streamPunctuationSymbols = new FileStream(assemblyPath + "\\Resource\\streamPunctuationSymbols.txt", FileMode.Create);
            Stream streamOtherInformation = new FileStream(assemblyPath + "\\Resource\\streamOtherInformation.txt", FileMode.Create);

            StreamWriter StreamWriterOffers = new StreamWriter(streamOffers, System.Text.Encoding.Unicode);
            StreamWriter StreamWriterWords = new StreamWriter(streamWords, System.Text.Encoding.Unicode);
            StreamWriter StreamWriterWordsStatistic = new StreamWriter(streamWordsStatistic, System.Text.Encoding.Unicode);
            StreamWriter StreamWriterPunctuationSymbols = new StreamWriter(streamPunctuationSymbols, System.Text.Encoding.Unicode);
            StreamWriter StreamWriterOtherInformation = new StreamWriter(streamOtherInformation, System.Text.Encoding.Unicode);

            string offersSplitPattern = @"(?<=(?<!Dr|Mr|Ms|St|aet|Fig|pp|(?:[A-Za-z]\.[A-Za-z])| [A-Z]|[A-Z]{2,})(?:[\.?!]{1,})(?=(?:[""*)}\]]{0,}[\s]{1,}[""]{0,1}[A-Z0-9*#_({[=])))";
            Regex offersSplitRegex = new Regex(offersSplitPattern);

            string wordsPattern = @"[\w]{1,}";
            //string wordsPattern = @"(?:[\w<>|+=&_@#$%*(){}[\]]{1,})";
            Regex wordsRegex = new Regex(wordsPattern);

            string punctPattern = @"(?:["",.?!:;']{1,1})";
            Regex punctRegex = new Regex(punctPattern);

            string tempString = null;

            try
            {
                using (var sr = new StreamReader(resPath))
                {
                    var stringLine = sr.ReadLine() + " ";

                    while (stringLine != null)
                    {
                        var offersItems = offersSplitRegex.Split(string.Concat(tempString, stringLine + " ")); // in dictionary test dell  + "\n"

                        for (int i = 0; i < offersItems.Length - 1; i++)
                        {
                            checkLongOffersOfSimbals(offersNumber, offersItems[i].Length);

                            var words = wordsRegex.Matches(offersItems[i]);
                            checkShortOffersOfWords(offersNumber, words.Count);

                            foreach (Match word in words)
                            {

                                await StreamWriterWords.WriteLineAsync(word.Value);

                                var charsWord = word.Value.ToLower();

                                addToWordsDictionary(charsWord);
                                addToletterCountDyctionary(charsWord.ToCharArray());
                            }

                            var punctSimbls = punctRegex.Matches(offersItems[i]);
                            foreach (Match simbl in punctSimbls)
                            {
                                punctList.Append(simbl.Value);
                            }

                            await StreamWriterOffers.WriteLineAsync(offersItems[i]);

                            offersNumber++;
                        }

                        tempString = offersItems.Last();
                        stringLine = sr.ReadLine();

                        //offersNumber++;
                    }

                }
                await StreamWriterPunctuationSymbols.WriteLineAsync(punctList);

                //StringBuilder buferString = new StringBuilder();
                foreach (var word in wordsCountDictyonary.OrderBy(word => word.Key))
                {
                    await StreamWriterWordsStatistic.WriteLineAsync($"{word.Key} - {word.Value.ToString()}");
                }



                await StreamWriterOffers.FlushAsync();
                await streamOffers.DisposeAsync();

                using (StreamReader sr = new StreamReader(assemblyPath + "\\Resource\\SampleOffers.txt"))
                {
                    bool writteLong = false;
                    bool writeCurz = false;

                    int lineNumber = 0;

                    var dict = letterCountDyctionary.OrderBy(e => e.Value).Last();
                    await StreamWriterOtherInformation.WriteLineAsync($"The most common letter:\n {dict.Key} - {dict.Value.ToString()}");

                    while (sr != null)
                    {
                        var ci = await sr.ReadLineAsync();

                        if (lineNumber == longsteOffersOfSimbalsIndex)
                        {
                            //var longStr = await sr.ReadLineAsync();
                            await StreamWriterOtherInformation.WriteLineAsync($"This is a great offer for the number of characters:\n {ci}");
                            writteLong = true;
                        }
                        if (lineNumber == shortestOffersOfWordsIndex)
                        {
                            //var Str = await sr.ReadLineAsync();
                            await StreamWriterOtherInformation.WriteLineAsync($"This is the smallest sentence in terms of the number of words:\n {ci}");
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
                Console.WriteLine("\n --------------------------------------------------------");
                Console.WriteLine($"\n {e}");
                Console.WriteLine("\n --------------------------------------------------------");

                await StreamWriterOffers.FlushAsync();
            }
            finally
            {
                await StreamWriterPunctuationSymbols.FlushAsync();
                await StreamWriterWordsStatistic.FlushAsync();
                await StreamWriterWords.FlushAsync();
                await StreamWriterOtherInformation.FlushAsync();

                streamOffers.DisposeAsync();
                streamWords.DisposeAsync();
                streamOtherInformation.DisposeAsync();
                streamWordsStatistic.DisposeAsync();
                streamPunctuationSymbols.DisposeAsync();
                Process.Start("explorer.exe", assemblyPath + "\\Resource\\");
            }
        }

        public static void checkLongOffersOfSimbals(int index, int length)
        {
            if (offersNumber == 0)
            {
                longsteOffersOfSimbalsIndex = 0;
                longsteOffersOfSimbalLength = length;
            }
            else if (longsteOffersOfSimbalLength < length)
            {
                longsteOffersOfSimbalLength = length;
                longsteOffersOfSimbalsIndex = index;
            }

        }
        public static void checkShortOffersOfWords(int index, int count)
        {
            if (offersNumber == 0)
            {
                shortestOffersOfWordsIndex = 0;
                shortestOffersOfWordsCount = count;
            }
            else if (shortestOffersOfWordsCount > count)
            {
                shortestOffersOfWordsCount = count;
                shortestOffersOfWordsIndex = index;
            }

        }

        public static void addToWordsDictionary(string word)
        {
            int tempInt;
            if (!int.TryParse(word, out tempInt))
            {
                if (wordsCountDictyonary.ContainsKey(word))
                {
                    wordsCountDictyonary[word]++;
                }
                else
                {
                    wordsCountDictyonary.Add(word, 1);
                }
            }
        }

        public static void addToletterCountDyctionary(char[] leters)
        {
            foreach (var leter in leters)
            {
                if (letterCountDyctionary.ContainsKey(leter))
                {
                    letterCountDyctionary[leter]++;
                }
                else
                {
                    letterCountDyctionary.Add(leter, 0);
                }
            }
        }
    }
}

        

        
