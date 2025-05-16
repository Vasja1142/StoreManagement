// Файл: StoreManagement.Tests/DomainModelTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreManagement.Domain;
using System;
using System.IO; // Для StringWriter, StringReader, StreamReader
using System.Globalization; // Для CultureInfo
using System.Linq; // Для Enumerable.Contains в RandomInit тестах

namespace StoreManagement.Tests
{
    // Вспомогательный класс только для тестирования базового Goods
    public class TestOnlyGoods : Goods
    {
        public TestOnlyGoods() : base() { }
        public TestOnlyGoods(string name, decimal price, string manufacturer) : base(name, price, manufacturer) { }
        public TestOnlyGoods(Goods other) : base(other) { }

        // Реализуем абстрактные методы, но они не будут вызываться в этих тестах
        public override void Init() { throw new NotImplementedException("Init should not be called on TestOnlyGoods in these tests."); }
        public override void RandomInit() { throw new NotImplementedException("RandomInit should not be called on TestOnlyGoods in these tests."); }
        public override object Clone() { return new TestOnlyGoods(this); }
    }

    [TestClass]
    public partial class DomainModelTests
    {
        private CultureInfo _originalCulture;
        private StringWriter _stringWriter;
        private TextWriter _originalConsoleOut;
        private TextReader _originalConsoleIn;

        [TestInitialize]
        public void TestInitialize()
        {
            _originalCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");

            _originalConsoleOut = Console.Out;
            _stringWriter = new StringWriter();
            Console.SetOut(_stringWriter);

            _originalConsoleIn = Console.In; // Сохраняем оригинальный Console.In
        }

        [TestCleanup]
        public void TestCleanup()
        {
            CultureInfo.CurrentCulture = _originalCulture;

            Console.SetOut(_originalConsoleOut);
            _stringWriter.Dispose();

            Console.SetIn(_originalConsoleIn); // Восстанавливаем оригинальный Console.In
        }

        // --- Вспомогательные методы для создания тестовых объектов ---
        private Product CreateTestProduct(string name = "Test Product", decimal price = 10m, string manufacturer = "Test Manufacturer", DateTime? expirationDate = null)
        {
            return new Product(name, price, manufacturer, expirationDate ?? DateTime.Today.AddDays(7));
        }

        private DairyProduct CreateTestDairyProduct(string name = "Test Dairy", decimal price = 12m, string manufacturer = "Test DairyMan", DateTime? expirationDate = null, double fat = 3.2, double vol = 0.5)
        {
            return new DairyProduct(name, price, manufacturer, expirationDate ?? DateTime.Today.AddDays(5), fat, vol);
        }

        private Toy CreateTestToy(string name = "Test Toy", decimal price = 20m, string manufacturer = "Test ToyMaker", int age = 3, string material = "Plastic")
        {
            return new Toy(name, price, manufacturer, age, material);
        }

        // --- Тесты для Goods (базовый класс) ---

        [TestMethod]
        public void Goods_DefaultConstructor_InitializesDefaultValues()
        {
            var goods = new TestOnlyGoods();
            Assert.AreEqual("Без имени", goods.Name, "Default Name is incorrect.");
            Assert.AreEqual(0m, goods.Price, "Default Price is incorrect.");
            Assert.AreEqual("Неизвестен", goods.Manufacturer, "Default Manufacturer is incorrect.");
        }

        [TestMethod]
        public void Goods_ParameterizedConstructor_InitializesProperties()
        {
            var goods = new TestOnlyGoods("Specific Name", 123.45m, "SpecificMan");
            Assert.AreEqual("Specific Name", goods.Name);
            Assert.AreEqual(123.45m, goods.Price);
            Assert.AreEqual("SpecificMan", goods.Manufacturer);
        }

