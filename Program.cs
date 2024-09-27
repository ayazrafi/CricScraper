using HtmlAgilityPack;
using System;
using System.Linq;
using System.IO;
using BD.Repository;
using System.Data;
using BD.UtilityMethod;
using ScraperApp.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using RestSharp;
using System.Reflection.Metadata;
using System.Numerics;


public class Player : PlayerPartInfo
{
    public string Name { get; set; }
    public int RunsScored { get; set; }
    public int Fours { get; set; }
    public int Sixes { get; set; }
    public int WicketsTaken { get; set; }
    public int MaidenOvers { get; set; }
    public double EconomyRate { get; set; }
    public bool IsCaptain { get; set; } = false;
    public bool IsViceCaptain { get; set; } = false;
    public int batterpoint { get; set; }
    public int bowlingpoint { get; set; }
    public int FieldingOtherpoint { get; set; }
    public int finalpoint { get; set; }
    public int captainid { get; set; }
    public int vicecaptanid { get; set; }
    public int playerid { get; set; }
    public int Balls { get; set; }
    public double strikerate { get; set; }
    //public Player(string name)
    //{
    //    Name = name;
    //}

    public int CalculateBowlingPoints()
    {
        int points = 0;
        points += WicketsTaken * 25; // 25 points per wicket
        points += MaidenOvers * 12; // 8 points per maiden over

        // Bonus for 4-wicket and 5-wicket hauls
        if (WicketsTaken >= 5)
            points += 16;
        else if (WicketsTaken >= 4)
            points += 8;
        else if (WicketsTaken >= 3)
            points += 4;

        points += LBW * 8;
        points += Bowled * 8;
        points += Catch * 8;
        if (Catch >= 3)
            points += 4;
        points += Stumping * 12;
        points += (int)(runout * 12);

        // Economy rate calculation and points

        double economyRate = EconomyRate;
        if (economyRate > 0 && economyRate < 5)
            points += 6;
        else if (economyRate >= 5 && economyRate < 6)
            points += 4;
        else if (economyRate >= 6 && economyRate < 7)
            points += 2;
        else if (economyRate >= 10 && economyRate < 11)
            points -= 2;
        else if (economyRate >= 11 && economyRate < 12)
            points -= 4;
        else if (economyRate >= 12)
            points -= 6;


        return points;
    }

    public int CalculateBattingPoints()
    {
        int points = 0;
        points += RunsScored; // 1 point per run
        points += Fours; // 1 point per four
        points += Sixes * 2; // 2 points per six

        // Bonus for half-century and century
        if (RunsScored >= 100)
            points += 16;
        else if (RunsScored >= 50 && RunsScored < 100)
            points += 8;
        else if (RunsScored >= 30)
            points += 4;

        if (Balls >= 10)
        {
            // Strike rate calculation and points
            double strikeRate = strikerate;

            if (strikeRate >= 60 && strikeRate < 70)
                points -= 2;
            else if (strikeRate >= 50 && strikeRate < 60)
                points -= 4;
            else if (strikeRate > 0 && strikeRate < 50)
                points -= 6;
            else if (strikeRate >= 170)
                points += 6;
            else if (strikeRate >= 150 && strikeRate < 170)
                points += 4;
            else if (strikeRate >= 130 && strikeRate < 150)
                points += 2;
            else if (strikeRate >= 150)
                points += 4;
        }
        // Duck penalty, excluding bowlers
        if (RunsScored == 0)
            points -= 2;

        return points;
    }

}

public class PlayerPartInfo
{
    public string PlayerName { get; set; }
    public int Catch { get; set; }
    public int LBW { get; set; }
    public double runout { get; set; }
    public int Stumping { get; set; }
    public int Bowled { get; set; }
}

