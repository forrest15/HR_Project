﻿using Data.Infrastructure;
using Domain.DTO.DTOModels.SetupDTO;
using Domain.Entities.Setup;
using Domain.Repositories;

namespace WebApi.Controllers
{
    public class LocationsController : BoTController<Location, LocationDTO>
    {
        public LocationsController(IDataRepositoryFactory repoFatory, IErrorRepository errorRepo)
            : base(repoFatory, errorRepo)
        {
        }
    }
}