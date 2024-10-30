using CarBook.Application.Features.Mediator.Commands.FeatureCommands;
using CarBook.Application.Interfaces;
using CarBook.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CarBook.Application.Features.Mediator.Handlers.FeatureHandlers
{
    public class CreateFeatureCommandHandler : IRequestHandler<CreateFeatureCommand>
    {
        private readonly IRepository<Feature> _featureRepository;
        private readonly IRepository<Car> _carRepository;
        private readonly IRepository<CarFeature> _carFeatureRepository;

        public CreateFeatureCommandHandler(IRepository<Feature> featureRepository, IRepository<Car> carRepository, IRepository<CarFeature> carFeatureRepository)
        {
            _featureRepository = featureRepository;
            _carRepository = carRepository;
            _carFeatureRepository = carFeatureRepository;
        }

        public async Task Handle(CreateFeatureCommand request, CancellationToken cancellationToken)
        {
            var feature = new Feature
            {
                Name = request.Name
            };

            await _featureRepository.CreateAsync(feature);

            var cars = await _carRepository.GetAllAsync();
            foreach (var car in cars)
            {
                var carFeature = new CarFeature
                {
                    CarID = car.CarID,
                    FeatureID = feature.FeatureID,
                    Available = false
                };

                await _carFeatureRepository.CreateAsync(carFeature);
            }

        }
    }
}