class Program
{
    public void Calcgst()
    {  // Example values
        //double totalEntryFee = 100.0;
        //double platformFeePercentage = 10.0;
        //double gstRate = 28.0;

        //// Calculate Platform Fee
        //double platformFee = (platformFeePercentage / 100) * totalEntryFee;

        //// Calculate GST on Platform Fee
        //double gstAmount = (gstRate / 100) * platformFee;

        //// Calculate Net Platform Fee After GST
        //double netPlatformFee = platformFee - gstAmount;

        //Console.WriteLine($"Total Entry Fee: ₹{totalEntryFee}");
        //Console.WriteLine($"Platform Fee: ₹{platformFee}");
        //Console.WriteLine($"GST Amount: ₹{gstAmount}");
        //Console.WriteLine($"Net Platform Fee After GST: ₹{netPlatformFee}");
        double entryFee = 150.0;
        double platformFeePercentage = 5.0;
        double gstRate = 28.0;

        // Calculate Platform Fee
        double platformFee = (platformFeePercentage / 100) * entryFee;

        // Calculate GST on Platform Fee
        double gstAmount = (gstRate / 100) * platformFee;

        // Calculate Net Platform Fee After GST
        double netPlatformFee = platformFee - gstAmount;

        Console.WriteLine($"Player's Entry Fee: ₹{entryFee}");
        Console.WriteLine($"Platform Fee: ₹{platformFee}");
        Console.WriteLine($"GST Amount: ₹{gstAmount}");
        Console.WriteLine($"Net Platform Fee After GST: ₹{netPlatformFee}");
    }
    static void Main(string[] args)
    {
        //var p1 = new Program();
        //p1.Calcgst();
        //return;

        Program p = new Program();
        p.ReadCricBuzzInfoSuad();
       // p.ReadMatchData("matchhead");


    }
    public void MakeScore()
    {
        var matchid = 10;
        var db = new DBRepository();
        //var userteam = db.GetDataTable("select mtp.playerid,mtp.matchid,mut.captanid,mut.vicecaptanid,mt.playername" +
        //    " from bigdream.matchuserteamplayer mtp left join bigdream.matchuserteam " +
        //    "mut on mtp.matchuserteamid=mut.userteamid left join bigdream.matchteam " +
        //    "mt on mt.playerid=mtp.playerid  where mtp.matchid=" + matchid + ";");
        var userteam = db.GetDataTable("select mt.playerid,mt.matchid,mt.playername" +
           " from  bigdream.matchteam " +
           "mt   where mt.matchid=" + matchid + ";");
        var lstuserteam = userteam.AsEnumerable().Select(a => new Player
        {
            playerid = a["playerid"].ToString().intTP(),
            Name = a["playername"].ToString().Trim(),
            //vicecaptanid = a["vicecaptanid"].ToString().intTP(),
            // captainid = a["captanid"].ToString().intTP()
        }).ToList();
        // int captanid = lstuserteam.Select(a => a.captainid).Distinct().First();
        //int vicecaptanid = lstuserteam.Select(a => a.vicecaptanid).Distinct().First();
        //lstuserteam.ForEach(a =>
        //{
        //    if (a.playerid == captanid)
        //        a.IsCaptain = true;
        //    else if (a.playerid == vicecaptanid)
        //        a.IsViceCaptain = true;
        //});

        //Step-1
        //Get Match Team
        //Step-2
        //Loop match team player
        //find the player in the inninglist and according to type calculate point and store into variable.



        Program p = new Program();
        //p.ReadCricBuzzInfoSuad();
        var lstfirst = new List<Dictionary<string, string>>();
        var tup1 = p.ReadInning("firstinning");
        var tup2 = p.ReadInning("secondinning");
        var lstPlayerPart = new List<PlayerPartInfo>();
        tup1.Item2.AddRange(tup2.Item2);
        lstPlayerPart = tup1.Item2;
        if (lstPlayerPart.Any())
        {
            lstPlayerPart = lstPlayerPart.GroupBy(a => a.PlayerName).Select(a => new PlayerPartInfo()
            {
                PlayerName = a.First().PlayerName,
                Catch = a.Sum(j => j.Catch),
                LBW = a.Sum(j => j.LBW),
                runout = a.Sum(j => j.runout),
                Stumping = a.Sum(j => j.Stumping),
                Bowled = a.Sum(j => j.Bowled)

            }).ToList();
        }
        lstfirst.AddRange(tup1.Item1);
        lstfirst.AddRange(tup2.Item1);
        lstuserteam.ForEach(a =>
        {
            var playernamearry = a.Name.Split(" ").ToList();
            playernamearry.Add(a.Name);
            var obj = p.CheckPlayerExists(playernamearry, lstfirst, "Bowler");
            var player = new Player();
            if (obj != null)
            {
                player.WicketsTaken = obj["Wickets"].ToString().intTP();
                player.MaidenOvers = obj["Maidens"].ToString().intTP();
                player.EconomyRate = obj["Econ"].ToString().doubleTP();
            }
            //
            var objpart = p.CheckPlayerPartExists(playernamearry, lstPlayerPart);
            if (objpart != null)
            {
                player.Catch = objpart.Catch;
                player.LBW = objpart.LBW;
                player.runout = objpart.runout;
                player.Stumping = objpart.Stumping;
                player.Bowled = objpart.Bowled;
            }
            var points = player.CalculateBowlingPoints();
            a.bowlingpoint = points;
            a.FieldingOtherpoint = 0;

            var objbat = p.CheckPlayerExists(playernamearry, lstfirst, "Batter");
            if (objbat != null)
            {
                player = new Player()
                {
                    Balls = objbat["Balls"].ToString().intTP(),
                    RunsScored = objbat["Runs"].ToString().intTP(),
                    Fours = objbat["4s"].ToString().intTP(),
                    Sixes = objbat["6s"].ToString().intTP(),
                    strikerate = objbat["SR"].ToString().doubleTP(),
                };
                var battinpoint = player.CalculateBattingPoints();
                a.batterpoint = battinpoint;
            }
        });
        //p.ReadInning("firstinning");
        //var lstbatter = lstfirst.Where(a => a.ContainsKey("Batter")).ToList();
        //var lstbowler = lstfirst.Where(a => a.ContainsKey("Bowler")).ToList();
        //foreach (var item in lstbatter)
        //{
        //   
        //}
        string query = "";
        var querypart = string.Join(",", lstuserteam.Select(a =>
        {
            var score = (a.batterpoint + a.bowlingpoint + 4);
            var scorepart = a.IsCaptain == true ? 2 * score : (a.IsViceCaptain == true ? 1.5 * score : score);
            return "(" + a.playerid + "," + matchid + "," + scorepart + ")";

        }).ToList());
        db.ExecQuery("insert into bigdream.matchteampoints (playerid,matchid,score)" +
            " values " + querypart + "");
    }
    public Dictionary<string, string> CheckPlayerExists(List<string> namearray,
        List<Dictionary<string, string>> lstplayer, string type)
    {
        Dictionary<string, string> obj = null;//new Dictionary<string, string>();
        foreach (var name in namearray)
        {
            var countdata = lstplayer.FindAll(j => j.ContainsKey(type) && j[type].Trim().ToLower() == name.ToLower().Trim());
            if (countdata.Count == 1)
            {
                //calculate bowling point store into the list
                obj = countdata.First();
                break;
            }
        }
        return obj;
    }

