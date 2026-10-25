using Core.Domain;

namespace Core;

public static class SampleData
{
    public static List<Product> Products() => new()
    {
        Product.Create("P-001", "SKU-001", "Цемент М400 25кг", "шт", 100),
        Product.Create("P-002", "SKU-002", "Пісок будівельний", "т", 25),
        Product.Create("P-003", "SKU-003", "Цегла червона", "шт", 1500),
        Product.Create("P-004", "SKU-004", "Ґрунтовка ГФ-021", "л", 40),
        Product.Create("P-005", "SKU-005", "Арматура 12мм", "м", 600),
        Product.Create("P-006", "SKU-006", "Шпаклівка 20кг", "мішок", 80),
        Product.Create("P-007", "SKU-007", "Клей для плитки 25кг", "мішок", 120),
        Product.Create("P-008", "SKU-008", "Гіпсокартон 12.5мм", "лист", 90),
        Product.Create("P-009", "SKU-009", "Профіль для ГКЛ 60х27", "шт", 200),
        Product.Create("P-010", "SKU-010", "Саморізи 3.5х25", "пачка", 50),
        Product.Create("P-011", "SKU-011", "Фарба акрилова 10л", "відро", 15),
        Product.Create("P-012", "SKU-012", "Лак деревозахисний", "л", 30),
        Product.Create("P-013", "SKU-013", "Піна монтажна 750мл", "шт", 65),
        Product.Create("P-014", "SKU-014", "Герметик силіконовий", "шт", 45),
        Product.Create("P-015", "SKU-015", "Кабель UTP cat6", "м", 500)
    };
}