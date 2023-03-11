using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using CottrellA4.Models;
using Newtonsoft.Json;

namespace CottrellA4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string file = "Data/movies.csv";
            FileReader reader = new FileReader();
            JsonReader jsonReader = new JsonReader();


            if (!File.Exists(file))
            {
                Console.WriteLine("The file does not exist");
            }
            else
            {
                string choice;
                do
                {
                    // display choices to user
                    Console.WriteLine("1) Add Movie");
                    Console.WriteLine("2) View All Movies");
                    Console.WriteLine("Enter to quit");


                    // input selection
                    choice = Console.ReadLine();
                    Console.WriteLine($"User Choice: {choice}");

                    // create parallel lists of movie details
                    // lists must be used since we do not know number of lines of data
                    List<UInt64> MovieIds = new List<UInt64>();
                    List<string> MovieTitles = new List<string>();
                    List<string> MovieGenres = new List<string>();
                    // to populate the lists with data, read from the data file
                    try
                    {
                        StreamReader sr = new StreamReader(file);
                        // first line contains column headers
                        sr.ReadLine();
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            // first look for quote(") in string
                            // this indicates a comma(,) in movie title
                            int idx = line.IndexOf('"');
                            if (idx == -1)
                            {
                                // no quote = no comma in movie title
                                // movie details are separated with comma(,)
                                string[] movieDetails = line.Split(',');
                                // 1st array element contains movie id
                                MovieIds.Add(UInt64.Parse(movieDetails[0]));
                                // 2nd array element contains movie title
                                MovieTitles.Add(movieDetails[1]);
                                // 3rd array element contains movie genre(s)
                                // replace "|" with ", "
                                MovieGenres.Add(movieDetails[2].Replace("|", ", "));
                            }
                            else
                            {
                                // quote = comma in movie title
                                // extract the movieId
                                MovieIds.Add(UInt64.Parse(line.Substring(0, idx - 1)));
                                // remove movieId and first quote from string
                                line = line.Substring(idx + 1);
                                // find the next quote
                                idx = line.IndexOf('"');
                                // extract the movieTitle
                                MovieTitles.Add(line.Substring(0, idx));
                                // remove title and last comma from the string
                                line = line.Substring(idx + 2);
                                // replace the "|" with ", "
                                MovieGenres.Add(line.Replace("|", ", "));
                            }
                        }
                        // close file when done
                        sr.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    Console.WriteLine($"Movies in file {MovieIds.Count}");

                    if (choice == "1")
                    {
                        jsonReader.AddMovie(MovieIds,MovieTitles,MovieGenres,file);
                    }
                    else if (choice == "2")
                    {
                        jsonReader.viewMovie(MovieIds,MovieTitles,MovieGenres,file);
                    }
                } while (choice == "1" || choice == "2");
            }
        }
    }


    public class JsonReader
    {
        public string json = " ";

        public void viewMovie(List<UInt64> MovieIds, List<string> MovieTitles, List<string> MovieGenres, string file)
        {
            //The adding to movies seemed to work, the viewing continues to give me issues.
            Movie m = JsonConvert.DeserializeObject<Movie>(json);
            UInt64 theId = m.ID;
            string theTitle = m.Title;
            string theGenre = m.Genres;



            // Display All Movies
            // loop thru Movie Lists
            Console.WriteLine("Please make a selection");
            Console.WriteLine("1) See # amount of movies");
            Console.WriteLine("2) Search by genre");
            Console.WriteLine("3) Search by Title Keyword");
            Console.WriteLine("4) See all movies");
            string seeAll = Console.ReadLine();

            if (seeAll == "1")
            {
                Console.WriteLine("How many Movies would you like to see?");
                int numberOfMovies = Convert.ToInt32(Console.ReadLine());
                for (int i = 0; i < numberOfMovies; i++)
                {
                    // display movie details
                    Console.WriteLine($"Id: {MovieIds[i]}");
                    Console.WriteLine($"Title: {MovieTitles[i]}");
                    Console.WriteLine($"Genre(s): {MovieGenres[i]}");
                    Console.WriteLine();
                }
            }
            else if (seeAll == "2")
            {

                Console.WriteLine("What genre would you like to see?");
                string inputGenre = Console.ReadLine();
                for (int i = 0; i < MovieIds.Count; i++)
                {
                    var genre = MovieGenres[i];
                    if (genre.Contains(inputGenre))
                    {
                        Console.WriteLine($"Id: {MovieIds[i]}");
                        Console.WriteLine($"Title: {MovieTitles[i]}");
                        Console.WriteLine($"Genre(s): {MovieGenres[i]}");
                        Console.WriteLine();
                    }

                }

            }
            else if (seeAll == "3")
            {
                Console.WriteLine("What title Keyword would you like to see?");
                string inputTitleKeyword = Console.ReadLine();
                for (int i = 0; i < MovieIds.Count; i++)
                {
                    var title = MovieTitles[i];
                    if (title.Contains(inputTitleKeyword))
                    {
                        Console.WriteLine($"Id: {MovieIds[i]}");
                        Console.WriteLine($"Title: {MovieTitles[i]}");
                        Console.WriteLine($"Genre(s): {MovieGenres[i]}");
                        Console.WriteLine();
                    }

                }
            }
            else if (seeAll == "4")
            {
                for (int i = 0; i < MovieIds.Count; i++)
                {
                    Console.WriteLine($"Id: {MovieIds[i]}");
                    Console.WriteLine($"Title: {MovieTitles[i]}");
                    Console.WriteLine($"Genre(s): {MovieGenres[i]}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Not a vailid input");
            }
        }

        public void AddMovie(List<UInt64> MovieIds, List<string> MovieTitles, List<string> MovieGenres, string file)
        {
            int index = 0;
            Movie movie = new Movie();
            //placeholders for vaulues
            movie.ID = 1;
            movie.Title = "Title";
            movie.Genres = "Genres";

            string json = JsonConvert.SerializeObject(movie);

            Console.WriteLine("Enter the movie title");
            // input title
            string movieTitle = Console.ReadLine();
            // check for duplicate title
            List<string> LowerCaseMovieTitles = MovieTitles.ConvertAll(t => t.ToLower());
            if (LowerCaseMovieTitles.Contains(movieTitle.ToLower()))
            {
                Console.WriteLine("That movie has already been entered");
                Console.WriteLine($"Duplicate movie title {movieTitle}");
            }
            else
            {
                // generate movie id - use max value in MovieIds + 1
                UInt64 movieId = MovieIds.Max() + 1;
                // input genres
                List<string> genres = new List<string>();
                string genre;
                do
                {
                    // ask user to enter genre
                    Console.WriteLine("Enter genre (or done to quit)");
                    // input genre
                    genre = Console.ReadLine();
                    // if user enters "done"
                    // or does not enter a genre do not add it to list
                    if (genre != "done" && genre.Length > 0)
                    {
                        genres.Add(genre);
                    }
                } while (genre != "done");
                // specify if no genres are entered
                if (genres.Count == 0)
                {
                    genres.Add("(no genres listed)");
                }
                // use "|" as delimeter for genres
                string genresString = string.Join("|", genres);
                // if there is a comma(,) in the title, wrap it in quotes
                movieTitle = movieTitle.IndexOf(',') != -1 ? $"\"{movieTitle}\"" : movieTitle;
                // display movie id, title, genres
                Console.WriteLine($"{movieId},{movieTitle},{genresString}");
                // create file from data
                StreamWriter sw = new StreamWriter(file, true);
                sw.WriteLine($"{movieId},{movieTitle},{genresString}");
                sw.Close();
                // add movie details to Lists
                MovieIds.Add(movieId);
                MovieTitles.Add(movieTitle);
                MovieGenres.Add(genresString);
                // log transaction
                Console.WriteLine($"Movie added");
            }
        }
    }




    //public class FileReader 
    //{
    //    public void viewMovie(List<UInt64> MovieIds, List<string> MovieTitles, List<string> MovieGenres, string file)
    //    {
    //        // Display All Movies
    //        // loop thru Movie Lists
    //        Console.WriteLine("Please make a selection");
    //        Console.WriteLine("1) See # amount of movies");
    //        Console.WriteLine("2) Search by genre");
    //        Console.WriteLine("3) Search by Title Keyword");
    //        Console.WriteLine("4) See all movies");
    //        string seeAll = Console.ReadLine();

    //        if (seeAll == "1")
    //        {
    //            Console.WriteLine("How many Movies would you like to see?");
    //            int numberOfMovies = Convert.ToInt32(Console.ReadLine());
    //            for (int i = 0; i < numberOfMovies; i++)
    //            {
    //                // display movie details
    //                Console.WriteLine($"Id: {MovieIds[i]}");
    //                Console.WriteLine($"Title: {MovieTitles[i]}");
    //                Console.WriteLine($"Genre(s): {MovieGenres[i]}");
    //                Console.WriteLine();
    //            }
    //        }
    //        else if (seeAll == "2")
    //        {

    //            Console.WriteLine("What genre would you like to see?");
    //            string inputGenre = Console.ReadLine();
    //            for (int i = 0; i < MovieIds.Count; i++)
    //            {
    //                var genre = MovieGenres[i];
    //                if (genre.Contains(inputGenre))
    //                {
    //                    Console.WriteLine($"Id: {MovieIds[i]}");
    //                    Console.WriteLine($"Title: {MovieTitles[i]}");
    //                    Console.WriteLine($"Genre(s): {MovieGenres[i]}");
    //                    Console.WriteLine();
    //                }

    //            }

    //        }
    //        else if (seeAll == "3")
    //        {
    //            Console.WriteLine("What title Keyword would you like to see?");
    //            string inputTitleKeyword = Console.ReadLine();
    //            for (int i = 0; i < MovieIds.Count; i++)
    //            {
    //                var title = MovieTitles[i];
    //                if (title.Contains(inputTitleKeyword))
    //                {
    //                    Console.WriteLine($"Id: {MovieIds[i]}");
    //                    Console.WriteLine($"Title: {MovieTitles[i]}");
    //                    Console.WriteLine($"Genre(s): {MovieGenres[i]}");
    //                    Console.WriteLine();
    //                }

    //            }
    //        }
    //        else if (seeAll == "4")
    //        {
    //            for (int i = 0; i < MovieIds.Count; i++)
    //            {
    //                // display movie details
    //                Console.WriteLine($"Id: {MovieIds[i]}");
    //                Console.WriteLine($"Title: {MovieTitles[i]}");
    //                Console.WriteLine($"Genre(s): {MovieGenres[i]}");
    //                Console.WriteLine();
    //            }
    //        }
    //        else
    //        {
    //            Console.WriteLine("Not a vailid input");
    //        }
    //    }
    //    public void AddMovie(List<UInt64> MovieIds, List<string> MovieTitles, List<string> MovieGenres, string file)
    //    {
           

    //        Console.WriteLine("Enter the movie title");
    //        // input title
    //        string movieTitle = Console.ReadLine();
    //        // check for duplicate title
    //        List<string> LowerCaseMovieTitles = MovieTitles.ConvertAll(t => t.ToLower());
    //        if (LowerCaseMovieTitles.Contains(movieTitle.ToLower()))
    //        {
    //            Console.WriteLine("That movie has already been entered");
    //            Console.WriteLine($"Duplicate movie title {movieTitle}");
    //        }
    //        else
    //        {
    //            // generate movie id - use max value in MovieIds + 1
    //            UInt64 movieId = MovieIds.Max() + 1;
    //            // input genres
    //            List<string> genres = new List<string>();
    //            string genre;
    //            do
    //            {
    //                // ask user to enter genre
    //                Console.WriteLine("Enter genre (or done to quit)");
    //                // input genre
    //                genre = Console.ReadLine();
    //                // if user enters "done"
    //                // or does not enter a genre do not add it to list
    //                if (genre != "done" && genre.Length > 0)
    //                {
    //                    genres.Add(genre);
    //                }
    //            } while (genre != "done");
    //            // specify if no genres are entered
    //            if (genres.Count == 0)
    //            {
    //                genres.Add("(no genres listed)");
    //            }
    //            // use "|" as delimeter for genres
    //            string genresString = string.Join("|", genres);
    //            // if there is a comma(,) in the title, wrap it in quotes
    //            movieTitle = movieTitle.IndexOf(',') != -1 ? $"\"{movieTitle}\"" : movieTitle;
    //            // display movie id, title, genres
    //            Console.WriteLine($"{movieId},{movieTitle},{genresString}");
    //            // create file from data
    //            StreamWriter sw = new StreamWriter(file, true);
    //            sw.WriteLine($"{movieId},{movieTitle},{genresString}");
    //            sw.Close();
    //            // add movie details to Lists
    //            MovieIds.Add(movieId);
    //            MovieTitles.Add(movieTitle);
    //            MovieGenres.Add(genresString);
    //            // log transaction
    //            Console.WriteLine($"Movie added");
    //        }
    //    }
    //}
}