        [TestMethod]
        public void Goods_CopyConstructor_CopiesProperties()
        {
            var original = new TestOnlyGoods("Original", 50m, "OriginalMan");
            var copy = new TestOnlyGoods(original);

            Assert.AreEqual(original.Name, copy.Name);
            Assert.AreEqual(original.Price, copy.Price);
            Assert.AreEqual(original.Manufacturer, copy.Manufacturer);
            Assert.AreNotSame(original, copy, "Copy constructor should create a new instance.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Goods_CopyConstructor_NullSource_ThrowsArgumentNullException()
        {
            var copy = new TestOnlyGoods((Goods)null);
        }

        [TestMethod]
        public void Goods_Properties_SetAndGetCorrectly()
        {
            var goods = new TestOnlyGoods();
            goods.Name = "New Name";
            goods.Price = 123.45m;
            goods.Manufacturer = "New Manufacturer";

            Assert.AreEqual("New Name", goods.Name);
            Assert.AreEqual(123.45m, goods.Price);
            Assert.AreEqual("New Manufacturer", goods.Manufacturer);
        }

        [TestMethod]
        public void Goods_Name_CanBeNullOrEmpty()
        {
            var goods = new TestOnlyGoods();
            goods.Name = null;
            Assert.IsNull(goods.Name, "Name should be settable to null.");

            goods.Name = string.Empty;
            Assert.AreEqual(string.Empty, goods.Name, "Name should be settable to empty string.");
        }

        [TestMethod]
        public void Goods_Price_CanBeZeroOrNegative()
        {
            var goods = new TestOnlyGoods();
            goods.Price = 0m;
            Assert.AreEqual(0m, goods.Price, "Price should be settable to zero.");

            goods.Price = -10.5m;
            Assert.AreEqual(-10.5m, goods.Price, "Price should be settable to negative.");
        }

        [TestMethod]
        public void Goods_Manufacturer_CanBeNullOrEmpty()
        {
            var goods = new TestOnlyGoods();
            goods.Manufacturer = null;
            Assert.IsNull(goods.Manufacturer, "Manufacturer should be settable to null.");

            goods.Manufacturer = string.Empty;
            Assert.AreEqual(string.Empty, goods.Manufacturer, "Manufacturer should be settable to empty string.");
        }

        [TestMethod]
        public void Goods_CompareTo_NullOther_ReturnsOne()
        {
            var good1 = new TestOnlyGoods("Milk", 10m, "Farm");
            Assert.AreEqual(1, good1.CompareTo(null), "CompareTo(null) should return 1.");
        }

        [TestMethod]
        public void Goods_CompareTo_DifferentPrices_ReturnsCorrectComparison()
        {
            var good1 = new TestOnlyGoods("Milk", 10m, "Farm");
            var good2 = new TestOnlyGoods("Cheese", 20m, "Farm");
            var good3 = new TestOnlyGoods("Bread", 10m, "Bakery");

            Assert.IsTrue(good1.CompareTo(good2) < 0, "good1 (10m) should be less than good2 (20m).");
            Assert.IsTrue(good2.CompareTo(good1) > 0, "good2 (20m) should be greater than good1 (10m).");
            Assert.AreEqual(0, good1.CompareTo(good3), "good1 (10m) should be equal to good3 (10m) in price comparison.");
        }

        [TestMethod]
        public void Goods_CompareTo_NegativePrices()
        {
            var good1 = new TestOnlyGoods("G1", -20m, "M");
            var good2 = new TestOnlyGoods("G2", -10m, "M");
            Assert.IsTrue(good1.CompareTo(good2) < 0, "good1 (-20m) should be less than good2 (-10m).");
            Assert.IsTrue(good2.CompareTo(good1) > 0, "good2 (-10m) should be greater than good1 (-20m).");
        }

        [TestMethod]
        public void Goods_CompareTo_ZeroPrice()
        {
            var good1 = new TestOnlyGoods("G1", 0m, "M");
            var good2 = new TestOnlyGoods("G2", 10m, "M");
            var good3 = new TestOnlyGoods("G3", -10m, "M");
            Assert.IsTrue(good1.CompareTo(good2) < 0);
            Assert.IsTrue(good1.CompareTo(good3) > 0);
            Assert.AreEqual(0, good1.CompareTo(new TestOnlyGoods("G4", 0m, "M")));
        }

        [TestMethod]
        public void Goods_Equals_Self_ReturnsTrue()
        {
            var good1 = new TestOnlyGoods("Milk", 10m, "Farm");
            Assert.IsTrue(good1.Equals(good1), "Object should be equal to itself.");
        }

        [TestMethod]
        public void Goods_Equals_NullObject_ReturnsFalse()
        {
            var good1 = new TestOnlyGoods("Milk", 10m, "Farm");
            Assert.IsFalse(good1.Equals(null), "Equals(null) should return false.");
        }

        [TestMethod]
        public void Goods_Equals_DifferentType_ReturnsFalse()
        {
            var good1 = new TestOnlyGoods("Milk", 10m, "Farm");
            var otherType = new object();
            Assert.IsFalse(good1.Equals(otherType), "Equals with different type should return false.");
        }

        [TestMethod]
        public void Goods_Equals_SameProperties_ReturnsTrue()
        {
            var good1 = new TestOnlyGoods("Milk", 10m, "Farm");
            var good2 = new TestOnlyGoods("Milk", 10m, "Farm");
            Assert.IsTrue(good1.Equals(good2), "Goods with same properties should be equal.");
        }

        [TestMethod]
        public void Goods_Equals_DifferentPrice_ReturnsFalse()
        {
            var good1 = new TestOnlyGoods("SameName", 10m, "SameMan");
            var good2 = new TestOnlyGoods("SameName", 11m, "SameMan");
            Assert.IsFalse(good1.Equals(good2), "Goods with different prices should not be equal.");
        }

        [TestMethod]
        public void Goods_Equals_DifferentName_ReturnsFalse()
        {
            var good1 = new TestOnlyGoods("Name1", 10m, "SameMan");
            var good2 = new TestOnlyGoods("Name2", 10m, "SameMan");
            Assert.IsFalse(good1.Equals(good2), "Goods with different names should not be equal.");
        }

        [TestMethod]
        public void Goods_Equals_DifferentManufacturer_ReturnsFalse()
        {
            var good1 = new TestOnlyGoods("SameName", 10m, "Man1");
            var good2 = new TestOnlyGoods("SameName", 10m, "Man2");
            Assert.IsFalse(good1.Equals(good2), "Goods with different manufacturers should not be equal.");
        }

        [TestMethod]
        public void Goods_Equals_NullOrEmptyName_ReturnsCorrectly()
        {
            var g1 = new TestOnlyGoods(name: null, price: 10m, manufacturer: "Man");
            var g2 = new TestOnlyGoods(name: null, price: 10m, manufacturer: "Man");
            var g3 = new TestOnlyGoods(name: "Name", price: 10m, manufacturer: "Man");
            var g4 = new TestOnlyGoods(name: string.Empty, price: 10m, manufacturer: "Man");
            var g5 = new TestOnlyGoods(name: string.Empty, price: 10m, manufacturer: "Man");

            Assert.IsTrue(g1.Equals(g2), "Two goods with null names and same other props should be equal.");
            Assert.IsFalse(g1.Equals(g3), "Good with null name and good with non-null name should not be equal.");
            Assert.IsTrue(g4.Equals(g5), "Two goods with empty names and same other props should be equal.");
            Assert.IsFalse(g1.Equals(g4), "Good with null name and good with empty name should not be equal.");
        }

        [TestMethod]
        public void Goods_Equals_NullOrEmptyManufacturer_ReturnsCorrectly()
        {
            var g1 = new TestOnlyGoods(name: "Name", price: 10m, manufacturer: null);
            var g2 = new TestOnlyGoods(name: "Name", price: 10m, manufacturer: null);
            var g3 = new TestOnlyGoods(name: "Name", price: 10m, manufacturer: "Man");
            var g4 = new TestOnlyGoods(name: "Name", price: 10m, manufacturer: string.Empty);
            var g5 = new TestOnlyGoods(name: "Name", price: 10m, manufacturer: string.Empty);

            Assert.IsTrue(g1.Equals(g2));
            Assert.IsFalse(g1.Equals(g3));
            Assert.IsTrue(g4.Equals(g5));
            Assert.IsFalse(g1.Equals(g4));
        }

        [TestMethod]
        public void Goods_GetHashCode_ConsistentForSameObject()
        {
            var good1 = new TestOnlyGoods("Milk", 10m, "Farm");
            int hash1 = good1.GetHashCode();
            int hash2 = good1.GetHashCode();
            Assert.AreEqual(hash1, hash2, "GetHashCode should return consistent value for the same object state.");
        }

        [TestMethod]
        public void Goods_GetHashCode_EqualObjectsHaveEqualHashCodes()
        {
            var good1 = new TestOnlyGoods("Milk", 10m, "Farm");
            var good2 = new TestOnlyGoods("Milk", 10m, "Farm");
            Assert.AreEqual(good1.GetHashCode(), good2.GetHashCode(), "Equal objects should have equal hash codes.");
        }

        [TestMethod]
        public void Goods_GetHashCode_ChangesWithPropertyChange()
        {
            var good1 = new TestOnlyGoods("Milk", 10m, "Farm");
            int initialHash = good1.GetHashCode();
            good1.Price = 11m;
            int newHash = good1.GetHashCode();
            Assert.AreNotEqual(initialHash, newHash, "GetHashCode should change if a relevant property changes.");
            good1.Price = 10m;
            Assert.AreEqual(initialHash, good1.GetHashCode(), "GetHashCode should revert if property reverts.");
        }

        [TestMethod]
        public void Goods_GetHashCode_WithNullOrEmptyNameOrManufacturer()
        {
            var g1 = new TestOnlyGoods(name: null, price: 10m, manufacturer: "Man");
            var g2 = new TestOnlyGoods(name: "Name", price: 10m, manufacturer: null);
            var g3 = new TestOnlyGoods(name: string.Empty, price: 10m, manufacturer: "Man");
            var g4 = new TestOnlyGoods(name: "Name", price: 10m, manufacturer: string.Empty);

            Assert.IsNotNull(g1.GetHashCode());
            Assert.IsNotNull(g2.GetHashCode());
            Assert.IsNotNull(g3.GetHashCode());
            Assert.IsNotNull(g4.GetHashCode());

            var g5 = new TestOnlyGoods(name: null, price: 10m, manufacturer: "Man");
            Assert.AreEqual(g1.GetHashCode(), g5.GetHashCode(), "Hashcodes for same state (with null) should match.");
        }

        [TestMethod]
        public void Goods_ToString_FormatsCorrectly()
        {
            var goods = new TestOnlyGoods("Test Good", 12.34m, "Test Man");
            // Учитываем текущую культуру для форматирования валюты
            string expectedPrice = string.Format(CultureInfo.CurrentCulture, "{0:C}", 12.34m);
            string expected = $"TestOnlyGoods: Test Good, Цена: {expectedPrice}, Произв.: Test Man";
            Assert.AreEqual(expected, goods.ToString());
        }

        [TestMethod]
        public void Goods_ToString_WithNullOrEmptyProperties()
        {
            var gNullName = new TestOnlyGoods(name: null, price: 5m, manufacturer: "Man");
            var gEmptyMan = new TestOnlyGoods(name: "Test", price: 5m, manufacturer: string.Empty);

            string expectedPrice = string.Format(CultureInfo.CurrentCulture, "{0:C}", 5.00m);
            string expected1 = $"TestOnlyGoods: , Цена: {expectedPrice}, Произв.: Man";
            Assert.AreEqual(expected1, gNullName.ToString());

            string expected2 = $"TestOnlyGoods: Test, Цена: {expectedPrice}, Произв.: ";
            Assert.AreEqual(expected2, gEmptyMan.ToString());
        }



        [TestMethod]
        public void Goods_ShallowCopy_CreatesMemberwiseClone()
        {
            var original = new TestOnlyGoods("Original Good", 50m, "OriginalMan");
            var copy = original.ShallowCopy() as Goods;

            Assert.IsNotNull(copy);
            Assert.AreNotSame(original, copy, "ShallowCopy instance should be different.");
            Assert.AreEqual(original.Name, copy.Name);
            Assert.AreEqual(original.Price, copy.Price);
            Assert.AreEqual(original.Manufacturer, copy.Manufacturer);

            copy.Price = 60m;
            Assert.AreEqual(50m, original.Price, "Changing Price in shallow copy should not affect original's Price.");
        }

        // --- Тесты для Product ---

        [TestMethod]
        public void Product_DefaultConstructor_InitializesCorrectly()
        {
            var product = new Product();
            Assert.AreEqual("Без имени", product.Name);
            // ExpirationDate по умолчанию не инициализируется, будет default(DateTime)
            // Сеттер сработает только при присваивании.
            Assert.AreEqual(default(DateTime), product.ExpirationDate);
        }

        [TestMethod]
        public void Product_Constructor_InitializesPropertiesCorrectly()
        {
            string name = "Test Product";
            decimal price = 19.99m;
            string manufacturer = "Test Manufacturer";
            DateTime expirationDate = DateTime.Today.AddDays(10);

            var product = new Product(name, price, manufacturer, expirationDate);

            Assert.AreEqual(name, product.Name);
            Assert.AreEqual(price, product.Price);
            Assert.AreEqual(manufacturer, product.Manufacturer);
            Assert.AreEqual(expirationDate.Date, product.ExpirationDate.Date);
        }

        [TestMethod]
        public void Product_CopyConstructor_CopiesProperties()
        {
            var original = CreateTestProduct("OriginalProd", 25m, "ManProd", DateTime.Today.AddDays(3));
            var copy = new Product(original);

            Assert.AreEqual(original.Name, copy.Name);
            Assert.AreEqual(original.Price, copy.Price);
            Assert.AreEqual(original.Manufacturer, copy.Manufacturer);
            Assert.AreEqual(original.ExpirationDate, copy.ExpirationDate);
            Assert.AreNotSame(original, copy);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Product_SetExpirationDate_PastDate_ThrowsArgumentOutOfRangeException()
        {
            var product = CreateTestProduct();
            DateTime pastDate = DateTime.Today.AddDays(-1);
            product.ExpirationDate = pastDate;
        }

        [TestMethod]
        public void Product_ExpirationDate_SetToToday_IsValid()
        {
            var product = CreateTestProduct();
            DateTime today = DateTime.Today;
            product.ExpirationDate = today;
            Assert.AreEqual(today, product.ExpirationDate.Date);
        }

        [TestMethod]
        public void Product_Equals_SameProperties_ReturnsTrue()
        {
            DateTime expDate = DateTime.Today.AddDays(5);
            var product1 = CreateTestProduct(name: "Apple", price: 1.0m, manufacturer: "Orchard", expirationDate: expDate);
            var product2 = CreateTestProduct(name: "Apple", price: 1.0m, manufacturer: "Orchard", expirationDate: expDate);
            Assert.IsTrue(product1.Equals(product2), "Products with same properties should be equal.");
        }

        [TestMethod]
        public void Product_Equals_DifferentExpirationDate_ReturnsFalse()
        {
            var product1 = CreateTestProduct(expirationDate: DateTime.Today.AddDays(5));
            var product2 = CreateTestProduct(expirationDate: DateTime.Today.AddDays(6));
            Assert.IsFalse(product1.Equals(product2), "Products with different expiration dates should not be equal.");
        }

        [TestMethod]
        public void Product_Equals_DifferentTimeSameDate_ReturnsTrue()
        {
            DateTime dateOnly = DateTime.Today.AddDays(5);
            var product1 = CreateTestProduct(expirationDate: dateOnly.AddHours(10));
            var product2 = CreateTestProduct(expirationDate: dateOnly.AddHours(15));
            Assert.IsTrue(product1.Equals(product2), "Products with same date but different time should be equal (date part comparison).");
        }

        [TestMethod]
        public void Product_Equals_DifferentBaseProperties_ReturnsFalse()
        {
            DateTime expDate = DateTime.Today.AddDays(5);
            var product1 = CreateTestProduct(name: "ProdA", price: 1m, expirationDate: expDate);
            var product2 = CreateTestProduct(name: "ProdB", price: 1m, expirationDate: expDate); // Different name
            Assert.IsFalse(product1.Equals(product2));
        }

        [TestMethod]
        public void Product_GetHashCode_EqualObjectsHaveEqualHashCodes()
        {
            DateTime expDate = DateTime.Today.AddDays(5);
            var product1 = CreateTestProduct(name: "Apple", price: 1.0m, manufacturer: "Orchard", expirationDate: expDate);
            var product2 = CreateTestProduct(name: "Apple", price: 1.0m, manufacturer: "Orchard", expirationDate: expDate);
            Assert.AreEqual(product1.GetHashCode(), product2.GetHashCode());
        }


        [TestMethod]
        public void Product_Clone_CreatesIndependentCopy()
        {
            var original = CreateTestProduct("Orange", 2.5m, "Grove", DateTime.Today.AddDays(3));
            var clone = original.Clone() as Product;

            Assert.IsNotNull(clone);
            Assert.AreNotSame(original, clone);
            Assert.IsTrue(original.Equals(clone));

            clone.Price = 3.0m;
            clone.ExpirationDate = DateTime.Today.AddDays(1);
            Assert.AreEqual(2.5m, original.Price);
            Assert.AreNotEqual(original.ExpirationDate, clone.ExpirationDate);
        }

        [TestMethod]
        public void Product_ShallowCopy_IsMemberwiseClone()
        {
            var original = CreateTestProduct("ProdSC", 10m, "ManSC", DateTime.Today.AddDays(1));
            var copy = original.ShallowCopy() as Product;

            Assert.IsNotNull(copy);
            Assert.AreNotSame(original, copy);
            Assert.AreEqual(original.Name, copy.Name);
            Assert.AreEqual(original.Price, copy.Price);
            Assert.AreEqual(original.Manufacturer, copy.Manufacturer);
            Assert.AreEqual(original.ExpirationDate, copy.ExpirationDate);
        }

        [TestMethod]
        public void Product_RandomInit_SetsPropertiesWithinExpectedRanges()
        {
            var product = new Product();
            product.RandomInit();

            Assert.IsTrue(product.Price >= 50m && product.Price <= 550m, $"Price {product.Price} out of expected range (50-550).");
            Assert.IsTrue(product.ExpirationDate.Date >= DateTime.Today.AddDays(1) && product.ExpirationDate.Date <= DateTime.Today.AddDays(90),
                $"ExpirationDate {product.ExpirationDate:yyyy-MM-dd} out of expected range (Today+1 to Today+90).");

            string[] productNames = { "Хлеб", "Масло", "Сыр", "Колбаса", "Йогурт", "Сок" };
            string[] manufacturers = { "Местный Хлебозавод", "Молочный Мир", "Мясной Дом", "Сады Придонья" };
            Assert.IsTrue(productNames.Contains(product.Name), $"Generated product name '{product.Name}' is not in the expected list.");
            Assert.IsTrue(manufacturers.Contains(product.Manufacturer), $"Generated manufacturer '{product.Manufacturer}' is not in the expected list.");
        }

        // --- Тесты для DairyProduct ---
        [TestMethod]
        public void DairyProduct_DefaultConstructor_InitializesCorrectly()
        {
            var dairy = new DairyProduct();
            Assert.AreEqual("Без имени", dairy.Name);
            Assert.AreEqual(default(DateTime), dairy.ExpirationDate);
            Assert.AreEqual(0, dairy.FatContent, "Default FatContent should be 0.");
            Assert.AreEqual(0, dairy.Volume, "Default Volume should be 0.");
        }

        [TestMethod]
        public void DairyProduct_Constructor_InitializesAllPropertiesCorrectly()
        {
            string name = "Test Yogurt";
            decimal price = 2.50m;
            string manufacturer = "Dairy Corp";
            DateTime expirationDate = DateTime.Today.AddDays(14);
            double fatContent = 3.2;
            double volume = 0.5;

            var dairy = new DairyProduct(name, price, manufacturer, expirationDate, fatContent, volume);

            Assert.AreEqual(name, dairy.Name);
            Assert.AreEqual(price, dairy.Price);
            Assert.AreEqual(manufacturer, dairy.Manufacturer);
            Assert.AreEqual(expirationDate.Date, dairy.ExpirationDate.Date);
            Assert.AreEqual(fatContent, dairy.FatContent, 0.001);
            Assert.AreEqual(volume, dairy.Volume, 0.001);
        }

        [TestMethod]
        public void DairyProduct_CopyConstructor_CopiesProperties()
        {
            var original = CreateTestDairyProduct("OriginalDairy", 30m, "ManDairy", DateTime.Today.AddDays(4), 2.5, 0.75);
            var copy = new DairyProduct(original);

            Assert.AreEqual(original.Name, copy.Name);
            Assert.AreEqual(original.Price, copy.Price);
            Assert.AreEqual(original.Manufacturer, copy.Manufacturer);
            Assert.AreEqual(original.ExpirationDate, copy.ExpirationDate);
            Assert.AreEqual(original.FatContent, copy.FatContent, 0.001);
            Assert.AreEqual(original.Volume, copy.Volume, 0.001);
            Assert.AreNotSame(original, copy);
        }

        [TestMethod]
        public void DairyProduct_Properties_SetValidEdgeValues()
        {
            var dairy = CreateTestDairyProduct();
            dairy.FatContent = 0.0;
            Assert.AreEqual(0.0, dairy.FatContent, 0.0001);

            dairy.Volume = 0.001;
            Assert.AreEqual(0.001, dairy.Volume, 0.0001);
        }

        [TestMethod]
        public void DairyProduct_Equals_SameProperties_ReturnsTrue()
        {
            DateTime expDate = DateTime.Today.AddDays(3);
            var dairy1 = CreateTestDairyProduct(name: "Milk", price: 1.5m, manufacturer: "Farm", expirationDate: expDate, fat: 3.5, vol: 1.0);
            var dairy2 = CreateTestDairyProduct(name: "Milk", price: 1.5m, manufacturer: "Farm", expirationDate: expDate, fat: 3.5, vol: 1.0);
            Assert.IsTrue(dairy1.Equals(dairy2));
        }

        [TestMethod]
        public void DairyProduct_Equals_DifferentFatContent_ReturnsFalse()
        {
            DateTime expDate = DateTime.Today.AddDays(3);
            var dairy1 = CreateTestDairyProduct(expirationDate: expDate, fat: 3.5);
            var dairy2 = CreateTestDairyProduct(expirationDate: expDate, fat: 2.5);
            Assert.IsFalse(dairy1.Equals(dairy2));
        }

        [TestMethod]
        public void DairyProduct_Equals_DifferentVolume_ReturnsFalse()
        {
            DateTime expDate = DateTime.Today.AddDays(3);
            var dairy1 = CreateTestDairyProduct(expirationDate: expDate, vol: 1.0);
            var dairy2 = CreateTestDairyProduct(expirationDate: expDate, vol: 1.1);
            Assert.IsFalse(dairy1.Equals(dairy2));
        }

        [TestMethod]
        public void DairyProduct_Equals_DifferentBaseProperties_ReturnsFalse()
        {
            var dairy1 = CreateTestDairyProduct(name: "DairyA");
            var dairy2 = CreateTestDairyProduct(name: "DairyB"); // Different name
            Assert.IsFalse(dairy1.Equals(dairy2));
        }

        [TestMethod]
        public void DairyProduct_Equals_NearToleranceFatContent()
        {
            var baseDairy = CreateTestDairyProduct(fat: 1.000);
            var dairyEq = CreateTestDairyProduct(fat: 1.0005);
            var dairyNeq = CreateTestDairyProduct(fat: 1.0015);

            Assert.IsTrue(baseDairy.Equals(dairyEq), "Dairy products with fat content differing by less than tolerance should be equal.");
            Assert.IsFalse(baseDairy.Equals(dairyNeq), "Dairy products with fat content differing by more than tolerance should not be equal.");
        }

        [TestMethod]
        public void DairyProduct_Equals_NearToleranceVolume()
        {
            var baseDairy = CreateTestDairyProduct(vol: 1.0000);
            var dairyEq = CreateTestDairyProduct(vol: 1.0005);
            var dairyNeq = CreateTestDairyProduct(vol: 1.0015);

            Assert.IsTrue(baseDairy.Equals(dairyEq), "Dairy products with volume differing by less than tolerance should be equal.");
            Assert.IsFalse(baseDairy.Equals(dairyNeq), "Dairy products with volume differing by more than tolerance should not be equal.");
        }

        [TestMethod]
        public void DairyProduct_GetHashCode_EqualObjectsHaveEqualHashCodes()
        {
            DateTime expDate = DateTime.Today.AddDays(3);
            var dairy1 = CreateTestDairyProduct(name: "Milk", price: 1.5m, manufacturer: "Farm", expirationDate: expDate, fat: 3.5, vol: 1.0);
            var dairy2 = CreateTestDairyProduct(name: "Milk", price: 1.5m, manufacturer: "Farm", expirationDate: expDate, fat: 3.5, vol: 1.0);
            Assert.AreEqual(dairy1.GetHashCode(), dairy2.GetHashCode());
        }




        [TestMethod]
        public void DairyProduct_Clone_CreatesIndependentCopy()
        {
            var original = CreateTestDairyProduct("Kefir", 1.2m, "DairyLand", DateTime.Today.AddDays(5), 1.0, 0.9);
            var clone = original.Clone() as DairyProduct;

            Assert.IsNotNull(clone);
            Assert.AreNotSame(original, clone);
            Assert.IsTrue(original.Equals(clone));

            clone.FatContent = 2.5;
            Assert.AreEqual(1.0, original.FatContent, 0.001);
        }

        [TestMethod]
        public void DairyProduct_ShallowCopy_IsMemberwiseClone()
        {
            var original = CreateTestDairyProduct("DairySC", 10m, "ManSC", DateTime.Today.AddDays(1), 1.2, 0.3);
            var copy = original.ShallowCopy() as DairyProduct;

            Assert.IsNotNull(copy);
            Assert.AreNotSame(original, copy);
            Assert.AreEqual(original.Name, copy.Name);
            Assert.AreEqual(original.Price, copy.Price);
            Assert.AreEqual(original.ExpirationDate, copy.ExpirationDate);
            Assert.AreEqual(original.FatContent, copy.FatContent, 0.001);
            Assert.AreEqual(original.Volume, copy.Volume, 0.001);
        }

        [TestMethod]
        public void DairyProduct_RandomInit_SetsPropertiesWithinExpectedRanges()
        {
            var dairy = new DairyProduct();
            dairy.RandomInit();

            string[] dairyNames = { "Молоко", "Кефир", "Сметана", "Творог", "Ряженка" };
            string[] dairyManufacturers = { "Простоквашино", "Домик в деревне", "Веселый Молочник", "Савушкин Продукт" };

            Assert.IsTrue(dairyNames.Contains(dairy.Name), $"Generated dairy name '{dairy.Name}' is not in the expected list.");
            Assert.IsTrue(dairyManufacturers.Contains(dairy.Manufacturer), $"Generated dairy manufacturer '{dairy.Manufacturer}' is not in the expected list.");
            Assert.IsTrue(dairy.FatContent >= 0.0 && dairy.FatContent <= 10.0, $"FatContent {dairy.FatContent} out of range (0.0-10.0).");
            Assert.IsTrue(dairy.Volume >= 0.20 && dairy.Volume <= 1.70, $"Volume {dairy.Volume} out of range (0.20-1.70).");
            Assert.IsTrue(dairy.ExpirationDate.Date >= DateTime.Today.AddDays(1) && dairy.ExpirationDate.Date <= DateTime.Today.AddDays(90));
        }

        // --- Тесты для Toy ---
        [TestMethod]
        public void Toy_DefaultConstructor_InitializesCorrectly()
        {
            var toy = new Toy();
            Assert.AreEqual("Без имени", toy.Name);
            Assert.AreEqual(0, toy.AgeRestriction, "Default AgeRestriction should be 0.");
            Assert.AreEqual("Не указан", toy.Material, "Default Material is incorrect.");
        }

        [TestMethod]
        public void Toy_Constructor_InitializesAllPropertiesCorrectly()
        {
            string name = "Teddy Bear";
            decimal price = 25.00m;
            string manufacturer = "ToyJoy";
            int ageRestriction = 3;
            string material = "Plush";

            var toy = new Toy(name, price, manufacturer, ageRestriction, material);

            Assert.AreEqual(name, toy.Name);
            Assert.AreEqual(price, toy.Price);
            Assert.AreEqual(manufacturer, toy.Manufacturer);
            Assert.AreEqual(ageRestriction, toy.AgeRestriction);
            Assert.AreEqual(material, toy.Material);
        }

        [TestMethod]
        public void Toy_CopyConstructor_CopiesProperties()
        {
            var original = CreateTestToy("OriginalToy", 40m, "ManToy", 5, "Wood");
            var copy = new Toy(original);

            Assert.AreEqual(original.Name, copy.Name);
            Assert.AreEqual(original.Price, copy.Price);
            Assert.AreEqual(original.Manufacturer, copy.Manufacturer);
            Assert.AreEqual(original.AgeRestriction, copy.AgeRestriction);
            Assert.AreEqual(original.Material, copy.Material);
            Assert.AreNotSame(original, copy);
        }

        [TestMethod]
        public void Toy_Properties_SetValidEdgeValuesAndNulls()
        {
            var toy = CreateTestToy();
            toy.AgeRestriction = 0;
            Assert.AreEqual(0, toy.AgeRestriction);

            toy.Material = null; // Допустимо по сеттеру
            Assert.IsNull(toy.Material, "Material should be settable to null.");

            toy.Material = string.Empty;
            Assert.AreEqual(string.Empty, toy.Material, "Material should be settable to empty string.");
        }

        [TestMethod]
        public void Toy_Equals_SameProperties_ReturnsTrue()
        {
            var toy1 = CreateTestToy(name: "Car", price: 10m, manufacturer: "HotWheels", age: 3, material: "Metal");
            var toy2 = CreateTestToy(name: "Car", price: 10m, manufacturer: "HotWheels", age: 3, material: "Metal");
            Assert.IsTrue(toy1.Equals(toy2));
        }

        [TestMethod]
        public void Toy_Equals_DifferentMaterial_ReturnsFalse()
        {
            var toy1 = CreateTestToy(material: "Metal");
            var toy2 = CreateTestToy(material: "Plastic");
            Assert.IsFalse(toy1.Equals(toy2));
        }

        [TestMethod]
        public void Toy_Equals_DifferentAgeRestriction_ReturnsFalse()
        {
            var toy1 = CreateTestToy(age: 3);
            var toy2 = CreateTestToy(age: 4);
            Assert.IsFalse(toy1.Equals(toy2));
        }

        [TestMethod]
        public void Toy_Equals_DifferentBaseProperties_ReturnsFalse()
        {
            var toy1 = CreateTestToy(name: "ToyA");
            var toy2 = CreateTestToy(name: "ToyB"); // Different name
            Assert.IsFalse(toy1.Equals(toy2));
        }

        [TestMethod]
        public void Toy_Equals_MaterialCasingAndNulls()
        {
            var toy1 = CreateTestToy(material: "Plastic");
            var toy2 = CreateTestToy(material: "plastic");
            var toy3 = CreateTestToy(material: null);
            var toy4 = CreateTestToy(material: null);

            Assert.IsFalse(toy1.Equals(toy2), "Material comparison should be case-sensitive.");
            Assert.IsTrue(toy3.Equals(toy4), "Two toys with null material and same other props should be equal.");
            Assert.IsFalse(toy1.Equals(toy3), "Toy with non-null material and toy with null material should not be equal.");
        }

        [TestMethod]
        public void Toy_GetHashCode_EqualObjectsHaveEqualHashCodes()
        {
            var toy1 = CreateTestToy(name: "Car", price: 10m, manufacturer: "HotWheels", age: 3, material: "Metal");
            var toy2 = CreateTestToy(name: "Car", price: 10m, manufacturer: "HotWheels", age: 3, material: "Metal");
            Assert.AreEqual(toy1.GetHashCode(), toy2.GetHashCode());
        }

        [TestMethod]
        public void Toy_ToString_IncludesBaseAndToySpecifics()
        {
            var toy = CreateTestToy("Robot", 29.99m, "RoboCorp", 8, "Metal/Plastic");
            string expectedPrice = string.Format(CultureInfo.CurrentCulture, "{0:C}", 29.99m);
            string expectedString = $"Toy: Robot, Цена: {expectedPrice}, Произв.: RoboCorp, Возраст: 8+, Материал: Metal/Plastic";
            Assert.AreEqual(expectedString, toy.ToString());
        }



        [TestMethod]
        public void Toy_Clone_CreatesIndependentCopy()
        {
            var original = CreateTestToy("Doll", 15m, "Barbie", 5, "Plastic");
            var clone = original.Clone() as Toy;

            Assert.IsNotNull(clone);
            Assert.AreNotSame(original, clone);
            Assert.IsTrue(original.Equals(clone));

            clone.Material = "Vinyl";
            Assert.AreEqual("Plastic", original.Material);
        }

        [TestMethod]
        public void Toy_ShallowCopy_IsMemberwiseClone()
        {
            var original = CreateTestToy("ToySC", 10m, "ManSC", 1, "MatSC");
            var copy = original.ShallowCopy() as Toy;

            Assert.IsNotNull(copy);
            Assert.AreNotSame(original, copy);
            Assert.AreEqual(original.Name, copy.Name);
            Assert.AreEqual(original.Price, copy.Price);
            Assert.AreEqual(original.AgeRestriction, copy.AgeRestriction);
            Assert.AreEqual(original.Material, copy.Material);
        }

        [TestMethod]
        public void Toy_RandomInit_SetsPropertiesWithinExpectedRanges()
        {
            var toy = new Toy();
            toy.RandomInit();

            string[] toyNames = { "Машинка", "Кукла", "Конструктор", "Мяч", "Плюшевый мишка", "Пазл" };
            string[] toyManufacturers = { "Lego", "Hasbro", "Mattel", "Мир Детства", "Полесье" };
            string[] materials = { "Пластик", "Текстиль", "Дерево", "Резина", "Металл" };

            Assert.IsTrue(toyNames.Contains(toy.Name), $"Generated toy name '{toy.Name}' is not in the expected list.");
            Assert.IsTrue(toyManufacturers.Contains(toy.Manufacturer), $"Generated toy manufacturer '{toy.Manufacturer}' is not in the expected list.");
            Assert.IsTrue(materials.Contains(toy.Material), $"Generated material '{toy.Material}' is not in the expected list.");
            Assert.IsTrue(toy.Price >= 100m && toy.Price <= 2100m, $"Price {toy.Price} out of range (100-2100).");
            Assert.IsTrue(toy.AgeRestriction >= 0 && toy.AgeRestriction <= 12, $"AgeRestriction {toy.AgeRestriction} out of range (0-12).");
        }


        // Тесты для protected ReadInt, ReadDouble, ReadDateTime лучше делать через Init()
        // или сделать эти методы internal и использовать InternalsVisibleTo.
        // Здесь приводим примеры для Init().

        // --- Тесты для Init() методов с симуляцией ввода ---

        [TestMethod]
        public void Product_Init_SimulatedCorrectInput_SetsProperties()
        {
            var product = new Product();
            string name = "Тестовый Продукт Init";
            string price = "250,75"; // ru-RU
            string manufacturer = "Производитель Init";
            string expDateStr = DateTime.Today.AddDays(100).ToString("yyyy-MM-dd");
            string simulatedInput = $"{name}\n{price}\n{manufacturer}\n{expDateStr}\n";

            using (var sr = new StringReader(simulatedInput))
            {
                Console.SetIn(sr);
                product.Init();
            }

            Assert.AreEqual(name, product.Name);
            Assert.AreEqual(decimal.Parse(price, CultureInfo.CurrentCulture), product.Price);
            Assert.AreEqual(manufacturer, product.Manufacturer);
            Assert.AreEqual(DateTime.Parse(expDateStr).Date, product.ExpirationDate.Date);
        }



        [TestMethod]
        public void Toy_Init_SimulatedCorrectInput_SetsProperties()
        {
            var toy = new Toy();
            string name = "Конструктор Init";
            string price = "1200";
            string manufacturer = "Леголенд";
            string age = "6";
            string material = "Пластик ABS";
            string simulatedInput = $"{name}\n{price}\n{manufacturer}\n{age}\n{material}\n";
            using (var sr = new StringReader(simulatedInput))
            {
                Console.SetIn(sr);
                toy.Init();
            }

            Assert.AreEqual(name, toy.Name);
            Assert.AreEqual(decimal.Parse(price, CultureInfo.CurrentCulture), toy.Price);
            Assert.AreEqual(manufacturer, toy.Manufacturer);
            Assert.AreEqual(int.Parse(age), toy.AgeRestriction);
            Assert.AreEqual(material, toy.Material);
        }

    }
}