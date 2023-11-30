﻿
class Program
{
    private static Item[] items;

    public static void Run()
    {
        while (true)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Добавить элемент");
            Console.WriteLine("2. Удалить элемент");
            Console.WriteLine("3. Просмотр инвентаря");
            Console.WriteLine("4. Сортировка по имени");
            Console.WriteLine("5. Сортировка по количеству");
            Console.WriteLine("6. Фильтрация по минимальному количеству");
            Console.WriteLine("7. Закрыть");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddItem();
                    break;
                case "2":
                    RemoveItem();
                    break;
                case "3":
                    ViewInventory();
                    break;
                case "4":
                    SortItemsByName();
                    ViewInventory();
                    break;
                case "5":
                    SortItemsByQuantity();
                    ViewInventory();
                    break;
                case "6":
                    FilterItemsByQuantity();
                    break;
                case "7":
                    SaveAndExit();
                    return;
                default:
                    Console.WriteLine("Некорректный выбор. Пожалуйста, выберите 1, 2, 3, 4, 5, 6 или 7.");
                    break;
            }
        }
    }

    private static void AddItem()
    {
        Console.WriteLine("Введите данные для нового предмета:");
        Console.Write("Имя предмета: ");
        string name = Console.ReadLine();

        Console.Write("Количество: ");
        int quantity;
        while (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
        {
            Console.WriteLine("Введите корректное положительное число.");
            Console.Write("Количество: ");
        }

        int existingItemIndex = Array.FindIndex(items, item => item?.Name == name);
        if (existingItemIndex != -1)
        {
            items[existingItemIndex].Quantity += quantity;
        }
        else
        {
            Item[] newItems = new Item[items.Length + 1];

            for (int i = 0; i < items.Length; i++)
            {
                newItems[i] = items[i];
            }

            newItems[newItems.Length - 1] = new Item { Name = name, Quantity = quantity };

            items = newItems;
        }

        SaveData();
    }

    private static void RemoveItem()
    {
        if (items.Length == 0)
        {
            Console.WriteLine("Нет элементов для удаления.");
            return;
        }

        Console.WriteLine("Выберите номер элемента для удаления:");

        for (int i = 0; i < items.Length; i++)
        {
            Console.WriteLine($"{i + 1}. Name: {items[i].Name}, Quantity: {items[i].Quantity}");
        }

        int indexToRemove;
        while (!int.TryParse(Console.ReadLine(), out indexToRemove) || indexToRemove < 1 || indexToRemove > items.Length)
        {
            Console.WriteLine("Введите корректный номер элемента.");
            Console.Write("Введите номер элемента: ");
        }

        Console.Write($"Введите количество элементов для удаления (от 1 до {items[indexToRemove - 1].Quantity}): ");
        int quantityToRemove;
        while (!int.TryParse(Console.ReadLine(), out quantityToRemove) || quantityToRemove < 1 || quantityToRemove > items[indexToRemove - 1].Quantity)
        {
            Console.WriteLine($"Введите корректное число от 1 до {items[indexToRemove - 1].Quantity}.");
            Console.Write("Введите количество элементов для удаления: ");
        }

        items[indexToRemove - 1].Quantity -= quantityToRemove;

        if (items[indexToRemove - 1].Quantity == 0)
        {
            Item[] newItems = new Item[items.Length - 1];

            for (int i = 0; i < indexToRemove - 1; i++)
            {
                newItems[i] = items[i];
            }

            for (int i = indexToRemove; i < items.Length; i++)
            {
                newItems[i - 1] = items[i];
            }

            items = newItems;
        }

        SaveData();
    }

    private static void ViewInventory()
    {
        if (items.Length == 0)
        {
            Console.WriteLine("Инвентарь пуст.");
        }
        else
        {
            Console.WriteLine("Информация о предметах в инвентаре:");
            for (int i = 0; i < items.Length; i++)
            {
                Console.WriteLine($"{i + 1}. Name: {items[i].Name}, Quantity: {items[i].Quantity}");
            }
        }
    }

    private static void SortItemsByName()
    {
        ItemProcessor.SortItems(items, (item1, item2) => item1.Name.CompareTo(item2.Name));
    }

    private static void SortItemsByQuantity()
    {
        ItemProcessor.SortItems(items, (item1, item2) => item1.Quantity.CompareTo(item2.Quantity));
    }

    private static void FilterItemsByQuantity()
    {
        Console.Write("Введите минимальное количество: ");
        int minQuantity;
        while (!int.TryParse(Console.ReadLine(), out minQuantity) || minQuantity < 0)
        {
            Console.WriteLine("Введите корректное неотрицательное число.");
            Console.Write("Минимальное количество: ");
        }

        Item[] filteredItems = ItemProcessor.FilterItems(items, item => item.Quantity >= minQuantity);
        Console.WriteLine("Отфильтрованные данные:");
        foreach (var item in filteredItems)
        {
            Console.WriteLine($"Name: {item.Name}, Quantity: {item.Quantity}");
        }
    }

    private static void SaveData()
    {
        DataHandler<Item[]> dataHandler = new DataHandler<Item[]>();
        dataHandler.SaveToBinaryFile(items, "binary_data.dat");
        dataHandler.SaveToJsonFile(items, "json_data.json");
        Console.WriteLine("Данные успешно сохранены.");
    }

    private static void SaveAndExit()
    {
        SaveData();
        Console.WriteLine("Программа завершена.");
    }

    static void Main()
    {
        DataHandler<Item[]> dataHandler = new DataHandler<Item[]>();

        if (File.Exists("binary_data.dat"))
        {
            items = dataHandler.LoadFromBinaryFile("binary_data.dat");
            if (items == null)
            {
                Console.WriteLine("Ошибка при загрузке данных из бинарного файла. Создан новый массив.");
                items = new Item[0];
            }
        }
        else
        {
            Console.WriteLine("Бинарный файл не существует. Создан новый массив.");
            items = new Item[0];
        }

        Run();
    }
}