    public PlayerPartInfo CheckPlayerPartExists(List<string> namearray,
        List<PlayerPartInfo> lstplayer)
    {
        PlayerPartInfo obj = null;//new Dictionary<string, string>();
        foreach (var name in namearray)
        {
            var countdata = lstplayer.FindAll(j => j.PlayerName.Trim().ToLower() == name.ToLower().Trim());
            if (countdata.Count == 1)
            {
                //calculate bowling point store into the list
                obj = countdata.First();
                break;
            }
        }
        return obj;
    }
    public void ReadEspnCricinfoSquad()
    {

        var matchid = 1;
        var matchcategoryid = 1;
        var matchleagueid = 1;
        var db = new DBRepository();



        var htmlDoc = new HtmlDocument();
        var html = File.ReadAllText(@"D:\\CricScraper\\htmldata1.txt");
        htmlDoc.LoadHtml(html);

        //var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='ds-w-full ds-table ds-table-sm " +
        //    "ds-table-bordered ds-border-collapse ds-border ds-border-line ds-table-auto ds-bg-fill-content-prime']");

        //var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='ds-w-full']");
        var table = htmlDoc.DocumentNode.SelectSingleNode("//table[contains(@class, 'ds-table')]");

        // Extract table header
        var headerRow = table.SelectSingleNode(".//thead//tr");
        var firstteam = "";
        var secondteam = "";
        var teamcounter = 0;
        var matchplayer = new List<MatchPlayer>();
        if (headerRow != null)
        {
            var headerCells = headerRow.SelectNodes("th");
            foreach (var cell in headerCells)
            {
                var span = cell.SelectSingleNode("span");
                if (span != null)
                {
                    teamcounter++;
                    if (teamcounter == 1)
                        firstteam = span.InnerText.Trim();
                    else if (teamcounter == 2)
                        secondteam = span.InnerText.Trim();
                    Console.Write(span.InnerText + "\t");
                }
            }
            Console.WriteLine();
        }
        var uobj = new Utility();
        var fristtemaid = uobj.SaveOriginalTeam(firstteam, "");
        var secondtemaid = uobj.SaveOriginalTeam(secondteam, "");
        var dtmatch = db.GetDataTable("insert into bigdream.match (matchname,matchcategoryid," +
            "leagueid,createdate,eventdate,team1,team2) values ('" + firstteam.Trim() + " VS " + secondteam.Trim() + "'" +
            ",'" + matchcategoryid + "','" + matchleagueid + "'," +
            "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
            "'" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "','" + fristtemaid + "','" + secondtemaid + "');select last_insert_id();");
        if (dtmatch.Rows.Count > 0)
        {
            matchid = dtmatch.Rows[0][0].ToString().intTP();
        }

        var dtPlayerCategory = db.GetDataTable("SELECT * FROM bigdream.playercategory");
        var lstplayercategory = dtPlayerCategory.AsEnumerable().Select(a => new
        {
            playercategoryId = a["playercategoryId"].ToString().intTP(),
            playercategoryshortform = a["playercategoryshortform"].ToString().Trim()
        });
        var lstorignalmatch = db.GetDataTable("SELECT * FROM bigdream.originalteam").AsEnumerable().Select(a => new
        {
            originalteamid = a["originalteamid"].ToString().intTP(),
            teamname = a["teamname"].ToString().Trim()
        });

        var rows = table.SelectNodes(".//tbody//tr");

        foreach (var row in rows)
        {
            var colcounter = 0;
            var cells = row.SelectNodes(".//td");
            foreach (var cell in cells)
            {
                var objMatch = new MatchPlayer();
                objMatch.matchid = matchid.ToString();

                var div = cell.SelectSingleNode(".//div[contains(@class, 'ds-text-tight-s ds-font-regular ds-text-center ds-pt-1')]");
                if (div != null)
                {
                    Console.Write(div.InnerText + "\t");
                }
                else
                {
                    colcounter++;
                    if (colcounter == 1)
                    {
                        objMatch.originalmatchid = lstorignalmatch.FirstOrDefault(a => a.teamname.ToLower() == firstteam.ToString().Trim().ToLower())?.originalteamid.ToString() ?? "0";
                    }
                    else if (colcounter == 2)
                    {
                        objMatch.originalmatchid = lstorignalmatch.FirstOrDefault(a => a.teamname.ToLower() == secondteam.ToString().Trim().ToLower())?.originalteamid.ToString() ?? "0";
                    }
                    var nameNode = cell.SelectSingleNode(".//a//span");
                    var roleNode = cell.SelectSingleNode(".//p");

                    if (nameNode != null)
                    {

                        objMatch.playername = nameNode.InnerText.Trim();
                        Console.Write(nameNode.InnerText.Trim() + " ");
                    }
                    else
                        Console.Write("-" + " ");

                    if (roleNode != null)
                    {
                        objMatch.playercategory = lstplayercategory.FirstOrDefault(a => a.playercategoryshortform.ToLower() == roleNode.InnerText.Trim().PlayerCategory().ToLower()).playercategoryId.ToString();//roleNode.InnerText.Trim();
                                                                                                                                                                                                                 // Console.WriteLine(lstplayercategory.FirstOrDefault(a => a.playercategoryshortform.ToLower() == roleNode.InnerText.Trim().PlayerCategory().ToLower()).playercategoryId);
                    }
                    else
                        Console.Write("-" + " ");
                    objMatch.point = 5;
                    objMatch.isannnouce = 1;
                    objMatch.credit = 6.5;
                    matchplayer.Add(objMatch);
                }

            }
        }
        matchplayer = matchplayer.FindAll(a => !(a.playercategory == null || a.playername == null));
        string query = "";
        var querypart = string.Join(",", matchplayer.Select(a =>
        {
            return "('" + a.playername + "','" + a.playercategory + "','" + a.matchid + "'," +
            "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + a.originalmatchid + "'," +
            "'" + a.point + "','" + a.credit + "','" + a.isannnouce + "')";

        }).ToList());
        db.ExecQuery("insert into bigdream.matchteam (playername,playercategoryid,matchid," +
            "createdate,originalmatchid,point,credit,isannouce) values " + querypart + "");
    }
    public static string RemoveTextWithinParentheses(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Regular expression to match text within parentheses, including the parentheses themselves
        string pattern = @"\([^)]*\)";

        // Replace matched text with an empty string
        string result = Regex.Replace(input, pattern, string.Empty);

        // Trim any resulting extra spaces
        return result.Trim();
    }
    public List<MatchPlayer> GetWKAllRounder(List<MatchPlayer> lst, List<PlayerCategory> lstplayercategory)
    {
        var lstcat = lstplayercategory.Where(a => a.playercategoryId == 1 || a.playercategoryId == 4);
        lst.ForEach(a =>
        {
            var array = a.playercategory.Split(new char[] { ' ', '-' });
            array = array.Select(a => a.ToLower()).ToArray();
            //var catinfo = "";
            //if (array.Length > 1)
            //{
            //    if (array[1].Trim() != "")
            //        catinfo = array[1].ToLower();
            //    else
            //        catinfo = array[0].ToLower();
            //}
            //else if (array.Length > 0)
            //{
            //    catinfo = array[0].ToLower();
            //}
            lstcat.ToList().ForEach(b =>
            {
                b.playercategory.ToLower().Split(",").ToList().ForEach(c =>
                {
                    var obj = array.Contains(c);
                    //var obj = b.playercategory.ToLower().Split(",").All(j => array.Contains(j));
                    // var obj = lstcat.FirstOrDefault(b => b.playercategory.ToLower().Split(",").All(j => array.Contains(j)));
                    if (obj)
                    {
                        a.playercategoryid = b.playercategoryId;
                    }
                });

            });

        });
        return lst;
    }
    public static string ReplaceSubstrings(string input, string[] substringsToRemove)
    {
        if (string.IsNullOrEmpty(input) || substringsToRemove == null || substringsToRemove.Length == 0)
        {
            return input;
        }

        foreach (string substring in substringsToRemove)
        {
            if (!string.IsNullOrEmpty(substring))
            {
                input = input.Replace(substring, string.Empty);
            }
        }

        return input.Trim();
    }
    public async void ReadCricBuzzInfoSuad()
    {
        //var htmlDoc = new HtmlDocument();
        //var html1 = File.ReadAllText(@"D:\\CricScraper\\cricbuzzsquad1.txt");
        //htmlDoc.LoadHtml(html1);
        // Select all player card elements

        //var matchdata = ReadMatchData("cricbuzzsmatch").Result;
        //dynamic d = JsonConvert.DeserializeObject(matchdata);
        var matchid = 12;//d.matchid;
        var fristtemaid = 14;//d.originalteam1id;
        var secondtemaid = 15;//d.originalteam2id;
        var db = new DBRepository();
        var dtPlayerCategory = db.GetDataTable("SELECT * FROM bigdream.playercategory where matchcategoryid=1 order by serialno");
        var lstplayercategory = dtPlayerCategory.AsEnumerable().Select(a => new PlayerCategory
        {
            playercategoryId = a["playercategoryId"].ToString().intTP(),
            playercategory = a["playercategory1"].ToString().Trim(),
            playercategoryshortform = a["playercategoryshortform"].ToString().Trim()
        }).ToList();
        var lstPlayer1 = await GetPlayerLeftList("team1");
        var lstPlayer2 = await GetPlayerRightList("team2");
        GetWKAllRounder(lstPlayer1, lstplayercategory);
        GetWKAllRounder(lstPlayer2, lstplayercategory);
        lstPlayer1.Where(a => a.playercategoryid == null || a.playercategoryid == 0).ToList().ForEach(a =>
        {
            var array = a.playercategory.Split(new char[] { ' ', '-' });
            var catinfo = "";
            //if (array.Length > 1)
            //{
            //    if (array[1].Trim() != "")
            //        catinfo = array[1].ToLower();
            //    else
            //        catinfo = array[0].ToLower();
            //}
            //else
            if (array.Length > 0)
            {
                catinfo = array[0].ToLower();
            }

            var item = catinfo;
            a.matchid = matchid.ToString();
            a.originalmatchid = fristtemaid.ToString();
            a.playername = a.playername.Split(' ')[0] + ' ' + a.playername.Split(' ')[1];
            a.playercategoryid = lstplayercategory.FirstOrDefault(b => b.playercategory.ToLower().Split(",").Contains(item))?.playercategoryId ?? 0;
        });
        lstPlayer2.Where(a => a.playercategoryid == null || a.playercategoryid == 0).ToList().ForEach(a =>
        {
            var array = a.playercategory.Split(new char[] { ' ', '-' });
            var catinfo = "";
            //if (array.Length > 1)
            //{
            //    if (array[1].Trim() != "")
            //        catinfo = array[1].ToLower();
            //    else
            //        catinfo = array[0].ToLower();
            //}
            //else
            if (array.Length > 0)
            {
                catinfo = array[0].ToLower();
            }
            var item = catinfo; //a.playercategory.Split(new char[] { ' ', '-' })[0].ToLower();
            a.matchid = matchid.ToString();
            a.playername = a.playername.Split(' ')[0] + ' ' + a.playername.Split(' ')[1];
            a.originalmatchid = secondtemaid.ToString();
            a.playercategoryid = lstplayercategory.FirstOrDefault(b => b.playercategory.ToLower().Split(",").Contains(item))?.playercategoryId ?? 0;
        });
        lstPlayer1.ForEach(a =>
        {
            a.matchid = matchid.ToString();
            a.originalmatchid = fristtemaid.ToString();
        });
        lstPlayer2.ForEach(a =>
        {
            a.matchid = matchid.ToString();
            a.originalmatchid = secondtemaid.ToString();
        });
        lstPlayer1.AddRange(lstPlayer2);
        lstPlayer1 = lstPlayer1.FindAll(a => !(a.playercategoryid == null || a.playername == null || a.playername == "" || a.playercategoryid == 0));
        string query = "";
        var querypart = string.Join(",", lstPlayer1.Select(a =>
        {
            return "('" + a.playername + "','" + a.playercategoryid + "','" + a.matchid + "'," +
            "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + a.originalmatchid + "'," +
            "'" + a.point + "','" + a.credit + "','" + a.isannnouce + "','" + a.playerurl + "')";

        }).ToList());
        db.ExecQuery("insert into bigdream.matchteam (playername,playercategoryid,matchid," +
            "createdate,originalmatchid,point,credit,isannouce,playerimagesource) values " + querypart + "");
    }



