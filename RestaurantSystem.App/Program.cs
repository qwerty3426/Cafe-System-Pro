using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using RestaurantSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Staff waiter = new Staff("Микола", "Старший офіціант");
RestaurantOrderProcessor orderProcessor = new RestaurantOrderProcessor();

List<MenuItem> repositoryMenu = new List<MenuItem>
{
    ProductFactory.CreateProduct("Гарячі страви", "Борщ український", 120.00m),
    ProductFactory.CreateProduct("Гарячі страви", "Піца Чотири Сири", 210.00m),
    ProductFactory.CreateProduct("Гарячі страви", "Суп Том-Ям", 160.00m),
    ProductFactory.CreateProduct("Гарячі страви", "Паста Карбонара", 185.00m),
    ProductFactory.CreateProduct("Гарячі страви", "Стейк курячий з грилем", 230.00m),

    ProductFactory.CreateProduct("Холодні напої", "Кока-Кола в склі", 50.00m, 0.5),
    ProductFactory.CreateProduct("Холодні напої", "Спрайт", 50.00m, 0.5),
    ProductFactory.CreateProduct("Холодні напої", "Фанта", 50.00m, 0.5),
    ProductFactory.CreateProduct("Холодні напої", "Сік апельсиновий", 60.00m, 0.4),

    ProductFactory.CreateProduct("Кава", "Еспресо", 40.00m, 0.06),
    ProductFactory.CreateProduct("Кава", "Капучино", 55.00m, 0.3),
    ProductFactory.CreateProduct("Кава", "Лате Макіато", 65.00m, 0.35)
};

Order currentDraftOrder = new Order(waiter);
List<string> systemLogs = new List<string>();

var savedState = StorageService.LoadState();
if (savedState != null)
{
    FinancialMonitor.Instance.RegisterPayment(savedState.TotalRevenue);
    systemLogs = savedState.SystemLogs;

    foreach (var dtoItem in savedState.CurrentDraftItems)
    {
        currentDraftOrder += new MenuItem(dtoItem.Name, dtoItem.Price) { Category = "Відновлено" };
    }

    systemLogs.Add("[ПОРТАТИВНІСТЬ]: Стан системи успішно десеріалізовано з файлу JSON.");
}
else
{
    systemLogs.Add("Система закладу успішно ініціалізована. Створено нову базу даних файлу.");
}

void AutoSave()
{
    StorageService.SaveState(FinancialMonitor.Instance.TotalRevenue, systemLogs, currentDraftOrder);
}

orderProcessor.OrderReady += (sender, readyOrder) =>
{
    systemLogs.Add($"[ОПОВІЩЕННЯ ДЛЯ {waiter.Name.ToUpper()}]: 🔔 Замовлення №{readyOrder.OrderNumber} повністю ГОТОВЕ! Заберіть страву на роздачі.");
    AutoSave();
};

