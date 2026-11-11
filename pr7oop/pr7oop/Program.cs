using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace PracticalXmlApp
{
    // === 1, 2, 4. Клас Product з підтримкою XmlIgnore ===
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Undefined";
        public decimal Price { get; set; }

        [XmlIgnore] // Завдання 4: виключення властивості із серіалізації
        public string InternalNotes { get; set; } = "Конфіденційно";

        public Product() { }
        public Product(int id, string name, decimal price, string notes)
        {
            Id = id;
            Name = name;
            Price = price;
            InternalNotes = notes;
        }
    }

    // === 3. Клас Student ===
    public class Student
    {
        public string FullName { get; set; } = "Undefined";
        public double AverageGrade { get; set; }

        public Student() { }
        public Student(string fullName, double averageGrade)
        {
            FullName = fullName;
            AverageGrade = averageGrade;
        }
    }

    // === 6. Клас Book для каталогу ===
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "Undefined";
        public string Author { get; set; } = "Undefined";
        public int Year { get; set; }

        public Book() { }
        public Book(int id, string title, string author, int year)
        {
            Id = id;
            Title = title;
            Author = author;
            Year = year;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.InputEncoding = System.Text.Encoding.Unicode;

            while (true)
            {
                Console.WriteLine("\n================ ПРАКТИЧНІ ЗАВДАННЯ ================");
                Console.WriteLine("1. Створити та серіалізувати об’єкт Product в XML");
                Console.WriteLine("2. Десеріалізувати Product з XML та вивести властивості");
                Console.WriteLine("3. Зберегти список 5 студентів у XML, знайти макс. бал");
                Console.WriteLine("4. Перевірка [XmlIgnore] для виключення властивості");
                Console.WriteLine("5. Зчитування XML-файлу з обробленням винятків");
                Console.WriteLine("6. Консольний каталог книг (додавання, пошук, видалення, збереження)");
                Console.WriteLine("0. Вихід");
                Console.Write("Оберіть номер завдання (0-6): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        Task1_SerializeProduct();
                        break;
                    case "2":
                        Task2_DeserializeProduct();
                        break;
                    case "3":
                        Task3_StudentsManagement();
                        break;
                    case "4":
                        Task4_XmlIgnoreDemo();
                        break;
                    case "5":
                        Task5_SafeFileReader();
                        break;
                    case "6":
                        Task6_BookCatalog();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }
            }
        }

        // --- Завдання 1 ---
        static void Task1_SerializeProduct()
        {
            Console.WriteLine("--- Завдання 1: Серіалізація об'єкта Product в XML ---");
            Product product = new Product(101, "Ігровий ноутбук", 45000m, "Секретний постачальник");

            XmlSerializer serializer = new XmlSerializer(typeof(Product));
            using (FileStream fs = new FileStream("product.xml", FileMode.Create))
            {
                serializer.Serialize(fs, product);
            }
            Console.WriteLine("Об'єкт успішно серіалізовано у файл product.xml!");
            Console.WriteLine($"Дані об'єкта: Id={product.Id}, Name={product.Name}, Price={product.Price}");
        }

        // --- Завдання 2 ---
        static void Task2_DeserializeProduct()
        {
            Console.WriteLine("--- Завдання 2: Десеріалізація Product з XML ---");
            if (!File.Exists("product.xml"))
            {
                Console.WriteLine("Файл product.xml не знайдено! Спершу виконайте завдання 1.");
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Product));
            using (FileStream fs = new FileStream("product.xml", FileMode.Open))
            {
                Product? product = serializer.Deserialize(fs) as Product;
                if (product != null)
                {
                    Console.WriteLine($"Успішно десеріалізовано:");
                    Console.WriteLine($"ID: {product.Id}");
                    Console.WriteLine($"Назва: {product.Name}");
                    Console.WriteLine($"Ціна: {product.Price}");
                }
            }
        }

        // --- Завдання 3 ---
        static void Task3_StudentsManagement()
        {
            Console.WriteLine("--- Завдання 3: Робота зі списком студентів ---");
            Student[] students = new Student[]
            {
                new Student("Олександр Петренко", 85.4),
                new Student("Марія Коваленко", 94.2),
                new Student("Іван Шевченко", 78.9),
                new Student("Анна Бондаренко", 91.5),
                new Student("Максим Мельник", 88.0)
            };

            XmlSerializer serializer = new XmlSerializer(typeof(Student[]));
            using (FileStream fs = new FileStream("students.xml", FileMode.Create))
            {
                serializer.Serialize(fs, students);
            }
            Console.WriteLine("Список із 5 студентів збережено у students.xml.");

            using (FileStream fs = new FileStream("students.xml", FileMode.Open))
            {
                Student[]? loadedStudents = serializer.Deserialize(fs) as Student[];
                if (loadedStudents != null && loadedStudents.Length > 0)
                {
                    var topStudent = loadedStudents.OrderByDescending(s => s.AverageGrade).First();
                    Console.WriteLine($"\nВідновлений список студентів та їх середній бал:");
                    foreach (var s in loadedStudents)
                    {
                        Console.WriteLine($"- {s.FullName}: {s.AverageGrade}");
                    }
                    Console.WriteLine($"\n🏆 Студент з найвищим середнім балом: {topStudent.FullName} ({topStudent.AverageGrade})");
                }
            }
        }

        // --- Завдання 4 ---
        static void Task4_XmlIgnoreDemo()
        {
            Console.WriteLine("--- Завдання 4: Перевірка [XmlIgnore] ---");
            Product product = new Product(202, "Миша", 1200m, "Конфіденційна внутрішня замітка");

            XmlSerializer serializer = new XmlSerializer(typeof(Product));
            using (FileStream fs = new FileStream("product_ignore.xml", FileMode.Create))
            {
                serializer.Serialize(fs, product);
            }

            Console.WriteLine("Об'єкт серіалізовано. У файлі product_ignore.xml властивість InternalNotes відсутня завдяки атрибуту [XmlIgnore].");
            Console.WriteLine($"У пам'яті InternalNotes = \"{product.InternalNotes}\"");
        }

        // --- Завдання 5 ---
        static void Task5_SafeFileReader()
        {
            Console.WriteLine("--- Завдання 5: Зчитування файлу з обробленням винятків ---");
            Console.Write("Введіть ім'я файлу для читання (наприклад, product.xml або неіснуючий): ");
            string fileName = Console.ReadLine() ?? "product.xml";

            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string content = reader.ReadToEnd();
                    Console.WriteLine("\nВміст файлу успішно прочитано:");
                    Console.WriteLine(content);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"[Помилка]: Файл '{fileName}' не знайдено.");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("[Помилка]: Вказаний шлях до каталогу не знайдено.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("[Помилка]: Доступ до файлу заборонено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Неочікувана помилка]: {ex.Message}");
            }
        }

        // --- Завдання 6 ---
        static void Task6_BookCatalog()
        {
            List<Book> books = new List<Book>();
            string filePath = "books.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));

            if (File.Exists(filePath))
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        var loaded = serializer.Deserialize(fs) as List<Book>;
                        if (loaded != null) books = loaded;
                    }
                }
                catch { /* Ігноруємо якщо файл пошкоджений */ }
            }

            while (true)
            {
                Console.WriteLine("\n--- КАТАЛОГ КНИГ ---");
                Console.WriteLine("1. Показати всі книги");
                Console.WriteLine("2. Додати книгу");
                Console.WriteLine("3. Пошук книги за назвою");
                Console.WriteLine("4. Видалити книгу за ID");
                Console.WriteLine("5. Зберегти зміни у файл");
                Console.WriteLine("0. Повернутися в головне меню");
                Console.Write("Виберіть дію: ");

                string action = Console.ReadLine();
                Console.WriteLine();

                if (action == "1")
                {
                    if (books.Count == 0) Console.WriteLine("Каталог порожній.");
                    else
                    {
                        foreach (var b in books)
                            Console.WriteLine($"ID: {b.Id} | Назва: {b.Title} | Автор: {b.Author} | Рік: {b.Year}");
                    }
                }
                else if (action == "2")
                {
                    Console.Write("Введіть ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        Console.Write("Введіть назву: ");
                        string title = Console.ReadLine() ?? "Без назви";
                        Console.Write("Введіть автора: ");
                        string author = Console.ReadLine() ?? "Невідомий";
                        Console.Write("Введіть рік видання: ");
                        if (int.TryParse(Console.ReadLine(), out int year))
                        {
                            books.Add(new Book(id, title, author, year));
                            Console.WriteLine("Книгу успішно додано!");
                        }
                        else Console.WriteLine("Некоректний рік.");
                    }
                    else Console.WriteLine("Некоректний ID.");
                }
                else if (action == "3")
                {
                    Console.Write("Введіть пошуковий запит (назву): ");
                    string query = Console.ReadLine()?.ToLower() ?? "";
                    var found = books.Where(b => b.Title.ToLower().Contains(query)).ToList();

                    if (found.Count == 0) Console.WriteLine("Нічого не знайдено.");
                    else
                    {
                        Console.WriteLine("Знайдені книги:");
                        foreach (var b in found)
                            Console.WriteLine($"ID: {b.Id} | Назва: {b.Title} | Автор: {b.Author} | Рік: {b.Year}");
                    }
                }
                else if (action == "4")
                {
                    Console.Write("Введіть ID книги для видалення: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        int removed = books.RemoveAll(b => b.Id == id);
                        if (removed > 0) Console.WriteLine("Книгу видалено.");
                        else Console.WriteLine("Книгу з таким ID не знайдено.");
                    }
                }
                else if (action == "5")
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        serializer.Serialize(fs, books);
                    }
                    Console.WriteLine("Каталог успішно збережено у файл books.xml!");
                }
                else if (action == "0")
                {
                    break;
                }
            }
        }
    }
}