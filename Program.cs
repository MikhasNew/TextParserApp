using System;
using System.Collections.Generic;
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

        static int longsteOffersIndex;
        static int longsteOffersWordsLength;
        static int shortestOffersIndex;
        static int shortestOffersWordsLengtht;

        public static int offersNumber = 0;

        static async Task Main(string[] args)
        {
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

            string tempString = "";

            try
            {
                using (var sr = new StreamReader(resPath))
                {
                    var stringLine = sr.ReadLine() + "\n";

                    while (stringLine != null)
                    {
                        var offersItems = offersSplitRegex.Split(string.Concat(tempString, stringLine + "\n")); // in dictionary test dell  + "\n"

                        for (int i = 0; i < offersItems.Length - 1; i++)
                        {
                            checkLongAndShortOffers(offersItems[i].Length);

                            var words = wordsRegex.Matches(offersItems[i]);
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

                        offersNumber++;
                    }

                }
                await StreamWriterPunctuationSymbols.WriteLineAsync(punctList);
                
                //StringBuilder buferString = new StringBuilder();
                foreach (var word in wordsCountDictyonary.OrderBy(word => word.Key))
                {
                    await StreamWriterWordsStatistic.WriteLineAsync($"{word.Key} - {word.Value.ToString()}");
                }

                //StringBuilder longsteString = new StringBuilder();
                
                //await StreamWriterOffers.FlushAsync();
                //await streamOffers.DisposeAsync();
                //using (StreamReader sr = new StreamReader(assemblyPath + "\\Resource\\SampleOffers.txt"))
                //{
                //    //bool writteLong = false;
                //    //bool writeCurz = false;
                //    int lineNumber=0;
                //    await sr.ReadLineAsync();

                //    while (sr != null)
                //    {
                //        if (lineNumber == longsteOffersIndex)
                //        {
                //            var longStr = await sr.ReadLineAsync();
                //            await StreamWriterOtherInformation.WriteLineAsync(longStr);
                //        }
                //        if (lineNumber == shortestOffersIndex)
                //        {
                //            var longStr = await sr.ReadLineAsync();
                //            await StreamWriterOtherInformation.WriteLineAsync(longStr);
                //        }
                //        lineNumber++;
                //    }
                //}


            }
            catch (Exception e)
            {
                Console.WriteLine("\n --------------------------------------------------------");
                Console.WriteLine($"\n {e}");
                Console.WriteLine("\n --------------------------------------------------------");
            }
            finally
            {
                await StreamWriterOffers.FlushAsync();
                await StreamWriterPunctuationSymbols.FlushAsync();
                await StreamWriterWordsStatistic.FlushAsync();
                await StreamWriterWords.FlushAsync();
                await streamOtherInformation.FlushAsync();

                streamOffers.DisposeAsync();
                streamWords.DisposeAsync();
                streamOtherInformation.DisposeAsync();
                streamWordsStatistic.DisposeAsync();
                streamPunctuationSymbols.DisposeAsync();
            }
        }

        public static void checkLongAndShortOffers(int length)
        {
            if (offersNumber == 0)
            {
                longsteOffersIndex = 0;
                longsteOffersWordsLength = length;

                shortestOffersIndex = 0;
                shortestOffersWordsLengtht = length;
            }
            else if (longsteOffersWordsLength < length)
            {
                longsteOffersWordsLength = length;
                longsteOffersIndex = offersNumber;
            }
            else if (shortestOffersWordsLengtht > length)
            {
                shortestOffersIndex = offersNumber;
                shortestOffersWordsLengtht = length;
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

        

        //public static KeyValuePair<char, int> getMostCommonLetter()
        //{
           
        //    foreach (var word in wordsCountDictyonary)
        //    {
        //       addToletterCountDyctionary(word.Key.ToCharArray());
        //    }
            
            
        //    return letterCountDyctionary.OrderByDescending(e => e.Value).First();

        //}

    }
}