    public static string GetStringBetweenParentheses(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        // Regular expression to match text within parentheses
        string pattern = @"\(([^)]*)\)";
        Match match = Regex.Match(input, pattern);

        // Check if a match is found
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        // Return empty string if no match is found
        return string.Empty;
    }

    public async Task<List<MatchPlayer>> GetPlayerLeftList(string filename)
    {
        var lstplayers = new List<MatchPlayer>();
        var htmlDoc = new HtmlDocument();
        var html1 = File.ReadAllText(@"D:\\CricScraper\\" + filename + ".txt");
        htmlDoc.LoadHtml(html1);
        var playerCards = htmlDoc.DocumentNode.SelectNodes("//div[@class='cb-col cb-col-100']//a");
        var removetextfromname = new string[] { "Batting", "Bowling", "Allrounder", "WK-Batter" };
        if (playerCards != null)
        {
            var wb = new WebClient();
            foreach (var playerCard in playerCards)
            {
                var profileLink = playerCard.GetAttributeValue("href", string.Empty);
                var imgNode = playerCard.SelectSingleNode(".//img[@class='cb-plyr-img-left']");
                var imgSrc = imgNode?.GetAttributeValue("src", string.Empty);
                var nameNode = playerCard.SelectSingleNode(".//div[@class='cb-player-name-left']/div");
                var name = nameNode?.InnerText.Trim().Split("<br>")[0].Trim();
                var roleNode = playerCard.SelectSingleNode(".//span[@class='cb-font-12 text-gray']");
                var role = roleNode?.InnerText.Trim();

                Console.WriteLine($"Name: {name}");
                Console.WriteLine($"Role: {role}");
                Console.WriteLine($"Profile Link: {profileLink}");
                Console.WriteLine($"Image Source: {imgSrc}");
                Console.WriteLine();
                var objmatch = new MatchPlayer();
                objmatch.playername = ReplaceSubstrings(RemoveTextWithinParentheses(name.Trim()), removetextfromname);
                objmatch.playercategory = role;
                objmatch.playerurl = imgSrc;
                objmatch.playerurl = await uploadimage(imgSrc);
                objmatch.isannnouce = 1;
                objmatch.credit = 9.5;
                objmatch.point = 7;
                lstplayers.Add(objmatch);
            }
        }
        else
        {
            Console.WriteLine("No player cards found.");
        }
        return lstplayers;
    }