string GetPageLayout(string title, string activeTab, string bodyContent)
{
    string navMain = activeTab == "main" ? "active" : "";
    string navMenu = activeTab == "menu" ? "active" : "";
    string navOrders = activeTab == "orders" ? "active" : "";

    return $@"
    <!DOCTYPE html>
    <html lang='uk'>
    <head>
        <meta charset='UTF-8'>
        <title>{title}</title>
        <style>
            :root {{ --primary: #2c3e50; --accent: #16a085; --bg: #f8f9fa; --card-bg: #ffffff; }}
            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 0; background-color: var(--bg); color: #333; }}
            .navbar {{ background-color: var(--primary); padding: 15px 0; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
            .nav-container {{ max-width: 1100px; margin: 0 auto; display: flex; justify-content: space-between; align-items: center; padding: 0 20px; }}
            .brand {{ color: white; font-size: 22px; font-weight: bold; text-decoration: none; }}
            .nav-links {{ display: flex; gap: 20px; }}
            .nav-links a {{ color: #bdc3c7; text-decoration: none; font-weight: 500; padding: 8px 16px; border-radius: 6px; transition: all 0.2s; }}
            .nav-links a:hover, .nav-links a.active {{ color: white; background-color: rgba(255,255,255,0.1); }}
            .nav-links a.active {{ border-bottom: 3px solid var(--accent); border-radius: 6px 6px 0 0; }}
            .content-wrapper {{ max-width: 1100px; margin: 40px auto; padding: 0 20px; }}
            .grid {{ display: grid; grid-template-columns: 2fr 1fr; gap: 30px; }}
            .full-width {{ grid-column: 1 / -1; }}
            .card {{ background: var(--card-bg); border-radius: 12px; padding: 25px; box-shadow: 0 4px 16px rgba(0,0,0,0.04); margin-bottom: 25px; border: 1px solid #eaeaea; }}
            h2 {{ color: var(--primary); margin-top: 0; font-size: 20px; border-bottom: 2px solid #f1f2f6; padding-bottom: 10px; }}
            .balance-box {{ font-size: 36px; font-weight: bold; color: #27ae60; margin: 15px 0; }}
            .badge {{ background: #e8f8f5; color: var(--accent); padding: 4px 10px; border-radius: 5px; font-size: 12px; font-weight: bold; }}
            .menu-section {{ margin-bottom: 30px; }}
            .menu-category-title {{ font-size: 18px; color: var(--accent); font-weight: bold; margin-bottom: 15px; padding-left: 5px; border-left: 4px solid var(--accent); }}
            .menu-grid {{ display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 20px; }}
            .menu-item-card {{ background: #fff; border: 1px solid #e1e8ed; border-radius: 10px; padding: 15px; display: flex; justify-content: space-between; align-items: center; transition: transform 0.2s; }}
            .menu-item-card:hover {{ transform: translateY(-3px); box-shadow: 0 6px 12px rgba(0,0,0,0.05); }}
            .btn {{ background-color: #3498db; color: white; padding: 10px 16px; border: none; border-radius: 6px; cursor: pointer; text-decoration: none; font-weight: 600; display: inline-block; font-size: 14px; }}
            .btn:hover {{ background-color: #2980b9; }}
            .btn-sm {{ padding: 6px 12px; font-size: 12px; }}
            .btn-success {{ background-color: #2ecc71; }}
            .btn-success:hover {{ background-color: #27ae60; }}
            .btn-warning {{ background-color: #e67e22; }}
            .btn-warning:hover {{ background-color: #d35400; }}
            .btn-danger {{ background-color: #e74c3c; }}
            .btn-danger:hover {{ background-color: #c0392b; }}
            .log-line {{ padding: 8px; border-bottom: 1px solid #34495e; font-family: 'Courier New', monospace; font-size: 13px; color: #ecf0f1; }}
            .log-line:last-child {{ border-bottom: none; }}
        </style>
    </head>
    <body>
        <div class='navbar'>
            <div class='nav-container'>
                <a href='/' class='brand'>🍔 Кафе-Система Pro</a>
                <div class='nav-links'>
                    <a href='/' class='{navMain}'>🏠 Головна сторінка</a>
                    <a href='/menu' class='{navMenu}'>📋 Розділи Меню</a>
                    <a href='/orders' class='{navOrders}'>🍳 Черга та Логи</a>
                </div>
            </div>
        </div>
        <div class='content-wrapper'>
            {bodyContent}
        </div>
    </body>
    </html>";
}

app.MapGet("/", () =>
{
    string draftItemsHtml = currentDraftOrder.Items.Count == 0
        ? "<p style='color:#7f8c8d;'>Кошик порожній. Перейдіть у вкладку 'Розділи Меню', щоб додати страви.</p>"
        : string.Join("", currentDraftOrder.Items.Select(i => $"<div style='display:flex; justify-content:space-between; margin-bottom:8px;'><span>• {i.Name}</span><strong>{i.GetPrice():F2} грн</strong></div>"));

    var body = $@"
    <div class='grid'>
        <div>
            <div class='card'>
                <h2>👋 Панель Управління (Збереження JSON активоване)</h2>
                <p>Поточна робоча зміна обслуговується: <strong>{waiter.Name}</strong></p>
                <p>Посада в системі ресторану: <span class='badge'>{waiter.Role}</span></p>
                <p style='margin-top:10px; color:#27ae60; font-size:13px;'>💾 Кожна дія автоматично зберігається в стійкий файл 'restaurant_data.json'.</p>
            </div>
            
            <div class='card'>
                <h2>🛒 Поточний Чернетковий Чек (Замовлення №{currentDraftOrder.OrderNumber})</h2>
                <div style='background:#fdfefe; border:1px dashed #ccc; padding:15px; border-radius:8px; margin-bottom:15px;'>
                    {draftItemsHtml}
                    {(currentDraftOrder.Items.Count > 0 ? $"<hr><div style='display:flex; justify-content:space-between; font-weight:bold;'><span>Всього до сплати:</span><span>{currentDraftOrder.GetTotalSum():F2} грн</span></div>" : "")}
                </div>
                <a href='/commit-order' class='btn btn-success'>🚀 Надіслати замовлення на кухню</a>
                <a href='/clear-draft' class='btn btn-danger'>🗑️ Очистити кошик</a>
            </div>
        </div>
        
        <div>
            <div class='card'>
                <h2>💰 Сейф Ресторану (Каса)</h2>
                <p style='color:#7f8c8d; font-size:12px; margin:0;'>Сукупний дохід (Singleton)</p>
                <div class='balance-box'>{FinancialMonitor.Instance.TotalRevenue:F2} грн</div>
                <p style='font-size:12px; color:#27ae60;'>● Дані завантажено з ПЗУ файлу</p>
            </div>
            
            <div class='card'>
                <h2>⚠️ Тестування винятків (Крок 6/8)</h2>
                <p style='font-size:13px; color:#7f8c8d;'>Перевірка стійкості архітектури до порожніх чеків:</p>
                <a href='/test-empty-exception' class='btn btn-danger btn-sm' style='width:100%; text-align:center;'>🔥 Спровокувати порожній чек</a>
            </div>
        </div>
    </div>";

    return Results.Content(GetPageLayout("Головна — Кафе-Система", "main", body), "text/html", System.Text.Encoding.UTF8);
});

app.MapGet("/menu", () =>
{
    string categoriesHtml = "";
    var groups = repositoryMenu.GroupBy(m => m.Category);

    foreach (var group in groups)
    {
        string itemsInCat = "";
        foreach (var item in group)
        {
            string itemDetails = item is DrinkItem d ? $"<span style='font-size:11px; color:#7f8c8d;'>({d.Volume}л)</span>" : "";
            
            string decoratorButtons = "";
            if (group.Key == "Гарячі страви" && item.Name.Contains("Піца"))
            {
                decoratorButtons = $"<a href='/add-decorated?name={Uri.EscapeDataString(item.Name)}&addon=cheese' class='btn btn-sm btn-warning' style='margin-left:5px; background:#9b59b6;'>🧀 +Сир</a>";
            }
            else if (group.Key == "Кава" && !item.Name.Contains("Еспресо"))
            {
                decoratorButtons = $"<a href='/add-decorated?name={Uri.EscapeDataString(item.Name)}&addon=syrup' class='btn btn-sm btn-warning' style='margin-left:5px; background:#d35400;'>🍯 +Сироп</a>";
            }

            itemsInCat += $@"
            <div class='menu-item-card'>
                <div>
                    <div style='font-weight:600; color:#2c3e50;'>{item.Name} {itemDetails}</div>
                    <div style='color:#27ae60; font-weight:bold; font-size:14px; margin-top:4px;'>{item.GetPrice():F2} грн</div>
                </div>
                <div style='display:flex; align-items:center;'>
                    <a href='/add-to-draft?name={Uri.EscapeDataString(item.Name)}' class='btn btn-sm'>＋ Звичайний</a>
                    {decoratorButtons}
                </div>
            </div>";
        }

        categoriesHtml += $@"
        <div class='menu-section'>
            <div class='menu-category-title'>{group.Key}</div>
            <div class='menu-grid'>
                {itemsInCat}
            </div>
        </div>";
    }

    var body = $@"
    <div class='card full-width'>
        <h2>📋 Інтерактивне Меню Ресторану з топінгами (Патерн Decorator)</h2>
        <p style='color:#7f8c8d; margin-bottom:25px;'>Оберіть базову страву або додайте до неї модифікатори, які динамічно змінять назву та прорахують ціну.</p>
        {categoriesHtml}
    </div>";

    return Results.Content(GetPageLayout("Категорії Меню — Кафе-Система", "menu", body), "text/html", System.Text.Encoding.UTF8);
});

app.MapGet("/orders", () =>
{
    string logsHtml = string.Join("", systemLogs.AsEnumerable().Reverse().Select(l => $"<div class='log-line'>{l}</div>"));

    var body = $@"
    <div class='grid'>
        <div>
            <div class='card'>
                <h2>🍳 Керування чергою кухонних цехів з Retry Policy</h2>
                <p>Натисніть кнопку нижче. При обробці замовлення задіяна Retry Policy з експоненційною затримкою. Якщо кухонний апарат видасть помилку, система повторить спробу автоматично.</p>
                <div style='margin: 20px 0;'>
                    <a href='/cook-next' class='btn btn-warning' style='font-size:16px; padding:15px 25px;'>🔥 Приготувати наступне замовлення з черги</a>
                </div>
                <p style='font-size:13px; color:#e67e22;'>* Для логування відмовостійкості використовуються делегати Action<string>.</p>
            </div>
        </div>
        
        <div>
            <div class='card'>
                <h2>📜 Монітор подій</h2>
                <div style='background:#1e272e; color:#f5f6fa; padding:15px; border-radius:8px; max-height:350px; overflow-y:auto;'>
                    {logsHtml}
                </div>
            </div>
        </div>
    </div>";

    return Results.Content(GetPageLayout("Черга та Логи — Кафе-Система", "orders", body), "text/html", System.Text.Encoding.UTF8);
});

app.MapGet("/add-to-draft", (string name) =>
{
    var match = repositoryMenu.FirstOrDefault(m => m.Name == name);
    if (match != null)
    {
        currentDraftOrder += match;
        systemLogs.Add($"[ОФІЦІАНТ]: Додано в кошик замовлення №{currentDraftOrder.OrderNumber}: {match.Name}.");
        AutoSave();
    }
    return Results.Content("<script>window.location.href='/menu';</script>", "text/html");
});

app.MapGet("/add-decorated", (string name, string addon) =>
{
    var match = repositoryMenu.FirstOrDefault(m => m.Name == name);
    if (match != null)
    {
        MenuItem decoratedItem = match;

        if (addon == "cheese")
        {
            decoratedItem = new ExtraCheeseDecorator(match);
        }
        else if (addon == "syrup")
        {
            decoratedItem = new CoffeeSyrupDecorator(match);
        }

        currentDraftOrder += decoratedItem;
        systemLogs.Add($"[ДЕКОРАТОР]: У кошик додано модифікований продукт: {decoratedItem.Name} за ціною {decoratedItem.GetPrice():F2} грн.");
        AutoSave();
    }
    return Results.Content("<script>window.location.href='/menu';</script>", "text/html");
});

app.MapGet("/clear-draft", () =>
{
    currentDraftOrder.Dispose();
    currentDraftOrder = new Order(waiter);
    systemLogs.Add("[ОФІЦІАНТ]: Поточну чернетку кошика скасовано.");
    AutoSave();
    return Results.Content("<script>window.location.href='/';</script>", "text/html");
});

app.MapGet("/commit-order", () =>
{
    try
    {
        orderProcessor.ProcessOrder(currentDraftOrder);
        systemLogs.Add($"[СИСТЕМА]: Замовлення №{currentDraftOrder.OrderNumber} успішно зафіксовано в черзі.");
        currentDraftOrder = new Order(waiter);
        AutoSave();
    }
    catch (Exception ex)
    {
        systemLogs.Add($"[ЗАХИСТ]: {ex.Message}");
        AutoSave();
    }
    return Results.Content("<script>window.location.href='/orders';</script>", "text/html");
});

app.MapGet("/cook-next", () =>
{
    orderProcessor.CookNextOrderInQueue((msg) =>
    {
        systemLogs.Add(msg);
    });
    AutoSave();
    return Results.Content("<script>window.location.href='/orders';</script>", "text/html");
});

app.MapGet("/test-empty-exception", () =>
{
    try
    {
        Order emptyTest = new Order(waiter);
        orderProcessor.ProcessOrder(emptyTest);
        return Results.Redirect("/");
    }
    catch (EmptyOrderException ex)
    {
        return Results.Content($@"
            <div style='font-family:sans-serif; margin:50px; padding:30px; border:2px solid #e74c3c; background:#fdf2f2; border-radius:12px;'>
                <h1 style='color:#c0392b; margin-top:0;'>⚠️ Перехоплено Custom Exception (Практична 6/8)</h1>
                <p style='font-size:16px;'><strong>Клас помилки:</strong> Domain.EmptyOrderException</p>
                <p style='font-size:16px;'><strong>Повідомлення інваріанту:</strong> {ex.Message}</p>
                <hr style='border:1px solid #e74c3c;'>
                <a href='/' style='font-size:15px; color:#3498db; font-weight:bold; text-decoration:none;'>← Повернутися на головну сторінку</a>
            </div>", "text/html", System.Text.Encoding.UTF8);
    }
});

app.Run();
