using CarBook.Application.Features.Mediator.Commands.FeatureCommands;
using CarBook.Application.Interfaces;
using CarBook.Application.Interfaces.CarFeatureInterfaces;
using CarBook.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBook.Application.Features.Mediator.Handlers.FeatureHandlers
{
    public class CreateFeatureCommandHandler : IRequestHandler<CreateFeatureCommand>
    {
        private readonly IRepository<Feature> _repository;
        private readonly ICarFeatureRepository _carFeatureRepository;
        public CreateFeatureCommandHandler(IRepository<Feature> repository, ICarFeatureRepository carFeatureRepository)
        {
            _repository = repository;
            _carFeatureRepository = carFeatureRepository;
        }

        public async Task Handle(CreateFeatureCommand request, CancellationToken cancellationToken)
        {
            var feature = new Feature
            {
                Name = request.Name
            };

            await _repository.CreateAsync(feature);

            _carFeatureRepository.AddNewFeatureToAllCars(feature);
        }
    }
}