    public async Task<List<MatchPlayer>> GetPlayerRightList(string filename)
    {
        var lstplayers = new List<MatchPlayer>();
        var htmlDoc = new HtmlDocument();
        var html1 = File.ReadAllText(@"D:\\CricScraper\\" + filename + ".txt");
        htmlDoc.LoadHtml(html1);
        //var playerCards = htmlDoc.DocumentNode.SelectNodes("//div[@class='cb-col cb-col-100']//a");
        var removetextfromname = new string[] { "Batting", "Bowling", "Allrounder", "WK-Batter" };
        var playerNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='cb-col cb-col-100']");
        if (playerNodes != null)
        {

            foreach (var playerNode in playerNodes)
            {

                var linkNode = playerNode.SelectSingleNode(".//a");
                //player.ProfileUrl = "https://www.cricbuzz.com" + linkNode.GetAttributeValue("href", string.Empty);

                var imageNode = playerNode.SelectSingleNode(".//img");
                // player.ImageUrl = imageNode.GetAttributeValue("src", string.Empty);

                var nameNode = playerNode.SelectSingleNode(".//div[@class='cb-player-name-right']/div");
                var nameText = nameNode.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.None);
                //player.Name = nameText[0].Trim();
                //player.Role = nameNode.SelectSingleNode(".//span")?.InnerText.Trim() ?? string.Empty;

                //players.Add(player);


                var profileLink = "https://www.cricbuzz.com" + linkNode.GetAttributeValue("href", string.Empty);
                //var imgNode = imageNode.GetAttributeValue("src", string.Empty);//playerCard.SelectSingleNode(".//img[@class='cb-plyr-img-left']");
                var imgSrc = imageNode?.GetAttributeValue("src", string.Empty);
                //var nameNode = playerCard.SelectSingleNode(".//div[@class='cb-player-name-left']/div");
                var name = nameText[0].Trim();//nameNode?.InnerText.Trim().Split("<br>")[0].Trim();
                //var roleNode = playerCard.SelectSingleNode(".//span[@class='cb-font-12 text-gray']");
                var role = nameNode.SelectSingleNode(".//span")?.InnerText.Trim() ?? string.Empty; //roleNode?.InnerText.Trim();

                Console.WriteLine($"Name: {name}");
                Console.WriteLine($"Role: {role}");
                Console.WriteLine($"Profile Link: {profileLink}");
                Console.WriteLine($"Image Source: {imgSrc}");
                Console.WriteLine();
                var objmatch = new MatchPlayer();
                objmatch.playername = ReplaceSubstrings(RemoveTextWithinParentheses(name.Trim()), removetextfromname);
                objmatch.playercategory = role;
                objmatch.playerurl = imgSrc;
                objmatch.playerurl = await uploadimage(imgSrc);
                objmatch.isannnouce = 1;
                objmatch.credit = 9.5;
                objmatch.point = 7;
                lstplayers.Add(objmatch);
            }
        }
        else
        {
            Console.WriteLine("No player cards found.");
        }
        return lstplayers;
    }


    public async Task<string> uploadimage(string imagesrc)
    {
        var final = "";
        try
        {
            //var url = "http://api.bestie11.com/api/Save/uploadplayerImageSrc"; // Replace with your URL
            //var formData = new MultipartFormDataContent();

            // Add string content
            //formData.Add(new StringContent("value1"), "key1");
            //formData.Add(new StringContent("value2"), "key2");
            var wb = new WebClient();
            if (!Directory.Exists("playerimages"))
            {
                Directory.CreateDirectory("playerimages");
            }
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagesrc)}";
            final = fileName;
            wb.DownloadFile(imagesrc, "playerimages/" + fileName);
            // Add file content
            //var fileStream = new FileStream("playerimages/" + fileName, FileMode.Open, FileAccess.Read);
            //var fileContent = new StreamContent(fileStream);
            //fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            //formData.Add(fileContent, "file", Path.GetFileName("playerimages/" + fileName));

            //var client = new RestClient(url);
            //var request = new RestRequest(url,Method.Post);
            //request.AddFile("file", "playerimages/" + fileName);
            //var response = await client.ExecuteAsync(request);

            //// Check the response code first
            //if (response.IsSuccessful)
            //{
            //    Console.WriteLine("Upload successful.");
            //    Console.WriteLine($"Response content: {response.Content}");
            //}
            //else
            //{
            //    Console.WriteLine("Upload failed.");
            //    Console.WriteLine($"Status code: {(int)response.StatusCode}");
            //    Console.WriteLine($"Response: {response.Content}");
            //}

            //using (var client = new HttpClient())
            //{
            //    try
            //    {
            //        //var response = await client.PostAsync(url, formData);
            //        HttpResponseMessage response = null;
            //        try
            //        {
            //            Console.WriteLine("Sending request...");
            //            response = await client.PostAsync(url, formData);
            //        }
            //        catch (TaskCanceledException ex)
            //        {
            //            Console.WriteLine("Request timed out.");
            //            Console.WriteLine($"Error: {ex.Message}");
            //            //return;
            //        }
            //        if (response.IsSuccessStatusCode)
            //        {
            //            Console.WriteLine("Upload successful.");
            //            var responseContent = await response.Content.ReadAsStringAsync();
            //            final = responseContent;
            //            try
            //            {
            //                File.Delete("playerimages/" + fileName);
            //            }
            //            catch (Exception ex)
            //            {

            //            }
            //        }
            //        else
            //        {
            //            Console.WriteLine("Upload failed.");
            //            Console.WriteLine($"Status code: {response.StatusCode}");
            //            var responseContent = await response.Content.ReadAsStringAsync();
            //            Console.WriteLine($"Response: {responseContent}");
            //            final = responseContent;
            //        }
            //    }
            //    catch (Exception ex)
            //    { 

            //    }
            //}
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            Console.WriteLine($"Inner exception: {e.InnerException?.Message}");
        }
        catch (IOException e)
        {
            Console.WriteLine($"File error: {e.Message}");
            Console.WriteLine($"Inner exception: {e.InnerException?.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unexpected error: {e.Message}");
            Console.WriteLine($"Inner exception: {e.InnerException?.Message}");
        }
        return final;
    }

    public async Task<string> ReadMatchData(string filename)
    {
        var matchid = 1;
        var matchcategoryid = 1;
        var matchleagueid = 1;
        var db = new DBRepository();
        var htmlDoc = new HtmlDocument();
        var html1 = File.ReadAllText(@"D:\\CricScraper\\" + filename + ".txt");
        htmlDoc.LoadHtml(html1);

        // Select all team elements
        var teamLinks = htmlDoc.DocumentNode.SelectNodes("//div[@class='cb-col cb-col-100 cb-teams-hdr text-bold pad10 ng-scope']//a");
        var fristtemaid = 0;
        var secondtemaid = 0;
        if (teamLinks != null)
        {
            int count = 0;
            var team1 = "";
            var team2 = "";
            var team1url = "";
            var team2url = "";
            foreach (var teamLink in teamLinks)
            {
                count++;

                var profileLink = teamLink.GetAttributeValue("href", string.Empty);
                var imgNode = teamLink.SelectSingleNode(".//img");
                var imgSrc = imgNode?.GetAttributeValue("src", string.Empty);
                var nameNode = teamLink.SelectSingleNode(".//div[contains(@class, 'pad5')][last()]");
                var name = nameNode?.InnerText.Trim();

                Console.WriteLine($"Team Name: {name}");
                Console.WriteLine($"Profile Link: {profileLink}");
                Console.WriteLine($"Image Source: {imgSrc}");
                Console.WriteLine();
                if (count == 1)
                {
                    team1 = name;
                    team1url = await uploadimage(imgSrc);
                }
                else if (count == 2)
                {
                    team2 = name;
                    team2url = await uploadimage(imgSrc);
                }
            }
            var uobj = new Utility();
            fristtemaid = uobj.SaveOriginalTeam(team1, team1url);
            secondtemaid = uobj.SaveOriginalTeam(team2, team2url);
            var dtmatch = db.GetDataTable("insert into bigdream.match (matchname,matchcategoryid," +
                "leagueid,createdate,eventdate,team1,team2) values ('" + team1.Trim() + " VS " + team2.Trim() + "'" +
                ",'" + matchcategoryid + "','" + matchleagueid + "'," +
                "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                "'" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "','" + fristtemaid + "','" + secondtemaid + "');select last_insert_id();");
            if (dtmatch.Rows.Count > 0)
            {
                matchid = dtmatch.Rows[0][0].ToString().intTP();
            }

        }
        else
        {
            Console.WriteLine("No team links found.");
        }
        dynamic user = new ExpandoObject();
        user.matchid = matchid;
        user.originalteam1id = fristtemaid;
        user.originalteam2id = secondtemaid;

        return JsonConvert.SerializeObject(user);
    }

    public List<PlayerPartInfo> GetPlayerPartInfo(HtmlDocument htmlDoc)
    {
        // XPath to find the rows where players got out by catching the ball
        //var xpath = "//div[contains(@class, 'cb-col-33')]/span[contains(text(), 'c ')]";
        var xpath = "//div[contains(@class, 'cb-col-33')]/span";

        var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
        if (nodes == null)
        {
            Console.WriteLine("No players found who got out by catching the ball.");
            return new List<PlayerPartInfo>();
        }

        var playersWhoGotOutByCatch = new List<string>();
        var playerpart = new List<PlayerPartInfo>();
        foreach (var node in nodes)
        {
            var text = node.InnerText;
            var spliter = new string[] { " c and b ", " lbw b ", "run out ", "c ", "st " };
            foreach (var item in spliter)
            {
                var playerNames = text.Split(new string[] { item, "b " },
                StringSplitOptions.RemoveEmptyEntries)
                                   .Select(name => name.Trim())
                                   .Where(name => !string.IsNullOrEmpty(name))
                                   .ToList();
                if ((playerNames.Any() && playerNames.Count > 1) || (playerNames.Any() && text.Contains("run out")))
                {
                    if (text.Contains(item.Trim()) && item.Trim() == "c and b")
                    {
                        var objpart = new PlayerPartInfo();
                        objpart.PlayerName = playerNames[1];
                        objpart.Catch = 1;
                        playerpart.Add(objpart);
                        break;
                    }
                    else if (text.Contains(item.Trim()) && item.Trim() == "lbw b")
                    {
                        var objpart = new PlayerPartInfo();
                        objpart.PlayerName = playerNames[1];
                        objpart.LBW = 1;
                        playerpart.Add(objpart);
                        break;
                    }
                    else if (text.Contains(item.Trim()) && item.Trim() == "run out")
                    {

                        var parentheses = GetStringBetweenParentheses(playerNames[0]);
                        if (parentheses.Contains("/"))
                        {
                            if (parentheses.Split('/').Length > 1)
                            {
                                var objpart = new PlayerPartInfo();
                                objpart.PlayerName = parentheses.Split('/')[0];
                                objpart.runout = 0.5;
                                playerpart.Add(objpart);
                                objpart = new PlayerPartInfo();
                                objpart.PlayerName = parentheses.Split('/')[1];
                                objpart.runout = 0.5;
                                playerpart.Add(objpart);
                                break;
                            }
                        }
                        else if (!parentheses.Contains("/"))
                        {
                            var objpart = new PlayerPartInfo();
                            objpart.PlayerName = playerNames[0];
                            objpart.runout = 1;
                            playerpart.Add(objpart);
                            break;
                        }
                    }
                    else if (text.Contains(item.Trim()) && item.Trim() == "c")
                    {
                        var objpart = new PlayerPartInfo();
                        objpart.PlayerName = playerNames[0].Split('b')[0];
                        objpart.Catch = 1;
                        playerpart.Add(objpart);
                        break;

                    }
                    else if (text.Contains(item.Trim()) && item.Trim() == "st")
                    {
                        var objpart = new PlayerPartInfo();
                        objpart.PlayerName = playerNames[0].Split('b')[0];
                        objpart.Stumping = 1;
                        playerpart.Add(objpart);
                        break;

                    }
                    //playersWhoGotOutByCatch.AddRange(playerNames);
                    //break;
                }
            }


        }
        return playerpart;
    }

    public List<PlayerPartInfo> GetPlayerPartInfo2(HtmlDocument htmlDoc)
    {
        // XPath to find the rows where players got out by catching the ball
        //var xpath = "//div[contains(@class, 'cb-col-33')]/span[contains(text(), 'c ')]";
        var xpath = "//div[contains(@class, 'cb-col-33')]/span";

        var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
        if (nodes == null)
        {
            Console.WriteLine("No players found who got out by catching the ball.");
            return new List<PlayerPartInfo>();
        }
        var playersWhoGotOutByCatch = new List<string>();
        var playerpart = new List<PlayerPartInfo>();
        foreach (var node in nodes)
        {
            var text = node.InnerText;

            if (text.Contains("c and b "))
            {
                var playerNames = text.Split(new string[] { "c and b " },
            StringSplitOptions.RemoveEmptyEntries)
                               .Select(name => name.Trim())
                               .Where(name => !string.IsNullOrEmpty(name))
                               .ToList();
                var objpart = new PlayerPartInfo();
                objpart.PlayerName = playerNames[0];
                objpart.Catch = 1;
                playerpart.Add(objpart);
            }
            else if (text.Contains("lbw b "))
            {
                var playerNames = text.Split(new string[] { "lbw b " },
            StringSplitOptions.RemoveEmptyEntries)
                               .Select(name => name.Trim())
                               .Where(name => !string.IsNullOrEmpty(name))
                               .ToList();
                var objpart = new PlayerPartInfo();
                objpart.PlayerName = playerNames[0];
                objpart.LBW = 1;
                playerpart.Add(objpart);
            }
            else if (text.Contains("run out "))
            {
                var playerNames = text.Split(new string[] { "run out " },
           StringSplitOptions.RemoveEmptyEntries)
                              .Select(name => name.Trim())
                              .Where(name => !string.IsNullOrEmpty(name))
                              .ToList();
                var parentheses = GetStringBetweenParentheses(playerNames[0]);
                if (parentheses.Contains("/"))
                {
                    if (parentheses.Split('/').Length > 1)
                    {
                        var objpart = new PlayerPartInfo();
                        objpart.PlayerName = parentheses.Split('/')[0];
                        objpart.runout = 0.5;
                        playerpart.Add(objpart);
                        objpart = new PlayerPartInfo();
                        objpart.PlayerName = parentheses.Split('/')[1];
                        objpart.runout = 0.5;
                        playerpart.Add(objpart);
                    }
                }
                else if (!parentheses.Contains("/"))
                {
                    var objpart = new PlayerPartInfo();
                    objpart.PlayerName = playerNames[0];
                    objpart.runout = 1;
                    playerpart.Add(objpart);
                }
            }
            else if (text.Contains("st "))
            {
                var playerNames = text.Split(new string[] { "st " },
            StringSplitOptions.RemoveEmptyEntries)
                               .Select(name => name.Trim())
                               .Where(name => !string.IsNullOrEmpty(name))
                               .ToList();
                var objpart = new PlayerPartInfo();
                objpart.PlayerName = playerNames[0];
                objpart.Stumping = 1;
                playerpart.Add(objpart);

            }
            else if (text.Contains("c ") && text.Contains("b "))
            {
                var playerNames = text.Split(new string[] { "c ", "b " },
           StringSplitOptions.RemoveEmptyEntries)
                              .Select(name => name.Trim())
                              .Where(name => !string.IsNullOrEmpty(name))
                              .ToList();
                var objpart = new PlayerPartInfo();
                objpart.PlayerName = playerNames[0];
                objpart.Catch = 1;
                playerpart.Add(objpart);
            }
            else if (text.Contains("b "))
            {
                var playerNames = text.Split(new string[] { "b " },
           StringSplitOptions.RemoveEmptyEntries)
                              .Select(name => name.Trim())
                              .Where(name => !string.IsNullOrEmpty(name))
                              .ToList();
                var objpart = new PlayerPartInfo();
                objpart.PlayerName = playerNames[0];
                objpart.Bowled = 1;
                playerpart.Add(objpart);
            }


            //playersWhoGotOutByCatch.AddRange(playerNames);
            //break;
        }
        return playerpart;
    }

    public Tuple<List<Dictionary<string, string>>, List<PlayerPartInfo>> ReadInning(string filename)
    {
        var db = new DBRepository();
        var htmlDoc = new HtmlDocument();
        var html1 = File.ReadAllText(@"D:\\CricScraper\\" + filename + ".txt");
        htmlDoc.LoadHtml(html1);

        var battingRows = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'cb-scrd-itms')]");
        var battingData = new List<Dictionary<string, string>>();


        foreach (var row in battingRows)
        {
            var batterNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-25')]");
            var runNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-8')][1]");
            var ballNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-8')][2]");
            var fourNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-8')][3]");
            var sixNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-8')][4]");
            var srNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-8')][5]");

            if (batterNode != null && runNode != null && ballNode != null)
            {
                var batter = RemoveTextWithinParentheses(batterNode.InnerText.Trim());
                var runs = runNode.InnerText.Trim();
                var balls = ballNode.InnerText.Trim();
                var fours = fourNode?.InnerText.Trim() ?? "0";
                var sixes = sixNode?.InnerText.Trim() ?? "0";
                var sr = srNode?.InnerText.Trim() ?? "0.00";

                battingData.Add(new Dictionary<string, string>
                    {
                        { "Batter", batter },
                        { "Runs", runs },
                        { "Balls", balls },
                        { "4s", fours },
                        { "6s", sixes },
                        { "SR", sr }
                    });
            }
        }

        //Console.WriteLine("Batting Data:");
        //foreach (var data in battingData)
        //{
        //    Console.WriteLine($"Batter: {data["Batter"]}, Runs: {data["Runs"]}, Balls: {data["Balls"]}, 4s: {data["4s"]}, 6s: {data["6s"]}, SR: {data["SR"]}");
        //}

        // Extract bowling data
        var bowlingRows = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'cb-scrd-itms') and not(contains(@class, 'cb-col-25'))]");
        var bowlingData = new List<Dictionary<string, string>>();

        foreach (var row in bowlingRows)
        {
            var bowlerNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-38')]");
            var oversNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-8')][1]");
            var maidensNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-8')][2]");
            var runsNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-10')][1]");
            var wicketsNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-8')][3]");
            var nbNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-8')][4]");
            var wdNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-8')][5]");
            var econNode = row.SelectSingleNode(".//div[contains(@class, 'cb-col-10')][2]");

            if (bowlerNode != null && oversNode != null && runsNode != null)
            {
                var bowler = RemoveTextWithinParentheses(bowlerNode.InnerText.Trim());
                var overs = oversNode.InnerText.Trim();
                var maidens = maidensNode?.InnerText.Trim() ?? "0";
                var runs = runsNode.InnerText.Trim();
                var wickets = wicketsNode?.InnerText.Trim() ?? "0";
                var nb = nbNode?.InnerText.Trim() ?? "0";
                var wd = wdNode?.InnerText.Trim() ?? "0";
                var econ = econNode?.InnerText.Trim() ?? "0.00";

                bowlingData.Add(new Dictionary<string, string>
                    {
                        { "Bowler", bowler },
                        { "Overs", overs },
                        { "Maidens", maidens },
                        { "Runs", runs },
                        { "Wickets", wickets },
                        { "NB", nb },
                        { "WD", wd },
                        { "Econ", econ }
                    });
            }
        }
        bowlingData.AddRange(battingData);

        //Console.WriteLine("\nBowling Data:");
        //foreach (var data in bowlingData)
        //{
        //    Console.WriteLine($"Bowler: {data["Bowler"]}, Overs: {data["Overs"]}, Maidens: {data["Maidens"]}, Runs: {data["Runs"]}, Wickets: {data["Wickets"]}, NB: {data["NB"]}, WD: {data["WD"]}, Econ: {data["Econ"]}");
        //}
        var playerpartinfo = GetPlayerPartInfo2(htmlDoc);
        return Tuple.Create(bowlingData, playerpartinfo);
    }
}

