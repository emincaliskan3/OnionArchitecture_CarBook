using CarBook.Application.Interfaces.CarPricingInterfaces;
using CarBook.Application.ViewModels;
using CarBook.Domain.Entities;
using CarBook.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBook.Persistence.Repositories.CarPricingRepositories
{
    public class CarPricingRepository : ICarPricingRepository
    {
        private readonly CarBookContext _context;
        public CarPricingRepository(CarBookContext context)
        {
            _context = context;
        }
        public List<CarPricing> GetCarPricingWithCars()
        {
            var values = _context.CarPricings.Include(x => x.Car).ThenInclude(y => y.Brand).Include(x => x.Pricing).Where(z => z.PricingID == 2).ToList();
            return values;
        }

        public List<CarPricingViewModel> GetCarPricingWithTimePeriod()
        {
            // Sorgu, CarPricings, Cars ve Brands tablolarını birleştiriyor
            var query = from carPricing in _context.CarPricings
                        join car in _context.Cars on carPricing.CarID equals car.CarID
                        join brand in _context.Brands on car.BrandID equals brand.BrandID
                        select new
                        {
                            car.Model,
                            car.CoverImageUrl,
                            BrandName = brand.Name,
                            carPricing.PricingID,
                            carPricing.Amount
                        };

            // Sorguyu çalıştırarak veriyi çekiyoruz
            var data = query.ToList();

            // Model adına göre gruplama ve pivot işlemi yapılıyor
            var pivotData = data
                .GroupBy(d => new { d.Model, d.BrandName, d.CoverImageUrl }) // Model, marka adı ve kapak resmi URL'sine göre gruplama
                .Select(g => new CarPricingViewModel
                {
                    Model = g.Key.Model, // Model adı
                    Brand = g.Key.BrandName, // Marka adı
                    CoverImageUrl = g.Key.CoverImageUrl, // Kapak resmi URL'si
                    Amounts = g.OrderBy(x => x.PricingID) // PricingID'ye göre sıralama
                               .Select(x => x.Amount) // Amount değerlerini seçme
                               .ToList() // Listeye dönüştürme
                })
                .ToList();

            return pivotData; // İşlenmiş veriyi döndürme
        }


    }

}





//public List<CarPricingViewModel> GetCarPricingWithTimePeriod()
//{
//    // Bir liste oluşturuluyor, her bir araç ve fiyat bilgisi bu listede saklanacak
//    List<CarPricingViewModel> values = new List<CarPricingViewModel>();

//    // Veritabanı bağlantısından komut oluşturuluyor
//    using (var command = _context.Database.GetDbConnection().CreateCommand())
//    {
//        // SQL sorgusu tanımlanıyor
//        command.CommandText = "Select * From (Select Model,Name,CoverImageUrl,PricingID,Amount From CarPricings " +
//                              "Inner Join Cars On Cars.CarID=CarPricings.CarId " +
//                              "Inner Join Brands On Brands.BrandID=Cars.BrandID) As SourceTable " +
//                              "Pivot (Sum(Amount) For PricingID In ([2],[3],[4])) as PivotTable;";
//        // Komut tipi SQL metni olarak ayarlanıyor
//        command.CommandType = System.Data.CommandType.Text;

//        // Veritabanı bağlantısı açılıyor
//        _context.Database.OpenConnection();

//        // Sorgu çalıştırılıyor ve veri okuyucu (reader) ile sonuçlar çekiliyor
//        using (var reader = command.ExecuteReader())
//        {
//            // Okunacak veri olduğu sürece döngüye giriyor
//            while (reader.Read())
//            {
//                // Her bir satır için yeni bir CarPricingViewModel nesnesi oluşturuluyor
//                CarPricingViewModel carPricingViewModel = new CarPricingViewModel()
//                {
//                    // Marka adı alınıyor
//                    Brand = reader["Name"].ToString(),
//                    Model = reader["Model"].ToString(),
//                    CoverImageUrl = reader["CoverImageUrl"].ToString(),
//                    // Fiyatlar listesi oluşturuluyor ve her PricingID’ye göre fiyat ekleniyor
//                    Amounts = new List<decimal>
//                    {
//                        Convert.ToDecimal(reader["2"]), // PricingID = 2 için fiyat
//                        Convert.ToDecimal(reader["3"]), // PricingID = 3 için fiyat
//                        Convert.ToDecimal(reader["4"])  // PricingID = 4 için fiyat
//                    }
//                };
//                // Model nesnesi listeye ekleniyor
//                values.Add(carPricingViewModel);
//            }
//        }
//        // Veritabanı bağlantısı kapatılıyor
//        _context.Database.CloseConnection();
//        return values;
//    }
//}