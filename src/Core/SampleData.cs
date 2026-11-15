using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IReadOnlyList<Product> Products() =>
    [
        Product.Create("id01", "SKU-101", "Кабель ВВГнг 3х2.5", "м", 120),
        Product.Create("id02", "SKU-102", "Кабель UTP Cat5e", "м", 85),
        Product.Create("id03", "SKU-103", "Гофра ПВХ 20мм", "м", 50),
        Product.Create("id04", "SKU-104", "Труба металопластикова", "м", 8),
        Product.Create("id05", "SKU-201", "Автомат 16А", "шт", 45),
        Product.Create("id06", "SKU-202", "Розетка подвійна", "шт", 30),
        Product.Create("id07", "SKU-203", "Вимикач одноклавішний", "шт", 25),
        Product.Create("id08", "SKU-204", "Коробка розподільча", "шт", 60),
        Product.Create("id09", "SKU-205", "Світильник LED 36W", "шт", 4),
        Product.Create("id10", "SKU-206", "Датчик руху", "шт", 12),
        Product.Create("id11", "SKU-207", "Клемник WAGO 3-пров.", "шт", 11),
        Product.Create("id12", "SKU-301", "Дюбель 6х40 (100 шт)", "уп", 18),
        Product.Create("id13", "SKU-302", "Стяжка кабельна 200мм", "уп", 40),
        Product.Create("id14", "SKU-303", "Ізолента ПВХ чорна", "уп", 7),
        Product.Create("id15", "SKU-401", "Фарба акрилова біла", "л", 15),
        Product.Create("id16", "SKU-402", "Ґрунтовка глибока", "л", 22),
        Product.Create("id17", "SKU-403", "Розчинник 646", "л", 5)
    ];
}