public class Utility
{
    public int SaveOriginalTeam(string team, string url)
    {
        var db = new DBRepository();
        int teamid = 0;
        var dtteam = db.GetDataTable("select * from bigdream.originalteam where teamname='" + team + "'");
        if (dtteam.Rows.Count > 0)
            teamid = dtteam.Rows[0]["originalteamid"].ToString().intTP();
        else
        {
            teamid = db.GetDataTable("insert into bigdream.originalteam (iconurl,teamname,teamshortname,createdate)" +
                " values ('" + url + "','" + team + "','" + (team.Count() >= 3 ? team.Substring(0, 3).ToUpper() : team) + "'," +
                "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'); select LAST_INSERT_ID(); ").Rows[0][0].ToString().intTP();

        }
        return teamid;
    }
}
public static class Extension
{
    public static string PlayerCategory(this string category)
    {
        var playercate = "";
        if (category.ToLower().Contains("allrounder"))
        {
            playercate = "AR";
        }
        else if (category.ToLower().Contains("wicketkeeper"))
        {
            playercate = "WK";
        }
        else if (category.ToLower().Contains("batter"))
        {
            playercate = "BT";
        }
        else if (category.ToLower().Contains("bowler"))
        {
            playercate = "BOWL";
        }
        return playercate;
    }
}