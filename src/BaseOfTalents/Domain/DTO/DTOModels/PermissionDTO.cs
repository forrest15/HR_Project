﻿using Domain.Entities.Enum;
using System.Collections.Generic;

namespace Domain.DTO.DTOModels
{
    public class PermissionDTO : BaseEntityDTO
    {
        public string Description { get; set; }
        public AccessRights AccessRights { get; set; }

        public IEnumerable<int> RoleIds { get; set; }
    }
}
