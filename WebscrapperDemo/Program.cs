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

public class Movie
{
    public string? Title { get; set; }
    public string? Score { get; set; }
    public int Year { get; set; }
    public string? Cast { get; set; }
    public string? Link { get; set; }
}
public class Job
{
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyLocation { get; set; }
    public string? Keywoards { get; set; }
    public string? Link { get; set; }
}
public class Video
{
    public string? Link { get; set; }
    public string? VideoTitle { get; set; }
    public string? Uploader { get; set; }
    public string? NumberOfViews { get; set; }
}



namespace WebScraperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
        
        Console.WriteLine("What Site do you want to scrape: \n1. Youtube\n2. ICT Jobs\n3. RottenTomatoes");
            var siteChoice = Console.ReadLine();

            while (string.IsNullOrEmpty(siteChoice) || siteChoice != "1" && siteChoice != "2" && siteChoice != "3")
            {
                Console.WriteLine("You have to make a choice:\n1. Youtube\n2.ICT Jobs\n3. RottenTomatoes");
                siteChoice = Console.ReadLine();
            }


            var counter = 1;
            var arrCounter = 0;

            string[] allLinesArray = new string[6];

            // Create a list of objects
            List<Movie> movieObjects = new List<Movie>();
            List<Job> jobObjects = new List<Job>();
            List<Video> videoObjects = new List<Video>();

            //Get attribute function
            string GetAttributeWithDefault(IWebElement element, string attributeName, string defaultValue)
            {
                string attributeValue = element.GetAttribute(attributeName);
                if (string.IsNullOrEmpty(attributeValue))
                {
                    attributeValue = defaultValue;
                }
                return attributeValue.Replace(",","|");
            }

            if (siteChoice == "1")
            {
                Console.WriteLine("Enter what to searh for on youtube:");
                var userInput = Console.ReadLine();
                while (string.IsNullOrEmpty(userInput))
                {
                    Console.WriteLine("You have to enter what to search for on youtube:");
                    userInput = Console.ReadLine();
                }

                //remove start and end whitespaces
                userInput = userInput.Trim();

                //Open Chrome
                IWebDriver driver = new ChromeDriver();

                //set wait
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));



                //rand Search on Youtube
                driver.Navigate().GoToUrl("https://www.youtube.com/results?search_query=" + userInput + "&sp=CAI%253D");



                //wait till cookie is visible
                //IWebElement cookie = wait.Until(driver => driver.FindElement(By.XPath("/html/body/ytd-app/ytd-consent-bump-v2-lightbox/tp-yt-paper-dialog/div[4]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]")));

                try
                {
                    IWebElement cookie = wait.Until(driver => driver.FindElement(By.XPath("/html/body/ytd-app/ytd-consent-bump-v2-lightbox/tp-yt-paper-dialog/div[4]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]")));
                    //accept cookie
                    cookie.Click();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
             

                //wait for 5 seconds
                Thread.Sleep(5000); // Wait for 5 seconds

               
                string videoHeader = "Video Link,Title,Uploader, Views";
                allLinesArray[arrCounter] = videoHeader;
                Console.WriteLine(allLinesArray[arrCounter]);
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


                    string videoTitle = driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/div/h3/a/yt-formatted-string", counter))).Text;
                    string videoUploader = driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[2]/ytd-channel-name/div/div/yt-formatted-string/a", counter))).Text;
                    string videoViews = driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/ytd-video-meta-block/div[1]/div[2]/span[1]", counter))).Text;
                    string link = driver.FindElement(By.XPath(String.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/div/h3/a", counter))).Text;
                    string result = link +","+videoTitle.Replace(",", "|") + "," + videoUploader.Replace(",", "|") + "," + videoViews;
                    Console.WriteLine(result);
                    Video videoObject = new Video
                    {
                        Link = link,
                        VideoTitle = videoTitle,
                        Uploader = videoUploader,
                        NumberOfViews = videoViews,
                        
                    };

                    videoObjects.Add(videoObject);
                    arrCounter++;
                    allLinesArray[arrCounter] = result;
                    
                    counter++;
                }

                Console.WriteLine("Done Youtube");
            }
            if (siteChoice == "2")
            {
                Console.WriteLine("Enter what to searh for on ICT Jobs:");
                var userInput = Console.ReadLine();
                while (string.IsNullOrEmpty(userInput))
                {
                    Console.WriteLine("You have to enter what to search for on youtube:");
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
                Thread.Sleep(4000); // Wait for 4 seconds
                //Click on by date
                driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[1]/div[2]/div/div[2]/span[2]/a")).Click();

                //wait for 10 seconds
                Thread.Sleep(10000);
                string jobHeader = "Job Title,CompanyName,Company Location, Keywoards, Link to Job";
                allLinesArray[arrCounter] = jobHeader;
                IList<IWebElement> allJobs = driver.FindElements(By.CssSelector("[class='search-item  clearfix']"));
   

                while (counter <= 5)
                {
                    //wait until job is displayed
                    wait.Until(driver => driver.FindElement(By.XPath(String.Format("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[{0}]", counter))));

                    IWebElement job = allJobs[arrCounter];
                    wait.Until(driver => job.FindElement(By.CssSelector(("[class='job-keywords']"))));

                    var jobTitle = job.FindElement(By.ClassName(("job-title")));
                    var companyName = job.FindElement(By.ClassName(("job-company")));
                    var companyLocation = job.FindElement(By.ClassName(("job-location")));
                    var keywoards = job.FindElement(By.CssSelector(("[class='job-keywords']")));
                    var link = job.FindElement(By.ClassName(("search-item-link")));

                    string jobresult = jobTitle.Text.Replace(",", "|") + "," + companyName.Text.Replace(",", "|") + "," + companyLocation.Text.Replace(",","|") + "," + keywoards.Text.Replace(",", "|") + "," + link.GetAttribute("href");
                    Console.WriteLine(counter + " " + jobresult);
                    Job jobObject = new Job
                    {
                        JobTitle = jobTitle.Text,
                        CompanyName = companyName.Text,
                        CompanyLocation = companyLocation.Text,
                        Keywoards = keywoards.Text,
                        Link = link.GetAttribute("href")
                    };

                    jobObjects.Add(jobObject);
                    counter++;
                    arrCounter++;
                    allLinesArray[arrCounter] = jobresult;                    
                }
                Console.WriteLine("Done ICT jobs");

            }
            if (siteChoice == "3")
            {
                Console.WriteLine("Enter a name to search for on ....:");
                var userInput = Console.ReadLine();
                while (string.IsNullOrEmpty(userInput))
                {
                    Console.WriteLine("You have to enter a name  to search for on ...:");
                    userInput = Console.ReadLine();
                }

                
                userInput = userInput.Trim().Replace(" ", "+");

                //Open Chrome
                IWebDriver driver = new ChromeDriver();

                //set wait
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                //open link
                driver.Navigate().GoToUrl("https://www.rottentomatoes.com/search?search=" + userInput);



                //wait till cookie is visible
                IWebElement cookie = wait.Until(driver => driver.FindElement(By.Id("onetrust-accept-btn-handler")));

                //accept cookie
                cookie.Click();

                //Click on Movies Section
                wait.Until(e => e.FindElement(By.CssSelector("[data-filter='movie']"))).Click();
                
                //wait for 5 seconds
                Thread.Sleep(3000); 

                
                

                

                IList<IWebElement> allMovies = driver.FindElements(By.TagName("search-page-media-row"));
                string movieHeader = "Title,Score,Release Year, Major Cast, Link to movie";
                allLinesArray[arrCounter] = movieHeader;
                foreach (IWebElement eachMovie in allMovies)
                {
                    IWebElement title = eachMovie.FindElement(By.CssSelector("[slot='title']"));
                    string year = GetAttributeWithDefault(eachMovie,"releaseYear","No release Year");
                    string tomatometerScore = GetAttributeWithDefault(eachMovie, "tomatometerscore", "-");
                    string cast = GetAttributeWithDefault(eachMovie, "cast", "-");
                    IWebElement link = eachMovie.FindElement(By.CssSelector("[data-qa='thumbnail-link']"));
                    string linkToMovie = GetAttributeWithDefault(link, "href", "No link");
                    var movie = title.Text.Replace(",", "|") + "," + tomatometerScore + "," + year + "," + cast + "," + linkToMovie;
                    string titleString = title.Text.Replace(",", "|");
                    Console.WriteLine(movie);
                    Movie movieObject = new Movie
                    {
                        Title = titleString,
                        Score = tomatometerScore,
                        Year = int.Parse(year),
                        Cast = cast,
                        Link = linkToMovie
                    };

                    movieObjects.Add(movieObject);
                    
                    counter++;
                    arrCounter++;
                    allLinesArray[arrCounter] = movie;
                    if (counter == 6)
                    {
                        break;
                    }


                }
                
                Console.WriteLine("Movies Done");
            }
            using (StreamWriter writer = new StreamWriter("jsonOut.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
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
            using (StreamWriter writer = new StreamWriter("output.csv"))
            {

                writer.WriteLine("Sep=,");
                // Loop over the array
                foreach (string item in allLinesArray)
                {
                    writer.WriteLine(item);
                    
                }
            }
            Console.WriteLine("Done");
            Process.Start("open","output.csv");
            Console.ReadLine();


        }

    }
}

