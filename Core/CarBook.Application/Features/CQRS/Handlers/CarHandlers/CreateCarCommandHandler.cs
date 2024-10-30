using CarBook.Application.Features.CQRS.Commands.CarCommands;
using CarBook.Application.Interfaces;
using CarBook.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarBook.Application.Features.CQRS.Handlers.CarHandlers
{
    public class CreateCarCommandHandler
    {
        private readonly IRepository<Car> _carRepository;
        private readonly IRepository<Feature> _featureRepository;
        private readonly IRepository<CarFeature> _carFeatureRepository;

        public CreateCarCommandHandler(IRepository<Car> carRepository, IRepository<Feature> featureRepository, IRepository<CarFeature> carFeatureRepository)
        {
            _carRepository = carRepository;
            _featureRepository = featureRepository;
            _carFeatureRepository = carFeatureRepository;
        }

        public async Task Handle(CreateCarCommand command)
        {
            // Yeni bir araba oluştur ve kaydet
            var car = new Car
            {
                BigImageUrl = command.BigImageUrl,
                Luggage = command.Luggage,
                Km = command.Km,
                Model = command.Model,
                Seat = command.Seat,
                Transmission = command.Transmission,
                CoverImageUrl = command.CoverImageUrl,
                BrandID = command.BrandID,
                Fuel = command.Fuel
            };

            await _carRepository.CreateAsync(car);

            // Tüm mevcut özellikleri getir ve yeni araba için CarFeature ekle
            var features = await _featureRepository.GetAllAsync();
            foreach (var feature in features)
            {
                var carFeature = new CarFeature
                {
                    CarID = car.CarID,
                    FeatureID = feature.FeatureID,
                    Available = false // Başlangıç değeri olarak false atıyoruz
                };

                await _carFeatureRepository.CreateAsync(carFeature);
            }
        }
    }
}
