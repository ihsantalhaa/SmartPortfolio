using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartPortfolio.Data;
using SmartPortfolio.Models;
using SmartPortfolio.ViewModels;
using System;
using System.Data;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SmartPortfolio.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IRole> _roleManager;
        private readonly UserManager<IUser> _userManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardController(RoleManager<IRole> roleManager, UserManager<IUser> userManager, ApplicationDbContext db, SignInManager<IUser> signInManager, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                IUser? user = await _db.Users.FindAsync(userId.ToString());
                if (user != null)
                {
                    ViewBag.Portfolio = user.UserName;
                }
                return View();
            }
            return BadRequest("Portfolio Not Found!");
        }

        public async Task<IActionResult> PortfolioIndex(int? id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userPortfolios = from p in _db.Portfolios
                                 where p.UserId == userId
                                 select p;
            List<Portfolio> portfolios = await userPortfolios.ToListAsync();
            List<PortfolioDetailsViewModel> modelList = new List<PortfolioDetailsViewModel>();
            foreach (var i in portfolios)
            {
                PortfolioDetailsViewModel item = new PortfolioDetailsViewModel
                {
                    Id = i.PortfolioId,
                    PorfolioName = i.PortfolioName!,
                    PorfolioDescription = i.PortfolioDescription,
                };
                modelList.Add(item);
            }
            return View(modelList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePortfolio(int? id)
        {
            if (id != null)
            {
                Portfolio? portfolio = await _db.Portfolios.FindAsync(id);
                if (portfolio != null)
                {
                    _db.Portfolios.Remove(portfolio);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        return RedirectToAction("PortfolioIndex", "Dashboard");
                    }
                    return BadRequest("Error While Deleting Group!");
                }
            }
            return BadRequest("Portfolio Not Found!");
        }

        [HttpPost]
        public async Task<IActionResult> EditPortfolio(PortfolioDetailsViewModel? model)
        {
            if (model?.PorfolioName != null)
            {
                Portfolio? findedPortfolio = await _db.Portfolios.FindAsync(model.Id);
                if (findedPortfolio != null)
                {
                    findedPortfolio.PortfolioName = model.PorfolioName;
                    findedPortfolio.PortfolioDescription = model.PorfolioDescription;
                    _db.Update(findedPortfolio);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        return RedirectToAction("PortfolioIndex");
                    }
                    return BadRequest("Error While Updating Portfolio!");
                }
                return BadRequest("Portfolio Not Found!");
            }
            return BadRequest("Portfolio Name Mustn't Null!");
        }

        public async Task<IActionResult> EditPortfolioView(int? id)
        {
            if (id != null)
            {
                Portfolio? portfolio= await _db.Portfolios.FindAsync(id);
                if (portfolio != null)
                {
                    PortfolioDetailsViewModel portfolioDetails = new PortfolioDetailsViewModel
                    {
                        Id = portfolio.PortfolioId,
                        PorfolioDescription = portfolio.PortfolioDescription,
                        PorfolioName = portfolio.PortfolioName
                    };
                    return View(portfolioDetails);
                }
            }
            return BadRequest("Portfolio Not Found!");
        }

        [HttpPost]
        public async Task<IActionResult> AddPortfolio(PortfolioDetailsViewModel? model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (model?.PorfolioName != null && userId != null)
            {
                IUser? user = await _userManager.FindByIdAsync(userId!);
                if (user != null) {
                    Portfolio portfolio = new Portfolio
                    {
                        User = user,
                        PortfolioName = model.PorfolioName,
                        PortfolioDescription = model.PorfolioDescription
                    };
                    await _db.AddAsync(portfolio);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        return RedirectToAction("PortfolioIndex");
                    }
                    return BadRequest("Error While Adding Portfolio!");
                }
            }
            return BadRequest("Portfolio Name Mustn't Null!");
        }

        public IActionResult AddPortfolioView()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSignedUser(UpdateSignedUserViewModel? model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (model?.Username != null && model?.Email != null && model?.PhoneNumber != null && userId != null)
            {
                IUser? findedUser = await _userManager.FindByNameAsync(model!.Username!);
                IUser? emailFindedUser = await _userManager.FindByEmailAsync(model!.Email!);
                IUser? user = await _userManager.FindByIdAsync(userId);
                if ((findedUser == user && emailFindedUser == user && user != null) || (findedUser == null && emailFindedUser == null && user != null))
                {
                    user.UserName = model.Username!.Replace(" ", "");
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        if(model.Password != null && model.ConfirmPassword != null && model.Password == model.ConfirmPassword)
                        {
                            var result2 = await _userManager.ChangePasswordAsync(user, model.Password!, model.ConfirmPassword!);
                            if (result2.Succeeded)
                            {
                                return RedirectToAction("LogOut", "Home");
                            }
                            TempData["Message"] = "Passwords Not Same !!!";
                        }
                        TempData["Message"] = "User Updated But Password Not Changed !!!";
                    }
                    return RedirectToAction("PortfolioIndex", "Dashboard");
                }
                TempData["Message"] = "User Available !!!";
            }
            return RedirectToAction("UpdateSignedUserView", model);
        }

        public async Task<IActionResult> UpdateSignedUserView(UpdateSignedUserViewModel? data)
        {
            if(!ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    IUser? user = await _userManager.FindByIdAsync(userId!);
                    if (user != null)
                    {
                        UpdateSignedUserViewModel model = new UpdateSignedUserViewModel
                        {
                            Username = user.UserName,
                            Email = user.Email!,
                            PhoneNumber = user.PhoneNumber,
                        };
                        return View(model);
                    }
                }
            }
            return View(data);
        }

        public async Task<IActionResult> ListAssetsView(int? id)
        {
            Portfolio? portfolio = await _db.Portfolios.FindAsync(id);
            if (portfolio != null)
            {
                var portfolioAssets = from p in _db.Assets
                                      where p.PortfolioId == portfolio.PortfolioId
                                      select p;
                List<Asset> values = await portfolioAssets.ToListAsync();

                List<ListAssetsViewModel> modelList = [];
                foreach (var item in values)
                {
                    string state;
                    double stateValue = 0.00;
                    double stateAmountValue = 0.00;
                    double stateRatio = 0.00;
                    var cost = item.Cost;
                    var lastPrice = item.LastPrice;
                    var amount = item.Amount;

                    if (lastPrice <= cost)
                    {
                        state = "Loss";
                        stateAmountValue = cost - lastPrice;
                    }
                    else
                    {
                        state = "Profit";
                        stateAmountValue = lastPrice - cost;
                    }

                    stateAmountValue = cost - lastPrice;
                    stateRatio = (stateAmountValue / cost) * 100;
                    stateValue = stateAmountValue * amount;

                    ListAssetsViewModel model = new ListAssetsViewModel
                    {
                        AssetId = item.AssetId,
                        UpdateDate = item.UpdateDate,
                        LastPrice = item.LastPrice.ToString(),
                        SymbolName = item.SymbolName,
                        SymbolType = item.SymbolType,
                        Amount = item.Amount,
                        Cost = item.Cost,
                        State = state,
                        StateValue = stateValue,
                        StateAmountValue = stateAmountValue,
                        StateRatio = stateRatio
                    };
                    modelList.Add(model);
                }
                ViewBag.Message = portfolio.PortfolioName;
                TempData["PortfolioId"] = id;
                return View(modelList);
            }
            return BadRequest("Asset Not Found!!!");
        }

        public async Task<IActionResult> AddAssetView(int? id)
        {
            if(id != null)
            {
                TempData["PortfolioId"] = id;
                List<AddAssetViewModel> modelList = [];
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("http://127.0.0.1:5000/api/symbols");

                if (response.IsSuccessStatusCode)
                {

                    var apidata = await response.Content.ReadAsStringAsync();
                    //List<dynamic> items = JsonConvert.DeserializeObject<List<dynamic>>(apidata);
                    List<List<dynamic>> items2 = JsonConvert.DeserializeObject<List<List<dynamic>>>(apidata);
                    for(int iter = 0; iter < items2.Count; iter++) {

                        var details = items2[iter][1];

                        AddAssetViewModel model = new AddAssetViewModel
                        {
                            PortfolioId = id.Value,
                            SymbolName = items2[iter][0],
                            ClosedPrice = DParser(details["Kapanis"].ToString()),
                            FK = double.Parse(details["F/K"].ToString()),
                            FdFavok = double.Parse(details["FD/FAVOK"].ToString()),
                            FdSell = double.Parse(details["FD/Satislar"].ToString()),
                            PdDd = double.Parse(details["PD/DD"].ToString()),
                            LastPrice = DParser(items2[iter][2].ToString())//items2[iter][2].ToString(),
                        };
                        modelList.Add(model);

                    }
                }
                return View(modelList);
            }
            return BadRequest("Api Error");
        }
        public static double DParser(string value)
        {
            int dotCount = value.Split('.').Length;
            if (dotCount == 1)
            {
                return Convert.ToDouble(value);
            }
            if (dotCount > 1)
            {
                int sonNoktaIndex = value.LastIndexOf('.');
                if (sonNoktaIndex != -1)
                {
                    value = value.Substring(0, sonNoktaIndex).Replace(".", "") + value.Substring(sonNoktaIndex);
                }
            }
            return Convert.ToDouble(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAsset(AddAssetViewModel? model)
        {
            if (model != null)
            {
                Asset asset = new Asset
                {
                    PortfolioId = model.PortfolioId,
                    LastPrice = model.LastPrice,
                    SymbolName = model.SymbolName,
                };
                await _db.Assets.AddAsync(asset);
                var result = await _db.SaveChangesAsync();
                if (result > 0)
                {
                    return RedirectToAction("ListAssetsView", "Dashboard", new { id = model.PortfolioId} );
                }
                return BadRequest("Error While Adding Asset!");
            }
            return BadRequest("Asset Mustn't Null!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsset(int? AssetId, int? PortfolioId)
        { 
            if(AssetId != null && PortfolioId != null)
            {
                Asset? asset = await _db.Assets.FindAsync(AssetId);
                if(asset != null)
                {
                    _db.Assets.Remove(asset);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        return RedirectToAction("ListAssetsView", "Dashboard", new { id = PortfolioId });
                    }
                }
            }
            return BadRequest("Asset Not Deleted!");
        }

        public async Task<IActionResult> AddOrderView(int? AssetId)
        {
            if(AssetId != null)
            {
                Asset? asset = await _db.Assets.FindAsync(AssetId);
                if(asset?.SymbolName != null)
                {
                    ViewData["AssetId"] = AssetId;
                    ViewData["SymbolName"] = asset.SymbolName;
                    ViewData["LastPrice"] = asset.LastPrice;
                    ViewData["Amount"] = asset.Amount;
                    ViewData["Cost"] = asset.Cost;
                }
                return View();
            }
            return BadRequest("Asset Not Found!");
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(Order? model, string actionType)
        {
            if (model?.Amount != null && model?.AssetId != null)
            {
                Asset? asset = await _db.Assets.FindAsync(model.AssetId);
                if (actionType == "Buy" && asset != null)
                {
                    double cost;
                    int amount = asset.Amount + model.Amount;
                    if (asset.Cost != 0)
                    {
                        double newCostSumAsset = asset.Amount * asset.Cost;
                        double newCostSumModel = model.Amount * model.Price;
                        double newCostSum = newCostSumAsset + newCostSumModel;
                        cost = newCostSum / amount;
                    }
                    else
                    {
                        cost = model.Price;
                    }
                    
                    Order order = new Order
                    {
                        AssetId = model.AssetId,
                        OrderType = "Market Buy",
                        Date = DateTime.Now,
                        Amount = model.Amount,
                        Price = model.Price
                    };
                    asset.Cost = cost;
                    asset.Amount = amount;
                    _db.Orders.Add(order);
                    _db.Assets.Update(asset);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        return RedirectToAction("AddOrderView", "Dashboard", new { AssetId = asset.AssetId });
                    }

                }
                else if (actionType == "Sell" && asset != null)
                {
                    int amount = asset.Amount - model.Amount;
                    if (amount < 0) {
                        return BadRequest("Not Have Enough Amount in Asset!");
                    }
                    Order order = new Order
                    {
                        AssetId = model.AssetId,
                        OrderType = "Market Sell",
                        Date = DateTime.Now,
                        Amount = model.Amount,
                        Price = model.Price
                    };
                    asset.Amount = amount;
                    _db.Orders.Add(order);
                    _db.Assets.Update(asset);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        return RedirectToAction("AddOrderView", "Dashboard", new { AssetId = model.AssetId });
                    }
                }
                return BadRequest("Error While Adding Asset!");
            }
            return BadRequest("Order Mustn't Null!");
        }

        public async Task<IActionResult> ListOrdersView(int? id)
        {
            if (id != null)
            {
                Portfolio? portfolio = await _db.Portfolios.FindAsync(id);
                List<PortfolioOrdersViewModel> modelList = [];
                var query = _db.Assets
                    .Where(k => k.PortfolioId == id)  // AssetId'ye göre filtreleme
                    .SelectMany(k => k.Orders!, (asset, order) => new PortfolioOrdersViewModel
                    {
                        AssetId = asset.AssetId,
                        SymbolName = asset.SymbolName,
                        Amount = order.Amount,
                        Price = order.Price,
                        Date = order.Date,
                        OrderType = order.OrderType
                    }).OrderByDescending(o => o.Date);
                modelList = query.ToList();

                return View(modelList);
            }
            return BadRequest("Portfolio Id Not Found!");
        }

        public async Task<IActionResult> AssetDetailsView(int? AssetId, int? PortfolioId)
        {
            if (AssetId != null && PortfolioId != null)
            {
                Asset? asset = await _db.Assets.FindAsync(AssetId);
                var query = _db.Orders
                    .Where(k => k.AssetId == AssetId)
                    .Select(k=> k)
                    .OrderByDescending(o => o.Date);

                //var portfOrders = from p in _db.Orders
                //                  where p.AssetId == AssetId
                //                  select p;
                List<Order> modelList = await query.ToListAsync();
                TempData["PortfolioId"] = PortfolioId;
                TempData["AssetName"] = asset?.SymbolName;
               
                return View(modelList);
            }
            return BadRequest("Asset Id Not Found!");
        }

        public async Task<IActionResult> AnalyzeAsset(int? id)
        {
            if(id != null)
            {
                Asset? asset = await _db.Assets.FindAsync(id);
                if(asset != null)
                {
                    TempData["AssetId"] = id;
                    List<AnalyzeAssetViewModel> modelList = [];
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync($"http://127.0.0.1:5000/api/stock/{asset.SymbolName}");
                    var response2 = await client.GetAsync($"http://127.0.0.1:5000/api/analysis/{asset.SymbolName}");
                    var response3 = await client.GetAsync($"http://127.0.0.1:5000/api/advanced-analysis/{asset.SymbolName}");
                    if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode && response3.IsSuccessStatusCode)
                    {

                        string apidata = await response.Content.ReadAsStringAsync();
                        var items = JsonConvert.DeserializeObject<List<dynamic>>(apidata);

                        string apidata2 = await response2.Content.ReadAsStringAsync();
                        var items2 = JsonConvert.DeserializeObject<List<dynamic>>(apidata2);

                        string apidata3 = await response3.Content.ReadAsStringAsync();
                        var items3 = JsonConvert.DeserializeObject<List<dynamic>>(apidata3)?.ToList();
                        

                        var emas = items2[0];
                        var jsonEma = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(emas);
                        var ema20 = JsonConvert.SerializeObject(jsonEma["EMA20"]);
                        var ema50 = JsonConvert.SerializeObject(jsonEma["EMA50"]);
                        var sPlusEma = JsonConvert.SerializeObject(jsonEma["SIGNAL+"]);
                        var sMinusEma = JsonConvert.SerializeObject(jsonEma["SIGNAL-"]);

                        ViewBag.Ema20 = ema20;
                        ViewBag.Ema50 = ema50;
                        ViewBag.SPlusEma = sPlusEma;
                        ViewBag.SMinusEma = sMinusEma;

                        var smas = items2[1];
                        var jsonSma = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(smas);
                        var sma = JsonConvert.SerializeObject(jsonSma["SMA"]);
                        var sd = JsonConvert.SerializeObject(jsonSma["SD"]);
                        var ub = JsonConvert.SerializeObject(jsonSma["UB"]);
                        var lb = JsonConvert.SerializeObject(jsonSma["LB"]);
                        var sPlusSma = JsonConvert.SerializeObject(jsonSma["SIGNAL+"]);
                        var sMinusSma = JsonConvert.SerializeObject(jsonSma["SIGNAL-"]);

                        ViewBag.Sma = sma;
                        ViewBag.Sd = sd;
                        ViewBag.Ub = ub;
                        ViewBag.Lb = lb;
                        ViewBag.SPlusSma = sPlusSma;
                        ViewBag.SMinusSma = sMinusSma;

                        var rsiValues = items2[2];
                        var jsonRsi = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(rsiValues);
                        var rsi = JsonConvert.SerializeObject(jsonRsi["RSI"]);
                        var sPlusRsi = JsonConvert.SerializeObject(jsonRsi["SIGNAL+"]);
                        var sMinusRsi = JsonConvert.SerializeObject(jsonRsi["SIGNAL-"]);

                        ViewBag.Rsi = rsi;
                        ViewBag.SPlusRsi = sPlusRsi;
                        ViewBag.SMinusRsi = sMinusRsi;

                        var macdValues = items2[3];
                        var jsonMacd = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(macdValues);
                        var macd = JsonConvert.SerializeObject(jsonMacd["MACD"]);
                        var sLine = JsonConvert.SerializeObject(jsonMacd["SIGNAL_LINE"]);
                        var sPlusMacd = JsonConvert.SerializeObject(jsonMacd["SIGNAL+"]);
                        var sMinusMacd = JsonConvert.SerializeObject(jsonMacd["SIGNAL-"]);

                        ViewBag.Macd = macd;
                        ViewBag.SLine = sLine;
                        ViewBag.SPlusMacd = sPlusMacd;
                        ViewBag.SMinusMacd = sMinusMacd;

                        var perValues = items2[4];
                        var jsonPer = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(perValues);
                        var perD = JsonConvert.SerializeObject(jsonPer["%D"]);
                        var perK = JsonConvert.SerializeObject(jsonPer["%K"]);

                        ViewBag.PerD = perD;
                        ViewBag.PerK = perK;

                        var superTrendValues = items2[5];
                        var jsonST = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(superTrendValues);
                        var st = JsonConvert.SerializeObject(jsonST["ST"]);
                        var stDirect = JsonConvert.SerializeObject(jsonST["SUPERTrendDirection"]);
                        var stLong = JsonConvert.SerializeObject(jsonST["SUPERTrendLong"]);
                        var stShort = JsonConvert.SerializeObject(jsonST["SUPERTrendShort"]);
                        var sPlusSt = JsonConvert.SerializeObject(jsonST["SIGNAL+"]);
                        var sMinusSt = JsonConvert.SerializeObject(jsonST["SIGNAL-"]);

                        ViewBag.St = st;
                        ViewBag.StDirect = stDirect;
                        ViewBag.StLong = stLong;
                        ViewBag.StShort = stShort;
                        ViewBag.SPlusSt = sPlusSt;
                        ViewBag.SMinusSt = sMinusSt;

                        var adxValues = items2[6];
                        var jsonAdx = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(adxValues);
                        var adx = JsonConvert.SerializeObject(jsonAdx["ADX"]);
                        var plusAdx = JsonConvert.SerializeObject(jsonAdx["PLUS_DI"]);
                        var minusAdx = JsonConvert.SerializeObject(jsonAdx["MINUS_DI"]);
                        var sPlusAdx = JsonConvert.SerializeObject(jsonAdx["SIGNAL+"]);
                        var sMinusAdx = JsonConvert.SerializeObject(jsonAdx["SIGNAL-"]);

                        ViewBag.Adx = adx;
                        ViewBag.PlusAdx = plusAdx;
                        ViewBag.MinusAdx = minusAdx;
                        ViewBag.SPlusAdx = sPlusAdx;
                        ViewBag.SMinusAdx = sMinusAdx;

                        for (int iter = 0; iter < items.Count; iter++)
                        {
                            var details = items[3];
                             var details2 = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(details);

                            var symbolName = items[0].ToString();
                            var symbolType = items[1].ToString();

                            var graphic = items[4];//["DATE"];

                            var jsonDatas = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(graphic);
                            string jsonDate = JsonConvert.SerializeObject(jsonDatas["DATE"]);
                            string jsonClosingTl = JsonConvert.SerializeObject(jsonDatas["CLOSING_TL"]);
                            string jsonLowTl = JsonConvert.SerializeObject(jsonDatas["LOW_TL"]);
                            string jsonHighTl = JsonConvert.SerializeObject(jsonDatas["HIGH_TL"]);
                            string jsonVolumeTl = JsonConvert.SerializeObject(jsonDatas["VOLUME_TL"]);
                            string jsonClosingUsd = JsonConvert.SerializeObject(jsonDatas["CLOSING_USD"]);
                            string jsonlowUsd = JsonConvert.SerializeObject(jsonDatas["LOW_USD"]);
                            string jsonHighUsd = JsonConvert.SerializeObject(jsonDatas["HIGH_USD"]);
                            string jsonVolumeUsd = JsonConvert.SerializeObject(jsonDatas["VOLUME_USD"]);
                            string jsonXu100Tl = JsonConvert.SerializeObject(jsonDatas["XU100_TL"]);
                            string jsonXu100Usd = JsonConvert.SerializeObject(jsonDatas["XU100_USD"]);

                            ViewBag.AssetId = asset.AssetId;
                            ViewBag.SymbolName = symbolName;
                            ViewBag.SymbolType = symbolType;

                            ViewBag.ClosedPrice = details2["Kapanis"];
                            ViewBag.FK = details2["F/K"];
                            ViewBag.FdFavok = details2["FD/FAVOK"];
                            ViewBag.FdSell = details2["FD/Satislar"];
                            ViewBag.PdDd = details2["PD/DD"];
                            ViewBag.LastPrice = details2["Kapanis"];

                            ViewBag.Date = jsonDate;
                            ViewBag.ClosingTl = jsonClosingTl;
                            ViewBag.ClosingUsd = jsonClosingUsd;
                            ViewBag.HighTl = jsonHighTl;
                            ViewBag.HighUsd = jsonHighUsd;
                            ViewBag.LowTl = jsonLowTl;
                            ViewBag.LowUsd = jsonlowUsd;
                            ViewBag.VolumeTl = jsonVolumeTl;
                            ViewBag.VolumeUsd = jsonVolumeUsd;
                            ViewBag.Xu100Tl = jsonXu100Tl;
                            ViewBag.Xu100Usd = jsonXu100Usd;
                            ViewBag.Fuzzy = items3[0];
                            ViewBag.Linear = items3[1];
                        }
                    }

                    return View();
                }
            }
            return BadRequest("Asset Error!");
        }


    }
}