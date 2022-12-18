using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Diagnostics;
using System.Security.Claims;

// Class to represent a video object
public class Video
{
    public string? Link { get; set; }
    public string? VideoTitle { get; set; }
    public string? Uploader { get; set; }
    public string? NumberOfViews { get; set; }
}

// Class to represent a job object
public class Job
{
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyLocation { get; set; }
    public string? Keywoards { get; set; }
    public string? Link { get; set; }
}

// Class to represent a movie object
public class Movie
{
    public string? Title { get; set; }
    public string? Score { get; set; }
    public int Year { get; set; }
    public string? Cast { get; set; }
    public string? Link { get; set; }
}


namespace WebScraperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Prompt the user to choose a site to scrape
            Console.WriteLine("What Site do you want to scrape: \n1. Youtube\n2. ICT Jobs\n3. RottenTomatoes");
            var siteChoice = Console.ReadLine();

            // Loop until the user provides a valid input (either "1", "2", or "3")
            while (string.IsNullOrEmpty(siteChoice) || siteChoice != "1" && siteChoice != "2" && siteChoice != "3")
            {
                Console.WriteLine("You have to make a choice:\n1. Youtube\n2.ICT Jobs\n3. RottenTomatoes");
                siteChoice = Console.ReadLine();
            }

            // Initialize variables
            var counter = 1;
            var arrCounter = 0;

            //Make a string array to store the header and 5 other records
            string[] allLinesArray = new string[6];

            // Create a list of objects
            List<Movie> movieObjects = new List<Movie>();
            List<Job> jobObjects = new List<Job>();
            List<Video> videoObjects = new List<Video>();

            // Function to get the value of an attribute for a given element, with a default value if the attribute does not exist
            string GetAttributeWithDefault(IWebElement element, string attributeName, string defaultValue)
            {
                string attributeValue = element.GetAttribute(attributeName);
                if (string.IsNullOrEmpty(attributeValue))
                {
                    attributeValue = defaultValue;
                }
                return attributeValue.Replace(",", "|");
            }


            // Scrape YouTube
            if (siteChoice == "1")
            {
                // Prompt the user to enter a search term
                Console.WriteLine("Enter what to searh for on youtube:");
                var userInput = Console.ReadLine();

                // Loop until the user provides a search term
                while (string.IsNullOrEmpty(userInput))
                {
                    Console.WriteLine("You have to enter what to search for on youtube:");
                    userInput = Console.ReadLine();
                }

                //remove start and end whitespaces and replace middle ones with +
                userInput = userInput.Trim().Replace(" ", "+");

                //Open Chrome
                IWebDriver driver = new ChromeDriver();

                //set wait
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));



                // Search for the user-provided term on YouTube
                driver.Navigate().GoToUrl("https://www.youtube.com/results?search_query=" + userInput + "&sp=CAI%253D");



                try
                {
                    // Wait until the cookie consent banner is visible and then click the "accept" button
                    IWebElement cookie = wait.Until(driver => driver.FindElement(By.XPath("/html/body/ytd-app/ytd-consent-bump-v2-lightbox/tp-yt-paper-dialog/div[4]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]")));
                    cookie.Click();
                }
                catch (Exception ex)
                {
                    //Display error message if cookie could not be accepted
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }


                //wait for 5 seconds
                Thread.Sleep(5000);

                //Make a videoHeader to be added to string array and then csv file
                string videoHeader = "Video Link,Title,Uploader, Views";

                //Add video header to string array
                allLinesArray[arrCounter] = videoHeader;

                //Run this loop for five times
                while (counter <= 5)
                {
                    //wait until video is displayed
                    wait.Until(driver => driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]", counter))));
                    //wait until title is displayed
                    wait.Until(driver => driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/div/h3/a/yt-formatted-string", counter))));
                    //wait until uploader name is displayed
                    wait.Until(driver => driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[2]/ytd-channel-name/div/div/yt-formatted-string/a", counter))));
                    //wait until views is displayed
                    wait.Until(driver => driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/ytd-video-meta-block/div[1]/div[2]/span[1]", counter))));
                    //wait until link is displayed
                    wait.Until(driver => driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/div/h3/a", counter))));

                    //Get video data
                    string videoTitle = driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/div/h3/a/yt-formatted-string", counter))).Text;
                    string videoUploader = driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[2]/ytd-channel-name/div/div/yt-formatted-string/a", counter))).Text;
                    string videoViews = driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/ytd-video-meta-block/div[1]/div[2]/span[1]", counter))).Text;
                    IWebElement linkTag = driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/div/h3/a", counter)));

                    //Get attribute href using GetAttributeWithDefault function
                    string link = GetAttributeWithDefault(linkTag, "href", "No link");

                    //Make a result variable and assign all video data to it
                    string result = link + "," + videoTitle.Replace(",", "|") + "," + videoUploader.Replace(",", "|") + "," + videoViews;

                    //Make a new video object and add all video data to it
                    Video videoObject = new Video
                    {
                        Link = link,
                        VideoTitle = videoTitle,
                        Uploader = videoUploader,
                        NumberOfViews = videoViews,

                    };

                    //Add video object to videoObjects
                    videoObjects.Add(videoObject);
                    arrCounter++;

                    //add results to string array
                    allLinesArray[arrCounter] = result;
                    counter++;
                }

                Console.WriteLine("Done Youtube");
            }
            // Scrape ICT Jobs
            if (siteChoice == "2")
            {
                // Prompt the user to enter a search term
                Console.WriteLine("Enter what job to searh for on ICT Jobs:");
                var userInput = Console.ReadLine();

                // Loop until the user provides a search term
                while (string.IsNullOrEmpty(userInput))
                {
                    Console.WriteLine("You have to enter a Job title:");
                    userInput = Console.ReadLine();
                }
                //Open Chrome
                IWebDriver driver = new ChromeDriver();
                //set wait
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                //remove start and end whitespaces and replace middle ones with +
                userInput = userInput.Trim().Replace(" ", "+");



                //Go to ICT Jobs
                driver.Navigate().GoToUrl("https://www.ictjob.be/en/search-it-jobs?keywords=" + userInput);

                //wait for 4 seconds
                Thread.Sleep(4000);

                //Click on by sort by date
                driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[1]/div[2]/div/div[2]/span[2]/a")).Click();

                //wait for 10 seconds
                Thread.Sleep(10000);

                //Make a jobHeader to be added to string array and then csv file
                string jobHeader = "Job Title,CompanyName,Company Location, Keywoards, Link to Job";

                //Add video header to string array
                allLinesArray[arrCounter] = jobHeader;

                //Get all jobs 
                IList<IWebElement> allJobs = driver.FindElements(By.CssSelector("[class='search-item  clearfix']"));

                //Run this loop for five times
                while (counter <= 5)
                {
                    //wait until job is displayed
                    wait.Until(driver => driver.FindElement(By.XPath(String.Format("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[{0}]", counter))));

                    //Get a job using its index
                    IWebElement job = allJobs[arrCounter];

                    //Wait till the job is found in DOM
                    wait.Until(driver => job.FindElement(By.CssSelector(("[class='job-keywords']"))));

                    //Get job data
                    var jobTitle = job.FindElement(By.ClassName(("job-title")));
                    var companyName = job.FindElement(By.ClassName(("job-company")));
                    var companyLocation = job.FindElement(By.ClassName(("job-location")));
                    var keywoards = job.FindElement(By.CssSelector(("[class='job-keywords']")));
                    var link = job.FindElement(By.ClassName(("search-item-link")));

                    //Make a jobresult variable and assign all job data to it
                    string jobresult = jobTitle.Text.Replace(",", "|") + "," + companyName.Text.Replace(",", "|") + "," + companyLocation.Text.Replace(",", "|") + "," + keywoards.Text.Replace(",", "|") + "," + link.GetAttribute("href");

                    //Make a new job object and add all video data to it
                    Job jobObject = new Job
                    {
                        JobTitle = jobTitle.Text,
                        CompanyName = companyName.Text,
                        CompanyLocation = companyLocation.Text,
                        Keywoards = keywoards.Text,
                        Link = link.GetAttribute("href")
                    };

                    //Add job object to jobObjects
                    jobObjects.Add(jobObject);
                    counter++;
                    arrCounter++;

                    //add job results to string array
                    allLinesArray[arrCounter] = jobresult;
                }


            }
            if (siteChoice == "3")
            {
                // Prompt the user to enter a search term
                Console.WriteLine("Enter a movie title to search for on RottenTomatoes:");
                var userInput = Console.ReadLine();
                // Loop until the user provides a search term
                while (string.IsNullOrEmpty(userInput))
                {
                    Console.WriteLine("You have to enter a movie title:");
                    userInput = Console.ReadLine();
                }

                //remove start and end whitespaces and replace middle ones with +
                userInput = userInput.Trim().Replace(" ", "+");

                //Open Chrome
                IWebDriver driver = new ChromeDriver();

                //set wait
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                //open link
                driver.Navigate().GoToUrl("https://www.rottentomatoes.com/search?search=" + userInput);



                //wait till cookie is visible and click it
                IWebElement cookie = wait.Until(driver => driver.FindElement(By.Id("onetrust-accept-btn-handler")));
                cookie.Click();

                //Click on Movies Section
                wait.Until(e => e.FindElement(By.CssSelector("[data-filter='movie']"))).Click();

                //wait for 3 seconds
                Thread.Sleep(3000);

                //Get all movies 
                IList<IWebElement> allMovies = driver.FindElements(By.TagName("search-page-media-row"));

                //Make a movieHeader to be added to string array and then csv file
                string movieHeader = "Title,Score,Release Year, Major Cast, Link to movie";

                //Add video header to string array
                allLinesArray[arrCounter] = movieHeader;

                //Do the following for each movies in all the movies
                foreach (IWebElement eachMovie in allMovies)
                {
                    //Get movie data
                    IWebElement titleElement = eachMovie.FindElement(By.CssSelector("[slot='title']"));
                    string year = GetAttributeWithDefault(eachMovie, "releaseYear", "No release Year");
                    string tomatometerScore = GetAttributeWithDefault(eachMovie, "tomatometerscore", "-");
                    string cast = GetAttributeWithDefault(eachMovie, "cast", "-");
                    IWebElement link = eachMovie.FindElement(By.CssSelector("[data-qa='thumbnail-link']"));
                    string linkToMovie = GetAttributeWithDefault(link, "href", "No link");
                    string title = titleElement.Text.Replace(",", "|");

                    //Make a movieResult variable and assign all job data to it
                    var movieResult = title + "," + tomatometerScore + "," + year + "," + cast + "," + linkToMovie;

                    //Make a new movie object and add all movie data to it
                    Movie movieObject = new Movie
                    {
                        Title = title,
                        Score = tomatometerScore,
                        Year = int.Parse(year),
                        Cast = cast,
                        Link = linkToMovie
                    };

                    //Add movie object to movieObjects
                    movieObjects.Add(movieObject);

                    counter++;
                    arrCounter++;
                    //add job results to string array
                    allLinesArray[arrCounter] = movieResult;

                    //Break code if counter = 6 so only five movies are stored
                    if (counter == 6)
                    {
                        break;
                    }


                }

                Console.WriteLine("Movies Done");
            }

            // Use streamwriter to create and a file by giving it a string add file extension ie. .json to assign file type
            using (StreamWriter writer = new StreamWriter("jsonOut.json"))
            {
                // use JsonSerializer to serialize objects into JSON format
                JsonSerializer serializer = new JsonSerializer();

                // switch statement to choose which list of objects to serialize based on user input
                switch (siteChoice)
                {
                    case "1":
                        serializer.Serialize(writer, videoObjects);
                        break;
                    case "2":
                        serializer.Serialize(writer, jobObjects);
                        break;
                    case "3":
                        serializer.Serialize(writer, movieObjects);
                        break;
                }
            }
            // Use streamwriter to create and a file by giving it a string add file extension ie. .csv to assign file type
            using (StreamWriter writer = new StreamWriter("output.csv"))
            {
                // Write the separator line for the CSV file
                writer.WriteLine("Sep=,");

                // Loop over the array of lines
                foreach (string item in allLinesArray)
                {
                    // Write each line to the CSV file
                    writer.WriteLine(item);
                }
            }
            // Output a message indicating that the process is done
            Console.WriteLine("Done");

            // Open the CSV and JSON file in the default program
            Process.Start("open", "output.csv");
            Process.Start("open", "jsonOut.json");

            //Must be commented
            Console.ReadLine();


        }

    }
}

